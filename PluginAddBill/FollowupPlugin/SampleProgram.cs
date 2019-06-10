using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;

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
            string json = "";
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(pBill.GetType());
                serializer.WriteObject(memoryStream, pBill);
                json = System.Text.Encoding.Default.GetString(memoryStream.ToArray());
            }


            try
            {
                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjUwMDAwMDAwMSIsImlhdCI6MTU1OTI0NzYyNCwiZXhwIjoxNTU5MzM0MDI0fQ.EE0DI8fvewgDKtlB89eZzQLtx0vW56_uar3l7ERCej";
                string requestData = json;

                string URI = "https://apidev.aboxplan.com/purchases/create";
                //string myParameters = "param1=value1&param2=value2&param3=value3";

                string postResult = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    wc.Headers[HttpRequestHeader.Authorization] = "Bearer " + token;
                    postResult = wc.UploadString(URI, requestData);
                }

                AboxServiceResponse aboxServiceResponse = null;
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(postResult));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(aboxServiceResponse.GetType());
                aboxServiceResponse = ser.ReadObject(ms) as AboxServiceResponse;
                ms.Close();

                if (aboxServiceResponse.header.code == 0)
                {
                    if (aboxServiceResponse.response.code == "MEMEX-0002")
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
                Entity eAbInvoice = (Entity)context.InputParameters["Target"];


                // Verify that the target entity represents a contact.
                // If not, this plug-in was not registered correctly.
                //new_aboxinvoice
                if (eAbInvoice.LogicalName != "new_aboxinvoice")
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


                    Bill lvBill = new Bill();

                    #region Contacto
                    EntityReference eReference = null;
                    var re = eAbInvoice.RelatedEntities;

                    if (eAbInvoice.Attributes.Contains("new_patientowner"))
                    {
                        eReference = (EntityReference)eAbInvoice.Attributes["new_patientowner"];
                    }

                    Entity parentContact = null;
                    if (eReference != null)
                    {

                        parentContact = service.Retrieve("contact", eReference.Id, new ColumnSet(true));
                    }

                    if (parentContact != null)
                    {

                      

                        //lvBill.patientId=Convert.ToInt32(eAbInvoice.Attributes["new_webid"]);
                        //new_abinvoicenumber

                        //lvBill.patientId = Convert.ToInt32(parentContact.Attributes["new_webid"].ToString());
                      

                    

                      

                    }
                    else
                    {
                        return;
                    }


                    #endregion

                    
                    #region Farmacia

                    /*Entity name: new_pharmacies
                     * Web id Field: new_webid
                     * Country Field: new_Country
                     * CR - 1
                     * PA - 2
                     * GT - 3
                     * HN - 4
                     * DO - 5
                     *                      * -----------
                     *Relationship name: new_new_pharmacies_new_aboxinvoice
                     * Relationship lookup:new_invoice_lookupId*/

                    EntityReference invoicePharmacyReference = null;

                    if (eAbInvoice.Attributes.Contains("new_invoice_lookupid"))
                    {
                        invoicePharmacyReference = (EntityReference)eAbInvoice.Attributes["new_invoice_lookupid"];
                    }

                    Entity parentPharmacy = null;

                    if (invoicePharmacyReference!=null)
                    {
                        parentPharmacy=service.Retrieve("new_pharmacies", invoicePharmacyReference.Id, new ColumnSet(true));
                    }

                    if (parentPharmacy!=null)
                    {
                    lvBill.pharmacyId = parentPharmacy.Attributes["new_webid"].ToString();

                    }




                    #endregion

                    
                    #region Imagen


                    //entityimage

                    EntityImageCollection imageCollection = eAbInvoice.GetAttributeValue<EntityImageCollection>("entityimage");

                    #endregion


                    #region Productos

                    lvBill.products = null;

                    /*entidad: new_abdrug
                     * Nombre relacion: new_aboxinvoice_new_abdrug
                     * nombre web Id: new_WebID
                     * nombre Pais: new_Country
                     * nombre Field lookup: new_InvoiceDrugRel
                     */
                    EntityReference productInvoiceReference = null;
                    EntityCollection invoiceProducts = null;

                    //new_patientowner

                    if (true)
                    {
                        QueryExpression qe = new QueryExpression("new_abdrug") {
                            ColumnSet=new ColumnSet(true),
                            LinkEntities = {
                               new LinkEntity()
                               {
                                   LinkFromEntityName="new_abdrug",
                                   LinkToEntityName="new_aboxinvoice",
                                   Columns=new ColumnSet(false),
                                   LinkCriteria=new FilterExpression()
                                   {
                                       FilterOperator=LogicalOperator.And,
                                       Conditions ={
                                           new ConditionExpression("new_aboxinvoiceid",ConditionOperator.Equal,eAbInvoice.Id)
                                       }
                                   }
                                  
                               }
                            }

                        };
                       

                        ///////////
                        ///////////
                        ///
                        //ConditionExpression condition = new ConditionExpression();
                        //condition.AttributeName = "new_invoicedrugrel";
                        //condition.Operator = ConditionOperator.Equal;
                        //condition.Values.Add(eAbInvoice.Id.ToString());
                        //FilterExpression filter = new FilterExpression();
                        //filter.Conditions.Add(condition);

                        //QueryExpression qe = new QueryExpression("new_abdrug");
                        //qe.ColumnSet.AddColumns("new_name","new_webid");
                        //qe.ColumnSet.AddColumns("new_invoicedrugrel");

                        //qe.Criteria.AddFilter(filter);

                        invoiceProducts = service.RetrieveMultiple(qe);

                        lvBill.products = new Product[invoiceProducts.Entities.Count];
                        int count = invoiceProducts.Entities.Count;
                        for (int i = 0; i < count; i++)
                        {
                            lvBill.products[i].id = invoiceProducts.Entities[i].Attributes["new_WebID"].ToString();
                            lvBill.products[i].quantity = 1;
                        }


                       

                    }

                  
                    



                    #endregion


                    #region Llamado Servicio

                    lvBill.billId = eAbInvoice.Attributes["new_abinvoicenumber"].ToString();
                    lvBill.billDate = "2019-03-15";
                    lvBill.billImageUrl = "";
                    lvBill.products = null;


                    bool requestDoneSuccessfully = this.requestAboxService(lvBill);

                    if (!requestDoneSuccessfully)
                    {
                        return;
                    }

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
