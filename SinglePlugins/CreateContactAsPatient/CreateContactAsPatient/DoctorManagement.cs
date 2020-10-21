﻿using System;
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

    public class DoctorManagement : IPlugin
    {
        private MShared sharedMethods = null;

        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoctorEntity doctorEntity = null;
        private RequestHelpers helperMethods = null;
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

                EntityReference targetEntity = null;

                string relationshipName = string.Empty;

                EntityReferenceCollection relatedEntities = null;

                EntityReference doctorRelated = null;


                Entity contact = null;



                UpdatePatientRequest.Request updatePatientRequest = null;

                #region Associate & Disassociate

                // if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate")
                if (context.MessageName.ToLower() == "associate" || context.MessageName.ToLower() == "disassociate")
                {

                    contactEntity = new ContactEntity();
                    sharedMethods = new MShared();
                    // Get the “Relationship” Key from context

                    if (context.InputParameters.Contains("Relationship"))

                    {
                        // Get the Relationship name for which this plugin fired
                        relationshipName = ((Relationship)context.InputParameters["Relationship"]).SchemaName;
                    }



                    // Check the "Relationship Name" with your intended one

                    if (relationshipName != ContactFields.ContactxDoctorRelationship)
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


                            string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country, ContactFields.UserType, ContactFields.IdType, ContactFields.Id, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname, ContactFields.Gender, ContactFields.Birthdate };
                            var columnSet = new ColumnSet(columnsToGet);
                            contact = service.Retrieve(contactEntity.EntitySingularName, targetEntity.Id, columnSet);


                            //updatePatientRequest.personalinfo = new UpdatePatientRequest.Request.Personalinfo();

                            updatePatientRequest = helperMethods.GetPatientUpdateStructure(contact, service);

                        }

                        #endregion

                        #region -> Related

                        doctorEntity = new DoctorEntity();
                        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;


                        if (relatedEntities.Count > 0)
                        {
                            if (context.MessageName.ToLower() == "associate")
                            {
                                #region -> Associate
                                int relatedEntitiesCount = relatedEntities.Count;
                                int contactCurrentRelatedDoctors = 0;
                                System.Collections.Generic.List<UpdatePatientRequest.Request.Medic> medicsToSave = new System.Collections.Generic.List<UpdatePatientRequest.Request.Medic>();
                                if (updatePatientRequest.medication == null)
                                {
                                    updatePatientRequest.medication = new UpdatePatientRequest.Request.Medication();
                                    updatePatientRequest.medication.medics = new UpdatePatientRequest.Request.Medic[relatedEntitiesCount];
                                }
                                else
                                {

                                    if (updatePatientRequest.medication.medics == null)
                                    {
                                        updatePatientRequest.medication.medics = new UpdatePatientRequest.Request.Medic[relatedEntitiesCount];
                                    }
                                    else
                                    {
                                        contactCurrentRelatedDoctors = updatePatientRequest.medication.medics.Length;

                                        if (contactCurrentRelatedDoctors > 0)
                                        {
                                            for (int i = 0; i < contactCurrentRelatedDoctors; i++)
                                            {
                                                medicsToSave.Add(updatePatientRequest.medication.medics[i]);
                                            }

                                            updatePatientRequest.medication.medics = new UpdatePatientRequest.Request.Medic[relatedEntitiesCount + contactCurrentRelatedDoctors];

                                        }


                                    }
                                }



                                for (int i = 0; i < relatedEntitiesCount; i++)
                                {

                                    doctorRelated = relatedEntities[i];

                                    Entity doctor = service.Retrieve(doctorEntity.EntitySingularName, doctorRelated.Id, new ColumnSet(DoctorFields.DoctorIdKey));
                                    if (doctor.Attributes.Contains(DoctorFields.DoctorIdKey))
                                    {
                                        medicsToSave.Add(new UpdatePatientRequest.Request.Medic
                                        {
                                            medicid = doctor.GetAttributeValue<string>(DoctorFields.DoctorIdKey)
                                        });
                                    }
                                }

                                int totalMedicsToSaveCount = updatePatientRequest.medication.medics.Length;
                                for (int i = 0; i < totalMedicsToSaveCount; i++)
                                {
                                    updatePatientRequest.medication.medics[i] = medicsToSave[i];
                                }
                                #endregion
                            }
                            else
                            {
                                #region -> Disassociate
                                if (updatePatientRequest.medication != null)
                                {

                                    if (updatePatientRequest.medication.medics != null)
                                    {
                                        System.Collections.Generic.List<UpdatePatientRequest.Request.Medic> medicsToSave = new System.Collections.Generic.List<UpdatePatientRequest.Request.Medic>();

                                        //Recorrer la lista de medicos que se estan desasociando del contacto
                                        foreach (var relatedItem in relatedEntities)
                                        {
                                            //Obtener la entidad con el Id de Medico
                                            Entity doctorToRemove = service.Retrieve(doctorEntity.EntitySingularName, relatedItem.Id, new ColumnSet(DoctorFields.DoctorIdKey));
                                            int medicsLength = updatePatientRequest.medication.medics.Length;

                                            //Buscar en la lista de medicos que tiene el usuario
                                            for (int i = 0; i < medicsLength; i++)
                                            {
                                                //Agregar a la lista de medicos a guardar, aquellos que no fueron desasociados
                                                if (updatePatientRequest.medication.medics[i].medicid != doctorToRemove.GetAttributeValue<string>(DoctorFields.DoctorIdKey))
                                                {
                                                    medicsToSave.Add(updatePatientRequest.medication.medics[i]);

                                                }
                                            }
                                        }

                                        //Enviar como null si no hay pacientes a guardar
                                        if (medicsToSave.Count == 0)
                                        {
                                            updatePatientRequest.medication.medics = null;
                                        }
                                        else
                                        {
                                            //Modificar el tamaño del array y agregar los médicos que se guardarán
                                            updatePatientRequest.medication.medics = new UpdatePatientRequest.Request.Medic[medicsToSave.Count];
                                            int length = updatePatientRequest.medication.medics.Length;
                                            for (int i = 0; i < length; i++)
                                            {
                                                updatePatientRequest.medication.medics[i] = medicsToSave[i];
                                            }
                                        }
                                    }

                                }
                                #endregion
                            }


                        }


                        #endregion


                        ///Request service POST
                        ///

                        sharedMethods = new MShared();


                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, updatePatientRequest);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        wrData.ContentType = "application/json";
                        wrData.Authorization = "Bearer "+ Constants.TokenForAboxServices;

                        wrData.Url = AboxServices.UpdatePatientService;


                        var serviceResponse = sharedMethods.DoPostRequest(wrData,trace);
                        UpdatePatientRequest.ServiceResponse serviceResponseProperties = null;
                        if (serviceResponse.IsSuccessful)
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.ServiceResponse));

                            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                            {
                                deserializer = new DataContractJsonSerializer(typeof(UpdatePatientRequest.ServiceResponse));
                                serviceResponseProperties = (UpdatePatientRequest.ServiceResponse)deserializer.ReadObject(ms);
                            }

                            if (serviceResponseProperties.response.code != "MEMCTRL-1014")
                            {
                                trace.Trace(Constants.ErrorMessageCodeReturned + serviceResponseProperties.response.code);
                                throw new InvalidPluginExecutionException(Constants.ErrorMessageTransactionCodeReturned + serviceResponseProperties.response.message);
                            }
                            else
                            {
                                //contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException(Constants.GeneralAboxServicesErrorMessage + serviceResponseProperties.response.message);
                        }

                    }









                }

                #endregion



            }
            catch (Exception ex)
            {

                throw new InvalidPluginExecutionException(ex.Message);
                //TODO: Crear Log
            }
        }
    }


}
