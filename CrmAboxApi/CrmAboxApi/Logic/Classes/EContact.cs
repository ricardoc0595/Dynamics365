using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using CrmAboxApi.Logic.Classes.Deserializing;
using CrmAboxApi.Logic.Classes.Helper;

//using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;
using Logic.CrmAboxApi.Classes.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;


namespace CrmAboxApi.Logic.Classes
{
    public class EContact : ContactEntity
    {
        private MShared sharedMethods = null;

        private CountryEntity countryEntity = null;
        private ProvinceEntity provinceEntity = null;
        private CantonEntity cantonEntity = null;
        private DistrictEntity districtEntity = null;
        private DoctorEntity doctorEntity = null;
        private EDose doseEntity = null;
        private string connectionString = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public EContact()
        {
            sharedMethods = new MShared();

            countryEntity = new CountryEntity();
            provinceEntity = new ProvinceEntity();
            cantonEntity = new CantonEntity();
            districtEntity = new DistrictEntity();
            doctorEntity = new DoctorEntity();
            connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
            doseEntity = new EDose();
        }

        #region -> Internal Methods
        private JObject GetCreateContactJsonStructure(PatientSignup signupProperties,Guid processId,bool isChildContact)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();
            OtherInterestEntity otherInterestEntity = new OtherInterestEntity();
            try
            {
                if (signupProperties != null)
                {
                    jObject.Add($"{ContactFields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd"));

                    if (!(String.IsNullOrEmpty(signupProperties.country)))
                    {
                        jObject.Add($"{ContactSchemas.Country}@odata.bind", $"/{countryEntity.EntityPluralName}({CountryFields.IdCountry}='{signupProperties.country}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.province)))
                    {
                        jObject.Add($"{ContactSchemas.Province}@odata.bind", $"/{provinceEntity.EntityPluralName}({ProvinceFields.IdProvince}='{signupProperties.contactinfo.province}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.canton)))
                    {
                        jObject.Add($"{ContactSchemas.Canton}@odata.bind", $"/{cantonEntity.EntityPluralName}({CantonFields.IdCanton}='{signupProperties.contactinfo.canton}')");
                    }

                    if (!(String.IsNullOrEmpty(signupProperties.contactinfo.district)))
                    {
                        jObject.Add($"{ContactSchemas.District}@odata.bind", $"/{districtEntity.EntityPluralName}({DistrictFields.IdDistrict}='{signupProperties.contactinfo.district}')");
                    }

                    if (isChildContact)
                    {
                        jObject.Add(ContactFields.IsChildContact, true);
                    }
                    else
                    {
                        jObject.Add(ContactFields.IsChildContact, false);
                    }

                    //if (!(String.IsNullOrEmpty(signupProperties.otherInterest)))
                    //{
                    //    jObject.Add($"{ContactFields.OtherInterestLookup}", signupProperties.otherInterest);
                    //}




                    if (!(String.IsNullOrEmpty(signupProperties.userType)))
                    {
                        if (signupProperties.userType == "02" || signupProperties.userType == "03")
                        {
                            if (signupProperties.patientid_primary != null)
                                jObject.Add(ContactFields.IdAboxPatient, signupProperties.patientid_primary.ToString());
                        }
                        else if (signupProperties.userType == "05")
                        {
                            if (signupProperties.patientid != null)
                                jObject.Add(ContactFields.IdAboxPatient, signupProperties.patientid.ToString());
                        }
                        else if (signupProperties.userType == "01")
                        {
                            if (signupProperties.patientid != null)
                                jObject.Add(ContactFields.IdAboxPatient, signupProperties.patientid.ToString());
                        }
                    }
                    else
                    {
                        //Los pacientes bajo cuido, en el CRM actualmente no tienen un tipo de usuario definido
                        if (isChildContact)
                        {
                            if (signupProperties.patientid != null)
                                jObject.Add(ContactFields.IdAboxPatient, signupProperties.patientid.ToString());
                        }
                    }


                    if (!(String.IsNullOrEmpty(signupProperties.otherInterest)))
                    {
                        jObject.Add($"{otherInterestEntity.EntitySingularName}@odata.bind", $"/{otherInterestEntity.EntityPluralName}({OtherInterestFields.Id}='{signupProperties.otherInterest}')");
                    }

                    if (signupProperties.personalinfo != null)
                    {
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.name)))
                            jObject.Add(ContactFields.Firstname, signupProperties.personalinfo.name);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.lastname)))
                            jObject.Add(ContactFields.Lastname, signupProperties.personalinfo.lastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.secondlastname)))
                            jObject.Add(ContactFields.SecondLastname, signupProperties.personalinfo.secondlastname);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.password)))
                            jObject.Add(ContactFields.Password, signupProperties.personalinfo.password);

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.dateofbirth)))
                            jObject.Add(ContactFields.Birthdate, signupProperties.personalinfo.dateofbirth);

                        //TODO: Max length esta en 14 actualmente, validar extension
                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.id)))
                            jObject.Add(ContactFields.Id, signupProperties.personalinfo.id);

                        

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.idtype)))
                        {
                            string idType = sharedMethods.GetIdTypeId(signupProperties.personalinfo.idtype);
                            jObject.Add(ContactFields.IdType, idType);

                        }

                        if (!(String.IsNullOrEmpty(signupProperties.personalinfo.gender)))
                            jObject.Add(ContactFields.Gender, sharedMethods.GetGenderValue(signupProperties.personalinfo.gender));

                        if (!(String.IsNullOrEmpty(signupProperties.userType)))
                        {
                            jObject.Add($"{ContactSchemas.UserType}@odata.bind", $"/new_usertypes({sharedMethods.GetUserTypeEntityId(signupProperties.userType)})");
                        }
                    }

                    if (signupProperties.contactinfo != null)
                    {
                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.email)))
                            jObject.Add(ContactFields.Email, signupProperties.contactinfo.email);

                        if (!(String.IsNullOrEmpty(signupProperties.contactinfo.phone)))
                            jObject.Add(ContactFields.Phone, signupProperties.contactinfo.phone);

                        if (!String.IsNullOrEmpty(signupProperties.contactinfo.mobilephone))
                            jObject.Add(ContactFields.SecondaryPhone, signupProperties.contactinfo.mobilephone);
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
                            medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({DoctorFields.DoctorIdKey}='{signupProperties.medication.medics[i].medicid}')"));
                        }

                        //if (productsArray != null)
                        //{
                        //    jObject.Add($"{ContactFields.ContactxProductRelationship}@odata.bind", productsArray);
                        //}

                        if (medicsArray != null)
                        {
                            if (medicsArray.Count>0)
                            {
                                jObject.Add($"{ContactFields.ContactxDoctorRelationship}@odata.bind", medicsArray);

                            }
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
                        jObject.Add($"{ContactFields.Interests}", values);
                    }

                    

                }

                return jObject;
            }
            catch (Exception ex)
            {

                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                jObject = null;
                return jObject;
            }
        }

        private JObject GetUpdateAccountRequestStructure(UpdateAccountRequest updateProperties,Guid processId)
        {
            OperationResult result = new OperationResult();
            JObject jObject = new JObject();

            try
            {
                if (updateProperties != null)
                {
                    jObject.Add($"{ContactFields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd"));

                    if (!(String.IsNullOrEmpty(updateProperties.Nombre)))
                        jObject.Add(ContactFields.Firstname, updateProperties.Nombre);

                    if (!(String.IsNullOrEmpty(updateProperties.Apellido1)))
                        jObject.Add(ContactFields.Lastname, updateProperties.Apellido1);

                    if (!(String.IsNullOrEmpty(updateProperties.Apellido2)))
                        jObject.Add(ContactFields.SecondLastname, updateProperties.Apellido2);

                    if (!(String.IsNullOrEmpty(updateProperties.FechaNacimiento)))
                        jObject.Add(ContactFields.Birthdate, updateProperties.FechaNacimiento);

                    if (!(String.IsNullOrEmpty(updateProperties.Genero)))
                        jObject.Add(ContactFields.Gender, sharedMethods.GetGenderValue(updateProperties.Genero));

                    if (!(String.IsNullOrEmpty(updateProperties.Telefono)))
                        jObject.Add(ContactFields.Phone, updateProperties.Telefono);

                    if (!(String.IsNullOrEmpty(updateProperties.Telefono2)))
                        jObject.Add(ContactFields.SecondaryPhone, updateProperties.Telefono2);

                    if (!(String.IsNullOrEmpty(updateProperties.Email)))
                    {
                        if (updateProperties.Email.Contains(Constants.NoEmailDefaultAddress))
                        {
                            jObject.Add(ContactFields.NoEmail, 1);
                        }
                        else
                        {
                            jObject.Add(ContactFields.Email, updateProperties.Email);
                            jObject.Add(ContactFields.NoEmail, 0);
                        }


                    }


                    if (!(String.IsNullOrEmpty(updateProperties.Provincia)))
                    {
                        jObject.Add($"{ContactSchemas.Province}@odata.bind", $"/{provinceEntity.EntityPluralName}({ProvinceFields.IdProvince}='{updateProperties.Provincia}')");
                    }

                    if (!(String.IsNullOrEmpty(updateProperties.Canton)))
                    {
                        jObject.Add($"{ContactSchemas.Canton}@odata.bind", $"/{cantonEntity.EntityPluralName}({CantonFields.IdCanton}='{updateProperties.Canton}')");
                    }

                    if (!(String.IsNullOrEmpty(updateProperties.Distrito)))
                    {
                        jObject.Add($"{ContactSchemas.District}@odata.bind", $"/{districtEntity.EntityPluralName}({DistrictFields.IdDistrict}='{updateProperties.Distrito}')");
                    }


                    if (updateProperties.medication != null)
                    {
                        //JArray productsArray = new JArray();
                        //JArray medicsArray = new JArray();
                        //ProductEntity productEntity = new ProductEntity();
                        //DoctorEntity doctorEntity = new DoctorEntity();

                        //int productsLength = updateProperties.medication.products.Length;
                        //for (int i = 0; i < productsLength; i++)
                        //{
                        //    productsArray.Add(new JValue($"/{productEntity.EntityPluralName}({ProductFields.ProductNumber}='{updateProperties.medication.products[i].productid}')"));
                        //}

                        //int medicsLength = updateProperties.medication.medics.Length;
                        //for (int i = 0; i < medicsLength; i++)
                        //{
                        //    medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({DoctorFields.DoctorIdKey}='{updateProperties.medication.medics[i].medicid}')"));
                        //}

                        //if (productsArray != null && productsArray.Count > 0)
                        //{
                        //    jObject.Add($"{ContactFields.ContactxProductRelationship}@odata.bind", productsArray);
                        //}

                        //if (medicsArray != null && medicsArray.Count > 0)
                        //{
                        //    jObject.Add($"{ContactFields.ContactxDoctorRelationship}@odata.bind", medicsArray);
                        //}
                    }

                    if (updateProperties.interests != null)
                    {
                        string values = "";
                        int length = updateProperties.interests.Length;
                        for (int i = 0; i < length; i++)
                        {
                            if (values != "")
                                values += "," + updateProperties.interests[i].interestid;
                            else
                                values += updateProperties.interests[i].interestid;
                        }
                        jObject.Add($"{ContactFields.Interests}", values);
                    }



                }

                return jObject;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                jObject = null;
                return jObject;
            }
        }

        private JObject GetUpdatePatientJsonStructure(UpdatePatientRequest updateProperties,Guid processId)
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
                            jObject.Add(ContactFields.Firstname, updateProperties.personalinfo.name);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.lastname)))
                            jObject.Add(ContactFields.Lastname, updateProperties.personalinfo.lastname);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.secondlastname)))
                            jObject.Add(ContactFields.SecondLastname, updateProperties.personalinfo.secondlastname);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.dateofbirth)))
                            jObject.Add(ContactFields.Birthdate, updateProperties.personalinfo.dateofbirth);

                        if (!(String.IsNullOrEmpty(updateProperties.personalinfo.gender)))
                            jObject.Add(ContactFields.Gender, sharedMethods.GetGenderValue(updateProperties.personalinfo.gender));
                    }

                    if (updateProperties.medication != null)
                    {
                        //JArray productsArray = new JArray();
                        //JArray medicsArray = new JArray();
                        //ProductEntity productEntity = new ProductEntity();
                        //DoctorEntity doctorEntity = new DoctorEntity();

                        //int productsLength = updateProperties.medication.products.Length;
                        //for (int i = 0; i < productsLength; i++)
                        //{
                        //    productsArray.Add(new JValue($"/{productEntity.EntityPluralName}({productEntity.Fields.ProductNumber}='{updateProperties.medication.products[i].productid}')"));
                        //}

                        //int medicsLength = updateProperties.medication.medics.Length;
                        //for (int i = 0; i < medicsLength; i++)
                        //{
                        //    medicsArray.Add(new JValue($"/{doctorEntity.EntityPluralName}({DoctorFields.DoctorIdKey}='{updateProperties.medication.medics[i].medicid}')"));
                        //}

                        //if (productsArray != null)
                        //{
                        //    jObject.Add($"{ContactFields.ContactxProductRelationship}@odata.bind", productsArray);
                        //}

                        //if (medicsArray != null)
                        //{
                        //    jObject.Add($"{ContactFields.ContactxDoctorRelationship}@odata.bind", medicsArray);
                        //}
                    }
                }

                return jObject;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                jObject = null;
                return jObject;
            }
        }

       

        private OperationResult ContactCreateRequest(JObject jsonObject,Guid processId)
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
                            client.DefaultRequestHeaders.Add("MSCRM.SuppressDuplicateDetection","false");

                            MethodBase m = MethodBase.GetCurrentMethod();
                            string url = $"contacts?$select={ContactFields.EntityId}";
                            


                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"ProcessID: {processId} Url:{url} Data:{jsonObject.ToString(Formatting.None)}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);


                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PostAsync(url, c).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                string userId = (string)body[ContactFields.EntityId];
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

                                if (err != null)
                                {
                                     log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);
                                }
                                    


                                operationResult.Code = "";
                                operationResult.Message = "Error al crear el contacto en el CRM";
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Crear Queue de procesos fallidos en BD para reprocesar
                        LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null,"",null,ex);
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);
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
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        private OperationResult ContactDeleteRequest(string contactId, Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                if (contactId != null)
                {
                    try
                    {
                        using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                        {
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                            MethodBase m = MethodBase.GetCurrentMethod();
                            //client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                            string url = $"{this.EntityPluralName}({contactId})";
                           

                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Url:{url}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);

                            //HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.DeleteAsync(url).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //string userId = (string)body[DoseFields.EntityId];
                                operationResult.Code = "";
                                operationResult.Message = "Contacto eliminado correctamente del CRM";
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

                                if (err != null)
                                {
                                    log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);
                                }
                                   
                                operationResult.Code = "Error al eliminar el contacto del CRM";
                                operationResult.Message = response.ReasonPhrase;
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null,"",null,ex);
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);
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
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        private OperationResult ContactRelatedDosesRequest(int idContact,Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                        MethodBase m = MethodBase.GetCurrentMethod();
                        string url = $"{this.EntityPluralName}({ContactFields.IdAboxPatient}={idContact})?$select={ContactFields.EntityId}&$expand={ContactFields.ContactxDoseRelationship}($select={DoseFields.EntityId},{DoseFields.Dose};$expand=new_ProductDose($select={ProductFields.ProductNumber}))";
                        

                        LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Url:{url}");
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);

                        //var response = client.GetAsync($"{this.EntityPluralName}({ContactFields.IdAboxPatient}={idContact})?$select={ContactFields.EntityId}&$expand={ContactFields.ContactxDoseRelationship}($select={DoseFields.EntityId},{DoseFields.Dose})").Result;
                        /*TODO: Optimizar este request, el expand no esta trayendo los datos de las dosis, se necesita sacar
                         el ID del producto que tiene esta entidad Dosis para mejorar los tiempos de actualizacion, Valorar
                        ANY o ALL filtros de web API que se pueden usar*/
                        var response = client.GetAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            // string doseId = (string)body[DoseFields.EntityId];
                            var dataFromBody = body[DoseFields.ContactxDoseRelationship];
                            JArray dosesRetrieved = JArray.FromObject(dataFromBody);
                            DoseRecord[] dosesFound = new DoseRecord[dosesRetrieved.Count];

                            for (int i = 0; i < dosesRetrieved.Count; i++)
                            {
                                dosesFound[i] = new DoseRecord
                                {
                                    DoseId = dosesRetrieved[i].SelectToken(DoseFields.EntityId).ToString(),
                                    Dose = dosesRetrieved[i].SelectToken(DoseFields.Dose).ToString(),
                                    //IdProduct = dosesRetrieved[i].SelectToken(DoseFields.DosexProduct).ToString()
                                };
                            }

                            operationResult.Code = "";
                            operationResult.Message = "Dosis extraídas correctamente";
                            operationResult.IsSuccessful = true;
                            operationResult.Data = dosesFound;
                        }
                        else
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                            JObject error = JObject.Parse(body.ToString());
                            CrmWebAPIError err = error.ToObject<CrmWebAPIError>();

                            
                            if (err != null)
                            {
                                log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                log.Properties["ProcessID"] = processId;
                                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                Logger.Log(log);
                            }
                               

                            operationResult.Code = "Error al obtener las dosis del contacto en el CRM";
                            operationResult.Message = response.ReasonPhrase;
                            operationResult.IsSuccessful = false;
                            operationResult.Data = null;
                            operationResult.InternalError = err;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                    log.Properties["ProcessID"] = processId;
                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    Logger.Log(log);
                    operationResult.Code = "";
                    operationResult.Message = ex.ToString();
                    operationResult.IsSuccessful = false;
                    operationResult.Data = null;
                }

                return operationResult;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        private OperationResult ContactRelatedDoctorsRequest(int idContact, Guid processId)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                        MethodBase m = MethodBase.GetCurrentMethod();
                        string url = $"{this.EntityPluralName}({ContactFields.IdAboxPatient}={idContact})?$select={ContactFields.EntityId}&$expand={ContactFields.ContactxDoctorRelationship}($select={DoctorFields.DoctorIdKey})";

                        LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Url:{url}");
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);

                        var response = client.GetAsync(url).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            // string doseId = (string)body[DoseFields.EntityId];
                            var dataFromBody = body[ContactFields.ContactxDoctorRelationship];
                            JArray doctorsRetrieved = JArray.FromObject(dataFromBody);
                            string[] idsFound = new string[doctorsRetrieved.Count];

                            for (int i = 0; i < doctorsRetrieved.Count; i++)
                            {
                                idsFound[i] = doctorsRetrieved[i].SelectToken(DoctorFields.DoctorIdKey).ToString();
                            }

                            operationResult.Code = "";
                            operationResult.Message = "Doctores extraídos correctamente";
                            operationResult.IsSuccessful = true;
                            operationResult.Data = idsFound;
                        }
                        else
                        {
                            //Get the response content and parse it.
                            JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //CrmWebAPIError userId = (CrmWebAPIError)body["error"];
                            JObject userId = JObject.Parse(body.ToString());
                            CrmWebAPIError err = userId.ToObject<CrmWebAPIError>();


                            
                            if (err != null)
                            {
                                log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                log.Properties["ProcessID"] = processId;
                                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                Logger.Log(log);
                            }
                                
                            operationResult.Code = "Error al obtener los doctores relacionados del contacto en el CRM";
                            operationResult.Message = response.ReasonPhrase;
                            operationResult.IsSuccessful = false;
                            operationResult.Data = null;
                            operationResult.InternalError = err;
                        }
                    }
                }
                catch (Exception ex)
                {

                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                    log.Properties["ProcessID"] = processId;
                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    Logger.Log(log);
                    operationResult.Code = "";
                    operationResult.Message = ex.ToString();
                    operationResult.IsSuccessful = false;
                    operationResult.Data = null;
                }

                return operationResult;
            }
            catch (Exception ex)
            {

                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        private OperationResult ContactUpdateRequest(JObject jsonObject, string idToUpdate,Guid processId)
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

                            //Este Header previene un Upsert (Que inserte el contacto si no existe)
                            client.DefaultRequestHeaders.Add("If-Match", "*");
                            MethodBase m = MethodBase.GetCurrentMethod();

                            string url = $"contacts({ContactFields.IdAboxPatient}={idToUpdate})";
                         

                            LogEventInfo log = new LogEventInfo(LogLevel.Debug, Logger.Name, $"Url:{url} Data:{jsonObject.ToString(Formatting.None)}");
                            log.Properties["ProcessID"] = processId;
                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            Logger.Log(log);

                            HttpContent c = new StringContent(jsonObject.ToString(Formatting.None), Encoding.UTF8, "application/json");
                            var response = client.PatchAsync(url, c).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                //Get the response content and parse it.
                                //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                                //string userId = (string)body[ContactFields.EntityId];
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

                              
                                if (err != null)
                                {
                                    log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Url:{url} ErrorCode:{err.error.code} ErrorMessage:{err.error.message} ResponseReasonPhrase:{response.ReasonPhrase}");
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);
                                }
                                
                                operationResult.Code = "";
                                operationResult.Message = "Error al actualizar el contacto en el CRM";
                                operationResult.IsSuccessful = false;
                                operationResult.Data = null;
                                operationResult.InternalError = err;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                        log.Properties["ProcessID"] = processId;
                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        Logger.Log(log);

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

                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                operationResult.Code = "";
                operationResult.Message = ex.ToString();
                operationResult.IsSuccessful = false;
                operationResult.Data = null;
                return operationResult;
            }
        }

        //TODO: Optimizar estos dos metodos que hacen lo mismo y buscar forma de reutilizarlos
        private List<string> MatchingMedicsFromPatientUpdate(UpdatePatientRequest.Medic[] array, string[] contactRelatedMedics)
        {
            List<string> matching = new List<string>();
            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                if (Array.IndexOf(contactRelatedMedics, array[i].medicid) > -1)
                {
                    matching.Add(array[i].medicid);
                }
            }
            return matching;

        }

        private List<string> MatchingMedicsFromAccountUpdate(UpdateAccountRequest.Medic[] array, string[] contactRelatedMedics)
        {
            List<string> matching = new List<string>();
            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                if (Array.IndexOf(contactRelatedMedics, array[i].medicid) > -1)
                {
                    matching.Add(array[i].medicid);
                }
            }
            return matching;

        }

        //TODO: Optimizar estos dos metodos que hacen lo mismo y buscar forma de reutilizarlos
        private List<string> MatchingProductsFromPatientUpdate(UpdatePatientRequest.Product[] array, DoseRecord[] contactRelatedProducts)
        {
            List<string> matching = new List<string>();
            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                int indexFound = Array.IndexOf(contactRelatedProducts, array[i].productid);
                if (indexFound > -1)
                {

                    if (Convert.ToInt32(contactRelatedProducts[indexFound].Dose) == sharedMethods.GetDoseFrequencyValue(array[i].frequency))
                    {
                        matching.Add(array[i].productid);

                    }
                }
            }
            return matching;

        }


        private List<string> MatchingProductsFromAccountUpdate(UpdateAccountRequest.Product[] array, DoseRecord[] contactRelatedProducts)
        {
            List<string> matching = new List<string>();
            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                int indexFound = Array.IndexOf(contactRelatedProducts, array[i].productid);
                if (indexFound > -1)
                {

                    if (Convert.ToInt32(contactRelatedProducts[indexFound].Dose) == sharedMethods.GetDoseFrequencyValue(array[i].frequency))
                    {
                        matching.Add(array[i].productid);

                    }
                }
            }
            return matching;

        }

       

        #endregion


        #region -> Public Methods

        public OperationResult CreateAsPatient(PatientSignup signupRequest, string idRelatedPatient, Guid processId, bool isChildContact = false)
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
                                        },processId);

                                        if (result.IsSuccessful)
                                        {
                                            dosesCreated[i] = (string)result.Data;
                                        }
                                        else
                                        {
                                            LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, $"Message:{result.Message} ProductId: {dosesArray[i].IdProduct}");
                                            log.Properties["ProcessID"] = processId;
                                            log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                            log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                            Logger.Log(log);

                                        }
                                    }

                                    //if (dosesCreated.Length != dosesArray.Length)
                                    //{
                                    //    throw (new Exception("Ha ocurrido un error creando las dosis del paciente " + signupProperties.personalinfo.id + " en el CRM"));
                                    //}
                                }
                                catch (Exception ex)
                                {
                                   
                                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name,null, $"ProcessID: {processId}",null,ex);
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);

                                    throw ex;
                                }
                            }
                        }
                    }

                    #endregion -> Dose Retrieve

                    JObject newContact = this.GetCreateContactJsonStructure(signupProperties,processId,isChildContact);

                    #region -> Patient Related

                    if (!String.IsNullOrEmpty(idRelatedPatient))
                    {
                        JArray patientsInChargeArray = new JArray();
                        patientsInChargeArray.Add(new JValue($"/{this.EntityPluralName}({idRelatedPatient})"));
                        newContact.Add($"{ContactFields.ContactxContactRelationship}@odata.bind", patientsInChargeArray);
                    }

                    #endregion -> Patient Related

                    #region -> Doses Related

                    if (dosesArray != null && dosesCreated != null)
                    {
                        if (dosesCreated.Length == dosesArray.Length)
                        {
                            JArray dosesToSave = new JArray();
                            int length = dosesCreated.Length;
                            for (int i = 0; i < length; i++)
                            {
                                if (dosesCreated[i] != null)
                                    dosesToSave.Add(new JValue($"/{doseEntity.EntityPluralName}({dosesCreated[i]})"));

                            }
                            if (dosesToSave.Count > 0)
                            {
                                newContact.Add($"{ContactFields.ContactxDoseRelationship}@odata.bind", dosesToSave);

                            }
                        }
                    }

                    #endregion -> Doses Related

                    responseObject = this.ContactCreateRequest(newContact, processId);
                }
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                
                //Logger.Error(ex,"");
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }

        public OperationResult CreateAsConsumer(PatientSignup signupRequest, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                PatientSignup signupProperties = signupRequest;

                if (signupProperties != null)
                {
                    JObject newContact = this.GetCreateContactJsonStructure(signupProperties, processId,false);
                    responseObject = this.ContactCreateRequest(newContact, processId);
                }
            }
            catch (Exception ex)
            {
                
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);

                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }


        public OperationResult CreateAsCaretaker(PatientSignup signupRequest,Guid processId)
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
                    OperationResult responseFromPatientUnderCareCreate = null;
                    OperationResult responseFromOwnerCreate = null;

                    /*Crear primeramente el paciente a cargo, para así continuar con el encargado pues ya se tiene el ID del paciente creado en CRM
                    para poder relacionarlo.*/

                    //TODO: Validar si son varios bajo cuido
                    #region -> Crear paciente bajo cuido como contacto

                    PatientSignup signupPatientUnderCare = null;
                    signupPatientUnderCare = new PatientSignup
                    {
                        patientid = signupPropertiesFromRequest.patientid,
                        patientid_primary = null,
                        country = signupPropertiesFromRequest.country,
                        userType = "",//Si se agrega un tipo de usuario default para usuarios bajo cuido, revisar la asignacion del id de paciente en metodo GetCreateContactJsonStructure 
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

                    responseFromPatientUnderCareCreate = this.CreateAsPatient(signupPatientUnderCare, null,processId,true);

                    #endregion 

                    /*Se crea el paciente a cargo correctamente, ya existe en Dynamics y se procede a crear el dueño de la cuenta
                     *para poder relacionarlo.*/

                    #region -> Crear Dueño de la cuenta como Contacto

                    PatientSignup signupPatientOwner = null;
                    if (responseFromPatientUnderCareCreate.IsSuccessful)
                    {
                        string idUnderCarePatient = responseFromPatientUnderCareCreate.Data.ToString();
                        signupPatientOwner = signupPropertiesFromRequest;
                        signupPatientOwner.medication = null;
                        signupPatientOwner.patientincharge = null;

                        responseFromOwnerCreate = this.CreateAsPatient(signupPatientOwner, idUnderCarePatient,processId,false);

                        if (responseFromOwnerCreate.IsSuccessful)
                        {
                            responseObject.IsSuccessful = true;
                            responseObject.Message = "Registro completado correctamente";
                            responseObject.Code = "";
                        }
                        else
                        {
                            
                            this.Delete(idUnderCarePatient,processId);

                            responseObject.IsSuccessful = false;
                            responseObject.Message = "Ocurrió un error al crear el dueño de la cuenta";
                            responseObject.InternalError = responseFromPatientUnderCareCreate.InternalError;
                            responseObject.Code = "";
                        }
                    }
                    else
                    {
                        responseObject.IsSuccessful = false;
                        responseObject.Message = "Ocurrió un error al crear el paciente a cargo";
                        responseObject.InternalError = responseFromPatientUnderCareCreate.InternalError;
                        responseObject.Code = "";
                    }

                    #endregion 
                }
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                responseObject.IsSuccessful = false;
                responseObject.Message = ex.Message;
                responseObject.Code = "";
            }

            return responseObject;
        }

        public OperationResult CreateAsTutor(PatientSignup signupRequest,Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                PatientSignup signupPropertiesFromRequest = signupRequest;

                if (signupRequest != null)
                {
                    OperationResult responseFromPatientUndercareCreation = null;
                    OperationResult responseFromOwnerCreate = null;

                    /*Crear primeramente el paciente a cargo, para así continuar con el encargado pues ya se tiene el ID del paciente creado en CRM
                    para poder relacionarlo.*/

                    #region -> Crear paciente bajo cuido como contacto

                    PatientSignup signupPatientUnderCare = null;
                    signupPatientUnderCare = new PatientSignup
                    {
                        country = signupPropertiesFromRequest.country,
                        userType = "",//Si se agrega un tipo de usuario default para usuarios bajo cuido, revisar la asignacion del id de paciente en metodo GetCreateContactJsonStructure
                        patientid = signupPropertiesFromRequest.patientid,
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

                    responseFromPatientUndercareCreation = this.CreateAsPatient(signupPatientUnderCare, null,processId,true);

                    #endregion -> Crear paciente bajo cuido como contacto

                    /*Se crea el paciente a cargo correctamente, ya existe en Dynamics y se procede a crear el dueño de la cuenta
                     *para poder relacionarlo.*/

                    #region -> Crear Dueño de la cuenta como Contacto

                    PatientSignup signupPatientOwner = null;
                    if (responseFromPatientUndercareCreation.IsSuccessful)
                    {
                        string idUnderCarePatient = responseFromPatientUndercareCreation.Data.ToString();
                        signupPatientOwner = signupPropertiesFromRequest;
                        signupPatientOwner.medication = null;
                        signupPatientOwner.patientincharge = null;

                        responseFromOwnerCreate = this.CreateAsPatient(signupPatientOwner, idUnderCarePatient,processId,false);

                        if (responseFromOwnerCreate.IsSuccessful)
                        {
                            responseObject.IsSuccessful = true;
                            responseObject.Message = "Registro completado correctamente";
                            responseObject.Code = "";
                        }
                        else
                        {
                            //TODO: Eliminar al contacto que se creo del paciente a cargo o quedara huerfano
                            this.Delete(idUnderCarePatient, processId);
                            responseObject.IsSuccessful = false;
                            responseObject.Message = "Ocurrió un error al crear el dueño de la cuenta";
                            responseObject.InternalError = responseFromPatientUndercareCreation.InternalError;
                            responseObject.Code = "";
                        }
                    }
                    else
                    {
                        responseObject.IsSuccessful = false;
                        responseObject.Message = "Ocurrió un error al crear el paciente a cargo";
                        responseObject.InternalError = responseFromPatientUndercareCreation.InternalError;
                        responseObject.Code = "";
                    }

                    #endregion -> Crear Dueño de la cuenta como Contacto
                }
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                responseObject.IsSuccessful = false;
                responseObject.Message = ex.Message;
                responseObject.Code = "";
            }

            return responseObject;
        }

        public OperationResult UpdateAccount(UpdateAccountRequest updateAccountRequest,Guid processId)
        {
            OperationResult result = null;
            try
            {
                if (updateAccountRequest != null)
                {
                    int contactId = Int32.Parse(updateAccountRequest.patientId);

                    if ((updateAccountRequest.medication != null) && updateAccountRequest.TipoUsuario == "01")
                    {


                        DoseRecord[] contactRelatedDoses = null;
                        #region Doses Delete

                        OperationResult dosesResult = null;
                        List<string> matchingDoses = null;
                        /*Eliminar las dosis que tenga el usuario para relacionarle las nuevas, no se hace un update, se hace un delete
                         completamente y se relacionan nuevas dosis*/
                        bool dosesDeleted = false;
                        if (!String.IsNullOrEmpty(updateAccountRequest.patientId))
                        {
                            dosesResult = this.GetDosesRelated(contactId, processId);
                            if (dosesResult.IsSuccessful)
                            {
                                contactRelatedDoses = (DoseRecord[])dosesResult.Data;

                                matchingDoses = this.MatchingProductsFromAccountUpdate(updateAccountRequest.medication.products, contactRelatedDoses);

                                if (contactRelatedDoses != null && contactRelatedDoses.Length > 0)
                                {
                                    if (contactRelatedDoses.Length > 0)
                                    {
                                        int deletedCount = 0;
                                        for (int i = 0; i < contactRelatedDoses.Length; i++)
                                        {
                                            //if (!matchingDoses.Contains(contactRelatedDoses[i].DoseId))
                                            //{
                                            OperationResult deleteResult = doseEntity.Delete(contactRelatedDoses[i].DoseId,processId);
                                            if (deleteResult.IsSuccessful)
                                            {
                                                deletedCount++;
                                            }
                                            //}

                                        }

                                    }
                                }
                            }
                            else
                            {
                                result = dosesResult;
                                return result;
                            }
                        }

                        #endregion 

                    }

                    var contactStructure = this.GetUpdateAccountRequestStructure(updateAccountRequest,processId);


                    if ((updateAccountRequest.medication != null) && updateAccountRequest.TipoUsuario == "01")
                    {
                        /*Este request se deja por fuera del metodo que crea toda la estructura de
                * Contacto porque es un proceso individual de crear una entidad de Dosis,
                * la cual puede eventualmente fallar o no crearse correctamente, ademas se necesita
                * ligar el resultado de esta operacion al request que crea el contacto en el crm
                */
                        DoseRecord[] dosesArray = null;
                        string[] dosesCreated = null;

                        #region -> Dose Create

                        if (updateAccountRequest.medication != null)
                        {
                            int dosesLength = updateAccountRequest.medication.products.Length;
                            dosesArray = new DoseRecord[dosesLength];
                            dosesCreated = new string[dosesLength];

                            for (int i = 0; i < dosesLength; i++)
                            {
                                string frequency = "";
                                if (!String.IsNullOrEmpty(updateAccountRequest.medication.products[i].other))
                                    frequency = updateAccountRequest.medication.products[i].other;
                                else
                                    frequency = updateAccountRequest.medication.products[i].frequency;

                                dosesArray[i] = new DoseRecord
                                {
                                    Dose = frequency,
                                    IdProduct = updateAccountRequest.medication.products[i].productid,
                                    ContactBinding = $"{this.EntityPluralName}({ContactFields.IdAboxPatient}={updateAccountRequest.patientId})"
                                };
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
                                            //OperationResult doseCreateResult = doseEntity.Create(new DoseRecord
                                            //{
                                            //    Dose = dosesArray[i].Dose,
                                            //    IdProduct = dosesArray[i].IdProduct
                                            //});
                                            //if (!matchingDoses.Contains(dosesArray[i].IdProduct))
                                            //{
                                            OperationResult doseCreateResult = doseEntity.Create(dosesArray[i],processId);

                                            if (doseCreateResult.IsSuccessful)
                                            {
                                                dosesCreated[i] = (string)doseCreateResult.Data;
                                            }
                                            //}

                                        }

                                        //if (dosesCreated.Length != dosesArray.Length)
                                        //{
                                        //    return new OperationResult
                                        //    {
                                        //        IsSuccessful = false,
                                        //        Message = "Ocurrió un error creando alguna de las dosis del paciente",
                                        //        InternalError = null,
                                        //        Code = ""
                                        //    };
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                                        log.Properties["ProcessID"] = processId;
                                        log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                        log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                        Logger.Log(log);
                                        throw ex;
                                    }
                                }
                            }
                        }

                        #endregion 


                        List<string> matchingMedics = null;
                        string[] contactRelatedDoctors = null;

                        #region Medics Disassociate

                        OperationResult doctorsResult = null;

                        /*TODO: validar posibilidad de identificar cuantos medicamentos del request hacen match con los
                         que tiene ya el contacto para evitar tener que hacer desasociaciones*/
                        doctorsResult = this.GetDoctorsRelated(contactId,processId);

                        bool contactsDisassociated = false;
                        if (doctorsResult.IsSuccessful)
                        {
                            contactRelatedDoctors = (string[])doctorsResult.Data;

                            matchingMedics = this.MatchingMedicsFromAccountUpdate(updateAccountRequest.medication.medics, contactRelatedDoctors);

                            if (contactRelatedDoctors != null)
                            {
                                if (contactRelatedDoctors.Length > 0)
                                {
                                    int disassociatedCount = 0;
                                    for (int i = 0; i < contactRelatedDoctors.Length; i++)
                                    {

                                        if (!matchingMedics.Contains(contactRelatedDoctors[i]))
                                        {
                                            EntityAssociation entityDisassociation = new EntityAssociation
                                            {
                                                RelatedEntityId = contactRelatedDoctors[i],
                                                RelatedEntityIdKeyToUse = DoctorFields.DoctorIdKey,
                                                RelatedEntityName = doctorEntity.EntityPluralName,
                                                TargetEntityId = updateAccountRequest.patientId,
                                                TargetEntityName = this.EntityPluralName,
                                                TargetIdKeyToUse = ContactFields.IdAboxPatient,
                                                RelationshipDefinitionName = ContactFields.ContactxDoctorRelationship
                                            };
                                            var disassociateResult = entityDisassociation.Disassociate(connectionString,processId);

                                            //if (disassociateResult.IsSuccessful)
                                            //{
                                            //    disassociatedCount++;
                                            //}
                                        }

                                    }
                                    //if (disassociatedCount == contactRelatedDoctors.Length)
                                    //{
                                    //    contactsDisassociated = true;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            result = doctorsResult;
                            return result;
                        }

                        //if ((contactRelatedDoctors != null) && (contactRelatedDoctors.Length > 0))
                        //{
                        //    if (!contactsDisassociated)
                        //    {
                        //        return new OperationResult
                        //        {
                        //            IsSuccessful = false,
                        //            Message = "Los doctores relacionados del contacto no se pudieron desasociar",
                        //            InternalError = null,
                        //            Code = ""
                        //        };
                        //    }
                        //}

                        #endregion 

                        #region Medics Associate

                        if (updateAccountRequest.medication != null)
                        {
                            if ((updateAccountRequest.medication.medics != null) && (updateAccountRequest.medication.medics.Length > 0))
                            {
                                var medicsLength = updateAccountRequest.medication.medics.Length;

                                for (int i = 0; i < medicsLength; i++)
                                {
                                    if (!matchingMedics.Contains(updateAccountRequest.medication.medics[i].medicid))
                                    {
                                        EntityAssociation entityAssociation = new EntityAssociation
                                        {
                                            RelatedEntityId = updateAccountRequest.medication.medics[i].medicid,
                                            RelatedEntityIdKeyToUse = DoctorFields.DoctorIdKey,
                                            RelatedEntityName = doctorEntity.EntityPluralName,
                                            TargetEntityId = updateAccountRequest.patientId,
                                            TargetEntityName = this.EntityPluralName,
                                            TargetIdKeyToUse = ContactFields.IdAboxPatient,
                                            RelationshipDefinitionName = ContactFields.ContactxDoctorRelationship
                                        };

                                        var associationResult = entityAssociation.Associate(connectionString,processId);

                                        if (!associationResult.IsSuccessful)
                                        {
                                            return new OperationResult
                                            {
                                                IsSuccessful = false,
                                                Message = "Ocurrió un error relacionando los doctores al contacto",
                                                InternalError = null,
                                                Code = ""
                                            };
                                        }
                                    }

                                }
                            }
                        }

                        #endregion Medics Associate


                    }


                    result = this.ContactUpdateRequest(contactStructure, updateAccountRequest.patientId,processId);
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
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                result.IsSuccessful = false;
                result.Message = ex.ToString();
                result.InternalError = null;
                result.Code = "";
                return result;
            }
        }

        public OperationResult UpdatePatient(UpdatePatientRequest updatePatientRequest,Guid processId)
        {
            OperationResult result = null;
            try
            {
                if (updatePatientRequest != null)
                {
                    int contactId = Int32.Parse(updatePatientRequest.patientid);

                    DoseRecord[] contactRelatedDoses = null;
                    #region Doses Delete

                    OperationResult dosesResult = null;
                    List<string> matchingDoses = null;
                    /*Eliminar las dosis que tenga el usuario para relacionarle las nuevas, no se hace un update, se hace un delete
                     completamente y se relacionan nuevas dosis*/
                    bool dosesDeleted = false;
                    if (!String.IsNullOrEmpty(updatePatientRequest.patientid))
                    {
                        dosesResult = this.GetDosesRelated(contactId,processId);
                        if (dosesResult.IsSuccessful)
                        {
                            contactRelatedDoses = (DoseRecord[])dosesResult.Data;

                            matchingDoses = this.MatchingProductsFromPatientUpdate(updatePatientRequest.medication.products, contactRelatedDoses);

                            if (contactRelatedDoses != null && contactRelatedDoses.Length > 0)
                            {
                                if (contactRelatedDoses.Length > 0)
                                {
                                    int deletedCount = 0;
                                    for (int i = 0; i < contactRelatedDoses.Length; i++)
                                    {
                                        //if (!matchingDoses.Contains(contactRelatedDoses[i].DoseId))
                                        //{
                                        OperationResult deleteResult = doseEntity.Delete(contactRelatedDoses[i].DoseId,processId);
                                        if (deleteResult.IsSuccessful)
                                        {
                                            deletedCount++;
                                        }
                                        //}

                                    }
                                    //if (deletedCount == contactRelatedDoses.Length)
                                    //{
                                    //    dosesDeleted = true;
                                    //}
                                }
                            }
                        }
                        else
                        {
                            result = dosesResult;
                            return result;
                        }
                    }

                    //if ((contactRelatedDoses != null) && (contactRelatedDoses.Length > 0))
                    //{
                    //    if (!dosesDeleted)
                    //    {
                    //        return new OperationResult
                    //        {
                    //            IsSuccessful = false,
                    //            Message = "Las dosis relacionadas del contacto no se pudieron eliminar",
                    //            InternalError = null,
                    //            Code = ""
                    //        };
                    //    }
                    //}

                    #endregion 

                    var contactStructure = this.GetUpdatePatientJsonStructure(updatePatientRequest,processId);

                    /*Este request se deja por fuera del metodo que crea toda la estructura de
                * Contacto porque es un proceso individual de crear una entidad de Dosis,
                * la cual puede eventualmente fallar o no crearse correctamente, ademas se necesita
                * ligar el resultado de esta operacion al request que crea el contacto en el crm
                */
                    DoseRecord[] dosesArray = null;
                    string[] dosesCreated = null;

                    #region -> Dose Create

                    if (updatePatientRequest.medication != null)
                    {
                        int dosesLength = updatePatientRequest.medication.products.Length;
                        dosesArray = new DoseRecord[dosesLength];
                        dosesCreated = new string[dosesLength];

                        for (int i = 0; i < dosesLength; i++)
                        {
                            string frequency = "";
                            if (!String.IsNullOrEmpty(updatePatientRequest.medication.products[i].other))
                                frequency = updatePatientRequest.medication.products[i].other;
                            else
                                frequency = updatePatientRequest.medication.products[i].frequency;

                            dosesArray[i] = new DoseRecord
                            {
                                Dose = frequency,
                                IdProduct = updatePatientRequest.medication.products[i].productid,
                                ContactBinding = $"{this.EntityPluralName}({ContactFields.IdAboxPatient}={updatePatientRequest.patientid})"
                            };
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
                                        //OperationResult doseCreateResult = doseEntity.Create(new DoseRecord
                                        //{
                                        //    Dose = dosesArray[i].Dose,
                                        //    IdProduct = dosesArray[i].IdProduct
                                        //});
                                        //if (!matchingDoses.Contains(dosesArray[i].IdProduct))
                                        //{
                                        OperationResult doseCreateResult = doseEntity.Create(dosesArray[i],processId);

                                        if (doseCreateResult.IsSuccessful)
                                        {
                                            dosesCreated[i] = (string)doseCreateResult.Data;
                                        }
                                        //}

                                    }

                                    //if (dosesCreated.Length != dosesArray.Length)
                                    //{
                                    //    return new OperationResult
                                    //    {
                                    //        IsSuccessful = false,
                                    //        Message = "Ocurrió un error creando alguna de las dosis del paciente",
                                    //        InternalError = null,
                                    //        Code = ""
                                    //    };
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                                    log.Properties["ProcessID"] = processId;
                                    log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                                    log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                    Logger.Log(log);
                                    throw ex;
                                }
                            }
                        }
                    }

                    #endregion 

                    List<string> matchingMedics = null;
                    string[] contactRelatedDoctors = null;
                    #region Medics Disassociate

                    OperationResult doctorsResult = null;

                    /*TODO: validar posibilidad de identificar cuantos medicamentos del request hacen match con los
                     que tiene ya el contacto para evitar tener que hacer desasociaciones*/
                    doctorsResult = this.GetDoctorsRelated(contactId,processId);

                    bool contactsDisassociated = false;
                    if (doctorsResult.IsSuccessful)
                    {
                        contactRelatedDoctors = (string[])doctorsResult.Data;

                        matchingMedics = this.MatchingMedicsFromPatientUpdate(updatePatientRequest.medication.medics, contactRelatedDoctors);

                        if (contactRelatedDoctors != null)
                        {
                            if (contactRelatedDoctors.Length > 0)
                            {
                                int disassociatedCount = 0;
                                for (int i = 0; i < contactRelatedDoctors.Length; i++)
                                {

                                    if (!matchingMedics.Contains(contactRelatedDoctors[i]))
                                    {
                                        EntityAssociation entityDisassociation = new EntityAssociation
                                        {
                                            RelatedEntityId = contactRelatedDoctors[i],
                                            RelatedEntityIdKeyToUse = DoctorFields.DoctorIdKey,
                                            RelatedEntityName = doctorEntity.EntityPluralName,
                                            TargetEntityId = updatePatientRequest.patientid,
                                            TargetEntityName = this.EntityPluralName,
                                            TargetIdKeyToUse = ContactFields.IdAboxPatient,
                                            RelationshipDefinitionName = ContactFields.ContactxDoctorRelationship
                                        };
                                        var disassociateResult = entityDisassociation.Disassociate(connectionString,processId);

                                        //if (disassociateResult.IsSuccessful)
                                        //{
                                        //    disassociatedCount++;
                                        //}
                                    }

                                }
                                //if (disassociatedCount == contactRelatedDoctors.Length)
                                //{
                                //    contactsDisassociated = true;
                                //}
                            }
                        }
                    }
                    else
                    {
                        result = doctorsResult;
                        return result;
                    }

                    //if ((contactRelatedDoctors != null) && (contactRelatedDoctors.Length > 0))
                    //{
                    //    if (!contactsDisassociated)
                    //    {
                    //        return new OperationResult
                    //        {
                    //            IsSuccessful = false,
                    //            Message = "Los doctores relacionados del contacto no se pudieron desasociar",
                    //            InternalError = null,
                    //            Code = ""
                    //        };
                    //    }
                    //}

                    #endregion Medics Disassociate

                    #region Medics Associate

                    if (updatePatientRequest.medication != null)
                    {
                        if ((updatePatientRequest.medication.medics != null) && (updatePatientRequest.medication.medics.Length > 0))
                        {
                            var medicsLength = updatePatientRequest.medication.medics.Length;

                            for (int i = 0; i < medicsLength; i++)
                            {
                                if (!matchingMedics.Contains(updatePatientRequest.medication.medics[i].medicid))
                                {
                                    EntityAssociation entityAssociation = new EntityAssociation
                                    {
                                        RelatedEntityId = updatePatientRequest.medication.medics[i].medicid,
                                        RelatedEntityIdKeyToUse = DoctorFields.DoctorIdKey,
                                        RelatedEntityName = doctorEntity.EntityPluralName,
                                        TargetEntityId = updatePatientRequest.patientid,
                                        TargetEntityName = this.EntityPluralName,
                                        TargetIdKeyToUse = ContactFields.IdAboxPatient,
                                        RelationshipDefinitionName = ContactFields.ContactxDoctorRelationship
                                    };

                                    var associationResult = entityAssociation.Associate(connectionString,processId);

                                    if (!associationResult.IsSuccessful)
                                    {
                                        return new OperationResult
                                        {
                                            IsSuccessful = false,
                                            Message = "Ocurrió un error relacionando los doctores al contacto",
                                            InternalError = null,
                                            Code = ""
                                        };
                                    }
                                }

                            }
                        }
                    }

                    #endregion Medics Associate

                    result = this.ContactUpdateRequest(contactStructure, updatePatientRequest.patientid,processId);
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
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                result.IsSuccessful = false;
                result.Message = ex.ToString();
                result.InternalError = null;
                result.Code = "";
                return result;
            }
        }

        public OperationResult GetDosesRelated(int contactId,Guid processId)
        {
            OperationResult result = null;
            try
            {
                result = this.ContactRelatedDosesRequest(contactId,processId);
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                result = new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = "Ocurrió un error obteniendo las dosis relacionadas",
                    InternalError = null,
                    Code = ""
                };
            }
            return result;
        }

        public OperationResult GetDoctorsRelated(int contactId,Guid processId)
        {
            OperationResult result = null;
            try
            {
                result = this.ContactRelatedDoctorsRequest(contactId,processId);
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                result = new OperationResult
                {
                    IsSuccessful = false,
                    Data = null,
                    Message = "Ocurrió un error obteniendo los doctores relacionados del paciente",
                    InternalError = null,
                    Code = ""
                };
            }
            return result;
        }


        public OperationResult Delete(string contactId, Guid processId)
        {
            OperationResult responseObject = new OperationResult();

            try
            {
                responseObject = this.ContactDeleteRequest(contactId, processId);
                return responseObject;
            }
            catch (Exception ex)
            {
                LogEventInfo log = new LogEventInfo(LogLevel.Error, Logger.Name, null, "", null, new Exception(ex.ToString()));
                log.Properties["ProcessID"] = processId;
                log.Properties["AppID"] = AboxDynamicsBase.Classes.Constants.ApplicationIdWebAPI;
                log.Properties["MethodName"] = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Logger.Log(log);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }

            return responseObject;
        }

        #endregion
    }
}