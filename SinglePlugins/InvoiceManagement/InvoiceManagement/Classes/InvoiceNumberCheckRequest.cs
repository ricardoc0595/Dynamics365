using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Classes
{
    [DataContract]
    public class InvoiceNumberCheckRequest
    {
        [DataContract]
        public class Request
        {
            [DataMember]
            public string purchaseNumber { get; set; }
            [DataMember]
            public int pharmacyId { get; set; }
            [DataMember]
            public string countryId { get; set; }


        }

        [DataContract]
        public class ServiceResponse
        {
            [DataMember]
            public Header header { get; set; }
            [DataMember]
            public Response response { get; set; }

            [DataContract]
            public class Header
            {
                [DataMember]
                public string code { get; set; }
                [DataMember]
                public string message { get; set; }
                [DataMember]
                public Headerdetails headerdetails { get; set; }
            }
            [DataContract]
            public class Headerdetails
            {
            }
            [DataContract]
            public class Response
            {
                [DataMember]
                public string code { get; set; }
                [DataMember]
                public string message { get; set; }
                [DataMember]
                public Details details { get; set; }
            }
            [DataContract]
            public class Details
            {
            }


        }

    }
}
