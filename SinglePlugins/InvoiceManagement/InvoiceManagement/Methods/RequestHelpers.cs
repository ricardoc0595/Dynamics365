using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using InvoiceManagement.Classes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;

namespace InvoiceManagement.Methods
{
    public class RequestHelpers
    {

        /// <summary>
        /// Obtiene un objeto para convertir a JSON y enviarlo a los servicios de Abox Plan.
        /// </summary>
        /// <param name="invoice">Entidad factura a la que se le extraen los atributos</param>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public InvoiceCreateRequest.Request GetInvoiceCreateRequestObject(Entity invoice, IOrganizationService service, ITracingService trace)
        {
            var request = new InvoiceCreateRequest.Request();
            MShared sharedMethods = new MShared();
            try
            {
                ContactEntity contactEntity = new ContactEntity();
                PharmacyEntity pharmacyEntity = new PharmacyEntity();

                Entity contact = null;
                if (invoice.Attributes.Contains(InvoiceFields.Customer))
                {
                    EntityReference contactReference = invoice.GetAttributeValue<EntityReference>(InvoiceFields.Customer);

                    string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country };
                    var columnSet = new ColumnSet(columnsToGet);
                    contact = service.Retrieve(contactEntity.EntitySingularName, contactReference.Id, columnSet);

                }

                if ((contact != null) && (contact.Attributes.Contains(ContactFields.IdAboxPatient)))
                {
                    request.patientId = contact.GetAttributeValue<int>(ContactFields.IdAboxPatient);
                }

                if (invoice.Attributes.Contains(InvoiceFields.InvoiceNumber))
                {
                    request.billId = invoice.GetAttributeValue<string>(InvoiceFields.InvoiceNumber);
                }

              
                if (invoice.Attributes.Contains(InvoiceFields.PurchaseDate))
                {
                    DateTime date = new DateTime();
                    date = invoice.GetAttributeValue<DateTime>(InvoiceFields.PurchaseDate);
                    if (date != null)
                    {
                        request.billDate = date.ToString("yyyy-MM-dd");
                    }
                }

                if (invoice.Attributes.Contains(InvoiceFields.ProductsSelectedJson))
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(InvoiceCreateRequest.Request.Product[]));

                    using (var ms = new System.IO.MemoryStream(Encoding.Unicode.GetBytes(invoice.GetAttributeValue<string>(InvoiceFields.ProductsSelectedJson))))
                    {
                        deserializer = new DataContractJsonSerializer(typeof(InvoiceCreateRequest.Request.Product[]));
                        request.products= (InvoiceCreateRequest.Request.Product[])deserializer.ReadObject(ms);
                    }
                
                }

                if (invoice.Attributes.Contains(InvoiceFields.InvoiceImageWebUrl))
                {
                    request.billImageUrl = invoice.GetAttributeValue<string>(InvoiceFields.InvoiceImageWebUrl);
                }



                if (invoice.Attributes.Contains(InvoiceFields.Pharmacy))
                {
                    EntityReference pharmacyReference = null;
                    pharmacyReference = (EntityReference)invoice.Attributes[InvoiceFields.Pharmacy];
                    if (pharmacyReference != null)
                    {
                        var pharmacyRetrieved = service.Retrieve(pharmacyEntity.EntitySingularName, pharmacyReference.Id, new ColumnSet(PharmacyFields.Id));
                        if (pharmacyRetrieved.Attributes.Contains(PharmacyFields.Id))
                        {
                            string idPharmacy = pharmacyRetrieved.GetAttributeValue<string>(PharmacyFields.Id);

                            if (!String.IsNullOrEmpty(idPharmacy))
                            {
                                request.pharmacyId = idPharmacy;
                            }
                        }
                    }
                }

