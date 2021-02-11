using AboxDynamicsBase.Classes;
using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using CrmAboxApi.Security;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Http;

namespace CrmAboxApi.Controllers
{
    public class SecurityController : ApiController
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // GET: Security
        [HttpPost]
        public IHttpActionResult Authenticate(WebApiLogin user)
        {
          

            OperationResult response = null;
            try
            {
                if (user != null)
                {

                    string hashedPassword = HashGenerator.Sha256_hash(user.Password);
                    if (hashedPassword.ToUpper()== ConfigurationManager.AppSettings["AdminPwd"])
                    {
                        var token = TokenGenerator.GenerateTokenJwt(user.User,"");
                        return Ok(new OperationResult
                        {
                            IsSuccessful = true,
                            Data = token,
                            Message = "",
                            Code = ""
                        });
                    }
                    else
                    {
                        return Content(HttpStatusCode.InternalServerError, new OperationResult
                        {
                            IsSuccessful = false,
                            Data = null,
                            Message = "Credenciales incorrectos",
                            Code = ""
                        });
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

               

                return Content(HttpStatusCode.InternalServerError, new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = ex.ToString(),
                    Code = ""
                });
            }
        }
    }
}