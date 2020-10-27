using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;

namespace CrmAboxApi.Logic.Classes
{
    public class EntityAssociation
    {
        public string TargetEntityName { get; set; }
        public string RelatedEntityName { get; set; }
        public string RelationshipDefinitionName { get; set; }
        public string TargetEntityId { get; set; }
        public string RelatedEntityId { get; set; }
        public string TargetIdKeyToUse { get; set; }
        public string RelatedEntityIdKeyToUse { get; set; }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public OperationResult Disassociate(string connectionString,Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                       
                        //HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                        string targetQuery = "";
                        string relatedQuery = "";

                        if (String.IsNullOrEmpty(this.TargetIdKeyToUse))
                            targetQuery = $"{this.TargetEntityName}({this.TargetEntityId})";
                        else
                            targetQuery = $"{this.TargetEntityName}({this.TargetIdKeyToUse}={this.TargetEntityId})";

                        if (String.IsNullOrEmpty(this.RelatedEntityIdKeyToUse))
                            relatedQuery = $"{this.RelationshipDefinitionName}({this.RelatedEntityId})";
                        else
                            relatedQuery = $"{this.RelationshipDefinitionName}({this.RelatedEntityIdKeyToUse}='{this.RelatedEntityId}')";


                        string url = $"{targetQuery}/{relatedQuery}/$ref";

                        MethodBase m = MethodBase.GetCurrentMethod();
                        Logger.Debug("ProcessID: {processId} Action: {actionName} ", processId, m.Name);

                        var response = client.DeleteAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //Get the response content and parse it.
                            //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //string userId = (string)body[this.Fields.EntityId];
                            operationResult.Code = "";
                            operationResult.Message = "Desasociación realizada correctamente";
                            operationResult.IsSuccessful = true;
                            operationResult.Data = null;
                        }
                        else
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                            JObject result = JObject.Parse(body.ToString());
                            CrmWebAPIError err = result.ToObject<CrmWebAPIError>();

                            if (err != null)
                                Logger.Error("ProcessID: {processId} Method:{methodName} Url:{url} ErrorCode:{errCode} ErrorMessage:{errorMessage} ResponseReasonPhrase:{reasonPhrase}", processId, m.Name, url, err.error.code, err.error.message, response.ReasonPhrase);
                            operationResult.Code = "Ocurrió un error desasociando las entidades";
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

        public OperationResult Associate(string connectionString,Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        string uri = ConnectionHelper.GetParameterValueFromConnectionString(connectionString, "Url");
                        uri += "/api/data/v9.1/";
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                      

                        JObject jsonObject = new JObject();
                        if (String.IsNullOrEmpty(this.TargetIdKeyToUse))
                        {
                            //si no viene un Id key se esta usando el ID de la entidad como tal y no un Key alternativo (los de Abox por ejemplo)
                            jsonObject.Add($"@odata.id", $"{uri}{this.RelatedEntityName}({this.RelatedEntityId})");
                        }
                        else
                        {
                            //Se usa el Key alternativo (los de Abox por ejemplo)
                            jsonObject.Add($"@odata.id", $"{uri}{this.RelatedEntityName}({this.RelatedEntityIdKeyToUse}='{this.RelatedEntityId}')");
                        }

                        string targetQuery = "";

                        if (String.IsNullOrEmpty(this.TargetIdKeyToUse))
                            targetQuery = $"{this.TargetEntityName}({this.TargetEntityId})";
                        else
                            targetQuery = $"{this.TargetEntityName}({this.TargetIdKeyToUse}={this.TargetEntityId})";

                        HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), System.Text.Encoding.UTF8, "application/json");

                        MethodBase m = MethodBase.GetCurrentMethod();
                        string url = $"{targetQuery}/{this.RelationshipDefinitionName}/$ref";
                        Logger.Debug("ProcessID: {processId} Action: {actionName} Data:{requestData}", processId, m.Name, jsonObject.ToString(Formatting.None));

                        var response = client.PostAsync(url, c).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            //Get the response content and parse it.
                            //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //string userId = (string)body[this.Fields.EntityId];
                            operationResult.Code = "";
                            operationResult.Message = "Asociación realizada correctamente";
                            operationResult.IsSuccessful = true;
                            operationResult.Data = null;
                        }
                        else
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                            JObject result = JObject.Parse(body.ToString());
                            CrmWebAPIError err = result.ToObject<CrmWebAPIError>();

                            if (err != null)
                                Logger.Error("ProcessID: {processId} Method:{methodName} Url:{url} ErrorCode:{errCode} ErrorMessage:{errorMessage} ResponseReasonPhrase:{reasonPhrase}", processId, m.Name, url, err.error.code, err.error.message, response.ReasonPhrase);
                            operationResult.Code = "Ocurrió un error asociando las entidades";
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
    }
}