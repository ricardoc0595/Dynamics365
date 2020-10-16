namespace CrmAboxApi.Logic.Classes.Deserializing
{
    public class PatientSignup
    {
        public int? patientid { get; set; }
        public string country { get; set; }
        public string userType { get; set; }
        public Personalinfo personalinfo { get; set; }
        public Contactinfo contactinfo { get; set; }
        public Patientincharge patientincharge { get; set; }
        public Medication medication { get; set; }
        public Interest[] interests { get; set; }
        public string otherInterest { get; set; }

        public class Personalinfo
        {
            public string idtype { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string lastname { get; set; }
            public string secondlastname { get; set; }
            public string gender { get; set; }
            public string dateofbirth { get; set; }
            public string password { get; set; }
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
            public string password { get; set; }
        }

        public class Patientincharge
        {
            public string idtype { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string lastname { get; set; }
            public string secondlastname { get; set; }
            public string gender { get; set; }
            public string dateofbirth { get; set; }
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