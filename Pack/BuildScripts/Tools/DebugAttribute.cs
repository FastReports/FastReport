
namespace CakeScript;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class DebugAttribute : Attribute
{
    public string Args { get; init; }

    public DebugAttribute()
    {
    }

    public DebugAttribute(string args)
    {
        Args = args;
    }
}