using AboxCrmPlugins.Classes;
using AboxDynamicsBase.Classes;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace AboxCrmPlugins.Methods
{
    public class MShared
    {
        public WebRequestResponse DoPostRequest(WebRequestData requestData, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                #region debug log

                this.LogPluginFeedback(new LogClass
                {
                    Exception = "",
                    Level = "debug",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"POSTing Url:{requestData.Url} Json:{requestData.InputData}",
                    ProcessId = ""
                }, trace);

                #endregion debug log
            }
            catch (Exception exc)
            {
            }

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
                        {
                            wrResponse.IsSuccessful = true;

                            //TODO:ELiminar para Produccion
                            try
                            {
                                this.LogPluginFeedback(new LogClass
                                {
                                    Exception = "",
                                    Level = "debug",
                                    ClassName = this.GetType().ToString(),
                                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                                    Message = $"Url:{requestData.Url} ResponseFromRequest:{wrResponse.Data}",
                                    ProcessId = ""
                                }, trace);
                            }
                            catch (Exception e)
                            {
                            }
                        }
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
                            }
                        }
                    }

                    #region error log

                    this.LogPluginFeedback(new LogClass
                    {
                        Exception = wex.ToString(),
                        Level = "error",
                        ClassName = this.GetType().ToString(),
                        MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"{error}",
                        ProcessId = ""
                    }, trace);

                    #endregion error log

                    wrResponse.Data = null;
                    wrResponse.ErrorMessage = wex.ToString();
                    wrResponse.IsSuccessful = false;
                }
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Excepción realizando el POST request",
                    ProcessId = ""
                }, trace);

                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                wrResponse.Data = null;
                wrResponse.IsSuccessful = false;
                wrResponse.ErrorMessage = ex.ToString();
            }

            return wrResponse;
        }

        public bool IsValidName(string input, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                string pattern = Constants.RegexValidName;
                Match m = Regex.Match(@input, pattern);
                return m.Success;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"",
                    ProcessId = ""
                }, trace);

                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }


        }

        public bool IsValidLastname(string input, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                string pattern = Constants.RegexValidLastname;
                Match m = Regex.Match(@input, pattern);
                return m.Success;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"",
                    ProcessId = ""
                }, trace);

                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
        }

        public bool IsValidMaxLength(string input,int maxLength, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                bool isValid = false;

                if (input.Length<=maxLength)
                {
                    isValid = true;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando el max length",
                    ProcessId = ""
                }, trace);

                //trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
        }

        public bool IsValidMinLength(string input, int minLength, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                bool isValid = false;

                if (input.Length >= minLength)
                {
                    isValid = true;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando el min length",
                    ProcessId = ""
                }, trace);

                //trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
        }


        public bool HasOnlyNumbers(string input, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                string pattern = Constants.RegexOnlyNumbers;
                Match m = Regex.Match(@input, pattern);
                return m.Success;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando",
                    ProcessId = ""
                }, trace);

                //trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
        }




        public bool IsValidPassword(string input, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                string pattern = Constants.RegexPassword;
                Match m = Regex.Match(@input, pattern);
                return m.Success;
            }
            catch (Exception ex)
            {
                this.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando el formato de contraseña",
                    ProcessId = ""
                }, trace);

                //trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
        }


        public Int32 GetAge(DateTime dateOfBirth)
        {
            try
            {
                var today = DateTime.Today;
                var a = (today.Year * 100 + today.Month) * 100 + today.Day;
                var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;
                int s = (a - b) / 10000;
                return s;
            }
            catch (Exception)
            {

                throw;
            }
           
            
        }

        public void LogPluginFeedback(LogClass log, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            try
            {
                WebRequestData wrData = new WebRequestData();
                MemoryStream memoryStream = new MemoryStream();
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LogClass));
                serializer.WriteObject(memoryStream, log);
                var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                memoryStream.Dispose();
                wrData.InputData = jsonObject;
                wrData.ContentType = "application/json";
                // wrData.Authorization = "Bearer " + Constants.TokenForAboxServices;
                wrData.Url = AboxServices.CrmWebAPILog;

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        var webClient = new WebClient();
                        if (wrData.ContentType != "")
                            webClient.Headers[HttpRequestHeader.ContentType] = wrData.ContentType;
                        if (wrData.Authorization != "")
                            webClient.Headers[HttpRequestHeader.Authorization] = wrData.Authorization;

                        // var code = "key";
                        string serviceUrl = wrData.Url;
                        webClient.UploadString(serviceUrl, wrData.InputData);

                        trace.Trace("Url:" + wrData.Url + " | Data:" + wrData.InputData);
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw;
            }
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
                        result = "other";
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