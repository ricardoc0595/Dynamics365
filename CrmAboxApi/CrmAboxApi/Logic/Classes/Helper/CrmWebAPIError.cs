using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Helper
{
    public class CrmWebAPIError
    {
        public Error error { get; set; }



        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
           
        }



    }
}