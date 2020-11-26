using AboxCrmPlugins.Classes;
using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using CreateContactAsPatient.Classes;
using CreateContactAsPatient.Methods;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CreateContactAsPatient
{
    public class ChildContactsAssociation : IPlugin
    {
        private MShared sharedMethods = null;

        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoctorEntity doctorEntity = null;
        private RequestHelpers helperMethods = null;

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

                EntityReference targetEntity = null;

                string relationshipName = string.Empty;

                EntityReferenceCollection relatedEntities = null;

                EntityReference doctorRelated = null;

                Entity parentContact = null;

                UpdatePatientRequest.Request updatePatientRequest = null;

                #region Associate & Disassociate

                // if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate")
                if (context.MessageName.ToLower() == "associate")
                {
                    contactEntity = new ContactEntity();

                    // Get the “Relationship” Key from context

                    if (context.InputParameters.Contains("Relationship"))

                    {
                        // Get the Relationship name for which this plugin fired
                        relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                    }

                    // Check the "Relationship Name" with your intended one

                    if (relationshipName != ContactFields.ContactxContactRelationship)
                    {
                        return;
                    }
                    else
                    {
                        productEntity = new ProductEntity();
                        updatePatientRequest = new UpdatePatientRequest.Request();

                        #region -> Target

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                        {
                            helperMethods = new RequestHelpers();
                            targetEntity = (EntityReference)context.InputParameters["Target"];

                            string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.UserType, ContactFields.Id, ContactFields.Country, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname,ContactFields.Email };
                            var columnSet = new ColumnSet(columnsToGet);
                            parentContact = service.Retrieve(contactEntity.EntitySingularName, targetEntity.Id, columnSet);
                        }

                        #endregion -> Target

                        #region -> Related

                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;

                        string userType = "";
                        if (parentContact.Attributes.Contains(ContactFields.UserType))
                        {
                            EntityReference userTypeReference = null;
                            userTypeReference = (EntityReference)parentContact.Attributes[ContactFields.UserType];
                            if (userTypeReference != null)
                            {
                                userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                            }
                        }

                        if (userType == "02" || userType == "03")
                        {
                            if (relatedEntities.Count > 0)
                            {
                                RequestHelpers reqHelpers = new RequestHelpers();

                                ContactMethods contactMethods = new ContactMethods();

                                var contactsRelated = contactMethods.GetContactChildContacts(parentContact, service);

                                if (contactsRelated != null)
                                {
                                    if (contactsRelated.Entities.Count + relatedEntities.Count > 1 || relatedEntities.Count > 1)
                                    {
                                     
                                        //TODO: QUICK CREATE se comporta distinto, entra aqui despues de haber asociado y esta mandando este mensaje
                                        //TODO:Hacer constante configurable de cantidad de bajo cuido permitidos

                                        Exception ex = new Exception("No es posible asignar más de un contacto bajo cuido.");
                                        ex.Data["HasFeedbackMessage"] = true;
                                        throw ex;
                                    }
                                }

                                string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country, ContactFields.UserType, ContactFields.IdType, ContactFields.Id, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname, ContactFields.Gender, ContactFields.Birthdate, ContactFields.Email };
                                var columnSet = new ColumnSet(columnsToGet);

                                for (int i = 0; i < relatedEntities.Count; i++)
                                {
                                    EntityReference r = relatedEntities[i];
                                    Entity childContactToAssociate = service.Retrieve(contactEntity.EntitySingularName, r.Id, columnSet);
                                    PatientSignupRequest.Request request = reqHelpers.GetSignupPatientUnderCareRequestObject(childContactToAssociate, parentContact, service, trace);

                                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PatientSignupRequest.Request));
                                    MemoryStream memoryStream = new MemoryStream();
                                    serializer.WriteObject(memoryStream, request);
                                    var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                                    memoryStream.Dispose();

                                    //Valores necesarios para hacer el Post Request
                                    WebRequestData wrData = new WebRequestData();
                                    wrData.InputData = jsonObject;

                                    wrData.ContentType = "application/json";
                                    wrData.Authorization = Constants.TokenForAboxServices;

                                    if (userType == "02")
                                        wrData.Url = AboxServices.CaretakerChildService;
                                    else if (userType == "03")
                                        wrData.Url = AboxServices.TutorChildService;

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

                                            #endregion debug log

                                            //TODO: No se esta respetando este throw y el plugin se esta ejecutando de todas formas
                                            Exception serviceEx = new Exception(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                                            serviceEx.Data["HasFeedbackMessage"] = true;
                                            throw serviceEx;
                                        }
                                        else
                                        {
                                            //Esta linea automaticamente llama al plugin de Update y se esta llamando sin intención
                                            //childContactToAssociate.Attributes.Add(ContactFields.IdAboxPatient, serviceResponseProperties.response.details.idPaciente);

                                            try
                                            {
                                                childContactToAssociate[ContactFields.IdAboxPatient] = serviceResponseProperties.response.details.idPaciente;
                                                service.Update(childContactToAssociate);
                                            }
                                            catch (Exception ex)
                                            {
                                                #region log

                                                try
                                                {
                                                    sharedMethods.LogPluginFeedback(new LogClass
                                                    {
                                                        Exception = ex.ToString(),
                                                        Level = "debug",
                                                        ClassName = this.GetType().ToString(),
                                                        MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                                                        Message = $"Error actualizando la entidad",
                                                        ProcessId = ""
                                                    }, trace);
                                                }
                                                catch (Exception e)
                                                {
                                                }

                                                #endregion log
                                            }

                                            try
                                            {
                                                sharedMethods.LogPluginFeedback(new LogClass
                                                {
                                                    Exception = "",
                                                    Level = "debug",
                                                    ClassName = this.GetType().ToString(),
                                                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                                                    Message = $"Valor de field de idaboxpatient {Convert.ToString(childContactToAssociate.GetAttributeValue<int>(ContactFields.IdAboxPatient))}, Valor devuelto por el servicio:{serviceResponseProperties.response.details.idPaciente}",
                                                    ProcessId = ""
                                                }, trace);
                                            }
                                            catch (Exception e)
                                            {
                                            }

                                            PatientSignupRequest.ServiceResponse welcomeServiceResponseProperties = null;
                                            PatientSignupRequest.Request requestForWelcome = reqHelpers.GetWelcomeMailRequestForTutorsAndCaretakers(parentContact, service, trace);

                                            serializer = new DataContractJsonSerializer(typeof(PatientSignupRequest.Request));
                                            memoryStream = new MemoryStream();
                                            serializer.WriteObject(memoryStream, requestForWelcome);
                                            var jsonObjectForWelcomeMail = Encoding.Default.GetString(memoryStream.ToArray());
                                            memoryStream.Dispose();

                                            WebRequestData wrDataWelcomeMail = new WebRequestData();
                                            wrDataWelcomeMail.InputData = jsonObjectForWelcomeMail;
                                            wrDataWelcomeMail.ContentType = "application/json";
                                            wrDataWelcomeMail.Authorization = Constants.TokenForAboxServices;
                                            wrDataWelcomeMail.Url = AboxServices.WelcomeSendMailService;

                                            var serviceResponseWelcome = sharedMethods.DoPostRequest(wrDataWelcomeMail, trace);

                                            if (serviceResponseWelcome.IsSuccessful)
                                            {
                                                DataContractJsonSerializer deserializerWelcomeResponse = new DataContractJsonSerializer(typeof(PatientSignupRequest.ServiceResponse));

                                                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponseWelcome.Data)))
                                                {
                                                    deserializerWelcomeResponse = new DataContractJsonSerializer(typeof(PatientSignupRequest.ServiceResponse));
                                                    welcomeServiceResponseProperties = (PatientSignupRequest.ServiceResponse)deserializerWelcomeResponse.ReadObject(ms);
                                                }

                                                if (welcomeServiceResponseProperties.response.code != "0")
                                                {
                                                    trace.Trace(Constants.ErrorMessageCodeReturned + welcomeServiceResponseProperties.response.code);

                                                    #region debug log

                                                    try
                                                    {
                                                        sharedMethods.LogPluginFeedback(new LogClass
                                                        {
                                                            Exception = "",
                                                            Level = "debug",
                                                            ClassName = this.GetType().ToString(),
                                                            MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                                                            Message = $"Url:{wrDataWelcomeMail.Url} ResponseCode:{welcomeServiceResponseProperties.response.code}",
                                                            ProcessId = ""
                                                        }, trace);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                    }

                                                    #endregion debug log

                                                    Exception serviceEx = new Exception(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                                                    serviceEx.Data["HasFeedbackMessage"] = true;
                                                    throw serviceEx;
                                                }
                                                else
                                                {
                                                }
                                            }
                                            else
                                            {
                                                trace.Trace(Constants.GeneralAboxServicesErrorMessage);
                                                //validar mensaje de respuesta
                                                throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage);
                                            }

                                            //var serviceResponse = sharedMethods.DoPostRequest(wrData, trace);
                                        }
                                    }
                                    else
                                    {
                                        trace.Trace(Constants.GeneralAboxServicesErrorMessage);
                                        throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                                    }
                                }
                            }
                        }

                        #endregion -> Related
                    }
                }

                #endregion Associate & Disassociate

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