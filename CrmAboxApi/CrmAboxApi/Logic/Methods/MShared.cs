
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using CrmAboxApi.Logic.Classes.Helper;
using System.Runtime.Remoting.Messaging;

namespace CrmAboxApi.Logic.Methods
{
    public class MShared
    {

        /// <summary>
        /// Gets the ProductEntity ID from the CRM
        /// </summary>
        /// <param name="productId">Product Internal ID, </param>
        /// <returns>Product ID from Abox Plan Database</returns>
        public string getProductEntityID(string productId)
        {
            //TODO: As a performance best practice, you must always use the $select system query option to limit the properties returned while retrieving data.
            return "";
        }

        public string GetUserTypeEntityId(string idType)
        {
            string result = "";
            try
            {
                switch (idType)
                {
                    case "01":
                        result = Constants.PatientIdType;
                        break;
                    case "02":
                        result = Constants.CareTakerIdType;
                        break;
                    case "03":
                        result = Constants.TutorIdType;
                        break;
                    case "04":
                        result = ""; //No se usa en produccion de Abox Plan
                        break;
                    case "05":
                        result = Constants.OtherInterestIdType;
                        break;
                    default:
                        result = null;
                        break;
                }

            }
            catch (Exception ex)
            {
                result = null;
                throw;
            }
            return result;

        }


        public int GetGenderValue(string genderValueFromJson)
        {
            int result=-1;
            try
            {
                switch (genderValueFromJson.ToLower())
                {
                    case "masculino":
                        result = Constants.MaleGenderValue;
                        break;
                    case "femenino":
                        result = Constants.FemaleGenderValue;
                        break;
                    default:
                        result = -1;
                        break;
                }

            }
            catch (Exception ex)
            {
                result = -1;
                
            }
            return result;

        }

    }
}
