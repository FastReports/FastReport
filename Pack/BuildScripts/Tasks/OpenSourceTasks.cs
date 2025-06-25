using Path = System.IO.Path;

namespace CakeScript;

partial class Program
{
    readonly string[] projects_OpenSource =
    [
      Path.Combine("FastReport.OpenSource", "FastReport.OpenSource.csproj"),
      Path.Combine("FastReport.Core.Web", "FastReport.OpenSource.Web.csproj"),
      Path.Combine("Extras", "OpenSource", "FastReport.OpenSource.Export.PdfSimple", "FastReport.OpenSource.Export.PdfSimple", "FastReport.OpenSource.Export.PdfSimple.csproj")
    ];

    //[Debug]
    [DependsOn(nameof(Prepare))]
    public void BuildOpenSource()
    {
        string versionNum = GetVersion();

        foreach (var csproj in projects_OpenSource)
        {
            var proj_path = Path.Combine(solutionDirectory, csproj);

            DotNetClean(proj_path);
        }

        foreach (var csproj in projects_OpenSource)
        {
            var proj_path = Path.Combine(solutionDirectory, csproj);

            DotNetRestore(proj_path);

            DotNetMSBuild(proj_path, new DotNetMSBuildSettings()
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
        string versionNum = GetVersion();

        DotNetPackSettings settings = new DotNetPackSettings
        {
            Configuration = config,
            NoBuild = true,
            NoRestore = true,
            OutputDirectory = OutDir,
            IncludeSymbols = true,
            SymbolPackageFormat = "snupkg"
        };

        if (IsDebug)
        {
            settings.IncludeSource = true;
        }

        settings.MSBuildSettings = new DotNetMSBuildSettings()
          .WithProperty("SolutionDir", solutionDirectory)
          .WithProperty("SolutionFileName", solutionFilename)
          .WithProperty("Version", versionNum);

        foreach (var proj in projects_OpenSource)
        {
            DotNetPack(Path.Combine(solutionDirectory, proj), settings);
        }
    }

    [Obsolete]
    [DependsOn(nameof(BuildOpenSource))]
    [DependsOn(nameof(PrepareNuget))]
    public void PackOpenSourcePlugins()
    {
        DotNetPackSettings settings = new DotNetPackSettings
        {
            Configuration = config,
            NoRestore = true,
            OutputDirectory = OutDir
        };

        string pluginsDirPath = PluginsDir;
        foreach (var proj in Plugins_Core)
        {
            string proj_path = Path.Combine(pluginsDirPath, $"FastReport.Data.{proj}", $"FastReport.OpenSource.Data.{proj}.csproj");

            DotNetRestore(proj_path);

            DotNetPack(proj_path, settings);
        }
    }

}
