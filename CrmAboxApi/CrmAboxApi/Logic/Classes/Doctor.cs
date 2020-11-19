using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes.Deserializing;
using CrmAboxApi.Logic.Classes.Helper;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;

namespace CrmAboxApi.Logic.Classes
{
    public class Doctor
    {
        private MShared sharedMethods = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private DoctorEntity doctorEntity;

        public Doctor()
        {
            sharedMethods = new MShared();
            doctorEntity = new DoctorEntity();
        }

        public OperationResult Delete(string idDoctor,Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("If-None-Match", "null");
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        //HttpContent c = new StringContent(contact1.ToString(Formatting.None), Encoding.UTF8, "application/json");
                        MethodBase m = MethodBase.GetCurrentMethod();
                        string url = $"{doctorEntity.EntityPluralName}({idDoctor})";
                        Logger.Debug("ProcessID: {processId} Url:{url} Action: {actionName} ", processId, url, m.Name);
                        var response = client.DeleteAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseObject.Code = "";
                            responseObject.Message = "Contacto eliminado correctamente del CRM";
                            responseObject.IsSuccessful = true;
                            responseObject.Data = null;

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
                                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                log.Properties["ProcessID"] = processId;
                                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                Logger.Log(log);
                            }
                              
                            responseObject.Code = "Error al eliminar el contacto del CRM";
                            responseObject.Message = response.ReasonPhrase;
                            responseObject.IsSuccessful = false;
                            responseObject.Data = null;
                            responseObject.InternalError = err;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, ex);
                    log.Properties["ProcessID"] = processId;
                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    Logger.Log(log);
                    responseObject.Code = "";
                    responseObject.Message = ex.ToString();
                    responseObject.IsSuccessful = false;
                    responseObject.Data = null;
                }
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, ex);
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

        public OperationResult RetrieveAll(Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();

                DoctorEntity doctorEntity = new DoctorEntity();

                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("If-None-Match", "null");
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        //HttpContent c = new StringContent(contact1.ToString(Formatting.None), Encoding.UTF8, "application/json");
                        string url = $"{doctorEntity.EntityPluralName}";
                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logger.Debug("ProcessID: {processId} Url:{url} Action: {actionName} ", processId, url, m.Name);
                        var response = client.GetAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseObject.Code = "";
                            responseObject.Message = "Doctores obtenidos correctamente";
                            responseObject.IsSuccessful = true;

                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                            JObject userId = JObject.Parse(body.ToString());
                            RetrieveDoctorFromWebAPI data = userId.ToObject<RetrieveDoctorFromWebAPI>();
                            responseObject.Data = data;
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
                                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                log.Properties["ProcessID"] = processId;
                                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                Logger.Log(log);
                            }
                               
                            responseObject.Code = "Error al eliminar el contacto del CRM";
                            responseObject.Message = response.ReasonPhrase;
                            responseObject.IsSuccessful = false;
                            responseObject.Data = null;
                            responseObject.InternalError = err;
                        }
                    }
                }
                catch (Exception ex)
                {

                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, ex);
                    log.Properties["ProcessID"] = processId;
                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    Logger.Log(log);
                    responseObject.Code = "";
                    responseObject.Message = ex.ToString();
                    responseObject.IsSuccessful = false;
                    responseObject.Data = null;
                }
            }
            catch (Exception ex)
            {

                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, ex);
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