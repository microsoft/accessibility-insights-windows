$basePath = $args[0]

if ([System.String]::IsNullOrWhiteSpace($($basePath)))
{
    Write-Host 'ERROR: No basePath specified!'
    exit 1
}

$branch = (git status | Out-String).Split([System.Environment]::Newline)[0].Split(" ")[2]
$sha = (git rev-parse HEAD | Out-String).Split([System.Environment]::Newline)[0]
$buildNumber = ${env:BUILD_BUILDNUMBER}
$productVersion = [IO.File]::ReadLines(${env:temp} + "\A11yInsightsVersionInfo.cs")[0].Split('\"')[1]

$folderPath = [IO.Path]::Combine($($basePath), 'signed', $($productVersion))
if (![IO.Directory]::Exists($($folderPath)))
{
    Write-Host 'ERROR: Path not found: ' $($folderPath) -Separator ' '
    exit 2
}

$metadataFilePath = [IO.Path]::Combine($($folderPath), 'build_info.json')

$json = @{"sha"=$($sha); "branch"=$($branch); "buildNumber"=$($buildNumber); "productVersion"=$($productVersion)} | ConvertTo-Json
Write-host $($metadataFilePath)
Write-Host $($json)
#$($json) | Out-File $($metadataFilePath)