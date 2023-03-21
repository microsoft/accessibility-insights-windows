# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Set build variables that will be consumed by downstream scripts. It sets the following variables:

    A11yInsightsTagName              is the associated tag name (e.g., v1.1.2227.01) for this release
    A11yInsightsMinProdVersionTag    is the minimum supported Prod Release once this version deploys to Prod

The script is assumed to run from $(Release.PrimaryArtifactSourceAlias)

#>

Set-StrictMode -Version Latest
$script:ErrorActionPreference = 'Stop'

$unsignedManifestFile = Join-Path "manifest" "ReleaseInfo.json"
Write-Host $unsignedManifestFile
$json = Get-Content $unsignedManifestFile | Out-String
$info = $json | ConvertFrom-Json

Write-Host "Unaigned Manifest:"
Write-Host $json

$a11yInsightsTagName = "v" + $info.current_version
$a11yInsightsMinProdVersionTag = "v" + $info.production_minimum_version

# Diagnostic use only--does NOT set environment variables
Write-Host "Diagnostics values:"
Write-Host ("A11yInsightsTagName = " + $($a11yInsightsTagName))
Write-Host ("A11yInsightsMinProdVersionTag = " + $($a11yInsightsMinProdVersionTag))

# Set the environment variables
Write-Host "##vso[task.setvariable variable=A11yInsightsTagName]$($a11yInsightsTagName)"
Write-Host "##vso[task.setvariable variable=A11yInsightsMinProdVersionTag]$($a11yInsightsMinProdVersionTag)"
