﻿namespace AboxDynamicsBase.Classes.Entities
{
    public class DoseEntity : EntityBase
    {
        public DoseEntity()
        {
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
        public const string DosexProduct = "new_ProductDose";
        public const string ContactxDose = "new_ContactDoseId";
    }
}