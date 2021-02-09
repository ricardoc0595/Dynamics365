if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.SharedLogic = {
    test: function () {
        alert("test Call");
    },

    Configuration: {

        Environment: "https://apidev.aboxplan.com",
        WebAPIEnvironment: "https://aboxcrmapidev.aboxplan.com",
        TokenForAboxServices: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImNybV9hYm94YXBpIiwiYXBpIjp0cnVlLCJpYXQiOjE2MDMzMTIzODB9.Cu8FYQoVWDcof_qFZ5CIA6K2OYloOEn9F-b_XahLf9w",
    },

    Entities: {

        ContactFields: {
            EntityId: "contactid",
            IdType: "new_idtype",
            Id: "new_id",
            UserType: "new_usertype",
            Firstname: "firstname",
            Lastname: "lastname",
            SecondLastname: "new_secondlastname",
            Password: "new_password",
            Email: "emailaddress1",
            Phone: "telephone2",
            SecondaryPhone: "mobilephone",
            Gender: "gendercode",
            Birthdate: "birthdate",
            ProductxContactId: "new_productcontactid",
            RegisterDay: "new_registerday",
            IdAboxPatient: "new_idaboxpatient",
            Country: "new_countryid",
            ContactxDoctorRelationship: "new_contact_new_doctor",
            ContactxProductRelationship: "new_product_contact",
            ContactxContactRelationship: "new_contact_contact",
            ContactxDoseRelationship: "new_contact_new_dose",
            Canton: "new_canton",
            District: "new_distrit",
            Province: "new_cityid",
            Interests: "new_clientinterest",
            ContactxContactLookup: "new_contactcontactid",
            IsChildContact: "new_ischildcontact",
            CountryLookup: "new_countryid",
            CityLookup: "new_cityid",
            CantonLookup: "new_canton",
            DistrictLookup: "new_distrit",
            NoEmail: "new_noemail",
            OtherInterestLookup: "new_otherinterest",
            IsUserTypeChange: "new_isusertypechange",
            ChangePasswordWebResource: "WebResource_changepassword"
        },
        ContactSchemas: {
            UserType: "new_UserType",
            ContactxDoseRelationship: "",
            Country: "new_CountryId",
            Province: "new_CityId",
            Canton: "new_Canton",
            District: "new_Distrit",
        },
        CountryFields: {

            EntityId: "new_countryid",
            IdCountry: "new_idcountry",
            Name: "new_name",
        },
        InvoiceFields: {
            InvoiceImageWebResource: "WebResource_invoiceimage",
            InvoiceImageWebUrl: "new_aboximageurl",
            InvoiceNumber: "new_invoicenumber",
            CaseInvoiceLookup: "new_caseinvoiceid",
            Customer: "customerid",
            Contact: "new_contactid",
            Country: "new_invoiceCountry",
            Pharmacy: "new_pharmacy",
            PurchaseDate: "new_purchasedate",
            EntityId: "invoiceid",
            ProductSelectionWebResource: "WebResource_invoiceproductselection",
            ProductsSelectedJson: "new_productsselectedjson",
            IdAboxInvoice: "new_idaboxinvoice"
        }

    },

    GetCountryCodeFromGuid: function (guidCountry) {

        var countryCode = "";
        switch (guidCountry.toLowerCase()) {
            case this.Constants.CRCountryIdLookup:
                countryCode = "CR"
                break;

            case this.Constants.HNCountryIdLookup:
                countryCode = "HN"
                break;

            case this.Constants.GTCountryIdLookup:
                countryCode = "GT"
                break;

            case this.Constants.PACountryIdLookup:
                countryCode = "PA"
                break;

            case this.Constants.DOCountryIdLookup:
                countryCode = "DO"
                break;

            case this.Constants.NICountryIdLookup:
                countryCode = "NI"
                break;

            default:
                countryCode = null;
                break;

        }

        return countryCode;

    },

    Constants: {

        PatientIdType: "15810a1e-c8d1-ea11-a812-000d3a33f637",
        CareTakerIdType: "fab60b2a-c8d1-ea11-a812-000d3a33f637",
        TutorIdType: "f4761324-c8d1-ea11-a812-000d3a33f637",
        OtherInterestIdType: "30f90330-c8d1-ea11-a812-000d3a33f637",
        MaleGenderValue: 1,
        FemaleGenderValue: 2,
        NationalIdValue: 1,
        ForeignerIdValue: 2,
        MinorIdValue: 3,
        DoseFrequencyOnePerDay: 1,
        DoseFrequencyTwoPerDay: 2,
        DoseFrequencyThreePerDay: 3,
        DoseFrequencyFourPerDay: 4,
        DoseFrequencyOther: 5,
        TokenForAboxServices: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImNybV9hYm94YXBpIiwiYXBpIjp0cnVlLCJpYXQiOjE2MDMzMTIzODB9.Cu8FYQoVWDcof_qFZ5CIA6K2OYloOEn9F-b_XahLf9w",
        CRCountryIdLookup: "c76fa4f3-2bfc-ea11-a815-000d3a30f195",
        HNCountryIdLookup: "cf6fa4f3-2bfc-ea11-a815-000d3a30f195",
        GTCountryIdLookup: "cb6fa4f3-2bfc-ea11-a815-000d3a30f195",
        PACountryIdLookup: "c96fa4f3-2bfc-ea11-a815-000d3a30f195",
        DOCountryIdLookup: "cd6fa4f3-2bfc-ea11-a815-000d3a30f195",
        NICountryIdLookup: "24f43452-b01e-eb11-a813-00224803f71b",
        GeneralFrontendErrorMessage: "Ha ocurrido un error en alguno de los componentes de este formulario, por favor intente nuevamente o contacte con soporte.",
        GeneralAboxServicesErrorMessage: "Ocurrió un error consultando los servicios de Abox Plan \n",
        ErrorMessageCodeReturned: "Error en transacción, Código de respuesta servicio:",
        ErrorMessageTransactionCodeReturned: "Ocurrió un error al guardar la información en Abox Plan:\n",
        GeneralPluginErrorMessage: "Ocurrió un error en la ejecución de un Plugin interno, por favor intenta nuevamente o comunícate con soporte.",
        ApplicationIdWebAPI: "WEBAPI",
        ApplicationIdPlugin: "PLUGIN",
        SubGridControls: {
            RelatedContacts: "RelatedContacts"
        },


    },

    get AboxServices() {
        return{
            AboxImageUploadUrl: this.Configuration.Environment + "/files/upload",
            ProductsSearch: this.Configuration.Environment + "/products/search",
            ChangePasswordCrm: this.Configuration.Environment + "/security/crm/changepassword"
        }
    },

    DataFormats: {
        getAllDataFormats: function () {

            var Formats = [
                { countryCode: "506", pais: "CR", MinID: "9", MaxID: "9", Alpha: false, Num: true, PHID: "xxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxxx", Address1: "Provincia", Address2: "Cantón", Address3: "Distrito", txtAddress1: "Selecciona una provincia", txtAddress2: "Selecciona un cantón", txtAddress3: "Selecciona un distrito", MinPhone: "8" },
                { countryCode: "505", pais: "Nic", MinID: "14", MaxID: "14", Alpha: true, Num: false, PHID: "xxxxxxxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxxx", Address1: "0", Address2: "0", Address3: "0", txtAddress1: "0", txtAddress2: "0", txAddress3: "0", MinPhone: "8" },
                { countryCode: "503", pais: "Sal", MinID: "9", MaxID: "9", Alpha: false, Num: true, PHID: "xxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxxx", Address1: "0", Address2: "0", Address3: "0", txtAddress1: "0", txtAddress2: "0", txAddress3: "0", MinPhone: "8" },
                { countryCode: "507", pais: "PA", MinID: "6", MaxID: "9", Alpha: true, Num: false, PHID: "xxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxx", Address1: "Provincia", Address2: "Distrito", Address3: "Corregimiento", txtAddress1: "Selecciona una provincia", txtAddress2: "Selecciona un distrito", txtAddress3: "Selecciona un corregimiento", MinPhone: "7" },
                { countryCode: "1", pais: "DO", MinID: "11", MaxID: "11", Alpha: false, Num: true, PHID: "xxxxxxxxxxx", MaxMinPhone: "10", PHPhone: "xxxxxxxxxx", Address1: "Provincia", Address2: "Ciudad", Address3: "0", txtAddress1: "Selecciona una provincia", txtAddress2: "Selecciona una ciudad", txtAddress3: "0", MinPhone: "8" },
                { countryCode: "504", pais: "HN", MinID: "13", MaxID: "13", Alpha: false, Num: true, PHID: "xxxxxxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxxx", Address1: "Departamento", Address2: "Municipio", Address3: "0", txtAddress1: "Selecciona un departamento", txtAddress2: "Selecciona un municipio", txtAddress3: "0", MinPhone: "8" },
                { countryCode: "502", pais: "GT", MinID: "13", MaxID: "13", Alpha: false, Num: true, PHID: "xxxxxxxxxxxxx", MaxMinPhone: "8", PHPhone: "xxxxxxxx", Address1: "Departamento", Address2: "Municipio", Address3: "0", txtAddress1: "Selecciona un departamento", txtAddress2: "Selecciona un municipio", txtAddress3: "0", MinPhone: "8" },
            ]

            return Formats;

        },
        getDataFormatsByCountry: function (country) {
            var allFormats = this.getAllDataFormats();
            var countryFormats = null;

            for (var i = 0; i < allFormats.length; i++) {
                if (allFormats[i].pais.toLowerCase() === country.toLowerCase()) {
                    countryFormats = allFormats[i];
                    break;
                }
            }
            return countryFormats;
        },

    },

    disableFields: function (formContext, fieldsToDisable) {

        fieldsToDisable.forEach(function (fieldName) {

            //var field = formContext.getAttribute(fieldName);
            var fieldControl = formContext.getControl(fieldName);

            if (fieldControl !== null) {
                fieldControl.setDisabled(true);
            }
        });

    },

    clearTextFields: function (formContext, fieldsToClear) {
        if (typeof fieldsToClear !== "undefined" && formContext !== "undefined") {

            if (fieldsToClear.constructor === Array) {
                fieldsToClear.forEach(function (fieldName) {

                    //var field = formContext.getAttribute(fieldName);
                    var field = formContext.getAttribute(fieldName);
                    if (field !== null) {
                        field.setValue("");
                    }
                });
            }

        }
    },

    enableFields: function (formContext, fieldsToEnable) {

        fieldsToEnable.forEach(function (fieldName) {

            //var field = formContext.getAttribute(fieldName);
            var fieldControl = formContext.getControl(fieldName);

            if (fieldControl !== null) {
                fieldControl.setDisabled(false);
            }
        });

    },

    DoPostRequest: async function (url, json, headers) {

        try {
            var myHeaders = new Headers();


            headers.forEach(function (header) {
                myHeaders.append(header.key, header.value);
            });

            // myHeaders.append("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6InR1dG9yMDIxMjIwMDIiLCJpYXQiOjE2MDg2NTM4ODgsImV4cCI6MTYwODc0MDI4OH0.5_MaCvTvzmMJwCXQ8AmyFcvAlUJkbL3_brKtPlQ_h7w");
            // myHeaders.append("Content-Type", "application/json");

            var raw = json;

            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                // redirect: 'follow'
            };

            var response = await fetch(url, requestOptions);
            console.log("respuesta fetch");
            console.log(response);
            return response;

        } catch (error) {
            console.error(error);
        }

    },

    retrieveRecordFromWebAPI: async function (entityName, entityId, options, Xrm) {
        Xrm.Utility.showProgressIndicator("");
        var entityResponse = null;


        return new Promise(function (resolve, reject) {
            Xrm.WebApi.retrieveRecord(entityName, entityId, options).then(function (response) {

                entityResponse = response;
                // return entityResponse;
                resolve(entityResponse);
            }, function (error) {
                Xrm.Utility.closeProgressIndicator();
                entityResponse = error;
                // return error;
                reject(entityResponse);
            });

        })

    },

    setFieldsRequired: function (formContext, fields) {
        if (typeof fields !== "undefined" && formContext !== "undefined") {

            if (fields.constructor === Array) {
                fields.forEach(function (fieldName) {

                    //var field = formContext.getAttribute(fieldName);
                    var field = formContext.getAttribute(fieldName);
                    if (field !== null) {
                        field.setRequiredLevel("required");
                    }
                });
            }

        }
    },
    setFieldsNonRequired: function (formContext, fields) {
        if (typeof fields !== "undefined" && formContext !== "undefined") {

            if (fields.constructor === Array) {
                fields.forEach(function (fieldName) {

                    //var field = formContext.getAttribute(fieldName);
                    var field = formContext.getAttribute(fieldName);
                    if (field !== null) {
                        field.setRequiredLevel("none");
                    }
                });
            }

        }
    },

    setFieldsVisible: function (formContext, fields) {
        if (typeof fields !== "undefined" && formContext !== "undefined") {

            if (fields.constructor === Array) {
                fields.forEach(function (fieldName) {

                    //var field = formContext.getAttribute(fieldName);
                    var fieldControl = formContext.getControl(fieldName);
                    if (fieldControl !== null) {
                        fieldControl.setVisible(true);
                    }
                });
            }

        }
    },
    setFieldsInvisible: function (formContext, fields) {
        if (typeof fields !== "undefined" && formContext !== "undefined") {

            if (fields.constructor === Array) {
                fields.forEach(function (fieldName) {

                    //var field = formContext.getAttribute(fieldName);
                    var fieldControl = formContext.getControl(fieldName);
                    if (fieldControl !== null) {
                        fieldControl.setVisible(false);
                    }
                });
            }

        }
    },

    elementAlreadyInList: function (idElement, list, idProperty) {

        //Funcion que se llama para verificar si el elemento que intenta agregar el usuario
        //Ya se encuentra en la lista recibe parametros: id del elemento, lista en donde buscar, nombre de la propiedad del objeto que contiene el id

        var length = list.length;
        var valueAlreadyExists = false;

        try {
            for (var i = 0; i < length; i++) {
                if (list[i][idProperty] === null || typeof list[i][idProperty] === "undefined") {
                    continue
                } else {
                    if (list[i][idProperty] === idElement) {
                        valueAlreadyExists = true;
                        break;
                    }

                }

            }
            if (valueAlreadyExists) {
                return true;
            } else {
                return false;
            }

        } catch (e) {
            return false;
        }

    },

    deleteItemFromList: function (idItem, list, idProperty) {

        idItem = idItem || null;
        list = list || [];
        idProperty = idProperty || null;

        var deleteIndex = null;
        var deleted = false;
        var length = list.length;

        try {
            for (var i = 0; i < length; i++) {

                if (list[i][idProperty].toString() === idItem.toString()) {
                    deleteIndex = i;
                    break;
                }

            }

            if (deleteIndex !== null) {
                list.splice(deleteIndex, 1);
                deleted = true;
            }
        } catch (e) {
            console.log("excepcion:" + e);
        }



        return deleted;

    },
    findElement: function (elementId, list, idProperty) {
        //Funcion que se llama para buscar un elemento en una lista 
        // recibe parametros: id del elemento, lista en donde buscar, nombre de la propiedad del objeto que contiene el id


        elementId = elementId || null;
        var found = null;

        try {
            if (elementId !== null) {

                var length = list.length;
                for (var i = 0; i < length; i++) {
                    if (list[i][idProperty] === null || typeof list[i][idProperty] === "undefined") {
                        continue
                    } else {
                        if (list[i][idProperty].toString() === elementId.toString()) {
                            var obj = list[i];
                            found = obj;
                            break;
                        }
                    }


                }
                return found;

            }

        } catch (e) {
            return found;
        }


    }


};

