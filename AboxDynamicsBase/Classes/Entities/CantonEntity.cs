namespace AboxDynamicsBase.Classes.Entities
{
    public class CantonEntity : EntityBase
    {
        //public CantonFields Fields { get; set; }
        public CantonSchemas Schemas { get; set; }

        public CantonEntity()
        {
            //this.Fields = new CantonFields();
            this.Schemas = new CantonSchemas();
            this.EntityPluralName = "new_cantons";
            this.EntitySingularName = "new_canton";
        }
    }

    public static class CantonFields
    {
        public const string EntityId = "new_cantonid";
        public const string IdCanton = "new_idcanton";
        public const string Name = "new_name";
        public const string ProvincexCanton = "new_provinciacanton";
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