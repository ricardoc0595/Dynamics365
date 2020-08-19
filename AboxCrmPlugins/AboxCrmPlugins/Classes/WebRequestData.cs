using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxCrmPlugins.Classes
{
    public class WebRequestData
    {
        public string ContentType { get; set; }
        public string Authorization { get; set; }

        public string InputData { get; set; }
        public string Url { get; set; }
        public WebRequestData()
        {
            ContentType = "";
            Authorization = "";
            InputData = "";
        }

    }
}
