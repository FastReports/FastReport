MVC Demo for FastReport.Net Core
================================

This article demonstrates how to use FastReport.Net for ASP.NET MVC
in ASP.NET Core applications.
First you need Professional or Enterprise edition of FastReport.Net. 
Also you can use Demo version.


How to compile with Visual Studio
---------------------------------

- you need the Visual Studio 2017.3 or higher; 
- you need the .NET Core SDK 2.0 or higher. Get it
  at https://www.microsoft.com/net/download/core;
- open the .sln file ;
- fix the FastReportCore dependency with NuGet. In order to do this, add local
  FastReport NuGet Repository. By default the package is located in the
  "C:\Program Files\FastReports\FastReport.Net Demo\Nugets" folder;
- compile the project and run the application.


How to compile with .NET Core SDK
---------------------------------

- you need the .NET Core SDK 2.0 or higher. Get it
  at https://www.microsoft.com/net/download/core;
- open the %AppData%\NuGet\NuGet.Config file;
- add local FastReport Nuget Repository to config file.
  By default the package is located in the
  "C:\Program Files\FastReports\FastReport.Net Demo\Nugets" folder;

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <packageSources>
        <add
             key="FastReportRepository"
             value="C:\Program Files\FastReports\FastReport.Net Demo\Nugets"
        />
      </packageSources>
    </configuration>

- run `dotnet restore` command for restoring the dependencies;
- run `dotnet run` command for starting the application.
 

How to run the application on the Linux systems
-----------------------------------------------

- install the `libgdiplus` library;
- install the `libx11-dev` library;
- install the `xvfb` application for running on linux server;
- set up an environment variable DISPLAY for linux server, e.g. DISPLAY=:99;
- for running the FastReport.OpenSource.MVC Demo use the following command:
    
    dotnet run

How to compile the Docker Image
-------------------------------

- you need the Visual Studio 2017.3 or higher; 
- you need the .NET Core SDK 2.0 or higher. Get it
  at https://www.microsoft.com/net/download/core;
- you need the Docker. Get it at https://www.docker.com;
- you need the Visual Studio Tools for Docker; get it
  at https://marketplace.visualstudio.com;
- open the .sln file ;
- fix the FastReportCore dependency with NuGet. In order to do this, add local
  FastReport NuGet Repository. By default, the package is located in the
  "C:\Program Files\FastReports\FastReport.Net Demo\Nugets" folder;
- set up the "Startup project" property to "docker-compose";
- set up "Release" build;
- compile the project;
- run the compiled image inside the desired container using
   
    docker run -d -p 5000:5000 --name fastreport FastReport.OpenSource.MVC:latest 

- in your browser follow to localhost:5000.    

NuGet Configuration
-------------------

The default location for the NuGet.Config file is:

Mac ~/.config/NuGet/NuGet.Config
Windows %AppData%\NuGet\NuGet.Config
Linux ~/.config/NuGet/NuGet.Config
