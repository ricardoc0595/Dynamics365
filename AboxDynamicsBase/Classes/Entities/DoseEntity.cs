namespace AboxDynamicsBase.Classes.Entities
{
    public class DoseEntity : EntityBase
    {
        //public DoseFields Fields { get; set; }
        public DoseSchemas Schemas { get; set; }

        public DoseEntity()
        {
            //this.Fields = new DoseFields();
            this.Schemas = new DoseSchemas();
            this.EntityPluralName = "new_doses";
            this.EntitySingularName = "new_dose";
        }
    }

    public static class DoseFields
    {
        public const string EntityId = "new_doseid";
        public const string IdDose = "new_iddose";
        public const string Name = "new_name";
        public const string DosexProduct = "new_productdose";
        public const string Dose = "new_dose";
        public const string ContactxDose = "new_contactdoseid";
        public const string ContactxDoseRelationship = "new_contact_new_dose";
    }

    public class DoseSchemas
    {
        public DoseSchemas()
        {
            DosexProduct = "new_ProductDose";
            ContactxDose = "new_ContactDoseId";
        }

        public string DosexProduct { get; set; }
        public string ContactxDose { get; set; }
        public string ContactxDoseRelationship { get; set; }
    }
}