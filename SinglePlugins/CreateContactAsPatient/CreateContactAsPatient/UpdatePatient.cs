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

    public class UpdatePatient : IPlugin
    {
        private MShared sharedMethods = null;

        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoseEntity doseEntity = null;
        private RequestHelpers helperMethods = null;

       

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

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
                        sharedMethods = new MShared();
                        if (contactUpdated != null)
                        {
                            helperMethods = new RequestHelpers();
                            #region -> Set request data based on Contact


                            //contactMethods = new ContactMethods();

                            string[] columnsToGet = new string[] { contactEntity.Fields.IdAboxPatient, contactEntity.Fields.Country,contactEntity.Fields.Province,contactEntity.Fields.Canton,contactEntity.Fields.District,contactEntity.Fields.Interests, contactEntity.Fields.UserType, contactEntity.Fields.IdType, contactEntity.Fields.Id, contactEntity.Fields.Firstname, contactEntity.Fields.SecondLastname, contactEntity.Fields.Lastname, contactEntity.Fields.Gender, contactEntity.Fields.Birthdate, contactEntity.Fields.ContactxContactLookup,contactEntity.Fields.Phone,contactEntity.Fields.SecondaryPhone,contactEntity.Fields.Email };
                            var columnSet = new ColumnSet(columnsToGet);
                            Entity contactData = service.Retrieve(contactEntity.EntitySingularName, contactUpdated.Id, columnSet);


                            string userType = "";

                            if (contactData.Attributes.Contains(contactEntity.Fields.UserType))
                            {
                                EntityReference userTypeReference = null;
                                userTypeReference = (EntityReference)contactData.Attributes[contactEntity.Fields.UserType];
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
                                if (userType == "01" && !(contactData.Attributes.Contains(contactEntity.Fields.ContactxContactLookup)))
                                {
                                    updatePatientRequest = helperMethods.GetPatientUpdateStructure(contactData, service);
                                }
                                else
                                {
                                    //Si es cuidador, tutor, o paciente que no está a cargo de nadie se usa el update account
                                    updateAccountRequest = helperMethods.GetAccountUpdateStructure(contactData,service);
                                }
                               
                            }



                            #endregion





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
                            wrData.Authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6InJjY3VpZDAxIiwiaWF0IjoxNjAxOTIwNTQ5LCJleHAiOjE2MDIwMDY5NDl9.6M-3n9In6R5ze-r0Z8d1eupIAQSfxyEGZuM7ymroZEY";

                            wrData.Url = serviceUrl;


                            var serviceResponse = sharedMethods.DoPostRequest(wrData);
                            UpdatePatientRequest.ServiceResponse updatePatientResponse = null;
                            UpdateAccountRequest.ServiceResponse updateAccountResponse = null;
                            if (serviceResponse.IsSuccessful)
                            {

                                if (updatePatientRequest!=null)
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

                                        throw new InvalidPluginExecutionException("Ocurrió un error al guardar la información en Abox Plan:\n" + updatePatientResponse.response.message);

                                    }
                                    else
                                    {
                                        //contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                                    }


                                }
                                else if (updateAccountRequest!=null)
                                {
                                    //Leer respuesta del servicio Update Account
                                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(UpdateAccountRequest.ServiceResponse));

                                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                                    {
                                        deserializer = new DataContractJsonSerializer(typeof(UpdateAccountRequest.ServiceResponse));
                                        updateAccountResponse = (UpdateAccountRequest.ServiceResponse)deserializer.ReadObject(ms);
                                    }

                                    if (updateAccountResponse.response.code != "MEMCTRL-1014")
                                    {

                                        throw new InvalidPluginExecutionException("Ocurrió un error al guardar la información en Abox Plan:\n" + updateAccountResponse.response.message);

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
                                throw new InvalidPluginExecutionException("Ocurrió un error al consultar los servicios de Abox Plan" + serviceResponse.ErrorMessage);
                            }

                        }


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
