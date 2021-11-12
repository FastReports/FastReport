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
            string versionNum = version + "-" + config.ToLower();
            if (IsRelease)
                versionNum = version;

            foreach(var csproj in projects_OpenSource)
            {
                var proj_path = Path.Combine(solutionDirectory, csproj);

                DotNetCoreClean(proj_path);
            }

            foreach (var csproj in projects_OpenSource)
            {
                var proj_path = Path.Combine(solutionDirectory, csproj);

                DotNetCoreRestore(proj_path);

                DotNetCoreMSBuild(proj_path, new DotNetCoreMSBuildSettings()
                  .SetConfiguration(config)
                  .WithTarget("Build")
                  .WithProperty("SolutionDir", solutionDirectory)
                  .WithProperty("SolutionFileName", solutionFilename)
                  .WithProperty("Version", versionNum)
                );
            }
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
                NoRestore = true,
                OutputDirectory = outdir
            };

            string pluginsDirPath = Path.Combine(solutionDirectory, pluginsRelativePath);
            foreach (var proj in Plugins_Core)
            {
                string proj_path = Path.Combine(pluginsDirPath, $"FastReport.Data.{proj}", $"FastReport.OpenSource.Data.{proj}.csproj");

                DotNetCoreRestore(proj_path);

                DotNetCorePack(proj_path, settings);
            }
        }


    }
}
