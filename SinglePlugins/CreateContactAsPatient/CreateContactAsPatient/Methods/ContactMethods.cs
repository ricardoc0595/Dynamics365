using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CreateContactAsPatient.Methods
{
    public class ContactMethods
    {
        /// <summary>
        /// Se encarga de obtener una entidad con la información completa de Actualización de datos.
        /// </summary>
        /// <param name="updatedContact">Entidad actualizada con los atributos que cambiaron</param>
        /// <param name="fullContact">Entidad completa</param>
        /// <returns>Entidad preparada para actualizarse mediante los servicios de Abox</returns>
        public Entity GetFullUpdatedContactData(Entity updatedContact, Entity fullContact)
        {
            return null;
        }

        public List<string> GetEntityValidationStatus(Entity contact, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            List<string> validationMessages = new List<string>();
          
            MShared sharedMethods = new MShared();
            try
            {

                bool isChildContact = false;
                if (contact.Attributes.Contains(ContactFields.IsChildContact))
                {
                    isChildContact = contact.GetAttributeValue<bool>(ContactFields.IsChildContact);
                }

                if (contact.Attributes.Contains(ContactFields.Id))
                {
                    
                    if (contact.Attributes.Contains(ContactFields.IdType))
                    {
                        int idType = contact.GetAttributeValue<OptionSetValue>(ContactFields.IdType).Value;

                        if (idType != Constants.ForeignerIdValue && idType != Constants.MinorIdValue)
                        {
                            if (!sharedMethods.IsValidNumericID(contact.Attributes[ContactFields.Id].ToString(), trace))
                            {
                                validationMessages.Add(ValidationMessages.Contact.IdentificationFormat);
                            }
                        }
                        else if (idType == Constants.ForeignerIdValue)
                        {
                            if (!sharedMethods.IsAlphanumeric(contact.Attributes[ContactFields.Id].ToString(), trace))
                            {
                                validationMessages.Add(ValidationMessages.Contact.AlphanumericForeignerIdFormat+" en el campo Cédula");
                            }
                        }
                    }
                   

                }

                

                if (contact.Attributes.Contains(ContactFields.Firstname))
                {
                    
                    if (!sharedMethods.IsValidName(contact.Attributes[ContactFields.Firstname].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.ValidName} en el campo Nombre");
                    }

                    if (!sharedMethods.IsValidMaxLength(contact.Attributes[ContactFields.Firstname].ToString(),Constants.MaxNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MaxLengthFormat,Constants.MaxNameLength)+" en el campo Nombre");
                    }

                    if (!sharedMethods.IsValidMinLength(contact.Attributes[ContactFields.Firstname].ToString(), Constants.MinNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MinLengthFormat, Constants.MinNameLength) + " en el campo Nombre");
                    }

                }

                if (contact.Attributes.Contains(ContactFields.Lastname))
                {
                    
                    if (!sharedMethods.IsValidLastname(contact.Attributes[ContactFields.Lastname].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.ValidName} en el campo Primer Apellido");
                    }

                    if (!sharedMethods.IsValidMaxLength(contact.Attributes[ContactFields.Lastname].ToString(), Constants.MaxNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MaxLengthFormat, Constants.MaxNameLength) + " en el campo Primer Apellido");
                    }

                    if (!sharedMethods.IsValidMinLength(contact.Attributes[ContactFields.Lastname].ToString(), Constants.MinNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MinLengthFormat, Constants.MinNameLength) + " en el campo Primer Apellido");
                    }

                }

                if (contact.Attributes.Contains(ContactFields.SecondLastname))
                {
                    
                    if (!sharedMethods.IsValidLastname(contact.Attributes[ContactFields.SecondLastname].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.ValidName} en el campo Segundo Apellido");
                    }

                    if (!sharedMethods.IsValidMaxLength(contact.Attributes[ContactFields.SecondLastname].ToString(), Constants.MaxNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MaxLengthFormat, Constants.MaxNameLength) + " en el campo Segundo Apellido");
                    }

                    if (!sharedMethods.IsValidMinLength(contact.Attributes[ContactFields.SecondLastname].ToString(), Constants.MinNameLength, trace))
                    {
                        validationMessages.Add(String.Format(ValidationMessages.Contact.MinLengthFormat, Constants.MinNameLength) + " en el campo Segundo Apellido");
                    }

                }

                if (contact.Attributes.Contains(ContactFields.Password))
                {

                    if (!isChildContact && !sharedMethods.IsValidPassword(contact.Attributes[ContactFields.Password].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.PasswordFormat}");
                    }

                }

                if (contact.Attributes.Contains(ContactFields.Birthdate))
                {
                    DateTime birthdate = new DateTime();
                    birthdate = contact.GetAttributeValue<DateTime>(ContactFields.Birthdate);
                    if (birthdate != null)
                    {
                        DateTime today = DateTime.Now;

                        if (sharedMethods.GetAge(birthdate) < 12 && !isChildContact)
                        {
                            validationMessages.Add(ValidationMessages.Contact.AgeLimit);
                        }
                    }
                }


                if (contact.Attributes.Contains(ContactFields.Phone))
                {
                    //Los contactos bajo cuido no validan este campo pues no lo tienen disponible, esta informacion la tiene le paciente a cargo de ellos
                    if (!isChildContact && !sharedMethods.HasOnlyNumbers(contact.Attributes[ContactFields.Phone].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.OnlyNumbers} en el campo Teléfono");
                    }
                }

                if (contact.Attributes.Contains(ContactFields.SecondaryPhone))
                {
                    //Los contactos bajo cuido no validan este campo pues no lo tienen disponible, esta informacion la tiene le paciente a cargo de ellos
                    if (!isChildContact && !sharedMethods.HasOnlyNumbers(contact.Attributes[ContactFields.SecondaryPhone].ToString(), trace))
                    {
                        validationMessages.Add($"{ValidationMessages.Contact.OnlyNumbers} en el campo Teléfono Opcional");
                    }
                }

                //if (contact.Attributes.Contains(ContactFields.Email))
                //{
                //    if (!sharedMethods.IsValidEmail(contact.Attributes[ContactFields.Email].ToString(), trace))
                //    {
                //        validationMessages.Add($"{ValidationMessages.Contact.OnlyNumbers} en el campo Teléfono Opcional");
                //    }
                //}

                return validationMessages;
            }
            catch (Exception ex)
            {
                sharedMethods.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando los datos de la entidad.",
                    ProcessId = ""
                }, trace);
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
               
                throw ex;
            }

        }

        public EntityCollection GetContactChildContacts(Entity contact, IOrganizationService service)
        {
            ContactEntity contactEntity = new ContactEntity();
            try
            {
                //Obtener los productos y dosis que ya tiene actualmente asignados el contacto
                string[] columnsToGet = new string[] { ContactFields.IdAboxPatient };
                var contactColumnSet = new ColumnSet(columnsToGet);
                Guid parentId = contact.Id;
                var query = new QueryExpression(contactEntity.EntitySingularName);
                query.Criteria.AddCondition(new ConditionExpression(ContactFields.ContactxContactLookup, ConditionOperator.Equal, parentId));
                query.ColumnSet = contactColumnSet;
                EntityCollection contactChildrenRecords = service.RetrieveMultiple(query);

                return contactChildrenRecords;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }
    }
}