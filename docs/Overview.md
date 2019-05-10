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

#### Runtime components
These assemblies provide the interaction with UIA, as well as layers that allow the application to interact with that information in a more structured manner:

Assembly | Responsibility
--- | ---
Axe.Windows.Actions | Provide a high-level set of Actions that are the primary interface into the Runtime components.
Axe.Windows.Core | Provide data abstractions which represent accessibility data in a platform-agnostic way.
Axe.Windows.Desktop | Provide platform-specific (Windows) implementations of the platform-agnostic data abstractions. The low-level interactions with UIA occur in this assembly.
Axe.Windows.Telemetry | Provides an interface which any caller can provide to capture telemetry from Axe.Windows
AccessibilityInsights.Win32 | Provide a wrapper around Win32-specific code that is needed by other assemblies.

#### Accessibility Rules
These assemblies evaluate the accessibility of an application based upon the data exposed via the platform-agnostic abstractions. Please visit the [Rules Overview](./RulesOverview.md) for a detailed description of the automated accessibility tests.

Assembly | Responsibility
--- | ---
Axe.Windows.Rules | Provide a library of rules, each of which scans the platform-agnostic information for issues that are likely to be problematic. For example, a button without an accessible label will be flagged as an error.
Axe.Windows.RulesSelection | Coordinate rule execution in a consistent and reproducible way.

#### Application Entry Points
These assemblies allow user interaction with the Runtime components and the Accessibility Rules.

Assembly | Responsibility
--- | ---
AccessibilityInsights | Provide the UI for most users. This application is built using WPF.
AccessibilityInsights.CommonUxComponents | Provide non-specialized visual elements used by the main app and extensions. This allows the main app and extensions to share ux components that are unrelated to the runtime.
AccessibilityInsights.DesktopUI | Provide platform-specific (Windows) implementations of platform-agnostic UI abstractions. This includes things like element highlighters.
AccessibilityInsights.Extensions | Provide extension points that allow certain non-core functionality to be implemented in a loosely coupled way.
AccessibilityInsights.SharedUx | Provide visual elements used by the main app. This code is in a separate assembly for historical reasons.
AccessibilityInsights.WebApiHost | Provide a local service that exposes scanning functionality on locally running applications.
Axe.Windows.Automation | Provide a layer that wraps key actions behind a simplified interface. This layer can then be used either from a .NET application or from PowerShell scripts.

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
The packaging projects exist to gather assemblies into their shipping vehicles:

Project | Responsibility
--- | ---
MSI | Builds the MSI file used by most users.
AxeWindows-CI | Builds the NuGet package that will be used by users who wish to scan via automation.

#### Tests
Unit tests are built using a combination of Moq and Microsoft Fakes. The folllowing assemblies exist for testing purposes:
- AccessibilityInsights.Extensions.AzureDevOpsTests
- AccessibilityInsights.Extensions.GitHubAutoUpdateUnitTests
- AccessibilityInsights.Extensions.GitHubUnitTests
- AccessibilityInsights.Extensions.TelemetryTests
- AccessibilityInsights.ExtensionsTests
- AccessibilityInsights.Fakes.Prebuild
- AccessibilityInsights.SetupLibraryUnitTests
- AccessibilityInsights.SharedUxTests
- AccessibilityInsights.WebApiHostTests
- Axe.Windows.ActionsTests
- Axe.Windows.AutomationTests
- Axe.Windows.CoreTests
- Axe.Windows.DesktopTests
- Axe.Windows.RuleSelectionTests
- Axe.Windows.RulesTest
- Axe.Windows.UnitTestSharedLibrary
- Axe.Windows.Win32Tests 
