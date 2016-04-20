$Environment = "AzureCloud"
. .\azureauth.ps1
. .\pslib\deploy_to_staging.ps1

#. .\pslib\clear_static_public_ip_production.ps1
#. .\pslib\clear_static_public_ip_staging.ps1
. .\pslib\swapping.ps1
#. .\pslib\set_static_public_ip_to_production.ps1

#. .\pslib\set_static_public_ip_to_staging.ps1
. .\pslib\removestaging.ps1
#. .\pslib\enablediagnostic.ps1