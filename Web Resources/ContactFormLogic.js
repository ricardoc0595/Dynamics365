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

            var fieldNames = [Abox.SharedLogic.Entities.ContactFields.IsUserTypeChange,Abox.SharedLogic.Entities.ContactFields.ChangePasswordWebResource];
            this.setFieldsInvisible(formContext, fieldNames);

            //Para poder implementar un mensaje mas amigable, hay que hacer un propio boton de guardar customizado y llamar al metodo save() del api de dynamics

            //this.setOnSaveAction(formContext,executionContext);

        } else if (formType === 2) {


            this.validateUpdateAvailability(formContext);

            var isChildContactField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IsChildContact);


            var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);
            if (idTypeField !== null) {
                var isChild = isChildContactField.getValue();
            }

            if (isChild) {
                this.setUnderCareRequiredFields("none", formContext);
            }

            this.setFieldsControlsAndAlerts(formContext);
            this.setUpdateFormLogic(formContext);
        }

    },

    setOnSaveAction: function (formContext, executionContext) {

        formContext.data.entity.addOnSave(function (ex) {
            console.log(ex);
        });

    },


    validateUpdateAvailability: function (formContext) {


        var idPatientField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdAboxPatient);

        if (idPatientField.getValue() === null || idPatientField.getValue() === "null") {

            formContext.ui.setFormNotification("Este contacto no posee un ID de paciente Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        }



    },

    setInitialData: function (formContext) {



    },

    setCreateFormLogic: function (formContext) {




    }
    ,


    setUpdateFormLogic: function (formContext) {

        var that = this;
        var passwordField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Password);
        var passwordControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Password);
        if (passwordField != null) {

            // passwordField.setValue("");
            passwordField.setRequiredLevel("none");
            // if (passwordControl) {
            //     passwordControl.setVisible(true);

            // }
        }


        var isChildContact = false;
        var isChildContactControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IsChildContact);
        var isChildContactField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IsChildContact);
        if (isChildContactControl !== null) {

            isChildContactControl.setVisible(false);
        }

        if (isChildContactField !== null) {
            if (isChildContactField.getValue() !== null) {
                isChildContact = isChildContactField.getValue();
            }
        }




        var clientInterestField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Interests);


        if (clientInterestField != null) {

            clientInterestField.setRequiredLevel("none");

        }



        var countryControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.CountryLookup);

        if (countryControl != null) {

            countryControl.setDisabled(true);
        }

        var userTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.UserType);
        var userTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.UserType);
        var idUserType = "";
        if (userTypeControl != null) {
            if (userTypeField !== null) {

                if (userTypeField.getValue() !== null) {
                    idUserType = userTypeField.getValue()[0].id.slice(1, -1)
                }
            }

            //Desactiva el control de tipo de usuario para todos los tipos de usuario menos de otro interes, estos pueden cambiar su tipo de perfil
            if (idUserType !== "" || isChildContact) {
                if ((idUserType.toLowerCase() !== Abox.SharedLogic.Constants.OtherInterestIdType) || isChildContact) {
                    userTypeControl.setDisabled(true);

                    var fields = [Abox.SharedLogic.Entities.ContactFields.IsUserTypeChange];
                    this.setFieldsInvisible(formContext, fields);

                } else {
                    var isUserTypeChangeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IsUserTypeChange);
                    if (isUserTypeChangeControl !== null) {

                        isUserTypeChangeControl.setVisible(true);
                    }
                    var isUserTypeChangeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IsUserTypeChange);

                    if (isUserTypeChangeField !== null) {
                        var fieldNames = [Abox.SharedLogic.Entities.ContactFields.Firstname, Abox.SharedLogic.Entities.ContactFields.Lastname, Abox.SharedLogic.Entities.ContactFields.SecondLastname, Abox.SharedLogic.Entities.ContactFields.IdType, Abox.SharedLogic.Entities.ContactFields.Id, Abox.SharedLogic.Entities.ContactFields.NoEmail, Abox.SharedLogic.Entities.ContactFields.Email, Abox.SharedLogic.Entities.ContactFields.Phone, Abox.SharedLogic.Entities.ContactFields.SecondaryPhone, Abox.SharedLogic.Entities.ContactFields.Gender, Abox.SharedLogic.Entities.ContactFields.Birthdate, Abox.SharedLogic.Entities.ContactFields.CityLookup, Abox.SharedLogic.Entities.ContactFields.CantonLookup, Abox.SharedLogic.Entities.ContactFields.DistrictLookup, Abox.SharedLogic.Entities.ContactFields.Interests, Abox.SharedLogic.Entities.ContactFields.Password, Abox.SharedLogic.Entities.ContactFields.OtherInterestLookup];

                        isUserTypeChangeField.addOnChange(function () {

                            var value = isUserTypeChangeField.getValue();

                            if (value) {
                                that.disableFields(formContext, fieldNames);

                            } else {
                                that.enableFields(formContext, fieldNames);
                            }


                        })
                    }

                }

            }




        }


        var relatedContactsControl = formContext.getControl(Abox.SharedLogic.Constants.SubGridControls.RelatedContacts);
        if (relatedContactsControl !== null) {


            if (isChildContact || (idUserType.toLowerCase() === Abox.SharedLogic.Constants.PatientIdType || idUserType.toLowerCase() === Abox.SharedLogic.Constants.OtherInterestIdType)) {
                relatedContactsControl.setVisible(false);
            }

        }

        var idTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IdType);
        if (idTypeControl != null) {
            idTypeControl.setDisabled(true);
        }

        var idControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Id);
        if (idControl != null) {
            idControl.setDisabled(true);
        }


        // var idAboxPatientField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdAboxPatient);

        // if (idAboxPatientField.getValue() === null || idAboxPatientField.getValue() === "null") {

        //     formContext.ui.setFormNotification("Este contacto no posee un ID de paciente Abox registrado, es posible que haya ocurrido un error y no se haya podido registrar correctamente.", "WARNING", null);
        // }

        var changePasswordWebResourceControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.ChangePasswordWebResource);


        var idField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Id);
        var personalIdContact = null;
        if (idField != null) {
            if (idField.getValue() !== null) {
                personalIdContact = idField.getValue();
            }

            if(personalIdContact!==null){
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
            }else{
                changePasswordWebResourceControl.setVisible(false);
            }
            

        }

       




    },

    disableFieldsUntilCountrySelected: function (formContext) {

        var fieldNames = [Abox.SharedLogic.Entities.ContactFields.UserType, Abox.SharedLogic.Entities.ContactFields.Firstname, Abox.SharedLogic.Entities.ContactFields.Lastname, Abox.SharedLogic.Entities.ContactFields.SecondLastname, Abox.SharedLogic.Entities.ContactFields.IdType, Abox.SharedLogic.Entities.ContactFields.Id, Abox.SharedLogic.Entities.ContactFields.NoEmail, Abox.SharedLogic.Entities.ContactFields.Email, Abox.SharedLogic.Entities.ContactFields.Phone, Abox.SharedLogic.Entities.ContactFields.SecondaryPhone, Abox.SharedLogic.Entities.ContactFields.Gender, Abox.SharedLogic.Entities.ContactFields.Birthdate, Abox.SharedLogic.Entities.ContactFields.CityLookup, Abox.SharedLogic.Entities.ContactFields.CantonLookup, Abox.SharedLogic.Entities.ContactFields.DistrictLookup, Abox.SharedLogic.Entities.ContactFields.Interests, Abox.SharedLogic.Entities.ContactFields.Password];

        var that = this;

        //se define funcion que se llamara para verificar si hay pais definido o no
        function initialCountryCheck() {
            if (countryLookupField.getValue() !== null) {

                countryControl.clearNotification();
                that.enableFields(formContext, fieldNames);
                var fieldsToClear = [Abox.SharedLogic.Entities.ContactFields.Id, Abox.SharedLogic.Entities.ContactFields.Phone, Abox.SharedLogic.Entities.ContactFields.SecondaryPhone];
                that.clearTextFields(formContext, fieldsToClear);

            } else {
                countryControl.setNotification("Seleccione un país");
                that.disableFields(formContext, fieldNames);
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

    setUnderCareRequiredFields: function (requiredLevel, formContext) {

        var visibility = requiredLevel === "required" ? true : false;

        //Access the field on the form
        var emailField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Email);
        var emailControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Email);
        //Check that field exist on the form before you try to Get/Set its value
        if (emailField != null) {
            // Get the value of the field
            // emailField = accountNumberField.getValue();

            // Set the value of the field
            emailField.setValue("");
            emailField.setRequiredLevel(requiredLevel);
            if (emailControl !== null) {
                emailControl.setVisible(visibility);
            }
        }

        var noEmailField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.NoEmail);
        var noEmailControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.NoEmail);

        if (noEmailField != null) {
            noEmailField.setValue(false);
            noEmailField.setRequiredLevel(requiredLevel);
            if (noEmailControl !== null) {
                noEmailControl.setVisible(visibility);
            }
        }

        var clientInterestField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Interests);
        var clientInterestControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Interests);

        if (clientInterestField != null) {
            clientInterestField.setValue(null);
            clientInterestField.setRequiredLevel(requiredLevel);
            if (clientInterestControl !== null) {
                clientInterestControl.setVisible(visibility);
            }
        }

        var userTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.UserType);
        var userTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.UserType);

        if (userTypeField != null) {
            userTypeField.setValue(null);
            userTypeField.setRequiredLevel(requiredLevel);
            if (userTypeControl !== null) {
                userTypeControl.setVisible(visibility);
            }
        }

        var passwordField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Password);
        var passwordControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Password);
        if (passwordField != null) {

            passwordField.setValue("");
            passwordField.setRequiredLevel(requiredLevel);
            if (passwordControl) {
                passwordControl.setVisible(visibility);

            }
        }

        var phoneField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Phone);
        var phoneControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Phone);
        if (phoneField != null) {

            phoneField.setValue("");
            phoneField.setRequiredLevel(requiredLevel);
            if (phoneControl !== null) {
                phoneControl.setVisible(visibility);
                phoneControl.clearNotification();
            }

        }

        var secondaryPhoneField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.SecondaryPhone);
        var secondaryPhoneControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.SecondaryPhone);
        if (secondaryPhoneField != null) {

            secondaryPhoneField.setValue("");
            secondaryPhoneField.setRequiredLevel(requiredLevel);
            if (secondaryPhoneControl !== null) {
                secondaryPhoneControl.setVisible(visibility);
                secondaryPhoneControl.clearNotification();
            }
        }



        var provinceLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.CityLookup);
        var provinceControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.CityLookup);
        if (provinceLookupField != null) {
            //var provinceValue = provinceLookupField.getValue();

            provinceLookupField.setValue(null);
            provinceLookupField.setRequiredLevel(requiredLevel);
            if (provinceControl !== null) {
                provinceControl.setVisible(visibility);
            }

            //Check if the field contains a value
            // if (provinceValue != null) {
            //     provinceValue.setValue(null);
            //     provinceValue.setRequiredLevel(requiredLevel);
            //     if (provinceControl !== null) {
            //         provinceControl.setVisible(visibility);

            //     }

            //       //To get the attributes of the field
            //     // guid = countryValue[0].id.slice(1, -1);
            //     // name = countryValue[0].name;
            //     // entityName = countryValue[0].entityType;

            // }

            // Set the value of the field
            // accountmanagerField.setValue([{
            //     id: "4BDB64C8-AA81-E911-B80C-00155D380105",
            //     name: "Joshua Sinkamba",
            //     entityType: "systemuser"
            // }]);

        }

        var cantonLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.CantonLookup);
        var cantonControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.CantonLookup);
        if (cantonLookupField != null) {

            cantonLookupField.setValue(null);
            cantonLookupField.setRequiredLevel(requiredLevel);
            if (cantonControl !== null)
                cantonControl.setVisible(visibility);

        }

        var districtLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.DistrictLookup);
        var districtControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.DistrictLookup);
        if (districtLookupField != null) {
            districtLookupField.setValue(null);
            districtLookupField.setRequiredLevel(requiredLevel);
            if (districtControl) {
                districtControl.setVisible(visibility);
            }


        }

    },


    getCountryFormat: function (formContext) {
        var countryCode = null;
        var countryLookupField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.CountryLookup);

        var dataFormat = null;
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

        return dataFormat;

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

    setFieldsControlsAndAlerts: function (formContext) {

        var that = this;
        var idField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Id);
        var idControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.Id);


        var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);
        var idTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IdType);


        if (idTypeField !== null) {

            idTypeField.addOnChange(function () {

                that.clearTextFields(formContext, [Abox.SharedLogic.Entities.ContactFields.Id]);

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

                        // if (idTypeValue.toString() === Abox.SharedLogic.Constants.MinorIdValue.toString()) {
                        //     isMinor = true;
                        // }

                    }
                }


                var value = idField.getValue();

                if (value !== null) {

                    if (isForeignId) {
                        that.setFieldNotification(value, null, idControl, 30, null);
                    } else {
                        that.setFieldNotification(value, null, idControl, dataFormat.MaxID, dataFormat.MinID);

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

                    that.setFieldNotification(value, null, phoneControl, dataFormat.MaxMinPhone, dataFormat.MaxMinPhone);

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

                    that.setFieldNotification(value, null, secondaryPhoneControl, dataFormat.MaxMinPhone, dataFormat.MaxMinPhone);

                }

            });

        }

    },
    setUnderCareLogic: function (formContext) {

        var that = this;
        var isChildContactField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IsChildContact);


        var idTypeField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.IdType);
        var idTypeControl = formContext.getControl(Abox.SharedLogic.Entities.ContactFields.IdType);
        var idField = formContext.getAttribute(Abox.SharedLogic.Entities.ContactFields.Id);
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

                that.setUnderCareRequiredFields("none", formContext);

                if (idTypeField.getValue() !== null) {

                    if (idTypeField.getValue() === 3) {

                        idTypeControl.clearNotification();

                    }
                }


            } else {

                that.setUnderCareRequiredFields("required", formContext);

                if (idTypeField.getValue() !== null) {

                    if (idTypeField.getValue() === 3) {

                        idTypeControl.setNotification("Este tipo de identificación solo está disponible para pacientes bajo cuido");

                    }
                }

            }
        });

    }


};
