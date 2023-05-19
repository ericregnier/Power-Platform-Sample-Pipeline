$authConfig = Get-Content .\Scripts\AuthConfig.json -Raw | ConvertFrom-Json;
$config = Get-Content .\Scripts\ScriptConfig.json -Raw | ConvertFrom-Json;

$environmentUrl = $config.EnvironmentUrl;
$environmentName = $environmentUrl.Replace("https://", "").Replace(".crm6.dynamics.com", "");

Write-Host "Authenticating to $environmentName" -ForegroundColor Yellow;
$clientId = $authConfig.ClientId;
$clientSecret = $authConfig.ClientSecret;

pac auth delete --name $environmentName 
pac auth create --url $environmentUrl --name $environmentName --tenant $authConfig.TenantId --applicationId $clientId --clientSecret $clientSecret

$solutions = $config.SolutionUnpackFolders.Split(",")
Foreach ($solution in $solutions) {
	Write-Host "Syncing the $solution to the current state of the solution in $environmentName" -ForegroundColor Yellow;
	cd $solution
	pac solution sync --processCanvasApps --packagetype 'Both'
	cd ..
}

Write-Host "Done, remember to commit only what you changed!" -ForegroundColor Green;