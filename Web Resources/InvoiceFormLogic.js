if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.InvoiceFunctions = {

    //Método que se ejecuta al cargar el formulario de contacto

    onSave: function (executionContext) {

        var formContext = executionContext.getFormContext();
        var productsJsonField = null;
        var imageUrlField = null;

        productsJsonField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson);
        imageUrlField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl);

        if (productsJsonField !== null) {
            if (productsJsonField.getValue() === null || productsJsonField.getValue() === "null" || productsJsonField.getValue() === "") {
                formContext.ui.setFormNotification("Debe seleccionar al menos un producto para registrar la factura", "ERROR", "productsRequired");
                executionContext.getEventArgs().preventDefault();
            }
        }

        if (imageUrlField !== null) {
            if (imageUrlField.getValue() === null || imageUrlField.getValue() === "null" || imageUrlField.getValue() === "") {
                formContext.ui.setFormNotification("Debe seleccionar un archivo para la factura", "ERROR", "imageNotSelected");
                executionContext.getEventArgs().preventDefault();
            }
        }

    },

    onLoad: function (executionContext) {

        //FormTypes Dynamics API
        // 0	Undefined
        // 1	Create
        // 2	Update
        // 3	Read Only
        // 4	Disabled
        // 6	Bulk Edit

        //Get the context of the form
        var formContext = executionContext.getFormContext();

        var formType = formContext.ui.getFormType();
        var that = this;
        if (formType === 1) { // Create

            Abox.SharedLogic.disableFields(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson]);
            Abox.SharedLogic.setFieldsRequired(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson]);
            // Abox.SharedLogic.setFieldsInvisible(formContext,[Abox.SharedLogic.Entities.InvoiceFields.ProductSelectionWebResource]);
            this.setInvoiceImageControl(formContext);

            Abox.SharedLogic.setFieldsInvisible(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson, Abox.SharedLogic.Entities.InvoiceFields.IdAboxInvoice]);

            var contactField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.Customer);
            if (contactField !== null) {
                contactField.addOnChange(function () {

                    that.loadAndEnableProductSection(formContext);

                });
            }


        } else if (formType === 2) { //Update

            this.validateUpdateAvailability(formContext);
            Abox.SharedLogic.disableFields(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson, Abox.SharedLogic.Entities.InvoiceFields.Customer]);
            Abox.SharedLogic.setFieldsRequired(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson]);
            Abox.SharedLogic.setFieldsInvisible(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl, Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson]);
            this.setInvoiceImageControl(formContext);
            this.loadAndEnableProductSection(formContext);


        }

    },

    loadAndEnableProductSection: function (formContext) {

        that = this;
        var contactData = this.getContactInfo(formContext);
        var invoiceData = this.getInvoiceInfo(formContext);

        var productsJsonField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson);

        if (productsJsonField !== null) {

            productsJsonField.addOnChange(function () {

                if (productsJsonField.getValue() !== null) {
                    formContext.ui.clearFormNotification("productsRequired");
                }
            })
        }

        contactData.then(function (data) {

            that.setInvoiceProductSelectionComponent(formContext, data, invoiceData);
        }, function (error) {
            console.error(error);
        });

    },

    setInvoiceProductSelectionComponent: function (formContext, contactData, invoiceData) {

        try {
            Xrm.Utility.showProgressIndicator("Inicializando componentes, por favor espere...");
            var productSelectionWebResourceControl = formContext.getControl(Abox.SharedLogic.Entities.InvoiceFields.ProductSelectionWebResource);
            var that = this;

            //validar si hay productos en la factura y cargarlos

            if (productSelectionWebResourceControl) {
                productSelectionWebResourceControl.getContentWindow().then(
                    function (contentWindow) {
                        contentWindow.initializeProductSelectionLogic(formContext, Abox.SharedLogic, Xrm, Abox.SharedLogic.Entities.InvoiceFields, contactData, invoiceData);
                        Xrm.Utility.closeProgressIndicator();
                        // Abox.SharedLogic.setFieldsVisible(formContext,[Abox.SharedLogic.Entities.InvoiceFields.ProductSelectionWebResource]);
                    }, function (error) {
                        Xrm.Utility.closeProgressIndicator();
                        console.error(error);
                        throw "Error inicializando el componente de productos";
                    }
                )
            }
            Xrm.Utility.closeProgressIndicator();
        } catch (error) {
            Xrm.Utility.closeProgressIndicator();
            console.error(error);
        }


    },

    getContactInfo: async function (formContext) {

        try {

            var contactField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.Customer);

            if (contactField.getValue() === null)
                return null;
            
            var contact = await this.getContactFromWebAPI(formContext, Xrm);
            
            var idPatient = null;
            var countryCode = null; 

            if(contact[Abox.SharedLogic.Entities.ContactFields.IdAboxPatient]!==null){
               idPatient= contact[Abox.SharedLogic.Entities.ContactFields.IdAboxPatient];
            }

            if (contact[Abox.SharedLogic.Entities.ContactSchemas.Country] !== null) {
                countryCode = contact[Abox.SharedLogic.Entities.ContactSchemas.Country][Abox.SharedLogic.Entities.CountryFields.IdCountry];
            }
            
            if (idPatient === null || idPatient === "" || typeof idPatient === "undefined") {
                Xrm.Navigation.openAlertDialog({ title:"Id de paciente no encontrado", text: "Este contacto no posee un Id de paciente Abox registrado y no es posible continuar con el proceso." });
                return null;
            }

            if (countryCode === null || countryCode === "" || typeof countryCode === "undefined") {
                Xrm.Navigation.openAlertDialog({ title:"País no identificado",text: "No se ha podido identificar el país del contacto seleccionado" });
                return null;
            }

            var products = await this.getProductsForPatient(idPatient, countryCode);
            
            return {
                idPatient: idPatient,
                countryCode: countryCode,
                products: products
            }
        } catch (error) {
            Xrm.Navigation.openErrorDialog({ details: error.stack, message: "Ocurrió un error al obtener la información del contacto seleccionado, por favor intente nuevamente." });
            return null;
        }


    },
    getInvoiceInfo: function (formContext) {

        try {
            var products = this.getProductsFromInvoice(formContext);
            var invoiceNumber = null;

            var invoiceNumberField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.InvoiceNumber);
            if (invoiceNumberField !== null) {
                if (invoiceNumberField.getValue() !== null) {
                    invoiceNumber = invoiceNumberField.getValue();
                }
            }
            return {
                invoiceNumber: invoiceNumber,
                products: products
            }
        } catch (error) {
            console.error(error);
            return null;
        }

    },

    setInvoiceImageControl: function (formContext) {

        var imageWebResourceControl = formContext.getControl(Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebResource);
        var imageUrlField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl);
        var imageUrl = "";
        if (imageUrlField !== null) {
            if (imageUrlField.getValue() !== null) {
                imageUrl = imageUrlField.getValue();
            }

            imageUrlField.addOnChange(function () {

                if (imageUrlField.getValue() !== null) {
                    formContext.ui.clearFormNotification("imageNotSelected");
                }

            });

        }


        if (imageWebResourceControl) {
            imageWebResourceControl.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setImageOnChangeLogic(formContext, Abox.SharedLogic.Entities.InvoiceFields, Abox.SharedLogic.AboxServices.AboxImageUploadUrl, Xrm);
                    if (imageUrl !== "") {
                        contentWindow.setImageElementSrc(imageUrl);
                    }
                }
            )
        }



    },

    setOnSaveAction: function (formContext, executionContext) {

        formContext.data.entity.addOnSave(function (ex) {
            console.log(ex);
        });

    },



    validateUpdateAvailability: function (formContext) {


        var idAboxInvoiceField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.IdAboxInvoice);

        if (idAboxInvoiceField.getValue() === null || idAboxInvoiceField.getValue() === "null") {
            formContext.ui.setFormNotification("Esta factura no posee un ID de Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        }

    },

    setUpdateFormLogic: function (formContext) {

        var that = this;


    },


    setFieldNotification: function (value, message, control, maxLength, minLength) {

        value.toString();

        if (typeof maxLength === "undefined") {
            maxLength = null;
        }

        if (typeof minLength === "undefined") {
            minLength = null;
        }

        if ((value.length > maxLength) && (maxLength !== null)) {
            control.setNotification("El valor debe ser de máximo " + maxLength + " caracteres");
        }
        else if ((value.length < minLength) && (minLength !== null)) {
            control.setNotification("El valor debe ser de mínimo " + minLength + " caracteres");
        } else {
            control.clearNotification();
        }

    },

    getProductsForPatient: async function (patientId, countryCode) {

        var productsForPatient = [];

        try {
            var url = Abox.SharedLogic.AboxServices.ProductsSearch;
            var headers = [
                {
                    key: "Authorization",
                    value: "Bearer " + Abox.SharedLogic.Constants.TokenForAboxServices
                },
                {
                    key: "Content-Type",
                    value: "application/json"
                }
            ];

            var json = JSON.stringify({ "patientId": patientId, "productid": "", "countryid": countryCode.toUpperCase(), "textsearch": "", "paging": { "page": 1, "pagesize": 2000 }, "sorter": { "column": "FAMILIA", "direction": 1 } });

            var response = await Abox.SharedLogic.DoPostRequest(url, json, headers);
            console.log("respuesta productos");
            console.log(response);

            var jsonResponse = await response.json();
            console.log("respuesta productos json");
            console.log(jsonResponse);

            if (typeof jsonResponse !== "undefined") {

                if (typeof jsonResponse.response.details.products !== "undefined") {
                    if (typeof jsonResponse.response.details.products !== "undefined") {
                        productsForPatient = jsonResponse.response.details.products;
                        console.log(productsForPatient);
                    }
                }
            }

        } catch (error) {
            console.error(error);
            productsForPatient = null;
        }

        return productsForPatient;


    },

    getProductsFromInvoice: function (formContext) {

        var invoiceProducts = null;

        try {

            var productsJsonField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.ProductsSelectedJson);

            if (productsJsonField !== null) {
                if (productsJsonField.getValue() !== null) {
                    invoiceProducts = JSON.parse(productsJsonField.getValue());
                }
            }

        } catch (error) {
            console.error(error);
            invoiceProducts = null;
        }

        return invoiceProducts;


    },

    getContactFromWebAPI: async function (formContext, Xrm) {
        try {
            var contactField = formContext.getAttribute(Abox.SharedLogic.Entities.InvoiceFields.Customer);
            var contactRetrieved = null;
            if (contactField !== null) {
                var contactValue = contactField.getValue();
                if (contactValue !== null) {

                    entityName = contactValue[0].entityType;
                    id = contactValue[0].id.slice(1, -1);
                    console.info("consultando contacto webapi...");

                    //?$select=new_idaboxpatient&$expand=new_CountryId($select=new_idcountry)
                    contactRetrieved = await Abox.SharedLogic.retrieveRecordFromWebAPI(entityName, id, "?$select=" + Abox.SharedLogic.Entities.ContactFields.IdAboxPatient + "&$expand=" + Abox.SharedLogic.Entities.ContactSchemas.Country + "($select=" + Abox.SharedLogic.Entities.CountryFields.IdCountry + ")", Xrm);
                    console.log(contactRetrieved);
                    Xrm.Utility.closeProgressIndicator();

                } else {
                    throw ("Valor de contacto no encontrado.");
                }

            } else {
                throw ("Contacto no encontrado");
            }

            return contactRetrieved;

        } catch (error) {
            Xrm.Utility.closeProgressIndicator();
            console.error("error consultando datos mediante webapi");
            console.log(error);
            return null;
        }
    }

};
