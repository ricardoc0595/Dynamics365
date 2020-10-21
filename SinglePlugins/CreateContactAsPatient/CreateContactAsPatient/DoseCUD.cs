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
    public class DoseCUD : IPlugin
    {
        private MShared sharedMethods = null;


        private ContactEntity contactEntity = null;
        private ProductEntity productEntity = null;
        private DoseEntity doseEntity = null;

        public void Execute(IServiceProvider serviceProvider)
        {
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                // The InputParameters collection contains all the data passed in the message request.

                

                /*Esta validación previene la ejecución del Plugin de cualquier
                * transacción realizada a través del Web API desde Abox que usa un usuario específico*/
                if (context.InitiatingUserId == new Guid("7dbf49f3-8be8-ea11-a817-002248029f77"))
                {
                    return;
                }

                Entity doseInput = null;
                Entity contact = null;
                Entity product = null;

                UpdatePatientRequest.Request updatePatientRequest = null;

                // The InputParameters collection contains all the data passed in the message request.
                if (context.InputParameters.Contains("Target") && (context.InputParameters["Target"] is Entity)|| context.InputParameters["Target"] is EntityReference)
                {
                    
                    contactEntity = new ContactEntity();
                    doseEntity = new DoseEntity();

                    //Cuando el plugin es un Delete, el inputParameter trae EntityReference, cuando es Create, trae Entity
                    if (context.InputParameters["Target"] is Entity)
                    {
                        /*Si existe una Preimagen, se obtiene toda la información de la entidad que se está actualizando de esta preimagen
                         y se cambia el valor que se está actualizando*/
                        if (context.PreEntityImages.Contains("UpdatedEntity"))
                        {
                            Entity updatedDose= (Entity)context.InputParameters["Target"];
                            doseInput = (Entity)context.PreEntityImages["UpdatedEntity"];
                            if (doseInput.Attributes.Contains(DoseFields.Dose)&& updatedDose.Attributes.Contains(DoseFields.Dose))
                            {
                                doseInput[DoseFields.Dose] = updatedDose[DoseFields.Dose];
                            }
                        }
                        else
                        {
                            doseInput = (Entity)context.InputParameters["Target"];
                        }
                       

                    }
                    else if(context.InputParameters["Target"] is EntityReference && context.PreEntityImages.Contains("DeletedEntity"))
                    {

                        doseInput = (Entity)context.PreEntityImages["DeletedEntity"];

                    }


                    if (doseInput.LogicalName != doseEntity.EntitySingularName)
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

                        if (doseInput.Attributes.Contains(DoseFields.ContactxDose))
                        {
                            EntityReference contactReference = (EntityReference)doseInput.Attributes[DoseFields.ContactxDose];
                            if (contactReference != null)
                            {
                                contactId = contactReference.Id;
                            }
                        }


                        string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country, ContactFields.UserType, ContactFields.IdType, ContactFields.Id, ContactFields.Firstname, ContactFields.SecondLastname, ContactFields.Lastname, ContactFields.Gender, ContactFields.Birthdate };
                        var columnSet = new ColumnSet(columnsToGet);

                        contact = service.Retrieve(contactEntity.EntitySingularName, contactId, columnSet);

                        if (contact != null)
                        {
                            RequestHelpers helperMethods = new RequestHelpers();

                            updatePatientRequest = helperMethods.GetPatientUpdateStructure(contact, service);

                        }



                        #endregion


                        doseEntity = new DoseEntity();
                        switch (context.MessageName.ToLower())
                        {
                            case "create":

                                #region Dose Created

                              
                                //Validar que exista la relación Dosis-Producto
                                if (doseInput.Attributes.Contains(DoseFields.DosexProduct))
                                {
                                    EntityReference productReference = null;
                                    //Se obtiene la referencia del producto que tiene la entidad Dosis
                                    productReference = (EntityReference)doseInput.Attributes[DoseFields.DosexProduct];
                                    if (productReference != null)
                                    {
                                        //Se obtiene el producto
                                        product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { ProductFields.ProductNumber }));

                                        //Se obtiene el ID del producto 
                                        if (product.Attributes.Contains(ProductFields.ProductNumber))
                                        {
                                            string frequency = "";

                                            if (doseInput.Attributes.Contains(DoseFields.Dose))
                                            {
                                                //frequency = doseInput.GetAttributeValue<string>(DoseFields.Dose);
                                                int value =(doseInput.GetAttributeValue<OptionSetValue>(DoseFields.Dose)).Value;
                                                frequency = sharedMethods.GetDoseFrequencyValue(value);

                                            }

                                            if (updatePatientRequest.medication==null)
                                            {
                                                updatePatientRequest.medication = new UpdatePatientRequest.Request.Medication();
                                            }

                                            if (updatePatientRequest.medication.products != null)
                                            {
                                                //guardar la cantidad de productos que tiene el paciente
                                                var tempProducts = updatePatientRequest.medication.products;

                                                //inicializar un nuevo array con una posición adicional que guardará el nuevo dosis-producto
                                                updatePatientRequest.medication.products = new UpdatePatientRequest.Request.Product[tempProducts.Length + 1];

                                                for (int i = 0; i < tempProducts.Length; i++)
                                                {
                                                    updatePatientRequest.medication.products[i] = new UpdatePatientRequest.Request.Product
                                                    {
                                                        frequency = tempProducts[i].frequency,
                                                        productid = tempProducts[i].productid
                                                    };
                                                }

                                                //agregar el nuevo dosis-producto al array
                                                updatePatientRequest.medication.products[updatePatientRequest.medication.products.Length - 1] = new UpdatePatientRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(ProductFields.ProductNumber)
                                                };
                                            }
                                            else
                                            {
                                                updatePatientRequest.medication.products = new UpdatePatientRequest.Request.Product[1];
                                                updatePatientRequest.medication.products[updatePatientRequest.medication.products.Length - 1] = new UpdatePatientRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(ProductFields.ProductNumber)
                                                };
                                            }


                                        }
                                    }
                                }



                                #endregion

                                break;

                            case "delete":

                                //Validar que exista la relación Dosis-Producto
                                if (doseInput.Attributes.Contains(DoseFields.EntityId))
                                {
                                    EntityReference productReference = null;
                                    productReference = (EntityReference)doseInput.Attributes[DoseFields.DosexProduct];

                                    //Se obtiene la referencia del producto que tiene la entidad Dosis
                                    product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { ProductFields.ProductNumber }));

                                    if (product != null)
                                    {
                                        //Se obtiene el ID del producto 
                                        if (product.Attributes.Contains(ProductFields.ProductNumber))
                                        {

                                            if (updatePatientRequest.medication != null)
                                            {
                                                System.Collections.Generic.List<UpdatePatientRequest.Request.Product> productsToSave = new System.Collections.Generic.List<UpdatePatientRequest.Request.Product>();

                                                for (int i = 0; i < updatePatientRequest.medication.products.Length; i++)
                                                {
                                                    /*Agregar a la lista de los productos-dosis que se enviarán al servicio todos los productos excepto el producto de la dosis que se está eliminando*/
                                                    if (updatePatientRequest.medication.products[i].productid != product.GetAttributeValue<string>(ProductFields.ProductNumber))
                                                    {
                                                        productsToSave.Add(updatePatientRequest.medication.products[i]);
                                                    }

                                                }

                                                int countProductsRelated = updatePatientRequest.medication.products.Length;

                                                //reducir el tamaño del array
                                                updatePatientRequest.medication.products = new UpdatePatientRequest.Request.Product[countProductsRelated - 1];

                                                //agregar los productos que se van a guardar.
                                                for (int i = 0; i < updatePatientRequest.medication.products.Length; i++)
                                                {
                                                    updatePatientRequest.medication.products[i] = productsToSave[i];
                                                }
                                            }



                                        }
                                    }



                                }

                                break;

                            case "update":

                                //Validar que exista la relación Dosis-Producto
                                if (doseInput.Attributes.Contains(DoseFields.DosexProduct))
                                {
                                    EntityReference productReference = null;
                                    //Se obtiene la referencia del producto que tiene la entidad Dosis
                                    productReference = (EntityReference)doseInput.Attributes[DoseFields.DosexProduct];
                                    if (productReference != null)
                                    {
                                        //Se obtiene el producto
                                        product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { ProductFields.ProductNumber }));

                                        //Se obtiene el ID del producto 
                                        if (product.Attributes.Contains(ProductFields.ProductNumber))
                                        {
                                            string frequency = "";

                                            if (doseInput.Attributes.Contains(DoseFields.Dose))
                                            {
                                                int value = (doseInput.GetAttributeValue<OptionSetValue>(DoseFields.Dose)).Value;
                                                frequency = sharedMethods.GetDoseFrequencyValue(value);
                                            }

                                            if (updatePatientRequest.medication == null)
                                            {
                                                updatePatientRequest.medication = new UpdatePatientRequest.Request.Medication();
                                            }
                                         

                                            

                                            for (int i = 0; i < updatePatientRequest.medication.products.Length; i++)
                                            {
                                                //Buscar el producto que se está actualizando para cambiarle los datos
                                                if (updatePatientRequest.medication.products[i].productid == product.GetAttributeValue<string>(ProductFields.ProductNumber))
                                                {
                                                    //actualizar la frecuencia, el producto no debe actualizarse, para esto se crea otro
                                                    updatePatientRequest.medication.products[i] = new UpdatePatientRequest.Request.Product
                                                    {
                                                        frequency = frequency,
                                                        productid = updatePatientRequest.medication.products[i].productid
                                                    };
                                                    break;
                                                }

                                            }


                                        }
                                    }
                                }

                                break;
                        }


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
                                throw new InvalidPluginExecutionException("Ocurrió un error al guardar la información en Abox Plan:\n" + serviceResponseProperties.response.message);

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





                        //TODO: Capturar excepción con servicios de Abox Plan y hacer un Logging
                    }

                }
            }
            catch (Exception ex)
            {
                trace.Trace(ex.ToString());
                throw new InvalidPluginExecutionException(ex.Message);
                //TODO: Crear Log
            }
        }
    }









}
