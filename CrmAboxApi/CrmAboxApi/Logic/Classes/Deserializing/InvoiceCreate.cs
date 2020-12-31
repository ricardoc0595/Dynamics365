using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class InvoiceCreate
    {
        public string country { get; set; }
        public string idFromDatabase { get; set; }
        public int patientId { get; set; }
        public string pharmacyId { get; set; }
        public string billId { get; set; }
        public string billDate { get; set; }
        public string billImageUrl { get; set; }
        public Product[] products { get; set; }


        public class Product
        {
            public string id { get; set; }
            public int quantity { get; set; }
        }


    }
}