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
    public class DoseCreate : IPlugin
    {
        private MShared sharedMethods = null;


        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoseEntity doseEntity = null;

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



                Entity doseCreated = null;
                Entity contact = null;
                Entity product = null;

                UpdatePatientRequest.Request updatePatientRequest = null;

                // The InputParameters collection contains all the data passed in the message request.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    contactEntity = new ContactEntity();
                    doseEntity = new DoseEntity();
                    doseCreated = (Entity)context.InputParameters["Target"];


                    if (doseCreated.LogicalName != doseEntity.EntitySingularName)
                    {
                        return;
                    }
                    else
                    {
                        sharedMethods = new MShared();

                        //Cast as Entity the dose being created




                        #region -> Related Contact

                        productEntity = new ProductEntity();
                        doseEntity = new DoseEntity();
                        updatePatientRequest = null;

                        Guid contactId = new Guid();

                        if (doseCreated.Attributes.Contains(doseEntity.Fields.ContactxDose))
                        {
                            EntityReference contactReference = (EntityReference)doseCreated.Attributes[doseEntity.Fields.ContactxDose];
                            if (contactReference != null)
                            {
                                contactId = contactReference.Id;
                            }
                        }


                        string[] columnsToGet = new string[] { contactEntity.Fields.IdAboxPatient, contactEntity.Fields.Country, contactEntity.Fields.UserType, contactEntity.Fields.IdType, contactEntity.Fields.Id, contactEntity.Fields.Firstname, contactEntity.Fields.SecondLastname, contactEntity.Fields.Lastname, contactEntity.Fields.Gender, contactEntity.Fields.Birthdate };
                        var columnSet = new ColumnSet(columnsToGet);

                        contact = service.Retrieve(contactEntity.EntitySingularName, contactId, columnSet);

                        if (contact != null)
                        {
                            RequestHelpers helperMethods = new RequestHelpers();
                            updatePatientRequest = helperMethods.GetPatientUpdateStructure(contact, service);

                        }



                        #endregion






                        #region Dose Created

                        doseEntity = new DoseEntity();



                        //updatePatientRequest.medication = new UpdatePatientRequest.Request.Medication();
                        //updatePatientRequest.medication.products = new UpdatePatientRequest.Request.Product[length];


                        if (doseCreated.Attributes.Contains(doseEntity.Fields.DosexProduct))
                        {
                            EntityReference productReference = null;
                            productReference = (EntityReference)doseCreated.Attributes[doseEntity.Fields.DosexProduct];
                            if (productReference != null)
                            {

                                product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { productEntity.Fields.ProductNumber }));

                                if (product.Attributes.Contains(productEntity.Fields.ProductNumber))
                                {
                                    string frequency = "";

                                    if (doseCreated.Attributes.Contains(doseEntity.Fields.Dose))
                                    {
                                        frequency = doseCreated.GetAttributeValue<string>(doseEntity.Fields.Dose);
                                    }
                                    var tempProducts = updatePatientRequest.medication.products;

                                    updatePatientRequest.medication.products = new UpdatePatientRequest.Request.Product[tempProducts.Length + 1];

                                    for (int i = 0; i < tempProducts.Length; i++)
                                    {
                                        updatePatientRequest.medication.products[i] = new UpdatePatientRequest.Request.Product
                                        {
                                            frequency = tempProducts[i].frequency,
                                            productid = tempProducts[i].productid
                                        };
                                    }



                                    updatePatientRequest.medication.products[updatePatientRequest.medication.products.Length - 1] = new UpdatePatientRequest.Request.Product
                                    {
                                        frequency = frequency,
                                        productid = product.GetAttributeValue<string>(productEntity.Fields.ProductNumber)
                                    };
                                }
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
                        wrData.Authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6InJjY3VpZDAxIiwiaWF0IjoxNjAxMDY5NTYwLCJleHAiOjE2MDExNTU5NjB9.Odlhy9XsWTcHe2aEY5j_0J6jFla3p63tAtM0mJm9eGo";

                        wrData.Url = AboxServices.UpdatePatientService;


                        var serviceResponse = sharedMethods.DoPostRequest(wrData);
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

                                throw new InvalidPluginExecutionException("Ocurrió un error al guardar la información en Abox Plan:\n" + serviceResponseProperties.response.message);

                            }
                            else
                            {
                                //contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Ocurrió un error al consultar los servicios de Abox Plan" + serviceResponseProperties.response.message);
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
