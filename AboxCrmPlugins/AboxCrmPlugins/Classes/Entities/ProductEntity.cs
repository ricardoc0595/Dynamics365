using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxCrmPlugins.Classes.Entities
{
    public class ProductEntity : EntityBase
    {
        public ProductFields Fields { get; set; }
        public ProductEntity()
        {
            this.Fields = new ProductFields();





            this.EntityName = "products";
        }
    }

    public class ProductFields : EntityBase.BaseFields
    {
        public ProductFields()
        {
            this.IsProductActive = "new_productactive";
            this.AnnualTherapyMax = "new_productannualtherapymax";
            this.AnnualTherapyMin = "new_productannualtherapymin";
            this.ApplyMultiBrandBonus = "new_productapplymultibrandbonus";
            this.Category = "new_productcategory";
            this.Code = "new_productcode";
            this.ProductxContactId = "new_productcontactid";
            this.Division = "new_productdivision";
            this.Exchange = "new_productexchange";
            this.ExchangeEquivalence = "new_productexchangeequivalence";
            this.Family = "new_productfamily";
            this.Flavor = "new_productflavor";
            this.InternalCode = "new_productinternalcode";
            this.MaxCosumption = "new_productmaxconsumption";
            this.Packing = "new_productpacking";
            this.Points = "new_productpoints";
            this.AboxPharmacyPoints = "new_productpointsaboxpharmacy";
            this.Blocked = "new_productsblocked";
            this.Bonus = "new_productsbonus";
            this.BonusUnits = "new_productsbonusunits";
            this.Equivalence = "new_productsequivproduct";
            this.ProductSubProduct = "new_productsubproduct";
            this.SuggestedPrice = "new_productsuggestedprice";
            this.TomasEquivalencia = "new_producttomas_unityequivalence";
            this.ProductIdKey = "productidkey"; // Entity key referring the Product ID saved in Abox database
            this.ProductNumber = "productnumber"; // Field which contains the Product ID in Abox plan database
            this.CreatedFromWebAPI = "new_createdfromwebapi";
        }

        public string IsProductActive { get; }
        public string AnnualTherapyMax { get; }
        public string AnnualTherapyMin { get; }
        public string ApplyMultiBrandBonus { get; }
        public string Category { get; }
        public string Code { get; }
        public string ProductxContactId { get; }
        public string Division { get; }
        
        public string Exchange { get; }
        public string ExchangeEquivalence { get; }
        public string Family { get; }
        public string Flavor { get; }
        public string InternalCode { get; }
        public string MaxCosumption { get; }
        public string Packing { get; }
        public string Points { get;  }
        public string AboxPharmacyPoints { get;}
        public string Blocked { get;  }
        public string Bonus { get;  }
        public string BonusUnits { get;  }
        public string Equivalence { get; set; }
        public string ProductSubProduct { get; }
        public string SuggestedPrice { get;  }
        public string TomasEquivalencia { get;  }
        public string ProductIdKey { get;  }
        public string ProductNumber { get; }

    }
}
