param(
	[String]$clusterName = "TalentGraber",
	[String]$installRoot = "c:",
    [String]$deploymentStorageAccount="talentgraberstorage",
    [String]$StorageAccountEndpoint = "core.chinacloudapi.cn"
)

$resourcesUri = "https://$deploymentStorageAccount.blob.$StorageAccountEndpoint/searchresource/deploysearch.zip"
$resourcesZip = "$installRoot\deploysearch.zip"

$scriptBase = "http://$deploymentStorageAccount.blob.$StorageAccountEndpoint/searchresource"

$jdkInstaller = "${installRoot}\jdk-8u45-windows-x64.exe"
$jdkPath = "C:\Program Files\JAVA\JDK"

$elasticSearchZipFile = "${installRoot}\elasticsearch-1.7.0.zip"
$elasticSearchPath = "$installRoot\elasticsearch-1.7.0"
$elasticSearchConfigPath = join-path $elasticSearchPath  '\config\elasticsearch.yml'

$jdbcimportZipFile = "$installRoot\elasticsearch-jdbc-1.7.0.1-dist.zip"
$jdbcimportorPath = "$installRoot\elasticsearch-jdbc-1.7.0.1"
$sqljdbc4 = "$installRoot\sqljdbc4.jar"
$sqljdbc41 = "$installRoot\sqljdbc41.jar"
$rebuildIndex = "$scriptBase\RebuildIndex.bat"

$ikZipFile = "$installRoot\elasticsearch-analysis-ik-1.4.0.zip"
$ikPath = "$installRoot\elasticsearch-analysis-ik-1.4.0"
$ikDictZipFile = "$installRoot\ik.zip"

