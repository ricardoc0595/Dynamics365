using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

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

        public OperationResult Disassociate(string connectionString)
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

                        Logger.Debug($"Disassociation of Entities | ");
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

                        var response = client.DeleteAsync($"{targetQuery}/{relatedQuery}/$ref").Result;
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

                            Logger.Error("", response.RequestMessage);
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
                    Logger.Error(ex.ToString());
                    operationResult.Code = "";
                    operationResult.Message = ex.ToString();
                    operationResult.IsSuccessful = false;
                    operationResult.Data = null;
                }

                return operationResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        public OperationResult Associate(string connectionString)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        string url = ConnectionHelper.GetParameterValueFromConnectionString(connectionString, "Url");
                        url += "/api/data/v9.1/";
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                        Logger.Debug($"Association of Entities | ");

                        JObject jsonObject = new JObject();
                        if (String.IsNullOrEmpty(this.TargetIdKeyToUse))
                        {
                            //si no viene un Id key se esta usando el ID de la entidad como tal y no un Key alternativo (los de Abox por ejemplo)
                            jsonObject.Add($"@odata.id", $"{url}{this.RelatedEntityName}({this.RelatedEntityId})");
                        }
                        else
                        {
                            //Se usa el Key alternativo (los de Abox por ejemplo)
                            jsonObject.Add($"@odata.id", $"{url}{this.RelatedEntityName}({this.RelatedEntityIdKeyToUse}='{this.RelatedEntityId}')");
                        }

                        string targetQuery = "";

                        if (String.IsNullOrEmpty(this.TargetIdKeyToUse))
                            targetQuery = $"{this.TargetEntityName}({this.TargetEntityId})";
                        else
                            targetQuery = $"{this.TargetEntityName}({this.TargetIdKeyToUse}={this.TargetEntityId})";

                        HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync($"{targetQuery}/{this.RelationshipDefinitionName}/$ref", c).Result;

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

                            Logger.Error("", response.RequestMessage);
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
                    Logger.Error(ex.ToString());
                    operationResult.Code = "";
                    operationResult.Message = ex.ToString();
                    operationResult.IsSuccessful = false;
                    operationResult.Data = null;
                }

                return operationResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }
    }
}