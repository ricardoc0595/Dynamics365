using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using System.Text;
using Newtonsoft.Json;
using CrmAboxApi.Logic.Classes.Deserializing;
using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;


namespace CrmAboxApi.Logic.Classes
{
    public class Contact
    {
        MShared sharedMethods = null;
        ContactEntity contactEntity = null;
        string connectionString = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Contact()
        {
            sharedMethods = new MShared();
            contactEntity = new ContactEntity();
            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        }

        private JObject GetCreateContactJsonStructure(PatientSignup signupProperties)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {
                if (signupProperties != null)
                {

                    jObject.Add($"{contactEntity.Fields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd"));
                    if (!String.IsNullOrEmpty(signupProperties.country))
                    {
                        int value = sharedMethods.GetCountryValueForOptionSet(signupProperties.country);
                        if (value > -1)
                            jObject.Add($"{contactEntity.Fields.Country}", value);
                    }

                    if (signupProperties.patientid != null)
                        jObject.Add(contactEntity.Fields.IdAboxPatient, signupProperties.patientid.ToString());

                    if (signupProperties.otherInterest != null)
                    {
                        int value;
                        bool parsed = Int32.TryParse(signupProperties.otherInterest.ToString(),out value);
                        if (parsed)
                        {
                            jObject.Add(contactEntity.Fields.OtherInterest, value);
                        }
                    }
                        


                    if (signupProperties.personalinfo != null)
                    {
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.name)))
                            jObject.Add(contactEntity.Fields.Firstname, signupProperties.personalinfo.name);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.lastname)))
                            jObject.Add(contactEntity.Fields.Lastname, signupProperties.personalinfo.lastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.secondlastname)))
                            jObject.Add(contactEntity.Fields.SecondLastname, signupProperties.personalinfo.secondlastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.password)))
                            jObject.Add(contactEntity.Fields.Password, signupProperties.personalinfo.password);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.dateofbirth)))
                            jObject.Add(contactEntity.Fields.Birthdate, signupProperties.personalinfo.dateofbirth);

                        //TODO: Max length esta en 14 actualmente, validar extension
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.id)))
                            jObject.Add(contactEntity.Fields.Id, signupProperties.personalinfo.id);

                        string idType = sharedMethods.GetIdTypeId(signupProperties.personalinfo.idtype);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.idtype)))
                            jObject.Add(contactEntity.Fields.IdType, idType);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.gender)))
                            jObject.Add(contactEntity.Fields.Gender, sharedMethods.GetGenderValue(signupProperties.personalinfo.gender));

                        if (!(String.IsNullOrEmpty(signupProperties.userType)))
                        {
                            jObject.Add($"new_UserType@odata.bind", $"/new_usertypes({sharedMethods.GetUserTypeEntityId(signupProperties.userType)})");
                        }


                    }

                    if (signupProperties.contactinfo != null)
                    {

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.email)))
                            jObject.Add(contactEntity.Fields.Email, signupProperties.contactinfo.email);

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.phone)))
                            jObject.Add(contactEntity.Fields.Phone, signupProperties.contactinfo.phone);

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.mobilephone)))
                            jObject.Add(contactEntity.Fields.SecondaryPhone, signupProperties.contactinfo.mobilephone);


                    }

                    if (signupProperties.medication != null)
                    {
                        JArray productsArray = new JArray();
                        JArray medicsArray = new JArray();
                        ProductEntity productEntity = new ProductEntity();
                        DoctorEntity doctorEntity = new DoctorEntity();


                        int productsLength = signupProperties.medication.products.Length;
                        for (int i = 0; i < productsLength; i++)
                        {
                            productsArray.Add(new JValue($"/{productEntity.EntityName}({productEntity.Fields.ProductNumber}='{signupProperties.medication.products[i].productid}')"));
                        }



                        int medicsLength = signupProperties.medication.medics.Length;
                        for (int i = 0; i < medicsLength; i++)
                        {
                            medicsArray.Add(new JValue($"/{doctorEntity.EntityName}({doctorEntity.Fields.DoctorIdKey}='{signupProperties.medication.medics[i].medicid}')"));
                        }


                        if (productsArray != null)
                        {
                            jObject.Add($"{contactEntity.Fields.ContactxProductRelationship}@odata.bind", productsArray);
                        }

                        if (medicsArray != null)
                        {
                            jObject.Add($"{contactEntity.Fields.ContactxDoctorRelationship}@odata.bind", medicsArray);
                        }
                    }


                }

                return jObject;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                jObject = null;
                return jObject;
            }


        }

        public OperationResult CreateAsPatient(PatientSignup signupRequest, string idRelatedPatient)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                PatientSignup signupProperties = signupRequest;
                var newContact = this.GetCreateContactJsonStructure(signupProperties);

                if (!String.IsNullOrEmpty(idRelatedPatient))
                {
                    JArray patientsInChargeArray = new JArray();
                    patientsInChargeArray.Add(new JValue($"/{contactEntity.EntityName}({idRelatedPatient})"));
                    newContact.Add($"{contactEntity.Fields.ContactxContactRelationship}@odata.bind", patientsInChargeArray);
                }

                responseObject = this.ContactCreateRequest(newContact);
                return responseObject;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }


        private OperationResult ContactCreateRequest(JObject jsonObject)
        {
            OperationResult operationResult = new OperationResult();
            try
            {

                if (jsonObject != null)
                {
                    try
                    {
                        using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                        {


                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PostAsync($"contacts?$select={contactEntity.Fields.EntityId}", c).Result;
                            if (response.IsSuccessStatusCode)
                            {

                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[contactEntity.Fields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Contacto creado correctamente en el CRM";
                                operationResult.IsSuccessful = true;
                                operationResult.Data = userId;

                            }
                            else
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                                JObject userId = JObject.Parse(body.ToString());
                                CrmWebAPIError err = userId.ToObject<CrmWebAPIError>();

                                Logger.Error("", response.RequestMessage);
                                operationResult.Code = "Error al crear el contacto en el CRM";
                                operationResult.Message = response.ReasonPhrase;
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                        operationResult.Code = "";
                        operationResult.Message = ex.ToString();
                        operationResult.IsSuccessful = false;
                        operationResult.Data = null;

                    }
                }



                return operationResult;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }

        }

        public OperationResult CreateAsCaretaker(PatientSignup signupRequest)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                PatientSignup signupPropertiesFromRequest = signupRequest;
                ContactEntity contactEntity = new ContactEntity();
                ProductEntity productEntity = new ProductEntity();
                DoctorEntity doctorEntity = new DoctorEntity();
                JArray productsArray = new JArray();
                JArray medicsArray = new JArray();
                if (signupRequest != null)
                {


                    OperationResult responseFromPatientInChargeCreate = null;
                    OperationResult responseFromOwnerCreate = null;

                    /*Crear primeramente el paciente a cargo, para así continuar con el encargado pues ya se tiene el ID del paciente creado en CRM
                    para poder relacionarlo.*/

                    #region -> Crear paciente bajo cuido como contacto
                    PatientSignup signupPatientUnderCare = null;
                    signupPatientUnderCare = new PatientSignup
                    {
                        country = signupPropertiesFromRequest.country,
                        userType = "01",
                        medication = signupPropertiesFromRequest.medication,
                        contactinfo = new PatientSignup.Contactinfo
                        {
                            email = signupPropertiesFromRequest.contactinfo.email,
                            canton = signupPropertiesFromRequest.contactinfo.canton,
                            district = signupPropertiesFromRequest.contactinfo.district,
                            phone = signupPropertiesFromRequest.contactinfo.phone,
                            mobilephone = signupPropertiesFromRequest.contactinfo.mobilephone,
                            password = "",
                            province = signupPropertiesFromRequest.contactinfo.province,
                            address = signupPropertiesFromRequest.contactinfo.address
                        },
                        patientincharge = new PatientSignup.Patientincharge
                        {
                            lastname = signupPropertiesFromRequest.patientincharge.lastname,
                            dateofbirth = signupPropertiesFromRequest.patientincharge.dateofbirth,
                            gender = signupPropertiesFromRequest.patientincharge.gender,
                            id = signupPropertiesFromRequest.patientincharge.id,
                            idtype = signupPropertiesFromRequest.patientincharge.idtype,
                            name = signupPropertiesFromRequest.patientincharge.name,
                            secondlastname = signupPropertiesFromRequest.patientincharge.secondlastname
                        },
                        personalinfo = new PatientSignup.Personalinfo
                        {
                            lastname = signupPropertiesFromRequest.patientincharge.lastname,
                            dateofbirth = signupPropertiesFromRequest.patientincharge.dateofbirth,
                            gender = signupPropertiesFromRequest.patientincharge.gender,
                            id = signupPropertiesFromRequest.patientincharge.id,
                            idtype = signupPropertiesFromRequest.patientincharge.idtype,
                            name = signupPropertiesFromRequest.patientincharge.name,
                            secondlastname = signupPropertiesFromRequest.patientincharge.secondlastname,
                            password = ""
                        },
                        interests = null,
                        otherInterest = null,


                    };

                    responseFromPatientInChargeCreate = this.CreateAsPatient(signupPatientUnderCare, null);

                    #endregion


                    /*Se crea el paciente a cargo correctamente, ya existe en Dynamics y se procede a crear el dueño de la cuenta
                     *para poder relacionarlo.*/

                    #region -> Crear Dueño de la cuenta como Contacto




                    PatientSignup signupPatientOwner = null;
                    if (responseFromPatientInChargeCreate.IsSuccessful)
                    {
                        string idUnderCarePatient = responseFromPatientInChargeCreate.Data.ToString();
                        signupPatientOwner = signupPropertiesFromRequest;
                        signupPatientOwner.medication = null;
                        signupPatientOwner.patientincharge = null;

                        responseFromOwnerCreate = this.CreateAsPatient(signupPatientOwner, idUnderCarePatient);

                        if (responseFromOwnerCreate.IsSuccessful)
                        {
                            responseObject.IsSuccessful = true;
                            responseObject.Message = "Registro completado correctamente";
                            responseObject.Code = "";

                        }
                        else
                        {
                            //TODO: Eliminar al contacto que se creo del paciente a cargo o quedara huerfano
                            responseObject.IsSuccessful = false;
                            responseObject.Message = "Ocurrió un error al crear el dueño de la cuenta";
                            responseObject.InternalError = responseFromPatientInChargeCreate.InternalError;
                            responseObject.Code = "";
                        }

                    }
                    else
                    {

                        responseObject.IsSuccessful = false;
                        responseObject.Message = "Ocurrió un error al crear el paciente a cargo";
                        responseObject.InternalError = responseFromPatientInChargeCreate.InternalError;
                        responseObject.Code = "";

                    }
                    #endregion


                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                responseObject.IsSuccessful = false;
                responseObject.Message = ex.Message;
                responseObject.Code = "";
            }

            return responseObject;
        }


        public OperationResult CreateAsTutor(PatientSignup signupRequest)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                PatientSignup signupPropertiesFromRequest = signupRequest;
                ContactEntity contactEntity = new ContactEntity();
                ProductEntity productEntity = new ProductEntity();
                DoctorEntity doctorEntity = new DoctorEntity();
                JArray productsArray = new JArray();
                JArray medicsArray = new JArray();
                if (signupRequest != null)
                {


                    OperationResult responseFromPatientInChargeCreate = null;
                    OperationResult responseFromOwnerCreate = null;

                    /*Crear primeramente el paciente a cargo, para así continuar con el encargado pues ya se tiene el ID del paciente creado en CRM
                    para poder relacionarlo.*/

                    #region -> Crear paciente bajo cuido como contacto
                    PatientSignup signupPatientUnderCare = null;
                    signupPatientUnderCare = new PatientSignup
                    {
                        country = signupPropertiesFromRequest.country,
                        userType = "01",
                        medication = signupPropertiesFromRequest.medication,
                        contactinfo = new PatientSignup.Contactinfo
                        {
                            email = signupPropertiesFromRequest.contactinfo.email,
                            canton = signupPropertiesFromRequest.contactinfo.canton,
                            district = signupPropertiesFromRequest.contactinfo.district,
                            phone = signupPropertiesFromRequest.contactinfo.phone,
                            mobilephone = signupPropertiesFromRequest.contactinfo.mobilephone,
                            password = "",
                            province = signupPropertiesFromRequest.contactinfo.province,
                            address = signupPropertiesFromRequest.contactinfo.address
                        },
                        patientincharge = new PatientSignup.Patientincharge
                        {
                            lastname = signupPropertiesFromRequest.patientincharge.lastname,
                            dateofbirth = signupPropertiesFromRequest.patientincharge.dateofbirth,
                            gender = signupPropertiesFromRequest.patientincharge.gender,
                            id = signupPropertiesFromRequest.patientincharge.id,
                            idtype = signupPropertiesFromRequest.patientincharge.idtype,
                            name = signupPropertiesFromRequest.patientincharge.name,
                            secondlastname = signupPropertiesFromRequest.patientincharge.secondlastname
                        },
                        personalinfo = new PatientSignup.Personalinfo
                        {
                            lastname = signupPropertiesFromRequest.patientincharge.lastname,
                            dateofbirth = signupPropertiesFromRequest.patientincharge.dateofbirth,
                            gender = signupPropertiesFromRequest.patientincharge.gender,
                            id = signupPropertiesFromRequest.patientincharge.id,
                            idtype = signupPropertiesFromRequest.patientincharge.idtype,
                            name = signupPropertiesFromRequest.patientincharge.name,
                            secondlastname = signupPropertiesFromRequest.patientincharge.secondlastname,
                            password = ""
                        },
                        interests = null,
                        otherInterest = null,


                    };

                    responseFromPatientInChargeCreate = this.CreateAsPatient(signupPatientUnderCare, null);

                    #endregion


                    /*Se crea el paciente a cargo correctamente, ya existe en Dynamics y se procede a crear el dueño de la cuenta
                     *para poder relacionarlo.*/

                    #region -> Crear Dueño de la cuenta como Contacto




                    PatientSignup signupPatientOwner = null;
                    if (responseFromPatientInChargeCreate.IsSuccessful)
                    {
                        string idUnderCarePatient = responseFromPatientInChargeCreate.Data.ToString();
                        signupPatientOwner = signupPropertiesFromRequest;
                        signupPatientOwner.medication = null;
                        signupPatientOwner.patientincharge = null;

                        responseFromOwnerCreate = this.CreateAsPatient(signupPatientOwner, idUnderCarePatient);

                        if (responseFromOwnerCreate.IsSuccessful)
                        {
                            responseObject.IsSuccessful = true;
                            responseObject.Message = "Registro completado correctamente";
                            responseObject.Code = "";

                        }
                        else
                        {
                            //TODO: Eliminar al contacto que se creo del paciente a cargo o quedara huerfano
                            responseObject.IsSuccessful = false;
                            responseObject.Message = "Ocurrió un error al crear el dueño de la cuenta";
                            responseObject.InternalError = responseFromPatientInChargeCreate.InternalError;
                            responseObject.Code = "";
                        }

                    }
                    else
                    {

                        responseObject.IsSuccessful = false;
                        responseObject.Message = "Ocurrió un error al crear el paciente a cargo";
                        responseObject.InternalError = responseFromPatientInChargeCreate.InternalError;
                        responseObject.Code = "";

                    }
                    #endregion


                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                responseObject.IsSuccessful = false;
                responseObject.Message = ex.Message;
                responseObject.Code = "";
            }

            return responseObject;
        }


        public OperationResult UpdateAccount()
        {
            try
            {

            throw new NotImplementedException("");
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}