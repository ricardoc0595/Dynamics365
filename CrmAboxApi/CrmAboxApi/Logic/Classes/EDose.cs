﻿using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes.Helper;

//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                            Logger.Debug("ProcessID: {processId} Url:{url} Action: {actionName} Data:{requestData}", processId, url, m.Name, jsonObject.ToString(Formatting.None));
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
                                    Logger.Error("ProcessID: {processId} Method:{methodName} Url:{url} ErrorCode:{errCode} ErrorMessage:{errorMessage} ResponseReasonPhrase:{reasonPhrase}", processId, m.Name, url, err.error.code, err.error.message, response.ReasonPhrase);
                                operationResult.Code = "Error al crear la relación Producto-Dosis en el CRM";
                                operationResult.Message = response.ReasonPhrase;
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                            Logger.Debug("ProcessID: {processId} Url:{url} Action: {actionName} ", processId, url, m.Name);
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
                                    Logger.Error("ProcessID: {processId} Method:{methodName} Url:{url} ErrorCode:{errCode} ErrorMessage:{errorMessage} ResponseReasonPhrase:{reasonPhrase}", processId, m.Name, url, err.error.code, err.error.message, response.ReasonPhrase);
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
                        Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
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
                Logger.Error(ex, "ProcessID: {processId} Method:{methodName}", processId, new StackTrace(ex).GetFrame(0).GetMethod().Name);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }
    }
}