                return request;
            }
            catch (Exception ex)
            {
                sharedMethods.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error al obtener la estructura para el request",
                    ProcessId = ""
                }, trace);
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }

        public InvoiceUpdateRequest.Request GetInvoiceUpdateRequestObject(Entity invoice, IOrganizationService service, ITracingService trace)
        {
            InvoiceUpdateRequest.Request request = null;
            MShared sharedMethods = new MShared();
            try
            {
                //Se reutiliza el método porque son casi las mismas propiedades
                InvoiceCreateRequest.Request objectRequest = this.GetInvoiceCreateRequestObject(invoice,service,trace);

                if (objectRequest!=null)
                {


                    request = new InvoiceUpdateRequest.Request
                    {
                        billDate = objectRequest.billDate,
                        billNumber = objectRequest.billId,
                        billImageUrl = objectRequest.billImageUrl,
                        patientId = objectRequest.patientId,
                        pharmacyId = objectRequest.pharmacyId,
                        
                    };
                    request.products = new InvoiceUpdateRequest.Request.Product[objectRequest.products.Length];
                    for (int i = 0; i < objectRequest.products.Length; i++)
                    {
                        request.products[i] = new InvoiceUpdateRequest.Request.Product
                        {
                            id = objectRequest.products[i].id,
                            quantity = objectRequest.products[i].quantity

                        };
                    }

                }

                if (request!=null)
                {

                    if (invoice.Attributes.Contains(InvoiceFields.InvoiceNumber))
                    {
                        request.billId = Convert.ToString(invoice.GetAttributeValue<int>(InvoiceFields.IdAboxInvoice));
                    }

                }

                return request;

            }
            catch (Exception ex)
            {
                sharedMethods.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error al obtener la estructura para el request",
                    ProcessId = ""
                }, trace);
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }


        public InvoiceNumberCheckRequest.Request CheckInvoiceNumberRequest(Entity invoice, IOrganizationService service, ITracingService trace)
        {
            InvoiceNumberCheckRequest.Request requestResponse = new InvoiceNumberCheckRequest.Request();
            MShared sharedMethods = new MShared();
            ContactEntity contactEntity = new ContactEntity();
            PharmacyEntity pharmacyEntity = new PharmacyEntity();
            CountryEntity countryEntity = new CountryEntity();
            try
            {
                Entity contact = null;
                if (invoice.Attributes.Contains(InvoiceFields.Customer))
                {
                    EntityReference contactReference = invoice.GetAttributeValue<EntityReference>(InvoiceFields.Customer);

                    string[] columnsToGet = new string[] { ContactFields.IdAboxPatient, ContactFields.Country };
                    var columnSet = new ColumnSet(columnsToGet);
                    contact = service.Retrieve(contactEntity.EntitySingularName, contactReference.Id, columnSet);

                    if (contact != null)
                    {
                        if (contact.Attributes.Contains(ContactFields.Country))
                        {
                            EntityReference countryReference = null;
                            countryReference = (EntityReference)contact.Attributes[ContactFields.Country];
                            if (countryReference != null)
                            {
                                var countryRetrieved = service.Retrieve(countryEntity.EntitySingularName, countryReference.Id, new ColumnSet(CountryFields.IdCountry));
                                if (countryRetrieved.Attributes.Contains(CountryFields.IdCountry))
                                {
                                    string country = countryRetrieved.GetAttributeValue<string>(CountryFields.IdCountry);

                                    if (!String.IsNullOrEmpty(country))
                                    {
                                        requestResponse.countryId = country;
                                    }
                                }
                            }
                        }
                    }

                    if (invoice.Attributes.Contains(InvoiceFields.Pharmacy)) {

                        EntityReference pharmacyReference = null;
                        pharmacyReference = (EntityReference)invoice.Attributes[InvoiceFields.Pharmacy];
                        if (pharmacyReference != null)
                        {
                            var pharmacyRetrieved = service.Retrieve(pharmacyEntity.EntitySingularName, pharmacyReference.Id, new ColumnSet(PharmacyFields.Id));
                            if (pharmacyRetrieved.Attributes.Contains(PharmacyFields.Id))
                            {
                                string idPharmacy = pharmacyRetrieved.GetAttributeValue<string>(PharmacyFields.Id);

                                if (!String.IsNullOrEmpty(idPharmacy))
                                {
                                    requestResponse.pharmacyId = Convert.ToInt32(idPharmacy);
                                }
                            }
                        }

                    }

                    if (invoice.Attributes.Contains(InvoiceFields.InvoiceNumber))
                    {
                        requestResponse.purchaseNumber = invoice.GetAttributeValue<string>(InvoiceFields.InvoiceNumber);
                    }

                }
                return requestResponse;
            }
            catch (Exception ex)
            {
                sharedMethods.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error al obtener la estructura para el request",
                    ProcessId = ""
                }, trace);
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                return null;
            }
        }

    }
}
