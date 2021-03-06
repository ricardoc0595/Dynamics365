﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class InvoiceEntity : EntityBase
    {

        public InvoiceEntity()
        {
            this.EntityPluralName = "invoices";
            this.EntitySingularName = "invoice";
        }

    }

    public static class InvoiceSchemas
    {
        public const string Pharmacy = "new_Pharmacy";
        public const string Country = "new_InvoiceCountry";
        public const string CustomerIdSchema = "customerid_contact";
        public const string CustomerContactSchema = "new_Customer_contact";
    }

    public static class InvoiceFields
    {
        public const string InvoiceImageWebResource = "WebResource_invoiceimage";
        public const string InvoiceImageWebUrl = "new_aboximageurl";
        public const string InvoiceNumber = "new_invoicenumber";
        public const string CaseInvoiceLookup = "new_caseinvoiceid";
        public const string Customer = "customerid";
        public const string Contact = "new_contactid";
        public const string Country = "new_invoicecountry";
        public const string Pharmacy = "new_pharmacy";
        public const string PurchaseDate = "new_purchasedate";
        public const string EntityId = "invoiceid";
        public const string ProductSelectionWebResource = "WebResource_invoiceproductselection";
        public const string ProductsSelectedJson = "new_productsselectedjson";
        public const string IdAboxInvoice="new_idaboxinvoice";
        public const string InvoiceXContactRelationship = "invoice_customer_contacts";
        public const string StatusCode      = "new_invoicestatus";
        public const string StatusReason    = "new_statusreason";
        public const string TotalAmount     = "new_totalamount";
        public const string RevisionTime1   = "new_revisiontime1";
        public const string RevisionTime2   = "new_revisiontime2";
        public const string PurchaseMethod  = "new_purchasemethod";
        public const string NonAboxProductsSelectedJson = "new_nonaboxproductsselectedjson";
    }


}
