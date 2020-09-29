using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class CantonEntity:EntityBase
    {

       
        public CantonFields Fields { get; set; }
        public CantonSchemas Schemas { get; set; }


        public CantonEntity()
        {

            this.Fields = new CantonFields();
            this.Schemas = new CantonSchemas();
            this.EntityPluralName = "new_cantons";
            this.EntitySingularName = "new_canton";
        }

    }




    public class CantonFields :EntityBase.BaseFields
    {
        public CantonFields()
        {
            EntityId = "new_cantonid";
            IdCanton = "new_idcanton";
            Name = "new_name";
            ProvincexCanton = "new_provinciacanton";
        }

        public string IdCanton { get;  }
        public string Name { get; set; }
        public string ProvincexCanton { get; set; }
    }

    public class CantonSchemas
    {
        public CantonSchemas()
        {
            ProvincexCanton = "new_ProvinciaCanton";
        }

        public string ProvincexCanton { get; set; }
    }

}
