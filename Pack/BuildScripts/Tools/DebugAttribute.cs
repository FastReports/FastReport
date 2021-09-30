using System;
using System.Reflection;

namespace CakeScript
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class DebugAttribute : Attribute
    {
        public string Args { get; set; }

        public DebugAttribute()
        {
        }

        public DebugAttribute(string args)
        {
            Args = args;
        }

    }
}