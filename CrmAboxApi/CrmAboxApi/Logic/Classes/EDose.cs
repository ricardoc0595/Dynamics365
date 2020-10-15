using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using System.Text;
using Newtonsoft.Json;
using CrmAboxApi.Logic.Classes.Deserializing;
//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using AboxDynamicsBase.Classes.Entities;

namespace CrmAboxApi.Logic.Classes
{
    public class EDose : AboxDynamicsBase.Classes.Entities.DoseEntity
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private MShared sharedMethods = null;
        string connectionString = null;


        public EDose()
        {
            sharedMethods = new MShared();
            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        }

        private JObject GetCreateDoseJsonStructure(DoseRecord doseRecord)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
            ProductEntity productEntity = new ProductEntity();
            try
            {

                if (!String.IsNullOrEmpty(doseRecord.IdProduct))
                {
                    
                    
                    jObject.Add($"{this.Schemas.DosexProduct}@odata.bind", new JValue($"/{productEntity.EntityPluralName}({productEntity.Fields.ProductNumber}='{doseRecord.IdProduct}')"));
                }

                if (!String.IsNullOrEmpty(doseRecord.ContactBinding))
                {
                    jObject.Add($"{this.Schemas.ContactxDose}@odata.bind", new JValue($"/{doseRecord.ContactBinding}"));
                }



                if (!(String.IsNullOrEmpty(doseRecord.Dose)))
                {
                    jObject.Add($"{this.Fields.Dose}", sharedMethods.GetDoseFrequencyValue(doseRecord.Dose));
                }




                return jObject;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                jObject = null;
                return jObject;
            }


        }
        private OperationResult DoseCreateRequest(JObject jsonObject)
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

                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PostAsync($"{this.EntityPluralName}?$select={this.Fields.EntityId}", c).Result;
                            if (response.IsSuccessStatusCode)
                            {

                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[this.Fields.EntityId];
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

                                Logger.Error("", response.RequestMessage);
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
                        Logger.Error(ex.ToString());
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
                Logger.Error(ex.ToString());
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }

        }

        private OperationResult DoseDeleteRequest(string doseId)
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
                            //client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                            //HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.DeleteAsync($"{this.EntityPluralName}({doseId})").Result;
                            if (response.IsSuccessStatusCode)
                            {

                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //string userId = (string)body[this.Fields.EntityId];
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

                                Logger.Error("", response.RequestMessage);
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
                        Logger.Error(ex.ToString());
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
                Logger.Error(ex.ToString());
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }

        }

        public OperationResult Create(DoseRecord doseRecord)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                
                var newDose = this.GetCreateDoseJsonStructure(doseRecord);

               

                responseObject = this.DoseCreateRequest(newDose);
                return responseObject;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }

        public OperationResult Delete(string doseId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {

                responseObject = this.DoseDeleteRequest(doseId);
                return responseObject;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }

    }
}