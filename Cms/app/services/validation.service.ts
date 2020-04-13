import { AbstractControl, ValidatorFn } from '@angular/forms'

export class ValidationService {
    static getValidatorErrorMessage(validatorName: string, label: string, validatorValue?: any) {
        let config = {
            'required': `${label} là bắt buộc`,
            'invalidCreditCard': 'Is invalid credit card number',
            'invalidEmailAddress': 'Invalid email address',
            'invalidPassword': 'Password phải lơn hơn 5 ký tự, và chứa 1 số.',
            'minlength': `${label} phải lơn hơn ${validatorValue.requiredLength} ký tự`,
            'maxlength': `${label} phải nhỏ hơn ${validatorValue.requiredLength} ký tự`,
            'minValue': `Giá trị ${label} không được nhỏ hơn ${validatorValue.requiredMinValue}`,
            'maxValue': `Giá trị ${label} không được lớn hơn hơn ${validatorValue.requiredMinValue}`
        };

        return config[validatorName];
    }

    static minValue(minValue: number): ValidatorFn {
        return (control: AbstractControl) => {
            let num = control.value;

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
    }

    static maxValue(maxValue: number): ValidatorFn {
        return (control: AbstractControl) => {
            let num = control.value;

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
    }

    static creditCardValidator(control: AbstractControl) {
        // Visa, MasterCard, American Express, Diners Club, Discover, JCB
        if (control.value.match(/^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/)) {
            return null;
        } else {
            return { 'invalidCreditCard': true };
        }
    }

    static emailValidator(control: AbstractControl) {
        // RFC 2822 compliant regex
        if (control.value.match(/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/)) {
            return null;
        } else {
            return { 'invalidEmailAddress': true };
        }
    }

    static passwordValidator(control: AbstractControl) {
        // {6,100}           - Assert password is between 6 and 100 characters
        // (?=.*[0-9])       - Assert a string has at least one number
        if (control.value.match(/^(?=.*[0-9])[a-zA-Z0-9!@#$%^&*]{6,100}$/)) {
            return null;
        } else {
            return { 'invalidPassword': true };
        }
    }
}
