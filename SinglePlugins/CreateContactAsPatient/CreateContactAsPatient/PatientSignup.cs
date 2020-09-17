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
using AboxCrmPlugins.Classes.Entities;

namespace CreateContactAsPatient
{
    public class PatientSignup : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // The InputParameters collection contains all the data passed in the message request.
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

                        var request = new PatientSignupRequest.Request();
                        ContactEntity contactEntity = new ContactEntity();

                        var s = contact.RelatedEntities;
                        string ss = "";
                        return;

                        request.country = "CR";
                        request.userType = "01";
                        



                        #region Personal Info
                        if (request.personalinfo!=null)
                        {
                            request.personalinfo.idtype = "0" + contact.Attributes[contactEntity.Fields.Id].ToString();
                            request.personalinfo.id = contact.Attributes[contactEntity.Fields.Id].ToString();
                            request.personalinfo.name = contact.Attributes[contactEntity.Fields.Firstname].ToString();
                            request.personalinfo.lastname = contact.Attributes[contactEntity.Fields.Lastname].ToString();
                            request.personalinfo.secondlastname = contact.Attributes[contactEntity.Fields.SecondLastname].ToString();
                            request.personalinfo.password = contact.Attributes[contactEntity.Fields.Password].ToString();
                            request.personalinfo.gender = contact.Attributes[contactEntity.Fields.Gender].ToString();
                            DateTime birthdate = new DateTime();
                            birthdate = contact.GetAttributeValue<DateTime>(contactEntity.Fields.Birthdate);
                            if (birthdate != null)
                            {
                                request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                            }
                        }
                        
                        #endregion

                        #region ContactInfo

                        if (request.contactinfo != null)
                        {
                           
                            request.contactinfo.phone = contact.Attributes[contactEntity.Fields.Phone].ToString();
                            request.contactinfo.email = contact.Attributes[contactEntity.Fields.Email].ToString() ;
                            request.contactinfo.province = "";
                            request.contactinfo.canton = "";
                            request.contactinfo.district = "";


                        }

                        #endregion

                        #region Medication

                        if (request.medication!=null)
                        {

                            #region Products
                            
                            //contact.RelatedEntities;
                            ////contact.
                            //service.Retrieve("product", , new ColumnSet(true));
                            //contact.Attributes[contactEntity.Fields.ContactxProductRelationShip]= new EntityReference("product",);


                            #endregion


                            #region Medics

                            #endregion

                        }

                        #endregion


                        #region Interests



                        #endregion


                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QuickSignupRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, request);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        wrData.ContentType = "application/json";
                        wrData.Url = "https://apidev.aboxplan.com/member/signup/patient";
                       


                        MShared sharedMethods = new MShared();
                        var serviceResponse = sharedMethods.DoPostRequest(wrData);
                        QuickSignupRequest.ServiceResponse serviceResponseProperties = null;
                        if (serviceResponse.IsSuccessful)
                        {
                            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(QuickSignupRequest.ServiceResponse));
                            
                            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serviceResponse.Data)))
                            {
                                deserializer = new DataContractJsonSerializer(typeof(QuickSignupRequest.ServiceResponse));
                                serviceResponseProperties = (QuickSignupRequest.ServiceResponse)deserializer.ReadObject(ms);
                            }

                            if (serviceResponseProperties.response.code != "MEMEX-0002")
                            {

                                throw new InvalidPluginExecutionException("Ocurrió un error al guardar la información en Abox Plan:\n" + serviceResponseProperties.response.message);

                            }
                            else
                            {
                                contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
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
