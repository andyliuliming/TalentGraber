1. 创建资源 

```makeresources-search.ps1 ```

2. 通过CustomScriptExtension部署ElasticSearch 

```deploy-search.ps1 ```
    
    从github的release page下载ElasticSearch部署所需资源，将这些资源解压后放在下面的目录结构
    ```
    HappyZLService/AutoDeploy/deploysearch/ 
        elasticsearch-1.7.0.zip 
        elasticsearch-1.7.0.zip 
        elasticsearch-analysis-ik-1.4.0.zip 
        elasticsearch-jdbc-1.7.0.1-dist.zip 
        elasticsearch-jdbc.zip 
        ik.zip 
        jdk-8u45-windows-x64.exe 
        sqljdbc4.jar 
        sqljdbc41.jar 
    ```
4. 通过ElasticSearch执行RebuildIndex 

```build-search-index.ps1```

5. 进入到search cluster上的机器里面跑以下命令

```schtasks /create /tn "Rebuild Index" /tr "C:\elasticsearch-jdbc-1.7.0.1\RebuildIndex.bat" /mo 10 /sc MINUTE 