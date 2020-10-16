namespace AboxDynamicsBase.Classes.Entities
{
    public class ProvinceEntity : EntityBase
    {
        public ProvinceFields Fields { get; set; }
        public ProvinceSchemas Schemas { get; set; }

        public ProvinceEntity()
        {
            this.Fields = new ProvinceFields();
            this.Schemas = new ProvinceSchemas();
            this.EntityPluralName = "new_cities";
            this.EntitySingularName = "new_city";
        }
    }

    public class ProvinceFields : EntityBase.BaseFields
    {
        public ProvinceFields()
        {
            EntityId = "new_cityid";
            IdProvince = "new_idcity";
            Name = "new_name";
            CountryxProvince = "new_citycountry";
        }

        public string IdProvince { get; }
        public string Name { get; set; }
        public string CountryxProvince { get; set; }
    }

    public class ProvinceSchemas
    {
        public ProvinceSchemas()
        {
            CountryxProvince = "new_CityCountry";
        }

        public string CountryxProvince { get; set; }
    }
}