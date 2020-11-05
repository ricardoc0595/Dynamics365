using AboxCrmPlugins.Classes;
using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using CreateContactAsPatient.Classes;
using CreateContactAsPatient.Methods;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CreateContactAsPatient
{
    public class PatientSignup : IPlugin
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
                    Entity contact = (Entity)context.InputParameters["Target"];

                    if (contact.LogicalName != "contact")
                    {
                        return;
                    }
                    else
                    {
                        
                        RequestHelpers reqHelpers = new RequestHelpers();

                        /*TODO: Cuando se esta creando un paciente que va a ser un paciente bajo cuido de un tutor/cuidador,
                        Se tiene que identificar este escenario para no hacer ningun request aca, la creacion del paciente se hace
                        desde el associate*/

                        if (contact.GetAttributeValue<bool>(ContactFields.IsChildContact))
                        {
                            return;
                        }

                        PatientSignupRequest.Request request = reqHelpers.GetSignupPatientRequestObject(contact, service,trace);



                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PatientSignupRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, request);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        trace.Trace("Objeto Json:" + jsonObject);
                        wrData.ContentType = "application/json";
                        
                        if (request.userType=="02"||request.userType=="03")
                        {
                            wrData.Url = AboxServices.MainPatientForTutorOrCaretakerService;
                            wrData.Authorization = Constants.TokenForAboxServices;
                        }
                        else
                        {
                            wrData.Url = AboxServices.PatientSignup;
                        }


                        trace.Trace("Url:" + wrData.Url);

                       
                        var serviceResponse = sharedMethods.DoPostRequest(wrData, trace);
                        PatientSignupRequest.ServiceResponse serviceResponseProperties = null;
                        if (serviceResponse.IsSuccessful)
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(PatientSignupRequest.ServiceResponse));

                            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                            {
                                deserializer = new DataContractJsonSerializer(typeof(PatientSignupRequest.ServiceResponse));
                                serviceResponseProperties = (PatientSignupRequest.ServiceResponse)deserializer.ReadObject(ms);
                            }

                            if (serviceResponseProperties.response.code != "MEMEX-0002")
                            {
                                trace.Trace(Constants.ErrorMessageCodeReturned + serviceResponseProperties.response.code);

                                #region debug log

                                try
                                {
                                    sharedMethods.LogPluginFeedback(new LogClass
                                    {
                                        Exception = "",
                                        Level = "debug",
                                        ClassName = this.GetType().ToString(),
                                        MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        Message = $"Url:{wrData.Url} ResponseCode:{serviceResponseProperties.response.code}",
                                        ProcessId = ""
                                    }, trace);
                                }
                                catch (Exception e)
                                {

                                }

                                #endregion

                                throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                            }
                            else
                            {
                                contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                            }
                        }
                        else
                        {
                            trace.Trace(Constants.GeneralAboxServicesErrorMessage);
                            throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                        }
                        //TODO: Capturar excepción con servicios de Abox Plan y hacer un Logging
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

                throw new InvalidPluginExecutionException(Constants.GeneralPluginErrorMessage);
                //TODO: Crear Log
            }
        }
    }
}