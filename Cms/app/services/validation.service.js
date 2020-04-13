System.register([], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var ValidationService;
    return {
        setters:[],
        execute: function() {
            ValidationService = (function () {
                function ValidationService() {
                }
                ValidationService.getValidatorErrorMessage = function (validatorName, label, validatorValue) {
                    var config = {
                        'required': label + " l\u00E0 b\u1EAFt bu\u1ED9c",
                        'invalidCreditCard': 'Is invalid credit card number',
                        'invalidEmailAddress': 'Invalid email address',
                        'invalidPassword': 'Mật khẩu phải lơn hơn 5 ký tự, và chứa 1 số.',
                        'minlength': label + " ph\u1EA3i l\u01A1n h\u01A1n " + validatorValue.requiredLength + " k\u00FD t\u1EF1",
                        'maxlength': label + " ph\u1EA3i nh\u1ECF h\u01A1n " + validatorValue.requiredLength + " k\u00FD t\u1EF1",
                        'minValue': "Gi\u00E1 tr\u1ECB " + label + " kh\u00F4ng \u0111\u01B0\u1EE3c nh\u1ECF h\u01A1n " + validatorValue.requiredMinValue,
                        'maxValue': "Gi\u00E1 tr\u1ECB " + label + " kh\u00F4ng \u0111\u01B0\u1EE3c l\u1EDBn h\u01A1n h\u01A1n " + validatorValue.requiredMinValue
                    };
                    return config[validatorName];
                };
                ValidationService.minValue = function (minValue) {
                    return function (control) {
                        var num = control.value;
                        if (isNaN(num) || num < minValue) {
                            return {
                                minValue: {
                                    actualValue: num,
                                    requiredMinValue: minValue
                                }
                            };
                        }
                        return null;
                    };
                };
                ValidationService.maxValue = function (maxValue) {
                    return function (control) {
                        var num = control.value;
                        if (isNaN(num) || num > maxValue) {
                            return {
                                minValue: {
                                    actualValue: num,
                                    requiredMaxValue: maxValue
                                }
                            };
                        }
                        return null;
                    };
                };
                ValidationService.creditCardValidator = function (control) {
                    // Visa, MasterCard, American Express, Diners Club, Discover, JCB
                    if (control.value.match(/^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/)) {
                        return null;
                    }
                    else {
                        return { 'invalidCreditCard': true };
                    }
                };
                ValidationService.emailValidator = function (control) {
                    // RFC 2822 compliant regex
                    if (control.value.match(/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/)) {
                        return null;
                    }
                    else {
                        return { 'invalidEmailAddress': true };
                    }
                };
                ValidationService.passwordValidator = function (control) {
                    // {6,100}           - Assert password is between 6 and 100 characters
                    // (?=.*[0-9])       - Assert a string has at least one number
                    if (control.value.match(/^(?=.*[0-9])[a-zA-Z0-9!@#$%^&*]{6,100}$/)) {
                        return null;
                    }
                    else {
                        return { 'invalidPassword': true };
                    }
                };
                return ValidationService;
            }());
            exports_1("ValidationService", ValidationService);
        }
    }
});
//# sourceMappingURL=validation.service.js.map