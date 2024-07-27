using Cake.Common.Tools.DotNet.Test;

namespace CakeScript;

partial class Program
{

    [DependsOn(nameof(Prepare))]
    [DependsOn(nameof(Clean))]
    public void Tests()
    {
        string solutionFile = SolutionFile;

        string versionNum = GetVersion();

        DotNetRestore(solutionFile);

        DotNetMSBuild(solutionFile, new DotNetMSBuildSettings()
          .SetConfiguration(config)
          .WithTarget("Build")
          .WithProperty("SolutionDir", solutionDirectory)
          .WithProperty("SolutionFileName", solutionFilename)
          .WithProperty("Version", versionNum)
        );

        DotNetTest(solutionFile, new DotNetTestSettings()
        {
            Configuration = config,
        });

        //dotnet build "%~dp0..\FastReport.Tests.Data.Core.sln" - c Release
        //pushd % ~dp0FastReport.Tests.Data.Core
        //dotnet test FastReport.Tests.Data.Core.sln
    }
}
