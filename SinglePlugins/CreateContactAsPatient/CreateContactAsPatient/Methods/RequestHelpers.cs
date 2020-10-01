
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
        public RequestHelpers()
        {
            doseEntity = new DoseEntity();
            sharedMethods = new MShared();
            contactEntity = new ContactEntity();
            productEntity = new ProductEntity();
            doctorEntity = new DoctorEntity();
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

                    if (contactDosesList!=null)
                    {
                        if (contactDosesList.Entities!=null)
                        {
                            if (contactDosesList.Entities.Count>0)
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



                    string entity1 = doctorEntity.EntitySingularName;

                    string entity2 = contactEntity.EntitySingularName;

                    string relationshipEntityName = contactEntity.Fields.ContactxDoctorRelationship;

                    QueryExpression querym = new QueryExpression(entity1);

                    querym.ColumnSet = new ColumnSet(true);

                    LinkEntity linkEntity1 = new LinkEntity(entity1, relationshipEntityName, doctorEntity.Fields.EntityId, doctorEntity.Fields.EntityId,Microsoft.Xrm.Sdk.Query.JoinOperator.Inner);

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
                                requestStructure.medication = new UpdatePatientRequest.Request.Medication();

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

    }
}
