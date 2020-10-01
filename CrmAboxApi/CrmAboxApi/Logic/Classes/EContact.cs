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
//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using AboxDynamicsBase.Classes.Entities;


namespace CrmAboxApi.Logic.Classes
{
    public class EContact : ContactEntity
    {
        MShared sharedMethods = null;
        
        CountryEntity countryEntity = null;
        ProvinceEntity provinceEntity = null;
        CantonEntity cantonEntity = null;
        DistrictEntity districtEntity = null;
        EDose doseEntity = null;
        string connectionString = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public EContact()
        {
            sharedMethods = new MShared();
            
            countryEntity = new CountryEntity();
            provinceEntity = new ProvinceEntity();
            cantonEntity = new CantonEntity();
            districtEntity = new DistrictEntity();
            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            doseEntity = new EDose();
        }

        private JObject GetCreateContactJsonStructure(PatientSignup signupProperties)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {
                if (signupProperties != null)
                {

                    jObject.Add($"{this.Fields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd"));


                    if (!(String.IsNullOrEmpty(signupProperties.country)))
                    {
                        jObject.Add($"{this.Schemas.Country}@odata.bind", $"/{countryEntity.EntityPluralName}({countryEntity.Fields.IdCountry}='{signupProperties.country}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.province)))
                    {
                        jObject.Add($"{this.Schemas.Province}@odata.bind", $"/{provinceEntity.EntityPluralName}({provinceEntity.Fields.IdProvince}='{signupProperties.contactinfo.province}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.canton)))
                    {
                        jObject.Add($"{this.Schemas.Canton}@odata.bind", $"/{cantonEntity.EntityPluralName}({cantonEntity.Fields.IdCanton}='{signupProperties.contactinfo.canton}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.district)))
                    {
                        jObject.Add($"{this.Schemas.District}@odata.bind", $"/{districtEntity.EntityPluralName}({districtEntity.Fields.IdDistrict}='{signupProperties.contactinfo.district}')");
                    }


                    if (signupProperties.patientid != null)
                        jObject.Add(this.Fields.IdAboxPatient, signupProperties.patientid.ToString());

                    if (!String.IsNullOrEmpty(signupProperties.otherInterest))
                    {
                        
                        bool parsed = Int32.TryParse(signupProperties.otherInterest.ToString(), out int aux);
                        if (parsed)
                        {
                            int value = Int32.Parse(signupProperties.otherInterest.ToString());
                            jObject.Add(this.Fields.OtherInterest, value);
                        }
                    }



                    if (signupProperties.personalinfo != null)
                    {
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.name)))
                            jObject.Add(this.Fields.Firstname, signupProperties.personalinfo.name);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.lastname)))
                            jObject.Add(this.Fields.Lastname, signupProperties.personalinfo.lastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.secondlastname)))
                            jObject.Add(this.Fields.SecondLastname, signupProperties.personalinfo.secondlastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.password)))
                            jObject.Add(this.Fields.Password, signupProperties.personalinfo.password);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.dateofbirth)))
                            jObject.Add(this.Fields.Birthdate, signupProperties.personalinfo.dateofbirth);

                        //TODO: Max length esta en 14 actualmente, validar extension
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.id)))
                            jObject.Add(this.Fields.Id, signupProperties.personalinfo.id);

                        string idType = sharedMethods.GetIdTypeId(signupProperties.personalinfo.idtype);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.idtype)))
                            jObject.Add(this.Fields.IdType, idType);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.gender)))
                            jObject.Add(this.Fields.Gender, sharedMethods.GetGenderValue(signupProperties.personalinfo.gender));

                        if (!(String.IsNullOrEmpty(signupProperties.userType)))
                        {
                            jObject.Add($"{this.Schemas.UserType}@odata.bind", $"/new_usertypes({sharedMethods.GetUserTypeEntityId(signupProperties.userType)})");
                        }


                    }

                    if (signupProperties.contactinfo != null)
                    {

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.email)))
                            jObject.Add(this.Fields.Email, signupProperties.contactinfo.email);

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.phone)))
                            jObject.Add(this.Fields.Phone, signupProperties.contactinfo.phone);

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.mobilephone)))
                            jObject.Add(this.Fields.SecondaryPhone, signupProperties.contactinfo.mobilephone);


                    }

                    if (signupProperties.medication != null)
                    {
                        //JArray dosesArray = new JArray();
                        //JArray productsArray = new JArray();
                        JArray medicsArray = new JArray();
                       
                        DoctorEntity doctorEntity = new DoctorEntity();


                        //int productsLength = signupProperties.medication.products.Length;
                        //for (int i = 0; i < productsLength; i++)
                        //{
                        //    productsArray.Add(new JValue($"/{productEntity.EntityPluralName}({productEntity.Fields.ProductNumber}='{signupProperties.medication.products[i].productid}')"));
                        //}





                        int medicsLength = signupProperties.medication.medics.Length;
                        for (int i = 0; i < medicsLength; i++)
                        {
                            medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({doctorEntity.Fields.DoctorIdKey}='{signupProperties.medication.medics[i].medicid}')"));
                        }


                        //if (productsArray != null)
                        //{
                        //    jObject.Add($"{this.Fields.ContactxProductRelationship}@odata.bind", productsArray);
                        //}

                        if (medicsArray != null)
                        {
                            jObject.Add($"{this.Fields.ContactxDoctorRelationship}@odata.bind", medicsArray);
                        }

                    }

                    if (signupProperties.interests != null)
                    {
                        string values = "";
                        int length = signupProperties.interests.Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (values != "")
                                values += "," + signupProperties.interests[i].interestid;
                            else
                                values += signupProperties.interests[i].interestid;
                        }
                        jObject.Add($"{this.Fields.Interests}", values);

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


        private JObject GetUpdateContactJsonStructure(UpdateAccountRequest updateProperties)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {
                if (updateProperties != null)
                {

                    jObject.Add($"{this.Fields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd"));



                    if (!(String.IsNullOrEmpty(updateProperties.Nombre)))
                        jObject.Add(this.Fields.Firstname, updateProperties.Nombre);

                    if (!(String.IsNullOrEmpty(updateProperties.Apellido1)))
                        jObject.Add(this.Fields.Lastname, updateProperties.Apellido1);

                    if (!(String.IsNullOrEmpty(updateProperties.Apellido2)))
                        jObject.Add(this.Fields.SecondLastname, updateProperties.Apellido2);



                    if (!(String.IsNullOrEmpty(updateProperties.FechaNacimiento)))
                        jObject.Add(this.Fields.Birthdate, updateProperties.FechaNacimiento);



                    if (!(String.IsNullOrEmpty(updateProperties.Genero)))
                        jObject.Add(this.Fields.Gender, sharedMethods.GetGenderValue(updateProperties.Genero));


                    if (!(String.IsNullOrEmpty(updateProperties.Telefono)))
                        jObject.Add(this.Fields.Phone, updateProperties.Telefono);

                    if (!(String.IsNullOrEmpty(updateProperties.Telefono2)))
                        jObject.Add(this.Fields.SecondaryPhone, updateProperties.Telefono2);

                    if (updateProperties.medication != null)
                    {
                        JArray productsArray = new JArray();
                        JArray medicsArray = new JArray();
                        ProductEntity productEntity = new ProductEntity();
                        DoctorEntity doctorEntity = new DoctorEntity();


                        int productsLength = updateProperties.medication.products.Length;
                        for (int i = 0; i < productsLength; i++)
                        {
                            productsArray.Add(new JValue($"/{productEntity.EntityPluralName}({productEntity.Fields.ProductNumber}='{updateProperties.medication.products[i].productid}')"));
                        }



                        int medicsLength = updateProperties.medication.medics.Length;
                        for (int i = 0; i < medicsLength; i++)
                        {
                            medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({doctorEntity.Fields.DoctorIdKey}='{updateProperties.medication.medics[i].medicid}')"));
                        }


                        if (productsArray != null && productsArray.Count > 0)
                        {
                            jObject.Add($"{this.Fields.ContactxProductRelationship}@odata.bind", productsArray);
                        }

                        if (medicsArray != null && medicsArray.Count > 0)
                        {
                            jObject.Add($"{this.Fields.ContactxDoctorRelationship}@odata.bind", medicsArray);
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


        private JObject GetUpdatePatientJsonStructure(UpdatePatientRequest updateProperties)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {

                if (updateProperties != null)
                {


                    if (updateProperties.personalinfo != null)
                    {
                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.name)))
                            jObject.Add(this.Fields.Firstname, updateProperties.personalinfo.name);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.lastname)))
                            jObject.Add(this.Fields.Lastname, updateProperties.personalinfo.lastname);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.secondlastname)))
                            jObject.Add(this.Fields.SecondLastname, updateProperties.personalinfo.secondlastname);


                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.dateofbirth)))
                            jObject.Add(this.Fields.Birthdate, updateProperties.personalinfo.dateofbirth);




                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.gender)))
                            jObject.Add(this.Fields.Gender, sharedMethods.GetGenderValue(updateProperties.personalinfo.gender));



                    }


                    if (updateProperties.medication != null)
                    {
                        JArray productsArray = new JArray();
                        JArray medicsArray = new JArray();
                        ProductEntity productEntity = new ProductEntity();
                        DoctorEntity doctorEntity = new DoctorEntity();


                        int productsLength = updateProperties.medication.products.Length;
                        for (int i = 0; i < productsLength; i++)
                        {
                            productsArray.Add(new JValue($"/{productEntity.EntityPluralName}({productEntity.Fields.ProductNumber}='{updateProperties.medication.products[i].productid}')"));
                        }



                        int medicsLength = updateProperties.medication.medics.Length;
                        for (int i = 0; i < medicsLength; i++)
                        {
                            medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({doctorEntity.Fields.DoctorIdKey}='{updateProperties.medication.medics[i].medicid}')"));
                        }


                        if (productsArray != null)
                        {
                            jObject.Add($"{this.Fields.ContactxProductRelationship}@odata.bind", productsArray);
                        }

                        if (medicsArray != null)
                        {
                            jObject.Add($"{this.Fields.ContactxDoctorRelationship}@odata.bind", medicsArray);
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

                if (signupProperties != null)
                {
                    DoseRecord[] dosesArray = null;
                    string[] dosesCreated = null;


                    /*Este request se deja por fuera del metodo que crea toda la estructura de
                     * Contacto porque es un proceso individual de crear una entidad de Dosis,
                     * la cual puede eventualmente fallar o no crearse correctamente, ademas se necesita
                     * ligar el resultado de esta operacion al request que crea el contacto en el crm                     
                     */
                    #region -> Dose Retrieve

                    if (signupProperties.medication != null)
                    {
                        int dosesLength = signupProperties.medication.products.Length;
                        dosesArray = new DoseRecord[dosesLength];
                        dosesCreated = new string[dosesLength];

                        for (int i = 0; i < dosesLength; i++)
                        {

                            string frequency = "";
                            if (!String.IsNullOrEmpty(signupProperties.medication.products[i].other))
                                frequency = signupProperties.medication.products[i].other;
                            else
                                frequency = signupProperties.medication.products[i].frequency;

                            dosesArray[i] = new DoseRecord { Dose = frequency, IdProduct = signupProperties.medication.products[i].productid };
                        }


                        if (dosesArray != null)
                        {
                            if (dosesArray.Length > 0)
                            {

                                try
                                {
                                    int length = dosesArray.Length;

                                    for (int i = 0; i < length; i++)
                                    {
                                        OperationResult result = doseEntity.Create(new DoseRecord
                                        {
                                            Dose = dosesArray[i].Dose,
                                            IdProduct = dosesArray[i].IdProduct
                                        });

                                        if (result.IsSuccessful)
                                        {
                                            dosesCreated[i] = (string)result.Data;

                                        }

                                    }

                                    if (dosesCreated.Length != dosesArray.Length)
                                    {
                                        throw (new Exception("Ha ocurrido un error creando las dosis del paciente " + signupProperties.personalinfo.id + " en el CRM"));
                                    }


                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex.ToString());
                                    throw ex;
                                }

                            }

                        }
                    }

                    #endregion

                    JObject newContact = this.GetCreateContactJsonStructure(signupProperties);


                    #region -> Patient Related

                    if (!String.IsNullOrEmpty(idRelatedPatient))
                    {
                        JArray patientsInChargeArray = new JArray();
                        patientsInChargeArray.Add(new JValue($"/{this.EntityPluralName}({idRelatedPatient})"));
                        newContact.Add($"{this.Fields.ContactxContactRelationship}@odata.bind", patientsInChargeArray);
                    }

                    #endregion

                    #region -> Doses Related

                    if (dosesArray != null && dosesCreated != null)
                    {
                        if (dosesCreated.Length == dosesArray.Length)
                        {
                            JArray dosesToSave = new JArray();
                            int length = dosesCreated.Length;
                            for (int i = 0; i < length; i++)
                            {
                                dosesToSave.Add(new JValue($"/{doseEntity.EntityPluralName}({dosesCreated[i]})"));

                            }

                            newContact.Add($"{this.Fields.ContactxDoseRelationship}@odata.bind", dosesToSave);

                        }
                    }

                    #endregion

                    responseObject = this.ContactCreateRequest(newContact);

                }

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
                            var response = client.PostAsync($"contacts?$select={this.Fields.EntityId}", c).Result;
                            if (response.IsSuccessStatusCode)
                            {

                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[this.Fields.EntityId];
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



        private OperationResult ContactUpdateRequest(JObject jsonObject, string idToUpdate)
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


                            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            //client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PatchAsync($"contacts({this.Fields.IdAboxPatient}={idToUpdate})", c).Result;
                            if (response.IsSuccessStatusCode)
                            {

                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //string userId = (string)body[this.Fields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Contacto actualizado correctamente en el CRM";
                                operationResult.IsSuccessful = true;
                                operationResult.Data = null;

                            }
                            else
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                                JObject userId = JObject.Parse(body.ToString());
                                CrmWebAPIError err = userId.ToObject<CrmWebAPIError>();

                                Logger.Error("", response.RequestMessage);
                                operationResult.Code = "Error al actualizar el contacto en el CRM";
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

                PatientSignup signupPropertiesFromRequest = signupRequest;



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



        public OperationResult UpdateAccount(UpdateAccountRequest updateAccountRequest)
        {
            OperationResult result = null;
            try
            {

                if (updateAccountRequest != null)
                {

                    var contactStructure = this.GetUpdateContactJsonStructure(updateAccountRequest);

                    result = this.ContactUpdateRequest(contactStructure, updateAccountRequest.patientId);




                }
                else
                {
                    result.IsSuccessful = false;
                    result.Message = "Datos de consulta incorrectos";
                    result.InternalError = null;
                    result.Code = "";

                }

                return result;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                result.IsSuccessful = false;
                result.Message = ex.ToString();
                result.InternalError = null;
                result.Code = "";
                return result;

            }
        }

        public OperationResult UpdatePatient(UpdatePatientRequest updatePatientRequest)
        {
            OperationResult result = null;
            try
            {

                if (updatePatientRequest != null)
                {

                    var contactStructure = this.GetUpdatePatientJsonStructure(updatePatientRequest);

                    result = this.ContactUpdateRequest(contactStructure, updatePatientRequest.patientid);

                }
                else
                {
                    result.IsSuccessful = false;
                    result.Message = "Datos de consulta incorrectos";
                    result.InternalError = null;
                    result.Code = "";

                }

                return result;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                result.IsSuccessful = false;
                result.Message = ex.ToString();
                result.InternalError = null;
                result.Code = "";
                return result;

            }
        }
    }
}