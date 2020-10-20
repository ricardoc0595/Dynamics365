﻿
using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes.Entities;
using CreateContactAsPatient.Classes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public UpdatePatientRequest.Request GetPatientUpdateStructure(Entity contact, IOrganizationService service)
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
                    #endregion




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
                                                    var optionSet= doseChild.GetAttributeValue<OptionSetValue>(DoseFields.Dose);
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








                    #endregion


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


                    #endregion

                }
            }
            catch (Exception ex)
            {
                requestStructure = null;
                return requestStructure;
            }

            return requestStructure;

        }

        public UpdateAccountRequest.Request GetAccountUpdateStructure(Entity contact, IOrganizationService service)
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

                    if (contact.Attributes.Contains(ContactFields.Email))
                        requestStructure.Email = contact.Attributes[ContactFields.Email].ToString();

                    if (contact.Attributes.Contains(ContactFields.Id))
                        requestStructure.user = contact.Attributes[ContactFields.Id].ToString();


                    if (contact.Attributes.Contains(ContactFields.Province))
                    {
                        EntityReference provinceReference = null;
                        provinceReference = (EntityReference)contact.Attributes[ContactFields.Province];
                        if (provinceReference != null)
                        {

                            var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(provinceEntity.Fields.IdProvince));
                            if (provinceRetrieved.Attributes.Contains(provinceEntity.Fields.IdProvince))
                            {
                                requestStructure.Provincia = provinceRetrieved.GetAttributeValue<string>(provinceEntity.Fields.IdProvince);
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
                            string value = interests[i].Value.ToString();

                            if (!String.IsNullOrEmpty(value))
                            {
                                requestStructure.interests[i] = new UpdateAccountRequest.Request.Interest {
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



                    #endregion




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
                                                    frequency = doseChild.GetAttributeValue<string>(DoseFields.Dose);
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








                    #endregion


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


                    #endregion

                }
            }
            catch (Exception ex)
            {
                requestStructure = null;
                return requestStructure;
            }

            return requestStructure;

        }

        public QuickSignupRequest.Request GetQuickSignupRequestObject(Entity contact, IOrganizationService service)
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

        public PatientSignupRequest.Request GetSignupPatientRequestObject(Entity contact, IOrganizationService service)
        {
            var request = new PatientSignupRequest.Request();
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


                request.personalinfo = new PatientSignupRequest.Request.Personalinfo();
                request.patientincharge = new PatientSignupRequest.Request.Patientincharge();
                #region Personal Info

                if (contact.Attributes.Contains(ContactFields.IdType))
                {
                    request.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                    request.patientincharge.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType)).Value;
                }



                if (contact.Attributes.Contains(ContactFields.Id))
                {
                    request.personalinfo.id = contact.Attributes[ContactFields.Id].ToString();
                    request.patientincharge.id = contact.Attributes[ContactFields.Id].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.Firstname))
                {
                    request.personalinfo.name = contact.Attributes[ContactFields.Firstname].ToString();
                    request.patientincharge.name = contact.Attributes[ContactFields.Firstname].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.Lastname))
                {
                    request.personalinfo.lastname = contact.Attributes[ContactFields.Lastname].ToString();
                    request.patientincharge.lastname= contact.Attributes[ContactFields.Lastname].ToString();
                }

                if (contact.Attributes.Contains(ContactFields.SecondLastname))
                {
                    request.personalinfo.secondlastname = contact.Attributes[ContactFields.SecondLastname].ToString();
                    request.patientincharge.secondlastname= contact.Attributes[ContactFields.SecondLastname].ToString();
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
                        request.patientincharge.gender = gender;
                    }



                }

                if (contact.Attributes.Contains(ContactFields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        request.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");
                        request.patientincharge.dateofbirth = birthdate.ToString("yyyy-MM-dd"); 
                    }
                }


                #endregion

                #region ContactInfo

                request.contactinfo = new PatientSignupRequest.Request.Contactinfo();


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

                            string value = provinceRetrieved.GetAttributeValue<string>(provinceEntity.Fields.IdProvince);

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

                            string value= cantonRetrieved.GetAttributeValue<string>(CantonFields.IdCanton);

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
                        string value = interests[i].Value.ToString();

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

    }
}
