using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{

    public class CountryEntity : EntityBase
    {

        public CountryFields Fields { get; set; }
        public CountrySchemas Schemas { get; set; }


        public CountryEntity()
        {

            this.Fields = new CountryFields();
            this.Schemas = new CountrySchemas();
            this.EntityPluralName = "new_countries";
            this.EntitySingularName = "new_country";
        }
    }




    public class CountryFields : EntityBase.BaseFields
    {
        public CountryFields()
        {
            EntityId = "new_countryid";
            IdCountry = "new_idcountry";
            Name = "new_name";
            
        }

        public string IdCountry { get; }
        public string Name { get; set; }
        
    }

    public class CountrySchemas
    {
        public CountrySchemas()
        {
            
        }

        
    }

    
}
