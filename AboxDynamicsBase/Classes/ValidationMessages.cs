using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AboxDynamicsBase.Classes
{
    public class ValidationMessages
    {

        public static class Contact
        {
            public const string ValidName = "Ingresar sólo letras y únicamente espacios entre palabras";
            public const string AgeLimit = "Para participar en el programa Abox el usuario debe tener al menos 12 años";
            public const string OnlyNumbers = "Sólo números están permitidos";
            public const string PasswordFormat = "La contraseña debe tener mínimo una letra mayúscula, una minúscula y un número. Debe ser de mínimo 8 caracteres y que no exceda los 30 caracteres";
            public const string MaxLengthFormat = "El máximo de caracteres permitidos es {0}";
            public const string MinLengthFormat = "El mínimo de caracteres permitidos es {0}";
        }

    }
}
