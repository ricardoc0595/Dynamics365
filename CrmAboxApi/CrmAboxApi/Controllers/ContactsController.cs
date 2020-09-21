using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using Logic.CrmAboxApi.Classes.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            Doctor doctorProcess = new Doctor();
            OperationResult response = null;
            var responseAllDoctors = doctorProcess.RetrieveAll();

            if (responseAllDoctors.IsSuccessful)
            {
                try
                {
                    RetrieveDoctorFromWebAPI doctorsResponse = (RetrieveDoctorFromWebAPI)responseAllDoctors.Data;

                    //doctorProcess.Delete(doctorsResponse.value[0].new_doctorid);

                   

                    foreach (var doc in doctorsResponse.value)
                    {
                        doctorProcess.Delete(doc.new_doctorid);
                    }

                    return Content(HttpStatusCode.OK, new OperationResult
                    {
                        Data = null,
                        Message = "Doctores eliminados",
                        IsSuccessful = true,

                    });

                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.InternalServerError, new OperationResult
                    {
                        Data = null,
                        Message = ex.ToString(),
                        IsSuccessful = false,
                        
                    }) ;
                  
                }


            }
            else
            {
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    Data = null,
                    Message = "Error",
                    IsSuccessful = false,

                });
            }

        }

        //POST api/contacts

        [HttpGet]
        public IHttpActionResult GetToken()
        {
            string token = "";
            CrmAboxApi.Logic.Methods.MDynamicsWebApiFunctions webAPiFunctions = new Logic.Methods.MDynamicsWebApiFunctions();

            token=webAPiFunctions.GetTestToken();
            return Ok(token);
           

        }

        public IHttpActionResult Post([FromBody] PatientSignup signupRequest)
        {

            Contact contactProcedures = new Contact();
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
                                response = contactProcedures.CreateAsPatient(signupRequest, null);
                                break;
                            case "02":
                                response = contactProcedures.CreateAsCaretaker(signupRequest);
                                break;
                            case "03":
                                response = contactProcedures.CreateAsTutor(signupRequest);
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
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""

                });

            }

        }

       [HttpPut]
        public IHttpActionResult AccountUpdate([FromBody] UpdateAccountRequest updateRequest)
        {
            Contact contactProcedures = new Contact();
            OperationResult response = null;
            try
            {

                if (updateRequest != null)
                {
                    OperationResult updateResult = contactProcedures.UpdateAccount(updateRequest);

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
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""

                });

            }
        }

        [HttpPut]
        public IHttpActionResult UpdatePatient([FromBody]UpdatePatientRequest updateRequest)
        {
            Contact contactProcedures = new Contact();
            OperationResult response = null;
            try
            {

                if (updateRequest != null)
                {
                    OperationResult updateResult = contactProcedures.UpdatePatient(updateRequest);

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
                Logger.Error(ex);
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
        public void Delete( int id)
        {
           

        }
    }
}
