## Changing version

This document provides a detailed description of the built-in mechanisms for changing versions. There are 2 scenarios that cause a version change:
1. [New versions being released](#new-versions-being-released)
2. [Changing channels](#changing-channels)

### New versions being released
This is the most common scenario that leads to a version change. The process works as follows:
- Accessibility Insights for Windows starts.
- It checks the result from [GitHubAutoUpdate](#githubautoupdate) to determines if an optional or a required update exists.
- It prompts the user to install any available updates.
- If the user chooses to install an update, it invokes the [VersionSwitcher](#versionswitcher) to install the new version.
- It exits the application after the VersionSwitcher has successfully launched with administrative privilege.
- It includes telemetry results written by the VersionSwitcher on its next boot.

### Changing channels
This scenario occurs after the user changes the release channel from the Settings tab. The application does the following:
- It makes a web call to retrieve the update manifest for the newly selected release channel.
- It checks the update manifest's digital signature to prevent tampering or corruption of the file.
- It invokes the [VersionSwitcher](#versionswitcher) to install the new version and change the release channel.
- It exits the application after the VersionSwitcher has successfully launched with administrative privilege.
- It includes telemetry results written by the VersionSwitcher on its next boot.

### Subsystems

#### GitHubAutoUpdate
`ApplicationInsights.Extensions.GitHubAutoUpdate.dll` does the following:
- It reads the registry to retrieve the currently installed version of Accessibility Insights for Windows.
- It makes a web call to retrieve the update manifest for the currently selected release channel.
- It checks the update manifest's digital signature to prevent tampering or corruption of the file.
- It extracts the update information from the manifest.
- It compares the installed application version against the update information. Depending on this comparison, one of the following statuses is returned:
  - The installed version is the most recent version. No update exists.
  - The installed version is supported but not the most recent version. An optional update exists.
  - The installed version is no longer supported. A required update exists.

#### VersionSwitcher
`AccessibilityInsights.VersionSwitcher.exe` and its required assemblies are included in the installation of Accessibility Insights for Windows. It provides a secure mechanism to change application versions. Its operation consists of two steps:
1. The contents of the `VersionSwitcher` folder are securely copied from its location under `%ProgramFiles(x86)%` to a temporary location. This is necessary because the MSI installer will modify the files under `%ProgramFiles(x86)%`. 
2. The copy of `AccessibilityInsights.VersionSwitcher.exe` is started with appropriate parameters.
3. `AccessibilityInsights.exe` waits for the copied binary to successfully initialize, then exits.
3. The copied binary does the following:
  - It obtains permission to run with administrative privileges. On most Windows systems, this means that the LUA dialog is displayed.
  - It reads the following command line parameters:
    - Where to load the MSI installer for the new version.
    - The expected size of the MSI installer.
    - The expected SHA512 for the contents of the MSI installer.
    - The newly selected release channel -- this is only specified in the [Changing channels](#changing-channels) scenario.
  - It retrieves the new MSI installer from its specified location.
  - It uses the provided MSI size and SHA512, along with a check of the MSI's digital signature, to prevent tampering or corruption of the MSI installer.
  - It transactionally runs the MSI installer to change versions, reverting to the previously installed version on error.
  - If a new release channel was specified, it updates the application's configuration file to reflect the new selection.
  - It writes its execution history to disk (see [Telemetry from upgrades](#telemetry-from-upgrades) for more details.
  - It re-launches the Accessibility Insights for Windows.

#### SetupLibrary
The `AccessibilityInsights.SetupLibrary` assembly contains most of the classes that are shared between `AccessibilityInsights.exe`, `AccessibilityInsights.Extensions.GitHubAutoUpdate.dll`, and `AccessibilityInsights.VersionSwitcher.exe`. This includes classes that:
- Retrieve files from the web.
- Validate digital signatures.
- Compute SHA512 values.
- Define data that gets shared between the different processes and workflows.

#### Telemetry from upgrades
`AccessibilityInsights.VersionSwitcher.exe` contains no direct telemetry signal. Its telemetry is captured as follows:
- During execution, it constructs an `ExecutionHistory` object.
- Before exiting, it serializes the `ExecutionHistory` object to disk, using `%temp%\AccessibilityInsights.VersionSwitcher.ExecutionHistory.json` as the output file. If the file already exists, it gets overwritten.
- When `AccessibilityInsights.exe` launches, it checks for the presence of this file. If it finds it, it uploads portions of the data as telemetry. It intentionally excludes the `LocalDetails` data, which is provided for local debugging and troubleshooting of upgrade failures.
- After the telemetry has been created, `%temp%\AccessibilityInsights.VersionSwitcher.ExecutionHistory.json` is deleted.

For more specific details of the generated telemetry, please refer to [Upgrade_VersionSwitcherResults](TelemetryDetails.md/#upgrade_versionswitcherresults).
