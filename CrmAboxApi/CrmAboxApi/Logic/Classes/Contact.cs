using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Configuration;
using CrmAboxApi.Logic.Classes.Helper;
using Logic.CrmAboxApi.Classes.Helper;
using System.Text;
using Newtonsoft.Json;
using CrmAboxApi.Logic.Classes.Deserializing;
using AboxCrmPlugins.Classes.Entities;
using CrmAboxApi.Logic.Methods;


namespace CrmAboxApi.Logic.Classes
{
    public class Contact
    {
        MShared sharedMethods = null;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Contact()
        {
            sharedMethods = new MShared();
        }

        public ServiceResponse Create(PatientSignup signupRequest)
        {
            ServiceResponse responseObject = new ServiceResponse();

            try
            {
                string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
                PatientSignup signupProperties = signupRequest;
                ContactEntity contactEntity = new ContactEntity();

                JArray productsArray = new JArray();
                foreach (var product in signupProperties.medication.products)
                {
                    productsArray.Add(new JValue($"/products(productnumber='{product.productid}')"));
                }

                var contact1 = new JObject
                        {
                            { $"{contactEntity.Fields.Firstname}", signupProperties.personalinfo.name },
                            { $"{contactEntity.Fields.Lastname}", signupProperties.personalinfo.lastname },
                            { $"{contactEntity.Fields.SecondLastname}", signupProperties.personalinfo.secondlastname },
                            { $"{contactEntity.Fields.Password}", signupProperties.personalinfo.password},
                            { $"{contactEntity.Fields.Birthdate}", DateTime.Parse(signupProperties.personalinfo.dateofbirth).ToString("yyyy-MM-dd") }, //formato

                            //TODO: Max length esta en 14 actualmente, validar extension
                             { $"{contactEntity.Fields.Id}", signupProperties.personalinfo.id},
                            { $"{contactEntity.Fields.Country}", 1},
                            { $"{contactEntity.Fields.Email}", signupProperties.contactinfo.email },
                            { $"{contactEntity.Fields.Phone}", signupProperties.contactinfo.phone },
                            { $"{contactEntity.Fields.SecondaryPhone}", signupProperties.contactinfo.mobilephone },
                            { $"{contactEntity.Fields.IdType}", Int32.Parse(signupProperties.personalinfo.idtype)<10?$"0{signupProperties.personalinfo.idtype}":Convert.ToString(signupProperties.personalinfo.idtype)},
                            { $"new_UserType@odata.bind", $"/new_usertypes({sharedMethods.GetUserTypeEntityId(signupProperties.userType)})"}, // User type
                            { $"{contactEntity.Fields.Gender}", sharedMethods.GetGenderValue(signupProperties.personalinfo.gender) },

                            { "new_contact_product@odata.bind", productsArray},
                            //TODO: enviar desde el json, el ID que retorno el servicio de Abox con el Id de paciente
                            /*{ $"{contactEntity.Fields.IdAboxPatient}", signupProperties.personalinfo.idtype }*/
                            { $"{contactEntity.Fields.RegisterDay}", DateTime.Now.ToString("yyyy-MM-dd") } // fecha de registro
                          

                        };


                try
                {
                    using (HttpClient client = ConnectionHelper.GetHttpClient(connectionString, ConnectionHelper.clientId, ConnectionHelper.redirectUrl))
                    {


                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        //client.DefaultRequestHeaders.Add("If-None-Match", "null"); 
                        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                        HttpContent c = new StringContent(contact1.ToString(Formatting.None), Encoding.UTF8, "application/json");
                        var response = client.PostAsync("contacts", c).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //Get the response content and parse it.  
                            //JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                            //Guid userId = (Guid)body["UserId"];



                            responseObject.Code = "";
                            responseObject.Message = "Contacto creado correctamente en el CRM";
                            responseObject.IsSuccessful = true;
                            responseObject.Data = null;

                        }
                        else
                        {
                            responseObject.Code = "Error al crear el contacto en el CRM";
                            responseObject.Message = response.ReasonPhrase;
                            responseObject.IsSuccessful = false;
                            responseObject.Data = null;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    responseObject.Code = "";
                    responseObject.Message = ex.ToString();
                    responseObject.IsSuccessful = false;
                    responseObject.Data = null;

                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                responseObject.Code = "";
                responseObject.Message = ex.ToString();
                responseObject.IsSuccessful = false;
                responseObject.Data = null;
            }


            return responseObject;
        }


    }
}