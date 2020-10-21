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
    public class KnowYourNumberSignup : IPlugin
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

                if (contact.Attributes.Contains(ContactFields.UserType))
                {
                    EntityReference userTypeReference = null;
                    userTypeReference = (EntityReference)contact.Attributes[ContactFields.UserType];
                    if (userTypeReference != null)
                    {
                        request.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                    }
                }



                #region Personal Info
                request.personalinfo = new QuickSignupRequest.Request.Personalinfo();

                if (contact.Attributes.Contains(ContactFields.IdType))
                {
                    request.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                }



                if (contact.Attributes.Contains(ContactFields.Id))
                {
                    request.personalinfo.id = contact.Attributes[ContactFields.Id].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.Firstname))
                {
                    request.personalinfo.name = contact.Attributes[ContactFields.Firstname].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.Lastname))
                {
                    request.personalinfo.lastname = contact.Attributes[ContactFields.Lastname].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.SecondLastname))
                {
                    request.personalinfo.secondlastname = contact.Attributes[ContactFields.SecondLastname].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.Password))
                {
                    request.personalinfo.password = contact.Attributes[ContactFields.Password].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.Gender))
                {
                    int val = (contact.GetAttributeValue<OptionSetValue>(ContactFields.Gender)).Value;
                    string gender = sharedMethods.GetGenderValue(val);
                    if (!String.IsNullOrEmpty(gender))
                    {
                        request.personalinfo.gender = gender;

                    }
                }

                if (contact.Attributes.Contains(ContactFields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                    }
                }


                #endregion

                #region ContactInfo

                request.contactinfo = new QuickSignupRequest.Request.Contactinfo();


                if (contact.Attributes.Contains(ContactFields.Phone))
                {
                    request.contactinfo.phone = contact.Attributes[ContactFields.Phone].ToString();

                }

                if (contact.Attributes.Contains(ContactFields.Email))
                {
                    request.contactinfo.email = contact.Attributes[ContactFields.Email].ToString();
                }



                if (contact.Attributes.Contains(ContactFields.Country))
                {
                    EntityReference countryReference = null;
                    countryReference = (EntityReference)contact.Attributes[ContactFields.Country];
                    if (countryReference != null)
                    {

                        var countryRetrieved = service.Retrieve(countryEntity.EntitySingularName, countryReference.Id, new ColumnSet(CountryFields.IdCountry));
                        if (countryRetrieved.Attributes.Contains(CountryFields.IdCountry))
                        {

                            string country = countryRetrieved.GetAttributeValue<string>(CountryFields.IdCountry);

                            if (!String.IsNullOrEmpty(country))
                            {
                                request.country = country;

                            }

                        }
                    }
                }


                if (contact.Attributes.Contains(ContactFields.Province))
                {
                    EntityReference provinceReference = null;
                    provinceReference = (EntityReference)contact.Attributes[ContactFields.Province];
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


                if (contact.Attributes.Contains(ContactFields.Canton))
                {
                    EntityReference cantonReference = null;
                    cantonReference = (EntityReference)contact.Attributes[ContactFields.Canton];
                    if (cantonReference != null)
                    {

                        var cantonRetrieved = service.Retrieve(cantonEntity.EntitySingularName, cantonReference.Id, new ColumnSet(CantonFields.IdCanton));
                        if (cantonRetrieved.Attributes.Contains(CantonFields.IdCanton))
                        {

                            bool parsed = Int32.TryParse(cantonRetrieved.GetAttributeValue<string>(CantonFields.IdCanton), out int aux);

                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(cantonRetrieved.GetAttributeValue<string>(CantonFields.IdCanton));
                                request.contactinfo.canton = parsedValue;

                            }

                        }
                    }
                }

                if (contact.Attributes.Contains(ContactFields.District))
                {
                    EntityReference districtReference = null;
                    districtReference = (EntityReference)contact.Attributes[ContactFields.District];
                    if (districtReference != null)
                    {
                        Entity district = new Entity(districtEntity.EntitySingularName);
                        var districtRetrieved = service.Retrieve(districtEntity.EntitySingularName, districtReference.Id, new ColumnSet(DistrictFields.IdDistrict));
                        if (districtRetrieved.Attributes.Contains(DistrictFields.IdDistrict))
                        {

                            bool parsed = Int32.TryParse(districtRetrieved.GetAttributeValue<string>(DistrictFields.IdDistrict), out int aux);
                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(districtRetrieved.GetAttributeValue<string>(DistrictFields.IdDistrict));
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
                //    //contact.Attributes[ContactFields.ContactxProductRelationShip]= new EntityReference("product",);


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
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                /*Esta validación previene la ejecución del Plugin de cualquier
                * transacción realizada a través del Web API desde Abox*/
                if (context.InitiatingUserId == new Guid("7dbf49f3-8be8-ea11-a817-002248029f77"))
                {
                    return;
                }


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
                        QuickSignupRequest.Request request = this.GetSignupRequestObject(contact, service);

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


                        var serviceResponse = sharedMethods.DoPostRequest(wrData,trace);
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

                                throw new InvalidPluginExecutionException(Constants.ErrorMessageTransactionCodeReturned + serviceResponseProperties.response.message);

                            }
                            else
                            {
                                contact.Attributes.Add("new_idaboxpatient", serviceResponseProperties.response.details.idPaciente);
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

                throw new InvalidPluginExecutionException(ex.Message);
                //TODO: Crear Log
            }
        }
    }


    

}
