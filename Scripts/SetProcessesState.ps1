<#
.SYNOPSIS
    Updates in bulk the state of processes based on a json config file.

.DESCRIPTION
    SettingsFilePath
        The input json settings file to read the processes from including path and file name
        Format of json:
        [
            {
                "workflowid":  "00000000-0000-0000-0000-000000000000",
                "name":  "Name",
                "statecode":  "Draft",
                "statuscode":  "Draft"
            }
        ]
    NumberOfChildFlows
        Not currently used - Expected level of child processes/flows to retry state change. Not currently used
#>

Param(
    [string] [parameter(Mandatory = $true)] $ConnectionString,
    [string] [parameter(Mandatory = $true)] $SettingsFilePath,
    [string] [parameter()] $ImpersonatedUserGuid,
    [int] [parameter()] $NumberOfChildFlows = 3
)

$Conn = Get-CrmConnection -ConnectionString $ConnectionString
Write-Host "Connected to" $Conn.ConnectedOrgFriendlyName

if ($ImpersonatedUserGuid) {
    $Conn.OrganizationWebProxyClient.CallerId = $ImpersonatedUserGuid #can't turn on with a SPN!
}

$processes = Get-Content $SettingsFilePath -Raw | ConvertFrom-Json 
$failedProcesses = @()

$retryCount = $NumberOfChildFlows #retrying in case there our child flows.
do {
    Write-Host "Retries Left:" $retryCount

    if($processes.Count -gt 0) {
        foreach ($process in $processes) {
            try {
                $currentState = Get-CrmRecord -EntityLogicalName workflow -Id $process.workflowid -Fields statecode,statuscode

                if($currentState.statecode -ne $process.statecode -or $currentState.statuscode -ne $process.statuscode) {
                    Set-CrmRecordState -EntityLogicalName workflow -Id $process.workflowid -StateCode $process.statecode -StatusCode $process.statuscode
                    Write-Host "Updated state of" $process.name "("$process.workflowid") to" $process.statecode -ForegroundColor Green
                }        
            }
            catch {
                $failedProcesses = $failedProcesses + $process
                Write-Host "Could not update state of" $process.name "("$process.workflowid")."
            }
        }
        $processes = $failedProcesses
        $failedProcesses = @()
        $retryCount--
    }
    else {
        $retryCount = 0
        Write-Host "No processes to deactivate" -ForegroundColor Yellow
    }
}
While ($processes.Count -gt 0 -and $retryCount -gt 0)