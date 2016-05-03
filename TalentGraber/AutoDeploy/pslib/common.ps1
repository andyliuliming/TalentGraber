######################  common config     ############################
$buildType = "Debug"
$Prefix = "TalentGraber"

###################### storage related  #############################
$storageType="Standard_GRS"
$storageAccountName=$Prefix.ToLower() + "storage"
$searchVMStorageAccount = "searchstorage"
$StorageAccountEndpoint = ""

######################  database related  ############################
$goldDataBaseName = "talentgrabergold"
$collation = "SQL_Latin1_General_CP1_CI_AS"
$databaseEdition="Basic"
#https://msdn.microsoft.com/en-us/library/Dn546722.aspx
$databaseVersion=2.0
$databaseMaxSize=(2*1024*1024*1024)# this is in bytes
$databaseUserName=""
$databasePassword=""
$databaseServerName=""


######################  azure service related  ############################
$AffinityGroupName = "TalentGraberAffinityGroup"
$ProductAzureServiceName = "TalentGraber"
$ProductAzureServiceProjectName = "ProductAzureService"
$ProductServiceRoleName = "ProductService"
$ProductDaemonRoleProjectName = "WorkerRole"

$AllowedCrossDomainHost="*"
$ResourceGroupName="TalentGraberResourceGroup"

#################################### search related #######################################
$searchPort=":9200"

$ErrorActionPreference= 'Stop'
$currentFolder = pwd
$currentFolder="$currentFolder/.."

function ReplaceRealValue
{
    param
    (
    $TemplateOrigin
    )
    # get the account key
    $TemplateOrigin=$TemplateOrigin -replace "{DiagnosticStorageAccountName}",$storageAccountName
    $TemplateOrigin=$TemplateOrigin -replace "{DiagnosticStorageAccountKey}",$storageAccountKey
    $TemplateOrigin=$TemplateOrigin -replace "{DATABASE_SERVER_NAME}",$databaseServerName
    $TemplateOrigin=$TemplateOrigin -replace "{DATABASE_SUFFIX}",$databaseSuffix
    $TemplateOrigin=$TemplateOrigin -replace "{GOLD_DATABASENAME}",$goldDataBaseName
    $TemplateOrigin=$TemplateOrigin -replace "{DATABASE_USER_NAME}",$databaseUserName
    $TemplateOrigin=$TemplateOrigin -replace "{DATABASE_PASSWORD}",$databasePassword
    $TemplateOrigin=$TemplateOrigin -replace "{ProductServiceAddress}",$ProductServiceAddress
    $TemplateOrigin=$TemplateOrigin -replace "{StorageAccountEndpoint}",$StorageAccountEndpoint
    $TemplateOrigin=$TemplateOrigin -replace "{SearchServiceAddress}","http://$searchLBIPName$searchDnsSuffix$searchPort"
    # ##################### network related ##############################
    $TemplateOrigin=$TemplateOrigin -replace "{AllowedCrossDomainHost}",$AllowedCrossDomainHost
    return $TemplateOrigin
}