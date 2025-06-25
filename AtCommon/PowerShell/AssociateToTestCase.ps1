[CmdletBinding()]
Param(
   [Parameter(Mandatory=$true)]
   [string]$TestCaseId,

   [Parameter(Mandatory=$true)]
   [string]$TestName
)

# send the request
$encodedCredentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f "",$Global:AzureApiKey)))
$headers = @{ Authorization = "Basic $encodedCredentials" }
$url = "https://dev.azure.com/agilityhealth-net/_apis/wit/workitems/$($TestCaseId)?api-version=5.0"

$body = @"
[
{
"op": "add",
"path": "/fields/Microsoft.VSTS.TCM.AutomatedTestName",
"value": "$TestName"
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

$result = Invoke-RestMethod -Method Patch -Uri $url -Headers $headers -Body $body -ContentType 'application/json-patch+json'

# parse the response
write-host $result