using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FastReport.Web.Services.Helpers
{
    internal class IntelliSenseHelper
    {
        private static readonly Regex GenericTypeExtractorRegex = new(@"`\d+");

        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public |
                                                  System.Reflection.BindingFlags.NonPublic |
                                                  System.Reflection.BindingFlags.Instance |
                                                  System.Reflection.BindingFlags.Static;

        private static readonly ConcurrentDictionary<string, List<TypeInfo>> CachedNamespaceInfo = new();
        private static readonly ConcurrentDictionary<string, ClassDetails> CachedClassDetails = new();

        private readonly List<string> _userIntelliSenseAssemblies;
        private static readonly List<string> IntelliSenseAssemblies = new()
        {
            "FastReport.SkiaDrawing", "FastReport.DataVisualization", "FastReport.Compat", "FastReport", "System.Private.CoreLib"
        };

        public IntelliSenseHelper(List<string> intelliSenseAssemblies)
        {
            _userIntelliSenseAssemblies = intelliSenseAssemblies;
        }

        public ClassDetails GetClassDetails(string className)
        {
            return CachedClassDetails.GetOrAdd(className, _ => FetchClassDetails(className));
        }

        public Dictionary<string, List<TypeInfo>> GetNamespacesInfo(IReadOnlyCollection<string> namespaces)
        {
            var result = new Dictionary<string, List<TypeInfo>>();

            foreach (var ns in namespaces)
                result[ns] = CachedNamespaceInfo.GetOrAdd(ns, _ => GetNamespaceInfo(ns));

            return result;
        }

        private ClassDetails FetchClassDetails(string className)
        {
            var type = FindTypeByName(className);
            if (type == null) return null;

            var methods = GetMethodDetails(type);
            var fields = GetFieldDetails(type);
            var properties = GetPropertyDetails(type);

            return new ClassDetails
            {
                ClassName = type.FullName,
                Methods = methods,
                Fields = fields,
                Properties = properties
            };
        }

        private static IEnumerable<PropertyDetails> GetPropertyDetails(IReflect type)
        {
            return type.GetProperties(BindingFlags)
                .Where(p => p.CanRead || p.CanWrite)
                .Select(p => new PropertyDetails
                {
                    Label = p.Name,
                    Type = p.CanWrite ? "variable" : "constant",
                    PropertyType = p.PropertyType.FullName,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite
                }).ToList();
        }

        private static IEnumerable<FieldDetails> GetFieldDetails(IReflect type)
        {
            return type.GetFields(BindingFlags)
                .Where(f => f.IsPublic)
                .Select(f => new FieldDetails
                {
                    Label = f.Name,
                    Type = "variable",
                    FieldType = f.FieldType.FullName, 
                    IsPublic = f.IsPublic,
                    IsStatic = f.IsStatic
                }).ToList();
        }

        private static IEnumerable<MethodDetails> GetMethodDetails(IReflect type)
        {
            return type.GetMethods(BindingFlags)
                .Where(m => !m.IsSpecialName && m.IsPublic)
                .Select(m => new MethodDetails
                {
                    Label = m.Name,
                    Type = "method",
                    ReturnType = m.ReturnType.FullName,
                    Parameters =
                        m.GetParameters().Select(p => new ParameterDetails { Name = p.Name, Type = p.ParameterType.FullName }).ToList(),
                    IsPublic = m.IsPublic,
                    IsStatic = m.IsStatic
                }).ToList();
        }

        private Type FindTypeByName(string typeName)
        {
            var assemblies = FilterAssembliesByNames(_userIntelliSenseAssemblies)
                .Concat(FilterAssembliesByNames(IntelliSenseAssemblies));

            var type = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == typeName);

            return type;
        }

        private static IEnumerable<Assembly> FilterAssembliesByNames(ICollection<string> assemblyNames)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly =>
                    assemblyNames.Contains(assembly.GetName().Name));
        }

        private static string GetTypeCategory(Type type)
        {
            if (type.IsClass)
                return "class";
            if (type.IsEnum)
                return "enum";
            if (type.IsValueType && !type.IsEnum)
                return "struct";
            return type.IsInterface ? "interface" : "unknown";
        }

        private List<TypeInfo> GetNamespaceInfo(string namespaceName)
        {
            var availableAssemblies = _userIntelliSenseAssemblies.Concat(IntelliSenseAssemblies).ToArray();
            var assemblies = FilterAssembliesByNames(availableAssemblies).ToList();

            var typesInNamespace = assemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => type.Assembly.FullName != null && type.Assembly.FullName.StartsWith(namespaceName));

            var typesInfo = typesInNamespace.Where(type => !type.IsGenericType).Select(type => new TypeInfo
            {
                Label = GenericTypeExtractorRegex.Replace(type.Name, ""),
                Type = GetTypeCategory(type)
            }).GroupBy(type => type.Label).Select(group => group.First()).ToList();

            return typesInfo;
        }
    }
}