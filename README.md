# Extensify

Extensify is a plugin-based app framework in .NET 10.  
The host discovers external plugin DLLs at runtime and executes them by command.

## Architecture Checklist

- [x] `Extensify.Abstractions` contains shared plugin contracts only.
- [x] `Extensify.App` references `Abstractions`, `Loader`, and `Utils`.
- [x] `Extensify.Loader` references `Abstractions` and `Utils`.
- [x] Plugins reference only `Abstractions`.
- [x] Plugins are discovered dynamically from the `plugins` folder.

## Quality Checklist

- [x] Reflection-based plugin discovery.
- [x] Validation for plugin metadata (`Name`, `Description`, `Command`).
- [x] Duplicate command detection.
- [x] Non-fatal load error collection and console warnings.
- [x] Optional isolated assembly loading using `AssemblyLoadContext`.

## Run Locally

```powershell
dotnet build .\Extensify.slnx

$pluginDir = ".\src\Extensify.App\bin\Debug\net10.0\plugins"
New-Item -ItemType Directory -Path $pluginDir -Force | Out-Null

Copy-Item .\src\Extensify.Plugins.Calculator\bin\Debug\net10.0\Extensify.Plugins.Calculator.dll $pluginDir -Force
Copy-Item .\src\Extensify.Plugins.WeatherMock\bin\Debug\net10.0\Extensify.Plugins.WeatherMock.dll $pluginDir -Force

dotnet run --project .\src\Extensify.App\Extensify.App.csproj
```

## Release Readiness

- [ ] Add automated tests (unit + integration for loader behavior).
- [ ] Add CI pipeline (`dotnet build`, tests, formatting).
- [ ] Add semantic versioning and release notes.
- [ ] Add signed plugin distribution strategy.
