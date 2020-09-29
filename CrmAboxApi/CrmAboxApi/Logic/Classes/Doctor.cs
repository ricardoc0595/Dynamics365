using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Configuration;
using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using System.Text;
using Newtonsoft.Json;
using CrmAboxApi.Logic.Classes.Deserializing;

using CrmAboxApi.Logic.Methods;
using AboxDynamicsBase.Classes.Entities;


namespace CrmAboxApi.Logic.Classes
{
    public class Doctor
    {
        MShared sharedMethods = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private DoctorEntity doctorEntity;
        public Doctor()
        {
            sharedMethods = new MShared();
            doctorEntity = new DoctorEntity();
        }

        public OperationResult Delete(string idDoctor)
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
                        var response = client.DeleteAsync($"{doctorEntity.EntityPluralName}({idDoctor})").Result;
                        //if (response.IsSuccessStatusCode)
                        //{

                        //    responseObject.Code = "";
                        //    responseObject.Message = "Contacto eliminado correctamente del CRM";
                        //    responseObject.IsSuccessful = true;
                        //    responseObject.Data = null;

                        //}
                        //else
                        //{
                        //    //Get the response content and parse it.
                        //    JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        //    //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                        //    JObject userId = JObject.Parse(body.ToString());
                        //    CrmWebAPIError err = userId.ToObject<CrmWebAPIError>();

                        //    Logger.Error("", response.RequestMessage);
                        //    responseObject.Code = "Error al eliminar el contacto del CRM";
                        //    responseObject.Message = response.ReasonPhrase;
                        //    responseObject.IsSuccessful = false;
                        //    responseObject.Data = null;
                        //    responseObject.InternalError = err;
                        //}

                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    responseObject.Code = "";
                    responseObject.Message = ex.ToString();
                    responseObject.IsSuccessful = false;
                    responseObject.Data = null;

                }





            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }


        public OperationResult RetrieveAll()
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
                        var response = client.GetAsync($"{doctorEntity.EntityPluralName}").Result;
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

                            Logger.Error("", response.RequestMessage);
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
                    Logger.Error(ex);
                    responseObject.Code = "";
                    responseObject.Message = ex.ToString();
                    responseObject.IsSuccessful = false;
                    responseObject.Data = null;

                }





            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }

    }
}