function unzip($zip_file, $destination)
{
    #unzip files
	$shell = new-object -com shell.application
	$zip = $shell.NameSpace($zip_file)
	$unzipDest = $shell.Namespace("${destination}\")

	$unzipped = $null

	foreach($item in $zip.items())
	{
		$unzipDest.copyhere($item, 0x14)
		$unzipped = Join-path $unzipDest.Self.Path -ChildPath $item.name
	}
    return $unzipped
}

function download_dependencies()
{
    if(-not (Test-Path $resourcesZip))
    {
        Invoke-WebRequest $resourcesUri -OutFile $resourcesZip
    }

    unzip $resourcesZip $installRoot
}

function install_jdk()
{
    if(Test-Path $jdkPath)
    {
        Write-Host "JDK already installed" -ForegroundColor Gray
        return
    }
	#install java
	Start-Process -FilePath $jdkInstaller -ArgumentList "/s INSTALLDIR=`"$jdkPath`"" -PassThru -Wait
}

function install_es()
{ 
    if(Test-Path $elasticSearchPath)
    {
        Write-Host "Elastic search bin already installed" -ForegroundColor Gray
    }

    #unzip elasticsearch
	unzip $elasticSearchZipFile ${installRoot}

	$elasticSearchBinPath = join-path $elasticSearchPath '\bin'

	$esAreadyInstalled = $FALSE		
	$configOK = $true  

    # Validate the configuration
	write-host "Validating config" -foregroundcolor green

	if (-not (test-path $elasticSearchBinPath))
	{
		write-host "ElasticSearch bin path doesn't exist: $elasticSearchBinPath" -foregroundcolor red
		return;	
	}

	if (-not (test-path $jdkPath))
	{
		write-host "Java JDK path doesn't exist: $jdkPath"  -foregroundcolor red
		return;
	}
		
	# Setting JAVA_HOME	
	write-host "setting JAVA_HOME system variable" -foregroundcolor green

	if ([environment]::GetEnvironmentVariable("JAVA_HOME", "machine") -eq $null)
	{
		[environment]::setenvironmentvariable("JAVA_HOME", $jdkpath, "machine")
		$env:JAVA_HOME = $jdkpath
	}
    write-host "JAVA_HOME system variable set to: " $jdkpath -foregroundcolor gray

	# Set cluster name
	Write-Host "Setting cluster name" -foregroundcolor green 
		

	(Get-Content $elasticSearchConfigPath) | ForEach-Object { $_ -replace "#?\s?cluster.name: .+" , "cluster.name: $clusterName" } | Set-Content $elasticSearchConfigPath
	Write-Host "Cluster name is: $clusterName" -foregroundcolor gray	

	#Install ElasticSearch as a service
	Write-Host "Install ElasticSearch as a service" -foregroundcolor green 

	cd $elasticSearchBinPath 

	if (-not (Get-Service "elasticsearch-service-x64" -ErrorAction SilentlyContinue) )
	{
		.\service.bat install
		Write-Host "ElasticSearch installed." -foregroundcolor gray
	}
	else
	{
		Write-Host "ElasticSearch have been already installed." -foregroundcolor gray
		$esAreadyInstalled = $TRUE
	}

	cd $elasticSearchBinPath 
}

function install_head()
{

	# Install HEAD plugin	
	Write-Host "Install HEAD plugin" -foregroundcolor green 

	.\plugin.bat -install mobz/elasticsearch-head

	Write-Host "HEAD plugin installed." -foregroundcolor gray
}

function install_jdbcimporter()
{
    if(Test-Path $jdbcimportorPath)
    {
        Write-Host "JDBC Importor already installed" -ForegroundColor Gray
        return
    }

    Write-Host "Install JDBC Importor" -foregroundcolor green 
    
	unzip $jdbcimportZipFile $installRoot

	Copy-Item $sqljdbc4  "$jdbcimportorPath\lib\sqljdbc4.jar"
	Copy-Item $sqljdbc41  "$jdbcimportorPath\lib\sqljdbc41.jar"
    Invoke-WebRequest $rebuildIndex -OutFile "$jdbcimportorPath\RebuildIndex.bat"


    Write-Host "Create schedule task for update index" -foregroundcolor gray 
    schtasks /create /tn "Rebuild Index" /tr "$jdbcimportorPath\RebuildIndex.bat" /mo 10 /sc MINUTE 

	Write-Host "JDBC Importor Installed" -foregroundcolor green 
}

function install_ik()
{    
    if(Test-Path $ikPath)
    {
        Write-Host "IK plugin already installed"
        return
    }

	Write-Host "Install IK plugin" -foregroundcolor green 

    $ikPluginPath = "$elasticSearchPath\plugins\elasticsearch-analysis-ik"

    New-Item $ikPath -type directory -ErrorAction SilentlyContinue
    New-Item $ikPluginPath -type directory -ErrorAction SilentlyContinue

    unzip $ikZipFile $ikPath;

    Copy-Item "$ikPath\*"  $ikPluginPath -Force
    
    Add-Content $elasticSearchConfigPath "`n"
    Add-Content $elasticSearchConfigPath "########################### IK ####################################`n"
    Add-Content $elasticSearchConfigPath "index.analysis.analyzer.ik.type : `"ik`"`n"
    
    
    $dictPath = unzip $ikDictZipFile $installRoot

    Copy-Item "$dictPath\*" "$elasticSearchPath\config\ik" -Recurse -Force

	Write-Host "Plugin IK Installed" -foregroundcolor gray
}

function finish_config()
{
    # Start ElasticSearch service	
	Write-Host "Start ElasticSearch service" -foregroundcolor green

	Start-Service 'elasticsearch-service-x64'

	Write-Host "Set service startup type to automatic" -foregroundcolor green 

	Set-Service 'elasticsearch-service-x64' -StartupType automatic

	# Enable ports through firewall 
	New-NetFirewallRule -DisplayName "Allow Port 9200 in" -Direction Inbound -LocalPort 9200 -Protocol TCP -Action Allow
	New-NetFirewallRule -DisplayName "Allow Port 9200 out" -Direction Outbound -LocalPort 9200 -Protocol TCP -Action Allow
	New-NetFirewallRule -DisplayName "Allow Port 9300 in" -Direction Inbound -LocalPort 9300 -Protocol TCP -Action Allow
	New-NetFirewallRule -DisplayName "Allow Port 9300 out" -Direction Outbound -LocalPort 9300 -Protocol TCP -Action Allow

	Write-Host "Completed with success" -foregroundcolor green 
}

Write-Host "Run deployment under account:" -foregroundcolor gray
whoami

Stop-Service elasticsearch-service-x64 -ErrorAction SilentlyContinue

download_dependencies
install_jdk
install_es
install_head
install_jdbcimporter
install_ik
finish_config

Restart-Service elasticsearch-service-x64 -ErrorAction SilentlyContinue
