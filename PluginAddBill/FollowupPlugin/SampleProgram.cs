using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.IO;
using System.ServiceModel;

namespace PowerApps.Samples
{
    /// <summary>
    /// The plug-in creates a task activity after a new account is created. The activity reminds the user to
    /// follow-up with the new account customer one week after the account was created.
    /// </summary>
    /// <remarks>Register this plug-in on the Create message, account entity, and asynchronous mode.
    /// </remarks>

    public sealed class FollowupPlugin : IPlugin
    {

        //Respoonse
        public class Headerdetails
        {
        }

        public class Header
        {
            public int code { get; set; }
            public string message { get; set; }
            public Headerdetails headerdetails { get; set; }
        }

        public class Details
        {
            public int idPaciente { get; set; }
        }

        public class Response
        {
            public string code { get; set; }
            public string message { get; set; }
            public Details details { get; set; }
        }

        public class AboxServiceResponse
        {
            public Header header { get; set; }
            public Response response { get; set; }
        }
        
        public class Bill
        {
            public int patientId { get; set; }
            public string pharmacyId { get; set; }
            public string billId { get; set; }
            public string billDate { get; set; }
            public string billImageUrl { get; set; }
            public Product[] products { get; set; }
        }

        public class Product
        {
            public string id { get; set; }
            public int quantity { get; set; }
        }


        private Entity RetrieveEntityById(IOrganizationService service, string strEntityLogicalName, Guid guidEntityId)

        {

            Entity RetrievedEntityById = service.Retrieve(strEntityLogicalName, guidEntityId, new ColumnSet(true)); //it will retrieve the all attrributes

            return RetrievedEntityById;

        }

        private void requestAboxService()
        {
            Bill bill = new Bill();
            string json = JsonConvert.SerializeObject(bill);

            try
            {
                string ServiceName = "";
                string url = "";
                string requestData = json;
                var webrequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/json; charset=utf-8";
                using (Stream webstream = webrequest.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webstream))
                {

                    requestWriter.Write(requestData);

                }
                try
                {
                    using (var response = webrequest.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var jsonrequest = reader.ReadToEnd();

                        AboxServiceResponse abServiceResponse = JsonConvert.DeserializeObject<AboxServiceResponse>
                            (jsonrequest);

                        if (abServiceResponse.header.code == 0)
                        {
                            if (abServiceResponse.response.code == "MEMEX-0002")
                            {
                                //TODO:Create logic if successfull

                            }
                            else
                            {
                               

                            }


                        }
                        else
                        {
                           
                        }
                    }

                }
                catch (System.Net.WebException wex)
                {

                    string error = "";
                    string statusCode = "";
                    if (wex.Response != null)
                    {
                        using (var errorResponse = (System.Net.HttpWebResponse)wex.Response)
                        {

                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                error = reader.ReadToEnd();

                                //TODO: use JSON.net to parse this string and look at the error message
                            }
                        }

                    }

                   
                }


            }
            catch (Exception ex)
            {

               
            }
        }



        /// <summary>
        /// Execute method that is required by the IPlugin interface.
        /// </summary>
        /// <param name="serviceProvider">The service provider from which you can obtain the
        /// tracing service, plug-in execution context, organization service, and more.</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));



            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity entity = (Entity)context.InputParameters["Target"];

                // Verify that the target entity represents an account.
                // If not, this plug-in was not registered correctly.
                if (entity.LogicalName != "account")
                    return;

                try
                {


                    // Create a task activity to follow up with the account customer in 7 days. 
                    //Entity followup = new Entity("task");

                    //followup["subject"] = "Send e-mail to the new customer.";
                    //followup["description"] =
                    //    "Follow up with the customer. Check if there are any new issues that need resolution.";
                    //followup["scheduledstart"] = DateTime.Now.AddDays(7);
                    //followup["scheduledend"] = DateTime.Now.AddDays(7);
                    //followup["category"] = context.PrimaryEntityName;




                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    // Refer to the account in the task activity.
                    if (context.OutputParameters.Contains("id"))
                    {

                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "account";

                        //followup["regardingobjectid"] =
                        //new EntityReference(regardingobjectidType, regardingobjectid);
                        Entity entity2 = RetrieveEntityById(service, "account", regardingobjectid);

                        String strAccountName = string.Empty;

                        if (entity.Attributes.Contains("name"))

                        {

                            strAccountName = entity2.Attributes["name"].ToString();

                        }


                    }

                    // Obtain the organization service reference.





                    // Create the task in Microsoft Dynamics CRM.
                    tracingService.Trace("FollowupPlugin: Successfully created the task activity.");

                    //service.Create(followup);
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in the FollowupPlugin plug-in.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowupPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
