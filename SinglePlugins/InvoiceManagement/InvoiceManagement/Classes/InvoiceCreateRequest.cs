using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Classes
{
    [DataContract]
    public class InvoiceCreateRequest
    {

        public class Request
        {
            [DataMember]
            public int patientId { get; set; }
            [DataMember]
            public string pharmacyId { get; set; }
            [DataMember]
            public string billId { get; set; }
            [DataMember]
            public string billDate { get; set; }
            [DataMember]
            public string billImageUrl { get; set; }
            [DataMember]
            public Product[] products { get; set; }

            [DataContract]
            public class Product
            {
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public int quantity { get; set; }
            }
        }

        [DataContract]
        public class ServiceResponse
        {
            [DataMember]
            public Header header { get; set; }
            [DataMember]
            public Response response { get; set; }

            [DataContract]
            public class Header
            {
                [DataMember]
                public string code { get; set; }
                [DataMember]
                public string message { get; set; }
                [DataMember]
                public Headerdetails headerdetails { get; set; }
            }

            [DataContract]
            public class Headerdetails
            {
            }

            [DataContract]
            public class Response
            {
                [DataMember]
                public string code { get; set; }
                [DataMember]
                public string message { get; set; }
                [DataMember]
                public Details details { get; set; }
            }

            [DataContract]
            public class Details
            {
                [DataMember]
                public string idFactura { get; set; }
                [DataMember]
                public List<Validationresult> validationresults { get; set; }
            }

            public class Validationresult

            {

                public Product product { get; set; }

                public Validationresults validationresults { get; set; }

            }

            public class Validationresults

            {

                public bool result { get; set; }

                public List<ValidationMessage> validationMessages { get; set; }

            }

            public class ValidationMessage
            {
                public string message { get; set; }
                public string validation_type { get; set; }
            }

            public class Product

            {

                public string Familia { get; set; }

                public string Pais { get; set; }

                public string Producto { get; set; }

                public string ID_Producto { get; set; }

                public int? MaxCanje { get; set; }

                public int? MaxCompra { get; set; }

                public int? Bloqueado { get; set; }

                public int? Activo { get; set; }

                public int? Tomas { get; set; }

                public object InicioDeCorte { get; set; }

                public object FinDeCorte { get; set; }

                public int? TopeMensual { get; set; }
                public int? TopeAnual { get; set; }

                public int? Consumo { get; set; }

                public int? ConsumoPermitido { get; set; }

                public int? qty { get; set; }

                public string family { get; set; }

                public int? tomas { get; set; }

                public int? consumopermitidoparafamiliaestemes { get; set; }

                public int? topemensualpormarca { get; set; }
                public int? Extension { get; set; }
            }


        }



    }
}
