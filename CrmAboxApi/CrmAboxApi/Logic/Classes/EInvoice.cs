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
            CountryEntity countryEntity = new CountryEntity();
            try
            {
                if (invoiceProperties != null)
                {

                    if (invoiceProperties.billDate!=null)
                    {
                        DateTime date = DateTime.Parse(invoiceProperties.billDate);     
                        jObject.Add($"{InvoiceFields.PurchaseDate}", date.ToString("yyyy-MM-dd"));
                    }


                    if (invoiceProperties.patientId>0)
                    {
                        jObject.Add($"{InvoiceSchemas.CustomerIdSchema}@odata.bind", $"/{contactEntity.EntityPluralName}({ContactFields.IdAboxPatient}={invoiceProperties.patientId})");
                    }

                    if (invoiceProperties.patientId > 0)
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

                    if (!(String.IsNullOrEmpty(invoiceProperties.country)))
                    {
                        jObject.Add($"{InvoiceSchemas.Country}@odata.bind", $"/{countryEntity.EntityPluralName}({CountryFields.IdCountry}='{invoiceProperties.country}')");
                    }

                    if (!String.IsNullOrEmpty(invoiceProperties.status))
                    {
                        int status = sharedMethods.GetInvoiceStatusValue(invoiceProperties.status);
                        if (status>-1)
                        {
                            jObject.Add(InvoiceFields.StatusCode, status);
                        }

                    }

                    if (!String.IsNullOrEmpty(invoiceProperties.statusReason))
                    {
                        jObject.Add(InvoiceFields.StatusReason, invoiceProperties.statusReason);
                    }

                    if (invoiceProperties.revisionTime1!=null && invoiceProperties.revisionTime1>-1)
                    {
                        jObject.Add(InvoiceFields.RevisionTime1, invoiceProperties.revisionTime1);
                    }

                    if (invoiceProperties.revisionTime2 != null && invoiceProperties.revisionTime2 > -1)
                    {
                        jObject.Add(InvoiceFields.RevisionTime2, invoiceProperties.revisionTime2);
                    }

                    if (invoiceProperties.totalAmount != null)
                    {
                        jObject.Add(InvoiceFields.TotalAmount, invoiceProperties.totalAmount);
                    }

                    if (!String.IsNullOrEmpty(invoiceProperties.purchaseMethod))
                    {
                        jObject.Add(InvoiceFields.PurchaseMethod, invoiceProperties.purchaseMethod);
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


        private JObject GetUpdateInvoiceJsonStructure(InvoiceUpdate invoiceProperties, Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
            CountryEntity countryEntity = new CountryEntity();
            try
            {
                if (invoiceProperties != null)
                {
                    

                    if (invoiceProperties.billDate!=null) {
                        DateTime date = DateTime.Parse(invoiceProperties.billDate);
                        jObject.Add($"{InvoiceFields.PurchaseDate}", date.ToString("yyyy-MM-dd"));
                    }

                    if (invoiceProperties.patientId > 0)
                    {
                        jObject.Add($"{InvoiceSchemas.CustomerIdSchema}@odata.bind", $"/{contactEntity.EntityPluralName}({ContactFields.IdAboxPatient}={invoiceProperties.patientId})");
                    }

                    if (invoiceProperties.patientId > 0)
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


                    if (invoiceProperties.products != null)
                    {
                        var serialized = JsonConvert.SerializeObject(invoiceProperties.products);
                        jObject.Add($"{InvoiceFields.ProductsSelectedJson}", serialized);
                    }

                  

                    if (!(String.IsNullOrEmpty(invoiceProperties.country)))
                    {
                        jObject.Add($"{InvoiceSchemas.Country}@odata.bind", $"/{countryEntity.EntityPluralName}({CountryFields.IdCountry}='{invoiceProperties.country}')");
                    }

                    if (String.IsNullOrEmpty(invoiceProperties.status))
                    {
                        int status = sharedMethods.GetInvoiceStatusValue(invoiceProperties.status);
                        if (status > -1)
                        {
                            jObject.Add(InvoiceFields.StatusCode, AboxDynamicsBase.Classes.Constants.PendingInvoiceDropdownValue);
                        }

                    }

                    if (String.IsNullOrEmpty(invoiceProperties.statusReason))
                    {
                        jObject.Add(InvoiceFields.StatusReason, invoiceProperties.statusReason);
                    }

                    if (invoiceProperties.revisionTime1 != null && invoiceProperties.revisionTime1 > -1)
                    {
                        jObject.Add(InvoiceFields.RevisionTime1, invoiceProperties.revisionTime1);
                    }

                    if (invoiceProperties.revisionTime2 != null && invoiceProperties.revisionTime2 > -1)
                    {
                        jObject.Add(InvoiceFields.RevisionTime2, invoiceProperties.revisionTime2);
                    }

                    if (invoiceProperties.totalAmount != null)
                    {
                        jObject.Add(InvoiceFields.TotalAmount, invoiceProperties.totalAmount);
                    }

                    if (String.IsNullOrEmpty(invoiceProperties.purchaseMethod))
                    {
                        jObject.Add(InvoiceFields.PurchaseMethod, invoiceProperties.purchaseMethod);
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

        private JObject GetInvoiceApproveJsonStructure(InvoiceApprove invoiceProperties, Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
         
            try
            {
                if (invoiceProperties != null)
                {

                    jObject.Add(InvoiceFields.StatusCode, AboxDynamicsBase.Classes.Constants.ApprovedStatusInvoiceDropdownValue);

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

        private JObject GetInvoiceRejectJsonStructure(InvoiceReject invoiceProperties, Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {
                if (invoiceProperties != null)
                {

                    jObject.Add(InvoiceFields.StatusCode, AboxDynamicsBase.Classes.Constants.RejectedStatusInvoiceDropdownValue);

                    if (!String.IsNullOrEmpty(invoiceProperties.Reason))
                        jObject.Add(InvoiceFields.StatusReason, invoiceProperties.Reason);

                    


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



        private OperationResult InvoiceUpdateRequest(JObject jsonObject,string idToUpdate, Guid processId)
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
                            
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            client.DefaultRequestHeaders.Add("If-Match", "*");

                            MethodBase m = MethodBase.GetCurrentMethod();
                            string url = $"{this.EntityPluralName}({InvoiceFields.IdAboxInvoice}={idToUpdate})";



                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Url:{url} Data:{jsonObject.ToString(Formatting.None)}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);


                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PatchAsync(url, c).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //response.Content.read
                                //string userId = (string)body[InvoiceFields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Factura actualizada correctamente en el CRM";
                                operationResult.IsSuccessful = true;
                                operationResult.Data = null;
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
                                operationResult.Message = "Error al actualizar la factura en el CRM";
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

        public OperationResult UpdateInvoice(InvoiceUpdate invoiceCreateRequest, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                InvoiceUpdate invoiceCreateProperties = invoiceCreateRequest;

                if (invoiceCreateProperties != null)
                {


                    JObject invoiceUpdateObject = this.GetUpdateInvoiceJsonStructure(invoiceCreateProperties, processId);


                    responseObject = this.InvoiceUpdateRequest(invoiceUpdateObject,invoiceCreateRequest.idFromDatabase, processId);
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

        public OperationResult ApproveInvoice(InvoiceApprove invoiceApproveRequest, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                

                if (invoiceApproveRequest != null)
                {


                    JObject invoiceUpdateObject = this.GetInvoiceApproveJsonStructure(invoiceApproveRequest, processId);


                    responseObject = this.InvoiceUpdateRequest(invoiceUpdateObject, invoiceApproveRequest.InvoiceId, processId);
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

        public OperationResult RejectInvoice(InvoiceReject invoiceRejectRequest, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                if (invoiceRejectRequest != null)
                {
                    JObject invoiceUpdateObject = this.GetInvoiceRejectJsonStructure(invoiceRejectRequest, processId);
                    responseObject = this.InvoiceUpdateRequest(invoiceUpdateObject, invoiceRejectRequest.InvoiceId, processId);
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

        public OperationResult GetInvoice(string idToSearch,string idPropertyName, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {


                if (!String.IsNullOrEmpty(idToSearch) && !String.IsNullOrEmpty(idPropertyName))
                {

                    responseObject = this.InvoiceGetRequest(idToSearch, idPropertyName, processId);
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

        private OperationResult InvoiceGetRequest(string idToSearch, string idPropertyName, Guid processId)
        {

            OperationResult operationResult = new OperationResult();
            try
            {

                if (!String.IsNullOrEmpty(idToSearch)&&!String.IsNullOrEmpty(idPropertyName))
                {
                    try
                    {
                        using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                        {
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");

                            MethodBase m = MethodBase.GetCurrentMethod();
                            string url = "";
                            if (idPropertyName == InvoiceFields.EntityId)
                            {

                                url = $"{this.EntityPluralName}({idToSearch})?$select={InvoiceFields.EntityId},{InvoiceFields.IdAboxInvoice}";
                            }
                            else if (idPropertyName == InvoiceFields.IdAboxInvoice)
                            {
                                url = $"{this.EntityPluralName}({idPropertyName}={idToSearch})?$select={InvoiceFields.EntityId},{InvoiceFields.IdAboxInvoice}";
                            }




                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Url:{url} Data:idToSearch:{idToSearch}|idPropertyName:{idPropertyName}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);


                            
                            var response = client.GetAsync(url).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string invoiceIdGuid = (string)body[InvoiceFields.EntityId];
                                string invoiceIdAbox = (string)body[InvoiceFields.IdAboxInvoice];
                                operationResult.Code = "";
                                operationResult.Message = "Factura obtenida correctamente del CRM";
                                operationResult.IsSuccessful = true;
                                IDictionary<string, string> responseProperties = new Dictionary<string, string>();
                                responseProperties.Add(new KeyValuePair<string, string>(InvoiceFields.EntityId, invoiceIdGuid));
                                responseProperties.Add(new KeyValuePair<string, string>(InvoiceFields.IdAboxInvoice,invoiceIdAbox));
                                operationResult.Data = responseProperties;
                                
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
                                operationResult.Message = "Error al obtener la factura en el CRM";
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
    }
}