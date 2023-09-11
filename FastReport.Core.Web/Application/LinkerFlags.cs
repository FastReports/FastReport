using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FastReport.Web
{
    internal static class LinkerFlags
    {
        internal const DynamicallyAccessedMemberTypes ExportTypeMembers = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            | DynamicallyAccessedMemberTypes.PublicProperties;

        internal const DynamicallyAccessedMemberTypes All = DynamicallyAccessedMemberTypes.All;
    }

#if !NET5_0_OR_GREATER
    [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
    Inherited = false)]
    internal sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicallyAccessedMembersAttribute"/> class
        /// with the specified member types.
        /// </summary>
        /// <param name="memberTypes">The types of members dynamically accessed.</param>
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
        }
    }

    //
    // Summary:
    //     Specifies the types of members that are dynamically accessed. This enumeration
    //     has a System.FlagsAttribute attribute that allows a bitwise combination of its
    //     member values.
    [Flags]
    internal enum DynamicallyAccessedMemberTypes
    {
        //
        // Summary:
        //     Specifies all members.
        All = -1,
        //
        // Summary:
        //     Specifies no members.
        None = 0,
        //
        // Summary:
        //     Specifies the default, parameterless public constructor.
        PublicParameterlessConstructor = 1,
        //
        // Summary:
        //     Specifies all public constructors.
        PublicConstructors = 3,
        //
        // Summary:
        //     Specifies all non-public constructors.
        NonPublicConstructors = 4,
        //
        // Summary:
        //     Specifies all public methods.
        PublicMethods = 8,
        //
        // Summary:
        //     Specifies all non-public methods.
        NonPublicMethods = 16,
        //
        // Summary:
        //     Specifies all public fields.
        PublicFields = 32,
        //
        // Summary:
        //     Specifies all non-public fields.
        NonPublicFields = 64,
        //
        // Summary:
        //     Specifies all public nested types.
        PublicNestedTypes = 128,
        //
        // Summary:
        //     Specifies all non-public nested types.
        NonPublicNestedTypes = 256,
        //
        // Summary:
        //     Specifies all public properties.
        PublicProperties = 512,
        //
        // Summary:
        //     Specifies all non-public properties.
        NonPublicProperties = 1024,
        //
        // Summary:
        //     Specifies all public events.
        PublicEvents = 2048,
        //
        // Summary:
        //     Specifies all non-public events.
        NonPublicEvents = 4096,
        //
        // Summary:
        //     Specifies all interfaces implemented by the type.
        Interfaces = 8192
    }
#endif
}
