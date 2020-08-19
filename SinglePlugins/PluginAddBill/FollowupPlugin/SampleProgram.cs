using FollowupPlugin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

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
        [DataContract]
        public class AboxServiceResponse
        {
            [DataMember]
            public Header header { get; set; }
            [DataMember]
            public Response response { get; set; }
        }

        [DataContract]
        public class Bill
        {
            [DataMember]
            public int patientId { get; set; }
            [DataMember]
            public string pharmacyId { get; set; }
            [DataMember]
            public string billId { get; set; }
            [DataMember]
            public string billDate { get; set; }
            [DataMember]
            public string billImageUrl { get; set; }
            [DataMember]
            public Product[] products { get; set; }
        }

        [DataContract]
        public class Product
        {
            [DataMember]

            public string id { get; set; }
            [DataMember]

            public int quantity { get; set; }
        }



        private string webAddress;

        /// <summary>
        /// The plug-in constructor.
        /// </summary>
        /// <param name="config">The Web address to access. An empty or null string
        /// defaults to accessing www.bing.com. The Web address can use the HTTP or
        /// HTTPS protocol.</param>
        public FollowupPlugin(string config)
        {
            if (String.IsNullOrEmpty(config))
            {
                webAddress = "https://apidev.aboxplan.com/member/signup/kyn";
            }
            else
            {
                webAddress = config;
            }
        }


        private async void requestAboxService2(QuickSignupRequest.Request quickSignupRequest, ITracingService tracingService)
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", wsKey);
            client.BaseAddress = new Uri("https://apidev.aboxplan.com/member/signup/kyn");

            //Rather than using a typed object, construct the JSON object manually using strings
            string jsonBody = "{'patientid':null,'country':'"+quickSignupRequest.country+"','userType':'"+quickSignupRequest.userType+"','personalinfo':{'idtype':'01','id':'"+quickSignupRequest.personalinfo.id+"','name':'"+quickSignupRequest.personalinfo.name+"','lastname':'"+quickSignupRequest.personalinfo.lastname+"','secondlastname':'"+quickSignupRequest.personalinfo.secondlastname+"','gender':null,'dateofbirth':null,'password':'"+quickSignupRequest.personalinfo.password+"'},'contactinfo':{'province':null,'canton':null,'district':null,'phone':'"+quickSignupRequest.contactinfo.phone+"','mobilephone':'','address':null,'email':'"+quickSignupRequest.contactinfo.email+"','password':'"+quickSignupRequest.contactinfo.password+"'},'patientincharge':null,'medication':null,'interests':null,'otherInterest':null}";

            tracingService.Trace("Antes de llamar " + webAddress);
            //Rather than using PostJsonAsync use PostAsync
            HttpResponseMessage response = await client.PostAsync("", new StringContent(jsonBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            tracingService.Trace("Despues de llamar " + webAddress);
            if (response.IsSuccessStatusCode)
            {
                
                string result = await response.Content.ReadAsStringAsync();
                
                //Rather than using DeserializeObject, parse the json string manually
                //var parsingResp = ((string)result).ParseWSResponse();
                //var json = new JavaScriptSerializer();
                //var data = json.Deserialize<Dictionary<string, Dictionary<string, string>>[]>(result);
            }
            else
            {
               
            }
        }

        private Entity RetrieveEntityById(IOrganizationService service, string strEntityLogicalName, Guid guidEntityId)

        {

            Entity RetrievedEntityById = service.Retrieve(strEntityLogicalName, guidEntityId, new ColumnSet(true)); //it will retrieve the all attrributes

            return RetrievedEntityById;

        }

        private bool requestAboxService(QuickSignupRequest.Request quickSignupRequest,ITracingService tracingService)
        {
            bool lvResponse = false;
            string json = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(quickSignupRequest.GetType());
                serializer.WriteObject(memoryStream, quickSignupRequest);
                json = System.Text.Encoding.Default.GetString(memoryStream.ToArray());
            }


            try
            {
                //string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjUwMDAwMDAwMSIsImlhdCI6MTU1OTI0NzYyNCwiZXhwIjoxNTU5MzM0MDI0fQ.EE0DI8fvewgDKtlB89eZzQLtx0vW56_uar3l7ERCej";
                //string requestData = json;
                string requestData = "{'patientid':null,'country':'" + quickSignupRequest.country + "','userType':'" + quickSignupRequest.userType + "','personalinfo':{'idtype':'01','id':'" + quickSignupRequest.personalinfo.id + "','name':'" + quickSignupRequest.personalinfo.name + "','lastname':'" + quickSignupRequest.personalinfo.lastname + "','secondlastname':'" + quickSignupRequest.personalinfo.secondlastname + "','gender':null,'dateofbirth':null,'password':'" + quickSignupRequest.personalinfo.password + "'},'contactinfo':{'province':null,'canton':null,'district':null,'phone':'" + quickSignupRequest.contactinfo.phone + "','mobilephone':'','address':null,'email':'" + quickSignupRequest.contactinfo.email + "','password':'" + quickSignupRequest.contactinfo.password + "'},'patientincharge':null,'medication':null,'interests':null,'otherInterest':null}";

                string URI = "https://apidev.aboxplan.com/member/signup/kyn";
                //string myParameters = "param1=value1&param2=value2&param3=value3";

                string postResult = "";
                //using (WebClient wc = new WebClient())
                //{
                //    wc.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                //    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjUwMDAwMDAwMSIsImlhdCI6MTU1OTI0NzYyNCwiZXhwIjoxNTU5MzM0MDI0fQ.EE0DI8fvewgDKtlB89eZzQLtx0vW56_uar3l7ERCej";
                //    tracingService.Trace("Antes de llamar " + postResult);
                //    postResult = wc.UploadString(URI, requestData);
                //}

                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                   //client.UseDefaultCredentials = true;
                    string responseBytes = client.UploadString(webAddress,requestData);
                    string response = responseBytes;
                    tracingService.Trace(response);

                    // For demonstration purposes, throw an exception so that the response
                    // is shown in the trace dialog of the Microsoft Dynamics CRM user interface.
                   //throw new InvalidPluginExecutionException("WebClientPlugin completed successfully.");
                }

                QuickSignupRequest.ServiceResponse aboxServiceResponse = null;
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(postResult));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(aboxServiceResponse.GetType());
                aboxServiceResponse = ser.ReadObject(ms) as QuickSignupRequest.ServiceResponse;
                tracingService.Trace("Respuesta servicio " + postResult);
                ms.Close();

                if (aboxServiceResponse.header.code == 0)
                {
                   

                    if (aboxServiceResponse.header.code == 0)
                    {
                        if (aboxServiceResponse.response.code == "MEMEX-0002")
                        {
                            lvResponse = true;

                        }
                        else
                        {
                            lvResponse = false;

                        }


                    }


                }
                else
                {
                    lvResponse = false;
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
                Entity contact = (Entity)context.InputParameters["Target"];


                // Verify that the target entity represents a contact.
                // If not, this plug-in was not registered correctly.
                //new_aboxinvoice
                if (contact.LogicalName != "contact")
                    return;

                try
                {

                    #region AbInvoice

                    //Entity parentEntity = service.Retrieve("contact", eAbInvoice.Id, new ColumnSet(true));
                    // Get the Relationship name for which this plugin fired

                    //string relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                    //new_abfacturaid
                    //Relationship rel = (Relationship)context.InputParameters["Relationship"];



                    // Check the "Relationship Name" with your intended one
                    //new_contact_aboxinvoice
                    //if (relationshipName != "new_contact_aboxinvoice")

                    //{

                    //    return;

                    //}

                    #endregion


                    QuickSignupRequest.Request request = new QuickSignupRequest.Request();

                    #region Contacto
                    //EntityReference entityReference = null;
                    //var re = contact.RelatedEntities;

                    //if (contact.Attributes.Contains("new_patientowner"))
                    //{
                    //    entityReference = (EntityReference)contact.Attributes["new_patientowner"];
                    //}

                    //Entity parentContact = null;
                    //if (entityReference != null)
                    //{

                    //    parentContact = service.Retrieve("contact", entityReference.Id, new ColumnSet(true));
                    //}

                    //if (parentContact != null)
                    //{

                      

                    //    //lvBill.patientId=Convert.ToInt32(eAbInvoice.Attributes["new_webid"]);
                    //    //new_abinvoicenumber

                    //    //lvBill.patientId = Convert.ToInt32(parentContact.Attributes["new_webid"].ToString());
                      

                    

                      

                    //}
                    //else
                    //{
                    //    return;
                    //}


                    #endregion

                 


                    #region Llamado Servicio


                    request.country= "CR";
                    request.userType= "01";
                    request.personalinfo = new QuickSignupRequest.Request.Personalinfo();
                    request.personalinfo.idtype= contact.Attributes["new_usertype"].ToString();
                    request.personalinfo.id= contact.Attributes["new_id"].ToString();
                    request.personalinfo.name = contact.Attributes["firstname"].ToString();
                    request.personalinfo.lastname = contact.Attributes["lastname"].ToString();
                    request.personalinfo.secondlastname = contact.Attributes["new_secondlastname"].ToString();
                    request.personalinfo.password= contact.Attributes["new_password"].ToString();
                    request.contactinfo = new QuickSignupRequest.Request.Contactinfo();
                    request.contactinfo.phone= contact.Attributes["telephone2"].ToString();
                    request.contactinfo.email= contact.Attributes["emailaddress1"].ToString();

                    this.requestAboxService(request,tracingService);

                   // bool requestDoneSuccessfully = this.requestAboxService(request);

                   

                    #endregion


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
