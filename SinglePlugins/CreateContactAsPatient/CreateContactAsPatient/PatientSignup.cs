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
                        var contactFields = new ContactEntity.Fields();
                        request.country = "CR";
                        request.userType = "01";

                        #region Personal Info
                        if (request.personalinfo!=null)
                        {
                            request.personalinfo.idtype = "0" + contact.Attributes[contactFields.IdTypeFieldName].ToString();
                            request.personalinfo.id = contact.Attributes[contactFields.IdFieldName].ToString();
                            request.personalinfo.name = contact.Attributes[contactFields.FirstnameFieldName].ToString();
                            request.personalinfo.lastname = contact.Attributes[contactFields.LastnameFieldName].ToString();
                            request.personalinfo.secondlastname = contact.Attributes[contactFields.SecondLastnameFieldName].ToString();
                            request.personalinfo.password = contact.Attributes[contactFields.PasswordFieldName].ToString();
                            request.personalinfo.gender = contact.Attributes[contactFields.GenderFieldName].ToString();
                            DateTime birthdate = new DateTime();
                            birthdate = contact.GetAttributeValue<DateTime>(contactFields.BirthdateFieldName);
                            if (birthdate != null)
                            {
                                request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                            }
                        }
                        
                        #endregion

                        #region ContactInfo

                        if (request.contactinfo != null)
                        {
                            request.contactinfo.phone = contact.Attributes[contactFields.PhoneFieldName].ToString();
                            request.contactinfo.email = contact.Attributes[contactFields.EmailFieldName].ToString() ;
                            request.contactinfo.province = "";
                            request.contactinfo.canton = "";
                            request.contactinfo.district = "";


                        }

                        #endregion

                        #region Medication

                        if (request.medication!=null)
                        {

                            #region Products



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
                                //TODO: El valor que devuelve el PatientID del servicio debe actualizar el patientID del Contacto
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
