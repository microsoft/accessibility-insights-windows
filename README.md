<!-- Copyright (c) Microsoft Corporation. All rights reserved.
     Licensed under the MIT License. -->
# Accessibility Insights for Windows

[![Build Status](https://dev.azure.com/mseng/AzureDevOps/_apis/build/status/Accessibility%20Insights%20for%20Windows%20Signed?branchName=master)](https://dev.azure.com/mseng/AzureDevOps/_build/latest?definitionId=7909&branchName=master)

![Product Logo](./brand/brand-blue-128px.png)


Accessibility Insights for Windows is the project for Accessibility tools on Windows platform (Win7/Win8x/Win10).

## Installing Accessibility Insights for Windows
You can install the application from TBA.

## Building the code
### 1. Clone the repository
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

### 2. Open the solution in visual studio
- Use the `src/AccessibilityInsights.sln` file to open the solution.

### 3. Build and run unit tests
Note: The MSI project requires WIX. Download and install the Wix toolset from http://wixtoolset.org/. 
## More Information
  Visit the [Overview of Accessibility Insights for Windows](./docs/Overview.md) page.

## Testing
We use the unit test framework from Visual Studio. Find more information in our [FAQ section](docs/FAQ.md).

## Contributing
All contributions are welcome! Please read through our guidelines on [contributions](Contributing.md) to this project.

## FAQ
Please visit our [FAQ section](docs/FAQ.md) to get answers to common questions.
