namespace AboxDynamicsBase.Classes.Entities
{
    public class ProductEntity : EntityBase
    {
        //public ProductFields Fields { get; set; }
        public ProductSchemas Schemas { get; set; }

        public ProductEntity()
        {
            //this.Fields = new ProductFields();
            this.Schemas = new ProductSchemas();
            this.EntitySingularName = "product";
            this.EntityPluralName = "products";
        }
    }

    public class ProductSchemas
    {
    }

    public static class ProductFields
    {
        public const string IsProductActive = "new_productactive";
        public const string AnnualTherapyMax = "new_productannualtherapymax";
        public const string AnnualTherapyMin = "new_productannualtherapymin";
        public const string ApplyMultiBrandBonus = "new_productapplymultibrandbonus";
        public const string Category = "new_productcategory";
        public const string Code = "new_productcode";
        public const string ProductxContactId = "new_productcontactid";
        public const string Division = "new_productdivision";
        public const string Exchange = "new_productexchange";
        public const string ExchangeEquivalence = "new_productexchangeequivalence";
        public const string Family = "new_productfamily";
        public const string Flavor = "new_productflavor";
        public const string InternalCode = "new_productinternalcode";
        public const string MaxCosumption = "new_productmaxconsumption";
        public const string Packing = "new_productpacking";
        public const string Points = "new_productpoints";
        public const string AboxPharmacyPoints = "new_productpointsaboxpharmacy";
        public const string Blocked = "new_productsblocked";
        public const string Bonus = "new_productsbonus";
        public const string BonusUnits = "new_productsbonusunits";
        public const string Equivalence = "new_productsequivproduct";
        public const string ProductSubProduct = "new_productsubproduct";
        public const string SuggestedPrice = "new_productsuggestedprice";
        public const string TomasEquivalencia = "new_producttomas_unityequivalence";
        public const string ProductIdKey = "productidkey"; // Entity key referring the Product ID saved in Abox database
        public const string ProductNumber = "productnumber"; // Field which contains the Product ID in Abox plan database
        public const string CreatedFromWebAPI = "new_createdfromwebapi";
    }
}