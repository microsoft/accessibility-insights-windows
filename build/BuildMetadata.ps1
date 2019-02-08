# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

$basePath = $args[0]

if ([System.String]::IsNullOrWhiteSpace($($basePath)))
{
    Write-Host 'ERROR: No basePath specified!'
    exit 1
}

$revParse = git rev-parse HEAD | Out-String
Write-Host "*** Result of git rev-parse HEAD:" $($revParse)

$buildNumber = ${env:BUILD_BUILDNUMBER}
Write-Host "*** Build Number:" $($buildNumber)

$rawVersion = [IO.File]::ReadLines(${env:temp} + "\A11yInsightsVersionInfo.cs")[0]
Write-Host "*** Raw Version:" $($rawVersion)

$sha = $($revParse).Split([System.Environment]::Newline)[0]
$productVersion = $($rawVersion).Split('\"')[1]

$folderPath = [IO.Path]::Combine($($basePath), 'signed', $($productVersion))
if (![IO.Directory]::Exists($($folderPath)))
{
    Write-Host 'ERROR: Path not found: ' $($folderPath)
    exit 2
}

$metadataFilePath = [IO.Path]::Combine($($folderPath), 'build_info.json')

$json = @{"sha"=$($sha); "buildNumber"=$($buildNumber); "productVersion"=$($productVersion)} | ConvertTo-Json
Write-Host "*** Writing to file: " $($metadataFilePath)
Write-Host "*** Contents of metadata file: " $($json)
$($json) | Out-File $($metadataFilePath)