. .\pslib\common.ps1
######################  common config     ############################
$location = "China East"
$Environment = "AzureChinaCloud"
$subscriptionId = "cc1624c7-3f1d-4ed3-a855-668a86e96ad8"

. .\pslib\common-credential.ps1

######################  azure service related  ############################
$ProductServiceAddress="http://"+$ProductAzureServiceName+".chinacloudapp.cn"
$StorageAccountEndpoint = "core.chinacloudapi.cn"
$databaseSuffix = "database.chinacloudapi.cn"

$AllowedCrossDomainHost="*"

$ErrorActionPreference= 'Stop'
$currentFolder = pwd
$currentFolder="$currentFolder/.."

$searchLBIPName = "searchlbip"
$searchClusterInstanceCount = 1
$searchVMNamePrefix = "searchVM"
$searchDnsSuffix =".chinaeast.cloudapp.chinacloudapi.cn"