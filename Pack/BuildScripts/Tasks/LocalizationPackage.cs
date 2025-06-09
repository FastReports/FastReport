using Path = System.IO.Path;
using static CakeScript.Startup;


namespace CakeScript;

partial class Program
{
    [DependsOn(nameof(PrepareNuget))]
    public void FastReportLocalization()
    {
        const string projName = "FastReport.Localization";
        string packDir = PackDir;
        string packFRLocalizationDir = Path.Combine(packDir, projName);
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
            new NuSpecContent{Source = Path.Combine(packDir, FRLOGO192PNG), Target = ""},
            new NuSpecContent{Source = Path.Combine(packDir, MIT_LICENSE), Target = ""},
            new NuSpecContent{Source = Path.Combine(packFRLocalizationDir, "**", "*.*"), Target = ""},
            new NuSpecContent{Source = Path.Combine(localizationDir, "*.frl"), 
                Target = Path.Combine("build", "Localization")}
        };

        // generate nuspec
        var nuGetPackSettings = new NuGetPackSettings
        {
            Id = projName,
            Authors = ["Fast Reports Inc."],
            Owners = ["Fast Reports Inc."],
            Description = "FastReport.Localization includes localization files for FastReport .NET, FastReport.Core, FastReport.WPF, FastReport.Avalonia, FastReport.Mono and FastReport.OpenSource",
            ProjectUrl = new Uri("https://www.fast-report.com/products/fast-report-net"),
            Icon = FRLOGO192PNG,
            License = new NuSpecLicense { Type = "file", Value = MIT_LICENSE },
            Tags = ["fastreport", "localization", "frl"],
            Version = version,
            BasePath = tempDir,
            OutputDirectory = GetOutdir(projName),
            Files = packFiles,
        };

        UseNugetLocalization(nuGetPackSettings, ProductType.Localization);

        // pack
        NuGetPack(nuGetPackSettings);
    }

}
