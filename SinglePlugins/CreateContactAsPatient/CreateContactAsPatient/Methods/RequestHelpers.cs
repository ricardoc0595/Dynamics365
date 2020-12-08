using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using CreateContactAsPatient.Classes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CreateContactAsPatient.Methods
{
    public class RequestHelpers
    {
        private MShared sharedMethods = null;
        private ContactEntity contactEntity = null;
        private DoseEntity doseEntity = null;
        private ProductEntity productEntity = null;
        private DoctorEntity doctorEntity = null;
        private DistrictEntity districtEntity = null;
        private CantonEntity cantonEntity = null;
        private ProvinceEntity provinceEntity = null;

        public RequestHelpers()
        {
            doseEntity = new DoseEntity();
            sharedMethods = new MShared();
            contactEntity = new ContactEntity();
            productEntity = new ProductEntity();
            doctorEntity = new DoctorEntity();
            districtEntity = new DistrictEntity();
            cantonEntity = new CantonEntity();
            provinceEntity = new ProvinceEntity();
        }

        /// <summary>
        /// Recibe una entidad Contacto y prepara un Objeto serializable para actualizar en los servicios de Abox Plan
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public UpdatePatientRequest.Request GetPatientUpdateStructure(Entity contact, IOrganizationService service, ITracingService trace)
        {
            UpdatePatientRequest.Request requestStructure = new UpdatePatientRequest.Request();

            try
            {
                if (contact != null)
                {
                    #region -> Set request data based on Contact

                    requestStructure.personalinfo = new UpdatePatientRequest.Request.Personalinfo();

                    if (contact.Attributes.Contains(ContactFields.Firstname))
                        requestStructure.personalinfo.name = contact.GetAttributeValue<string>(ContactFields.Firstname);

                    if (contact.Attributes.Contains(ContactFields.Lastname))
                        requestStructure.personalinfo.lastname = contact.GetAttributeValue<string>(ContactFields.Lastname);

                    if (contact.Attributes.Contains(ContactFields.SecondLastname))
                        requestStructure.personalinfo.secondlastname = contact.GetAttributeValue<string>(ContactFields.SecondLastname);

                    if (contact.Attributes.Contains(ContactFields.Id))
                        requestStructure.personalinfo.id = contact.GetAttributeValue<string>(ContactFields.Id);

                    if (contact.Attributes.Contains(ContactFields.IdAboxPatient))
                        requestStructure.patientid = Convert.ToString(contact.GetAttributeValue<int>(ContactFields.IdAboxPatient));

                    if (contact.Attributes.Contains(ContactFields.Gender))
                    {
                        int val = (contact.GetAttributeValue<OptionSetValue>(ContactFields.Gender)).Value;
                        string gender = sharedMethods.GetGenderValue(val);
                        if (!String.IsNullOrEmpty(gender))
                        {
                            requestStructure.personalinfo.gender = gender;
                        }
                    }

                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        requestStructure.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");
                    }

                    if (contact.Attributes.Contains(ContactFields.IdType))
                    {
                        requestStructure.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                    }

                    if (contact.Attributes.Contains(ContactFields.UserType))
                    {
                        EntityReference userTypeReference = null;
                        userTypeReference = (EntityReference)contact.Attributes[ContactFields.UserType];
                        if (userTypeReference != null)
                        {
                            requestStructure.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                        }
                    }

                    if (contact.Attributes.Contains(ContactFields.Country))
                    {
                        EntityReference countryReference = null;
                        CountryEntity countryEntity = new CountryEntity();
                        countryReference = (EntityReference)contact.Attributes[ContactFields.Country];
                        if (countryReference != null)
                        {
                            var countryRetrieved = service.Retrieve(countryEntity.EntitySingularName, countryReference.Id, new ColumnSet(CountryFields.IdCountry));
                            if (countryRetrieved.Attributes.Contains(CountryFields.IdCountry))
                            {
                                string country = countryRetrieved.GetAttributeValue<string>(CountryFields.IdCountry);

                                if (!String.IsNullOrEmpty(country))
                                {
                                    requestStructure.country = country;
                                }
                            }
                        }
                    }

                    #endregion -> Set request data based on Contact

                    #region -> Productos actuales del Contacto 1:N

                    //Obtener los productos y dosis que ya tiene actualmente asignados el contacto

                    string[] doseColumnsToGet = new string[] { DoseFields.Dose, DoseFields.DosexProduct };
                    var doseColumnSet = new ColumnSet(doseColumnsToGet);

                    Guid parentId = contact.Id;
                    var query = new QueryExpression(doseEntity.EntitySingularName);
                    query.Criteria.AddCondition(new ConditionExpression(DoseFields.ContactxDose, ConditionOperator.Equal, parentId));
                    query.ColumnSet = doseColumnSet;
                    var contactDosesList = service.RetrieveMultiple(query);

                    if (contactDosesList != null)
                    {
                        if (contactDosesList.Entities != null)
                        {
                            if (contactDosesList.Entities.Count > 0)
                            {
                                int length = contactDosesList.Entities.Count;
                                requestStructure.medication = new UpdatePatientRequest.Request.Medication();

                                requestStructure.medication.products = new UpdatePatientRequest.Request.Product[length];
                                for (int i = 0; i < length; i++)
                                {
                                    var doseChild = contactDosesList[i];

                                    if (doseChild.Attributes.Contains(DoseFields.DosexProduct))
                                    {
                                        EntityReference productReference = null;
                                        productReference = (EntityReference)doseChild.Attributes[DoseFields.DosexProduct];
                                        if (productReference != null)
                                        {
                                            Entity product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { ProductFields.ProductNumber }));

                                            if (product.Attributes.Contains(ProductFields.ProductNumber))
                                            {
                                                string frequency = "";

                                                if (doseChild.Attributes.Contains(DoseFields.Dose))
                                                {
                                                    var optionSet = doseChild.GetAttributeValue<OptionSetValue>(DoseFields.Dose);
                                                    frequency = sharedMethods.GetDoseFrequencyValue(optionSet.Value);
                                                    //frequency = doseChild.GetAttributeValue<OptionSetValue>(DoseFields.Dose);
                                                }

                                                requestStructure.medication.products[i] = new UpdatePatientRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(ProductFields.ProductNumber)
                                                };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion -> Productos actuales del Contacto 1:N

                    #region -> Médicos actuales del contacto N:N

                    //En Dynamics, la relación N:N está definida desde Doctores a Contactos

                    string entity1 = doctorEntity.EntitySingularName;

                    string entity2 = contactEntity.EntitySingularName;

                    string relationshipEntityName = ContactFields.ContactxDoctorRelationship;

                    QueryExpression querym = new QueryExpression(entity1);

                    querym.ColumnSet = new ColumnSet(true);

                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, DoctorFields.EntityId, DoctorFields.EntityId, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);

                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, ContactFields.EntityId, ContactFields.EntityId, JoinOperator.Inner);

                    //linkEntity2.EntityAlias = "alias";

                    linkEntity1.LinkEntities.Add(linkEntity2);

                    querym.LinkEntities.Add(linkEntity1);

                    // Add condition to match

                    linkEntity2.LinkCriteria = new FilterExpression();

                    linkEntity2.LinkCriteria.AddCondition(new ConditionExpression(ContactFields.EntityId, ConditionOperator.Equal, contact.Id));

                    EntityCollection doctorsOfContact = service.RetrieveMultiple(querym);

                    if (doctorsOfContact != null)
                    {
                        if (doctorsOfContact.Entities != null)
                        {
                            if (doctorsOfContact.Entities.Count > 0)
                            {
                                int length = doctorsOfContact.Entities.Count;

                                if (requestStructure.medication == null)
                                {
                                    requestStructure.medication = new UpdatePatientRequest.Request.Medication();
                                }

                                requestStructure.medication.medics = new UpdatePatientRequest.Request.Medic[length];
                                for (int i = 0; i < length; i++)
                                {
                                    var medic = doctorsOfContact[i];

                                    if (medic.Attributes.Contains(DoctorFields.DoctorIdKey))
                                    {
                                        string id = medic.GetAttributeValue<string>(DoctorFields.DoctorIdKey);

                                        requestStructure.medication.medics[i] = new UpdatePatientRequest.Request.Medic
                                        {
                                            medicid = id,
                                        };
                                    }
                                }
                            }
                        }
                    }

                    if (requestStructure.medication==null)
                    {
                        requestStructure.medication = new UpdatePatientRequest.Request.Medication();
                    }

                    if (requestStructure.medication.products==null)
                    {
                        requestStructure.medication.products = new UpdatePatientRequest.Request.Product[0];
                    }

                    if (requestStructure.medication.medics==null)
                    {
                        requestStructure.medication.medics = new UpdatePatientRequest.Request.Medic[0];
                    }

                    #endregion -> Médicos actuales del contacto N:N
                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());

                requestStructure = null;
                return requestStructure;
            }

            return requestStructure;
        }

        public UpdateAccountRequest.Request GetAccountUpdateStructure(Entity contact, IOrganizationService service, ITracingService trace)
        {
            UpdateAccountRequest.Request requestStructure = new UpdateAccountRequest.Request();

            try
            {
                if (contact != null)
                {
                    #region -> Set request data based on Contact

                    //requestStructure.personalinfo = new UpdatePatientRequest.Request.Personalinfo();

                    if (contact.Attributes.Contains(ContactFields.Firstname))
                        requestStructure.Nombre = contact.GetAttributeValue<string>(ContactFields.Firstname);

                    if (contact.Attributes.Contains(ContactFields.Lastname))
                        requestStructure.Apellido1 = contact.GetAttributeValue<string>(ContactFields.Lastname);

                    if (contact.Attributes.Contains(ContactFields.SecondLastname))
                        requestStructure.Apellido2 = contact.GetAttributeValue<string>(ContactFields.SecondLastname);

                    //if (contact.Attributes.Contains(ContactFields.Id))
                    //    requestStructure. = contact.GetAttributeValue<string>(ContactFields.Id);

                    if (contact.Attributes.Contains(ContactFields.IdAboxPatient))
                        requestStructure.patientId = Convert.ToString(contact.GetAttributeValue<int>(ContactFields.IdAboxPatient));

                    if (contact.Attributes.Contains(ContactFields.Phone))
                        requestStructure.Telefono = Convert.ToString(contact.GetAttributeValue<string>(ContactFields.Phone));

                    if (contact.Attributes.Contains(ContactFields.SecondaryPhone))
                        requestStructure.Telefono2 = Convert.ToString(contact.GetAttributeValue<string>(ContactFields.SecondaryPhone));

                 

                    bool saveWithoutEmail = false;
                    if (contact.Attributes.Contains(ContactFields.NoEmail))
                    {
                        saveWithoutEmail = Convert.ToBoolean(contact.GetAttributeValue<OptionSetValue>(ContactFields.NoEmail).Value);
                    }

                    if (saveWithoutEmail)
                    {
                        //TODO: Cual sera el correo default desde CRM
                        if (contact.Attributes.Contains(ContactFields.Id))
                        {
                            requestStructure.Email = contact.Attributes[ContactFields.Id].ToString() + "_" + Guid.NewGuid().ToString() + Constants.NoEmailDefaultAddress;
                        }
                    }
                    else
                    {
                        if (contact.Attributes.Contains(ContactFields.Email))
                        {
                            requestStructure.Email = contact.Attributes[ContactFields.Email].ToString();
                        }
                    }


                    if (contact.Attributes.Contains(ContactFields.Id))
                        requestStructure.user = contact.Attributes[ContactFields.Id].ToString();

                    if (contact.Attributes.Contains(ContactFields.Province))
                    {
                        EntityReference provinceReference = null;
                        provinceReference = (EntityReference)contact.Attributes[ContactFields.Province];
                        if (provinceReference != null)
                        {
                            var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(ProvinceFields.IdProvince));
                            if (provinceRetrieved.Attributes.Contains(ProvinceFields.IdProvince))
                            {
                                requestStructure.Provincia = provinceRetrieved.GetAttributeValue<string>(ProvinceFields.IdProvince);
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
                                requestStructure.Canton = cantonRetrieved.GetAttributeValue<string>(CantonFields.IdCanton);
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
                                requestStructure.Distrito = districtRetrieved.GetAttributeValue<string>(DistrictFields.IdDistrict);
                            }
                        }
                    }

                    if (contact.Attributes.Contains(ContactFields.Gender))
                    {
                        int val = (contact.GetAttributeValue<OptionSetValue>(ContactFields.Gender)).Value;
                        string gender = sharedMethods.GetGenderValue(val);
                        if (!String.IsNullOrEmpty(gender))
                        {
                            requestStructure.Genero = gender;
                        }
                    }

                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        requestStructure.FechaNacimiento = birthdate.ToString("yyyy-MM-dd");
                    }

                    if (contact.Attributes.Contains(ContactFields.UserType))
                    {
                        EntityReference userTypeReference = null;
                        userTypeReference = (EntityReference)contact.Attributes[ContactFields.UserType];
                        if (userTypeReference != null)
                        {
                            requestStructure.TipoUsuario = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                        }
                    }

                    if (contact.Attributes.Contains(ContactFields.Interests))
                    {
                        var interests = (contact.GetAttributeValue<OptionSetValueCollection>(ContactFields.Interests));
                        requestStructure.interests = new UpdateAccountRequest.Request.Interest[interests.Count];

                        for (int i = 0; i < interests.Count; i++)
                        {
                            string value = "";
                            if (interests[i].Value < 10)
                            {
                                value += "0";
                            }
                            value += interests[i].Value.ToString();

                            if (!String.IsNullOrEmpty(value))
                            {
                                requestStructure.interests[i] = new UpdateAccountRequest.Request.Interest
                                {
                                    interestid = value,
                                    //TODO: Traer cantidad real de relaciones
                                    relations = new UpdateAccountRequest.Request.Relation[] {
                                        new UpdateAccountRequest.Request.Relation
                                        {
                                            relation=new UpdateAccountRequest.Request.Relation1
                                            {
                                                other="",
                                                relationid="Para mí"
                                            }
                                        }
                                    }
                                };
                            }
                        }
                    }

                    #endregion -> Set request data based on Contact

                    #region -> Productos actuales del Contacto 1:N

                    //Obtener los productos y dosis que ya tiene actualmente asignados el contacto

                    string[] doseColumnsToGet = new string[] { DoseFields.Dose, DoseFields.DosexProduct };
                    var doseColumnSet = new ColumnSet(doseColumnsToGet);

                    Guid parentId = contact.Id;
                    var query = new QueryExpression(doseEntity.EntitySingularName);
                    query.Criteria.AddCondition(new ConditionExpression(DoseFields.ContactxDose, ConditionOperator.Equal, parentId));
                    query.ColumnSet = doseColumnSet;
                    var contactDosesList = service.RetrieveMultiple(query);

                    if (contactDosesList != null)
                    {
                        if (contactDosesList.Entities != null)
                        {
                            if (contactDosesList.Entities.Count > 0)
                            {
                                int length = contactDosesList.Entities.Count;
                                requestStructure.medication = new UpdateAccountRequest.Request.Medication();

                                requestStructure.medication.products = new UpdateAccountRequest.Request.Product[length];
                                for (int i = 0; i < length; i++)
                                {
                                    var doseChild = contactDosesList[i];

                                    if (doseChild.Attributes.Contains(DoseFields.DosexProduct))
                                    {
                                        EntityReference productReference = null;
                                        productReference = (EntityReference)doseChild.Attributes[DoseFields.DosexProduct];
                                        if (productReference != null)
                                        {
                                            Entity product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { ProductFields.ProductNumber }));

                                            if (product.Attributes.Contains(ProductFields.ProductNumber))
                                            {
                                                string frequency = "";

                                                if (doseChild.Attributes.Contains(DoseFields.Dose))
                                                {
                                                    int value = (doseChild.GetAttributeValue<OptionSetValue>(DoseFields.Dose)).Value;
                                                    frequency = sharedMethods.GetDoseFrequencyValue(value);
                                                }

                                                requestStructure.medication.products[i] = new UpdateAccountRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(ProductFields.ProductNumber)
                                                };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    

                    #endregion -> Productos actuales del Contacto 1:N

                    #region -> Médicos actuales del contacto N:N

                    //En Dynamics, la relación N:N está definida desde Doctores a Contactos

                    string entity1 = doctorEntity.EntitySingularName;

                    string entity2 = contactEntity.EntitySingularName;

                    string relationshipEntityName = ContactFields.ContactxDoctorRelationship;

                    QueryExpression querym = new QueryExpression(entity1);

                    querym.ColumnSet = new ColumnSet(true);

                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, DoctorFields.EntityId, DoctorFields.EntityId, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);

                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, ContactFields.EntityId, ContactFields.EntityId, JoinOperator.Inner);

                    //linkEntity2.EntityAlias = "alias";

                    linkEntity1.LinkEntities.Add(linkEntity2);

                    querym.LinkEntities.Add(linkEntity1);

                    // Add condition to match

                    linkEntity2.LinkCriteria = new FilterExpression();

                    linkEntity2.LinkCriteria.AddCondition(new ConditionExpression(ContactFields.EntityId, ConditionOperator.Equal, contact.Id));

                    EntityCollection doctorsOfContact = service.RetrieveMultiple(querym);

                    if (doctorsOfContact != null)
                    {
                        if (doctorsOfContact.Entities != null)
                        {
                            if (doctorsOfContact.Entities.Count > 0)
                            {
                                int length = doctorsOfContact.Entities.Count;

                                if (requestStructure.medication == null)
                                {
                                    requestStructure.medication = new UpdateAccountRequest.Request.Medication();
                                }

                                requestStructure.medication.medics = new UpdateAccountRequest.Request.Medic[length];
                                for (int i = 0; i < length; i++)
                                {
                                    var medic = doctorsOfContact[i];

                                    if (medic.Attributes.Contains(DoctorFields.DoctorIdKey))
                                    {
                                        string id = medic.GetAttributeValue<string>(DoctorFields.DoctorIdKey);

                                        requestStructure.medication.medics[i] = new UpdateAccountRequest.Request.Medic
                                        {
                                            medicid = id,
                                        };
                                    }
                                }
                            }
                        }
                    }


                    #endregion -> Médicos actuales del contacto N:N

                    //Si se envia null al servicio da error, se inicializan en arrays vacios

                    if (requestStructure.medication == null)
                    {
                        requestStructure.medication = new UpdateAccountRequest.Request.Medication();
                    }

                    if (requestStructure.medication.medics==null)
                    {
                        requestStructure.medication.medics = new UpdateAccountRequest.Request.Medic[0];
                    }

                    if (requestStructure.medication.products == null)
                    {
                        requestStructure.medication.products = new UpdateAccountRequest.Request.Product[0];
                    }

                }
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                requestStructure = null;
                return requestStructure;
            }

            return requestStructure;
        }

        public QuickSignupRequest.Request GetQuickSignupRequestObject(Entity contact, IOrganizationService service, ITracingService trace)
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

                #endregion Personal Info

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
                        var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(ProvinceFields.IdProvince));
                        if (provinceRetrieved.Attributes.Contains(ProvinceFields.IdProvince))
                        {
                            bool parsed = Int32.TryParse(provinceRetrieved.GetAttributeValue<string>(ProvinceFields.IdProvince), out int aux);

                            if (parsed)
                            {
                                int parsedValue = Int32.Parse(provinceRetrieved.GetAttributeValue<string>(ProvinceFields.IdProvince));
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

                #endregion ContactInfo

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

                #endregion Medication

                return request;
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }

        public PatientSignupRequest.Request GetSignupPatientRequestObject(Entity contact, IOrganizationService service, ITracingService trace)
        {
            var request = new PatientSignupRequest.Request();
            try
            {
                ContactEntity contactEntity = new ContactEntity();
                CantonEntity cantonEntity = new CantonEntity();
                DistrictEntity districtEntity = new DistrictEntity();
                ProvinceEntity provinceEntity = new ProvinceEntity();
                CountryEntity countryEntity = new CountryEntity();

                bool addPatientInChargeInfo = false;

                if (contact.Attributes.Contains(ContactFields.UserType))
                {
                    EntityReference userTypeReference = null;
                    userTypeReference = (EntityReference)contact.Attributes[ContactFields.UserType];
                    string userTypeFromContactBeingCreated = userTypeReference.Id.ToString();
                       // sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());

                    if (userTypeFromContactBeingCreated== Constants.CareTakerIdType || userTypeFromContactBeingCreated == Constants.TutorIdType)
                    {
                        //Un usuario tutor o cuidador, en aboxplan se guarda como user type 01, el que está bajo cuido si se registra como 02 o 03
                        request.userType = "01";
                    }
                    else
                    {
                        request.userType = sharedMethods.GetUserTypeId(userTypeFromContactBeingCreated);

                        if (request.userType == "01" || request.userType == "05")
                            addPatientInChargeInfo = true;
                    }
                }

                bool isChildContact = false;
                if (contact.Attributes.Contains(ContactFields.IsChildContact))
                {
                    isChildContact = contact.GetAttributeValue<bool>(ContactFields.IsChildContact);
                }


                request.personalinfo = new PatientSignupRequest.Request.Personalinfo();
                request.patientincharge = new PatientSignupRequest.Request.Patientincharge();

                #region Personal Info

                if (contact.Attributes.Contains(ContactFields.IdType))
                {
                    request.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                    if (addPatientInChargeInfo)
                        request.patientincharge.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                }

                if (contact.Attributes.Contains(ContactFields.Id))
                {
                    request.personalinfo.id = contact.Attributes[ContactFields.Id].ToString();
                    if (addPatientInChargeInfo)
                        request.patientincharge.id = contact.Attributes[ContactFields.Id].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.Firstname))
                {
                    request.personalinfo.name = contact.Attributes[ContactFields.Firstname].ToString();
                    if (addPatientInChargeInfo)
                        request.patientincharge.name = contact.Attributes[ContactFields.Firstname].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.Lastname))
                {
                    request.personalinfo.lastname = contact.Attributes[ContactFields.Lastname].ToString();
                    if (addPatientInChargeInfo)
                        request.patientincharge.lastname = contact.Attributes[ContactFields.Lastname].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.SecondLastname))
                {
                    request.personalinfo.secondlastname = contact.Attributes[ContactFields.SecondLastname].ToString();
                    if (addPatientInChargeInfo)
                        request.patientincharge.secondlastname = contact.Attributes[ContactFields.SecondLastname].ToString();
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
                        if (addPatientInChargeInfo)
                            request.patientincharge.gender = gender;
                    }
                }



                if (contact.Attributes.Contains(ContactFields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        DateTime today = DateTime.Now;

                        request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");
                        if (addPatientInChargeInfo)
                            request.patientincharge.dateofbirth = birthdate.ToString("yyyy-MM-dd");


                       

                    }
                }

                #endregion Personal Info

                #region ContactInfo

                request.contactinfo = new PatientSignupRequest.Request.Contactinfo();

                if (contact.Attributes.Contains(ContactFields.Phone))
                {
                    request.contactinfo.phone = contact.Attributes[ContactFields.Phone].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.SecondaryPhone))
                {
                    request.contactinfo.mobilephone = contact.Attributes[ContactFields.SecondaryPhone].ToString();
                }



                bool saveWithoutEmail = false;
                if (contact.Attributes.Contains(ContactFields.NoEmail))
                {
                    saveWithoutEmail = Convert.ToBoolean(contact.GetAttributeValue<OptionSetValue>(ContactFields.NoEmail).Value);
                }

                if (saveWithoutEmail)
                {
                    //TODO: Cual sera el correo default desde CRM
                    if (contact.Attributes.Contains(ContactFields.Id))
                    {
                        request.contactinfo.email = contact.Attributes[ContactFields.Id].ToString() + "_" + Guid.NewGuid().ToString() + Constants.NoEmailDefaultAddress;
                    }                 
                }
                else
                {
                    if (contact.Attributes.Contains(ContactFields.Email))
                    {
                        request.contactinfo.email = contact.Attributes[ContactFields.Email].ToString();
                    }
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
                        var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(ProvinceFields.IdProvince));
                        if (provinceRetrieved.Attributes.Contains(ProvinceFields.IdProvince))
                        {
                            string value = provinceRetrieved.GetAttributeValue<string>(ProvinceFields.IdProvince);

                            if (!String.IsNullOrEmpty(value))
                            {
                                request.contactinfo.province = value;
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
                            string value = cantonRetrieved.GetAttributeValue<string>(CantonFields.IdCanton);

                            if (!String.IsNullOrEmpty(value))
                            {
                                request.contactinfo.canton = value;
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
                            string value = districtRetrieved.GetAttributeValue<string>(DistrictFields.IdDistrict);
                            if (!String.IsNullOrEmpty(value))
                            {
                                request.contactinfo.district = value;
                            }
                        }
                    }
                }

                if (contact.Attributes.Contains(ContactFields.Interests))
                {
                    var interests = (contact.GetAttributeValue<OptionSetValueCollection>(ContactFields.Interests));
                    request.interests = new List<PatientSignupRequest.Request.Interest>();

                    for (int i = 0; i < interests.Count; i++)
                    {
                        string value = "";
                        if (interests[i].Value < 10)
                        {
                            value += "0";
                        }
                        value += interests[i].Value.ToString();

                        if (!String.IsNullOrEmpty(value))
                        {
                            request.interests.Add(new PatientSignupRequest.Request.Interest
                            {
                                interestid = value,
                                //TODO: Traer cantidad real de relaciones
                                relations = new List<PatientSignupRequest.Request.Relation> {
                                        new PatientSignupRequest.Request.Relation
                                        {
                                            relation=new PatientSignupRequest.Request.Relation1
                                            {
                                                other="",
                                                relationid="Para mí"
                                            }
                                        }
                                    }
                            });
                        }
                    }
                }

                #endregion ContactInfo


                if (contact.Attributes.Contains(ContactFields.OtherInterestLookup))
                {
                    EntityReference otherInterestReference = null;
                    OtherInterestEntity otherIntEntity = new OtherInterestEntity();
                    otherInterestReference = (EntityReference)contact.Attributes[ContactFields.OtherInterestLookup];
                    if (otherInterestReference != null)
                    {
                        var otherInterestRetrieved = service.Retrieve(otherIntEntity.EntitySingularName, otherInterestReference.Id, new ColumnSet(OtherInterestFields.Id));
                        if (otherInterestRetrieved.Attributes.Contains(OtherInterestFields.Id))
                        {
                            string otherInterestValue = otherInterestRetrieved.GetAttributeValue<string>(OtherInterestFields.Id);

                            if (!String.IsNullOrEmpty(otherInterestValue))
                            {
                                request.otherInterest = otherInterestValue;
                            }
                        }
                    }
                }


                return request;
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }

        public PatientSignupRequest.Request GetSignupPatientUnderCareRequestObject(Entity childContact, Entity parentContact, IOrganizationService service, ITracingService trace)
        {
            var request = new PatientSignupRequest.Request();
            try
            {
                CountryEntity countryEntity = new CountryEntity();

                if (parentContact.Attributes.Contains(ContactFields.UserType))
                {
                    EntityReference userTypeReference = null;
                    userTypeReference = (EntityReference)parentContact.Attributes[ContactFields.UserType];
                    if (userTypeReference != null)
                    {
                        request.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                    }
                }

                if (parentContact.Attributes.Contains(ContactFields.Country))
                {
                    EntityReference countryReference = null;
                    countryReference = (EntityReference)parentContact.Attributes[ContactFields.Country];
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

                request.personalinfo = new PatientSignupRequest.Request.Personalinfo();
                request.patientincharge = new PatientSignupRequest.Request.Patientincharge();

                #region Personal Info

                //Agregar al request la informacion del usuario a cargo
                if (parentContact.Attributes.Contains(ContactFields.Id))
                {
                    string parentId = parentContact.GetAttributeValue<string>(ContactFields.Id);
                    request.personalinfo.id = parentId;
                }

                #endregion Personal Info

                #region Patient In Charge

                if (childContact.Attributes.Contains(ContactFields.IdType))
                {
                    request.patientincharge.idtype = "0" + (childContact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                }

                //TODO: Tomar en cuenta pacientes menores de edad
                if (childContact.Attributes.Contains(ContactFields.Id))
                    request.patientincharge.id = childContact.GetAttributeValue<string>(ContactFields.Id);

                if (childContact.Attributes.Contains(ContactFields.Firstname))
                {
                    request.patientincharge.name = childContact.Attributes[ContactFields.Firstname].ToString();
                }

                if (childContact.Attributes.Contains(ContactFields.Lastname))
                {
                    request.patientincharge.lastname = childContact.Attributes[ContactFields.Lastname].ToString();
                }

                if (childContact.Attributes.Contains(ContactFields.SecondLastname))
                {
                    request.patientincharge.secondlastname = childContact.Attributes[ContactFields.SecondLastname].ToString();
                }

                if (childContact.Attributes.Contains(ContactFields.Gender))
                {
                    int val = (childContact.GetAttributeValue<OptionSetValue>(ContactFields.Gender)).Value;
                    string gender = sharedMethods.GetGenderValue(val);
                    if (!String.IsNullOrEmpty(gender))
                    {
                        request.patientincharge.gender = gender;
                    }
                }

                if (childContact.Attributes.Contains(ContactFields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = childContact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        request.patientincharge.dateofbirth = birthdate.ToString("yyyy-MM-dd");
                    }
                }

                #endregion Patient In Charge

                #region ContactInfo

                request.contactinfo = new PatientSignupRequest.Request.Contactinfo();

                #endregion ContactInfo

                return request;
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }

        public PatientSignupRequest.Request GetWelcomeMailRequestForTutorsAndCaretakers(Entity parentContact, IOrganizationService service, ITracingService trace)
        {
            var request = new PatientSignupRequest.Request();
            try
            {
                CountryEntity countryEntity = new CountryEntity();

                if (parentContact.Attributes.Contains(ContactFields.UserType))
                {
                    EntityReference userTypeReference = null;
                    userTypeReference = (EntityReference)parentContact.Attributes[ContactFields.UserType];
                    if (userTypeReference != null)
                    {
                        request.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                    }
                }

                if (parentContact.Attributes.Contains(ContactFields.Country))
                {
                    EntityReference countryReference = null;
                    countryReference = (EntityReference)parentContact.Attributes[ContactFields.Country];
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

                request.personalinfo = new PatientSignupRequest.Request.Personalinfo();
                request.patientincharge = new PatientSignupRequest.Request.Patientincharge();

                //Agregar al request la informacion del usuario a cargo
                if (parentContact.Attributes.Contains(ContactFields.Id))
                {
                    string parentId = parentContact.GetAttributeValue<string>(ContactFields.Id);
                    request.personalinfo.id = parentId;
                }

                if (parentContact.Attributes.Contains(ContactFields.Firstname))
                    request.personalinfo.name = parentContact.GetAttributeValue<string>(ContactFields.Firstname);

                if (parentContact.Attributes.Contains(ContactFields.Lastname))
                    request.personalinfo.lastname = parentContact.GetAttributeValue<string>(ContactFields.Lastname);

                if (parentContact.Attributes.Contains(ContactFields.SecondLastname))
                    request.personalinfo.secondlastname = parentContact.GetAttributeValue<string>(ContactFields.SecondLastname);

                if (parentContact.Attributes.Contains(ContactFields.Password))
                    request.personalinfo.password = parentContact.GetAttributeValue<string>(ContactFields.Password);

                #region ContactInfo

                request.contactinfo = new PatientSignupRequest.Request.Contactinfo();

                if (parentContact.Attributes.Contains(ContactFields.Email))
                    request.contactinfo.email = parentContact.GetAttributeValue<string>(ContactFields.Email);

                #endregion ContactInfo

                return request;
            }
            catch (Exception ex)
            {
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }
    }
}