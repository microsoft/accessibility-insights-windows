# Overview of Accessibility Insights for Windows

## Tool Overview
Windows UI Automation (UIA) is the platform-provided way for accessibility tools to interact with programs. A fully accessible program is a program whose full functionality can be accessed exclusively through the UIA-provided mechanism. AccessibilityInsights for Windows uses the UIA-provided information to scan programs for accessibility issues. The basic units of UIA are:

- Hierarchy: The entire surface of all interactive applications is represented as a tree of Elements.
- Elements: Each Element may optionally have any combination of Properties, Patterns, and child Elements.
- Properties: Each Property is a Key/Value pair that identifies some aspect of the Element. The key is the property type (for example, IsVisible) and the value is the state of that property (for example, true or false).
- Patterns: There are several patterns that map to functionality that is exposed via the UI. For example, a list element will typically support a pattern that exposes all of the items in the list, as well as the ability to learn which item is current selected, and the ability to select a different element, if desired.

Accessibilty Insights will take the data provided by UIA, then will compare that data to a set of rules that identify cases where that data would create issues for any users who relied upon UIA to interact with the application. The results of these rules are then displayed in an environment that supports exploration of the data in its appropriate context. The results can be saved to disk for later use or analysis. If these files are later loaded into Accessibility Insights, the rich exploration environment is re-created.

## Code Organization
The code is organized into the following general areas:

### Runtime components
These assemblies provide the interaction with UIA, as well as layers that allow the application to interact with that information in a more structured manner:

Assembly | Responsibilty
--- | ---
AccessibilityInsights.Core | Provide data abstractions that are intended to represent UIA in a platform-agnostic manner.
AccessibilityInsights.Desktop | Provide platform-specific (Windows) implementations of the platform-agnostic data abstractions. The low-level interactions with UIA occur in this assembly.
AccessibilityInsights.DesktopUI | Provide platform-specific (Windows) implementations of platform-agnostic UI abstractions. This includes things like element highlighters.
AccessibilityInsights.Actions | Provide a high-level set of Actions that are the primary interface into the Runtime components.
AccessibilityInsights.Extensions | Provide extension points that allow certain non-core functionality to be implemented in a loosely coupled way.
AccessibilityInsights.Win32 | Provide a wrapper around Win32-specific code that is needed by other assemblies.

### Accessibility Rules
These assemblies evaluate the accessibility of an application based upon the data exposed via the platform-agnostic abstractions.

Assembly | Responsibility
--- | ---
AccessibilityInsights.Rules | Provide a library of rules, each of which scans the platform-agnostic information for issues that are likely to be problematic. For example, a button without an accessible label will be flagged as an error.
AccessibilityInsights.RulesSelection | Select the appropriate set of rules to run on a specific application, then coordinate their execution in a consistent and reproducible way.

### Application Entry Points
These assemblies allow user interaction with the Runtime components and the Accessibility Rules.

Assembly | Responsibility
--- | ---
AccessibilityInsights | Provide the UI for most users. This application is built using WPF.
AccessibilityInsights.SharedUx | Provide visual elements used by the main app. This code is in a separate assembly for historical reasons.
AccessibiltyInsights.WebApiHost | Provide a local service that exposes scanning functionality on locally running applications.
AccessibilityAutomations.Automation | Provide a layer that wraps key actions behind a simplified interface. This layer can then be used either from a .NET application or from PowerShell scripts.

### Extensions
Extensions are intended to allow loose coupling of non-core code. They build upon [Managed Extensibility Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/). At the moment, extensions provide the following capabilities:

- Telemetry : Send information about product usage. **Limited** diagnostic data is also sent to help identify trends in user issues.
- Updates : Provide a centralized control mechanism to notify users of updated releases.
- Bug Filing : Enable users to file accessibility bugs directly from within the product.

The following extensions exist:

Assembly | Responsibility
--- | ---
AccessibilityInsights.Extensions.AutoUpdate | Implement a file-based update mechanism. Can only be used when all users are guaranteed to have access to the file share.
AccessibilityInsights.Extensions.GitHubAutoUpdate | Implement a web-based update mechanism. Requires that the files be publicly accessible via a Uri.
AccessibilityInsights.Extensions.AzureDevOps | Implement bug filing support using AzureDevOps.
AccessibilityInsights.Extensions.Telemetry | Implement simple telemetry built upon [Microsoft ApplicationInsights](https://www.nuget.org/packages/Microsoft.ApplicationInsights).

### Packaging
The packaging projects exist to gather assemblies into their shipping vehicles:

Project | Responsibility
--- | ---
MSI | Builds the MSI file that will be used by most users. This requires the Wix tools from http://wixtoolset.org.
ApplicationInsights-CI | Builds the NuGet package that will be used by users who wish to scan via automation.

### Tests
Unit tests are built using a combination of Moq and Microsoft Fakes. The folllowing assemblies exist for testing purposes:
- AccessibilityInsights.ActionsTests
- AccessibilityInsights.AutomationTests
- AccessibilityInsights.CoreTests
- AccessibilityInsights.DesktopTests
- AccessibilityInsights.Extensions.AzureDevOpsTests
- AccessibilityInsights.Extensions.GitHubAutoUpdateUnitTests
- AccessibilityInsights.Extensions.TelemetryTests
- AccessibilityInsights.ExtensionsTests
- AccessibilityInsights.Fakes.Prebuild
- AccessibilityInsights.RuleSelectionTests
- AccessibilityInsights.RulesTest
- AccessibilityInsights.SharedUxTests
- AccessibilityInsights.WebApiHostTests
- AccessibilityInsights.Win32Tests 
