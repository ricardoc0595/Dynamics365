using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Classes
{
    [DataContract]
    public class InvoiceCreateRequest
    {

        public class Request
        {
            [DataMember]
            public int patientId { get; set; }
            [DataMember]
            public string pharmacyId { get; set; }
            [DataMember]
            public string billId { get; set; }
            [DataMember]
            public string billDate { get; set; }
            [DataMember]
            public string billImageUrl { get; set; }
            [DataMember]
            public Product[] products { get; set; }

            [DataContract]
            public class Product
            {
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public int quantity { get; set; }
            }
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
                [DataMember]
                public string idFactura { get; set; }
            }


        }



    }
}
