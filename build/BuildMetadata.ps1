$basePath = $args[0]

if ([System.String]::IsNullOrWhiteSpace($($basePath)))
{
    Write-Host 'ERROR: No basePath specified!'
    exit 1
}

$status = git status | Out-String
Write-host "*** Result of git status:" $($status)

$revParse = git rev-parse HEAD | Out-String
Write-Host "*** Result of git rev-parse HEAD:" $($revParse)

$buildNumber = ${env:BUILD_BUILDNUMBER}
Write-Host "*** Build Number:" $($revParse)

$rawVersion = [IO.File]::ReadLines(${env:temp} + "\A11yInsightsVersionInfo.cs")[0]
Write-Host "*** Raw Version:" $($revParse)

$branch = $($status).Split([System.Environment]::Newline)[0].Split(" ")[2]
$sha = $($revParse).Split([System.Environment]::Newline)[0]
$productVersion = $($rawVersion).Split('\"')[1]

$folderPath = [IO.Path]::Combine($($basePath), 'signed', $($productVersion))
if (![IO.Directory]::Exists($($folderPath)))
{
    Write-Host 'ERROR: Path not found: ' $($folderPath)
    exit 2
}

$metadataFilePath = [IO.Path]::Combine($($folderPath), 'build_info.json')

$json = @{"sha"=$($sha); "branch"=$($branch); "buildNumber"=$($buildNumber); "productVersion"=$($productVersion)} | ConvertTo-Json
Write-Host $($metadataFilePath)
Write-Host $($json)
#$($json) | Out-File $($metadataFilePath)