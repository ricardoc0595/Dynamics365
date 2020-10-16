using CrmAboxApi.Logic.Classes.Helper;
using System;

namespace Logic.CrmAboxApi.Classes.Helper
{
    public class OperationResult
    {
        public OperationResult()
        {
            Data = new Object();
            Message = "";
            Code = "";
        }

        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string Code { get; set; }
        public Object Data { get; set; }
        public CrmWebAPIError InternalError { get; set; }
    }
}