using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CreateContactAsPatient.Classes
{
    public class QuickSignupRequest
    {
        [DataContract]
        public class Request
        {
            [DataMember]
            public object patientid { get; set; }
            [DataMember]

            public string country { get; set; }
            [DataMember]

            public string userType { get; set; }
            [DataMember]

            public Personalinfo personalinfo { get; set; }
            [DataMember]

            public Contactinfo contactinfo { get; set; }
            [DataMember]

            public object patientincharge { get; set; }
            [DataMember]

            public object medication { get; set; }
            [DataMember]

            public object interests { get; set; }
            [DataMember]

            public object otherInterest { get; set; }


            [DataContract]
            public class Personalinfo
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

                public object gender { get; set; }
                [DataMember]

                public object dateofbirth { get; set; }
                [DataMember]

                public string password { get; set; }
            }

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
                public string phone { get; set; }
                [DataMember]
                public string mobilephone { get; set; }
                [DataMember]
                public object address { get; set; }
                [DataMember]
                public string email { get; set; }
                [DataMember]
                public string password { get; set; }
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
            public class Headerdetails
            {
            }

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
            public class Details
            {
                [DataMember]

                public int idPaciente { get; set; }
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



        }








    }
}
