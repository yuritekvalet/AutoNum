using Microsoft.Xrm.Sdk;
using System;

namespace AutoincrementAccount
{
    public interface ILocalPluginContext
    {
        IOrganizationService OrganizationService { get; }
        IPluginExecutionContext PluginExecutionContext { get; }
        ITracingService TracingService { get; }
    }

    public class LocalPluginContext : ILocalPluginContext
    {
        private readonly IServiceProvider _serviceProvider;
        private IPluginExecutionContext _pluginExecutionContext;
        private ITracingService _tracingService;
        private IOrganizationServiceFactory _organizationServiceFactory;
        private IOrganizationService _organizationService;

        public IOrganizationService OrganizationService
        {
            get
            {
                return _organizationService ?? (_organizationService = OrganizationServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId));
            }
        }

        public IPluginExecutionContext PluginExecutionContext
        {
            get
            {
                return _pluginExecutionContext ??
                       (_pluginExecutionContext = (IPluginExecutionContext)_serviceProvider.GetService(typeof(IPluginExecutionContext)));
            }
        }

        public ITracingService TracingService
        {
            get
            {
                return _tracingService ?? (_tracingService = (ITracingService)_serviceProvider.GetService(typeof(ITracingService)));
            }
        }

        private IOrganizationServiceFactory OrganizationServiceFactory
        {
            get { return _organizationServiceFactory ?? (_organizationServiceFactory = (IOrganizationServiceFactory)_serviceProvider.GetService(typeof(IOrganizationServiceFactory))); }
        }

        public LocalPluginContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new InvalidPluginExecutionException("serviceProvider");
        }

       
    }
}
