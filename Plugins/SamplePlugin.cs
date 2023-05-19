namespace Sample.Dataverse.Plugins
{
    [CrmPluginRegistration(MessageNameEnum.Update
       , "eric_Table"
       , StageEnum.PreOperation
       , ExecutionModeEnum.Synchronous
       , "statecode"
       , "Sample.Dataverse.Plugins.SamplePlugin: Update of statecode"
       , 1, IsolationModeEnum.Sandbox
       , Description = "Sample.Dataverse.Plugins.SamplePlugin.Update"
       , FriendlyName = "Sample.Dataverse.Plugins.SamplePlugin.Update"
       , Id = "33e75130-de55-4b53-9093-56b86e9914db")]
    public class SamplePlugin : PluginBase
    {        
        public override void Execute(ILocalPluginContext localPluginContext)
        {
            var tracingService = localPluginContext.TracingService;
            var pluginContext = localPluginContext.PluginExecutionContext;
            var orgService = localPluginContext.OrganizationService;
        }
    }
}
