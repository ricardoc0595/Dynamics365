using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxCrmPlugins.Classes.Entities
{
    public class ContactEntity :EntityBase
    {
        public ContactFields Fields { get; set; }
        public ContactEntity()
        {

            this.Fields = new ContactFields();
            this.EntityName = "contacts";
        }
    }

    public class ContactSchemas
    {
        //TODO: Crear un Objeto que lleve los valores de FIelds y de Schemas, para centralizarlo de mejor forma
    }

    public class ContactFields
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
        public string EntityId { get; }
        public string ContactxContactRelationship { get; set; }
        public string OtherInterest { get;  }

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
            this.Country = "new_country";
            this.ContactxDoctorRelationship = "new_contact_new_doctor";
            this.ContactxProductRelationship = "new_product_contact";
            this.ContactxContactRelationship = "new_contact_contact";
            this.OtherInterest = "new_otrointeres";
        }


    }
}
