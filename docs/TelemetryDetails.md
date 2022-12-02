## Telemetry Details

This document provides detailed information about telemetry events. It reflects the current state of the code and will be updated as telemetry is modified. Telemetry Events come in 2 types:
- [Application Events](#application-events) identify typical application usage (start the app, run tests, save a file, etc.) in ways that allow evaluation of how the app is being used, but without including any user-identifiable data.
- [Exception Events](#exception-events) identify application errors and provide visibility into problems that users are experiencing, again without including any user-identifiable data.

### Application Events
Application events are queried from the `customEvents` table. All application events follow the same pattern and contain 3 types of data:
- The [Common Data Properties](#common-data-properties).
- A `name` property that matches one of the events defined below.
- An optional set of event-specific properties, as defined with each event.

#### ColorContrast_AutoDetect
Trigger: Automatic contrast detection is run.
Additional properties:
Name | Value
--- | ---
`Confidence` | The confidence of the analysis. One of `High`, `Mid`, `Low`, or `None`.
`BitmapSize` | The size in pixels (width times height) of the bitmap being processed.

#### ColorContrast_Click_Autodetect_Toggle
Trigger: The user clicks the "Auto detect contrast ratio" button in the color contrast view.
Additional properties:
Name | Value
--- | ---
`IsNowEnabled` | one of `true` or `false` indicating whether automatic detection is enabled.

#### ColorContrast_Click_Dropdown
Trigger: The user opens a color picker popup in the Color Contrast view.  
Additional properties: None.

#### ColorContrast_Click_Eyedropper
Trigger: The user clicks on an eyedropper in the Color Contrast view.  
Additional properties: None.

#### ColorContrast_Click_HexChange
Trigger: The user enters a value in a hex dialog in the Color Contrast view.  
Additional properties: None.

#### Custom_UIA
Trigger: The application was configured with a `CustomUIA.json` file as described in the [online documentation](https://accessibilityinsights.io/docs/en/windows/reference/faq/#does-accessibility-insights-for-windows-support-custom-ui-automation-properties). This event is sent _only_ if a valid `Custom_UIA.json` exists.
Additional properties:
Name | Value 
--- | ---
`CustomUIAPropertyCount` | The count of custom UIA properties that were defined in the `CustomUIA.json` file.

#### Event_Load
Trigger: The user successfully opens a previously-saved A11yEvents file.  
Additional properties: None.

#### Event_Save
Trigger: The user successfully saves an A11yEvents file.  
Additional properties: None.

#### Event_Start_Record
Trigger: The user begins event recording.  
Additional properties: None.

#### Hierarchy_Load_NewFormat
Trigger: The user successfully opens an A11yTest file.  
Additional properties:
Name | Value
--- | ---
`customDimensions.FileMode` | Indicates the app mode when the file was opened. Current values: `Contrast`, `Inspect`, or `Test`.

#### Hierarchy_Save
Trigger: The user successfully saves an A11yTest file.  
Additional properties: None.

#### Hilighter_Expand_AllDescendants
Trigger: The user clicks on the "Expand all descendants" context menu in the test mode hierarchy.  
Additional properties: None.

#### Issue_File_Attempt
Trigger: The user attempts the "File Issue" experience, but with no confirmed success (could be fire-and-forget or it could be a network failure).  
Additional properties: 
Name | Value
--- | ---
`customDimensions.IssueReporter` | The ServiceName property of the associated issue reporter. Current values are `Azure Boards` for ADO or `GitHub` for GitHub.

#### Issue_Save
Trigger: The user completes the "File Issue" experience, with confirmed success.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.IssueReporter` | The ServiceName property of the associated issue reporter. Current values are `Azure Boards` for ADO or `GitHub` for GitHub.
`customDimension.RuleId` | *Present only if Axe.Windows flagged an issue*. Reports the rule flagged by Axe.Windows.
`customDimension.UIFramework` | *Present only if Axe.Windows flagged an issue*. Reports the UI Framework of the UI Automation element flagged by Axe.Windows.

#### Mainwindow_Startup
Trigger: The user starts the application.  
Additional properties:
Name | Value
--- | ---
`customDimensions.UIAccessEnabled` | `True` if the user has explicitly enabled UIAccess, otherwise `False`. 
`customDimensions.InstalledDotNetFrameworkVersion` | The numeric version of the installed .NET Framework version. If this value is 528040 or greater, then .NET Framework 4.8 is installed.
`customDimensions.OsArchitecture` | The architecture of the current Windows platform. Supported values: `x86` or `x64`

#### Mainwindow_Timer_Started
Trigger: The user uses the "Scan with Timer" feature with a non-default value. *Note that this does not send data if the default value is used*.   
Additional properties: 
Name | Value
--- | ---
`customDimensions.Seconds` | The number of seconds specified by the user.

#### Pattern_Invoke_Action
Trigger: The user invokes a pattern through the patterns UI.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.PatternMethod` | The pattern method that was invoked.

#### ReleaseChannel_ChangeConsidered
Trigger: The user selects a new channel and is being shown the dialog to confirm the change.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.ReleaseChannel` | The user's existing ReleaseChannel.
`customDimensions.ReleaseChannelConsidered` | The user's considered selection for ReleaseChannel.

#### Scan_File_Bug
Trigger: The user begins the process of filing a bug.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.By` | The source of the bug filing. Current values are `HowtoFix`, `Hierarchy`, or `AutomatedChecks`.
`customDimensions.IsAlreadyLoggedIn` | `True` if the IssueReporter has all needed config information, otherwise `False`.
`customDimensions.IssueReporter` | The ServiceName property of the associated issue reporter. Current values are `Azure Boards` for ADO or `GitHub` for GitHub.

#### TabStop_Record_On
Trigger: The user begins to record tab stops.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.Scope` | The scope of the selection. Current values are `App` or `Element`.

#### TabStop_Select_Records
Trigger: The user selects one or more tab stops that have been recorded.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.By` | The count of selected items.

#### TestSelection_Set_Scope
Trigger: The user sets the selection scope via the command bar.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.Scope` | The scope of the selection. Current values are `App` or `Element`.

#### Test_Requested
Trigger: The user triggers an automated scan.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.By` | The trigger mechanism. Current values are `HotKey`, `Beaker`, `HierarchyMode`, or `Timer`.
`customDimensions.Scope` | The scope of the selection. Current values are `App` or `Element`.

#### Upgrade_DoInstallation
Trigger: The user completes an install from the Upgrade dialog.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.UpdateInstallerUpdateTime` | The time it took to download the new MSI, validate it, and launch the VersionSwitcher. Does not include the execution time of the VersionSwitcher. Format is Hours:Minutes:Seconds.FractionalSeconds.
`customDimensions.UpdateResult` | The result of the operation. Current values are `Unknown`, `Success`, or `RequiredUpgrade`.

#### Upgrade_GetUpgradeOption
Trigger: The application checks for upgrade at application startup.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.UpdateInitializationTime` | The updater's measurement of how long it took to determine the UpdateOption. Format is Hours:Minutes:Seconds.FractionalSeconds.
`customDimensions.UpdateManifestRequestUri` | The URI used to request the update manifest
`customDimensions.UpdateManifestResponseUri` | The URI the ultimately provided the update manifest
`customDimensions.UpdateManifestSizeInBytes` | The byte count of the retrieved update manifest
`customDimensions.UpdateOptionWaitTime` | The app's measurement of how long it took to determine the UpdateOption. It is different from UpdateInitializationTime, since they start at different times on different threads. Format is Hours:Minutes:Seconds.FractionalSeconds.
`customDimensions.UpdateOption` | The UpdateOption that was returned to the app (will be ignored if we timed out).
`customDimensions.UpdateTimedOut` | `True` if the AutoUpdate process exceeded the 2 second timeout, otherwise `False`.

#### Upgrade_InstallationError
Trigger: The user triggers an install from the Upgrade dialog, and an error occurred.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.Error` | The UpdateResult, which is always `Unknown`. *Note: Should this provide more useful information?*

#### Upgrade_Update_Dismiss
Trigger: The user presses the "Later" button in the Upgrade dialog.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.MSIVersion` | The version of the previously installed client. *note: We already have the old version, should this log the new version instead?*

#### Upgrade_Update_ReleaseNote
Trigger: The user presses the "Release Notes" button from the Upgrade dialog.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.Error` | The error from clicking the release notes button *note: Should this include the Exception type and/or the Uri to the release notes?*

#### Upgrade_VersionSwitcherResults
Trigger: The application starts up after running `AccessibilityInsights.VersionSwitcher.exe` to either upgrade or change release channels
Additional properties: 
Name | Value
--- | ---
`customDimensions.ActualMsiSha512` | The _actual_ SHA512 of the MSI file
`customDimensions.ActualMsiSize` | The _actual_ size (in bytes) of the downladed MSI file
`customDimensions.ExecutionTimeInMilliseconds` | The number of milliseconds spent running the VersionSwitcher
`customDimensions.ExpectedMsiSha512` | The _expected_ SHA512 of the MSI file. Will be set to `null` if the SHA512 was unknown
`customDimensions.ExpectedMsiSize` | The _expected_ size (in bytes) of the MSI file. Will be set to 0 if the size was unknown
`customDimensions.NewChannel` | The new channel if VersionSwitcher was used to change channels. Supported values are `Production`, `Insider`, or `Canary` if the channel was changed, or `null` if this was an upgrade within the same channel
`customDimensions.RequestedMsi` | The `Uri` used to request the MSI file
`customDimensions.ResolvedMsi` | The final `Uri` (after all redirects) that identifies where the source of the MSI file
`customDimensions.Result` | The result of the operation. Supported values: `Unknown`, `ErrorBadCommandLine`, `ErrorMsiDownloadFailed`, `ErrorMsiBadSignature`, `ErrorMsiSizeMismatch`, `ErrorMsiSha512Mismatch`, `ErrorInstallingMsi`, or `Success`.
`customDimensions.StartingVersion` | The installed version of Accessibility Insights for Windows before the version switch

### Events from Axe.Windows
Accessibility Insights for Windows provides a mechanism by which Axe.Windows is able to provide telemetry that then gets merged into the telemetry stream that already exists for the application. These events inherit all of the [Common Data Properties](#common-data-properties), and appear just like events that originate from Accessibility Insights for Windows. These will be documented in the Axe.Windows repo, but are duplicated here for convenience:

#### Scan_Statistics
Trigger: An automated scan completes.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.ElementsInScan` | The number of elements included in the scan.
`customDimensions.UpperBoundExceeded` | `True` if ElementsInScan exceeds our upper bound of 20,000, otherwise `False`.
`customDimensions.ScanDurationInMilliseconds` | The time (measured in milliseconds) spent running the scan.

#### SingleRule_Tested_Results
Trigger: A single rule runs on all elements within a scan.  
Additional properties: 
Name | Value
--- | ---
`customDimensions.TestResults` | The JSON-serialized data summarizing the results of this rule (where it was run and the results it produced). Contains a RuleId and an array of Result objects. Each result object contains a ControlType, a UIFramework, an optional Fail result, and an optional Pass Result. Pass and Fail results can appear in any order within the object.

Here's an expanded example of the JSON-serialized object, containing a mixture of Pass and Fail results, to better visualize this data (*note that the data in the telemetry stream is **not** expanded*):

```
{
    "RuleId":"NameNotNull",
    "Results":[
        {
            "ControlType":"Text",
            "UIFramework":"Win32",
            "Fail":"2",
            "Pass":"7"
        },
        {
            "ControlType":"Button",
            "UIFramework":"Win32",
            "Pass":"2",
            "Fail":"1"
        },
        {
            "ControlType":"TabItem",
            "UIFramework":"Win32",
            "Pass":"4"
        },
        {
            "ControlType":"Hyperlink",
            "UIFramework":"Win32",
            "Pass":"1"
        }
    ]
}
```		

### Exception Events
Exception events are queried from the `exceptions` table. All exception events follow the same pattern and contains 2 types of data:
- The [Common Data Properties](#common-data-properties).
- [Exception-Specific Properties](#exception-specific-properties).

#### Exception-Specific Properties
These properties exist only for exceptions:

Name | Description | Sample
--- | --- | --- 
`type` | The .NET type of the innermost Exception that was thrown. | `System.InvalidOperationException`
`assembly` | Identifies the assembly where the outermost Exception was caught. | `AccessibilityInsights.SharedUx, Version=1.1.899.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`
`method` | Identifies the method where the outermost Exception was caught. | `AccessibilityInsights.SharedUx.Controls.HierarchyControl+<>c__DisplayClass55_0.<OnSelected>b__0`
`innermostType` | The .NET type of the innermost Exception that was thrown. *note: This property exists only if the innermost Exception was wrapped by another Exception.* | 
`innermostMessage` | The `Message` property of the innermost Exception that was thrown. *note: This property exists only in the innermost Exception was wrapped by another Exception.* | 
`outerType` | The .NET type of the outermost Exception that was thrown. This may be different from the `type` that was thrown by the innermost Exception. | `System.InvalidOperationException`
`outerMessage` | The `Message` property of the outermost Exception that was thrown. | `The specified Visual is not an ancestor of this Visual.`
`outerAssembly` | The assembly from which the outermost Exception was thrown. | `PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35`
`outerMethod` | The method from which the outermost Exception was thrown. | `System.Windows.Media.Visual.TrySimpleTransformToAncestor`
`problemId` | A concatenation of the innermost Exception type and the method from which the innermost Exception was thrown. | `System.InvalidOperationException at AccessibilityInsights.SharedUx.Controls.HierarchyControl+<>c__DisplayClass55_0.<OnSelected>b__0`
`details` | A JSON-serialized array of [ExceptionDetail objects](#exceptiondetail-objects) that correspond to this Exception. The first item in the array is always the outermost Exception. | `[{"parsedStack":[{"method":"System.Windows.Media.Visual.TrySimpleTransformToAncestor","level":0,"line":0,"assembly":"PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"},{"method":"System.Windows.Media.Visual.TransformToAncestor","level":1,"line":0,"assembly":"PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"},{"method":"AccessibilityInsights.SharedUx.Controls.HierarchyControl+<>c__DisplayClass55_0.<OnSelected>b__0","level":2,"line":0,"assembly":"AccessibilityInsights.SharedUx, Version=1.1.899.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"}],"outerId":"0","message":"The specified Visual is not an ancestor of this Visual.","type":"System.InvalidOperationException","id":"25819492"}]`

##### ExceptionDetail Objects
Each ExceptionDetail object represents a single Exception that was thrown, along with its stack trace. Exceptions can be nested, with each inner Exception pointing to its immediate wrapper Exception. The following properties are defined:

Name | Description | Sample
--- | --- | ---
`type` | The type of Exception that was thrown. | `System.InvalidOperationException`
`message` | The `Message` property specified when this Exception was thrown. | `The specified Visual is not an ancestor of this Visual.`
`id` | A system-assigned ID for this specific exception event. This is used to link the Exception chain. | *some id*
`outerId` | A system-assigned value that maps to the id or the immediate parent of this Exception. Will be 0 for the outermost Exception. | *some id*
`parsedStack` | A JSON-serialized array of [StackFrame Objects[(#stackframe-objects)] that identify where this Exception was thrown. Index 0 is the innermost frame, index 1 is the next innermost, etc. | `[{"method":"System.Windows.Media.Visual.TrySimpleTransformToAncestor","level":0,"line":0,"assembly":"PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"},{"method":"System.Windows.Media.Visual.TransformToAncestor","level":1,"line":0,"assembly":"PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"},{"method":"AccessibilityInsights.SharedUx.Controls.HierarchyControl+<>c__DisplayClass55_0.<OnSelected>b__0","level":2,"line":0,"assembly":"AccessibilityInsights.SharedUx, Version=1.1.899.1, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"}]`

##### StackFrame Objects
Each StackFrame object represents a frame on the exception stack with the 4 following properties:
Name | Description | Sample
--- | --- | ---
`level` | The count of stack frames from the innermost Exception. | `1`
`assembly` | The assembly of the indicated stack frame. | `PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35`
`method` | The method of the indicated stack frame. | `System.Windows.Media.Visual.TransformToAncestor`
`line` | If available, the line of the indicated stack frame (0 if unavailable). | `0`

### Common Data Properties
Common data properties are included with every telemetry event. Some of these properties are pipeline-controlled, and some are application-controlled. The following tables outline these properties.

#### Pipeline-controlled properties
The following properties are controlled by the telemetry pipeline and are useful for queries--note that this list is intentionally limited to those properties that are most likely to be useful for writing queries:

Name | Description | Event Sample | Exception Sample
--- | --- | --- | ---
`timestamp` | The UTC time that the event occurred. | `2019-06-21T06:23:05.435691Z` | `2019-06-20T15:16:56.334167Z`
`client_City` | Client city based on reverse IP lookup in pipeline. | `Hyderabad` | `Redmond`
`client_StateOrProvince` | Client State/Province based on reverse IP lookup in pipeline. | `Telangana` | `Washington`
`client_CountryOrRegion` | Client County/Region based on reverse IP lookup in pipeline. | `India` | `United States`

#### Application-controlled properties
The following properties are controlled by the application--note that this list is intentionally limited to those properties that are most likely to be useful for writing queries. Most of these properties are included in the `customProperties` field of the telemetry events. Some of these values are dynamic and may change during a session, as outlined below:

Name | Description | Dynamic Value? | Sample
--- | --- | --- | ---
`client_OS` | App-specified value to represent the client OS version. See [client_OS Values](#client_os-values) for details. | No
`customDimensions.Version` | The app version with no leading zeros. | No | `1.1.1467.1`
`customDimensions.ReleaseChannel` | The client's Release Channel. Current values are `Production`, `Insider`, or `Canary`. | No | `Production`
`customDimensions.AppSessionID` | A Guid that allows correlation within the process session. | No | *some guid*
`customDimensions.InstallationID` | A Guid that allows correlation of activity on a client within a calendar month. | No | *some guid*
`customDimensions.View` | The app's view. Current values are`Live`, `CapturingData`, `Recording`, `TabStop`, or `ElementHowToFix`. | Yes | `Live`
`customDimensions.UIFramework` | See [customDimension.UIFramework identifiers](#uiframework-identifiers) for details. | Yes
`customDimensions.ModeName` | The app's page. Current values are `Start`, `Test`, `Inspect`, `CCA`, `Events`, or `Exit`. | Yes | `Start`
`customDimensions.ModeSessionId` | A Guid that allows correlation within a specific mode change within a session. | Yes | *some guid*

##### client_OS Values
The Windows version is fetched directly from the registry. The CurrentVersion and CurrentBuildVersion registry values from the "HKLM\SOFTWARE\Microsoft\Windows NT" key are combined into a single string format. The current mappings (based on a blend of https://en.wikipedia.org/wiki/Windows_10_version_history, https://en.wikipedia.org/wiki/Windows_11_version_history, and http://www.jrsoftware.org/ishelp/index.php?topic=winvernotes) include:

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
6.3.19043 | Windows 10 (21H1)
6.3.19044 | Windows 10 (21H2)
6.3.20348 | Windows Server 2022 (21H2)
6.3.22000 | Windows 11 (21H2)

This table includes only officially published builds. The data in table may also include builds that were not officially published, such as insider or preview builds.

##### UIFramework identifiers
These identifiers are provided by UI frameworks and allow assistive technologies to provide framework-specific behaviors. This table reports the values (sorted alphabetically) that have appeared in telemetry, and their corresponding UI frameworks:

Value | Framework
--- | ---
`Avalonia` | Avalonia framework (.NET)
`Chrome` | Chromium (C++, includes Google Chrome and newer versions of Microsoft Edge)
`DirectUI` | DirectUI framework (C++)
`Gecko` | Gecko browser engine (C++, used in FireFox and Thunderbird)
`InternetExplorer` | Microsoft Internet Explorer
`JUCE` | Juce framework (C++)
`MicrosoftEdge` | Microsoft Edge (non-Chromium version)
`nexacro` | Nexacro platform (HTML5/JavaScript)
`Qt` | Qt framework (multiple languages)
`Silveright` | Silverlight (.NET)
`SWT` | Standard Widgets Toolkit (Java)
`Win32` | Generic Win32 apps (multiple languages)
`WinForm` | Windows Forms (.NET)
`WPF` | Windows Presentation Framework (.NET)
`XAML` | Universal Windows Platform (.NET)

### Sample Queries
Queries are written using the [Kusto Query Language](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/). Here are some sample queries:

#### How many sessions started in the last day?
```
customEvents 
| where timestamp >= ago(1d)
| where name == 'Mainwindow_Startup'
| summarize count()
```

#### How many unique install ID's have started sessions in the last day?
```
customEvents 
| where timestamp >= ago(1d)
| where name == 'Mainwindow_Startup'
| extend installId = tostring(customDimensions.InstallationID)
| extend version = tostring(customDimensions.Version)
| summarize dcount(installId)
```

#### How many unique install ID's have started sessions in the last day, broken down by app version, as a barchart?
```
customEvents 
| where timestamp >= ago(1d)
| where name == 'Mainwindow_Startup'
| extend installId = tostring(customDimensions.InstallationID)
| extend version = tostring(customDimensions.Version)
| summarize dcount(installId) by version
| sort by version desc
| render barchart
```

#### How many unique install ID's have started sessions in the last 60 days, per day, broken down by app version, as a barchart?
```
customEvents
| where timestamp > ago(60d)
| where name == 'Mainwindow_Startup'
| project installId=tostring(customDimensions.InstallationID), days=bin(timestamp, 1d), version=tostring(customDimensions.Version)
| distinct installId, version, days
| summarize count() by days, version
| render barchart
```

#### What are the top 10 Exceptions that have been reported in version 1.1.1467.1 in the last 7 days?
```
exceptions
| where timestamp >= ago(7d)
| extend version = tostring(customDimensions.Version)
| where version == '1.1.1467.1'
| summarize count(problemId) by problemId
| sort by count_problemId desc
| take 10
```

#### What are the top 10 Exceptions that have been reported in version 1.1.1467.1 in the last 7 days, but that occur outside of UIAutomationClient.dll?
```
exceptions
| where timestamp  >= ago(7d)
| extend version = tostring(customDimensions.Version)
| where version == '1.1.1467.1'
| where not(problemId contains 'UIAutomationClient')
| summarize count(problemId) by problemId
| sort by count_problemId desc
| take 10
```
#### What UI frameworks have been scanned in the last 30 days, in descending order of count?
```
customEvents 
| where timestamp >= ago(30d)
| where name == 'Scan_Statistics'
| extend framework = tostring(customDimensions.UIFramework)
| where framework != ''
| summarize count() by framework
| sort by count_ desc
```
