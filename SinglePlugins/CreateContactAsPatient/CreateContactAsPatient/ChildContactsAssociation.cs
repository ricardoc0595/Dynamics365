using AboxCrmPlugins.Methods;
using AboxCrmPlugins.Classes;
using AboxDynamicsBase.Classes.Entities;
using CreateContactAsPatient.Classes;
using CreateContactAsPatient.Methods;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using AboxDynamicsBase.Classes;

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

                Entity contact = null;

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

                            string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.UserType };
                            var columnSet = new ColumnSet(columnsToGet);
                            contact = service.Retrieve(contactEntity.EntitySingularName, targetEntity.Id, columnSet);

                            //updatePatientRequest.personalinfo = new UpdatePatientRequest.Request.Personalinfo();

                            //updatePatientRequest = helperMethods.GetPatientUpdateStructure(contact, service, trace);
                        }

                        #endregion -> Target

                        #region -> Related


                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;

                        string userType = "";
                        if (contact.Attributes.Contains(ContactFields.UserType))
                        {
                            EntityReference userTypeReference = null;
                            userTypeReference = (EntityReference)contact.Attributes[ContactFields.UserType];
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
                                //Hacer constante configurable de cantidad de bajo cuido permitidos
                                if (relatedEntities.Count >= 2)
                                {
                                    throw new InvalidPluginExecutionException("Solo puede asociarse un paciente a un usuario tutor o cuidador");
                                }

                                string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country, ContactFields.UserType, ContactFields.IdType, ContactFields.Id, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname, ContactFields.Gender, ContactFields.Birthdate };
                                var columnSet = new ColumnSet(columnsToGet);

                                for (int i = 0; i < relatedEntities.Count; i++)
                                {
                                    EntityReference r = relatedEntities[i];
                                    Entity childContactToAssociate = service.Retrieve(contactEntity.EntitySingularName, r.Id, columnSet);
                                    PatientSignupRequest.Request request = reqHelpers.GetSignupPatientUnderCareRequestObject(childContactToAssociate, service, trace);


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


                                    if (userType == "02")
                                        wrData.Url = AboxServices.CaretakerChildService;
                                    else if (userType == "03")
                                        wrData.Url = AboxServices.TutorChildService;



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

                                            //TODO: Llamar servicio de enviar correo



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

                        #endregion

                    }
                }

                #endregion Associate 
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

