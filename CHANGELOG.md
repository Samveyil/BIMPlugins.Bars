# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.1.0] / 2026-11-06
### Added
- `RevitProgressBar.SetCancelText` to override the cancel button caption.
- `ProgressBarData.CancelText` and `ProgressBarData.GetDefaultCancelText`.
- `RevitProgressBarUtils` with one-call `Run` methods.
- XML documentation for the public API, README with examples and screenshots.
- Build with NUKE (`BIMPlugins.Nuke` 1.1.5) and publish to nuget.org from GitHub Actions with Trusted Publishing.
### Changed
- The cancel button caption follows the Revit interface language: `Отмена` for Russian, `Cancel` for any other language (was always `Отмена`).
- `net8.0-windows` is targeted as `net8.0-windows7.0` so the package builds with MSBuild.
- The package version is set in `build/Build.cs`.

## [2.0.1] / 2026-11-06
First release on nuget.org.
### Features
- One package for Revit 2019–2025: `net47` (Revit 2019 API) and `net8.0-windows` (Revit 2025 API).
- `RevitProgressBar`: progress bar in the Revit status bar with `Run`, `Increment`, cancel button, indeterminate mode and refresh settings.
- `RevitOptionsBar`: custom WPF content in the Revit options bar (`Show`, `Hide`).
- `StatusBarController` and `RevitRibbonController` to replace the status bar and disable the ribbon during long operations.
- Package metadata: MIT license, README, icon, project and repository links.

[2.1.0]: https://github.com/Samveyil/BIMPlugins.Bars/tree/v2.1.0
[2.0.1]: https://www.nuget.org/packages/BIMPlugins.Bars/2.0.1
