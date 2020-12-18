using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement
{
    public class InvoiceCreate : IPlugin
    {

        private MShared sharedMethods = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            sharedMethods = new MShared();

            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                // The InputParameters collection contains all the data passed in the message request.

                /*Esta validación previene la ejecución del Plugin de cualquier
                * transacción realizada a través del Web API desde Abox*/
                if (context.InitiatingUserId == new Guid("7dbf49f3-8be8-ea11-a817-002248029f77"))
                {
                    return;
                }

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.
                    Entity invoice = (Entity)context.InputParameters["Target"];

                    if (invoice.LogicalName != "invoice")
                    {
                        return;
                    }
                    else
                    {

                        Entity fullInvoice = service.Retrieve("invoice",invoice.Id,new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                        var k = "";
                    }

                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());

                try
                {
                    sharedMethods.LogPluginFeedback(new LogClass
                    {
                        Exception = ex.ToString(),
                        Level = "error",
                        ClassName = this.GetType().ToString(),
                        MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = "Excepción en plugin",
                        ProcessId = ""
                    }, trace);
                }
                catch (Exception e)
                {
                    trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + e.ToString());
                }

                if (ex.Data["HasFeedbackMessage"] != null)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                else
                {
                    throw new InvalidPluginExecutionException(Constants.GeneralPluginErrorMessage);
                }
            }
        }

    }
}
