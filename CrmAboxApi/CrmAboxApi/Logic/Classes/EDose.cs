using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes.Helper;

//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace CrmAboxApi.Logic.Classes
{
    public class EDose : AboxDynamicsBase.Classes.Entities.DoseEntity
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private MShared sharedMethods = null;
        private string connectionString = null;

        public EDose()
        {
            sharedMethods = new MShared();
            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        }

        private JObject GetCreateDoseJsonStructure(DoseRecord doseRecord,Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
            ProductEntity productEntity = new ProductEntity();
            try
            {
                if (!String.IsNullOrEmpty(doseRecord.IdProduct))
                {
                    jObject.Add($"{DoseSchemas.DosexProduct}@odata.bind", new JValue($"/{productEntity.EntityPluralName}({ProductFields.ProductNumber}='{doseRecord.IdProduct}')"));
                }

                if (!String.IsNullOrEmpty(doseRecord.ContactBinding))
                {
                    jObject.Add($"{DoseSchemas.ContactxDose}@odata.bind", new JValue($"/{doseRecord.ContactBinding}"));
                }

                if (!(String.IsNullOrEmpty(doseRecord.Dose)))
                {
                    jObject.Add($"{DoseFields.Dose}", sharedMethods.GetDoseFrequencyValue(doseRecord.Dose));
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

        private OperationResult DoseCreateRequest(JObject jsonObject,Guid processId)
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
                            MethodBase m = MethodBase.GetCurrentMethod();
                            string url = $"{this.EntityPluralName}?$select={DoseFields.EntityId}";
                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            

                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $" Url:{url} Data:{jsonObject.ToString(Formatting.None)}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);

                            var response = client.PostAsync(url, c).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[DoseFields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Dosis creada correctamente en el CRM";
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
                                operationResult.Message = "Error al crear la relación Producto-Dosis en el CRM";
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
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

        private OperationResult DoseDeleteRequest(string doseId,Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                if (doseId != null)
                {
                    try
                    {
                        using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                        {
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            MethodBase m = MethodBase.GetCurrentMethod();
                            //client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                            string url = $"{this.EntityPluralName}({doseId})";

                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Url:{url}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);

                            //HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.DeleteAsync(url).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //string userId = (string)body[DoseFields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Dosis eliminada correctamente del CRM";
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
                                   
                                operationResult.Code = "Error al eliminar la dosis del CRM";
                                operationResult.Message = response.ReasonPhrase;
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
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

        public OperationResult Create(DoseRecord doseRecord,Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                var newDose = this.GetCreateDoseJsonStructure(doseRecord,processId);

                responseObject = this.DoseCreateRequest(newDose,processId);
                return responseObject;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }

        public OperationResult Delete(string doseId,Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                responseObject = this.DoseDeleteRequest(doseId,processId);
                return responseObject;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }
    }
}