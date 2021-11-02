#region USING
using System;
using System.IO;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using Cake.Core.Tooling;
using Cake.Common.IO;
using Cake.Incubator.Project;
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
#endregion


namespace CakeScript
{
    partial class Program
    {
        readonly string[] projects_OpenSource = new []
        {
          Path.Combine("FastReport.OpenSource", "FastReport.OpenSource.csproj"),
          Path.Combine("FastReport.Core.Web", "FastReport.OpenSource.Web.csproj"),
          Path.Combine("Extras", "OpenSource", "FastReport.OpenSource.Export.PdfSimple", "FastReport.OpenSource.Export.PdfSimple", "FastReport.OpenSource.Export.PdfSimple.csproj")
        };

        //[Debug]
        [DependsOn(nameof(Prepare))]
        public void BuildOpenSource()
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
        }


        [DependsOn(nameof(BuildOpenSource))]
        [DependsOn(nameof(PrepareNuget))]
        public void PackOpenSource()
        {
            string versionNum = version + "-" + config.ToLower();
            if (IsRelease)
                versionNum = version;

            DotNetCorePackSettings settings = new DotNetCorePackSettings
            {
                Configuration = config,
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = outdir,
                IncludeSymbols = true,
                SymbolPackageFormat = "snupkg"
            };

            if (IsDebug)
            {
                settings.IncludeSource = true;
            }

            settings.MSBuildSettings = new DotNetCoreMSBuildSettings()
              .WithProperty("SolutionDir", solutionDirectory)
              .WithProperty("SolutionFileName", solutionFilename)
              .WithProperty("Version", versionNum);

            foreach (var proj in projects_OpenSource)
            {
                DotNetCorePack(Path.Combine(solutionDirectory, proj), settings);
            }
        }


        [DependsOn(nameof(BuildOpenSource))]
        [DependsOn(nameof(PrepareNuget))]
        public void PackOpenSourcePlugins()
        {
            DotNetCorePackSettings settings = new DotNetCorePackSettings
            {
                Configuration = config,
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = outdir
            };

            string pluginsDirPath = Path.Combine(solutionDirectory, pluginsRelativePath);
            foreach (var proj in Plugins_Core)
            {
                string proj_path = Path.Combine(pluginsDirPath, $"FastReport.Data.{proj}", $"FastReport.OpenSource.Data.{proj}.csproj");

                DotNetCorePack(proj_path, settings);
            }
        }


    }
}
