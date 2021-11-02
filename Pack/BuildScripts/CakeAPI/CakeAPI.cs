using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using Cake.Core.Tooling;
using Cake.Common.IO;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Incubator.Project;
using Cake.Incubator.GlobbingExtensions;
using Cake.Common.Solution.Project;
using Cake.Common.Xml;
using Cake.Core.Configuration;
using Cake.FileHelpers;
using static Cake.Core.IO.GlobberExtensions;
using static Cake.Incubator.Project.ProjectParserExtensions;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.NuGet.Restore;

namespace CakeScript
{
    static class CakeAPI
    {
        private static readonly IGlobber Globber;
        private static readonly ICakeContext Context;


        static CakeAPI()
        {
            var env = new CakeEnvironment(new CakePlatform(), new CakeRuntime());
            var fileSystem = new FileSystem();
            var cakeLog = new CakeBuildLog(new CakeConsole(env));
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

        public static void DotNetCoreClean(string project)
            => Context.DotNetCoreClean(project);

        public static void DotNetCoreRestore(string project)
            => Context.DotNetCoreRestore(project);

        public static void DotNetCorePack(string project, DotNetCorePackSettings packSettings)
            => Context.DotNetCorePack(project, packSettings);

        public static void DotNetCoreMSBuild(string projectOrDirectory, DotNetCoreMSBuildSettings settings)
            => Context.DotNetCoreMSBuild(projectOrDirectory, settings);

        public static void DotNetCoreTest(string project, DotNetCoreTestSettings settings)
            => Context.DotNetCoreTest(project, settings);

        public static void NuGetPack(NuGetPackSettings settings)
            => Context.NuGetPack(settings);

        public static void NuGetPack(FilePath filePath, NuGetPackSettings settings)
            => Context.NuGetPack(filePath, settings);

        public static void NuGetRestore(FilePath targetFilePath)
            => Context.NuGetRestore(targetFilePath);

        public static bool DirectoryExists(DirectoryPath path)
            => Context.DirectoryExists(path);

        public static void CreateDirectory(DirectoryPath path)
            => Context.CreateDirectory(path);

        public static void DeleteFiles(GlobPattern pattern)
            => Context.DeleteFiles(pattern);

        public static void DeleteDirectory(DirectoryPath path, DeleteDirectorySettings settings)
            => Context.DeleteDirectory(path, settings);

        public static CustomProjectParserResult ParseProject(FilePath project, string configuration)
            => Context.ParseProject(project, configuration);

        public static CustomProjectParserResult ParseProject(FilePath project, string configuration, string platform)
            => Context.ParseProject(project, configuration, platform);

        public static FilePath[] ReplaceTextInFiles(string globberPattern, string findText, string replaceText)
            => Context.ReplaceTextInFiles(globberPattern, findText, replaceText);

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

        public static FilePathCollection GetFiles(string[] patterns)
            => Context.GetFiles(patterns);

        public static string XmlPeek(FilePath filePath, string xpath)
            => Context.XmlPeek(filePath, xpath);

        public static FilePath[] ReplaceRegexInFiles(string globberPattern, string rxFindPattern, string replaceText)
            => Context.ReplaceRegexInFiles(globberPattern, rxFindPattern, replaceText);
    }
}
