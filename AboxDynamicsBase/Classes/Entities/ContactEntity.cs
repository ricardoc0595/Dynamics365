using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes.Entities
{
    public class ContactEntity :EntityBase
    {
        public ContactFields Fields { get; set; }
        public ContactSchemas Schemas { get; set; }
        public ContactEntity()
        {

            this.Fields = new ContactFields();
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

    public class ContactFields : EntityBase.BaseFields
    {
        public string IdType { get; }
        public string Id { get; }
        public string UserType { get; set; }
        public string Country { get; }
        public string Firstname { get; }
        public string Lastname { get; }
        public string SecondLastname { get; }
        public string Password { get; }
        public string Email { get; }
        public string Phone { get; }
        public string SecondaryPhone { get; }
        public string Gender { get; }
        public string Birthdate { get; }
        public string ProductxContactId { get; }
        public string RegisterDay { get; }
       
        public string IdAboxPatient { get; }
        public string ContactxDoctorRelationship { get;  }
        public string ContactxProductRelationship { get; }
        
        public string ContactxContactRelationship { get; }
        public string OtherInterest { get; }
        public string Canton { get; }
        public string District { get; }
        public string Province { get; }
        public string ContactxDoseRelationship { get;  }
        public string Interests { get;  }

        public ContactFields()
        {
            this.EntityId = "contactid";
            this.IdType = "new_idtype";
            this.Id = "new_id";
            this.UserType = "new_usertype";
            this.Firstname = "firstname";
            this.Lastname = "lastname";
            this.SecondLastname = "new_secondlastname";
            this.Password = "new_password";
            this.Email = "emailaddress1";
            this.Phone = "telephone2";
            this.SecondaryPhone = "mobilephone";
            this.Gender = "gendercode";
            this.Birthdate = "birthdate";
            this.ProductxContactId = "new_productcontactid";
            this.RegisterDay = "new_registerday";
            this.IdAboxPatient = "new_idaboxpatient";
            this.Country = "new_countryid";
            this.ContactxDoctorRelationship = "new_contact_new_doctor";
            this.ContactxProductRelationship = "new_product_contact";
            this.ContactxContactRelationship = "new_contact_contact";
            this.ContactxDoseRelationship = "new_contact_new_dose";
            this.OtherInterest = "new_otrointeres";
            this.Canton = "new_canton";
            this.District = "new_distrit";
            this.Province = "new_cityid";
            this.Interests = "new_clientinterest";
        }


    }
}
