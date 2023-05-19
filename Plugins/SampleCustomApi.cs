using System;

namespace Sample.Dataverse.Plugins
{
    [CrmPluginRegistration("eric_SampleCustomApi")]
    public class SampleCustomApi : PluginBase
    {
        public override void Execute(ILocalPluginContext localPluginContext)
        {
            var tracingService = localPluginContext.TracingService;
            var pluginContext = localPluginContext.PluginExecutionContext;
            var orgService = localPluginContext.OrganizationService;

            var inputParam = pluginContext.InputParameters.Contains("InputParam");
            pluginContext.OutputParameters["OutputParam"] = String.Empty;
        }
    }
}
