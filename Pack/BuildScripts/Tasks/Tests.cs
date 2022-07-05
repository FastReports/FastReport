#region USING
using System;
using System.IO;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using Cake.Core.Tooling;
using Cake.Common.IO;
using Cake.Common.Solution.Project;
using Path = System.IO.Path;

using System.Collections.Generic;
using System.Linq;

using static CakeScript.CakeAPI;
using static CakeScript.Startup;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet.Test;
#endregion


namespace CakeScript
{
    partial class Program
    {

        [DependsOn(nameof(Prepare))]
        [DependsOn(nameof(Clean))]
        public void Tests()
        {
            string solutionFile = SolutionFile;

            string versionNum = GetVersion();

            DotNetRestore(solutionFile);

            DotNetMSBuild(solutionFile, new DotNetMSBuildSettings()
              .SetConfiguration(config)
              .WithTarget("Build")
              .WithProperty("SolutionDir", solutionDirectory)
              .WithProperty("SolutionFileName", solutionFilename)
              .WithProperty("Version", versionNum)
            );

            DotNetTest(solutionFile, new DotNetTestSettings()
            {
                Configuration = config,
            });

            //dotnet build "%~dp0..\FastReport.Tests.Data.Core.sln" - c Release
            //pushd % ~dp0FastReport.Tests.Data.Core
            //dotnet test FastReport.Tests.Data.Core.sln

        }
    }
}
