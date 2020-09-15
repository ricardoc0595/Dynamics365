using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class RetrieveDoctorFromWebAPI
    {
        public string odatacontext { get; set; }
        public Value[] value { get; set; }
        public string odatanextLink { get; set; }

        

        public class Value
        {
            public string new_doctorid { get; set; }
           
        }


    }
}