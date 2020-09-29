using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class EntityBase
    {
        public string EntitySingularName { get; set; }
        public string EntityPluralName { get; set; }
        public string EntityId { get; set; }
        public class BaseFields
        {
            //TODO: Buscar forma de identificar el usuario que hace el llamado para que en los plugin se sepa que es
            //desde web api y no ejecutar el plugin.
            public string CreatedFromWebAPI { get; set; }
            public string EntityId { get; set; }


        }

    }
}
