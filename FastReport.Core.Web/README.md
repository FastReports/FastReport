## How to use **FastReport.Web for Core**

1. Add **FastReport.Web** as nuget dependency in your **ASP.NET Core** project:

```xml
<PackageReference Include="FastReport.Web" Version="*" />
```

2. Register **FastReport.Web** in the *Configure* method of your *Startup* class:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    ...
    app.UseFastReport();
    ...
}
```

3. Create **WebReport** object and render it in your view file:

```csharp
public IActionResult Index()
{
    var webReport = new WebReport();
    webReport.Report.Load("path/to/report.frx");

    return View(webReport);
}
```

```html
@model FastReport.Web.WebReport

<div>
    @await Model.Render()
</div>
```

## Browsers support

FastReport.Web for Core supports latest versions of Chrome, Firefox, Safari, Opera and Edge browsers.