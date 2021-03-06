﻿using AboxDynamicsBase.Classes;
using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CrmAboxApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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

        //TODO: Ocultar para produccion
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetToken()
        {
            string token = "";
            CrmAboxApi.Logic.Methods.MDynamicsWebApiFunctions webAPiFunctions = new Logic.Methods.MDynamicsWebApiFunctions();

            token = webAPiFunctions.GetTestToken();
            return Ok(token);
        }


        //TODO: Ocultar para produccion
        [HttpGet]
        public IHttpActionResult WhoAmI()
        {
            LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Message");
            log.Properties["ProcessID"] = "sssadad";
            log.Properties["AppID"] = "PLUGIN";
            log.Properties["MethodName"] = "TestMethod";
            Logger.Log(log);
            string token = "";
            CrmAboxApi.Logic.Methods.MDynamicsWebApiFunctions webAPiFunctions = new Logic.Methods.MDynamicsWebApiFunctions();

            token = webAPiFunctions.whoAmIFunction();
            return Ok(token);
        }

        [HttpPost]
        [Authorize]
        public IHttpActionResult LogPluginFeedback([FromBody] LogClass log)
        {
            try
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
                LogEventInfo s = new LogEventInfo(level, log.ClassName, $"Message:{log.Message} Exception:{log.Exception}");
                
                s.Properties["ProcessID"] = log.ProcessId;
                s.Properties["AppID"] = "PLUGIN";
                s.Properties["MethodName"] = log.MethodName;
                Logger.Log(s);

              

                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });

            }
           
        }


        /// <summary>
        ///  Endpoint que Registra un usuario como Contacto dentro de Dynamics, segun el tipo de usuario cuidador-tutor-paciente-otrointeres
        /// </summary>
        /// <param name="signupRequest">JSON exitoso enviado previamente a los servicios de Abox plan, además los ID de pacientes necesarios</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult PatientSignup([FromBody] PatientSignup signupRequest)
        {
            Guid processId = Guid.NewGuid();
            
            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(signupRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

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
                                response = contactProcedures.CreateAsPatient(signupRequest, null,processId,false);
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
               
                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name,null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(signupRequest)} **END**",null,new Exception(ex.ToString()));
                logEx.Properties["ProcessID"] = processId;
                logEx.Properties["AppID"] = Constants.ApplicationIdWebAPI;
                logEx.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(logEx);

                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }

        /// <summary>
        /// Actualiza los datos de un paciente que posee una cuenta primaria, es decir, es un Usuario de tipo paciente o Cuidador/Tutor sin ningun paciente a cargo
        /// </summary>
        /// <param name="updateRequest">JSON exitoso enviado previamente a los servicios de Abox plan, además los ID de pacientes necesarios</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult AccountUpdate([FromBody] UpdateAccountRequest updateRequest)
        {
            Guid processId = Guid.NewGuid();
        
            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(updateRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);


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
                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name,null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(updateRequest)} **END**",null,new Exception(ex.ToString()));
                logEx.Properties["ProcessID"] = processId;
                logEx.Properties["AppID"] = Constants.ApplicationIdWebAPI;
                logEx.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(logEx);

                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }

        /// <summary>
        /// Endpoint encargado de actualizar un contacto en el CRM luego de haber cambiado su tipo de perfil. Por ejemplo, cambio de tipo usuario "Otro interes" a usuario
        /// Paciente o Cuidador/Tutor
        /// </summary>
        /// <param name="request">JSON con las propiedades necesarias para la actualizacion</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateFromSignIntoAccount([FromBody] UserTypeChangeRequest request)
        {
            Guid processId = Guid.NewGuid();

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(request)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);


            EContact contactProcedures = new EContact();
            
            try
            {
                if (request != null)
                {
                    OperationResult updateResult = contactProcedures.UpdateContactFromSignIntoAccount(request, processId);

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
                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name, null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(request)} **END**", null, new Exception(ex.ToString()));
                logEx.Properties["ProcessID"] = processId;
                logEx.Properties["AppID"] = Constants.ApplicationIdWebAPI;
                logEx.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(logEx);

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
        [Authorize]
        public IHttpActionResult UpdatePatient([FromBody] UpdatePatientRequest updateRequest)
        {
            Guid processId = Guid.NewGuid();
            

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(updateRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

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
                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name,null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(updateRequest)} **END**",null,new Exception(ex.ToString()));
                logEx.Properties["ProcessID"] = processId;
                logEx.Properties["AppID"] = Constants.ApplicationIdWebAPI;
                logEx.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(logEx);
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
        [Authorize]
        public void Delete(int id)
        {



        }
    }
}