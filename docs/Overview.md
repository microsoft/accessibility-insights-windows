## Overview of Accessibility Insights for Windows

### Tool overview
Windows UI Automation (UIA) is the platform-provided way for accessibility tools to interact with programs. A fully accessible program is a program whose full functionality can be accessed exclusively through the UIA-provided mechanism. Accessibility Insights for Windows uses the UIA-provided information to scan programs for accessibility issues. The basic units of UIA are:

- Hierarchy: The entire surface of all interactive applications is represented as a tree of Elements.
- Elements: Each Element may optionally have any combination of Properties, Patterns, and child Elements.
- Properties: Each Property is a Key/Value pair that identifies some aspect of the Element. The key is the property type (for example, IsVisible) and the value is the state of that property (for example, true or false).
- Patterns: There are several patterns that map to functionality that is exposed via the UI. For example, a list element will typically support a pattern that exposes all of the items in the list, as well as the ability to learn which item is currently selected, and the ability to select a different element, if desired.

Accessibility Insights uses [Axe.Windows](https://www.nuget.org/packages/Axe.Windows) to take the data provided by UIA and compare it to a set of rules which identify cases where the given data would create issues for users of the assistive technologies which consume UIA data. The results of these rules are then displayed in an environment that supports exploration of the data in its appropriate context. The results can be saved to disk for later use or analysis. If these files are later loaded into Accessibility Insights, the rich exploration environment is re-created.

### Code organization
The code is organized into the following general areas:

#### Core application projects
These projects provide the primary interactive layer around the data exposed by [Axe.Windows](https://www.nuget.org/packages/Axe.Windows)

Project | Responsibility
--- | ---
AccessibilityInsights | Provide the UI for most users. This application uses WPF and targets .NET Framework 4.8.
CommonUxComponents | Provide non-specialized visual elements used by the core application and extensions. This allows the core application and extensions to share ux components that are unrelated to the runtime.
Extensions | Provide extension points that allow certain non-core functionality to be implemented in a loosely coupled way.
SharedUx | Provide visual elements used by the core application. This code is in a separate assembly for historical reasons.
Win32 | Provide a wrapper around Win32-specific code that is needed by other assemblies.

#### Extension projects
Extensions are intended to allow loose coupling of non-core code. They build upon the [Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/). At the moment, extensions provide the following capabilities:

- Telemetry : Send information about product usage. **Limited** diagnostic data is also sent to help identify trends in user issues.
- Updates : Provide a centralized control mechanism to notify users of updated releases.
- Issue Filing : Enable users to file accessibility issues directly from within the product.

The following extensions exist:

Project | Responsibility
--- | ---
Extensions.DiskLoggingTelemetry | Implement simple telemetry to disk. Does not deploy with the production version.
Extensions.GitHubAutoUpdate | Implement a web-based update mechanism. Requires that the files be publicly accessible via a uniform resource identifier (URI).
Extensions.AzureDevOps | Implement issue filing support using Azure Boards.
Extensions.Github | Implement issue filing support using GitHub issues.
Extensions.Telemetry | Implement simple telemetry built upon [Microsoft ApplicationInsights](https://www.nuget.org/packages/Microsoft.ApplicationInsights).

#### Version handling projects
These projects are used for upgrades and for changing release channels:

Project | Responsibility
--- | ---
VersionSwitcher | Provide an out-of-process tool to switch user versions during upgrades or if the user changes to a different release channel.
SetupLibrary | Provide setup-related classes that are shared across AccessibilityInsights, Extensions.GitHubAutoUpdate, and VersionSwitcher projects.
Manifest | Create a signed manifest that helps protect the update mechanism from potential abuse.

#### Package-related projects
These projects create the MSI file that we ultimately ship:

Project | Responsibility
--- | ---
MSI | Create the MSI file that installs the application.
CustomActions | Provide application-specific actions that are triggered during MSI installation.
CustomActions.Package | Package the CustomActions into the format needed by MSI.

#### Test projects
The following projects exist for testing purposes:
Project | Comment
--- | ---
CustomActionsUnitTests | Unit tests for CustomActions project.
Extensions.DiskLoggingTelemetryTests | Unit tests for Extensions.DiskLoggingTelemetry project.
Extensions.AzureDevOpsTests | Unit tests for Extensions.AzureDevOps project.
Extensions.GitHubAutoUpdateUnitTests | Unit tests for Extensions.GitHubAutoUpdate project.
Extensions.GitHubUnitTests | Unit tests for Extensions.GitHub project.
Extensions.TelemetryTests | Unit tests for Extensions.Telemetry project.
ExtensionsTests | Unit tests for Extensions project.
SetupLibraryUnitTests | Unit tests for SetupLibrary project.
SharedUxTests | Unit tests for SharedUx project.
AccessibilityInsightsUnitTests | Unit tests for AccessibilityInsights project.
MsiFileTests | Unit tests to validate MSI project.
UITests | Integration tests to run AIWin. Tests for basic functionality and accessibility issues.

For more information on test project standards, please refer to [Unit Test Bar and Standards](UnitTestBarAndStandards.md).