using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class DistrictEntity :EntityBase
    {

        public DistrictFields Fields { get; set; }
        public DistrictSchemas Schemas { get; set; }


        public DistrictEntity()
        {

            this.Fields = new DistrictFields();
            this.Schemas = new DistrictSchemas();
            this.EntityPluralName = "new_distrits";
            this.EntitySingularName = "new_distrit";
        }

    }


    public class DistrictFields : EntityBase.BaseFields
    {
        public DistrictFields()
        {
            EntityId = "new_distritid";
            IdDistrict = "new_iddistrit";
            Name = "new_name";
            CantonxDistrict = "new_distritcity";
        }

        public string IdDistrict { get; }
        public string Name { get; set; }
        public string CantonxDistrict { get; set; }
    }

    public class DistrictSchemas
    {
        public DistrictSchemas()
        {
            CantonxDistrict = "new_DistritCity";
        }

        public string CantonxDistrict { get; set; }
    }


}
