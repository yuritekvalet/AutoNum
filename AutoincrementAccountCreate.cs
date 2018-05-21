using Microsoft.Xrm.Sdk;
using System;

namespace AutoincrementAccount
{
    /// 
    /// Плагин осуществляет автонумерацию сущности Account. 
    /// Данная реализация возможно также через сторониие плагины, которые
    /// осуществляют приращения значения, зарегистированных сущностей.
    /// Приращение счетчика и получение его значения также возможна через Workflow.
    /// 
    public class AutoincrementAccountCreate : IPlugin
        {
        private static readonly object SyncLock = new object();
        public void Execute(IServiceProvider serviceProvider)
        {
            var localContext = new LocalPluginContext(serviceProvider);
            if (localContext.PluginExecutionContext.MessageName == "Create")
                ExecuteAutoincrementAccount(localContext);
        }


        protected void ExecuteAutoincrementAccount(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException("Не удалось получить локальный контекст");
            }
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;

            string entityname = "account";
            string fieldName = "accountnumber";
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {
                    if (context.IsInTransaction)
                    {
                        lock (SyncLock)
                        {
                            AutoNumberGenerate AutoGen = new AutoNumberGenerate();
                            Entity entity = (Entity)context.InputParameters["Target"];
                            string entityLogicalName = entity.LogicalName;

                            if (entityLogicalName == entityname)
                            {
                                AutoGen.GenerateAutoNumber(service, entity, entityLogicalName,fieldName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    localContext.TracingService.Trace("AutoNumber Plugin Exception: " + ex);
                    throw new InvalidPluginExecutionException(ex.ToString());
                }
            }
        }
    }
}
