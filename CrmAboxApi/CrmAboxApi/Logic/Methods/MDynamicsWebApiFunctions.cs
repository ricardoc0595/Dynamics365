
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using CrmAboxApi.Logic.Classes.Helper;


namespace CrmAboxApi.Logic.Methods
{
    public class MDynamicsWebApiFunctions
    {

        public string whoAmIFunction()
        {
            
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();

            try
            {
                using (HttpClient client =  ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                {
                    // Use the WhoAmI function
                    var response =  client.GetAsync("WhoAmI").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //Get the response content and parse it.  
                        JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Guid userId = (Guid)body["UserId"];
                        return userId.ToString();

                    }
                    else
                    {
                        return "error";
                    }

                }
            }
            catch (Exception ex)
            {
                return "error excepcion";
                throw ex;
            }
            return "";
        }

    }
}