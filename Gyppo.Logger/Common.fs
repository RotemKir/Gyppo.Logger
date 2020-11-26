namespace Gyppo.Logger

open System
open System.IO

module Common =
    
    type SourceFile =
        {
            Path : string
            Content : string
        }

    type LogLevel =
        | Debug
        | Information
        | Warning
        | Error
    
    let log (level : LogLevel) text = printfn "[%A] %s" level text
    
    let logz<'a> level text (value : 'a) = 
        log level text
        value

    let rec getSourceFiles directory =
        if String.IsNullOrWhiteSpace(directory) then
            getSourceFiles <| Directory.GetCurrentDirectory()
        else
            log LogLevel.Information <| sprintf "Get *.cs files from '%s'" directory
            Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)

    let writeSourceFile sourceFile =
        log LogLevel.Information <| sprintf "Updating source file '%s'" sourceFile.Path
        File.WriteAllText(sourceFile.Path, sourceFile.Content)

    let (>>=) option binder = Option.bind binder option