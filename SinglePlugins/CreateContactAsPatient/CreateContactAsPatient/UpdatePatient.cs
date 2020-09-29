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

                            #region -> Set request data based on Contact

                            updatePatientRequest = helperMethods.GetPatientUpdateStructure(contactUpdated,service);

                            #endregion


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
