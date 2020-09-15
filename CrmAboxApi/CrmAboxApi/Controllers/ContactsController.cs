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
            ServiceResponse response = null;
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

                    return Content(HttpStatusCode.OK, new ServiceResponse
                    {
                        Data = null,
                        Message = "Doctores eliminados",
                        IsSuccessful = true,

                    });

                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.InternalServerError, new ServiceResponse
                    {
                        Data = null,
                        Message = ex.ToString(),
                        IsSuccessful = false,
                        
                    }) ;
                  
                }


            }
            else
            {
                return Content(HttpStatusCode.InternalServerError, new ServiceResponse
                {
                    Data = null,
                    Message = "Error",
                    IsSuccessful = false,

                });
            }

        }

        // POST api/contacts
        [HttpPost]
        [ActionName("patient")]
        public IHttpActionResult Post([FromBody]PatientSignup signupRequest)
        {

            Contact contactProcedures = new Contact();
            ServiceResponse response = null;
            try
            {

                if (signupRequest != null)
                {
                    response = contactProcedures.Create(signupRequest);
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

                    return Content(HttpStatusCode.BadRequest, new ServiceResponse
                    {
                        Code = "",
                        IsSuccessful = false,
                        Data = null,
                        Message = "La solicitud JSON enviada es incorrecta"
                    }) ;

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, new ServiceResponse
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""

                }) ;

            }
           
        }

        // PUT api/contacts/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/contacts/5
        public void Delete( int id)
        {
           

        }
    }
}
