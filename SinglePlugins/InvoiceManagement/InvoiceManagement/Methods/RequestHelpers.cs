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

        public InvoiceCreateRequest.Request GetInvoiceCreateRequestObject(Entity invoice, IOrganizationService service, ITracingService trace)
        {
            var request = new InvoiceCreateRequest.Request();
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
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());
                throw ex;
            }
        }

    }
}
