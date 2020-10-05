using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateContactAsPatient.Classes
{


    public class UpdateAccountRequest
    {

        public class Request
        {
            public string user { get; set; }
            public string patientId { get; set; }
            public string Nombre { get; set; }
            public string Apellido1 { get; set; }
            public string Apellido2 { get; set; }
            public string Genero { get; set; }
            public string Telefono { get; set; }
            public string Telefono2 { get; set; }
            public string Email { get; set; }
            public string Provincia { get; set; }
            public string Canton { get; set; }
            public string Distrito { get; set; }
            public object Direccion { get; set; }
            public string FechaNacimiento { get; set; }
            public string TipoUsuario { get; set; }
            public object ImagenFondo { get; set; }
            public object ImagenAvatar { get; set; }
            public object Password { get; set; }
            public Medication medication { get; set; }
            public Interest[] interests { get; set; }


            public class Medication
            {
                public Product[] products { get; set; }
                public Medic[] medics { get; set; }
            }

            public class Product
            {
                public string productid { get; set; }
                public string frequency { get; set; }
                public object other { get; set; }
                public string name { get; set; }
            }

            public class Medic
            {
                public string medicid { get; set; }
                public string name { get; set; }
            }

            public class Interest
            {
                public string interestid { get; set; }
                public Relation[] relations { get; set; }
            }

            public class Relation
            {
                public Relation1 relation { get; set; }
            }

            public class Relation1
            {
                public string relationid { get; set; }
                public string other { get; set; }
            }

        }

        public class ServiceResponse
        {
            public Header header { get; set; }
            public Response response { get; set; }

           

            public class Header
            {
                public int code { get; set; }
                public string message { get; set; }
                public Headerdetails headerdetails { get; set; }
            }

            public class Headerdetails
            {
            }

            public class Response
            {
                public string code { get; set; }
                public string message { get; set; }
                public Details details { get; set; }
            }

            public class Details
            {
            }


        }


    }




}
