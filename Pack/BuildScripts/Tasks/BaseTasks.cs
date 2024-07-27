using static CakeScript.Startup;

namespace CakeScript;

partial class Program
{
    bool IsDemo = true;
    bool IsRelease = false;
    bool IsProfessional = false;
    bool IsDebug = false;
    bool IsTest = false;

    string config = Argument("config", "Debug");
    string solutionDirectory = Argument("solution-directory", "");
    readonly string solutionFilename = Argument("solution-filename", "FastReport.Net.sln");
    readonly string version = Argument("vers", "1.0.0.0");
    string outdir = Argument("out-dir", "");
    static readonly string pluginsRelativePath = Path.Combine("Extras", "Core", "FastReport.Data");
    readonly bool NuGetSpecialDirectorySort = Argument("nugetSpecialDirectorySort", false);

    readonly string[] Plugins_Core = new[]
    {
      "Postgres",
      "MsSql",
      "MySql",
      "Json",
      "Couchbase",
      "MongoDB",
      "OracleODPCore",
      "RavenDB",
      "SQLite",
      "ClickHouse",
      "Firebird",
      "Excel",
      "Cassandra",
      "Odbc",
    };

    internal string SolutionFile => Path.Combine(solutionDirectory, solutionFilename);

    string PackDir => Path.Combine(solutionDirectory, "Pack");

    internal string PluginsDir => Path.Combine(solutionDirectory, pluginsRelativePath);

    public Program(string[] args)
    {
        
    }
    
    public void Init()
    {
        IsDebug = config.ToLower() == "debug";
        IsDemo = config.ToLower() == "demo";
        IsRelease = config.ToLower() == "release";
        IsProfessional = config.ToLower() == "professional";
        IsTest = config.ToLower() == "test";

        Information($"CONFIG: {config}");
    }


    [DependsOn(nameof(Init))]
    public void Prepare()
    {
        if (String.IsNullOrWhiteSpace(solutionDirectory))
        {
            var dir = Directory.GetCurrentDirectory();

            try
            {
                for (int i = 0; i < 7; i++)
                {
                    var find_path = Path.Combine(dir, solutionFilename);
                    if (File.Exists(find_path))
                    {
                        solutionDirectory = dir;
                        break;
                    }
                    dir = Path.GetDirectoryName(dir);
                }
            }
            catch
            {
            }
        }
        if (String.IsNullOrWhiteSpace(solutionDirectory))
            throw new FileNotFoundException($"File `{solutionFilename}` was not found!");
        solutionDirectory = Path.GetFullPath(solutionDirectory);
        if (!solutionDirectory.EndsWith(Path.DirectorySeparatorChar))
            solutionDirectory += Path.DirectorySeparatorChar.ToString();

        Information($"solutionDirectory: {solutionDirectory}");
    }


    [DependsOn(nameof(Prepare))]
    public void PrepareNuget()
    {
        if (String.IsNullOrWhiteSpace(outdir))
        {
            outdir = Path.Combine(solutionDirectory, "fr_nuget");
        }
        else if (!Path.IsPathRooted(outdir))
        {
            outdir = Path.Combine(solutionDirectory, outdir);
        }

        if (!Directory.Exists(outdir))
            Directory.CreateDirectory(outdir);

        Information($"outdir: {outdir}");
    }

    [DependsOn(nameof(Prepare))]
    public void Clean()
    {
        string solutionFile = SolutionFile;
        DotNetMSBuild(solutionFile, new DotNetMSBuildSettings()
          .SetConfiguration(config)
          .WithTarget("CleanObjAndBin")
          .WithProperty("SolutionDir", solutionDirectory)
          .WithProperty("SolutionFileName", solutionFilename));

        DotNetClean(solutionFile);
    }

    public static void Default()
    {
        Information("Use: ");
        Information("   --name=value");

        Information("");

        Information("|>  --target= - change task, see the task with command cake --tree. Core, Plugins, Test-Connectors.");
        Information("|>  --config= - change config. Debug, Release, Demo.");
        Information("|>  --vers= - change version");
        Information("|>  --solution-directory= - change solution directory");
        Information("|>  --solution-filename= - change solution file name");
        Information("|>  --out-dir= - change output dir for packages");
        Information("");
        Information("OR: if you want to run 'Debug' mode, please add [Debug] attribute to your method");

    }

    internal string GetVersion()
    {
        string versionNum = $"{version}-{GetVersionSuffix()}";
        if (IsRelease)
            versionNum = version;
        return versionNum;
    }


    internal string GetVersionSuffix()
    {
        if (IsRelease)
            return string.Empty;

        return config.ToLower();
    }


    private string GetDirNameByProject(string project)
    {
        const string DEMO_DIR = "forAll";
        const string NET_STD_DIR = "net";
        const string NET_ENT_DIR = "net_ent";
        const string MONO_DIR = "mono";

        if (IsDemo)
            return DEMO_DIR;

        return project switch
        {
            "FastReport.Localization" => DEMO_DIR,
            "FastReport.Compat" => DEMO_DIR,
            "FastReport.DataVisualization" => DEMO_DIR,
            //"FastReport.Core" => NET_STD_DIR,
            //"FastReport.Web" => NET_STD_DIR,
            //"FastReport.SkiaDrawing" => NET_STD_DIR,
            //"FastReport.Compat.Skia" => NET_STD_DIR,
            //"FastReport.DataVisualization.Skia" => NET_STD_DIR,
            //"FastReport.Core.Skia" => NET_STD_DIR,
            //"FastReport.Web.Skia" => NET_STD_DIR,
            "FastReport.Blazor.Wasm" => NET_ENT_DIR,
            "FastReport.Mono" => MONO_DIR,
            _ => NET_STD_DIR,
            //_ => throw new NotImplementedException($"Unknown project {project}")
        };
    }

    internal string GetOutdir(string projectOrPath)
    {
        if (!NuGetSpecialDirectorySort)
            return outdir;

        string project;
        var firstExtSymbol = Path.GetExtension(projectOrPath)[1];   // ".csproj" or ".**"
        if (char.IsUpper(firstExtSymbol))
            // it's a project
            project = projectOrPath;
        else
            project = Path.GetFileNameWithoutExtension(projectOrPath);

        var additionDir = GetDirNameByProject(project);
        return Path.Combine(outdir, additionDir);
    }
}
