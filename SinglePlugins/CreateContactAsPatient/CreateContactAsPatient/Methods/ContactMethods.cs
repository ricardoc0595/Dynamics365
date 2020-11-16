using AboxDynamicsBase.Classes.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

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