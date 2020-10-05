
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

                    if (contact.Attributes.Contains(contactEntity.Fields.Firstname))
                        requestStructure.personalinfo.name = contact.GetAttributeValue<string>(contactEntity.Fields.Firstname);

                    if (contact.Attributes.Contains(contactEntity.Fields.Lastname))
                        requestStructure.personalinfo.lastname = contact.GetAttributeValue<string>(contactEntity.Fields.Lastname);

                    if (contact.Attributes.Contains(contactEntity.Fields.SecondLastname))
                        requestStructure.personalinfo.secondlastname = contact.GetAttributeValue<string>(contactEntity.Fields.SecondLastname);

                    if (contact.Attributes.Contains(contactEntity.Fields.Id))
                        requestStructure.personalinfo.id = contact.GetAttributeValue<string>(contactEntity.Fields.Id);

                    if (contact.Attributes.Contains(contactEntity.Fields.IdAboxPatient))
                        requestStructure.patientid = Convert.ToString(contact.GetAttributeValue<int>(contactEntity.Fields.IdAboxPatient));



                    if (contact.Attributes.Contains(contactEntity.Fields.Gender))
                    {
                        int val = (contact.GetAttributeValue<OptionSetValue>(contactEntity.Fields.Gender)).Value;
                        string gender = sharedMethods.GetGenderValue(val);
                        if (!String.IsNullOrEmpty(gender))
                        {
                            requestStructure.personalinfo.gender = gender;

                        }
                    }

                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(contactEntity.Fields.Birthdate);
                    if (birthdate != null)
                    {
                        requestStructure.personalinfo.dateofbirth = birthdate.ToString("yyyy-MM-dd");

                    }

                    if (contact.Attributes.Contains(contactEntity.Fields.IdType))
                    {
                        requestStructure.personalinfo.idtype = "0" + (contact.GetAttributeValue<OptionSetValue>(contactEntity.Fields.IdType)).Value;
                    }


                    if (contact.Attributes.Contains(contactEntity.Fields.UserType))
                    {
                        EntityReference userTypeReference = null;
                        userTypeReference = (EntityReference)contact.Attributes[contactEntity.Fields.UserType];
                        if (userTypeReference != null)
                        {
                            requestStructure.userType = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                        }
                    }



                    if (contact.Attributes.Contains(contactEntity.Fields.Country))
                    {
                        EntityReference countryReference = null;
                        CountryEntity countryEntity = new CountryEntity();
                        countryReference = (EntityReference)contact.Attributes[contactEntity.Fields.Country];
                        if (countryReference != null)
                        {

                            var countryRetrieved = service.Retrieve(countryEntity.EntitySingularName, countryReference.Id, new ColumnSet(countryEntity.Fields.IdCountry));
                            if (countryRetrieved.Attributes.Contains(countryEntity.Fields.IdCountry))
                            {

                                string country = countryRetrieved.GetAttributeValue<string>(countryEntity.Fields.IdCountry);

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


                    string[] doseColumnsToGet = new string[] { doseEntity.Fields.Dose, doseEntity.Fields.DosexProduct };
                    var doseColumnSet = new ColumnSet(doseColumnsToGet);

                    Guid parentId = contact.Id;
                    var query = new QueryExpression(doseEntity.EntitySingularName);
                    query.Criteria.AddCondition(new ConditionExpression(doseEntity.Fields.ContactxDose, ConditionOperator.Equal, parentId));
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

                                    if (doseChild.Attributes.Contains(doseEntity.Fields.DosexProduct))
                                    {
                                        EntityReference productReference = null;
                                        productReference = (EntityReference)doseChild.Attributes[doseEntity.Fields.DosexProduct];
                                        if (productReference != null)
                                        {

                                            Entity product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { productEntity.Fields.ProductNumber }));

                                            if (product.Attributes.Contains(productEntity.Fields.ProductNumber))
                                            {
                                                string frequency = "";

                                                if (doseChild.Attributes.Contains(doseEntity.Fields.Dose))
                                                {
                                                    frequency = doseChild.GetAttributeValue<string>(doseEntity.Fields.Dose);
                                                }


                                                requestStructure.medication.products[i] = new UpdatePatientRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(productEntity.Fields.ProductNumber)
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

                    string relationshipEntityName = contactEntity.Fields.ContactxDoctorRelationship;

                    QueryExpression querym = new QueryExpression(entity1);

                    querym.ColumnSet = new ColumnSet(true);

                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, doctorEntity.Fields.EntityId, doctorEntity.Fields.EntityId, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);

                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, contactEntity.Fields.EntityId, contactEntity.Fields.EntityId, JoinOperator.Inner);

                    //linkEntity2.EntityAlias = "alias";

                    linkEntity1.LinkEntities.Add(linkEntity2);

                    querym.LinkEntities.Add(linkEntity1);

                    // Add condition to match 

                    linkEntity2.LinkCriteria = new FilterExpression();

                    linkEntity2.LinkCriteria.AddCondition(new ConditionExpression(contactEntity.Fields.EntityId, ConditionOperator.Equal, contact.Id));

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


                                    if (medic.Attributes.Contains(doctorEntity.Fields.DoctorIdKey))
                                    {
                                        string id = medic.GetAttributeValue<string>(doctorEntity.Fields.DoctorIdKey);


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

                    if (contact.Attributes.Contains(contactEntity.Fields.Firstname))
                        requestStructure.Nombre = contact.GetAttributeValue<string>(contactEntity.Fields.Firstname);

                    if (contact.Attributes.Contains(contactEntity.Fields.Lastname))
                        requestStructure.Apellido1 = contact.GetAttributeValue<string>(contactEntity.Fields.Lastname);

                    if (contact.Attributes.Contains(contactEntity.Fields.SecondLastname))
                        requestStructure.Apellido2 = contact.GetAttributeValue<string>(contactEntity.Fields.SecondLastname);

                    //if (contact.Attributes.Contains(contactEntity.Fields.Id))
                    //    requestStructure. = contact.GetAttributeValue<string>(contactEntity.Fields.Id);

                    if (contact.Attributes.Contains(contactEntity.Fields.IdAboxPatient))
                        requestStructure.patientId = Convert.ToString(contact.GetAttributeValue<int>(contactEntity.Fields.IdAboxPatient));

                    if (contact.Attributes.Contains(contactEntity.Fields.Phone))
                        requestStructure.Telefono = Convert.ToString(contact.GetAttributeValue<string>(contactEntity.Fields.Phone));

                    if (contact.Attributes.Contains(contactEntity.Fields.SecondaryPhone))
                        requestStructure.Telefono2 = Convert.ToString(contact.GetAttributeValue<string>(contactEntity.Fields.SecondaryPhone));

                    if (contact.Attributes.Contains(contactEntity.Fields.Email))
                        requestStructure.Email = contact.Attributes[contactEntity.Fields.Email].ToString();

                    if (contact.Attributes.Contains(contactEntity.Fields.Id))
                        requestStructure.user = contact.Attributes[contactEntity.Fields.Id].ToString();


                    if (contact.Attributes.Contains(contactEntity.Fields.Province))
                    {
                        EntityReference provinceReference = null;
                        provinceReference = (EntityReference)contact.Attributes[contactEntity.Fields.Province];
                        if (provinceReference != null)
                        {

                            var provinceRetrieved = service.Retrieve(provinceEntity.EntitySingularName, provinceReference.Id, new ColumnSet(provinceEntity.Fields.IdProvince));
                            if (provinceRetrieved.Attributes.Contains(provinceEntity.Fields.IdProvince))
                            {
                                requestStructure.Provincia = provinceRetrieved.GetAttributeValue<string>(provinceEntity.Fields.IdProvince);
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
                                requestStructure.Canton = cantonRetrieved.GetAttributeValue<string>(cantonEntity.Fields.IdCanton);
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
                                requestStructure.Distrito = districtRetrieved.GetAttributeValue<string>(districtEntity.Fields.IdDistrict);
                            }
                        }
                    }




                    if (contact.Attributes.Contains(contactEntity.Fields.Gender))
                    {
                        int val = (contact.GetAttributeValue<OptionSetValue>(contactEntity.Fields.Gender)).Value;
                        string gender = sharedMethods.GetGenderValue(val);
                        if (!String.IsNullOrEmpty(gender))
                        {
                            requestStructure.Genero = gender;

                        }
                    }

                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(contactEntity.Fields.Birthdate);
                    if (birthdate != null)
                    {
                        requestStructure.FechaNacimiento = birthdate.ToString("yyyy-MM-dd");

                    }



                    if (contact.Attributes.Contains(contactEntity.Fields.UserType))
                    {
                        EntityReference userTypeReference = null;
                        userTypeReference = (EntityReference)contact.Attributes[contactEntity.Fields.UserType];
                        if (userTypeReference != null)
                        {
                            requestStructure.TipoUsuario = sharedMethods.GetUserTypeId(userTypeReference.Id.ToString());
                        }
                    }



                    if (contact.Attributes.Contains(contactEntity.Fields.Interests))
                    {
                        var interests = (contact.GetAttributeValue<OptionSetValueCollection>(contactEntity.Fields.Interests));
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


                    string[] doseColumnsToGet = new string[] { doseEntity.Fields.Dose, doseEntity.Fields.DosexProduct };
                    var doseColumnSet = new ColumnSet(doseColumnsToGet);

                    Guid parentId = contact.Id;
                    var query = new QueryExpression(doseEntity.EntitySingularName);
                    query.Criteria.AddCondition(new ConditionExpression(doseEntity.Fields.ContactxDose, ConditionOperator.Equal, parentId));
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

                                    if (doseChild.Attributes.Contains(doseEntity.Fields.DosexProduct))
                                    {
                                        EntityReference productReference = null;
                                        productReference = (EntityReference)doseChild.Attributes[doseEntity.Fields.DosexProduct];
                                        if (productReference != null)
                                        {

                                            Entity product = service.Retrieve(productEntity.EntitySingularName, productReference.Id, new ColumnSet(new string[] { productEntity.Fields.ProductNumber }));

                                            if (product.Attributes.Contains(productEntity.Fields.ProductNumber))
                                            {
                                                string frequency = "";

                                                if (doseChild.Attributes.Contains(doseEntity.Fields.Dose))
                                                {
                                                    frequency = doseChild.GetAttributeValue<string>(doseEntity.Fields.Dose);
                                                }


                                                requestStructure.medication.products[i] = new UpdateAccountRequest.Request.Product
                                                {
                                                    frequency = frequency,
                                                    productid = product.GetAttributeValue<string>(productEntity.Fields.ProductNumber)
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

                    string relationshipEntityName = contactEntity.Fields.ContactxDoctorRelationship;

                    QueryExpression querym = new QueryExpression(entity1);

                    querym.ColumnSet = new ColumnSet(true);

                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, doctorEntity.Fields.EntityId, doctorEntity.Fields.EntityId, Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);

                    LinkEntity linkEntity2 = new LinkEntity(relationshipEntityName, entity2, contactEntity.Fields.EntityId, contactEntity.Fields.EntityId, JoinOperator.Inner);

                    //linkEntity2.EntityAlias = "alias";

                    linkEntity1.LinkEntities.Add(linkEntity2);

                    querym.LinkEntities.Add(linkEntity1);

                    // Add condition to match 

                    linkEntity2.LinkCriteria = new FilterExpression();

                    linkEntity2.LinkCriteria.AddCondition(new ConditionExpression(contactEntity.Fields.EntityId, ConditionOperator.Equal, contact.Id));

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


                                    if (medic.Attributes.Contains(doctorEntity.Fields.DoctorIdKey))
                                    {
                                        string id = medic.GetAttributeValue<string>(doctorEntity.Fields.DoctorIdKey);


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



    }
}
