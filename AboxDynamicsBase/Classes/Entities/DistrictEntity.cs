namespace AboxDynamicsBase.Classes.Entities
{
    public class DistrictEntity : EntityBase
    {
        public DistrictEntity()
        {
            //this.Fields = new DistrictFields();

            this.EntityPluralName = "new_distrits";
            this.EntitySingularName = "new_distrit";
        }
    }

    public class DistrictFields
    {
        public const string EntityId = "new_distritid";
        public const string IdDistrict = "new_iddistrit";
        public const string Name = "new_name";
        public const string CantonxDistrict = "new_distritcity";
    }

    public static class DistrictSchemas
    {
        public const string CantonxDistrict = "new_DistritCity";
    }
}