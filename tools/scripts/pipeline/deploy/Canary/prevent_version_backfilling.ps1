# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Release pipeline script that runs to prevent backfilling of Canary releases. It fails if any releases
exist that are newer than this release.

It consumes the following environment variables:
    A11yInsightsVersion    is the version (e.g., 1.1.2227.01) begin published by the pipeline
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
    if ($null -eq $env:A11yInsightsVersion)
    {
        Write-Error "Could not find an environment variable for 'A11yInsightsVersion'"
        exit 1
    }
    if ($null -eq $env:OctokitVersion)
    {
        Write-Error "Could not find an environment variable for 'OctokitVersion'"
        exit 1
    }
}

# Extracts a version from the release name.
function Get-ReleaseVersion($specificRelease)
{
    $versionString = $specificRelease.Name
    if($versionString -match ".*v(\d+\.\d+\.\d+\.\d+).*")
    {
        return $matches[1]
    }
    else
    {
        return [string]::Empty
    }
}

# Dump the sorted releases for debugging
function Write-ReleaseMap($releaseMap)
{
    Write-Host "==================================="
    Write-Host "Sorted Releases"
    Write-Host "Version       TagName         Name"
    Write-Host "-------       -------         ----"
    foreach($releaseKV in $releaseMap)
    {
        Write-Host $releaseKV.Key "  " $releaseKV.Value.TagName "  " $releaseKV.Value.Name
    }
    Write-Host "==================================="
}

# Sorts the releases based on the version number
function SortReleases($releases)
{
    $releaseMap = @{}
    foreach($release in $releases)
    {
        $releaseVersion = [System.Version](Get-ReleaseVersion $release)
        if(-not [string]::IsNullOrEmpty($releaseVersion))
        {
            $releaseMap.Add($releaseVersion,$release)
        }
    }

    $releaseMap = $releaseMap.GetEnumerator() | Sort-Object -Descending -Property Name

    Write-ReleaseMap $releaseMap

    return $releaseMap
}

# Return the newest release version
function Get-NewestReleaseVersion($releaseMap)
{
    return $releaseMap[0].Key
}

# Fail the script if this would result in a backfill
function DisallowBackfill($newestVersion)
{
    $releaseVersion = New-Object System.Version($env:A11yInsightsVersion)

    Write-Host "Comparing release version" $releaseVersion "to newest release" $newestVersion

    if ($newestVersion -gt $releaseVersion)
    {
        Write-Error "Backfill validation FAILED"
    }
    else
    {
        Write-Host "Backfill validation passed"
    }
}

# Initialize the client
function Get-Client()
{
    # Load the octokit dll
    Add-Type -Path ((Get-Location).Path + "\Octokit.$($env:OctokitVersion)\lib\netstandard2.0\Octokit.dll")

    # Get a new client
    $productHeader = [Octokit.ProductHeaderValue]::new("Canary-Pipeline-PreValidation")
    $client = [Octokit.GitHubClient]::new($productHeader)

    return $client
}

# Main program
CheckEnvironmentVariables
$client = Get-Client
$releases = $client.Repository.Release.GetAll($Owner, $Repo).Result

Write-Host "Unsorted Releases:" 
$releases | Select-Object -Property Name, TagName | Format-Table

$releaseMap = SortReleases $releases
$newestReleaseVersion = Get-NewestReleaseVersion($releaseMap)
DisallowBackfill $newestReleaseVersion
