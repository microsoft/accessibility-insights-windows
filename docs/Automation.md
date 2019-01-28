# Accessibility Insights for Windows - Automation

## Overview
To enable various automation scenarios, we have created an assembly
(`AccessibilityInsights.Automation.dll`) that exposes a targeted set of
Accessibility Insights functionality to automation systems. This can be used in
a variety of ways, including as part of a standalone test system, as part of a
PowerShell script, or as part of a CI/CD solution. One or more scans can be
performed during a test run, and the outputs will be saved to disk. No UI is
provided in automation mode, and the code is intended to be compatible with
Windows 7 or newer.

## General Characteristics

### Fully Synchronous
Since these commands are all stateful, they are intentionally synchronous within
a process. If you attempt to call into the commands concurrently, the first one
to obtain the lock will execute, then another, then another. This is by design
and is not expected to change at any future time. If you have a scenario that
truly requires the command to execute in parallel, then you will need to create
a solution where you can make those calls from separate processes.

### Specifying parameters
Parameters are specified as a string-to-string dictionary. A hierarchical
parameter structure is supported, as follows:
-   Tier 3 parameters can be read from a JSON-serialized disk file, which is
    specified when the automation session begins. Each of these parameters
    will be used unless they are overridden by a corresponding Tier 2 or Tier 1
    parameter.
-   Tier 2 parameters can be specified via a .NET Dictionary\<string,string\>
    parameter that is passed to the Start command. Each of these parameters will
    be used for the lifetime of the automation session unless they are
    overridden by a Tier 1 parameter.
-   Tier 1 parameters can be passed to some commands. Tier 1 parameters are
    never overridden.

### Return Objects
Each command will return a .NET object to summarize the result of the operation.
These objects are detailed under each command:

## Command Details
### Start Command
The Start command initializes the AccessibilityInsights engine for automation.
It is required before the session can begin, and each session requires a
corresponding call to the Stop command. A given test process is allowed only one
active session at a time, and subsequent calls to Start will fail.

#### Inputs

##### .NET

The **StartCommand.Execute** method accepts the following parameters:

