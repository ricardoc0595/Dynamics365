namespace AboxDynamicsBase.Classes.Entities
{
    public class ProvinceEntity : EntityBase
    {
        public ProvinceEntity()
        {
            this.EntityPluralName = "new_cities";
            this.EntitySingularName = "new_city";
        }
    }

    public static class ProvinceFields
    {
        public const string EntityId = "new_cityid";
        public const string IdProvince = "new_idcity";
        public const string Name = "new_name";
        public const string CountryxProvince = "new_citycountry";
    }

    public static class ProvinceSchemas
    {
        public const string CountryxProvince = "new_CityCountry";
    }
}