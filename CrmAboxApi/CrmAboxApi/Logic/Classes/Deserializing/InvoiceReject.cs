using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class InvoiceReject
    {
        
        public string InvoiceId { get; set; }
        public string User { get; set; }
        public string Reason { get; set; }
        public string Duration { get; set; }
    }
}