﻿// валидация
// только запятая как десятичный разделитель
//$.validator.methods.number = function (value, element) {
//    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)?(?:,\d+)?$/.test(value); 
//}

// и точка и запятая
$.validator.methods.range = function(value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
};
$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
};


/* bootstrap 4 compatibility classes */
(function ($) {
    var defaultOptions = {
        validClass: 'is-valid',
        errorClass: 'is-invalid',
        highlight: function (element, errorClass, validClass) {
            $(element)
                .removeClass(validClass)
                .addClass(errorClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element)
                .removeClass(errorClass)
                .addClass(validClass);
        }
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);