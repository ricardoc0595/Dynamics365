namespace AboxDynamicsBase.Classes.Entities
{
    public class DoctorEntity : EntityBase
    {
        //public DoctorFields Fields { get; set; }
        public DoctorSchemas Schemas { get; set; }

        public DoctorEntity()
        {
            //this.Fields = new DoctorFields();
            this.Schemas = new DoctorSchemas();
            this.EntityPluralName = "new_doctors";
            this.EntitySingularName = "new_doctor";
        }
    }

    public class DoctorSchemas
    {
        //TODO: Crear un Objeto que lleve los valores de FIelds y de Schemas, para centralizarlo de mejor forma
    }

    public static class DoctorFields
    {
        public const string EntityId = "new_doctorid";
        public const string Address = "new_doctoraddress";
        public const string Canton = "new_doctorcanton";
        public const string Code = "new_doctorcode";
        public const string CodeCloseup = "new_doctorcodecloseup";
        public const string Country = "new_doctorcountry";
        public const string Division = "new_doctordivision";
        public const string Email = "new_doctoremail";
        public const string Identification = "new_doctoridentification";
        public const string Phone = "new_doctorphone";
        public const string Province = "new_doctorprovince";
        public const string Specialty = "new_medicalspeciality";
        public const string FullName = "new_doctor";
        public const string DoctorIdKey = "new_doctorid2";
        public const string IdDoctorEntityKey = "idkeyexternaldb";
    }
}