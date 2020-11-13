﻿using AboxCrmPlugins.Classes;
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
    public class UpdatePatient : IPlugin
    {
        private MShared sharedMethods = null;

        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoseEntity doseEntity = null;
        private RequestHelpers helperMethods = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            sharedMethods = new MShared();
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


                //TODO: Limitar ejecucion del PLugin cuando el ID ABOX PATIENT de Dynamics está vacío.


                /*Esta validación previene la ejecución del Plugin de cualquier
                 * transacción realizada a través del Web API desde Abox*/
                if (context.InitiatingUserId == new Guid("7dbf49f3-8be8-ea11-a817-002248029f77"))
                {
                    return;
                }
                
                Entity contactUpdated = null;
                UpdatePatientRequest.Request updatePatientRequest = null;
                UpdateAccountRequest.Request updateAccountRequest = null;
                ContactMethods contactMethods = null;

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    contactEntity = new ContactEntity();
                    contactUpdated = (Entity)context.InputParameters["Target"];
                    if (contactUpdated.LogicalName != contactEntity.EntitySingularName)
                    {
                        return;
                    }
                    else
                    {



                        if (contactUpdated != null)
                        {
                            /*Desde otros plugins, cuando se cambia un valor de un field a nivel de código o se crea una realcion,
                                * automáticamente llama la ejecución de este plugin. Esta validación se hace porque desde el plugin de ChildContactsAssociation,
                                luego de realizar el registro de tutor y el hijo en la BD mediante los servicios, se hace una asociación en Dynamics,
                               y por lo tanto este plugin se ejecuta para actualizar el valor del lookup del contacto hijo referenciando al contacto padre*/
                            if (contactUpdated.Attributes.Keys.Contains(ContactFields.ContactxContactLookup))
                            {
                                return;
                            }

                            if (contactUpdated.Attributes.Keys.Contains(ContactFields.IdAboxPatient))
                            {
                                return;
                            }

                          

                            helperMethods = new RequestHelpers();

                            #region -> Set request data based on Contact

                            //contactMethods = new ContactMethods();

                            string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country, ContactFields.Province, ContactFields.Canton, ContactFields.District, ContactFields.Interests, ContactFields.UserType, ContactFields.IdType, ContactFields.Id, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname, ContactFields.Gender, ContactFields.Birthdate, ContactFields.ContactxContactLookup, ContactFields.Phone, ContactFields.SecondaryPhone, ContactFields.Email };
                            var columnSet = new ColumnSet(columnsToGet);
                            Entity contactData = service.Retrieve(contactEntity.EntitySingularName, contactUpdated.Id, columnSet);


                            if (!contactData.Attributes.Contains(ContactFields.IdAboxPatient))
                            {
                                Exception ex = new Exception($"Este contacto no posee un ID de paciente Abox registrado. Por favor contacte al administrador.");
                                ex.Data["HasFeedbackMessage"] = true;
                                throw ex;
                            }



                            string userType = "";

                            if (contactData.Attributes.Contains(ContactFields.UserType))
                            {
                                EntityReference userTypeReference = null;
                                userTypeReference = (EntityReference)contactData.Attributes[ContactFields.UserType];
                                if (userTypeReference != null)
                                {
                                    userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                                }
                            }

                            /*Recorrer los atributos que cambiaron y sobreescribirselos a la entidad contacto actual para
                             tener la información completa que se enviará al servicio*/
                            foreach (string keyName in contactUpdated.Attributes.Keys)
                            {
                                if (contactData.Attributes.ContainsKey(keyName))
                                {
                                    contactData.Attributes[keyName] = contactUpdated.Attributes[keyName];
                                }
                            }

                            /*Dependiendo del tipo de usuario y si tiene contactos a cargo o no, se utiizan los servicios de
                             updateAccount o updatePatient*/
                            if (!string.IsNullOrEmpty(userType))
                            {
                                /*Validar si es un usuario tipo Paciente y no tiene un cuidador o tutor, se utilizara el servicio
                                 *de update patient*/
                                if (userType == "01" || (contactData.Attributes.Contains(ContactFields.ContactxContactLookup)))
                                {
                                    updatePatientRequest = helperMethods.GetPatientUpdateStructure(contactData, service,trace);
                                }
                                else
                                {
                                    //Si es cuidador, tutor, o paciente que no está a cargo de nadie se usa el update account
                                    updateAccountRequest = helperMethods.GetAccountUpdateStructure(contactData, service,trace);
                                }
                            }else if (contactData.Attributes.Contains(ContactFields.ContactxContactLookup))
                            {
                                updatePatientRequest = helperMethods.GetPatientUpdateStructure(contactData, service, trace);
                            }
                            else
                            {
                                string contactName = "";

                                if (contactData.Attributes.Contains(ContactFields.Firstname))
                                    contactName += contactData.GetAttributeValue<string>(ContactFields.Firstname);

                                if (contactData.Attributes.Contains(ContactFields.Lastname))
                                    contactName += " "+contactData.GetAttributeValue<string>(ContactFields.Lastname);

                                if (contactData.Attributes.Contains(ContactFields.SecondLastname))
                                    contactName += " "+contactData.GetAttributeValue<string>(ContactFields.SecondLastname);

                                Exception ex = new Exception($"Ocurrió un problema identificando el tipo de usuario del contacto.{(!String.IsNullOrEmpty(contactName)?"("+contactName+")":"")}");
                                ex.Data["HasFeedbackMessage"] = true;
                                throw ex;
                            }

                            #endregion -> Set request data based on Contact

                            DataContractJsonSerializer serializer = null;
                            MemoryStream memoryStream = new MemoryStream();
                            string serviceUrl = "";
                            if (updatePatientRequest != null)
                            {
                                serializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.Request));
                                serializer.WriteObject(memoryStream, updatePatientRequest);
                                serviceUrl = AboxServices.UpdatePatientService;
                            }
                            else if (updateAccountRequest != null)
                            {
                                serializer = new DataContractJsonSerializer(typeof(UpdateAccountRequest.Request));
                                serializer.WriteObject(memoryStream, updateAccountRequest);
                                serviceUrl = AboxServices.UpdateAccountService;
                            }

                            var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                            memoryStream.Dispose();

                            //Valores necesarios para hacer el Post Request
                            WebRequestData wrData = new WebRequestData();
                            wrData.InputData = jsonObject;
                            wrData.ContentType = "application/json";
                            wrData.Authorization = "Bearer " + Constants.TokenForAboxServices;

                            wrData.Url = serviceUrl;

                            var serviceResponse = sharedMethods.DoPostRequest(wrData, trace);
                            UpdatePatientRequest.ServiceResponse updatePatientResponse = null;
                            UpdateAccountRequest.ServiceResponse updateAccountResponse = null;
                            if (serviceResponse.IsSuccessful)
                            {
                                if (updatePatientRequest != null)
                                {
                                    //Leer respuesta del servicio de Update Patient

                                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.ServiceResponse));

                                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                                    {
                                        deserializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.ServiceResponse));
                                        updatePatientResponse = (UpdatePatientRequest.ServiceResponse)deserializer.ReadObject(ms);
                                    }

                                    if (updatePatientResponse.response.code != "MEMCTRL-1014")
                                    {
                                        throw new InvalidPluginExecutionException(Constants.ErrorMessageTransactionCodeReturned + updatePatientResponse.response.message);
                                    }
                                    else
                                    {
                                        //contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                                    }
                                }
                                else if (updateAccountRequest != null)
                                {
                                    //Leer respuesta del servicio Update Account
                                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(UpdateAccountRequest.ServiceResponse));

                                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                                    {
                                        deserializer = new DataContractJsonSerializer(typeof(UpdateAccountRequest.ServiceResponse));
                                        updateAccountResponse = (UpdateAccountRequest.ServiceResponse)deserializer.ReadObject(ms);
                                    }

                                    if (updateAccountResponse.response.code != "MEMCTRL-1015")
                                    {
                                        trace.Trace(Constants.ErrorMessageCodeReturned + updateAccountResponse.response.code);
                                        throw new InvalidPluginExecutionException(Constants.ErrorMessageTransactionCodeReturned + updateAccountResponse.response.message);
                                    }
                                    else
                                    {
                                        //contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Manejar error, esta llegando null cuando hay un error de protocolo
                                throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage);
                            }
                        }
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

                //TODO: Crear Log
            }
        }
    }
}