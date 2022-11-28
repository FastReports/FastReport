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
#endregion


namespace CakeScript
{
    partial class Program
    {
        [DependsOn(nameof(PrepareNuget))]
        public void FastReportLocalization()
        {
            string packDir = PackDir;
            string packFRLocalizationDir = Path.Combine(packDir, "FastReport.Localization");
            string localizationDir = Path.Combine(solutionDirectory, "Localization");

            if (!Directory.Exists(localizationDir))
                throw new Exception("'Localization' directory wasn't found on path: " + localizationDir);

            string tempDir = Path.Combine(packFRLocalizationDir, "tmp");
            if (DirectoryExists(tempDir))
            {
                DeleteDirectory(tempDir, new DeleteDirectorySettings
                {
                    Force = true,
                    Recursive = true
                });
            }
            CreateDirectory(tempDir);

            Information($"{Environment.NewLine}FastReport.Localization pack...", ConsoleColor.DarkMagenta);

            var packFiles = new[] {
                //new NuSpecContent{Source = Path.Combine(packDir, NET_LICENSE), Target = ""},
                new NuSpecContent{Source = Path.Combine(packDir, FRLOGO192PNG), Target = ""},
                new NuSpecContent{Source = Path.Combine(packDir, MIT_LICENSE), Target = ""},
                new NuSpecContent{Source = Path.Combine(packFRLocalizationDir, "**", "*.*"), Target = ""},
                new NuSpecContent{Source = Path.Combine(localizationDir, "*.frl"), 
                    Target = Path.Combine("build", "Localization")}
            };

            // generate nuspec
            var nuGetPackSettings = new NuGetPackSettings
            {
                Id = "FastReport.Localization",
                Authors = new[] { "Fast Reports Inc." },
                Owners = new[] { "Fast Reports Inc." },
                Description = "FastReport.Localization includes localization files for FastReport .NET, FastReport.Core, FastReport.CoreWin, FastReport.Mono and FastReport.OpenSource",
                ProjectUrl = new Uri("https://www.fast-report.com/en/product/fast-report-net"),
                Icon = FRLOGO192PNG,
                //IconUrl = new Uri("https://www.fast-report.com/download/images/frlogo-big.png"),
                License = new NuSpecLicense { Type = "file", Value = MIT_LICENSE },
                //LicenseUrl = new Uri(project.PackageLicenseUrl), // The licenseUrl and license elements cannot be used together.
                Tags = new[] { "fastreport", "localization" , "frl"},
                //FrameworkAssemblies = frameworkAssemblies,
            };

            nuGetPackSettings.Version = version;
            nuGetPackSettings.BasePath = tempDir;
            nuGetPackSettings.OutputDirectory = outdir;
            nuGetPackSettings.Files = packFiles;

            // pack
            NuGetPack(nuGetPackSettings);
        }

    }
}
