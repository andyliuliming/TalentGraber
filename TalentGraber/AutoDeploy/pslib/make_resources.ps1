if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
. .\pslib\azureadapter.ps1

$ErrorActionPreference= 'Continue'

MakeSureAffinityGroupExists -Location $location -AffinityGroupName $AffinityGroupName
# create the cloud services if not exists
MakeSureAzureServiceExists -ServiceName $ProductAzureServiceName -AffinityGroupName $AffinityGroupName

MakeSureResourceGroupExists -ResourceGroupName $ResourceGroupName -Location $location

$databaseSecpasswd = ConvertTo-SecureString $databasePassword -AsPlainText -Force
$databaseCredential = New-Object System.Management.Automation.PSCredential ($databaseUserName, $databaseSecpasswd)
MakeSureDatabaseServerExists -ResourceGroupName $ResourceGroupName -ServerName $databaseServerName -Credential $databaseCredential -Location $location
MakeSureDatabaseExists -ResourceGroupName $ResourceGroupName -DatabaseName $goldDataBaseName -ServerName $databaseServerName -DatabaseEdition $databaseEdition -DatabaseMaxSize $databaseMaxSize

MakeSureStorageAccountExists -StorageName $storageAccountName -Location $location


MakeSureSearchClusterExists -ResourceGroupName $ResourceGroupName `
                            -Location $location -storageAccountName $searchVMStorageAccount -searchVMNamePrefix $searchVMNamePrefix -StorageAccountEndpoint $StorageAccountEndpoint `
                            -lbIpName $searchLBIPName -numberOfInstances $searchClusterInstanceCount
