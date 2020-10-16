namespace AboxDynamicsBase.Classes.Entities
{
    public class DistrictEntity : EntityBase
    {
        // public DistrictFields Fields { get; set; }
        public DistrictSchemas Schemas { get; set; }

        public DistrictEntity()
        {
            //this.Fields = new DistrictFields();
            this.Schemas = new DistrictSchemas();
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

    public class DistrictSchemas
    {
        public DistrictSchemas()
        {
            CantonxDistrict = "new_DistritCity";
        }

        public string CantonxDistrict { get; set; }
    }
}