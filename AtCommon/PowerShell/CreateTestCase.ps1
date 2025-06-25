<#
.SYNOPSIS

Creates a new test case in Azure DevOps

.DESCRIPTION

Generates a new Test Case in Azure DevOps and associates a test method with the test case. A personal access token is 
required in you PowerShell profile. To add it run the following commands:

if (!(Test-Path -Path $PROFILE.CurrentUserAllHosts))
{ New-Item -Type File -Path $PROFILE.CurrentUserAllHosts -Force }

Add-Content $profile.CurrentUSerAllHosts -Value '$AzureApiKey = "ENTER_YOUR_PAT_HERE"'

The script can be run by passing the path to a .csv file, or providing the Fully Qualified Method Name and Tags

.PARAMETER File

Path to the .csv file. The file needs to have 2 columns: Name and Tags. The Tags must be delimited with semicolons.

.PARAMETER TestName

Fully Qualified method name. The test case will be named after the short method name.

.EXAMPLE

.\CreateTestCase.ps1 -TestName 'Sample.Test.Name' -Tags 'SampleTag1; SampleTag2'

.EXAMPLE

.\CreateTestCase.ps1 -File "$Env:USERPROFILE\Documents\TestCases.csv"

#>

[CmdletBinding()]
Param(
   [Parameter(Mandatory=$false, Position=0)]
   [string]$File,

   [Parameter(Mandatory=$false)]
   [string]$TestName,

   [Parameter(Mandatory=$false)]
   [string]$Tags
)


# if file parameter - parse it 
if ($File -ne "") {
    $testCases = ConvertFrom-Csv $(Get-Content $File)
}
else
{
    if ($TestName -eq "") {
        throw "You must provide a -TestName or -File parameter"
    }

    # create an array with the TestName and Tags that were passed
    $props = @{
        Name = $TestName
        Tags = $Tags
    }
    $testCases = @(new-object PSObject -Property $props)
}


# loop through the array of test cases
foreach ($case in $testCases) {

    # send the request
    $encodedCredentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f "",$Global:AzureApiKey)))
    $headers = @{ Authorization = "Basic $encodedCredentials" }
    $url = 'https://dev.azure.com/AgilityHealth-Net/Agility%20Health/_apis/wit/workitems/$Test%20Case?api-version=5.0-preview.3'

    $body = @"
[
  {
    "op": "add",
    "path": "/fields/System.Title",
    "value": "$($case.Name.Split('.')[-1])"
  },
  {
    "op": "add",
    "path": "/fields/System.Tags",
    "value": "$($case.Tags)"
  },
  {
    "op": "add",
    "path": "/fields/Microsoft.VSTS.TCM.AutomatedTestName",
    "value": "$($case.Name)"
  },
  {
    "op": "add",
    "path": "/fields/Microsoft.VSTS.TCM.AutomatedTestStorage",
    "value": "AgilityHealth_Automation.dll"
  },
  {
    "op": "add",
    "path": "/fields/Microsoft.VSTS.TCM.AutomatedTestId",
    "value": "$($(new-guid).ToString())"
  },
  {
    "op": "add",
    "path": "/fields/Microsoft.VSTS.TCM.AutomatedTestType",
    "value": "Selenium"
  },
   {
    "op": "add",
    "path": "/fields/Microsoft.VSTS.TCM.AutomationStatus",
    "value": "Automated"
  }
]
"@

    $result = Invoke-RestMethod -Method Post -Uri $url -Headers $headers -Body $body -ContentType 'application/json-patch+json'
    
    # parse the response
    write-host $result
    
}
