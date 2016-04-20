. .\pslib\Invoke-MsBuild.ps1
if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}
. .\pslib\azureadapter.ps1

# wrap up the javascript and the css
npm install -g requirejs
r.js.cmd -o ..\ProductService\management-build.js

Invoke-MsBuild -Path ..\TalentGraberService.sln -P "/t:Clean" #-ShowBuildWindow

# build the authentication lib
Invoke-MsBuild -Path ..\Common\Common.csproj -P "/P:Configuration=$buildType" # -ShowBuildWindow
# build the AzureEnvironment

# build the database model project
Invoke-MsBuild -Path ..\Model\Model.csproj -P "/P:Configuration=$buildType" #-ShowBuildWindow

# build the service base project
Invoke-MsBuild -Path ..\ServiceBase\ServiceBase.csproj -P "/P:Configuration=$buildType" #-ShowBuildWindow

Write-Host "DiagnosticStorageAccountName is $storageAccountName"
$storageAccountKey = GetStorageAccountKey -ResourceGroupName $ResourceGroupName -StorageAccountName $storageAccountName
Write-Host "diagnosticStorageKey is $storageAccountKey"

$intelliTraceConnectionString="BaseEndpoint="+$StorageAccountEndpoint+";Protocol=https;AccountName="+$storageAccountName+";AccountKey="+$storageAccountKey

$intelliTraceParameter='"'+"$intelliTraceConnectionString"+'"'

Invoke-MsBuild -Path ..\ProductService\ProductService.csproj -P "/P:Configuration=$buildType"# -ShowBuildWindow
Invoke-MsBuild -Path ..\ProductAzureService\ProductAzureService.ccproj -P "/P:Configuration=$buildType /t:Publish /p:TargetProfile=Cloud /p:EnableIntelliTrace=true /p:IntelliTraceConnectionString=$intelliTraceParameter"# -ShowBuildWindow