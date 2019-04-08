# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

#This script assumes that the OctoKit Nuget package is a custom install and is in the current folder.

$deleteList = @()

# Extracts a version from the release name.
function Extract-Version($specificRelease){
    $versionString = $specificRelease.Name
    if($versionString -match ".*v(\d+\.\d+\.\d+\.\d+).*"){
        return $matches[1]
    } else {
        $deleteList += $specificRelease
        return [string]::Empty
    }
}

# Sorts the releases based on the version number
function Sort-Releases($releases){
    $releaseMap = @{}
    foreach($release in $releases.Result){
        $releaseVersion = [System.version](Extract-Version $release)
        if(-not [string]::IsNullOrEmpty($releaseVersion)){
            $releaseMap.Add($releaseVersion,$release)
        }
    }

    $releaseMap = $releaseMap.GetEnumerator() | Sort-Object -Descending -Property Name
    return $releaseMap
}

# Delete a release
function Delete-Release($release){
    # Null checks
    if([string]::IsNullOrEmpty($release.Id)){
        Write-Host "No Release Id found"
        return;
    }

    Write-Host "Deleting release" + $release.Name
    $response = $instance.Repository.Release.Delete($repoId, $release.Id);

    if(-not ($response.Result -eq "NoContent" -and $response.IsCompleted -eq $True)){
        Write-Host "Failed to delete release -" $release.Name
    } 
}

# Load the octokit dll
Add-Type -Path ((Get-Location).Path + "\Octokit.0.32.0\lib\net45\Octokit.dll")

# Get a new client with random product header value
$productHeader = [Octokit.ProductHeaderValue]::new("AIWindows-CleanupScript")
$instance = [Octokit.GitHubClient]::new($productHeader)

# Add credentials for authentication
$instance.Credentials = [Octokit.Credentials]::new("$(gitPATX)")

# Get repo id
$repoId = $instance.Repository.Get("Microsoft", "accessibility-insights-windows").Result.Id;

# Get all releases
$releases = $instance.Repository.Release.GetAll($repoId)

Write-Host "Releases found" 
$releases.Result | Select-Object -Property Name, TagName, Id | Format-Table

$releaseMap = Sort-Releases $releases
Write-Host "==========================================================="
Write-Host "Sorted Releases" 
Write-Host ($releaseMap | Out-Default)
Write-Host "==========================================================="

$prodCount = 0
$insiderCount = 0
$canaryCount = 0;

foreach($releaseKV in $releaseMap){
    $release = $releaseKV.Value
    if ($release.Name -like "Insider*"){
        if($insiderCount -lt 2){
            $insiderCount++
            continue
        }
        $deleteList += $release
    } elseif ($release.Name -like "Production*"){
        if($prodCount -lt 2){
            $prodCount++
            continue
        }
        $deleteList += $release
    } elseif ($release.Name -like "Canary*"){
        if(($insiderCount -gt 0 -or $prodCount -gt 0) -and $canaryCount -ge 2){
            $deleteList += $release
            continue
        }
        $canaryCount++
    } else {
        # Rogues. Delete.
        $deleteList += $release
    }
}

if($deleteList.Count -eq 0){
    Write-Host "No releases to delete."
} else {
    $deleteList | %{ Delete-Release $_ }
}
