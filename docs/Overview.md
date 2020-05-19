## Overview of Accessibility Insights for Windows

### Tool Overview
Windows UI Automation (UIA) is the platform-provided way for accessibility tools to interact with programs. A fully accessible program is a program whose full functionality can be accessed exclusively through the UIA-provided mechanism. AccessibilityInsights for Windows uses the UIA-provided information to scan programs for accessibility issues. The basic units of UIA are:

- Hierarchy: The entire surface of all interactive applications is represented as a tree of Elements.
- Elements: Each Element may optionally have any combination of Properties, Patterns, and child Elements.
- Properties: Each Property is a Key/Value pair that identifies some aspect of the Element. The key is the property type (for example, IsVisible) and the value is the state of that property (for example, true or false).
- Patterns: There are several patterns that map to functionality that is exposed via the UI. For example, a list element will typically support a pattern that exposes all of the items in the list, as well as the ability to learn which item is currently selected, and the ability to select a different element, if desired.

Accessibility Insights takes the data provided by UIA and compares it to a set of rules which identify cases where the given data would create issues for users of the assistive technologies which consume UIA data. The results of these rules are then displayed in an environment that supports exploration of the data in its appropriate context. The results can be saved to disk for later use or analysis. If these files are later loaded into Accessibility Insights, the rich exploration environment is re-created.

### Code Organization
The code is organized into the following general areas:

#### Application Entry Points
These assemblies allow user interaction with the Runtime components and the Accessibility Rules.

Assembly | Responsibility
--- | ---
AccessibilityInsights | Provide the UI for most users. This application is built using WPF.
AccessibilityInsights.CommonUxComponents | Provide non-specialized visual elements used by the main app and extensions. This allows the main app and extensions to share ux components that are unrelated to the runtime.
AccessibilityInsights.Extensions | Provide extension points that allow certain non-core functionality to be implemented in a loosely coupled way.
AccessibilityInsights.SharedUx | Provide visual elements used by the main app. This code is in a separate assembly for historical reasons.
AccessibilityInsights.Win32 | Provide a wrapper around Win32-specific code that is needed by other assemblies.

#### Extensions
Extensions are intended to allow loose coupling of non-core code. They build upon the [Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/). At the moment, extensions provide the following capabilities:

- Telemetry : Send information about product usage. **Limited** diagnostic data is also sent to help identify trends in user issues.
- Updates : Provide a centralized control mechanism to notify users of updated releases.
- Issue Filing : Enable users to file accessibility issues directly from within the product.

The following extensions exist:

Assembly | Responsibility
--- | ---
AccessibilityInsights.Extensions.GitHubAutoUpdate | Implement a web-based update mechanism. Requires that the files be publicly accessible via a Uri.
AccessibilityInsights.Extensions.AzureDevOps | Implement issue filing support using Azure Boards.
AccessibilityInsights.Extensions.Github | Implement issue filing support using GitHub issues.
AccessibilityInsights.Extensions.Telemetry | Implement simple telemetry built upon [Microsoft ApplicationInsights](https://www.nuget.org/packages/Microsoft.ApplicationInsights).

#### Version Handling assemblies
Assembly | Responsibility
--- | ---
AccessibilityInsights.VersionSwitcher.exe | Provides an out-of-process tool to switch user versions during upgrades or if the user changes to a different release channel.
AccessibilityInsights.SetupLibrary | Provides setup-related classes that are used by both AccessibiltyInsights.exe and AccessibilityInsights.VersionSwitcher.exe

#### Packaging
The packaging project exists to gather assemblies into their shipping vehicle:

Project | Responsibility
--- | ---
MSI | Builds the MSI file that installs the application.

#### Tests
Unit tests are built using Moq. The following assemblies exist for testing purposes:
- AccessibilityInsights.Extensions.AzureDevOpsTests
- AccessibilityInsights.Extensions.GitHubAutoUpdateUnitTests
- AccessibilityInsights.Extensions.GitHubUnitTests
- AccessibilityInsights.Extensions.TelemetryTests
- AccessibilityInsights.ExtensionsTests
- AccessibilityInsights.SetupLibraryUnitTests
- AccessibilityInsights.SharedUxTests
