if (typeof (Abox) == "undefined") {

    Abox = { __namespace: true };
}

Abox.SharedLogic = {
    test: function () {
        alert("test Call");
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
        MinorIdValue : 3,
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
        GeneralAboxServicesErrorMessage: "Ocurrió un error consultando los servicios de Abox Plan \n",
        ErrorMessageCodeReturned: "Error en transacción, Código de respuesta servicio:",
        ErrorMessageTransactionCodeReturned: "Ocurrió un error al guardar la información en Abox Plan:\n",
        GeneralPluginErrorMessage: "Ocurrió un error en la ejecución de un Plugin interno, por favor intenta nuevamente o comunícate con soporte.",
        ApplicationIdWebAPI: "WEBAPI",
        ApplicationIdPlugin: "PLUGIN",
        SubGridControls:{
            RelatedContacts:"RelatedContacts"
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


};

