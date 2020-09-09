using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.CrmAboxApi.Classes.Helper
{
    public class ServiceResponse
    {
        public ServiceResponse()
        {
            Data = new Object();
            Message = "";
            Code = "";
        }

        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string Code { get; set; }
        public Object Data { get; set; }
    }
}
