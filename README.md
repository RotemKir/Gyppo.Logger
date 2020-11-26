# Gyppo.Logger
A dotnet tool to add metadata on ILogger calls using code analysis (Roslyn) in C# code.

*from wikipedia:*
>*A gyppo or gypo logger is a logger who runs or works for a small-scale logging operation that is independent from an established sawmill or lumber company.*
>
Gyppo adds a prefix to log messages that contain
 the names of the class name and method that logged the message, as well as the line in the source file (approximated).

It turns this:
```csharp
public class MyClass
{
    private readonly ILogger logger;
...
    public void DoStuff()
    {
        logger.LogInformation("This is a lovely log message");
    }
}
```

To this:
```csharp
public class MyClass
{
    private readonly ILogger logger;
...
    public void DoStuff()
    {
        logger.LogInformation("Class: 'MyClass', Method: 'DoStuff' (line 5) - "+"This is a lovely log message");
    }
}
```

## Installation
Run the following command:
```
dotnet tool install --global Gyppo.Logger
```

## Usage
You can simply run the following command:
```
gyppo
```
And the tool will process all the *.cs files in the current directory and sub-directories.

If you want to run it on a specific directory and its contents just pass a path parameter:
```
gyppo c:\repos\mycode\justprocessthisandsubdirectories
```

The intended usage is running this in your build pipeline, after getting the source and before building it.
