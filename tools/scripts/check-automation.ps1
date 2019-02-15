# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
  exit 1
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

if ($result.Completed -eq $false)
{
  Write-Host '*** AUTOMATION FAILED: SCAN FALIED TO COMPLETE' -ForegroundColor Red
  exit 2
}
elseif ($result.ScanResultsPassed -eq 0)
{
  Write-Host '*** AUTOMATION FAILED: SCAN FOUND NO PASSING RESULTS' -ForegroundColor Red
  exit 3
}
elseif ($result.ScanResultsFailed -eq 0)
{
  Write-Host '*** AUTOMATION FAILED: SCAN FOUND NO FAILING RESULTS' -ForegroundColor Red
  exit 4
}
elseif ($result.ScanResultsInconclusive -eq 0)
{
  Write-Host '*** AUTOMATION FAILED: SCAN FOUND NO INCONCLUSIVE RESULTS' -ForegroundColor Red
  exit 5
}
elseif ($result.ScanResultsUnsupported -ne 0)
{
  Write-Host '*** AUTOMATION FAILED: SCAN FOUND UNSUPPORTED RESULTS' -ForegroundColor Red
  exit 6
}
elseif ((Get-ChildItem $($outputPath) -Filter 'WildlifeManager.sarif').Length -eq 0)
{
  Write-Host '*** AUTOMATION FAILED: NO OUTPUT FILE GENERATED ***' -ForegroundColor Red
  exit 7
}
elseif ((Get-ChildItem $($outputPath) -Filter 'WildlifeManager.*Teest').Length -ne 0)
{
  Write-Host '*** AUTOMATION FAILED: INTERMEDIATE FILE NOT DELETED ***' -ForegroundColor Red
  exit 8
}

#keep these lines at the end of the script
Write-Host '*** AUTOMATION SUCCEEDED ***' -ForegroundColor Green
exit 0
