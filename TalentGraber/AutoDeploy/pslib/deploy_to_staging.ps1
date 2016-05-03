. .\pslib\azureadapter.ps1

if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
.\pslib\common-credential.ps1
.\pslib\build.ps1

Set-AzureSubscription -SubscriptionId $subscriptionId -CurrentStorageAccountName $storageAccountName

$storageAccountKey = GetStorageAccountKey -ResourceGroupName $ResourceGroupName -StorageAccountName $storageAccountName
Write-Host "DiagnosticStorageAccountName is $storageAccountName and key is $storageAccountKey"

$diagnosticStorageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

# Set-AzureDeployment -Upgrade we should check whether the azure service exists.
# i don't know why the CurrentStorageAccountName is mandatory...

#replace the values in the service service/diagnostic config
$ProductServiceConfig = "$currentFolder/$ProductAzureServiceProjectName/bin/$buildType/app.publish/ServiceConfiguration.Cloud.cscfg"
$cloudConfig = Get-Content -Encoding UTF8 -Path $ProductServiceConfig
$cloudConfig = ReplaceRealValue -TemplateOrigin $cloudConfig
Set-Content -Encoding UTF8 -Path $ProductServiceConfig -Value $cloudConfig

#todo generate the CloudDeploy.cscfg according to the cloud because.
$deploymentStorageKey = GetStorageAccountKey -ResourceGroupName $ResourceGroupName -StorageAccountName $storageAccountName
Write-Host "DeploymentStorageAccountName is $storageAccountName "
$deploymentStorageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $deploymentStorageKey

##### product service ############################################################
Write-Host "deploying the product service"
NewOrUpgradeDeployment -ServiceName $ProductAzureServiceName `
            -Package "$currentFolder/$ProductAzureServiceProjectName/bin/$buildType/app.publish/ProductAzureService.cspkg" `
            -Configuration $ProductServiceConfig `
            -StorageContext $deploymentStorageContext `
            -Slot Staging

###### wait for the role to be running status ###########################
While($true)
{
    $ProductAzureServiceStagingDeployment = Get-AzureDeployment -ServiceName $ProductAzureServiceName -Slot Staging
    if($ProductAzureServiceStagingDeployment.Status -eq "Running")
    {
        Write-Host "role status is $ProductAzureServiceStagingDeployment.Status"
        break;
    }
    else
    {
        Write-Host "$ProductAzureServiceName status is "+$ProductAzureServiceStagingDeployment.Status
        Start-Sleep -s 5
    }
}
