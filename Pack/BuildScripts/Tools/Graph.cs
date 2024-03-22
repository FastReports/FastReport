using System.Reflection;

namespace CakeScript;

internal class Graph
{
    internal sealed class GraphTask : IEquatable<GraphTask>
    {
        public string Name { get; }

        public MethodInfo Method { get; }

        public ICollection<GraphTask> Parents { get; } = new List<GraphTask>();

        public GraphTask(string name, MethodInfo method, IEnumerable<GraphTask> parents)
        {
            Name = name;
            Method = method;
            foreach (var parent in parents)
                Parents.Add(parent);
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphTask graphTask)
                return Equals(graphTask);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(GraphTask other)
        {
            if (other is null)
                return false;
            return Name.Equals(other.Name);
        }
    }

    public static Graph CreateTree(MethodInfo startMethod)
    {
        ArgumentNullException.ThrowIfNull(startMethod, nameof(startMethod));

        var startTask = GetTask(startMethod);

        return new Graph(startTask);
    }

    private static GraphTask GetTask(MethodInfo method)
    {
        var dependencies = GetDependencies(method);
        var graphTaskList = new List<GraphTask>();
        foreach(var dependencyMethod in dependencies)
        {
            graphTaskList.Add(GetTask(dependencyMethod));
        }
        return new GraphTask(method.Name, method, graphTaskList);
    }

    private static IEnumerable<MethodInfo> GetDependencies(MethodInfo method)
    {
        var attributes = method.GetCustomAttributes<DependsOnAttribute>();
        foreach(var attribute in attributes)
        {
            var dependsTask = attribute.DependsTask;
            yield return Startup.GetMethod(dependsTask);

        }
    }

    public IEnumerable<GraphTask> Run()
    {
        if(sortedTasks == null)
        {
            SpreadRays(StartTask, 0);

            sortedTasks = tasks
                .OrderByDescending(task => task.Item2)
                .Select(task => task.Item1);
        }
        return sortedTasks;
    }

    
    readonly List<(GraphTask, int)> tasks = new List<(GraphTask, int)>();
    private IEnumerable<GraphTask> sortedTasks;

    private readonly GraphTask StartTask;

    void SpreadRays(GraphTask task, int depth)
    {
        tasks.Add((task, depth));

        foreach (var parent in task.Parents)
        {
            // check if this parent exist => we try rewrite
            var existItem = tasks.Find(taskWithDepth => taskWithDepth.Item1.Equals(parent));
            if (existItem != (null, default))
            {
                if (existItem.Item2 < depth + 1)
                    tasks.Remove(existItem); // remove
                else // they are equals or greater => do nothing
                    continue;
            }

            SpreadRays(parent, depth + 1);
        }
    }

    private Graph(GraphTask startTask)
    {
        StartTask = startTask;
    }
}
