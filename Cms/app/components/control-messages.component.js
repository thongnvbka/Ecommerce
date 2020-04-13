System.register(['@angular/core', '@angular/forms', '../services/validation.service'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, forms_1, validation_service_1;
    var ControlMessages;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (forms_1_1) {
                forms_1 = forms_1_1;
            },
            function (validation_service_1_1) {
                validation_service_1 = validation_service_1_1;
            }],
        execute: function() {
            ControlMessages = (function () {
                function ControlMessages() {
                }
                Object.defineProperty(ControlMessages.prototype, "errorMessage", {
                    get: function () {
                        for (var propertyName in this.control.errors) {
                            if (this.control.errors.hasOwnProperty(propertyName) && this.control.touched) {
                                return validation_service_1.ValidationService.getValidatorErrorMessage(propertyName, this.label, this.control.errors[propertyName]);
                            }
                        }
                        return null;
                    },
                    enumerable: true,
                    configurable: true
                });
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', forms_1.FormControl)
                ], ControlMessages.prototype, "control", void 0);
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', String)
                ], ControlMessages.prototype, "label", void 0);
                ControlMessages = __decorate([
                    core_1.Component({
                        selector: 'control-messages',
                        template: "<span *ngIf=\"errorMessage !== null\" class=\"help-block has-error\">{{errorMessage}}</span>",
                        styles: ['.help-block.has-error{color: red;}']
                    }), 
                    __metadata('design:paramtypes', [])
                ], ControlMessages);
                return ControlMessages;
            }());
            exports_1("ControlMessages", ControlMessages);
        }
    }
});
//# sourceMappingURL=control-messages.component.js.map