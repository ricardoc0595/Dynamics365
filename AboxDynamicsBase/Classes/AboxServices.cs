using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes
{
    public class AboxServices
    {
        public const string _quickSignupService= "https://apidev.aboxplan.com/member/signup/kyn";
        private const string updatePatientService = "https://apidev.aboxplan.com/member/patient/update";
        private const string _updateAccountService = "https://apidev.aboxplan.com/member/account/update";
        public static string QuickSignupService => _quickSignupService;

        public static string UpdatePatientService => updatePatientService;

        public static string UpdateAccountService => _updateAccountService;
    }
}
