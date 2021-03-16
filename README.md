<!-- Copyright (c) Microsoft Corporation. All rights reserved.
     Licensed under the MIT License. -->
## ![Product Logo](./brand/brand-blue-48px.png) Accessibility Insights for Windows

[![Build Status](https://dev.azure.com/accessibility-insights/Accessibility%20Insights/_apis/build/status/Microsoft.accessibility-insights-windows-CI?branchName=main)](https://dev.azure.com/accessibility-insights/Accessibility%20Insights/_build/latest?definitionId=3&branchName=main)
[![Downloads](https://img.shields.io/github/downloads/microsoft/accessibility-insights-windows/total.svg)](https://github.com/Microsoft/accessibility-insights-windows/releases/latest)
[![Release](https://img.shields.io/github/release/microsoft/accessibility-insights-windows.svg)](https://github.com/Microsoft/accessibility-insights-windows/releases/latest)

Accessibility Insights for Windows is the project for Accessibility tools on the Windows platform.

### Installing Accessibility Insights for Windows
You can download and install the application from https://accessibilityinsights.io.

### Building the code
You can find more information on how to set up your development environment [here](./docs/SetUpDevEnv.md).

#### 1. Clone the repository
- Clone the repository using one of the following commands
  ``` bash
  git clone https://github.com/Microsoft/accessibility-insights-windows.git
  ```
  or
  ``` bash
  git clone git@github.com:Microsoft/accessibility-insights-windows.git
  ```
- Select the created directory
  ``` bash
  cd accessibility-insights-windows
  ```

#### 2. Open the solution in visual studio
- Use the `src/AccessibilityInsights.sln` file to open the solution.

#### 3. Build and run unit tests

#### 4. Build the MSI project
The MSI project is used to package all the code into an installer that can be used to install Accessibility Insights for Windows. This is not a required step. By default the MSI project is not built in either configuration.
Note: The MSI can only be built in the `Release` config.
- The MSI project requires WIX. Download and install the Wix toolset from http://wixtoolset.org/.
- To build the MSI locally, set build configuration to `Release`.
- Find the MSI project in the solution explorer under the `packages` folder.
- Right click on the MSI project and select `Build`.
- Once the build is complete, the MSI can be found at `\src\MSI\bin\Release`.

### More Information
Visit the [Overview of Accessibility Insights for Windows](./docs/Overview.md) page.

### Testing
We use the unit test framework from Visual Studio. Find more information in our [FAQ section](./docs/FAQ.md).

## Contributing
All contributions are welcome! Please read through our guidelines on [contributions](./Contributing.md) to this project.

## FAQ
Please visit our [FAQ section](./docs/FAQ.md) to get answers to common questions.

## Data/Telemetry
By opting into telemetry, you [help the community](https://go.microsoft.com/fwlink/?linkid=2077765) develop inclusive software. We collect anonymized data to identify the top accessibility issues found by users. This will help focus the accessibility tools and standards community to improve guidelines, rules engines, and features.

This project collects usage data and sends it to Microsoft to help improve our products and services. Read our [privacy statement](https://privacy.microsoft.com/en-us/privacystatement) to learn more.

## Reporting security vulnerabilities
If you believe you have found a security vulnerability in this project, please follow [these steps](https://technet.microsoft.com/en-us/security/ff852094.aspx) to report it. For more information on how vulnerabilities are disclosed, see [Coordinated Vulnerability Disclosure](https://technet.microsoft.com/en-us/security/dn467923).
