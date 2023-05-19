using Microsoft.Xrm.Sdk;

namespace Sample.Dataverse.Services
{
    public class SampleService
    {
        private readonly IOrganizationService _orgService;
        private readonly ITracingService _tracingService;

        public SampleService(IOrganizationService orgService, ITracingService tracingService = null)
        {
            this._orgService = orgService;
            this._tracingService = tracingService;
        }

        public void DoStuff()
        {

        }
    }
}