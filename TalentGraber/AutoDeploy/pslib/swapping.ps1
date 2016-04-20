if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
. .\pslib\azureadapter.ps1

$ErrorActionPreference= 'Continue'

Write-Host "Swapping the environment $ProductAzureServiceName"
Move-AzureDeployment -ServiceName $ProductAzureServiceName