using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class UserTypeChangeRequest
    {
        public int AboxPatientId { get; set; }
        public string PatientId { get; set; }
        public string UserType { get; set; }

        public CMedication Medication { get; set; }
        public CPatientincharge PatientInCharge { get; set; }
        public string Country { get; set; }
        public int PatientUndercareId { get; set; }

        public class Product
        {
            public string ProductId { get; set; }
            public string Frequency { get; set; }
            public string Other { get; set; }
        }

        public class Medic
        {
            public string Medicid { get; set; }
        }

        public class CMedication
        {
            public List<Product> Products { get; set; }
            public List<Medic> Medics { get; set; }
        }

        public class CPatientincharge
        {
            public string Idtype { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Lastname { get; set; }
            public string Secondlastname { get; set; }
            public string Gender { get; set; }
            public string Dateofbirth { get; set; }
        }

    }
}