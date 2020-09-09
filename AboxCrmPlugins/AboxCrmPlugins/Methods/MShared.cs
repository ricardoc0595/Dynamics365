
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using AboxCrmPlugins.Classes;
using System.Runtime.Remoting.Messaging;

namespace AboxCrmPlugins.Methods
{
    public class MShared
    {


        public WebRequestResponse DoPostRequest (WebRequestData requestData)
        {

            WebRequestResponse wrResponse = new WebRequestResponse();
            try
            {
                try
                {

                    using (WebClient client = new WebClient())
                    {
                        var webClient = new WebClient();
                        if (requestData.ContentType != "")
                            webClient.Headers[HttpRequestHeader.ContentType] = requestData.ContentType;


                        if (requestData.Authorization != "")
                            webClient.Headers[HttpRequestHeader.Authorization] = requestData.Authorization;


                        // var code = "key";
                        string serviceUrl = requestData.Url;
                        wrResponse.Data = webClient.UploadString(serviceUrl, requestData.InputData);

                        if (wrResponse.Data != "")
                            wrResponse.IsSuccessful = true;

                    }

                }
                catch (WebException wex)
                {
                    wrResponse.Data = null;
                    wrResponse.ErrorMessage = wex.ToString();
                    wrResponse.IsSuccessful = false;
                    //TODO: Capturar excepción con servicios de Abox Plan y hacer un Logging
                }


            }
            catch (Exception ex)
            {
                wrResponse.Data= null;
                wrResponse.IsSuccessful = false;
                wrResponse.ErrorMessage = ex.ToString();
            }

            return wrResponse;
        }

      
    }
}
