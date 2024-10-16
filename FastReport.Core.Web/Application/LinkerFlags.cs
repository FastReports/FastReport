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
}
