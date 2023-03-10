# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Create the json file that will be packaged into AccessibilityInsights.Manifest.dll

.PARAMETER OctokitPath
The path to the Octokit assembly that will be used when fetching the current release version

.PARAMETER MsiBasePath
The fixed path to where the MSI folder is located. This is typically $(SolutionDir)MSI\bin\Release\MSI

.PARAMETER IsMandatoryProdUpdate
Whether or not this build includes a mandatory update for the Production ReleaseChannel

.PARAMETER OutputPath
The path and file name of the generated JSON file

.Example Usage 
.\create-json-for-manifest.ps1 -OctokitPath "%UserProfile%\.nuget\packages\Octokit\5.0.1\lib\netstandard2.0\Octokit.dll" -MsiBasePath "$(SolutionDir)MSI\bin\Release\MSI" -IsMandatoryProdUpdate "false" -OutputPath "$(SolutionDir)Manifest\obj\Release" -OutputFile "ReleaseInfo.json"
#>

param(
    [Parameter(Mandatory=$true)][string]$OctokitPath,
    [Parameter(Mandatory=$true)][string]$MsiBasePath,
    [Parameter(Mandatory=$true)][string]$IsMandatoryProdUpdate,
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

function Get-HighestBuiltVersion([string]$msiBasePath) {
    Write-Verbose "  Entering Get-HighestBuiltVersion"
    Write-Verbose "    msiBasePath = $msiBasePath"

    $folders = Get-ChildItem -Path $msiBasePath -Directory | select Name
    $highestBuiltVersion = ($folders | Sort-Object -Property Name -Descending)[0].Name

    Write-Verbose "  Exiting Get-HighestBuiltVersion with $highestBuiltVersion"
    return $highestBuiltVersion
}

function Get-MsiInfo([string]$msiFilePath) {
    Write-Verbose "  Entering Get-MsiInfo"
    Write-Verbose "    msiFilePath = $msiFilePath"

    $info = [ordered]@{}
    $info.msi_size_in_bytes = (Get-Item $msiFilePath).length
    $info.msi_sha_512 = (Get-FileHash -Path $msiFilePath -Algorithm "SHA512").Hash

    Write-Verbose "  Exiting Get-MsiInfo"
    return $info
}

# Initialize the client
function Get-Client([string]$octokitPath)
{
    Write-Verbose "    Entering Get-Client"
    # Load the octokit dll
    Write-Verbose "      Path to OctoKit.dll = $($octoKitPath)"
    Add-Type -Path $($octoKitPath)

    # Get a new client
    $productHeader = [Octokit.ProductHeaderValue]::new("CreateJsonForManifest")
    $client = [Octokit.GitHubClient]::new($productHeader)

    Write-Verbose "    Exiting Get-Client"
    return $client
}

function Get-MinimumProductionVersion([string]$currentVersion, [string]$octokitPath, [string]$isMandatoryProdUpdate) {
    Write-Verbose "  Entering Get-MinimumProductionVersion"
    Write-Verbose "    currentVersion = $currentVersion"
    Write-Verbose "    octokitPath = $octokitPath"
    Write-Verbose "    isMandatoryProdUpdate = $isMandatoryProdUpdate"

    if ($isMandatoryProdUpdate -eq "true") {
        Write-Verbose "    Using currentVersion for minimumProductionVersion"
        $minimumProductionVersion = $currentVersion
    } else {
        Write-Verbose "    Fetching version from 'https://github.com/microsoft/accessibility-insights-windows/releases/tag/latest'"

        $client = Get-Client $octokitPath
        $release = $client.Repository.Release.GetLatest("Microsoft", "accessibility-insights-windows").Result

        $minimumProductionVersion = $release.TagName.Substring(1)  # Tag names are in format of v1.1.1234.01 and we want to skip the 'v'
    }

    Write-Verbose "  Exiting Get-MinimumProductionVersion with value of $($minimumProductionVersion)"
    return $minimumProductionVersion
}

function CreateJsonForManifest([string]$msiBasePath, [string]$octokitPath, [string]$isMandatoryProdUpdate) {
    Write-Verbose "Entering CreateJsonForManifest"
    Write-Verbose "  msiBasePath = $msiBasePath"
    Write-Verbose "  octokitPath = $octokitPath"
    Write-Verbose "  isMandatoryProdUpdate = $isMandatoryProdUpdate"

    $highestBuiltVersion = Get-HighestBuiltVersion $msiBasePath
    $paddedVersion = CreatePaddedVersion $highestBuiltVersion
    $msiFilePath = Join-Path (Join-Path $msiBasePath $highestBuiltVersion) $MsiName

    $info = Get-MsiInfo $msiFilePath
    $info.installer_url = "https://www.github.com/Microsoft/accessibility-insights-windows/releases/download/v$paddedVersion/$MsiName"
    $info.release_notes_url = "https://www.github.com/Microsoft/accessibility-insights-windows/releases/tag/v$paddedVersion"
    $info.current_version = $paddedVersion
    $info.production_minimum_version = Get-MinimumProductionVersion $paddedVersion $octokitPath $IsMandatoryProdUpdate

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
$json = CreateJsonForManifest $MsiBasePath $OctokitPath $isMandatoryProdUpdate
CreateOutputFile $json $resolvedOutputPath $OutputFile
exit 0
