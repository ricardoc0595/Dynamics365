using AboxCrmPlugins.Classes;
using AboxDynamicsBase.Classes;
using System;
using System.IO;
using System.Net;

namespace AboxCrmPlugins.Methods
{
    public class MShared
    {
        public WebRequestResponse DoPostRequest(WebRequestData requestData, Microsoft.Xrm.Sdk.ITracingService trace)
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

                        trace.Trace("Url:" + requestData.Url + " | Data:" + requestData.InputData);
                        trace.Trace("Respuesta request Servicios Abox:" + wrResponse.Data);
                        if (wrResponse.Data != "")
                            wrResponse.IsSuccessful = true;
                    }
                }
                catch (WebException wex)
                {
                    string error = "";
                    string statusCode = "";
                    if (wex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)wex.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                error = reader.ReadToEnd();

                                //TODO: use JSON.net to parse this string and look at the error message
                            }
                        }
                    }
                    trace.Trace("Url:" + requestData.Url + " | Data:" + requestData.InputData);
                    trace.Trace(error);
                    wrResponse.Data = null;
                    wrResponse.ErrorMessage = wex.ToString();
                    wrResponse.IsSuccessful = false;
                    //TODO: Capturar excepción con servicios de Abox Plan y hacer un Logging
                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                wrResponse.Data = null;
                wrResponse.IsSuccessful = false;
                wrResponse.ErrorMessage = ex.ToString();
            }

            return wrResponse;
        }

        public string GetCountryValueForService(int dynamicsCountryValue)
        {
            string countryValue = "";
            switch (dynamicsCountryValue)
            {
                case 1:
                    countryValue = "CR";
                    break;

                case 2:
                    countryValue = "HN";
                    break;

                case 3:
                    countryValue = "GT";
                    break;

                case 4:
                    countryValue = "NI";
                    break;

                case 5:
                    countryValue = "PA";
                    break;

                case 6:
                    countryValue = "DO";
                    break;

                default:
                    countryValue = "";
                    break;
            }

            return countryValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idType">Lookup Id of the user type</param>
        /// <returns></returns>
        public string GetUserTypeId(string idType)
        {
            string result = "";
            try
            {
                switch (idType)
                {
                    case Constants.PatientIdType:
                        result = "01";
                        break;

                    case Constants.CareTakerIdType:
                        result = "02";
                        break;

                    case Constants.TutorIdType:
                        result = "03";
                        break;

                    case "04":
                        result = ""; //No se usa en produccion de Abox Plan
                        break;

                    case Constants.OtherInterestIdType:
                        result = "05";
                        break;

                    default:
                        result = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = null;
                throw;
            }
            return result;
        }

        public string GetGenderValue(int genderValueFromJsonDynamics)
        {
            string result = "";
            try
            {
                switch (genderValueFromJsonDynamics)
                {
                    case Constants.MaleGenderValue:
                        result = "Masculino";
                        break;

                    case Constants.FemaleGenderValue:
                        result = "Femenino";
                        break;

                    default:
                        result = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public string GetDoseFrequencyValue(int value)
        {
            string result = "";
            try
            {
                switch (value)
                {
                    case Constants.DoseFrequencyOnePerDay:
                        result = "1 al día";
                        break;

                    case Constants.DoseFrequencyTwoPerDay:
                        result = "2 al día";
                        break;

                    case Constants.DoseFrequencyThreePerDay:
                        result = "3 al día";
                        break;

                    case Constants.DoseFrequencyFourPerDay:
                        result = "4 al día";
                        break;

                    case Constants.DoseFrequencyOther:
                        result = "Otro";
                        break;

                    default:
                        result = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }
    }
}