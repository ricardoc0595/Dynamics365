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

        private bool requestAboxService(Bill pBill)
        {
            bool lvResponse = false;
            string json = JsonConvert.SerializeObject(pBill);

            try
            {
                string token = "";
                string url = "http://104.43.138.232:9006/create";
                string requestData = json;
                var webrequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                webrequest.Method = "POST";
                webrequest.Headers["Authorization"] = "Bearer " + token;
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
                                lvResponse = true;
                            }
                            else
                            {

                                lvResponse = false;
                            }


                        }
                        else
                        {
                            lvResponse = false;
                        }
                    }
                    return lvResponse;

                }
                catch (System.Net.WebException wex)
                {
                    lvResponse = false;
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

                    return lvResponse;

                   
                }


            }
            catch (Exception ex)
            {
                lvResponse = false;
                return lvResponse;
            }
        }



        /// <summary>
        /// Execute method that is required by the IPlugin interface.
        /// </summary>
        /// <param name="serviceProvider">The service provider from which you can obtain the
        /// tracing service, plug-in execution context, organization service, and more.</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            #region Inicializacion servicios y contexto

            //Extract the tracing service for use in debugging sandboxed plug-ins.
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));


            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));


            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            #endregion



            // The InputParameters collection contains all the data passed in the message request.
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.
                Entity eContact = (Entity)context.InputParameters["Target"];

                
                // Verify that the target entity represents a contact.
                // If not, this plug-in was not registered correctly.
                //new_aboxinvoice
                if (eContact.LogicalName != "contact")
                    return;

                try
                {


                    #region Contacto
                    
                    Bill lvBill = new Bill();

                    lvBill.patientId=Convert.ToInt32(eContact.Attributes["new_webid"]);
                    lvBill.billId = "Bill-365-1";
                    lvBill.billDate = "";
                    lvBill.billImageUrl = "";
                    lvBill.products = null;
                    #endregion



                    #region AbInvoice

                    // Get the Relationship name for which this plugin fired

                    string relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;

                    Relationship rel = (Relationship)context.InputParameters["Relationship"];

                   
                    Entity abInvoiceEntity = null;

                    // Check the "Relationship Name" with your intended one
                    //new_contact_aboxinvoice
                    if (relationshipName != "new_contact_aboxinvoice")

                    {

                        return;

                    }

                    #endregion


                    bool requestDoneSuccessfully = this.requestAboxService(lvBill);

                    if (!requestDoneSuccessfully)
                    {
                        return;
                    }

                    // Refer to the account in the task activity.
                    //if (context.OutputParameters.Contains("id"))
                    //{

                    //    Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                    //    string regardingobjectidType = "account";

                    //    //followup["regardingobjectid"] =
                    //    //new EntityReference(regardingobjectidType, regardingobjectid);
                    //    Entity entity2 = RetrieveEntityById(service, "account", regardingobjectid);

                    //    String strAccountName = string.Empty;

                    //    if (eContact.Attributes.Contains("name"))

                    //    {

                    //        strAccountName = entity2.Attributes["name"].ToString();

                    //    }


                    //}

                    // Obtain the organization service reference.
                    
                    // Create the task in Microsoft Dynamics CRM.
                    //tracingService.Trace("FollowupPlugin: Successfully created the task activity.");

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
