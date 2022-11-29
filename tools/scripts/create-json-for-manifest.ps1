# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Create the json file that will be packaged into AccessibilityInsights.Manifest.dll

.PARAMETER MsiBasePath
The fixed path to where the MSI folder is located. This is typically $(SolutionDir)MSI\bin\Release\MSI

.PARAMETER MinimumProductionVersion
The minimum supported production version

.PARAMETER OutputPath
The path and file name of the generated JSON file

.Example Usage 
.\create-json-for-manifest.ps1 -MsiBasePath "$(SolutionDir)MSI\bin\Release\MSI" -MinimumProductionVersion "1.1.1234.01" -OutputPath "$(SolutionDir)Manifest\obj\Release" -OutputFile "ReleaseInfo.json"
#>

param(
    [Parameter(Mandatory=$true)][string]$MsiBasePath,
    [Parameter(Mandatory=$true)][string]$MinimumProductionVersion,
    [Parameter(Mandatory=$true)][string]$OutputPath,
    [Parameter(Mandatory=$true)][string]$OutputFile
)

Set-StrictMode -Version Latest
$script:ErrorActionPreference = 'Stop'

# Uncomment the next line for debugging
#$VerbosePreference='continue'

Set-Variable MsiName -Option Readonly -Value "AccessibilityInsights.msi"

function CreatePaddedVersion([string]$version) {
    Write-Verbose "  Entering CreatePaddedVersion"
    Write-Verbose "    version = $version"

    $structuredVersion = [Version]::new($version)

    $paddedVersion = [String]::Format("{0}.{1}.{2:0000}.{3:00}", $structuredVersion.Major, $structuredVersion.Minor, $structuredVersion.Build, $structuredVersion.Revision)
    Write-Verbose "  Exiting CreatePaddedVersion with $paddedVersion"
    return $paddedVersion
}

function GetHighestBuiltVersion([string]$msiBasePath) {
    Write-Verbose "  Entering GetHighestBuiltVersion"
    Write-Verbose "    msiBasePath = $msiBasePath"

    $folders = Get-ChildItem -Path $msiBasePath -Directory | select Name
    $highestBuiltVersion = ($folders | Sort-Object -Property Name -Descending)[0].Name

    Write-Verbose "  Exiting GetHighestBuiltVersion with $highestBuiltVersion"
    return $highestBuiltVersion
}

function GetMsiInfo([string]$msiFilePath) {
    Write-Verbose "  Entering GetMsiInfo"
    Write-Verbose "    msiFilePath = $msiFilePath"

    $info = [ordered]@{}
    $info.msi_size_in_bytes = (Get-Item $msiFilePath).length
    $info.msi_sha_512 = (Get-FileHash -Path $msiFilePath -Algorithm "SHA512").Hash

    Write-Verbose "  Exiting GetMsiInfo"
    return $info
}

function CreateJsonForManifest([string]$msiBasePath, [string]$minimumProductionVersion) {
    Write-Verbose "Entering CreateJsonForManifest"
    Write-Verbose "  msiBasePath = $msiBasePath"
    Write-Verbose "  minimumProductionVersion = $minimumProductionVersion"

    $highestBuiltVersion = GetHighestBuiltVersion $msiBasePath
    $paddedVersion = CreatePaddedVersion $highestBuiltVersion
    $msiFilePath = Join-Path (Join-Path $msiBasePath $highestBuiltVersion) $MsiName

    $info = GetMsiInfo $msiFilePath
    $info.installer_url = "https://www.github.com/Microsoft/accessibility-insights-windows/releases/download/v$paddedVersion/$MsiName"
    $info.release_notes_url = "https://www.github.com/Microsoft/accessibility-insights-windows/releases/tag/v$paddedVersion"
    $info.production_minimum_version = $minimumProductionVersion
    $info.current_version = $paddedVersion

    $json = $info | ConvertTo-Json

    Write-Verbose "Exiting CreateJsonForManifest"
    return $json
}

function CreateOutputFile([string]$json, [string]$outputPath, [string]$outputFile) {
    Write-Verbose "Entering CreateOutputFile"
    Write-Verbose "  json = $json"
    Write-Verbose "  outputPath = $outputPath"
    Write-Verbose "  outputFile = $outputFile"

    if (!(Test-Path $outputPath)) {
        Write-Verbose "  Creating directory $outputPath"
        New-Item $outputPath -ItemType Directory | Out-Null
    }

    $fullPath = Join-Path $outputPath $outputFile
    $json | Out-File -FilePath $fullPath

    Write-Host "Successfully wrote manifest data to $fullPath"

    Write-Verbose "Exiting CreateOutputFile"
}

$resolvedOutputPath = Resolve-Path $OutputPath
$json = CreateJsonForManifest $MsiBasePath $MinimumProductionVersion
CreateOutputFile $json $resolvedOutputPath $OutputFile
exit 0
