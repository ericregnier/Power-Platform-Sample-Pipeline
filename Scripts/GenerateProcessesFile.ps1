<#
.SYNOPSIS
    Generates a json file of processes (workflows, business rules and cloud flows) and their state from the target env.

.DESCRIPTION
    OutFile
        The output json file to generate including path and file name
    Steps:
    1) Run ./GenerateProcessesFile.ps1 -OutFile "filename.json". Can me ran locally or within a pipeline.
    2) Enter your credential (optional) when prompted
#>
Param(
    [string] [parameter(Mandatory = $true)] $ConnectionString,
    [string] [parameter(Mandatory = $true)] $OutFile
)

if (!(Get-Module "Microsoft.Xrm.Data.Powershell")) { 
  Install-Module -Name Microsoft.Xrm.Data.Powershell -Force -Verbose -Scope CurrentUser -AllowClobber
}
else {
  Update-Module -Name Microsoft.Xrm.Data.Powershell -Force -Verbose
}

if (!$ConnectionString) {
  $connection = Get-CrmConnection -Interactive
}
else {
  $connection = Get-CrmConnection -ConnectionString $ConnectionString
}

$fetchXmlFlows = @"
<fetch>
  <entity name="workflow">
    <attribute name="workflowid" />
    <attribute name='name' />
    <attribute name='statecode' />
    <attribute name='statuscode' />
    <attribute name='category' />
    <filter type="and">
      <condition attribute="category" operator="eq" value="5" />
    </filter>
  </entity>
</fetch>
"@;

$fetchXmlWorkflows = @"
<fetch>
  <entity name="workflow">
    <attribute name="workflowid" />
    <attribute name='name' />
    <attribute name='statecode' />
    <attribute name='statuscode' />
    <attribute name='category' />
    <filter type="and">
      <condition attribute="type" operator="eq" value="1" />
      <condition attribute="rendererobjecttypecode" operator="null" />
      <filter type="or">
        <condition attribute="category" operator="eq" value="0" />
        <filter type="and">
          <condition attribute="category" operator="eq" value="1" />
          <condition attribute="languagecode" operator="eq-userlanguage" />
        </filter>
        <condition attribute="category" operator="eq" value="3" />
        <condition attribute="category" operator="eq" value="4" />
      </filter>
    </filter>
  </entity>
</fetch>
"@;

$fetchXmlBusinessRules = @"
<fetch>
  <entity name="workflow">
    <attribute name="workflowid" />
    <attribute name='name' />
    <attribute name='statecode' />
    <attribute name='statuscode' />
    <attribute name='category' />
    <filter type="and">
      <condition attribute="type" operator="eq" value="1" />
      <filter type="and">
        <condition attribute="rendererobjecttypecode" operator="null" />
        <filter type="or">
          <condition attribute="category" operator="eq" value="2" />
          <filter type="and">
            <condition attribute="category" operator="eq" value="1" />
            <condition attribute="languagecode" operator="eq-userlanguage" />
          </filter>
        </filter>
      </filter>
    </filter>
  </entity>
</fetch>
"@;

$fetchXmls = $fetchXmlFlows, $fetchXmlWorkflows, $fetchXmlBusinessRules
$processes = @()
foreach ($fetchXml in $fetchXmls) { 
    $response = Get-CrmRecordsByFetch -Fetch $fetchXml -conn $connection

    if ($response.CrmRecords.Count -gt 0) {
        $processes = $processes + $response.CrmRecords | Select-Object -Property workflowid, name, category, statecode, statuscode
        Write-Host "Added" $response.CrmRecords.Count "to list for `n" $fetchXml
    }
    else {
        Write-Host "All cloud flows are turned on for `n" $fetchXml
    }
}
Write-Host "Writing" $processes.Count "to json file"
$processes | ConvertTo-Json | Out-File $OutFile

if (!$ConnectionString) {
  Read-Host -Prompt "Json file generated. Press Enter to exit."
}