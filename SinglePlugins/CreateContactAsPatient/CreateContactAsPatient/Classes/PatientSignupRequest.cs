using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CreateContactAsPatient.Classes
{
    /// <summary>
    /// Clase que contiene las propiedades que se envían al servicio para registrar un usuario como paciente. Tiene tambien las propiedades que se reciben como respuesta del servicio
    /// </summary>
    public class PatientSignupRequest
    {
        /// <summary>
        /// Propiedades que se envian al servicio.
        /// </summary>
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
            public Patientincharge patientincharge { get; set; }

            [DataMember]
            public Medication medication { get; set; }

            [DataMember]
            public List<Interest> interests { get; set; }

            [DataMember]
            public string otherInterest { get; set; }

            public Request()
            {
                this.personalinfo = new Personalinfo();
                this.contactinfo = new Contactinfo();
                this.patientincharge = new Patientincharge();
                this.medication = new Medication();
                this.interests = new List<Interest>();
            }

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
                public string gender { get; set; }

                [DataMember]
                public string dateofbirth { get; set; }

                [DataMember]
                public string password { get; set; }
            }

            [DataContract]
            public class Contactinfo
            {
                [DataMember]
                public string province { get; set; }

                [DataMember]
                public string canton { get; set; }

                [DataMember]
                public string district { get; set; }

                [DataMember]
                public string phone { get; set; }

                [DataMember]
                public string mobilephone { get; set; }

                [DataMember]
                public string address { get; set; }

                [DataMember]
                public string email { get; set; }

                [DataMember]
                public string password { get; set; }
            }

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
            public class Medication
            {
                [DataMember]
                public List<Product> products { get; set; }

                [DataMember]
                public List<Medic> medics { get; set; }

                public Medication()
                {
                    this.products = new List<Product>();
                    this.medics = new List<Medic>();
                }
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
            public class Interest
            {
                [DataMember]
                public string interestid { get; set; }

                [DataMember]
                public List<Relation> relations { get; set; }
            }

            [DataContract]
            public class Relation
            {
                [DataMember]
                public Relation1 relation { get; set; }
            }

            [DataContract]
            public class Relation1
            {
                [DataMember]
                public string relationid { get; set; }

                [DataMember]
                public string other { get; set; }
            }
        }

        /// <summary>
        /// Respuesta recibida por el servicio, esta clase es utilizada para deserializar el json que devuelve el servicio
        /// </summary>
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
                public int idPaciente { get; set; }
            }
        }
    }
}