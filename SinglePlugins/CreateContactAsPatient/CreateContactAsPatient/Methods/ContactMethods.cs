
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


    }
}
