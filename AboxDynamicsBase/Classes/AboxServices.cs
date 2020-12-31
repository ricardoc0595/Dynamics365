namespace AboxDynamicsBase.Classes
{
    public static class AboxServices
    {
        
        public const string QuickSignupService = Configuration.Environment + "/member/signup/kyn";
        public const string UpdatePatientService = Configuration.Environment + "/member/patient/update";
        public const string UpdateAccountService = Configuration.Environment + "/member/account/update";
        public const string PatientSignup = Configuration.Environment + "/member/signup/crm/patient";
        public const string ConsumerSignup = Configuration.Environment + "/member/signup/crm/consumer";
        public const string CrmWebAPILog = Configuration.WebAPIEnvironment + "/api/contacts/LogPluginFeedback";
        public const string MainPatientForTutorOrCaretakerService = Configuration.Environment + "/member/signup/crm/main";
        public const string CaretakerChildService = Configuration.Environment + "/member/signup/crm/tutorchild";
        public const string TutorChildService = Configuration.Environment + "/member/signup/crm/tutorchild";
        public const string WelcomeSendMailService = Configuration.Environment + "/member/signup/crm/welcome_sendmail";
        public const string SignIntoAccountService = Configuration.Environment + "/member/account/patient/signintoaccount";
        public const string CreateInvoice = Configuration.Environment + "/purchases/create";
        public const string ChangePasswordCrm = Configuration.Environment + "/security/crm/changepassword";
    }
}