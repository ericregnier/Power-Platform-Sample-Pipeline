Param(
    [string] [parameter(Mandatory = $true)] $ConnectionString,
    [string] [parameter(Mandatory = $true)] $WorkingDirectory, #tried to specify directly in the pipeline task, but not working
)

$Conn = Get-CrmConnection -ConnectionString $ConnectionString
cd $WorkingDirectory

# Script to run after deployment, triggered by pipeline
#.\CreateTeams.ps1