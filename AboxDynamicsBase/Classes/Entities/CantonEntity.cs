namespace AboxDynamicsBase.Classes.Entities
{
    public class CantonEntity : EntityBase
    {
        //public CantonFields Fields { get; set; }

        public CantonEntity()
        {
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

    public static class CantonSchemas
    {
        public const string ProvincexCanton = "new_ProvinciaCanton";
    }
}