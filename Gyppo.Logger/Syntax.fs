namespace Gyppo.Logger

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.Extensions.Logging
open Gyppo.Logger.Common
open System.IO

module Syntax =

    type private InvocationExpressionContext =
        {
            Invocation : InvocationExpressionSyntax
            SemanticModel : SemanticModel
        }

    let private getInvocationFromContext invocationExpressionContext =
        invocationExpressionContext.Invocation

    let private loggerExtensionsTypeName = typeof<LoggerExtensions>.FullName

    let private getInvocationContainingType (symbolInfo : SymbolInfo) =     
        match symbolInfo with
        | x when x.Symbol <> null -> 
            Some symbolInfo.Symbol.ContainingType
        | x when x.CandidateReason = CandidateReason.OverloadResolutionFailure -> 
            Seq.map (fun (x : ISymbol) -> Some x.ContainingType) symbolInfo.CandidateSymbols
            |> Seq.head
        | _ -> None

    let private getInvocationSymbolInfo invocationExpressionContext =
        invocationExpressionContext.SemanticModel.GetSymbolInfo(invocationExpressionContext.Invocation)    
    
    let private isLoggerExtensionsType (typeSymbol : INamedTypeSymbol option) =
        match typeSymbol with
        | Some x when x.ToString() = loggerExtensionsTypeName -> true
        | _ -> false

    let private isSpecialType specialType (typeInfo : TypeInfo) =
        typeInfo.Type.SpecialType = specialType
        
    let private isStringArgument invocationExpressionContext (argument : ArgumentSyntax) =
        invocationExpressionContext.SemanticModel.GetTypeInfo(argument.Expression)
        |> isSpecialType SpecialType.System_String

    let private getFirstStringArgument invocationExpressionContext =
        invocationExpressionContext.Invocation.ArgumentList.Arguments
        |> Seq.find (isStringArgument invocationExpressionContext)

    let private getAncestor<'a when 'a :> SyntaxNode> (expression : SyntaxNode) =
        expression.Ancestors()
        |> Seq.pick (fun n -> 
            match n with
            | :? 'a as ancestor -> Some ancestor
            | _ -> None)

    let private createLogMessagePrefix invocationExpressionContext =
        let methodDeclaration = getAncestor<MethodDeclarationSyntax> invocationExpressionContext.Invocation
        let classDeclaration = getAncestor<ClassDeclarationSyntax> methodDeclaration
        let location = invocationExpressionContext.Invocation.GetLocation()
        sprintf "Class: '%s', Method: '%s' (line %i) - " 
            <| classDeclaration.Identifier.ValueText
            <| methodDeclaration.Identifier.ValueText
            <| location.GetLineSpan().StartLinePosition.Line

    let private addPrefixToMessageArgument invocationExpressionContext =
        let messageArgument = getFirstStringArgument invocationExpressionContext
        let prefix = createLogMessagePrefix invocationExpressionContext
        let prefixLiteral = SyntaxFactory.Literal(prefix)
        let prefixLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, prefixLiteral)
        let updatedMessageExpression = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, prefixLiteralExpression, messageArgument.Expression)
        let updatedMessageArgument = messageArgument.WithExpression(updatedMessageExpression);
        let updatedInvocation = invocationExpressionContext.Invocation.ReplaceNode(messageArgument, updatedMessageArgument)
        { invocationExpressionContext with Invocation = updatedInvocation }       

    let private isLoggerInvocationExpression =
        getInvocationSymbolInfo
        >> getInvocationContainingType
        >> isLoggerExtensionsType

    let private addPrefixToLogInvocations invocationExpressionContext =
        match isLoggerInvocationExpression invocationExpressionContext with
        | true -> addPrefixToMessageArgument invocationExpressionContext
        | false -> invocationExpressionContext 

    type LogPrefixSyntaxRewriter(semanticModel : SemanticModel) =
        class
        
        inherit CSharpSyntaxRewriter(false)
        
        let _semanticModel = semanticModel

        override this.VisitInvocationExpression(node) =
            { Invocation = node ; SemanticModel = _semanticModel }
            |> addPrefixToLogInvocations 
            |> getInvocationFromContext
            :> SyntaxNode
        
        end

    let private loggerType = typeof<ILogger>
    let private stringType = typeof<string>

    let private createSemanticModel (syntaxTree : SyntaxTree) =
        let loggerMetadataReference = MetadataReference.CreateFromFile(loggerType.Assembly.Location)
        let stringMetadataReference = MetadataReference.CreateFromFile(stringType.Assembly.Location)
        CSharpCompilation.Create("AssemblyName")
        |> fun c -> c.AddReferences(loggerMetadataReference)
        |> fun c -> c.AddReferences(stringMetadataReference)
        |> fun c -> c.AddSyntaxTrees(syntaxTree)
        |> fun c -> c.GetSemanticModel(syntaxTree, false)

    let private parseSyntaxTree path =
        let fileText = File.ReadAllText path
        CSharpSyntaxTree.ParseText(fileText)

    let private processSyntaxTree (syntaxTree : SyntaxTree) =
        let syntaxTreeRoot = syntaxTree.GetRoot()
        let rewriter = LogPrefixSyntaxRewriter <| createSemanticModel syntaxTree
        let rewrittenSyntaxNode = rewriter.Visit(syntaxTreeRoot)
        match rewrittenSyntaxNode = syntaxTreeRoot with
        | true -> None
        | false -> Some rewrittenSyntaxNode

    let private syntaxNodeToString path (syntaxNode : SyntaxNode) =
        Some { Path = path ; Content = syntaxNode.ToFullString() }

    let processSourceFile path =
        try 
            path 
            |> (logz LogLevel.Information <| sprintf "Process source file '%s'" path)
            |> parseSyntaxTree 
            |> processSyntaxTree
            >>= syntaxNodeToString path
        with
        | ex ->
            log LogLevel.Error <| sprintf "Error processing source file '%s': %s" path ex.Message
            None