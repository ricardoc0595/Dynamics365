using AboxDynamicsBase.Classes;
using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace CrmAboxApi.Controllers
{
    public class ContactsController : ApiController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // GET api/contacts
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/contacts/5
        public IHttpActionResult Get(int id)
        {
            EContact doctorProcess = new EContact();

            //var responseAllDoctors = doctorProcess.GetDosesRelated(id,processId);
            return null;
            //if (responseAllDoctors.IsSuccessful)
            //{
            //    try
            //    {
            //        RetrieveDoctorFromWebAPI doctorsResponse = (RetrieveDoctorFromWebAPI)responseAllDoctors.Data;

            //        //doctorProcess.Delete(doctorsResponse.value[0].new_doctorid);

            //        foreach (var doc in doctorsResponse.value)
            //        {
            //            doctorProcess.Delete(doc.new_doctorid);
            //        }

            //        return Content(HttpStatusCode.OK, new OperationResult
            //        {
            //            Data = null,
            //            Message = "Doctores eliminados",
            //            IsSuccessful = true,

            //        });

            //    }
            //    catch (Exception ex)
            //    {
            //        return Content(HttpStatusCode.InternalServerError, new OperationResult
            //        {
            //            Data = null,
            //            Message = ex.ToString(),
            //            IsSuccessful = false,

            //        }) ;

            //    }

            //}
            //else
            //{
            //    return Content(HttpStatusCode.InternalServerError, new OperationResult
            //    {
            //        Data = null,
            //        Message = "Error",
            //        IsSuccessful = false,

            //    });
            //}
        }

        //POST api/contacts

        [HttpGet]
        public IHttpActionResult GetToken()
        {
            string token = "";
            CrmAboxApi.Logic.Methods.MDynamicsWebApiFunctions webAPiFunctions = new Logic.Methods.MDynamicsWebApiFunctions();

            token = webAPiFunctions.GetTestToken();
            return Ok(token);
        }



        [HttpGet]
        public IHttpActionResult WhoAmI()
        {
            //LogEventInfo s = new LogEventInfo(LogLevel.Trace, Logger.Name,new Log);
           
            //s.Properties["AppID"] = "PLUGIN";
            //Logger.Log(s);
           //Logger.Trace(new TestLayout { Caps = false, Config1 = "ss", Config2 = "22" });
            string token = "";
            CrmAboxApi.Logic.Methods.MDynamicsWebApiFunctions webAPiFunctions = new Logic.Methods.MDynamicsWebApiFunctions();

            token = webAPiFunctions.whoAmIFunction();
            return Ok(token);
        }

        [HttpPost]
        public IHttpActionResult LogPluginFeedback([FromBody] LogClass log)
        {
            LogLevel level = null;
            switch (log.Level.ToLower())
            {
                case "info":
                    level = LogLevel.Info;
                    break;
                case "error":
                    level = LogLevel.Error;
                    break;
                case "trace":
                    level = LogLevel.Trace;
                    break;
                case "debug":
                    level = LogLevel.Debug;
                    break;
                default:
                    break;
            }
            LogEventInfo s = new LogEventInfo(level, log.ClassName,$"** PLUGIN ERROR ** Method:{log.MethodName} ProcessID:{log.ProcessId} Message:{log.Message} Exception:{log.Exception}");
        
            Logger.Log(s);
           
          
            return Ok();
        }


        [HttpPost]
        public IHttpActionResult PatientSignup([FromBody] PatientSignup signupRequest)
        {
            Guid processId = Guid.NewGuid();
            Logger.Debug("ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(signupRequest));
            EContact contactProcedures = new EContact();
            OperationResult response = null;
            try
            {
                if (signupRequest != null)
                {
                    if (!String.IsNullOrEmpty(signupRequest.userType))
                    {
                        switch (signupRequest.userType.ToLower())
                        {
                            case "01":
                                response = contactProcedures.CreateAsPatient(signupRequest, null,processId);
                                break;

                            case "02":
                                response = contactProcedures.CreateAsCaretaker(signupRequest,processId);
                                break;

                            case "03":
                                response = contactProcedures.CreateAsTutor(signupRequest,processId);
                                break;
                            case "05":
                                response = contactProcedures.CreateAsConsumer(signupRequest, processId);
                                break;
                        }
                    }

                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, response);
                    }
                }
                else
                {
                    
                    return Content(HttpStatusCode.BadRequest, new OperationResult
                    {
                        Code = "",
                        IsSuccessful = false,
                        Data = null,
                        Message = "La solicitud JSON enviada es incorrecta"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(signupRequest));
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }

        [HttpPost]
        public IHttpActionResult AccountUpdate([FromBody] UpdateAccountRequest updateRequest)
        {
            Guid processId = Guid.NewGuid();
            Logger.Debug("ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(updateRequest));
            EContact contactProcedures = new EContact();
            OperationResult response = null;
            try
            {
                if (updateRequest != null)
                {
                    OperationResult updateResult = contactProcedures.UpdateAccount(updateRequest,processId);
                    
                    if (updateResult.IsSuccessful)
                    {
                        return Ok(updateResult);
                    }
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, updateResult);
                    }
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new OperationResult
                    {
                        Code = "",
                        IsSuccessful = false,
                        Data = null,
                        Message = "La solicitud JSON enviada es incorrecta"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(updateRequest));
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }

        [HttpPost]
        public IHttpActionResult UpdatePatient([FromBody] UpdatePatientRequest updateRequest)
        {
            Guid processId = Guid.NewGuid();
            Logger.Debug("ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(updateRequest));
            EContact contactProcedures = new EContact();
            OperationResult response = null;
            try
            {
                if (updateRequest != null)
                {
                    OperationResult updateResult = contactProcedures.UpdatePatient(updateRequest,processId);

                    if (updateResult.IsSuccessful)
                    {
                        return Ok(updateResult);
                    }
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, updateResult);
                    }
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new OperationResult
                    {
                        Code = "",
                        IsSuccessful = false,
                        Data = null,
                        Message = "La solicitud JSON enviada es incorrecta"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ProcessID: {processId} Request hacia {requestUrl} con el JSON:**START** {jsonObject} **END**", processId, Request.RequestUri, JsonConvert.SerializeObject(updateRequest));
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }

        // DELETE api/contacts/5
        public void Delete(int id)
        {
        }
    }
}