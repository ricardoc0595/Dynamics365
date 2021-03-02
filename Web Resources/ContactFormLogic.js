if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.ContactFunctions = {

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

            this.setFieldsControlsAndAlerts(formContext);
            this.disableFieldsUntilCountrySelected(formContext);
            this.setUnderCareLogic(formContext);

            var fieldNames = [Abox.SharedLogic.Entities.ContactFields.IsUserTypeChange, Abox.SharedLogic.Entities.ContactFields.ChangePasswordWebResource, Abox.SharedLogic.Entities.ContactFields.ContactxContactLookup];
            Abox.SharedLogic.setFieldsInvisible(formContext, fieldNames);

            //Para poder implementar un mensaje mas amigable, hay que hacer un propio boton de guardar customizado y llamar al metodo save() del api de dynamics

            //this.setOnSaveAction(formContext,executionContext);

        } else if (formType === 2) {

            this.validateUpdateAvailability(formContext);
            this.setFieldsControlsAndAlerts(formContext);
            this.setUpdateFormLogic(formContext, Xrm);

        }

    },


    insertInvoiceInCrm: async function (formContext, invoiceToCreate) {


        /////
        try {

            var url = Abox.SharedLogic.AboxCrmAPIServices.CreateInvoice;

            // var url =  Xrm.Page.context.getClientUrl() + `/api/data/v9.1/invoices`;

            // var headers = [
            //     { key: "OData-MaxVersion", value: "4.0" },
            //     { key: "OData-Version", value: "4.0" },
            //     { key: "Accept", value: "application/json" },
            //     { key: "Content-Type", value: "application/json; charset=utf-8" },
            //     { key: "Prefer", value: "return=representation" },
            //     { key: "MSCRM.SuppressDuplicateDetection", value: "false" },
            //     { key: "MSCRMCallerID", value: "7dbf49f3-8be8-ea11-a817-002248029f77" }
            // ];

            var headers = [
                {
                    key: "Authorization",
                    value: "Bearer " + Abox.SharedLogic.Configuration.TokenForWebAPI
                },
                {
                    key: "Content-Type",
                    value: "application/json"
                }
            ];

            var products = [];

            invoiceToCreate.products.forEach(function (prod) {
                products.push({ "id": prod.id, "quantity": prod.quantity });
            });





            var json = JSON.stringify({
                "patientId": invoiceToCreate.patientId,
                "pharmacyId": invoiceToCreate.pharmacyId,
                "billId": invoiceToCreate.purchaseNumber,
                "idFromDatabase": invoiceToCreate.purchaseId,
                "billDate": invoiceToCreate.purchaseDate,
                "billImageUrl": invoiceToCreate.purchaseImage,
                "products": products,
                "country": invoiceToCreate.country,
                "status": invoiceToCreate.status,
                "statusReason": invoiceToCreate.statusReason,
                "totalAmount": invoiceToCreate.totalAmount,
                "revisionTime1": invoiceToCreate.revisionTime1,
                "revisionTime2": invoiceToCreate.revisionTime2,
                "purchaseMethod": invoiceToCreate.purchaseMethod,

            });


            //usando web api

            //  var json = JSON.stringify({
            //     "customerid_contact@odata.bind": "/contacts(new_idaboxpatient="+ invoiceToCreate.patientId+")",
            //     "new_Customer_contact@odata.bind": "/contacts(new_idaboxpatient="+ invoiceToCreate.patientId+")",
            //     "new_InvoiceCountry@odata.bind": "/new_countries(new_idcountry='"+invoiceToCreate.country+"')",
            //     "new_Pharmacy@odata.bind": "/new_pharmacies(new_idpharmacy='"+invoiceToCreate.pharmacyId+"')",
            //     "new_aboximageurl": invoiceToCreate.purchaseImage,
            //     "new_idaboxinvoice": invoiceToCreate.purchaseId,
            //     "new_invoicenumber": invoiceToCreate.purchaseNumber,
            //     "new_productsselectedjson":JSON.stringify(products),
            //     "new_purchasedate": invoiceToCreate.purchaseDate,
            //     "new_revisiontime1": invoiceToCreate.revisionTime1,
            //     "new_revisiontime2": invoiceToCreate.revisionTime2,
            //     "new_statusreason": invoiceToCreate.statusReason,
            //     "new_invoicestatus":invoiceToCreate.status,
            //     "totalamount": invoiceToCreate.totalAmount,
            //     "new_purchasemethod":invoiceToCreate.purchaseMethod
            //  }
            //  );




            var response = await Abox.SharedLogic.DoPostRequest(url, json, headers);
            return response;


        } catch (error) {

            console.error(error);
            return null;
        }




    },


    loadPatientInvoicesOnDemand: async function (formContext, Xrm) {


        var currentContact = formContext.data.entity;
        var currentContactAttributes = currentContact.attributes._collection;
        //validar si el usuario es paciente o bajo cuido, pues solo a estos se le registran las compras
        var userTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.UserType);
        var idUserType = "";
        if (userTypeField !== null) {

            if (userTypeField.getValue() !== null) {
                idUserType = userTypeField.getValue()[0].id.slice(1, -1)
            }
        }

        var invoicesAlreadyImported = false;
        var attribute = currentContactAttributes[Abox.SharedLogic.Entities.ContactFields.InvoicesAlreadyImported];
        if (attribute !== null) {
            invoicesAlreadyImported = !!attribute.getValue();
        }

        if ((idUserType.toLowerCase() === Abox.SharedLogic.Constants.PatientIdType) || (idUserType.toLowerCase() === Abox.SharedLogic.Constants.PatientUndercareIdType)) {

            //ejecutar la importacion solo la primera vez
            if (!invoicesAlreadyImported) {

                console.time();

                var patientAboxIdField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdAboxPatient);
                if (patientAboxIdField !== null) {
                    if (patientAboxIdField.getValue() !== null) {
                        patientAboxId = patientAboxIdField.getValue();
                    }
                }


                try {
                    var patientPurchases = [];

                    //obtener las facturas que ya tiene el usuario y evitar llamar el crm Web api

                    var response = await Abox.SharedLogic.DoGetRequest("https://api.jsonbin.io/b/6028ba733b303d3d96505f86/11", [
                        {
                            key: "Content-Type",
                            value: "application/json"
                        }
                    ]);

                    if (response !== null) {

                        var obj = await response.json();

                        console.table(obj.Purchases);

                        for (let i = 0; i < obj.Purchases.length; i++) {
                            const purchase = obj.Purchases[i];

                            var invoiceFound = false;
                            var error = false;


                            var urlx = Xrm.Page.context.getClientUrl() + "/api/data/v9.1/invoices(new_idaboxinvoice=" + parseInt(purchase.purchaseId) + ")?$select=new_idaboxinvoice";

                            //agregar header de autorizacion
                            // var urlx = Abox.SharedLogic.AboxCrmAPIServices.GetInvoiceByAboxId + purchase.purchaseId;
                            var responsex = await Abox.SharedLogic.DoGetRequest(urlx, []);

                            console.log(responsex);

                            if (responsex !== null) {

                                if (responsex.status === 404) {
                                    invoiceFound = false;
                                } else if (responsex.status === 200) {
                                    invoiceFound = true;
                                } else {
                                    invoiceFound = false;
                                    error = true;
                                }

                                if (!error) {

                                    if (!invoiceFound) {

                                        try {
                                            var inv = await this.insertInvoiceInCrm(formContext, purchase);
                                        } catch (error) {
                                            console.error(error);
                                            continue;
                                        }


                                    }
                                } else {
                                    continue;
                                }
                            } else {
                                // Xrm.Navigation.openErrorDialog({ details: `Error consultando ${urlx} para traer una factura`, message: "Ocurrió un error al consultar las facturas de este paciente." });
                                continue;
                            }

                        }

                        //Actualizar el field de invoicesAlreadyImported para que no vuelva a ejecutarse este proceso

                        // define the data to update a record
                        var data = {};
                        data[Abox.SharedLogic.Entities.ContactFields.InvoicesAlreadyImported] = true;
                        // update the record

                        var urlUpdateDynamicsWebAPI = Xrm.Page.context.getClientUrl() + `/api/data/v9.1/contacts(${currentContact._entityId.guid})`;

                        var headers = [
                            { key: "OData-MaxVersion", value: "4.0" },
                            { key: "OData-Version", value: "4.0" },
                            { key: "Accept", value: "application/json" },
                            { key: "Content-Type", value: "application/json; charset=utf-8" },
                            { key: "Prefer", value: "odata.include-annotations=\"*\"" },
                            { key: "MSCRMCallerID", value: "7dbf49f3-8be8-ea11-a817-002248029f77" }
                        ];

                        var json = JSON.stringify(data);
                        try {
                            var response = await Abox.SharedLogic.DoPatchRequest(urlUpdateDynamicsWebAPI, json, headers);

                        } catch (error) {
                            console.error(error);
                        }

                        console.timeEnd();


                    } else {
                        return null;
                    }


                    console.log("Fin patientPurchasesOnDemand");

                    return true;

                } catch (error) {
                    console.log(error);
                    // Xrm.Navigation.openErrorDialog({ details: `stacktrace: ${error.stack}`, message: "Ocurrió un error consul"});
                    return null;
                }


            }




        } else {
            return;
        }
        //llamar al servicio que trae las compras del paciente

    },



    validateUpdateAvailability: function (formContext) {


        var idPatientField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdAboxPatient);

        if (idPatientField.getValue() === null || idPatientField.getValue() === "null") {

            formContext.ui.setFormNotification("Este contacto no posee un ID de paciente Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        }



    },




    setUpdateFormLogic: function (formContext, Xrm) {

        var contactFields = Abox.SharedLogic.Entities.ContactFields;
        var that = this;
        var isChildContact = false;
        var isChildContactField = formContext.getAttribute(contactFields.IsChildContact);


        if (isChildContactField !== null) {
            if (isChildContactField.getValue() !== null) {
                isChildContact = isChildContactField.getValue();
            }
        }

        Abox.SharedLogic.setFieldsNonRequired(formContext, [contactFields.Password, contactFields.OtherInterestLookup, contactFields.Interests])
        Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.IsChildContact]);
        Abox.SharedLogic.disableFields(formContext, [contactFields.Id, contactFields.IdType, contactFields.OtherInterestLookup, contactFields.CountryLookup, contactFields.ContactxContactLookup]);



        var userTypeField = formContext.getAttribute(contactFields.UserType);
        var idUserType = "";


        if (userTypeField !== null) {
            if (userTypeField.getValue() !== null) {
                idUserType = userTypeField.getValue()[0].id.slice(1, -1)
            }
        }

        /*FIXME: cuando se crea un paciente bajo cuido desde cero en la pantalla de Create, el user Type va vacío y no entra en esta condicion,
        hay que asignarle el userType al paciente bajo cuido creado, o que esta condicion reciba el isChildCOntact
        */
        if (idUserType !== "" || isChildContact) {

            //Control de cambio de usuario solo disponible para pacientes de tipo Otro Interes
            if (idUserType.toLowerCase() === Abox.SharedLogic.Constants.OtherInterestIdType) {


                Abox.SharedLogic.setFieldsVisible(formContext, [contactFields.IsUserTypeChange]);

                var isUserTypeChangeField = formContext.getAttribute(contactFields.IsUserTypeChange);

                if (isUserTypeChangeField !== null) {
                    var fieldNames = [contactFields.Firstname, contactFields.Lastname, contactFields.SecondLastname, contactFields.IdType, contactFields.Id, contactFields.NoEmail, contactFields.Email, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Gender, contactFields.Birthdate, contactFields.CityLookup, contactFields.CantonLookup, contactFields.DistrictLookup, contactFields.Interests, contactFields.Password, contactFields.OtherInterestLookup];

                    isUserTypeChangeField.addOnChange(function () {

                        var value = isUserTypeChangeField.getValue();

                        if (value) {
                            Abox.SharedLogic.disableFields(formContext, fieldNames);
                            Abox.SharedLogic.setFieldsNonRequired(formContext, fieldNames);

                        } else {
                            Abox.SharedLogic.enableFields(formContext, fieldNames);
                            Abox.SharedLogic.setFieldsRequired(formContext, fieldNames);
                        }


                    })
                }

            } else {

                Abox.SharedLogic.disableFields(formContext, [contactFields.UserType]);
                Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.IsUserTypeChange]);

            }


            if (isChildContact) {
                               

                //Control campos que no son visibles por los pacientes bajo cuido
                Abox.SharedLogic.setFieldsInvisible(formContext,
                    [contactFields.Email,
                    contactFields.NoEmail,
                    contactFields.Interests,
                    contactFields.Password,
                    contactFields.Phone,
                    contactFields.SecondaryPhone,
                    contactFields.CityLookup,
                    contactFields.CantonLookup,
                    contactFields.DistrictLookup,
                    contactFields.IsChildContact,
                    contactFields.ChangePasswordWebResource]);

                Abox.SharedLogic.setFieldsNonRequired(formContext,
                    [contactFields.Email,
                    contactFields.NoEmail,
                    contactFields.Interests,
                    contactFields.Password,
                    contactFields.Phone,
                    contactFields.SecondaryPhone,
                    contactFields.CityLookup,
                    contactFields.CantonLookup,
                    contactFields.DistrictLookup,
                    contactFields.IsChildContact]);

            } else {

                Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.ContactxContactLookup]);
                var changePasswordWebResourceControl = formContext.getControl(contactFields.ChangePasswordWebResource);
                var idField = formContext.getAttribute(contactFields.Id);
                var personalIdContact = null;
                if (idField != null) {
                    if (idField.getValue() !== null) {
                        personalIdContact = idField.getValue();
                    }

                    if (personalIdContact !== null) {
                        if (changePasswordWebResourceControl) {
                            changePasswordWebResourceControl.getContentWindow().then(
                                function (contentWindow) {
                                    try {
                                        contentWindow.initializeChangePasswordWebResource(formContext, Abox.SharedLogic, Xrm, personalIdContact);
                                        contentWindow.setComponentSuccess();
                                    } catch (error) {
                                        contentWindow.setComponentFailure();
                                    }

                                }, function () {
                                    Xrm.Navigation.openErrorDialog({ details: "Error cargando componente de Cambio de contraseña", message: "Ocurrió un error cargando uno de los componentes de este formulario, por favor intente nuevamente." });
                                }
                            ).catch(function (error) {
                                console.error(error);
                                Xrm.Navigation.openErrorDialog({ details: error.stack, message: "Ocurrió un error cargando uno de los componentes de este formulario, por favor intente nuevamente." });
                            })
                        }
                    } else {
                        changePasswordWebResourceControl.setVisible(false);
                    }

                }

            }

        }


        if (isChildContact || (idUserType.toLowerCase() === Abox.SharedLogic.Constants.PatientIdType || idUserType.toLowerCase() === Abox.SharedLogic.Constants.OtherInterestIdType)) {
            Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.SubGridControls.RelatedContacts]);

            var onLoadFunction = function () {

                Xrm.Utility.showProgressIndicator("Obteniendo facturas del paciente,esto puede demorar unos minutos, por favor espere...");
                //si no viene el Id de paciente abox no hacer este llamado
                that.loadPatientInvoicesOnDemand(formContext, Xrm).then(function (success) {
                    Xrm.Utility.closeProgressIndicator();


                    if (success !== null) {
                        if (success) {
                            //remover evento on Load
                            relatedInvoicesControl.removeOnLoad(onLoadFunctionReference)
                            relatedInvoicesControl.refresh();

                        }
                    }

                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ details: error.stack, message: "No ha sido posible obtener las facturas de este paciente en este momento." });
                    Xrm.Utility.closeProgressIndicator();
                    console.log(error);
                });
            }

            //Se guarda referencia en esta variable para poder remover el eventoOnLoad del grid con esta referencia
            var onLoadFunctionReference = onLoadFunction;

            //Capturar evento on load del subgrid de facturas
            var relatedInvoicesControl = formContext.getControl(contactFields.SubGridControls.InvoicesGrid);
            if (relatedInvoicesControl !== null) {


                //relatedInvoicesControl.addOnLoad(onLoadFunction);

            }

        } else {


            //Tutores, cuidadores no registran facturas, se les registra a los pacientes bajo cuido, tampoco doctores ni productos, estos estan relacionados al paciente que tienen bajo cuido
            Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.SubGridControls.InvoicesGrid, contactFields.SubGridControls.RelatedProductsDosesGrid, contactFields.SubGridControls.RelatedDoctorsGrid]);

        }

    },

    setFieldControlsBasedOnCountry: function (formContext) {

        var dataFormat = this.getCountryFormat(formContext);
        if (dataFormat !== null) {
            if (dataFormat !== null) {
                //si el pais no utiliza distritos como parte de su division territorial.
                if (dataFormat.Address3 === "0") {
                    Abox.SharedLogic.setFieldsNonRequired(formContext, [Abox.SharedLogic.Entities.ContactFields.DistrictLookup])
                }
            }

            if (dataFormat.pais === "GT") {
                Abox.SharedLogic.setFieldsNonRequired(formContext, [Abox.SharedLogic.Entities.ContactFields.SecondLastname])
            } else {
                Abox.SharedLogic.setFieldsRequired(formContext, [Abox.SharedLogic.Entities.ContactFields.SecondLastname])
            }
        }


    },

    disableFieldsUntilCountrySelected: function (formContext) {

        var contactFields = Abox.SharedLogic.Entities.ContactFields;
        var fieldNames = [contactFields.UserType, contactFields.Firstname, contactFields.Lastname, contactFields.SecondLastname, contactFields.IdType, contactFields.Id, contactFields.NoEmail, contactFields.Email, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Gender, contactFields.Birthdate, contactFields.CityLookup, contactFields.CantonLookup, contactFields.DistrictLookup, contactFields.Interests, contactFields.Password];

        var that = this;

        //se define funcion que se llamara para verificar si hay pais definido o no
        function initialCountryCheck() {
            if (countryLookupField.getValue() !== null) {

                countryControl.clearNotification();
                Abox.SharedLogic.enableFields(formContext, fieldNames);
                var fieldsToClear = [Abox.SharedLogic.Entities.ContactFields.Id, Abox.SharedLogic.Entities.ContactFields.Phone, Abox.SharedLogic.Entities.ContactFields.SecondaryPhone];
                Abox.SharedLogic.clearFields(formContext, fieldsToClear);
                Abox.SharedLogic.clearLookupFields(formContext, [Abox.SharedLogic.Entities.ContactFields.CityLookup, Abox.SharedLogic.Entities.ContactFields.Canton, Abox.SharedLogic.Entities.ContactFields.DistrictLookup]);
                //

                that.setFieldControlsBasedOnCountry(formContext);


                //

            } else {
                countryControl.setNotification("Seleccione un país");
                Abox.SharedLogic.disableFields(formContext, fieldNames);
            }
        }

        var countryLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.CountryLookup);
        var countryControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.CountryLookup);

        if (countryLookupField != null) {

            var countryValue = countryLookupField.getValue();

            if (countryValue === null) {
                initialCountryCheck();
                countryLookupField.addOnChange(initialCountryCheck);
            } else {
                return;
            }
        }

        var idTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IdType);
        if (idTypeControl !== null) {
            idTypeControl.setDisabled(true);

        }

    },


    getCountryFormat: function (formContext) {
        var countryCode = null;
        var dataFormat = null;

        try {
            var countryLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.CountryLookup);

            if (countryLookupField != null) {

                var countryValue = countryLookupField.getValue();
                var guid = null;
                var name = null;
                var entityName = null;

                if (countryValue !== null) {
                    //To get the attributes of the field
                    guid = countryValue[0].id.slice(1, -1);
                    name = countryValue[0].name;
                    entityName = countryValue[0].entityType;
                }
                console.log("Valor de country:" + guid);
                if (guid !== null) {

                    countryCode = Abox.SharedLogic.GetCountryCodeFromGuid(guid);
                }
            }
            if (countryCode !== null) {

                dataFormat = Abox.SharedLogic.DataFormats.getDataFormatsByCountry(countryCode);
                console.log("Formato a usar:" + dataFormat);
            }
        } catch (error) {
            console.error(error);

        }



        return dataFormat;

    },

    setFieldsControlsAndAlerts: function (formContext) {

        var that = this;
        var idField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Id);
        var idControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Id);


        that.setFieldControlsBasedOnCountry(formContext);

        var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);

        if (idTypeField !== null) {

            idTypeField.addOnChange(function () {

                Abox.SharedLogic.clearFields(formContext, [Abox.SharedLogic.Entities.ContactFields.Id]);

            });
        }


        if (idField !== null) {
            idField.addOnChange(function () {

                var dataFormat = that.getCountryFormat(formContext);
                var isForeignId = false;

                //identificar si es extranjero
                var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);

                if (idTypeField != null) {
                    var idTypeValue = idTypeField.getValue();

                    if (idTypeValue !== null) {

                        if (idTypeValue.toString() === Abox.SharedLogic.Constants.ForeignerIdValue.toString()) {
                            isForeignId = true;
                        }

                    }
                }


                var value = idField.getValue();

                if (value !== null) {

                    if (isForeignId) {
                        Abox.SharedLogic.setFieldNotification(value, null, idControl, 30, null);
                    } else {
                        Abox.SharedLogic.setFieldNotification(value, null, idControl, dataFormat.MaxID, dataFormat.MinID);

                    }
                }

            });
        }





        var phoneField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Phone);
        var phoneControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Phone);

        if (phoneField !== null) {
            phoneField.addOnChange(function () {

                var dataFormat = that.getCountryFormat(formContext);

                var value = phoneField.getValue();

                if (value !== null) {

                    Abox.SharedLogic.setFieldNotification(value, null, phoneControl, dataFormat.MaxMinPhone, dataFormat.MaxMinPhone);

                }

            });
        }

        var secondaryPhoneField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.SecondaryPhone);
        var secondaryPhoneControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.SecondaryPhone);
        if (secondaryPhoneField != null) {

            secondaryPhoneField.addOnChange(function () {

                var dataFormat = that.getCountryFormat(formContext);

                var value = secondaryPhoneField.getValue();

                if (value !== null) {

                    Abox.SharedLogic.setFieldNotification(value, null, secondaryPhoneControl, dataFormat.MaxMinPhone, dataFormat.MaxMinPhone);

                }

            });

        }

    },
    setUnderCareLogic: function (formContext) {
        var contactFields = Abox.SharedLogic.Entities.ContactFields;
        var that = this;
        var isChildContactField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IsChildContact);
        var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);
        var idTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IdType);
        var idControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Id);
        if (idTypeField !== null) {
            idTypeField.addOnChange(function () {

                var isChild = isChildContactField.getValue();
                if (idTypeField.getValue() !== null) {

                    if (idTypeField.getValue() === 3 && !isChild) {

                        idTypeControl.setNotification("Este tipo de identificación solo está disponible para pacientes bajo cuido");

                    } else {
                        if (idTypeField.getValue() === 3) {
                            if (idControl !== null) {
                                idControl.clearNotification();
                            }
                        } else {
                            idTypeControl.clearNotification();
                        }
                    }
                }

            });
        }


        isChildContactField.addOnChange(function () {

            // Set the value of the field to TRUE
            //dontAllowEmailsField.setValue(true);

            // Set the value of the field to FALSE
            var isChild = isChildContactField.getValue();


            console.log("childContactChanged val:" + isChild);

            if (isChild) {


                //
                Abox.SharedLogic.setFieldsNonRequired(formContext, [contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email, contactFields.NoEmail]);

                Abox.SharedLogic.clearFields(formContext, [contactFields.Phone, contactFields.SecondaryPhone, contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email]);

                Abox.SharedLogic.setYesNoField(formContext, [contactFields.NoEmail], false);

                Abox.SharedLogic.setFieldsInvisible(formContext, [contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email, contactFields.NoEmail]);

                Abox.SharedLogic.clearFieldsNotification(formContext, [contactFields.Phone, contactFields.SecondaryPhone]);


                if (idTypeField.getValue() !== null) {

                    if (idTypeField.getValue() === 3) {

                        idTypeControl.clearNotification();

                    }
                }

                //


            } else {


                //

                Abox.SharedLogic.setFieldsRequired(formContext, [contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email, contactFields.NoEmail]);

                Abox.SharedLogic.clearFields(formContext, [contactFields.Phone, contactFields.SecondaryPhone, contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email]);

                Abox.SharedLogic.setYesNoField(formContext, [contactFields.NoEmail], false);

                Abox.SharedLogic.setFieldsVisible(formContext, [contactFields.Canton, contactFields.DistrictLookup, contactFields.CityLookup, contactFields.Phone, contactFields.SecondaryPhone, contactFields.Password, contactFields.UserType, contactFields.Interests, contactFields.Email, contactFields.NoEmail]);

                Abox.SharedLogic.clearFieldsNotification(formContext, [contactFields.Phone, contactFields.SecondaryPhone]);

                //

                if (idTypeField.getValue() !== null) {

                    if (idTypeField.getValue() === 3) {

                        idTypeControl.setNotification("Este tipo de identificación solo está disponible para pacientes bajo cuido");

                    }
                }

            }
        });

    }


};

