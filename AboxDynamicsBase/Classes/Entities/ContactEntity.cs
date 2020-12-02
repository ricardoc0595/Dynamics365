using System.Collections.Generic;

namespace AboxDynamicsBase.Classes.Entities
{
    public class ContactEntity : EntityBase
    {
        public ContactEntity()
        {
            this.EntityPluralName = "contacts";
            this.EntitySingularName = "contact";
        }
    }

   

    public static class ContactSchemas
    {
        public const string UserType = "new_UserType";
        public const string ContactxDoseRelationship = "";
        public const string Country = "new_CountryId";
        public const string Province = "new_CityId";
        public const string Canton = "new_Canton";
        public const string District = "new_Distrit";
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
        public const string Canton = "new_canton";
        public const string District = "new_distrit";
        public const string Province = "new_cityid";
        public const string Interests = "new_clientinterest";
        public const string ContactxContactLookup = "new_contactcontactid";
        public const string IsChildContact = "new_ischildcontact";
		public const string CountryLookup = "new_countryid";
        public const string CityLookup = "new_cityid";
        public const string CantonLookup = "new_canton";
        public const string DistrictLookup = "new_distrit";
        public const string NoEmail = "new_noemail";
        public const string OtherInterestLookup = "new_otherinterest";
    }
}