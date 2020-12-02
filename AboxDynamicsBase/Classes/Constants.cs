namespace AboxDynamicsBase.Classes
{
    public static class Constants
    {
        public const string PatientIdType = "15810a1e-c8d1-ea11-a812-000d3a33f637";
        public const string CareTakerIdType = "fab60b2a-c8d1-ea11-a812-000d3a33f637";
        public const string TutorIdType = "f4761324-c8d1-ea11-a812-000d3a33f637";
        public const string OtherInterestIdType = "30f90330-c8d1-ea11-a812-000d3a33f637";
        public const int MaleGenderValue = 1;
        public const int FemaleGenderValue = 2;
        public const int NationalIdValue = 1;
        public const int ForeignerIdValue = 2;
        public const int MinorIdValue = 3;
        public const int DoseFrequencyOnePerDay = 1;
        public const int DoseFrequencyTwoPerDay = 2;
        public const int DoseFrequencyThreePerDay = 3;
        public const int DoseFrequencyFourPerDay = 4;
        public const int DoseFrequencyOther = 5;
        public const string TokenForAboxServices = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImNybV9hYm94YXBpIiwiYXBpIjp0cnVlLCJpYXQiOjE2MDMzMTIzODB9.Cu8FYQoVWDcof_qFZ5CIA6K2OYloOEn9F-b_XahLf9w";

        public const string CRCountryIdLookup = "c76fa4f3-2bfc-ea11-a815-000d3a30f195";
        public const string HNCountryIdLookup = "cf6fa4f3-2bfc-ea11-a815-000d3a30f195";
        public const string GTCountryIdLookup = "cb6fa4f3-2bfc-ea11-a815-000d3a30f195";
        public const string PACountryIdLookup = "c96fa4f3-2bfc-ea11-a815-000d3a30f195";
        public const string DOCountryIdLookup = "cd6fa4f3-2bfc-ea11-a815-000d3a30f195";
        public const string NICountryIdLookup = "24f43452-b01e-eb11-a813-00224803f71b";

        public const string GeneralAboxServicesErrorMessage = "Ocurrió un error consultando los servicios de Abox Plan \n";
        public const string ErrorMessageCodeReturned = "Error en transacción, Código de respuesta servicio:";
        public const string ErrorMessageTransactionCodeReturned = "Ocurrió un error al guardar la información en Abox Plan:\n";
        public const string GeneralPluginErrorMessage = "Ocurrió un error en la ejecución de un Plugin interno, por favor intenta nuevamente o comunícate con soporte.";
        public const string ApplicationIdWebAPI = "WEBAPI";
        public const string ApplicationIdPlugin = "PLUGIN";

        public const string RegexValidName = @"^[a-zA-ZáéíóúñüàèÁÉÍÓÚÑÜ]+( [a-zA-ZáéíóúñüàèÁÉÍÓÚÑÜ]+)*$";
        public const string RegexValidLastname = @"^[a-zA-ZáéíóúñüàèÁÉÍÓÚÑÜ ]+([a-zA-ZáéíóúñüàèÁÉÍÓÚÑÜ ]+)*$";
        public const string RegexPassword = @"^(?!.*[ñÑ])(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,30}$";
        public const string RegexOnlyNumbers = @"^[0-9]+$";

        public const int MaxNameLength = 30;
        public const int MinNameLength = 3;
        public const string NoEmailDefaultAddress= "defaultCrm @loymark.com";



        //public enum S
        //{
        //    DA,
        //    DE
        //}
    }
}