**Name** | **Type** | **Description**
---|---|---
primaryConfig | Dictionary\<string, string\> | The set of Tier 2 parameters to apply during this session. See [Defined Constants](#defined-constants) for specific parameters.
configFile | string | The full path to a file that contains JSON-serialized Tier 3 parameters. Can be `string.Empty`. Must point to a valid JSON-serialized file if non-empty.

##### PowerShell

The **Start-AccessibilityInsights** cmdlet accepts the following optional
parameters:

**Name** | **Type** | **Description**
---|---|---
OutputFile | string  | The location where output from this session will be written. The file extension can (and should) be omitted. This parameter can be specified either at Start-AccessibilityInsights or at Invoke-Snapshot.
OutputFileFormat | string | The output file format to generate. This parameter can be specified either at Start-AccessibilityInsights or at Invoke-Snapshot.
TargetProcessId | string | The process id, expressed as a string, that will be scanned. This parameter can be specified either at Start-AccessibilityInsights or at Invoke-Snapshot.
ConfigFile | string | The full path to a file that contains JSON-serialized Tier 3 parameters. Must point to a valid JSON-serialized file if specified.
TeamName | string | The team name which will be used in telemetry. This parameter is only consumed at the start command.

#### Return object

The **StartCommandObject** has the following properties:

**Name** | **Type** | **Description**
---|---|---
Completed | bool | True if the command ran to completion. If true, then the Succeeded property indicates if the call succeeded.
SummaryMessage | string | A human-readable message to summarize the result. This message can change at any time, so do not rely on parsing it in tools.
Succeeded | bool | True if the Start command completed successfully.

Snapshot Command
----------------
The Snapshot command scans a single process for accessibility issues and saves
the output file for later analysis. It can only be called while an automation
session is open (i.e., between the Start and Stop commands):

#### Inputs

##### .NET

The **SnapshotCommand.Execute** method accepts the following parameters:

**Name** | **Type** | **Description**
---|---|---
primaryConfig | Dictionary\<string, string\> | The set of Tier 1 parameters to apply during this call. See [Defined Constants](#defined-constants) for specific parameters.

##### PowerShell

The **Invoke-Snapshot** cmdlet accepts the following parameters:

**Name** | **Type** | **Description**
---|---|---
OutputFile | string | The location where output from this session will be written. The file extension can (and should) be omitted.
TargetProcessId | string | The process id, expressed as a string, that will be scanned. This parameter can be specified either at Start-AccessibilityInsights or at Invoke-Snapshot.
OutputFileFormat | string | The output file format to generate. This parameter can be specified either at Start-AccessibilityInsights or at Invoke-Snapshot.

#### Return object

The **SnapshotCommandResult** object has the following properties:

**Name** | **Type** | **Description**
---|---|---
Completed | bool | True if the command ran to completion. If true, then the Succeeded property indicates if the call succeeded.
SummaryMessage | string | A human-readable message to summarize the result. This message can change at any time, so do not rely on parsing it in tools.
Succeeded | bool | True if the Start command completed successfully.
ScanResultsPassed | int | The count of visual elements that passed all accessibility scans.
ScanResultsFailed | int | The count of visual elements that failed one or more accessibility scans.
ScanResultsInconclusive | int | The count of visual elements that registered an inconclusive scan—these are typically cases where a human needs to evaluate the control and determine whether or not a fix is necessary.
ScanResultsUnsupported  | int | The count of visual elements that Accessibility Insights is unable to scan—these are typically elements like embedded web controls.
ScanResultsTotal | int | The sum of ScanResultsPassed, ScanResultsFailed, ScanResultsInconclusive, and ScanResultsUnsupported.

### Stop Command
The Stop command terminates an automation session. State and associated
resources are freed. The test framework can then exit or create another session
(via the Start command), as appropriate:

#### Inputs

##### .NET

The **StopCommand.Execute** method accepts no parameters.

##### PowerShell
The **Stop-AccessibilityInsights** cmdlet accepts no parameters

#### Return object
The StopCommandResult object has the following properties:

**Name** | **Type** | **Description**
---|---|---
Completed | bool | True if the command ran to completion. If true, then the Succeeded property indicates if the call succeeded.
SummaryMessage | string | A human-readable message to summarize the result. This message can change at any time, so do not rely on parsing it in tools.
Succeeded | bool | True if the Start command completed successfully.

### Defined Constants
The following constants are defined in the assembly:

**Name** | **Value** | **Description**
---|---|---
CommandConstStrings.TargetProcessId | “TargetProcessId” | The string representation (in decimal format) of the process ID to be scanned. Must be set in at least one tier (Tier 1/Tier 2/Tier 3).
CommandConstStrings.OutputPath | “OutputPath” | The file location where files from this automation session are to be written. If the target path directory does not exist, it will be created when the first output file is written to it. If the intent is for CI / CD, it is recommended that OutputPath not be set.
CommandConstStrings.OutputFile | “OutputFile” | The specific file (without path) where scan results are to be written. If the file already exists, it will be overwritten. The file extension can (and should) be omitted. Must be set in at least one tier (Tier 1/Tier 2/Tier 3).
CommandConstStrings.OutputFileFormat | “OutputFileFormat” | The specific format that the output file should be generated in. The files generated will use the output path and the output filename provided. The three supported values are:<ul><li> **a11yTest** will generate "\*.a11ytest" files.<li>**Sarif** = will generate "\*.sarif" files.<li>**All** will generate both an a11ytest file and a sarif file.</ul>This parameter has lower precedence than if an extension was passed in as a part of the filename. If no extension is passed in the filename and this parameter is not passed in, then files will be generated in the sarif format. May be set in at least one tier (Tier 1/Tier 2/Tier 3). *Default value = **Sarif***.
CommandConstStrings.NoViolationPolicy | “NoViolationPolicy” | Specifies how output files should be handled if no violations are found. <ul><li>**Discard** will discard the results file.<li>**Retain** will retain the results file.</ul>*Default value = **Retain***. Valid only as a Tier 3 setting.
CommandConstStrings.Discard  | “Discard” | Value for NoViolationPolicy.
CommandConstStrings.Retain | “Retain” | Value for NoViolationPolicy.
CommandConstStrings.TeamName | “TeamName” | Value for Team name in telemetry.

## Using the assembly
If you have installed Accessibility Insights for Windows, then you can access the
DLLs that are installed under ProgramFiles (typical path is **C:\\Program Files (x86)
\\AccessibilityInsights\\1.1**). You can also get the files
via a NuGet package Configure NuGet to retrieve the
**AccessibilityInsights.Automation.Windows** package from
<https://TBD/nuget/v3/index.json>,
then use the classes in the AccessibilityInsights.Automation namespace (see
examples below):

### From .NET code
-   Prerequisite: Your project *must* use .NET 4.7.1 (this is required by
    Accessibility Insights).
-   If you’re using NuGet, add the appropriate feed to your project.
-   Add **using AccessibilityInsights.Automation;** to your code.
-   Access the Start command via the **StartCommand.Execute** method.
-   Access the Snapshot command via the **SnapshotCommand.Execute** method.
-   Access the Stop command via the **StopCommand.Execute** method.

Sample C\# code—this is interactive, but yours doesn’t need to be:
```
    using System;
    using System.Collections.Generic;
    using AccessibilityInsights.Automation;

    namespace AccessibilityInsightsDemo
    {
        class Program
        {
            /// <summary>
            /// This is a quick and easy demo of the automation code
            /// </summary>
            static void Main(string[] args)
            {
                var parameters = new Dictionary<string, string>();
                string secondaryConfigFile = string.Empty;

                char[] delimiters = {'='};

                foreach (string arg in args)
                {
                    string[] pieces = arg.Split(delimiters);
                    if (pieces.Length == 2)
                    {
                        string key = pieces[0].Trim();
                        string value = pieces[1].Trim();

                        if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                        {
                            // Special case for SecondaryConfigFile
                            if (key.Equals("SecondaryConfigFile", StringComparison.OrdinalIgnoreCase))
                            {
                                secondaryConfigFile = value;
                            }
                            else
                            {
                                parameters[key] = value;
                            }
                            continue;
                        }
                    }

                    Console.WriteLine("Ignoring malformed input: {0}", arg);
                };

                Console.WriteLine(StartCommand.Execute(parameters secondaryConfigFile).ToString());

                int autoFileId = 0;

                while (true)
                {
                    Console.Write("Enter process ID to capture (blank to exit): ");
                    string input = Console.ReadLine();

                    if (input == string.Empty)
                        break;

                    if (!int.TryParse(input, out int processId))
                    {
                        Console.WriteLine("Not a valid int: " + input);
                        continue;
                    }

                    Dictionary<string, string> snapshotParameters = new Dictionary<string, string>
                    {
                        { CommandConstStrings.TargetProcessId, input },
                        { CommandConstStrings.OutputFile, autoFileId++.ToString() },
                    };
                    Console.WriteLine(SnapshotCommand.Execute(snapshotParameters).ToString());
                }
                Console.WriteLine(StopCommand.Execute().ToString());
            }
        }
    }
```

### From PowerShell
-   Start your PowerShell script from the folder where the AccessibilityInsights
    DLLs are located.
-   Load the assembly via **Import-Module AccessibilityInsights.Automation.dll**.
-   Access the Start command via the **Start-AccessibilityInsights** cmdlet.
-   Access the Snapshot command via the **Invoke-Snapshot** cmdlet.
-   Access the Stop command via the **Stop-AccessibilityInsights** cmdlet.

Sample PowerShell script (the 2 second delay exists to give the app time to initialize before it is scanned):
```
    Import-Module AccessibilityInsights.Automation.dll
    Start-AccessibilityInsights -OutputPath c:\MyFolder
    Start-Process -FilePath notepad.exe
    Start-Sleep 2
    $appProcId=get-process notepad |select -expand id
    Invoke-Snapshot -OutputFile notepadSnapshot -TargetProcessId $appProcId
    Stop-Process -Id $appProcId
    Stop-AccessibilityInsights
```