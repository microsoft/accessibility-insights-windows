# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Pipeline script that runs to confirm that the targeted release is ready to release to Production.
It returns an error if any of the following conditions are true:
  - The latest release does not exist
  - The latest release's TagName property does not match A11yInsightsTagName
  - The latest release's Prerelease property is true
  - The latest release's Draft property is true
  - The latest release's Name property does not contain the string "Production Release"
  - The latest release's Body property does not contain the string "Production Release"

It consumes the following environment variables:
    A11yInsightsTagName    is the name of the tag (e.g., v1.1.2227.01) associated with this release
    OctokitVersion         is the version of OctoKit we've pinned to. A previous task will install this package

The script is assumed to run from the default working directory of the pipeline.
#>

Set-StrictMode -Version Latest
$script:ErrorActionPreference = 'Stop'

# Constants
$Owner = "Microsoft"
$Repo = "accessibility-insights-windows"

function CheckEnvironmentVariables()
{
    if ($null -eq $env:A11yInsightsTagName)
    {
        Write-Error "Could not find an environment variable for 'A11yInsightsTagName'"
        exit 1
    }
    if ($null -eq $env:OctokitVersion)
    {
        Write-Error "Could not find an environment variable for 'OctokitVersion'"
        exit 1
    }
}

# Initialize the client
function Get-Client()
{
    # Load the octokit dll
    Add-Type -Path ((Get-Location).Path + "\Octokit.$($env:OctokitVersion)\lib\netstandard2.0\Octokit.dll")

    # Get a new client
    $productHeader = [Octokit.ProductHeaderValue]::new("Production-Pipeline-PreValidation")
    $client = [Octokit.GitHubClient]::new($productHeader)

    return $client
}

# Main program
CheckEnvironmentVariables
$productionReleaseMarker = "Production Release"

$client = Get-Client
$latestRelease = $client.Repository.Release.GetLatest($Owner, $Repo).Result

if ($null -eq $latestRelease)
{
    Write-Error "Unable to obtain the latest release!"
    Exit
}

Write-Host "===== Begin Latest Release Dump ====="
$latestRelease
Write-Host "=====  End Latest Release Dump  ====="


if ($latestRelease.TagName -ne $env:A11yInsightsTagName)
{
    Write-Error "Target tag '($($env:A11yInsightsTagName))' does not match latest release tag of '($($latestRelease.TagName))'"
    Exit
}

if ($latestRelease.Prerelease -eq $true)
{
    Write-Error "Target release '$($latestRelease.Name)' must not be marked as prerelease"
    Exit
}

if ($latestRelease.Draft -eq $true)
{
    Write-Error "Target release '$($latestRelease.Name)' must not be marked as draft"
    Exit
}

if (-Not ($latestRelease.Name -Match $ProductionReleaseMarker))
{
    Write-Error "Release Name must contain '$($ProductionReleaseMarker)'"
    Exit
}

if (-Not ($latestRelease.Body -Match $ProductionReleaseMarker))
{
    Write-Error "Release Body must contain '$($ProductionReleaseMarker)'"
    Exit
}

Write-Host "Release Prevalidation Succeeded"
