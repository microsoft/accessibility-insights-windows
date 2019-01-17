#Copyright (c) Microsoft. All rights reserved.
#Licensed under the MIT license. See LICENSE file in the project root for full license information.

#environment variables
param(
    $installationDir = 'C:\Program Files (x86)\AccessibilityInsights\1.1\'
    )

$VerbosePreference='continue'
$currLocation=Get-Location
$appPath=Join-path $currLocation '..\WildlifeManager\WildlifeManager.exe'
$outputPath=Join-path $currLocation '..\..\src\AccessibilityInsights.CI\bin\AutomationCheck'

Remove-Item $outputPath -Recurse -Force -ErrorAction Ignore | Out-Null
New-Item $outputPath -ItemType Directory | Out-Null

Write-Verbose "Register & start AccessibilityInsights"
Push-Location
Set-Location $installationDir
Write-Verbose '------------------------'
Write-Verbose 'Importing module'
Import-Module .\AccessibilityInsights.Automation.dll

Write-Verbose '------------------------'
Write-Verbose 'Starting AccessibilityInsights'
Start-AccessibilityInsights -OutputPath $($outputPath)

Write-Verbose '------------------------'
Write-Verbose 'Launching WildlifeManager'
Write-Verbose $appPath
Start-Process -FilePath ($appPath)
Start-Sleep 5
$procId=get-process WildlifeManager | select -expand id

Write-Verbose "WildlifeManager has processId of $($procId)"

Write-Verbose '------------------------'
Write-Verbose 'Invoking Snapshot'
$result=Invoke-Snapshot -OutputFile WildlifeManager -TargetProcessId $procId
Write-Verbose $($result)
Stop-Process -Id $procId

Write-Verbose '------------------------'
Write-Verbose 'Stopping AccessibilityInsights'
Stop-AccessibilityInsights
Pop-Location

Write-Host "Results stored in ..\..\src\AccessibilityInsights.CI\bin\AutomationCheck. Press ENTER to exit"
Read-Host
