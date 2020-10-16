namespace AboxDynamicsBase.Classes.Entities
{
    public class ContactEntity : EntityBase
    {
        //public ContactFields Fields { get; set; }
        public ContactSchemas Schemas { get; set; }

        public ContactEntity()
        {
            //this.Fields = new ContactFields();
            this.Schemas = new ContactSchemas();
            this.EntityPluralName = "contacts";
            this.EntitySingularName = "contact";
        }
    }

    public class ContactSchemas
    {
        public ContactSchemas()
        {
            UserType = "new_UserType";
            ContactxDoseRelationship = "";
            Country = "new_CountryId";
            Province = "new_CityId";
            Canton = "new_Canton";
            District = "new_Distrit";
        }

        //TODO: Crear un Objeto que lleve los valores de FIelds y de Schemas, para centralizarlo de mejor forma
        public string UserType { get; set; }

        public string ContactxDoseRelationship { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string Canton { get; set; }
        public string District { get; set; }
    }

    public static class ContactFields
    {
        public const string EntityId = "contactid";
        public const string IdType = "new_idtype";
        public const string Id = "new_id";
        public const string UserType = "new_usertype";
        public const string Firstname = "firstname";
        public const string Lastname = "lastname";
        public const string SecondLastname = "new_secondlastname";
        public const string Password = "new_password";
        public const string Email = "emailaddress1";
        public const string Phone = "telephone2";
        public const string SecondaryPhone = "mobilephone";
        public const string Gender = "gendercode";
        public const string Birthdate = "birthdate";
        public const string ProductxContactId = "new_productcontactid";
        public const string RegisterDay = "new_registerday";
        public const string IdAboxPatient = "new_idaboxpatient";
        public const string Country = "new_countryid";
        public const string ContactxDoctorRelationship = "new_contact_new_doctor";
        public const string ContactxProductRelationship = "new_product_contact";
        public const string ContactxContactRelationship = "new_contact_contact";
        public const string ContactxDoseRelationship = "new_contact_new_dose";
        public const string OtherInterest = "new_otrointeres";
        public const string Canton = "new_canton";
        public const string District = "new_distrit";
        public const string Province = "new_cityid";
        public const string Interests = "new_clientinterest";
        public const string ContactxContactLookup = "new_contactcontactid";
    }
}