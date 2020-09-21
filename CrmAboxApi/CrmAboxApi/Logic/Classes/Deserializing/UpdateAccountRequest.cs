using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class UpdateAccountRequest
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
            public string other { get; set; }
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
}