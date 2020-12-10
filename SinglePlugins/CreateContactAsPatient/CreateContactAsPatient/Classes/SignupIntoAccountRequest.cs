using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CreateContactAsPatient.Classes
{
    public class SignupIntoAccountRequest
    {
        [DataContract]
        public class Request
        {
            [DataMember]
            public Contactinfo contactinfo { get; set; }
            [DataMember]
            public string country { get; set; }
            [DataMember]
            public Medication medication { get; set; }
            [DataMember]
            public string patientId { get; set; }
            [DataMember]
            public Patientincharge patientincharge { get; set; }
            [DataMember]
            public Personalinfo personalinfo { get; set; }
            [DataMember]
            public string userType { get; set; }

            [DataContract]
            public class Contactinfo
            {
                [DataMember]
                public object province { get; set; }
                [DataMember]
                public object canton { get; set; }
                [DataMember]
                public object district { get; set; }
                [DataMember]
                public object phone { get; set; }
                [DataMember]
                public object mobilephone { get; set; }
                [DataMember]
                public object address { get; set; }
                [DataMember]
                public string email { get; set; }
            }
            [DataContract]
            public class Medication
            {
                [DataMember]
                public Product[] products { get; set; }
                [DataMember]
                public Medic[] medics { get; set; }
            }
            [DataContract]
            public class Product
            {
                [DataMember]
                public string productid { get; set; }
                [DataMember]
                public string frequency { get; set; }
                [DataMember]
                public string other { get; set; }
            }
            [DataContract]
            public class Medic
            {
                [DataMember]
                public string medicid { get; set; }
            }
            [DataContract]
            public class Patientincharge
            {
                [DataMember]
                public string idtype { get; set; }
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public string name { get; set; }
                [DataMember]
                public string lastname { get; set; }
                [DataMember]
                public string secondlastname { get; set; }
                [DataMember]
                public string gender { get; set; }
                [DataMember]
                public string dateofbirth { get; set; }
            }

            [DataContract]
            public class Personalinfo
            {
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public string name { get; set; }
                [DataMember]
                public string lastname { get; set; }
                [DataMember]
                public string dateofbirth { get; set; }
            }

        }


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
                public int code { get; set; }
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
