if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
. .\pslib\azureadapter.ps1

$ProductServiceDiagnosticConfig = "$currentFolder/$ProductAzureServiceProjectName/bin/$buildType/app.publish/Extensions/PaaSDiagnostics.$ProductServiceRoleName.PubConfig.xml"
$diagnosticConfig = Get-Content -Encoding UTF8 -Path $ProductServiceDiagnosticConfig
$diagnosticConfig = ReplaceRealValue -TemplateOrigin $diagnosticConfig
Set-Content -Encoding UTF8 -Path $ProductServiceDiagnosticConfig -Value $diagnosticConfig

$ProductWorkRoleDiagnosticConfig = "$currentFolder/$ProductAzureServiceProjectName/bin/$buildType/app.publish/Extensions/PaaSDiagnostics.$ProductDaemonRoleProjectName.PubConfig.xml"
$diagnosticConfig = Get-Content -Encoding UTF8 -Path $ProductWorkRoleDiagnosticConfig
$diagnosticConfig = ReplaceRealValue -TemplateOrigin $diagnosticConfig
Set-Content -Encoding UTF8 -Path $ProductWorkRoleDiagnosticConfig -Value $diagnosticConfig


Write-Host "setting the product service diagnostic extension"
SetDiagnosticExtension -ServiceName $ProductAzureServiceName -StorageContext $diagnosticStorageContext -RoleName $ProductDaemonRoleProjectName -DiagnosticConfigurationPath $ProductWorkRoleDiagnosticConfig -Slot Production
SetDiagnosticExtension -ServiceName $ProductAzureServiceName -StorageContext $diagnosticStorageContext -RoleName $ProductServiceRoleName -DiagnosticConfigurationPath $ProductServiceDiagnosticConfig -Slot Production