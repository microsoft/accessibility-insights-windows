# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#Copyright (c) Microsoft. All rights reserved.
#Licensed under the MIT license. See LICENSE file in the project root for full license information.

#environment variables
$VerbosePreference='continue'
Clear-Host

$flavor=$args[0]
switch ($flavor)
{
  ''        {$flavor='debug'; break}
  'debug'   {$flavor='debug'; break}
  'release' {$flavor='release'; break}
  default   {Write-Host -separator '' 'Argument "' $($flavor)'" is invalid. Please specify debug (default) or release'; exit}
}

$appPath='..\..\..\..\tools\WildlifeManager\WildlifeManager.exe'
$rootPath=Join-Path '..\..\src\AccessibilityInsights.CI\bin\' $flavor
$outputPath=Join-Path $rootPath 'AutomationCheck'

if (-Not (Test-Path (Join-Path $rootPath AccessibilityInsights.CI.exe)))
{
  Write-Host 'Please build the' $flavor 'version of AccessibilityInsight.CI before running this script'
  exit
}

Remove-Item $outputPath -Recurse -Force -ErrorAction Ignore | Out-Null
New-Item $outputPath -ItemType Directory | Out-Null

Write-Verbose "Register & start AccessibilityInsights"
Push-Location
Set-Location $rootPath
Write-Verbose '------------------------'
Write-Verbose 'Importing module'
Import-Module .\AccessibilityInsights.Automation.dll

Write-Verbose '------------------------'
Write-Verbose 'Starting AccessibilityInsights'
Start-AccessibilityInsights -OutputPath $($outputPath)

Write-Verbose '------------------------'
Write-Verbose 'Launching WildlifeManager'
Start-Process -FilePath $($appPath)
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

Write-Host "Press ENTER to exit"
Read-Host
