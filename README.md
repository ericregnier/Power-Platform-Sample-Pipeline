# Introduction 
Power Platform Sample Project and Azure DevOps Pipeline for Dataverse CI/CD
Sample repo for:
- All Dataverse related components (eg Plugins) and solutions
- Master/reference data used by the solution and scripts to promote that data to upstreams envs
- Scripts to get the target state of processes (workflows, flows and business rules) and set them in the target envs

## Dataverse Solutions
| Name | Description | Notes |
| ----------- | ----------- | ----------- |
| SampleCustomConnectorsSolution | Solution containing all custom connector objects | Limitation of custom connectors which need to be imported before the main solution |
| SampleSolution | Main solution containing all non-custom connector objects such as Tables and Apps  | Dependent on SampleCustomConnectorsSolution solution |

# Getting Started
- Some scripts uses the pac commands, download and install the lasts Power Platform CLI from https://aka.ms/PowerAppsCLI
- Setup the local configuration correctly to use the scripts. See: [ScriptConfig.json](#ScriptConfig.json) and [AuthConfig.json](#AuthConfig.json)
- To export and unpack the run `Scripts\ExportAndUnpack.ps1`
- After unpacking, review your changes and only commit what you've changed.

# ScriptConfig.json
- **DataverseConnectionString** - Set as `AuthType=ClientSecret;ClientId={0};ClientSecret={1};Url={2};LoginPrompt=Never` and do not modify
- **EnvironmentUrl** - The Dataverse environment URL. Format: `https://<org>.crm6.dynamics.com`
- **SolutionUnpackFolders** - The folders where the `SolutionName` is unpacked. Normally it's same as the `SolutionName`
- **TenantId** - AAD Tenant ID, can be found in Azure.

# AuthConfig.json
- **ClientId** - The application ID (aka service principal aka SPN aka app user) that has access to Dataverse. Can be found in Azure.
- **ClientSecret** - The secret for the ClientId. Can be found in KeyVault.
> **_IMPORTANT:_**
> This file should not be committed to the repo as it contains sensitive auth parameters and can be different for every developer.
