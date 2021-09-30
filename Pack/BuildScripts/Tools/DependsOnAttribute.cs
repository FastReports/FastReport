using System;
using System.Reflection;

namespace CakeScript
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class DependsOnAttribute : Attribute
    {
        public string DependsTask { get; }

        public DependsOnAttribute(string dependsTask)
        {
            DependsTask = dependsTask;
        }

        //public DependsOnAttribute(string dependsTask, bool condition)
        //{

        //}

    }
}