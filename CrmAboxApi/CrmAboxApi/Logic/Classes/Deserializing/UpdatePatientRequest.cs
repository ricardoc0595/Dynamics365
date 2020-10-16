namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class UpdatePatientRequest
    {
        public string country { get; set; }
        public string userType { get; set; }
        public string patientid { get; set; }
        public Personalinfo personalinfo { get; set; }
        public Contactinfo contactinfo { get; set; }
        public object patientincharge { get; set; }
        public Medication medication { get; set; }
        public object interests { get; set; }
        public string imagenfondo { get; set; }
        public string imagenavatar { get; set; }

        public class Personalinfo
        {
            public object idtype { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string lastname { get; set; }
            public string secondlastname { get; set; }
            public string gender { get; set; }
            public string dateofbirth { get; set; }
            public object password { get; set; }
        }

        public class Contactinfo
        {
            public string province { get; set; }
            public string canton { get; set; }
            public string district { get; set; }
            public string phone { get; set; }
            public string mobilephone { get; set; }
            public string address { get; set; }
            public string email { get; set; }
        }

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
        }

        public class Medic
        {
            public string medicid { get; set; }
        }
    }
}