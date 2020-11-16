namespace AboxDynamicsBase.Classes.Entities
{
    public class EntityBase
    {
        public string EntitySingularName { get; set; }
        public string EntityPluralName { get; set; }
        public string EntityId { get; set; }

        public class BaseFields
        {
            public string CreatedFromWebAPI { get; set; }

            public string EntityId { get; set; }
        }
    }
}