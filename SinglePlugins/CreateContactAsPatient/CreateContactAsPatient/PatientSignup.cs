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

namespace CreateContactAsPatient
{
    public class PatientSignup : IPlugin
    {
        private MShared sharedMethods = null;
        private QuickSignupRequest.Request GetSignupRequestObject(Entity contact, IOrganizationService service)
        {
                var request = new QuickSignupRequest.Request();
            try
            {
                ContactEntity contactEntity = new ContactEntity();
                CantonEntity cantonEntity = new CantonEntity();
                DistrictEntity districtEntity = new DistrictEntity();
                ProvinceEntity provinceEntity = new ProvinceEntity();
                CountryEntity countryEntity = new CountryEntity();

                if (contact.Attributes.Contains(contactEntity.Fields.UserType))
                {
                    EntityReference userTypeReference = null;
                    userTypeReference = (EntityReference)contact.Attributes[contactEntity.Fields.UserType];
                    if (userTypeReference != null)
                    {
                        request.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                    }
                }



                #region Personal Info
                request.personalinfo = new QuickSignupRequest.Request.Personalinfo();

                if (contact.Attributes.Contains(contactEntity.Fields.IdType))
                {
                    request.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(contactEntity.Fields.IdType)).Value;
                }



                if (contact.Attributes.Contains(contactEntity.Fields.Id))
                {
                    request.personalinfo.id = contact.Attributes[contactEntity.Fields.Id].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.Firstname))
                {
                    request.personalinfo.name = contact.Attributes[contactEntity.Fields.Firstname].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.Lastname))
                {
                    request.personalinfo.lastname = contact.Attributes[contactEntity.Fields.Lastname].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.SecondLastname))
                {
                    request.personalinfo.secondlastname = contact.Attributes[contactEntity.Fields.SecondLastname].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.Password))
                {
                    request.personalinfo.password = contact.Attributes[contactEntity.Fields.Password].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.Gender))
                {
                    int val = (contact.GetAttributeValue<OptionSetValue>(contactEntity.Fields.Gender)).Value;
                    string gender = sharedMethods.GetGenderValue(val);
                    if (!String.IsNullOrEmpty(gender))
                    {
                        request.personalinfo.gender = gender;

                    }
                }

                if (contact.Attributes.Contains(contactEntity.Fields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(contactEntity.Fields.Birthdate);
                    if (birthdate != null)
                    {
                        request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                    }
                }


                #endregion

                #region ContactInfo

                request.contactinfo = new QuickSignupRequest.Request.Contactinfo();


                if (contact.Attributes.Contains(contactEntity.Fields.Phone))
                {
                    request.contactinfo.phone = contact.Attributes[contactEntity.Fields.Phone].ToString();

                }

                if (contact.Attributes.Contains(contactEntity.Fields.Email))
                {
                    request.contactinfo.email = contact.Attributes[contactEntity.Fields.Email].ToString();
                }



                if (contact.Attributes.Contains(contactEntity.Fields.Country))
                {
                    EntityReference countryReference = null;
                    countryReference = (EntityReference)contact.Attributes[contactEntity.Fields.Country];
                    if (countryReference != null)
                    {

                        var countryRetrieved = service.Retrieve(countryEntity.EntitySingularName, countryReference.Id, new ColumnSet(countryEntity.Fields.IdCountry));
                        if (countryRetrieved.Attributes.Contains(countryEntity.Fields.IdCountry))
                        {

                            string country = countryRetrieved.GetAttributeValue<string>(countryEntity.Fields.IdCountry);

                            if (!String.IsNullOrEmpty(country))
                            {
                                request.country = country;

                            }

                        }
                    }
                }


                if (contact.Attributes.Contains(contactEntity.Fields.Province))
                {
                    EntityReference provinceReference = null;
                    provinceReference = (EntityReference)contact.Attributes[contactEntity.Fields.Province];
                    if (provinceReference != null)
                    {

                        var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(provinceEntity.Fields.IdProvince));
                        if (provinceRetrieved.Attributes.Contains(provinceEntity.Fields.IdProvince))
                        {

                            bool parsed = Int32.TryParse(provinceRetrieved.GetAttributeValue<string>(provinceEntity.Fields.IdProvince), out int aux);

                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(provinceRetrieved.GetAttributeValue<string>(provinceEntity.Fields.IdProvince));
                                request.contactinfo.province = parsedValue;

                            }

                        }
                    }
                }


                if (contact.Attributes.Contains(contactEntity.Fields.Canton))
                {
                    EntityReference cantonReference = null;
                    cantonReference = (EntityReference)contact.Attributes[contactEntity.Fields.Canton];
                    if (cantonReference != null)
                    {

                        var cantonRetrieved = service.Retrieve(cantonEntity.EntitySingularName, cantonReference.Id, new ColumnSet(cantonEntity.Fields.IdCanton));
                        if (cantonRetrieved.Attributes.Contains(cantonEntity.Fields.IdCanton))
                        {

                            bool parsed = Int32.TryParse(cantonRetrieved.GetAttributeValue<string>(cantonEntity.Fields.IdCanton), out int aux);

                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(cantonRetrieved.GetAttributeValue<string>(cantonEntity.Fields.IdCanton));
                                request.contactinfo.canton = parsedValue;

                            }

                        }
                    }
                }

                if (contact.Attributes.Contains(contactEntity.Fields.District))
                {
                    EntityReference districtReference = null;
                    districtReference = (EntityReference)contact.Attributes[contactEntity.Fields.District];
                    if (districtReference != null)
                    {
                        Entity district = new Entity(districtEntity.EntitySingularName);
                        var districtRetrieved = service.Retrieve(districtEntity.EntitySingularName, districtReference.Id, new ColumnSet(districtEntity.Fields.IdDistrict));
                        if (districtRetrieved.Attributes.Contains(districtEntity.Fields.IdDistrict))
                        {

                            bool parsed = Int32.TryParse(districtRetrieved.GetAttributeValue<string>(districtEntity.Fields.IdDistrict), out int aux);
                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(districtRetrieved.GetAttributeValue<string>(districtEntity.Fields.IdDistrict));
                                request.contactinfo.district = parsedValue;
                            }


                        }
                    }
                }




                #endregion

                #region Medication

                //if (request.medication!=null)
                //{

                //    #region Products

                //    //contact.RelatedEntities;
                //    ////contact.
                //    //service.Retrieve("product", , new ColumnSet(true));
                //    //contact.Attributes[contactEntity.Fields.ContactxProductRelationShip]= new EntityReference("product",);


                //    #endregion


                //    #region Medics

                //    #endregion

                //}

                #endregion

                #region Interests



                #endregion


                return request;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
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

                        sharedMethods = new MShared();
                        QuickSignupRequest.Request request = this.GetSignupRequestObject(contact,service);

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QuickSignupRequest.Request));
                        MemoryStream memoryStream = new MemoryStream();
                        serializer.WriteObject(memoryStream, request);
                        var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());
                        memoryStream.Dispose();

                        //Valores necesarios para hacer el Post Request
                        WebRequestData wrData = new WebRequestData();
                        wrData.InputData = jsonObject;
                        wrData.ContentType = "application/json";
                        
                        
                        wrData.Url = AboxServices.QuickSignupService;
                       
                        
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
