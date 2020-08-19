using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxCrmPlugins.Classes
{
    public class ContactEntity
    {

        public class Fields
        {
            private readonly string _IdType;
            private readonly string _Id;
            private readonly string _Country;
            private readonly string _Firstname;
            private readonly string _Lastname;
            private readonly string _SecondLastname;
            private readonly string _Password;
            private readonly string _Email;
            private readonly string _Phone;
            private readonly string _SecondaryPhone;
            private readonly string _Gender;
            private readonly string _Birthdate;

            public Fields()
            {
                this._IdType = "new_idtype";
                this._Id = "new_id";
                this._Firstname = "firstname";
                this._Lastname = "lastname";
                this._SecondLastname = "new_secondlastname";
                this._Password = "new_password";
                this._Email="emailaddress1";
                this._Phone = "telephone2";
                this._SecondaryPhone = "mobilephone";
                this._Gender = "";
                this._Birthdate = "birthdate";
                    
            }

            public string IdTypeFieldName { get => _IdType; }
            public string IdFieldName { get => _Id; }
            public string CountryFieldName { get => _Country; }
            public string FirstnameFieldName { get => _Firstname; }
            public string LastnameFieldName { get => _Lastname;}
            public string SecondLastnameFieldName { get => _SecondLastname;}
            public string PasswordFieldName { get => _Password; }
            public string EmailFieldName { get => _Email;  }
            public string PhoneFieldName { get => _Phone;  }
            public string SecondaryPhoneFieldName { get => _SecondaryPhone; }

            public string GenderFieldName { get => _Gender ; }
            public string BirthdateFieldName { get => _Birthdate; }



        }

        
       

    }
}
