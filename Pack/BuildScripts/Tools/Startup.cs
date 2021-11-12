using Cake.Common.Tools.NuGet.Pack;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CakeScript
{
    static class Startup
    {
        static readonly Dictionary<string, string> Args = new Dictionary<string, string>();

        static public void Main(string[] args)
        {
            ParseArgs(args);

            string target = string.Empty;
            if (Args.ContainsKey("target"))
            {
                target = Args["target"];
            }
#if DEBUG
            if(string.IsNullOrEmpty(target))
            {
                var method = GetDebugTask();
                if (method != null)
                {
                    var debugArgs = GetDebugArgs(method);
                    ParseArgs(debugArgs);
                    var graph = Graph.CreateTree(method);

                    RunTree(graph, args);
                    return;
                }
            }
#endif

            if (!string.IsNullOrEmpty(target))
            {
                var graph = Graph.CreateTree(target);

                RunTree(graph, args);
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
                if (argument == "--tree")
                {
                    ViewAllTasks();
                    return;
                }

                var splitted = argument.Split('=', 2);

                var arg = splitted[0].Remove(0, 2);
                var value = splitted[1].Trim('"');
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

        private static void RunTree(Graph graph, string[] args)
        {
            var sortedTasks = graph.Run();
            var prog = new Program(args);

            foreach (var sortedTask in sortedTasks)
            {
                RunTaskWrapper(sortedTask, prog);
            }
        }

        private static void RunTaskWrapper(Graph.GraphTask sortedTask, object instance)
        {
            Console.WriteLine("=============================");
            Console.WriteLine($"  {sortedTask.Name}");

            sortedTask.Method.Invoke(instance, Array.Empty<object>());

            Information($"  {sortedTask.Name} finished", ConsoleColor.Green);
            Console.WriteLine("=============================");
            Console.WriteLine();
        }


        public static void Information(object info)
        {
            if (info is NuSpecDependency dep)
            {
                Console.WriteLine($"ID: {dep.Id}, VERSION: {dep.Version}, TFM: {dep.TargetFramework}");
            }
            else
                Console.WriteLine(info.ToString());
        }

        public static void Information(object info, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Information(info);
            Console.ResetColor();
        }


        public static bool HasArgument(string argument)
        {
            if (Args.ContainsKey(argument))
                return true;

            return false;
        }

        public static string Argument(string argument)
        {
            if (Args.ContainsKey(argument))
                return Args[argument];

            throw new Exception($"Argument '{argument}' not found");
        }

        public static string Argument(string argument, string defaultValue)
        {
            if (Args.ContainsKey(argument))
                return Args[argument];
            else
                return defaultValue;
        }
    }
}
