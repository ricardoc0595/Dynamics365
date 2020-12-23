if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.InvoiceFunctions = {

    //Método que se ejecuta al cargar el formulario de contacto
    
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
        if (formType === 1) {

            Abox.SharedLogic.disableFields(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl]);
            Abox.SharedLogic.setFieldsRequired(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl]);
            this.setInvoiceImageControl(formContext);

        } else if (formType === 2) {
            
            Abox.SharedLogic.disableFields(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl]);
            Abox.SharedLogic.setFieldsRequired(formContext, [Abox.SharedLogic.Entities.InvoiceFields.InvoiceImageWebUrl]);
            this.setInvoiceImageControl(formContext);
            
            var contactData = this.getContactInfo(formContext);

            contactData.then(function(data){
                that.setInvoiceProductSelectionComponent(formContext,data);
            },function(error){
                console.error(error);
            });

        }

    },

    setInvoiceProductSelectionComponent: function (formContext,contactData) {

        try {
            Xrm.Utility.showProgressIndicator("Inicializando componentes, por favor espere...");
            var productSelectionWebResourceControl = formContext.getControl(Abox.SharedLogic.Entities.InvoiceFields.ProductSelectionWebResource);
            var that = this;

            if (productSelectionWebResourceControl) {
                productSelectionWebResourceControl.getContentWindow().then(
                    function (contentWindow) {
                        contentWindow.initializeProductSelectionLogic(formContext, Abox.SharedLogic, Xrm, Abox.SharedLogic.Entities.InvoiceFields,contactData);
                        Xrm.Utility.closeProgressIndicator();
                    }, function (error) {
                        Xrm.Utility.closeProgressIndicator();
                        console.error(error);
                        throw "Error inicializando el componente de productos";
                    }
                )
            }
        } catch (error) {
            Xrm.Utility.closeProgressIndicator();
            console.error(error);
        }


    },

    getContactInfo:async function(formContext){

        var contact = await this.getContactFromWebAPI(formContext, Xrm);
        console.log(contact[Abox.SharedLogic.Entities.ContactFields.IdAboxPatient]);
        var idPatient = contact[Abox.SharedLogic.Entities.ContactFields.IdAboxPatient];
        var countryCode = contact[Abox.SharedLogic.Entities.ContactSchemas.Country][Abox.SharedLogic.Entities.CountryFields.IdCountry];
        var products = await this.getProductsForPatient(idPatient, countryCode);

        return {
            idPatient: idPatient,
            countryCode: countryCode,
            products: products
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
        }


        if (imageWebResourceControl) {
            imageWebResourceControl.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setImageOnChangeLogic(formContext, Abox.InvoiceFunctions.InvoiceFields, Abox.SharedLogic.AboxServices.AboxImageUploadUrl, Xrm);
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


        // var idPatientField = formContext.getAttribute(this.ContactFields.IdAboxPatient);

        // if (idPatientField.getValue() === null || idPatientField.getValue() === "null") {

        //     formContext.ui.setFormNotification("Este contacto no posee un ID de paciente Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        // }



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

    getProductsForPatient: async function(patientId,countryCode){

        var productsForPatient = [];

        try {
            var url = Abox.SharedLogic.AboxServices.ProductsSearch;
            var headers = [
                {
                    key: "Authorization",
                    value: "Bearer "+Abox.SharedLogic.Constants.TokenForAboxServices
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
            return null;
        }

        return productsForPatient;
        

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
                    contactRetrieved = await Abox.SharedLogic.retrieveRecordFromWebAPI(entityName, id, "?$select=" + Abox.SharedLogic.Entities.ContactFields.IdAboxPatient+"&$expand="+Abox.SharedLogic.Entities.ContactSchemas.Country+"($select="+Abox.SharedLogic.Entities.CountryFields.IdCountry+")", Xrm);
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

