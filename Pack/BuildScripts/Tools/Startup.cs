using System.Reflection;
using System.Diagnostics;
using Cake.Core;

namespace CakeScript;

static class Startup
{
    static readonly Dictionary<string, string> Args = new Dictionary<string, string>();

    private static MethodInfo targetMethod;


    public static bool IsTargetMethod 
    {
        get
        {
            var stackTrace = new StackTrace(false);
            var frame = stackTrace.GetFrame(1);
            var callMethod = frame.GetMethod();

            bool result = callMethod == targetMethod;
            return result;
        }
    }

    public static async Task Main(string[] args)
    {
        ParseArgs(args);

        var target = string.Empty;
        if (Args.TryGetValue("target", out string value))
        {
            target = value;
        }
#if DEBUG
        if(string.IsNullOrEmpty(target))
        {
            targetMethod = GetDebugTask();
            if (targetMethod != null)
            {
                var debugArgs = GetDebugArgs(targetMethod);
                ParseArgs(debugArgs);
                var graph = Graph.CreateTree(targetMethod);

                await RunTree(graph, args);
                return;
            }
        }
#endif

        if (!string.IsNullOrEmpty(target))
        {
            targetMethod = GetMethod(target);

            var graph = Graph.CreateTree(targetMethod);

            await RunTree(graph, args);
        }
        else
            Program.Default();

    }

    private static string[] GetDebugArgs(MethodInfo method)
    {
        var args = method.GetCustomAttribute<DebugAttribute>().Args;
        if (string.IsNullOrEmpty(args))
            return Array.Empty<string>();

        //TODO: whitespace in path, for example "path="D:/Program Files""
        return args.Split(' ');
    }

    private static void ParseArgs(string[] args)
    {
        foreach (var argument in args)
        {
            if (string.IsNullOrEmpty(argument) || !argument.StartsWith("--"))
                continue;

            if (argument == "--tree")
            {
                ViewAllTasks();
                return;
            }

            var splitted = argument.Split('=', 2);

            var arg = splitted[0].Remove(0, 2);
            var value = splitted[1]?.Trim('"');
            Args.Add(arg, value);
        }
    }

    private static void ViewAllTasks()
    {
        var allTasks = typeof(Program)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(methodInfo => methodInfo.DeclaringType == typeof(Program))
            .OrderBy(methodInfo => methodInfo.Name[0]);

        foreach(var task in allTasks)
        {
            Information(task.Name);
        }
    }

    private static MethodInfo GetDebugTask()
    {
        var debugTask = typeof(Program)
            .GetMethods()
            .Where(method => method.GetCustomAttribute<DebugAttribute>() != null)
            .FirstOrDefault();

        return debugTask;
    }

    public static MethodInfo GetMethod(string methodName)
    {
        var methods = typeof(Program).GetMethods();
        foreach (var method in methods)
        {
            if (method.Name.Equals(methodName))
            {
                return method;
            }
        }

        throw new Exception($"Method with {methodName} name wasn't found!");
    }

    private static async Task RunTree(Graph graph, string[] args)
    {
        var sortedTasks = graph.Run();
        var prog = new Program(args);

        foreach (var sortedTask in sortedTasks)
        {
            await RunTaskWrapper(sortedTask, prog);
        }
    }

    private static async Task RunTaskWrapper(Graph.GraphTask sortedTask, object instance)
    {
        const string separator = "=============================";
        Console.WriteLine(separator);
        Console.WriteLine($"  {sortedTask.Name}");

        try
        {
            object result = sortedTask.Method.Invoke(instance, Array.Empty<object>());

            if (result is Task taskResult)
            {
                await taskResult.ConfigureAwait(false);
            }
        }
        catch (CakeException e)
        {
            Environment.Exit(e.ExitCode);
        }

        Information($"  {sortedTask.Name} finished", ConsoleColor.Green);
        Console.WriteLine(separator);
        Console.WriteLine();
    }


    public static void Information(object info)
    {
        if (info is NuSpecDependency dep)
        {
            Console.WriteLine($"ID: {dep.Id}, VERSION: {dep.Version}, TFM: {dep.TargetFramework}");
        }
        else
            Information(info.ToString());
    }

    public static void Information(string info)
    {
        Console.WriteLine(info);
    }

    public static void Information(object info, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Information(info);
        Console.ResetColor();
    }

    public static void Information(string info, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Information(info);
        Console.ResetColor();
    }

    public static void Warning(object info)
        => Information(info, ConsoleColor.Yellow);

    public static void Warning(string info)
        => Information(info, ConsoleColor.Yellow);

    public static void Error(string info)
        => Information(info, ConsoleColor.Red);

    public static bool HasArgument(string argument)
    {
        if (Args.ContainsKey(argument))
            return true;

        return false;
    }

    public static string Argument(string argument)
    {
        if (Args.TryGetValue(argument, out string value))
            return value;

        throw new Exception($"Argument '{argument}' not found");
    }

    public static string Argument(string argument, string defaultValue)
    {
        if (Args.TryGetValue(argument, out string value))
            return value;
        else
            return defaultValue;
    }

    public static T Argument<T>(string argument, T defaultValue)
    {
        if (Args.TryGetValue(argument, out string value))
        {
            var type = typeof(T);
            // special case for bool
            if (defaultValue is bool && string.IsNullOrEmpty(value))
                return (T)(object)true;

            return (T)Convert.ChangeType(value, type);
        }
        else
            return defaultValue;
    }
}
