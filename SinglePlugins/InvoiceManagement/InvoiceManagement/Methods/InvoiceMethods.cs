using AboxCrmPlugins.Methods;
using AboxDynamicsBase.Classes;
using AboxDynamicsBase.Classes.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InvoiceManagement.Methods
{
    public class InvoiceMethods
    {

        public List<string> GetEntityValidationStatus(Entity invoice, IOrganizationService service, Microsoft.Xrm.Sdk.ITracingService trace)
        {
            List<string> validationMessages = new List<string>();

            MShared sharedMethods = new MShared();
            ContactEntity contactEntity = new ContactEntity();
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
                        if (!contact.Attributes.Contains(ContactFields.IdAboxPatient))
                        {
                            validationMessages.Add($"Este contacto no posee un Id de paciente Abox registrado");
                        }

                    }
                }

                if (!invoice.Attributes.Contains(InvoiceFields.Pharmacy))
                {

                    validationMessages.Add($"La farmacia es requerida.");

                }


                if (invoice.Attributes.Contains(InvoiceFields.InvoiceImageWebUrl))
                {
                    if (String.IsNullOrEmpty(invoice.GetAttributeValue<string>(InvoiceFields.InvoiceImageWebUrl)))
                    {
                        validationMessages.Add($"La imagen de la factura es requerida.");
                    }
                }
                else
                {
                    validationMessages.Add($"La imagen de la factura es requerida.");
                }

                if (invoice.Attributes.Contains(InvoiceFields.ProductsSelectedJson))
                {
                    if (String.IsNullOrEmpty(invoice.GetAttributeValue<string>(InvoiceFields.ProductsSelectedJson)))
                    {
                        validationMessages.Add($"No se han podido identificar los productos requeridos para guardar esta factura.");
                    }
                }
                else
                {
                    validationMessages.Add($"No se han podido identificar los productos requeridos para guardar esta factura.");
                }




                if (invoice.Attributes.Contains(InvoiceFields.PurchaseDate))
                {
                    DateTime date = new DateTime();
                    date = invoice.GetAttributeValue<DateTime>(InvoiceFields.PurchaseDate);
                    if (date != null)
                    {
                        DateTime today = DateTime.Now;

                        if (date > today)
                        {
                            validationMessages.Add("La fecha de la factura no puede ser mayor a la fecha actual.");
                        }

                    }
                }
                else
                {
                    validationMessages.Add($"La fecha de la factura es requerida.");
                }



                //if (invoice.Attributes.Contains(InvoiceFields.PurchaseDate))
                //{
                //    DateTime date = new DateTime();
                //    date = invoice.GetAttributeValue<DateTime>(InvoiceFields.PurchaseDate);
                //    if (date != null)
                //    {
                //        DateTime today = DateTime.Now;

                //        if (DateTime.)
                //        {

                //        }

                //    }
                //}



                return validationMessages;
            }
            catch (Exception ex)
            {
                sharedMethods.LogPluginFeedback(new LogClass
                {
                    Exception = ex.ToString(),
                    Level = "error",
                    ClassName = this.GetType().ToString(),
                    MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Message = $"Error validando los datos de la entidad.",
                    ProcessId = ""
                }, trace);
                trace.Trace($"MethodName: {new StackTrace(ex).GetFrame(0).GetMethod().Name}|--|Exception: " + ex.ToString());

                throw ex;
            }

        }

    }
}
