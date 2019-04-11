# Constants
$owner = 'Microsoft'
$repositoryName = 'accessibility-insights-windows'

$pathToRepo = $($args[0])

function WriteDivider()
{
	Write-Host '-----------------------'
}

function GetStatus($trueOrFalse)
{
	If ($trueOrFalse -eq $true)
	{
		return 'PASSED'
	}

	return 'FAILED'
}

function ExtractChannelInfo($channel)
{
	$uri = [String]::Format('file:{0}\Channels\{1}\release_info.json', $pathToRepo, $channel).Replace('\', '/')
	$data = Invoke-RestMethod -Method Get -Uri $uri
	Write-Host 'Channel Info:' $data.default
	return $data.default
}

function ValidateReleaseExists($version)
{
	$tagName = 'v' + $version
	$parsedVersion = [System.Version]::new($($version))
	$valid = $knownTags.Contains($tagName) -and ($parsedVersion.Revision -gt 0)
	Write-Host $(GetStatus($valid)) 'Does release' $tagName 'exist?'
	return $valid
}

function ValidateWebResourceExists($webResource)
{
	$client = New-Object System.Net.WebClient
	$data = $client.DownloadString($webResource)
	
	$valid = $data.Length -gt 0
	Write-Host $(GetStatus($valid)) 'Does web resource' $webResource 'exist?'
	If ($valid -ne $true)
	{
		Write-Host 'StatusCode:', $statusCode
		Write-Host 'Content-Length', $contentLength
	}
	return $valid
}

function ValidateChannelInfo($channel)
{
	Write-Host 'Checking Info for' $channel
	$channelInfo = ExtractChannelInfo($channel)
	$isCurrentVersionValid = ValidateReleaseExists($channelInfo.current_version)
	$isMinimumVersionValid = ValidateReleaseExists($channelInfo.minimum_version)
	$isInstallerUrlValid = ValidateWebResourceExists($channelInfo.installer_url)
	$isReleaseNotesUrlValid = ValidateWebResourceExists($channelInfo.release_notes_url)
	WriteDivider
	return $isCurrentVersionValid -and $isMinimumVersionValid -and $isInstallerUrlValid -and $isReleaseNotesUrlValid
}

# Load the octokit dll
Add-Type -Path ((Get-Location).Path + '\Octokit.0.32.0\lib\net45\Octokit.dll')

# Get a new client with an appropriate product header value
$productHeader = [Octokit.ProductHeaderValue]::new("AIWindows-ReleaseInfoValidation")
$client = [Octokit.GitHubClient]::new($productHeader)
$client.Credentials = [Octokit.Credentials]::new("$(gitPATX)")

# Get tag names for existing releases
$knownTags = @()
Write-Host 'Existing Releases'
$releases = $client.Repository.Release.GetAll($owner, $repositoryName).Result
ForEach($release in $releases)
{
	$knownTags += $release.TagName
}
$knownTags

#$releases | Select-Object -Property Name, TagName, PreRelease, Id | Format-Table
WriteDivider

$isProductionValid = ValidateChannelInfo('Production')
$isInsiderValid = ValidateChannelInfo('Insider')

$isValidOverall = $isProductionValid -and $isInsiderValid

If ($isValidOverall -eq $true)
{
	Write-Host 'Successfully validated release_info.json files'
}
Else
{
	Write-Error 'Invalid release_info.json file(s)'
}
