if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
. .\pslib\azureadapter.ps1


$ErrorActionPreference= 'Continue'


$Slot="Staging"

Remove-AzureDeployment -ServiceName $ProductAzureServiceName -Slot $Slot -Force
Write-Host "$ProductAzureServiceName is removed"
