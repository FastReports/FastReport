using System.Collections.Generic;

namespace FastReport.Web.Services.Helpers
{
    internal class MemberDetailsBase
    {
        public string Label { get; init; }
        public string Type { get; init; }
    }

    internal class PropertyDetails : MemberDetailsBase
    {
        public string PropertyType { get; init; }
        public bool CanRead { get; init; }
        public bool CanWrite { get; init; }
    }

    internal class FieldDetails : MemberDetailsBase
    {
        public string FieldType { get; init; }
        public bool IsPublic { get; init; }
        public bool IsStatic { get; init; }
    }

    internal class MethodDetails : MemberDetailsBase
    {
        public string ReturnType { get; init; }
        public List<ParameterDetails> Parameters { get; init; }
        public bool IsPublic { get; init; }
        public bool IsStatic { get; init; }
    }

    internal class ParameterDetails
    {
        public string Name { get; init; }
        public string Type { get; init; }
    }

    internal class TypeInfo
    {
        public string Label { get; init; }
        public string Type { get; init; }
    }

    internal class ClassDetails
    {
        public string ClassName { get; init; }
        public IEnumerable<MethodDetails> Methods { get; init; }
        public IEnumerable<FieldDetails> Fields { get; init; }
        public IEnumerable<PropertyDetails> Properties { get; init; }
    }
}