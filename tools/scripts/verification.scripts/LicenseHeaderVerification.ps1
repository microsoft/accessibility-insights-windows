# Copyright (c) Microsoft. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.
#.\headers.ps1 -target '.\Powershell test\' -licenseHeaderPath .\testLicenseHeader.txt -extensions *.xaml,*.xml,*.wxs,*.cs,*.ps1 -addIfAbsent $false
param(
    $target,
    $licenseHeaderPath,
    $extensions,
    $addIfAbsent=$false
)

$nl=([Environment]::NewLine)
$failedFiles = @()
$VanillaLicenseHeader=""
$SplitVanillaLicenseHeader=@()
$excludeList="(\\packages\\|\\bin\\|\\obj\\|Designer.cs)"

function Get-FileText($pathToFile){
   return Get-Content $pathToFile -Raw -Encoding UTF8
}

function Get-CopyrightHeader($extension){
    switch ( $extension )
    {
     {(($extension -eq '.xaml') -or ($extension -eq '.xml') -or ($extension -eq '.wxs'))}{
        return $xmlLicense;
      }
     .cs {
        return $cSharpLicense;
     }
     .ps1{
        return $psLicense;
     }
     #Add more extensions support here
     default{
        throw ("Extension not supported")
     }
    }
}

function Get-LineCommentedHeader($SplitVanillaLicenseHeader, $lineComment){
    $licenseHeader=$null
    foreach($line in $SplitVanillaLicenseHeader){
        if(-Not ([string]::IsNullOrWhiteSpace($line))){
                $licenseHeader = "$licenseHeader$lineComment $line$nl"
        }
    }
    return $licenseHeader; 
}

function Get-BlockCommentedHeader($SplitVanillaLicenseHeader, $blockCommentStart, $blockCommentEnd){
    if($SplitVanillaLicenseHeader.length -le 0){
        throw (" License missing ");
    }
    $licenseHeader = "$blockCommentStart " +  $SplitVanillaLicenseHeader[0] +$nl
    $noOfSpaces= " " * ($blockCommentStart.length + 1)
    if($SplitVanillaLicenseHeader.length -gt 1){
        for($lineNum=1; $lineNum -lt $SplitVanillaLicenseHeader.length; $lineNum++){
            if(-Not ([string]::IsNullOrWhiteSpace($SplitVanillaLicenseHeader[$lineNum]))){
                $licenseHeader = "$licenseHeader$noOfSpaces" + $SplitVanillaLicenseHeader[$lineNum]
            }
        }
    }
    return $licenseHeader + $blockCommentEnd; 
}

$SplitVanillaLicenseHeader = (Get-FileText $licenseHeaderPath).split($nl)
$xmlLicense = Get-BlockCommentedHeader $SplitVanillaLicenseHeader "<!--" "-->"
$cSharpLicense =  Get-LineCommentedHeader $SplitVanillaLicenseHeader "//"
$psLicense= Get-LineCommentedHeader $SplitVanillaLicenseHeader "#"
#Add more extensions support here

(Get-ChildItem $target\* -Include $extensions -Recurse) | Where {$_.FullName -notmatch $excludeList} | Foreach-Object {
# (Get-ChildItem $target\* -Include $extensions -Exclude $excludeList -Recurse)| Foreach-Object {
    $path = $_.FullName
    $copyRightHeader=Get-CopyrightHeader $_.Extension
    $fileContent=Get-FileText $path
    if ($fileContent -ne $Null -and $fileContent.Contains($copyRightHeader)){
        echo "$path has copyright header"
    } else {
       $failedFiles += $path
       if($addIfAbsent){
       "$copyRightHeader" + $fileContent | Set-Content -NoNewLine $path -Encoding UTF8
       echo "Added the header to $path"
       }
    }
}

if($failedFiles -gt 0){
    echo "$nl"
    $wording = ('do not','did not')[$addIfAbsent]
    $message = "The following files " + $wording + " have a copyright header $nl" + ($failedFiles -join $nl) + "$nl"
    if($addIfAbsent){
        $message
    } else {
        throw ($message)
    }
}
