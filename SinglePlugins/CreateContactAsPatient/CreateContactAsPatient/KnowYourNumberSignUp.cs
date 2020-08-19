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
    public class KnowYourNumberSignup : IPlugin
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

                        var request = new QuickSignupRequest.Request();

                        request.country = "CR";
                        request.userType = "01";
                        request.personalinfo = new QuickSignupRequest.Request.Personalinfo();
                        request.personalinfo.idtype = "0" + contact.Attributes["new_idtype"].ToString();
                        request.personalinfo.id = contact.Attributes["new_id"].ToString();
                        request.personalinfo.name = contact.Attributes["firstname"].ToString();
                        request.personalinfo.lastname = contact.Attributes["lastname"].ToString();
                        request.personalinfo.secondlastname = contact.Attributes["new_secondlastname"].ToString();
                        request.personalinfo.password = contact.Attributes["new_password"].ToString();
                        request.contactinfo = new QuickSignupRequest.Request.Contactinfo();
                        request.contactinfo.phone = contact.Attributes["telephone2"].ToString();
                        request.contactinfo.mobilephone = contact.Attributes["mobilephone"].ToString();
                        request.contactinfo.email = contact.Attributes["emailaddress1"].ToString();

                        DateTime birthdate = new DateTime();
                        birthdate= contact.GetAttributeValue<DateTime>("birthdate");
                        if (birthdate!=null)
                        {
                        request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                        }



                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QuickSignupRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, request);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        wrData.ContentType = "application/json";
                        wrData.Url = "https://apidev.aboxplan.com/member/signup/kyn";
                       


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
