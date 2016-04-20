if($Environment -eq "AzureChinaCloud")
{
. .\pslib\common-china.ps1
}else
{
. .\pslib\common-global.ps1
}

$existingSubscriptions=Get-AzureSubscription

$existingSubscription = $null
if ($existingSubscriptions -ne  $null){
    foreach ( $item in $existingSubscriptions)
    {
        if($item.SubscriptionId -eq $subscriptionId){
            $existingSubscription=$item
        }
    }
}

if($existingSubscription -eq $null)
{
    Add-AzureAccount -Environment $Environment
}

Select-AzureSubscription -SubscriptionId $subscriptionId

#$existingRmSubscriptions = Get-AzureRmSubscription
#$existingRmSubscription=$null

#if ($existingRmSubscriptions -ne  $null){
#    foreach ( $item in $existingRmSubscriptions)
#    {
#        if($item.SubscriptionId -eq $subscriptionId){
#            $existingRmSubscription=$item
#        }
#    }
#}
#if($existingRmSubscription -eq $null)
#{
$EnvironmentToUse = Get-AzureRmEnvironment -Name $Environment
Add-AzureRmAccount -Environment $EnvironmentToUse
#}

Select-AzureRmSubscription -SubscriptionId $subscriptionId