# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
<#
.SYNOPSIS
Set build variables that will be consumed by downstream scripts. It sets the following variables:

    A11yInsightsMsiFile    is the relative path to the MSI file (e.g., 1.1.2227.01\AccessibilityInsights.msi)
    A11yInsightsVersion    is the version (e.g., 1.1.2227.01) for this release
    A11yInsightsTagName    is the associated tag name (e.g., v1.1.2227.01) for this release

This script is assumed to run from $(Release.PrimaryArtifactSourceAlias)

#>

Set-StrictMode -Version Latest
$script:ErrorActionPreference = 'Stop'

$msiFolder = ".\msi"
$a11yInsightsVersion = Get-ChildItem -Name -Directory $($msiFolder)
$a11yInsightsTagName = "v" + $($a11yInsightsVersion)
$a11yInsightsMsiFile = $($a11yInsightsVersion) + "\" + "AccessibilityInsights.msi"

# Diagnostic use only--does NOT set environment variables
Write-Host "Diagnostics values:"
Write-Host ("A11yInsightsVersion = " + $($a11yInsightsVersion))
Write-Host ("A11yInsightsTagName = " + $($a11yInsightsTagName))
Write-Host ("A11yInsightsMsiFile= " + $($a11yInsightsMsiFile))

# Set the environment variables
Write-Host "##vso[task.setvariable variable=A11yInsightsVersion]$($a11yInsightsVersion)"
Write-Host "##vso[task.setvariable variable=A11yInsightsTagName]$($a11yInsightsTagName)"
Write-Host "##vso[task.setvariable variable=A11yInsightsMsiFile]$($a11yInsightsMsiFile)"
