[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True,Position=1)]
   [string]$auth
)

$encodedCredentials = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($auth))
$headers = @{ Authorization = "Basic $encodedCredentials" }

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$Url = "https://agilityhealth-net.visualstudio.com/Agility%20Health/_apis/build/builds?api-version=4.1&statusFilter=inProgress&repositoryId=cc9d244e-d1a2-438e-9a73-b4c2566ef225&definitions=47,51,26,57,42,59,64"

$result = Invoke-RestMethod -Method 'Get' -Uri $url -Headers $headers 
if($result.count -gt 1){throw "Other builds in progress"}