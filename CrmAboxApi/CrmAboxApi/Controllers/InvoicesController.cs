using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using CrmAboxApi.Logic.Classes.Helper;

//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CrmAboxApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InvoicesController : ApiController
    {
        // GET: Invoices
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  Endpoint que Registra una factura dentro de Dynamics, 
        /// </summary>
        /// <param name="invoiceCreateRequest">JSON exitoso enviado previamente a los servicios de Abox plan, además el ID creado en Base de datos para esta factura</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult Create([FromBody] InvoiceCreate invoiceCreateRequest)
        {
            Guid processId = Guid.NewGuid();

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceCreateRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

            EInvoice invoiceProcedures = new EInvoice();
            OperationResult response = null;
            try
            {
                if (invoiceCreateRequest != null)
                {
                    response = invoiceProcedures.CreateInvoice(invoiceCreateRequest, processId);

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

                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name, null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceCreateRequest)} **END**", null, new Exception(ex.ToString()));
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

        [Authorize]
        [HttpPost]
        public IHttpActionResult Update([FromBody] InvoiceUpdate invoiceUpdateRequest)
        {
            Guid processId = Guid.NewGuid();

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceUpdateRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

            EInvoice invoiceProcedures = new EInvoice();
            OperationResult response = null;
            try
            {
                if (invoiceUpdateRequest != null)
                {
                    response = invoiceProcedures.UpdateInvoice(invoiceUpdateRequest, processId);

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

                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name, null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceUpdateRequest)} **END**", null, new Exception(ex.ToString()));
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
        public IHttpActionResult ApproveInvoice([FromBody] InvoiceApprove invoiceApproveRequest)
        {
            Guid processId = Guid.NewGuid();

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceApproveRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

            EInvoice invoiceProcedures = new EInvoice();
            OperationResult response = null;
            try
            {
                if (invoiceApproveRequest != null)
                {
                    response = invoiceProcedures.ApproveInvoice(invoiceApproveRequest, processId);

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

                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name, null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceApproveRequest)} **END**", null, new Exception(ex.ToString()));
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
        public IHttpActionResult RejectInvoice([FromBody] InvoiceReject invoiceRejectRequest)
        {
            Guid processId = Guid.NewGuid();

            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceRejectRequest)} **END**");
            log.Properties["ProcessID"] = processId;
            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Logger.Log(log);

            EInvoice invoiceProcedures = new EInvoice();
            OperationResult response = null;
            try
            {
                if (invoiceRejectRequest != null)
                {
                    response = invoiceProcedures.RejectInvoice(invoiceRejectRequest, processId);

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

                LogEventInfo logEx = new LogEventInfo(LogLevel.Error, Logger.Name, null, $"Request hacia {Request.RequestUri} con el JSON:**START** {JsonConvert.SerializeObject(invoiceRejectRequest)} **END**", null, new Exception(ex.ToString()));
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

    }
}