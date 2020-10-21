namespace AboxCrmPlugins.Classes.Entities
{
    public class EntityBase
    {
        public string EntityName { get; set; }

        public class BaseFields
        {
            //TODO: Buscar forma de identificar el usuario que hace el llamado para que en los plugin se sepa que es
            //desde web api y no ejecutar el plugin.
            public string CreatedFromWebAPI { get; set; }
        }
    }
}