# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Pipeline script that runs to perform cleanup of old releases. We want to keep the following:
  - All draft releases newer than the version being promoted to Production
  - The 2 (or 1 if IsForcedProdUpdate is true) most recent non-draft releases
  - The tags associated with non-draft releases that we are removing

It consumes the following environment variables:
    GITHUB_TOKEN                     is the R/W PAT to access the repo
    OctokitVersion                   is the version of OctoKit we've pinned to. A previous task will install this package
    DeleteReleases                   is true if cleanup should actually delete (if false, just report what would be cleaned up)
    A11yInsightsMinProdVersionTag    is the tag of the oldest prod release that we will retain

The script is assumed to run from the default working directory of the pipeline.
#>

Set-StrictMode -Version Latest
$script:ErrorActionPreference = 'Stop'

# Constants
$Owner = "Microsoft"
$Repo = "accessibility-insights-windows"

function CheckEnvironmentVariables()
{
    if ($null -eq $env:GITHUB_TOKEN)
    {
        Write-Error "Could not find an environment variable for 'GITHUB_TOKEN'"
        exit 1
    }
    if ($null -eq $env:OctokitVersion)
    {
        Write-Error "Could not find an environment variable for 'OctokitVersion'"
        exit 1
    }
    if ($null -eq $env:DeleteReleases)
    {
        Write-Error "Could not find an environment variable for 'DeleteReleases'"
        exit 1
    }
    if ($null -eq $env:A11yInsightsMinProdVersionTag)
    {
        Write-Error "Could not find an environment variable for 'A11yInsightsMinProdVersionTag'"
        exit 1
    }
}

# Initialize the client
function Get-Client()
{
    if ($null -eq $env:GITHUB_TOKEN)
    {
        throw "Run this script with the GITHUB_TOKEN environment variable set to a GitHub Personal Access Token with 'repo' permissions"
    }

    # Load the octokit dll
    Add-Type -Path ((Get-Location).Path + "\Octokit.$($env:OctokitVersion)\lib\netstandard2.0\Octokit.dll")

    # Get a new client
    $productHeader = [Octokit.ProductHeaderValue]::new("Production-Pipeline-PreValidation")
    $client = [Octokit.GitHubClient]::new($productHeader)

    # Add credentials for authentication
    $client.Credentials = [Octokit.Credentials]::new($env:GITHUB_TOKEN)
    return $client
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
    Write-Host "Version       Name"
    Write-Host "-------       ----"
    foreach($releaseKV in $releaseMap)
    {
        Write-Host $releaseKV.Key "  " $releaseKV.Value.Name
    }
    Write-Host "==================================="
}

# Sorts the releases based on the version number
function SortReleases($releases)
{
    $releaseMap = @{}
    foreach($release in $releases.Result)
    {
        $releaseVersion = [System.Version](Get-ReleaseVersion $release)
        if(-Not [string]::IsNullOrEmpty($releaseVersion))
        {
            $releaseMap.Add($releaseVersion,$release)
        }
    }

    $releaseMap = $releaseMap.GetEnumerator() | Sort-Object -Descending -Property Name

    Write-ReleaseMap $releaseMap

    return $releaseMap
}

# Delete a release
function Remove-ReleaseAndPerhapsItsTag($client, $release, $deleteReleases)
{
    # Null checks
    if([string]::IsNullOrEmpty($release.Id))
    {
        Write-Host "No Release Id found"
        return;
    }

    $wasPrerelease = $release.Prerelease
    if($deleteReleases -eq $true)
    {
        Write-Host "Deleting release '$($release.Name)'"
        $response = $client.Repository.Release.Delete($Owner, $Repo, $release.Id);

        if (-Not ($response.Result -eq "NoContent" -And $response.IsCompleted -eq $True))
        {
            Write-Host "Failed to delete release '$($release.Name)'"
        }
        else 
        {
            if ($wasPrerelease)
            {
                Write-Host "Deleting prerelease tag '$($release.TagName)"
                $client.Git.Reference.Delete($Owner, $Repo, "tags/$($release.TagName)").Wait()
            }
            else
            {
                Write-Host "Retaining release tag '$($release.TagName)'"
            }
        }
    }
    else
    {
        Write-Host "Would have deleted release '$($release.Name)'"
        if ($wasPrerelease)
        {
            Write-Host "Would have deleted prerelease tag '$($release.TagName)'"
        }
        else
        {
            Write-Host "Would have retained release tag '$($release.TagName)'"
        }
    }
}


# Main program
CheckEnvironmentVariables
$client = Get-Client
$releases = $client.Repository.Release.GetAll($Owner, $Repo)
$deleteReleases = $env:DeleteReleases -eq "true"
$minProdVersionTag = $env:A11yInsightsMinProdVersionTag

Write-Host "Releases found" 
$releases.Result | Select-Object -Property Name, TagName | Format-Table

$releaseMap = SortReleases $releases

$foundMinProdVersion = $false

foreach($releaseKV in $releaseMap)
{
    $release = $releaseKV.Value
    if ($foundMinProdVersion -eq $true)
    {
        $deleteList += $release
        continue
    }

    if (($release.Prerelease -eq $false) -and (($($release.TagName) -eq $($minProdVersionTag))))
    {
        $foundMinProdVersion = $true
    }
}

Write-Host "A11yInsightsMinProdVersionTag = $($minProdVersionTag)"
Write-Host "Delete releases option is set to $($deleteReleases)" 

if($deleteList.Count -eq 0)
{
    Write-Host "No releases to delete."
}
else
{
    $deleteList | ForEach-Object { Remove-ReleaseAndPerhapsItsTag $client $_ $deleteReleases }
}
