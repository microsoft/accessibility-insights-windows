## Telemetry Details

This document provides detailed information about telemetry events. It reflects the current state of the code and will be updated as telemetry is modified. Telemetry events consist of a set of common properties, as well as event-specific properties. This section outlines specifics of each:

### Common Data Fields
Common data is included with every event in the pipeline. Some of the data is pipeline-assigned, and some is app-assigned. The following tables outline this data

#### Pipeline Data
The following fields are set in the pipeline and are accessible for queries:

Name | Description | Event Sample | Exception Sample
--- | --- | --- | ---
timestamp | The UTC time that the event occurred | 2019-06-21T06:23:05.435691Z | 2019-06-20T15:16:56.334167Z
itemType | The type of event (used for routing) | customEvent | exception
customDimensions | User-defined data. See Context Properties
client_Type |Identifies the machine type (constant value) | PC | PC
client_OS | App-specified value to represent the client OS version. See Values for client_OS for details		
client_IP | Identifies client IP, always scrubbed to 0.0.0.0 | 0.0.0.0 | 0.0.0.0
client_City | Client city based on IP lookup in pipeline | Hyderabad | Redmond
client_StateOrProvince | Client State/Province based on IP lookup in pipeline | Telangana | Washington
client_CountryOrRegion | Client County/Region based on IP lookup in pipeline | India | United States
client_RoleInstance | App-specified value, not used | undefined |undefined
appId | Pipeline-assigned ID (constant value)
appName | Pipeline-provided name 
iKey | Instrumentation Key (pipeline-assigned Guid that is then specified in the code)
sdkVersion | Version of the ApplicationInsights SDK | dotnet:2.8.1-22898 | dotnet:2.8.1-22898
itemId | Pipeline-assigned Guid for this item | 19d56b2d-93ed-11e9-a1bb-bfc5b1450d77 | 7ae05a80-936e-11e9-9f2f-5b82d7bb4b57
itemCount | The count of items in the event (always 1) | 1 | 1

#### Values for client_OS
The Windows version is fetched directly from the registry. The CurrentVersion and CurrentBuildVersion registry values from the "HKLM\SOFTWARE\Microsoft\Windows NT" key are combined into a single string format. The current mappings (based on a blend of https://en.wikipedia.org/wiki/Windows_10_version_history and http://www.jrsoftware.org/ishelp/index.php?topic=winvernotes) include:

Value | OS Version
--- | ---
6.1.7601 | Windows 7 SP1 / Server 2008 R2 with Service Pack 1
6.2.9200 | Windows 8 / Server 2012
6.3.9600 | Windows 8.1 / Server 2012 R2
6.3.10240 | Windows 10 (1507)
6.3.10586 | Windows 10 (1511)
6.3.14393 | Windows 10 (1607) / Server 2016
6.3.15063 | Windows 10 (1703)
6.3.16299 | Windows 10 (1709)
6.3.17134 | Windows 10 (1803)
6.3.17763 | Windows 10 (1809) / Server 2019
6.3.18362 | Windows 10 (1903)
6.3.18363 | Windows 10 (1909)
6.3.19041 | Windows 10 (2004)
6.3.19042 | Windows 10 (20H2)

Builds on internal previews may also be in the data with version numbers greater than what appear in the table

#### Fixed properties within an app session
The following context properties remain fixed throughout the lifetime of the process:

Name | Description | Sample
--- | --- | ---
customDimensions.InstallationID | An installation-specific Guid that remains consistent within a calendar month but rotates between months
customDimensions.Version | The app version with no leading zeros | 1.1.1467.1
customDimensions.AppSessionID | A Guid that remains consistent for that app's process session
customDimensions.SessionType | Always "Desktop" | Desktop
customDimensions.ReleaseChannel | The client's Release Channel (`Production`, `Insider`, or `Canary`) | Production

#### Dynamic properties within an app session
The following context properties can vary during the lifetime of the process, depending on user actions

Name | Description
--- | ---
customDimensions.View | The app's view. Current values (`Live`, `CapturingData`, `Recording`, `TabStop`, or `ElementHowToFix`)
customDimensions.UIFramework | customDimension.UIFramework identifier | 
customDimensions.ModeName | The app's page. Current values are Start, Test, Inspect, CCA, Events, and Exit.
customDimensions.ModeSessionId | A Guid that gets generated every time the page mode changes

#### customDimension.UIFramework identifiers
These values come from 2 sources--some are from Axe.Windows, and others are dynamically reported by application frameworks

##### Values from Axe.Windows
Value | Meaning
--- | ---
MicrosoftEdge | Microsoft Edge (non-Chromium version)
InternetExplorer | Internet Explorer
WPF | WPF Apps
WinForm | Windows Forms Apps
Win32 | Win32 apps
XAML | WindowsUI apps

##### Values from application frameworks
The following framework have also been observed
Value | Meaning
--- | ---
Chrome | Chromium (includes Google Chrome and newer versios of Microsoft Edge)
DirectUI | DirectUI apps
Gecko | Undetermined
Qt | Undetermined
SWT | Undetermined
Avalonia | Undetermined

### Application-defined Events
Telemetry events are queried from the customEvents table. All events follow the same pattern. Each event contains 3 types of data:
- Common Data Fields
- A name property that identifies the specific telemetry event being reported
- An optional set of event-specific key/value pairs called properties (detailed with each event)

#### ColorContrast_Click_Dropdown
Meaning: The user has opened a color picker popup in the Color Contrast view  
Additional properties: None

#### ColorContrast_Click_Eyedropper
Meaning: The user has clicked on an eyedropper in the Color Contrast view  
Additional properties: None

#### ColorContrast_Click_HexChange
Meaning: The user has entered a value in a hex dialog in the Color Contrast view  
Additional properties: None

#### Event_Load
Meaning: The user has successfully opened a previously-saved A11yEvents file  
Additional properties: None

#### Event_Save
Meaning: The user has successfully saved an A11yEvents file  
Additional properties: None

#### Event_Start_Record
Meaning: The user has begun event recording  
Additional properties: None

#### Hierarchy_Load_NewFormat
Meaning: The user has successfully opened an A11yTest file  
Additional properties:
Name | Value
--- | ---
customDimensions.FileMode | Indicates the app mode when the file was opened. Valid values: `Contrast`,  `Inspect`, or `Test`

#### Hierarchy_Save
Meaning: The user has successfully saved an A11yTest file  
Additional properties: None

#### Hilighter_Expand_AllDescendants
Meaning: The user has clicked on the "Expand all descendants" context menu in test mode hierarchy  
Additional properties: None

#### Issue_File_Attempt
Meaning: The user has completed the "File Issue" experience, but with no confirmed success (could be fire-and-forget or it could be a network failure)
Additional properties: 
Name | Value
--- | ---
customDimensions.IssueReporter | The ServiceName property of the associated issue reporter. Current values are `Azure Boards` for ADO, or `GitHub` for GitHub

#### Issue_Save
Meaning: The user has completed the "File Issue" experience, with confirmed success  
Additional properties: 
Name | Value
--- | ---
customDimensions.IssueReporter | The ServiceName property of the associated issue reporter. Current values are Azure Boards for ADO, or GitHub for GitHub
customDimension.RuleId | Present only if axe-windows flagged an issue. Reports the rule flagged by axe-windows
customDimension.UIFramework | Present only if axe-windows flagged an issue. Reports the UI Framework of the UI Automation element flagged by axe-windows

