using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Sample.Dataverse.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        private readonly string _className;
       
        protected PluginBase()
        {
            _className = GetType().Name;
        }

        protected PluginBase(string unsecureString, string secureString)
        {
            _className = GetType().Name;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var localContext = new LocalPluginContext(serviceProvider);

            localContext.Trace($"Entered {_className}.Execute().");

            try
            {
                Execute(localContext);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                localContext.Trace(ex);

                throw;
            }
            finally
            {
                localContext.Trace($"Exited {_className}.Execute().");
            }
        }

        public abstract void Execute(ILocalPluginContext localContext);
    }

    public interface ILocalPluginContext
    {
        IOrganizationService OrganizationService { get; }

        IPluginExecutionContext PluginExecutionContext { get; }

        IServiceEndpointNotificationService CloudService { get; }

        ITracingService TracingService { get; }

        void Trace(string message, params object[] o);

        void Trace(FaultException<OrganizationServiceFault> exception);

        string MessageName { get; }
    }

    public class LocalPluginContext : ILocalPluginContext
    {
        private readonly IServiceProvider _serviceProvider;
        private IPluginExecutionContext _pluginExecutionContext;
        private ITracingService _tracingService;
        private IOrganizationServiceFactory _organizationServiceFactory;
        private IOrganizationService _organizationService;
        private IServiceEndpointNotificationService _cloudService;

        public LocalPluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
        }

        public IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return _pluginExecutionContext ??
                       (_pluginExecutionContext = (IPluginExecutionContext)_serviceProvider.GetService(typeof(IPluginExecutionContext)));
            }
        }

        public IServiceEndpointNotificationService CloudService
        {
            get
            {
                return _cloudService ??
                           (_cloudService = (IServiceEndpointNotificationService)_serviceProvider.GetService(typeof(IServiceEndpointNotificationService)));
            }
        }

        public IOrganizationService OrganizationService
        {
            get
            {
                return _organizationService ?? (_organizationService =
                    OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
            }
        }

        public string MessageName => PluginExecutionContext.MessageName;

        public ITracingService TracingService
        {
            get
            {
                return _tracingService ?? (_tracingService = (ITracingService)_serviceProvider.GetService(typeof(ITracingService)));
            }
        }

        private IOrganizationServiceFactory OrganizationServiceFactory
        {
            get
            {
                return _organizationServiceFactory ??
                    (_organizationServiceFactory = (IOrganizationServiceFactory)_serviceProvider.GetService(typeof(IOrganizationServiceFactory)));
            }
        }

        public void Trace(string message, params object[] o)
        {
            if (PluginExecutionContext == null)
            {
                SafeTrace(message, o);
            }
            else
            {
                SafeTrace(
                    "{0}, Correlation Id: {1}, Initiating User: {2}",
                    string.Format(message, o),
                    PluginExecutionContext.CorrelationId,
                    PluginExecutionContext.InitiatingUserId);
            }
        }

        public void Trace(FaultException<OrganizationServiceFault> exception)
        {
            // Trace the first message using the embedded Trace to get the Correlation Id and User Id out.
            Trace("Exception: {0}", exception.Message);

            SafeTrace(exception.StackTrace);

            if (exception.Detail != null)
            {
                SafeTrace("Error Code: {0}", exception.Detail.ErrorCode);
                SafeTrace("Detail Message: {0}", exception.Detail.Message);
                if (!string.IsNullOrEmpty(exception.Detail.TraceText))
                {
                    SafeTrace("Trace: ");
                    SafeTrace(exception.Detail.TraceText);
                }

                foreach (var item in exception.Detail.ErrorDetails)
                {
                    SafeTrace("Error Details: ");
                    SafeTrace(item.Key);
                    SafeTrace(item.Value.ToString());
                }

                if (exception.Detail.InnerFault != null)
                {
                    Trace(new FaultException<OrganizationServiceFault>(exception.Detail.InnerFault));
                }
            }
        }

        private void SafeTrace(string message, params object[] o)
        {
            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
            {
                return;
            }

            TracingService.Trace(message, o);
        }
    }
}
