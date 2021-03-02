using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class InvoiceUpdate
    {
        public string country { get; set; }
        public string idFromDatabase { get; set; }
        public int patientId { get; set; }
        public string pharmacyId { get; set; }
        public string billId { get; set; }
        public string billDate { get; set; }
        public string billImageUrl { get; set; }
        public Product[] products { get; set; }
        public string status { get; set; }
        public string statusReason { get; set; }
        public int? totalAmount { get; set; }
        public int? revisionTime1 { get; set; }
        public int? revisionTime2 { get; set; }
        public string purchaseMethod { get; set; }
        public NonAboxProduct[] nonAboxProducts { get; set; }

        public class Product
        {
            public string id { get; set; }
            public int quantity { get; set; }
            public int price { get; set; }
        }

        public class NonAboxProduct
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public int price { get; set; }
        }


    }
}