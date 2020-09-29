using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class DoseEntity : EntityBase
    {

        public DoseFields Fields { get; set; }
        public DoseSchemas Schemas { get; set; }


        public DoseEntity()
        {

            this.Fields = new DoseFields();
            this.Schemas = new DoseSchemas();
            this.EntityPluralName = "new_doses";
            this.EntitySingularName = "new_dose";
        }


    }

    public class DoseFields : EntityBase.BaseFields
    {
        public DoseFields()
        {
            EntityId = "new_doseid";
            IdDose = "new_iddose";
            Name = "new_name";
            DosexProduct = "new_productdose";
            Dose = "new_dose";
            ContactxDose = "new_contactdoseid";
            ContactxDoseRelationship = "new_contact_new_dose";
        }

        public string IdDose { get; }
        public string Name { get; }
        public string DosexProduct { get; set; }
        public string Dose { get; }
        public string ContactxDose { get; set; }
        public string ContactxDoseRelationship { get; set; }

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

