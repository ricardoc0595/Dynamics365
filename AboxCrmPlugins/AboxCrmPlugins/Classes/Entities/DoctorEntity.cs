using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxCrmPlugins.Classes.Entities
{
    public class DoctorEntity :EntityBase
    {
        public DoctorFields Fields { get; set; }
        public DoctorEntity()
        {
            this.Fields = new DoctorFields();
            this.EntityName = "new_doctors";
        }
    }

    public class DoctorSchemas
    {
        //TODO: Crear un Objeto que lleve los valores de FIelds y de Schemas, para centralizarlo de mejor forma
    }

    public class DoctorFields
    {
        public string EntityId { get; }
        public string Address { get; }
        public string Canton { get; set; }
        public string Code { get; }
        public string CodeCloseup { get; }
        public string Country { get; }
        public string Division { get; }
        public string Email { get; }
        public string Identification { get; }
        public string Phone { get; }
        public string Province { get; }
        public string Specialty { get; }
        public string FullName { get; }
        public string DoctorIdKey { get; } // Entity key referring the Product ID saved in Abox database
        public string IdDoctorEntityKey { get; }

        public DoctorFields()
        {
            this.EntityId = "new_doctorid";
            this.Address = "new_doctoraddress";
            this.Canton = "new_doctorcanton";
            this.Code = "new_doctorcode";
            this.CodeCloseup = "new_doctorcodecloseup";
            this.Country = "new_doctorcountry";
            this.Division = "new_doctordivision";
            this.Email = "new_doctoremail";
            this.Identification = "new_doctoridentification";
            this.Phone = "new_doctorphone";
            this.Province = "new_doctorprovince";
            this.Specialty = "new_medicalspeciality";
            this.FullName = "new_doctor";
            this.DoctorIdKey = "new_doctorid2";
            this.IdDoctorEntityKey = "idkeyexternaldb";

        }
    }
}
