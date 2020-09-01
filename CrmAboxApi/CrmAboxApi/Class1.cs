using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Configuration;
using CrmAboxApi.Classes.Helper;

namespace CrmAboxApi
{
    public class Class1
    {

        public string k()
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

        public async Task<string> DiscoveryAuthority2()
        {
            string url = "https://aboxsb.api.crm.dynamics.com";
            // e.g. you@yourorg.onmicrosoft.com
            string userName = ConfigurationManager.AppSettings["DynamicsCrmUser"];
            // e.g. y0urp455w0rd
            string password = ConfigurationManager.AppSettings["DynamicsCrmPassword"]; ;

            // Azure Active Directory registered app clientid for Microsoft samples
            string clientId = "6843632f-a6f3-4d1c-84ba-329d7026e286";

            var userCredential = new UserCredential(userName, password);
            string apiVersion = "9.1";
            string webApiUrl = $"{url}/api/data/v{apiVersion}/";



            AuthenticationParameters authParameters = await
                AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(webApiUrl));


            AuthenticationContext authContext = new AuthenticationContext(authParameters.Authority, false);

            AuthenticationResult r = authContext.AcquireToken(url, clientId, userCredential);






            var authHeader = new AuthenticationHeaderValue("Bearer", r.AccessToken);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(webApiUrl);
                client.DefaultRequestHeaders.Authorization = authHeader;

                // Use the WhoAmI function
                var response = client.GetAsync("WhoAmI").Result;

                if (response.IsSuccessStatusCode)
                {
                    //Get the response content and parse it.  
                    JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    Guid userId = (Guid)body["UserId"];
                    // Console.WriteLine("Your UserId is {0}", userId);
                }
                else
                {
                    //Console.WriteLine("The request failed with a status of '{0}'",
                    //            response.ReasonPhrase);
                }

                //Console.WriteLine("Press any key to exit.");
                //Console.ReadLine();
            }



            return authParameters.Authority;
        }


        public async void connect()
        {
            
            string k= await this.DiscoveryAuthority2();
            k = k;
            
        }

    }
}