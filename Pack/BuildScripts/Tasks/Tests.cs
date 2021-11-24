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
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Test;
#endregion


namespace CakeScript
{
    partial class Program
    {

        [DependsOn(nameof(Prepare))]
        public void Tests()
        {
            string solutionFile = Path.Combine(solutionDirectory, solutionFilename);

            string versionNum = version + "-" + config.ToLower();
            if (IsRelease)
                versionNum = version;

            DotNetCoreMSBuild(solutionFile, new DotNetCoreMSBuildSettings()
              .SetConfiguration(config)
              .WithTarget("CleanObjAndBin")
              .WithProperty("SolutionDir", solutionDirectory)
              .WithProperty("SolutionFileName", solutionFilename)
              .WithProperty("Version", versionNum));

            DotNetCoreClean(solutionFile);

            DotNetCoreRestore(solutionFile);

            DotNetCoreMSBuild(solutionFile, new DotNetCoreMSBuildSettings()
              .SetConfiguration(config)
              .WithTarget("Build")
              .WithProperty("SolutionDir", solutionDirectory)
              .WithProperty("SolutionFileName", solutionFilename)
              .WithProperty("Version", versionNum)
            );

            DotNetCoreTest(solutionFile, new DotNetCoreTestSettings()
            {
                Configuration = config,
            });

            //dotnet build "%~dp0..\FastReport.Tests.Data.Core.sln" - c Release
            //pushd % ~dp0FastReport.Tests.Data.Core
            //dotnet test FastReport.Tests.Data.Core.sln

        }
    }
}
