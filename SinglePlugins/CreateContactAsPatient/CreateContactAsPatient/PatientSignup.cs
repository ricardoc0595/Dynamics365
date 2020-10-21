using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using CreateContactAsPatient.Classes;
using AboxCrmPlugins;

using Microsoft.Xrm.Sdk;
using AboxCrmPlugins.Classes;
using AboxCrmPlugins.Methods;

using AboxDynamicsBase.Classes.Entities;
using Microsoft.Xrm.Sdk.Query;
using AboxDynamicsBase.Classes;
using CreateContactAsPatient.Methods;

namespace CreateContactAsPatient
{
    public class PatientSignup : IPlugin
    {
        private MShared sharedMethods = null;
        

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
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

                        sharedMethods = new MShared();
                        RequestHelpers reqHelpers = new RequestHelpers();
                        trace.Trace("Obtendo objeto para enviar a servicio Abox...");
                        PatientSignupRequest.Request request = reqHelpers.GetSignupPatientRequestObject(contact,service);

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
                        
                        
                        wrData.Url = AboxServices.PatientSignup;
                        trace.Trace("Url:"+wrData.Url);

                        var serviceResponse = sharedMethods.DoPostRequest(wrData,trace);
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
                                trace.Trace(Constants.ErrorMessageCodeReturned+ serviceResponseProperties.response.code);
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
               
                throw new InvalidPluginExecutionException(ex.Message);
                //TODO: Crear Log
            }
        }
    }




    




}
