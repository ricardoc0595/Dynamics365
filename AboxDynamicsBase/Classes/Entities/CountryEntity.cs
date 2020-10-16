namespace AboxDynamicsBase.Classes.Entities
{
    public class CountryEntity : EntityBase
    {
        //public CountryFields Fields { get; set; }
        public CountrySchemas Schemas { get; set; }

        public CountryEntity()
        {
            //this.Fields = new CountryFields();
            this.Schemas = new CountrySchemas();
            this.EntityPluralName = "new_countries";
            this.EntitySingularName = "new_country";
        }
    }

    public static class CountryFields
    {
        public const string EntityId = "new_countryid";
        public const string IdCountry = "new_idcountry";
        public const string Name = "new_name";
    }

    public class CountrySchemas
    {
        public CountrySchemas()
        {
        }
    }
}