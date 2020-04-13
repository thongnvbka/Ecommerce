import { Component, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { ValidationService } from '../services/validation.service';

@Component({
    selector: 'control-messages',
    template: `<span *ngIf="errorMessage !== null" class="help-block has-error">{{errorMessage}}</span>`,
    styles: ['.help-block.has-error{color: red;}']
})
export class ControlMessages {
    @Input() control: FormControl;
    @Input() label: string;

    constructor() { }

    get errorMessage() {
        for (let propertyName in this.control.errors) {
            if (this.control.errors.hasOwnProperty(propertyName) && this.control.touched) {
                return ValidationService.getValidatorErrorMessage(propertyName, this.label, this.control.errors[propertyName]);
            }
        }

        return null;
    }
}