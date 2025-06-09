using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using Cake.Core.Tooling;
using Cake.Common.Tools.NuGet;
using Cake.Common.Solution.Project;
using Cake.Common.Xml;
using Cake.Core.Configuration;
using static Cake.Core.IO.GlobberExtensions;
#if FILEHELPERS
using Cake.FileHelpers;
#endif
#if INCUBATOR
using Cake.Incubator.Project;
using Cake.Incubator.GlobbingExtensions;
using static Cake.Incubator.Project.ProjectParserExtensions;
#endif
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.NuGet.Restore;
using Cake.Common;
using Cake.Common.Tools.SignTool;
using Cake.Common.Tools.DotNet.Run;

namespace CakeScript;

static class CakeAPI
{
    private static readonly IGlobber Globber;
    private static readonly ICakeContext Context;


    static CakeAPI()
    {
        var env = new CakeEnvironment(new CakePlatform(), new CakeRuntime());
        var fileSystem = new FileSystem();

        var verbosity = Verbosity.Normal;
        if(Startup.HasArgument("cake-verbosity"))
        {
            verbosity = Enum.Parse<Verbosity>(Startup.Argument("cake-verbosity"));
        }
        var cakeLog = new CakeBuildLog(new CakeConsole(env), verbosity);

        var cakeConfiguration = new CakeConfiguration(new Dictionary<string, string>());
        var toolRepos = new ToolRepository(env);
        Globber = new Globber(fileSystem, env);
        var cakeDataService = new CakeDataService();
        var registry = new WindowsRegistry();
        var toolLocator = new ToolLocator(env, toolRepos, new ToolResolutionStrategy(fileSystem, env, Globber, cakeConfiguration, cakeLog));
        var cakeArgs = new CakeArguments(new List<string>().ToLookup(x => x));
        var procRunner = new ProcessRunner(fileSystem, env, cakeLog, toolLocator, cakeConfiguration);
        Context = new CakeContext(fileSystem, env, Globber, cakeLog, cakeArgs, procRunner, registry, toolLocator, cakeDataService, cakeConfiguration);
    }

    public static void MSBuild(FilePath solution, Action<MSBuildSettings> configurator)
        => Context.MSBuild(solution, configurator);

    public static void MSBuild(FilePath solution, MSBuildSettings settings)
        => Context.MSBuild(solution, settings);

    public static void DotNetRun(string project)
        => Context.DotNetRun(project);

    public static void DotNetRun(string project, ProcessArgumentBuilder arguments)
        => Context.DotNetRun(project, arguments);

    public static void DotNetRun(string project, ProcessArgumentBuilder arguments, DotNetRunSettings settings)
        => Context.DotNetRun(project, arguments, settings);

    public static void DotNetRun(string project, DotNetRunSettings settings)
        => Context.DotNetRun(project, settings);

    public static void DotNetClean(string project)
        => Context.DotNetClean(project);

    public static void DotNetRestore(string project)
        => Context.DotNetRestore(project);

    public static void DotNetRestore(string root, DotNetRestoreSettings settings)
        => Context.DotNetRestore(root, settings);

    public static void DotNetBuild(string projectOrDirectory, DotNetBuildSettings settings)
        => Context.DotNetBuild(projectOrDirectory, settings);

    public static void DotNetPack(string project, DotNetPackSettings packSettings)
        => Context.DotNetPack(project, packSettings);

    public static void DotNetMSBuild(string projectOrDirectory, DotNetMSBuildSettings settings)
        => Context.DotNetMSBuild(projectOrDirectory, settings);

    public static void DotNetTest(string project, DotNetTestSettings settings)
        => Context.DotNetTest(project, settings);

    public static void NuGetPack(NuGetPackSettings settings)
        => Context.NuGetPack(settings);

    public static void NuGetPack(FilePath filePath, NuGetPackSettings settings)
        => Context.NuGetPack(filePath, settings);

    public static void NuGetRestore(FilePath targetFilePath)
        => Context.NuGetRestore(targetFilePath);

    public static void NuGetRestore(FilePath targetFilePath, NuGetRestoreSettings settings)
        => Context.NuGetRestore(targetFilePath, settings);

    public static bool DirectoryExists(DirectoryPath path)
        => Context.DirectoryExists(path);

    public static void CreateDirectory(DirectoryPath path)
        => Context.CreateDirectory(path);

    public static void DeleteFiles(GlobPattern pattern)
        => Context.DeleteFiles(pattern);

    public static void DeleteDirectory(DirectoryPath path, DeleteDirectorySettings settings)
        => Context.DeleteDirectory(path, settings);

    public static void DeleteFilesAndDirs(string patternOrPath)
    {
        var files = GetFiles(patternOrPath);
        foreach (var file in files)
            Context.DeleteFile(file);

        var dirs = Context.GetDirectories(patternOrPath);
        foreach (var dir in dirs)
            Context.DeleteDirectory(dir, new DeleteDirectorySettings { Recursive = true, Force = true });
    }

#if INCUBATOR
    public static CustomProjectParserResult ParseProject(FilePath project, string configuration)
        => Context.ParseProject(project, configuration);

    public static CustomProjectParserResult ParseProject(FilePath project, string configuration, string platform)
        => Context.ParseProject(project, configuration, platform);
#endif

    public static void CopyDirectory(DirectoryPath source, DirectoryPath destination)
    => Context.CopyDirectory(source, destination);

    public static void CopyFiles(GlobPattern pattern, DirectoryPath targetDirectoryPath)
        => Context.CopyFiles(pattern, targetDirectoryPath);

    public static void CopyFiles(GlobPattern pattern, DirectoryPath targetDirectoryPath, bool preserveFolderStructure)
        => Context.CopyFiles(pattern, targetDirectoryPath, preserveFolderStructure);

    public static void MoveFile(FilePath filePath, FilePath targetFilePath)
        => Context.MoveFile(filePath, targetFilePath);

    public static void MoveFileToDirectory(FilePath filePath, DirectoryPath targetDirectoryPath)
        => Context.MoveFileToDirectory(filePath, targetDirectoryPath);

    public static void MoveFiles(GlobPattern pattern, DirectoryPath targetDirectoryPath)
        => Context.MoveFiles(pattern, targetDirectoryPath);

    public static IEnumerable<FilePath> GetFiles(string pattern)
        => Globber.GetFiles(pattern);

    public static IEnumerable<FilePath> GetFiles(GlobPattern pattern)
        => Globber.GetFiles(pattern);

#if INCUBATOR
    public static FilePathCollection GetFiles(string[] patterns)
        => Context.GetFiles(patterns);
#endif

    public static string XmlPeek(FilePath filePath, string xpath)
        => Context.XmlPeek(filePath, xpath);

    public static int StartProcess(FilePath fileName, ProcessSettings settings)
        => Context.StartProcess(fileName, settings);

    public static int StartProcess(FilePath fileName, string arguments)
        => Context.StartProcess(fileName, arguments);

    public static void Sign(string assembly, SignToolSignSettings settings)
        => Context.Sign(assembly, settings);

#if FILEHELPERS
    public static FilePath[] ReplaceRegexInFiles(string globberPattern, string rxFindPattern, string replaceText)
        => Context.ReplaceRegexInFiles(globberPattern, rxFindPattern, replaceText);

    public static FilePath[] ReplaceTextInFiles(string globberPattern, string findText, string replaceText)
        => Context.ReplaceTextInFiles(globberPattern, findText, replaceText);

#endif
}
