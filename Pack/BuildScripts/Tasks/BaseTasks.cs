using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static CakeScript.Startup;
using static CakeScript.CakeAPI;
using Cake.Common.Tools.DotNet.MSBuild;

namespace CakeScript
{
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
        readonly string pluginsRelativePath = Path.Combine("Extras", "Core", "FastReport.Data");

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
          "Cassandra"
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
            string versionNum = version + "-" + config.ToLower();
            if (IsRelease)
                versionNum = version;
            return versionNum;
        }
    }
}
