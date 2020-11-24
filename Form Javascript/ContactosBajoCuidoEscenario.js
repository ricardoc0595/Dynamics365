

if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.ContactFunctions = {

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
        OtherInterest: "new_otherinterest",
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
        NoEmail: "new_noemail"

    },

    setUnderCareRequiredFields: function (requiredLevel, formContext) {

        var visibility = requiredLevel === "required" ? true : false;

        //Access the field on the form
        var emailField = formContext.getAttribute(this.ContactFields.Email);
        var emailControl = formContext.getControl(this.ContactFields.Email);
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

        var noEmailField = formContext.getAttribute(this.ContactFields.NoEmail);
        var noEmailControl = formContext.getControl(this.ContactFields.NoEmail);

        if (noEmailField != null) {
            noEmailField.setValue(false);
            noEmailField.setRequiredLevel(requiredLevel);
            if (noEmailControl !== null) {
                noEmailControl.setVisible(visibility);
            }
        }

        var clientInterestField = formContext.getAttribute(this.ContactFields.Interests);
        var clientInterestControl = formContext.getControl(this.ContactFields.Interests);

        if (clientInterestField != null) {
            clientInterestField.setValue(null);
            clientInterestField.setRequiredLevel(requiredLevel);
            if (clientInterestControl !== null) {
                clientInterestControl.setVisible(visibility);
            }
        }

        var userTypeField = formContext.getAttribute(this.ContactFields.UserType);
        var userTypeControl = formContext.getControl(this.ContactFields.UserType);

        if (userTypeField != null) {
            userTypeField.setValue(null);
            userTypeField.setRequiredLevel(requiredLevel);
            if (userTypeControl !== null) {
                userTypeControl.setVisible(visibility);
            }
        }

        var passwordField = formContext.getAttribute(this.ContactFields.Password);
        var passwordControl = formContext.getControl(this.ContactFields.Password);
        if (passwordField != null) {

            passwordField.setValue("");
            passwordField.setRequiredLevel(requiredLevel);
            if (passwordControl) {
                passwordControl.setVisible(visibility);

            }
        }

        var phoneField = formContext.getAttribute(this.ContactFields.Phone);
        var phoneControl = formContext.getControl(this.ContactFields.Phone);
        if (phoneField != null) {

            phoneField.setValue("");
            phoneField.setRequiredLevel(requiredLevel);
            if (phoneControl !== null) {
                phoneControl.setVisible(visibility);
                phoneControl.clearNotification();
            }

        }

        var secondaryPhoneField = formContext.getAttribute(this.ContactFields.SecondaryPhone);
        var secondaryPhoneControl = formContext.getControl(this.ContactFields.SecondaryPhone);
        if (secondaryPhoneField != null) {

            secondaryPhoneField.setValue("");
            secondaryPhoneField.setRequiredLevel(requiredLevel);
            if (secondaryPhoneControl !== null) {
                secondaryPhoneControl.setVisible(visibility);
                secondaryPhoneControl.clearNotification();
            }
        }



        var provinceLookupField = formContext.getAttribute(this.ContactFields.CityLookup);
        var provinceControl = formContext.getControl(this.ContactFields.CityLookup);
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

        var cantonLookupField = formContext.getAttribute(this.ContactFields.CantonLookup);
        var cantonControl = formContext.getControl(this.ContactFields.CantonLookup);
        if (cantonLookupField != null) {

            cantonLookupField.setValue(null);
            cantonLookupField.setRequiredLevel(requiredLevel);
            if (cantonControl !== null)
                cantonControl.setVisible(visibility);

        }

        var districtLookupField = formContext.getAttribute(this.ContactFields.DistrictLookup);
        var districtControl = formContext.getControl(this.ContactFields.DistrictLookup);
        if (districtLookupField != null) {
            districtLookupField.setValue(null);
            districtLookupField.setRequiredLevel(requiredLevel);
            if (districtControl) {
                districtControl.setVisible(visibility);
            }


        }

        // var countryLookupField = formContext.getAttribute(this.ContactFields.CountryLookup);
        // var countryControl = formContext.getControl(this.ContactFields.CountryLookup);
        // if (countryLookupField != null) {

        //     countryLookupField.setValue(null);
        //     countryLookupField.setRequiredLevel(requiredLevel);
        //     if (countryControl !== null) {
        //         countryControl.setVisible(visibility);
        //     }

        // }


        // if (requiredLevel==="required") {


        //     this.setFieldsConfiguration(formContext);

        // }


    },

    setFieldsConfiguration: function (formContext, domInputsVisibleList) {

        

    },
   

    getCountryFormat: function (formContext) {
        var countryCode = null;
        var countryLookupField = formContext.getAttribute(this.ContactFields.CountryLookup);

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

    setFieldsAlerts: function (formContext) {

        var that = this;
        var idField = formContext.getAttribute(this.ContactFields.Id);
        var idControl = formContext.getControl(this.ContactFields.Id);

        if (idField !== null) {
            idField.addOnChange(function () {

                var dataFormat = that.getCountryFormat(formContext);
                var isForeignId = false;
                //identificar si es extranjero
                var idTypeField = formContext.getAttribute(that.ContactFields.IdType);

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
                        that.setFieldNotification(value, null, idControl, 30, null);
                    } else {
                        that.setFieldNotification(value, null, idControl, dataFormat.MaxID, dataFormat.MinID);
                    }
                }

            });
        }


        var phoneField = formContext.getAttribute(this.ContactFields.Phone);
        var phoneControl = formContext.getControl(this.ContactFields.Phone);

        if (phoneField !== null) {
            phoneField.addOnChange(function () {

                var dataFormat = that.getCountryFormat(formContext);

                var value = phoneField.getValue();

                if (value !== null) {

                    that.setFieldNotification(value, null, phoneControl, dataFormat.MaxMinPhone, dataFormat.MaxMinPhone);

                }

            });
        }

        var secondaryPhoneField = formContext.getAttribute(this.ContactFields.SecondaryPhone);
        var secondaryPhoneControl = formContext.getControl(this.ContactFields.SecondaryPhone);
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

    onLoad: function (executionContext) {



        //Get the context of the form
        var formContext = executionContext.getFormContext();

        var isChildContactField = formContext.getAttribute(this.ContactFields.IsChildContact);
        // var initialValue = isChildContactField.getValue();

        //var countryLookupField = formContext.getAttribute(this.ContactFields.CountryLookup);

        // if (initialValue) {
        //     this.setUnderCareRequiredFields("none", formContext);
        // } else {
        //     this.setUnderCareRequiredFields("required", formContext);
        // }

        var that = this;

        // setTimeout(that.setControlsVisibleInDom(formContext), 1000)

        // countryLookupField.addOnChange(function () {

        //     console.log("Cambio de pais... ejecutando metodo");
        //     that.setFieldsConfiguration(formContext,domInputsVisibleList);

        // });



        that.setFieldsAlerts(formContext);


        isChildContactField.addOnChange(function () {

            // Set the value of the field to TRUE
            //dontAllowEmailsField.setValue(true);

            // Set the value of the field to FALSE
            var isChild = isChildContactField.getValue();

            console.log("childContactChanged val:" + isChild);

            if (isChild) {

                that.setUnderCareRequiredFields("none", formContext);

            } else {

                that.setUnderCareRequiredFields("required", formContext);

            }
        });



    }
};

