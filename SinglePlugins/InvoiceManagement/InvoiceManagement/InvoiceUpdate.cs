using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AboxDynamicsBase.Classes.Entities;
using Microsoft.Xrm.Sdk.Query;
using InvoiceManagement.Methods;
using InvoiceManagement.Classes;
using System.Runtime.Serialization.Json;
using System.IO;
using AboxCrmPlugins.Classes;

namespace InvoiceManagement
{
    public class InvoiceUpdate : IPlugin
    {

        private MShared sharedMethods = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            sharedMethods = new MShared();

            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                // The InputParameters collection contains all the data passed in the message request.

                /*Esta validación previene la ejecución del Plugin de cualquier
                * transacción realizada a través del Web API desde Abox*/
                if (context.InitiatingUserId == new Guid("7dbf49f3-8be8-ea11-a817-002248029f77"))
                {
                    return;
                }

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.
                    Entity invoiceUpdated = (Entity)context.InputParameters["Target"];

                    if (invoiceUpdated.LogicalName != "invoice" )
                    {
                        return;
                    }
                    else
                    {
                        InvoiceEntity invoiceEntity = new InvoiceEntity();
                        RequestHelpers reqHelpers = new RequestHelpers();
                        InvoiceMethods invoiceMethods = new InvoiceMethods();


                        //

                        

                        string[] columnsToGet = new string[] { InvoiceFields.InvoiceNumber,InvoiceFields.IdAboxInvoice, InvoiceFields.Customer,InvoiceFields.Pharmacy,InvoiceFields.ProductsSelectedJson,InvoiceFields.InvoiceImageWebUrl,InvoiceFields.PurchaseDate,InvoiceFields.EntityId};
                        var columnSet = new ColumnSet(columnsToGet);
                        Entity invoiceToSave = service.Retrieve(invoiceEntity.EntitySingularName, invoiceUpdated.Id, columnSet);

                        //Limitar ejecucion del Plugin cuando el ID ABOX Invoice de Dynamics está vacío.
                        if (!invoiceToSave.Attributes.Contains(InvoiceFields.IdAboxInvoice))
                        {
                            Exception ex = new Exception($"Esta factura no posee un ID de factura Abox registrado. Por favor contacte al administrador.");
                            ex.Data["HasFeedbackMessage"] = true;
                            throw ex;
                        }

                        string newInvoiceNumber = null;
                        string originalInvoiceNumber = null;
                        if (invoiceUpdated.Attributes.Contains(InvoiceFields.InvoiceNumber))
                        {
                            newInvoiceNumber = invoiceUpdated.Attributes[InvoiceFields.InvoiceNumber].ToString();
                        }

                        if (invoiceToSave.Attributes.Contains(InvoiceFields.InvoiceNumber))
                        {
                            originalInvoiceNumber= invoiceToSave.Attributes[InvoiceFields.InvoiceNumber].ToString();
                        }


                        //borrar 

                        sharedMethods.LogPluginFeedback(new LogClass
                        {
                            Exception = "",
                            Level = "debug",
                            ClassName = this.GetType().ToString(),
                            MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                            Message = $"original: {originalInvoiceNumber} - New: {newInvoiceNumber}",
                            ProcessId = ""
                        }, trace);

                        //borrar

                        /*Recorrer los atributos que cambiaron y sobreescribirselos a la entidad factura actual para
                         tener la información completa que se enviará al servicio*/
                        foreach (string keyName in invoiceUpdated.Attributes.Keys)
                        {
                            if (invoiceToSave.Attributes.ContainsKey(keyName))
                            {
                                invoiceToSave.Attributes[keyName] = invoiceUpdated.Attributes[keyName];
                            }
                        }

                        //



                        var validationStatusMessages = invoiceMethods.GetEntityValidationStatus(invoiceToSave, service, trace);

                        if (validationStatusMessages.Count > 0)
                        {
                            string messageRows = "";


                            //foreach (var message in validationStatusMessages)
                            //{
                            //    messageRows += message+"\n";
                            //}

                            /*El mensaje que se envia al usuario a Dynamics es poco amigable y si se envia un mensaje muy largo, la forma en que lo muestra es completamente
                            ilegible, por esto solo se muestra un mensaje a la vez
                            Para mostrar un mensaje mas amigable, hay que implementar un propio boton de Save en el Ribbon*/
                            messageRows = validationStatusMessages[0];
                            Exception ex = new Exception($"{messageRows}");
                            ex.Data["HasFeedbackMessage"] = true;
                            throw ex;
                        }


                        #region Verificar número de factura

                        if ((newInvoiceNumber!=null) && (newInvoiceNumber!=originalInvoiceNumber))
                        {
                            //Verificar numero de factura

                            InvoiceNumberCheckRequest.Request checkInvoiceRequest = reqHelpers.CheckInvoiceNumberRequest(invoiceToSave, service, trace);

                            DataContractJsonSerializer serializerCheckNumber = new DataContractJsonSerializer(typeof(InvoiceNumberCheckRequest.Request));
                            MemoryStream memoryStreamCheckNumber = new MemoryStream();
                            serializerCheckNumber.WriteObject(memoryStreamCheckNumber, checkInvoiceRequest);
                            var jsonObjectCheckRequest = Encoding.Default.GetString(memoryStreamCheckNumber.ToArray());
                            memoryStreamCheckNumber.Dispose();

                            //Valores necesarios para hacer el Post Request
                            WebRequestData wrDataCheckNumber = new WebRequestData();
                            wrDataCheckNumber.InputData = jsonObjectCheckRequest;

                            wrDataCheckNumber.ContentType = "application/json";

                            // sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());

                            wrDataCheckNumber.Url = AboxServices.CheckInvoiceNumber;
                            wrDataCheckNumber.Authorization = Configuration.TokenForAboxServices;



                            var serviceResponseCheckNumber = sharedMethods.DoPostRequest(wrDataCheckNumber, trace);
                            InvoiceNumberCheckRequest.ServiceResponse serviceCheckNumberResponseProperties = null;
                            if (serviceResponseCheckNumber.IsSuccessful)
                            {
                                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(InvoiceNumberCheckRequest.ServiceResponse));

                                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponseCheckNumber.Data)))
                                {
                                    deserializer = new DataContractJsonSerializer(typeof(InvoiceNumberCheckRequest.ServiceResponse));
                                    serviceCheckNumberResponseProperties = (InvoiceNumberCheckRequest.ServiceResponse)deserializer.ReadObject(ms);
                                }
                            }

                            if (serviceCheckNumberResponseProperties != null)
                            {

                                if ((serviceCheckNumberResponseProperties.response != null) && (serviceCheckNumberResponseProperties.response.message != null))
                                {

                                    if (serviceCheckNumberResponseProperties.response.code.ToString() != "0")
                                    {
                                        Exception ex = new Exception($"{serviceCheckNumberResponseProperties.response.message}");
                                        ex.Data["HasFeedbackMessage"] = true;
                                        throw ex;
                                    }
                                }
                            }
                            else
                            {
                                Exception ex = new Exception("Ha ocurrido un error al intentar validar el número de la factura, por favor intente nuevamente.");
                                ex.Data["HasFeedbackMessage"] = true;
                                throw ex;
                            }
                        }



                        //end verificar numero de factura

                        #endregion

                        InvoiceUpdateRequest.Request request = reqHelpers.GetInvoiceUpdateRequestObject(invoiceToSave, service, trace);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(InvoiceUpdateRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, request);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        trace.Trace("Objeto Json:" + jsonObject);
                        wrData.ContentType = "application/json";

                        // sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());

                        wrData.Url = AboxServices.UpdateInvoice;
                        wrData.Authorization = Configuration.TokenForAboxServices;

                        

                        var serviceResponse = sharedMethods.DoPostRequest(wrData, trace);
                        InvoiceUpdateRequest.ServiceResponse serviceResponseProperties = null;
                        if (serviceResponse.IsSuccessful)
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(InvoiceUpdateRequest.ServiceResponse));

                            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                            {
                                deserializer = new DataContractJsonSerializer(typeof(InvoiceUpdateRequest.ServiceResponse));
                                serviceResponseProperties = (InvoiceUpdateRequest.ServiceResponse)deserializer.ReadObject(ms);
                            }


                            //TODO:Revisar si viene como 0 string o null desde el servicio
                            if (serviceResponseProperties.response.code.ToString() != "0")
                            {
                                trace.Trace(Constants.ErrorMessageCodeReturned + serviceResponseProperties.response.code);

                                //Exception serviceEx = new Exception(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);

                                //serviceEx.Data["HasFeedbackMessage"] = true;
                                //throw serviceEx;




                                //

                                if (serviceResponseProperties.response.details.validationresults.Count > 0)
                                {
                                    string messageRows = "";


                                    //foreach (var message in validationStatusMessages)
                                    //{
                                    //    messageRows += message+"\n";
                                    //}

                                    /*El mensaje que se envia al usuario a Dynamics es poco amigable y si se envia un mensaje muy largo, la forma en que lo muestra es completamente
                                    ilegible, por esto solo se muestra un mensaje a la vez
                                    Para mostrar un mensaje mas amigable, hay que implementar un propio boton de Save en el Ribbon*/
                                    string message = "";
                                    if (serviceResponseProperties.response.details.validationresults[0].validationresults != null)
                                    {
                                        if (serviceResponseProperties.response.details.validationresults[0].validationresults.validationMessages.Count > 0)
                                        {
                                            message = serviceResponseProperties.response.details.validationresults[0].validationresults.validationMessages.ElementAtOrDefault(0).message;
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(serviceResponseProperties.response.details.validationresults[0].message))
                                    {
                                        message = serviceResponseProperties.response.details.validationresults[0].message;
                                    }

                                    if (!String.IsNullOrEmpty(message))
                                    {
                                        messageRows = message;

                                    }
                                    else
                                    {
                                        messageRows = "Ocurrió un error al actualizar la factura, por favor intenta nuevamente.";
                                    }
                                    Exception ex = new Exception($"{messageRows}");
                                    ex.Data["HasFeedbackMessage"] = true;
                                    throw ex;
                                }

                                //

                            }
                            else
                            {
                                //invoiceUpdated.Attributes.Add(InvoiceFields.IdAboxInvoice, serviceResponseProperties.response.details.idFactura);
                            }
                        }
                        else
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(InvoiceUpdateRequest.ServiceResponse));

                            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                            {
                                deserializer = new DataContractJsonSerializer(typeof(InvoiceUpdateRequest.ServiceResponse));
                                serviceResponseProperties = (InvoiceUpdateRequest.ServiceResponse)deserializer.ReadObject(ms);
                            }

                            if (serviceResponseProperties != null && !String.IsNullOrEmpty(serviceResponseProperties.response.message))
                            {
                                Exception serviceEx = new Exception(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                                serviceEx.Data["HasFeedbackMessage"] = true;
                                throw serviceEx;
                            }
                            else
                            {
                                Exception ex = new Exception(Constants.GeneralAboxServicesErrorMessage);
                                ex.Data["HasFeedbackMessage"] = true;
                                throw ex;
                            }
                        }

                        ///

                    }

                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());

                try
                {
                    sharedMethods.LogPluginFeedback(new LogClass
                    {
                        Exception = ex.ToString(),
                        Level = "error",
                        ClassName = this.GetType().ToString(),
                        MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = "Excepción en plugin",
                        ProcessId = ""
                    }, trace);
                }
                catch (Exception e)
                {
                    trace.Trace($"MethodName: {new System.Diagnostics.StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + e.ToString());
                }

                if (ex.Data["HasFeedbackMessage"] != null)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
                else
                {
                    throw new InvalidPluginExecutionException(Constants.GeneralPluginErrorMessage);
                }
            }
        }

    }


}

