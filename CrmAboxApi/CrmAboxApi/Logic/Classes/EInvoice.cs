using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes.Deserializing;
using CrmAboxApi.Logic.Classes.Helper;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;

namespace CrmAboxApi.Logic.Classes
{
    public class EInvoice : InvoiceEntity
    {

        private MShared sharedMethods = null;
        private string connectionString = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private ContactEntity contactEntity = null;
        public PharmacyEntity pharmacyEntity = null;
        public EInvoice()
        {
            sharedMethods = new MShared();

            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            contactEntity = new ContactEntity();
            pharmacyEntity = new PharmacyEntity();
        }



        private JObject GetCreateInvoiceJsonStructure(InvoiceCreate invoiceProperties, Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
            
            try
            {
                if (invoiceProperties != null)
                {
                    jObject.Add($"{InvoiceFields.PurchaseDate}", DateTime.Now.ToString("yyyy-MM-dd"));


                    if (invoiceProperties.patientId>0)
                    {
                        jObject.Add($"{InvoiceSchemas.CustomerContactSchema}@odata.bind", $"/{contactEntity.EntityPluralName}({ContactFields.IdAboxPatient}={invoiceProperties.patientId})");
                    }


                    if (!(String.IsNullOrEmpty(invoiceProperties.billImageUrl)))
                        jObject.Add(InvoiceFields.InvoiceImageWebUrl, invoiceProperties.billImageUrl);



                    if (!(String.IsNullOrEmpty(invoiceProperties.pharmacyId)))
                    {
                        jObject.Add($"{InvoiceSchemas.Pharmacy}@odata.bind", $"/{pharmacyEntity.EntityPluralName}({PharmacyFields.Id}='{invoiceProperties.pharmacyId}')");
                    }

                    if (!(String.IsNullOrEmpty(invoiceProperties.billId)))
                        jObject.Add(InvoiceFields.InvoiceNumber, invoiceProperties.billId);


                    if (invoiceProperties.products!=null)
                    {
                        var serialized = JsonConvert.SerializeObject(invoiceProperties.products);
                        jObject.Add($"{InvoiceFields.ProductsSelectedJson}",serialized);
                    }

                    if (invoiceProperties.idFromDatabase != null)
                    {
                        jObject.Add($"{InvoiceFields.IdAboxInvoice}", invoiceProperties.idFromDatabase);
                    }


                    ////////////

                }

                return jObject;
            }
            catch (Exception ex)
            {

                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                jObject = null;
                return jObject;
            }
        }



        private OperationResult InvoiceCreateRequest(JObject jsonObject, Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {

                if (jsonObject != null)
                {
                    try
                    {
                        using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                        {
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                            client.DefaultRequestHeaders.Add("MSCRM.SuppressDuplicateDetection", "false");

                            MethodBase m = MethodBase.GetCurrentMethod();
                            string url = $"{this.EntityPluralName}?$select={InvoiceFields.EntityId}";



                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Url:{url} Data:{jsonObject.ToString(Formatting.None)}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);


                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PostAsync(url, c).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[InvoiceFields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Factura creada correctamente en el CRM";
                                operationResult.IsSuccessful = true;
                                operationResult.Data = userId;
                            }
                            else
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                                JObject userId = JObject.Parse(body.ToString());
                                CrmWebAPIError err = userId.ToObject<CrmWebAPIError>();

                                if (err != null)
                                {
                                    log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);
                                }



                                operationResult.Code = "";
                                operationResult.Message = "Error al crear la factura en el CRM";
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Crear Queue de procesos fallidos en BD para reprocesar
                        LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, ex);
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);
                        operationResult.Code = "";
                        operationResult.Message = ex.ToString();
                        operationResult.IsSuccessful = false;
                        operationResult.Data = null;
                    }
                }

                return operationResult;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }


        public OperationResult CreateInvoice(InvoiceCreate invoiceCreateRequest, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                InvoiceCreate invoiceCreateProperties = invoiceCreateRequest;

                if (invoiceCreateProperties != null)
                {
                  

                    JObject newInvoice = this.GetCreateInvoiceJsonStructure(invoiceCreateProperties, processId);

                   
                    responseObject = this.InvoiceCreateRequest(newInvoice, processId);
                }
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);

                //Logger.Error(ex,"");
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }


    }
}