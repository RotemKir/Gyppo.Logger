namespace Gyppo.Logger.Tests

open System.IO
open Gyppo.Logger

module SyntaxTests =

    open System
    open Xunit

    let getTestFilePath file = Path.Join("Files", file) 

    let processFile file =
        file
        |> getTestFilePath
        |> Syntax.processSourceFile

    let testFileNotChanged file =
        match processFile file with
        | None -> ignore()
        | Some _ -> failwith "Process shouldn't change source code"
    
    let testFileChanged file expectedContent =
        match processFile file with
        | None -> failwith "Process should change source code"
        | Some sourceFile -> 
            Assert.Equal(getTestFilePath file, sourceFile.Path)
            Assert.Equal(expectedContent, sourceFile.Content)

    [<Fact>]
    let ``Class with no logger, doesn't change the source code`` () =
        testFileNotChanged "ClassWithNoLogger.txt"
        
    [<Fact>]
    let ``Class with logger, no calls to logger, doesn't change the source code`` () =
        testFileNotChanged "ClassWithLoggerNoInvocation.txt"

    [<Fact>]
    let ``Log debug with message, adds prefix`` () =
        testFileChanged 
            "LogDebugMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogDebug("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log debug with exception, message, adds prefix`` () =
        testFileChanged 
            "LogDebugExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogDebug(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log debug with event id, message, adds prefix`` () =
        testFileChanged 
            "LogDebugEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogDebug(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log debug with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogDebugEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogDebug(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log information with message, adds prefix`` () =
        testFileChanged 
            "LogInformationMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogInformation("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log information with exception, message, adds prefix`` () =
        testFileChanged 
            "LogInformationExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogInformation(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log information with event id, message, adds prefix`` () =
        testFileChanged 
            "LogInformationEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogInformation(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log information with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogInformationEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogInformation(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log warning with message, adds prefix`` () =
        testFileChanged 
            "LogWarningMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogWarning("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
 
    [<Fact>]
    let ``Log warning with exception, message, adds prefix`` () =
        testFileChanged 
            "LogWarningExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogWarning(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log warning with event id, message, adds prefix`` () =
        testFileChanged 
            "LogWarningEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogWarning(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log warning with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogWarningEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogWarning(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log error with message, adds prefix`` () =
        testFileChanged 
            "LogErrorMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogError("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log error with exception, message, adds prefix`` () =
        testFileChanged 
            "LogErrorExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogError(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log error with event id, message, adds prefix`` () =
        testFileChanged 
            "LogErrorEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogError(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log error with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogErrorEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogError(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log critical with message, adds prefix`` () =
        testFileChanged 
            "LogCriticalMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogCritical("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log critical with exception, message, adds prefix`` () =
        testFileChanged 
            "LogCriticalExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogCritical(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log critical with event id, message, adds prefix`` () =
        testFileChanged 
            "LogCriticalEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogCritical(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log critical with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogCriticalEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogCritical(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log trace with message, adds prefix`` () =
        testFileChanged 
            "LogTraceMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogTrace("Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log trace with exception, message, adds prefix`` () =
        testFileChanged 
            "LogTraceExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogTrace(new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""

    [<Fact>]
    let ``Log trace with event id, message, adds prefix`` () =
        testFileChanged 
            "LogTraceEventIdMessage.txt" 
            """using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogTrace(new EventId(), "Class: 'MyClass', Method: 'Work' (line 13) - "+"message");
    }
}"""
    
    [<Fact>]
    let ``Log trace with event id, exception, message, adds prefix`` () =
        testFileChanged 
            "LogTraceEventIdExceptionMessage.txt" 
            """using Microsoft.Extensions.Logging;
using System;

public class MyClass
{
    private readonly ILogger logger;
    
    public MyClass(ILogger logger)
    {
        this.logger = logger;
    }
    
    public void Work()
    {
        logger.LogTrace(new EventId(), new Exception(), "Class: 'MyClass', Method: 'Work' (line 14) - "+"message");
    }
}"""       