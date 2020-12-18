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

        if (formType === 1) {

            Abox.SharedLogic.disableFields(formContext,[this.InvoiceFields.InvoiceImageWebUrl]);
            Abox.SharedLogic.setFieldsRequired(formContext,[this.InvoiceFields.InvoiceImageWebUrl]);
            this.setInvoiceImageControl(formContext);

        } else if (formType === 2) {

            Abox.SharedLogic.disableFields(formContext,[this.InvoiceFields.InvoiceImageWebUrl]);
            Abox.SharedLogic.setFieldsRequired(formContext,[this.InvoiceFields.InvoiceImageWebUrl]);
            this.setInvoiceImageControl(formContext);
            this.setInvoiceProductSelectionComponent(formContext);
            
        }

    },

    setInvoiceProductSelectionComponent:function(formContext){
        
        try {
            Xrm.Utility.showProgressIndicator("Inicializando componentes, por favor espere...");
            var productSelectionWebResourceControl = formContext.getControl(this.InvoiceFields.ProductSelectionWebResource);
            var that = this;

            if (productSelectionWebResourceControl) {
                productSelectionWebResourceControl.getContentWindow().then(
                    function (contentWindow) {
                        contentWindow.initializeProductSelectionLogic(formContext, Abox.SharedLogic, Xrm, that.InvoiceFields);
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

    setInvoiceImageControl:function(formContext){

        var imageWebResourceControl = formContext.getControl(this.InvoiceFields.InvoiceImageWebResource);
        var imageUrlField = formContext.getAttribute(this.InvoiceFields.InvoiceImageWebUrl);
        var imageUrl = "";
        if (imageUrlField !== null) {
            if (imageUrlField.getValue() !== null) {
                imageUrl = imageUrlField.getValue();
            }
        }

        
        if (imageWebResourceControl) {
            imageWebResourceControl.getContentWindow().then(
                function (contentWindow) {
                    contentWindow.setImageOnChangeLogic(formContext, Abox.InvoiceFunctions.InvoiceFields, Abox.SharedLogic.Constants.AboxImageUploadUrl, Xrm);
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

    InvoiceFields: {
        InvoiceImageWebResource:"WebResource_invoiceimage",
        InvoiceImageWebUrl:"new_aboximageurl",
        InvoiceNumber:"new_invoicenumber",
        CaseInvoiceLookup:"new_caseinvoiceid",
        Customer:"new_Customer",
        Country:"new_invoiceCountry",
        Pharmacy:"new_pharmacy",
        PurchaseDate:"new_purchasedate",
        EntityId:"invoiceid",
        ProductSelectionWebResource:"WebResource_invoiceproductselection",
        ProductsSelectedJson:"new_productsselectedjson"
    },

    validateUpdateAvailability: function (formContext) {


        // var idPatientField = formContext.getAttribute(this.ContactFields.IdAboxPatient);

        // if (idPatientField.getValue() === null || idPatientField.getValue() === "null") {

        //     formContext.ui.setFormNotification("Este contacto no posee un ID de paciente Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        // }



    },

    setInitialData: function (formContext) {



    },

    setCreateFormLogic: function (formContext) {




    }
    ,


    setUpdateFormLogic: function (formContext) {

        var that=this;
       

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

   
    


};

