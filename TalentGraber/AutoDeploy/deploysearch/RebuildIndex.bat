set DIR=C:\elasticsearch-jdbc-1.7.0.1
set LIB=%DIR%\lib
set BIN=%DIR%\bin

cd %DIR%

set JAVA_CMD="C:\Program Files\JAVA\JDK\bin\java"

set JDBC_IMPORTOR=%JAVA_CMD% ^
       -cp "%lib%\*" ^
       org.xbib.tools.Runner ^
       org.xbib.tools.JDBCImporter

set TALENTCANDIDATES={ ^
    "type": "jdbc", ^
    "jdbc": { ^
        "driver": "com.microsoft.sqlserver.jdbc.SQLServerDriver", ^
        "url": "jdbc:sqlserver://talentgraberdatabase.database.chinacloudapi.cn:1433;databaseName=talentgrabergold", ^
        "user": "TalentGraber", ^
        "password": "User@123", ^
        "sql": "select * , id as _id from dbo.TalentCandidates", ^
	"elasticsearch" : {^
             "cluster" : "TalentGraber",^
             "host" : "localhost",^
             "port" : 9300^
        },^
        "index" : "talentcandidates" ^
    } ^
}


echo %TALENTCANDIDATES% | %JDBC_IMPORTOR%
