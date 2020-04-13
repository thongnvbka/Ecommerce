"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var NumbericPipe = (function () {
    function NumbericPipe() {
    }
    NumbericPipe.prototype.transform = function (value, ext) {
        if (isNaN(value)) {
            return "";
        }
        if (parseFloat(value) === 0) {
            return "0";
        }
        if (ext == undefined)
            ext = "N0";
        if (value == null || value === "") {
            return "";
        }
        var radixPoint = Globalize.culture().numberFormat['.'];
        value = Globalize.format(value, ext).toString();
        if (value.split(radixPoint)[1] === "0000" || value.split(radixPoint)[1] === "000" || value.split(radixPoint)[1] === "00" || value.split(radixPoint)[1] === "0") {
            value = value.split(radixPoint)[0];
        }
        return value;
    };
    NumbericPipe = __decorate([
        core_1.Pipe({ name: 'numberic' }), 
        __metadata('design:paramtypes', [])
    ], NumbericPipe);
    return NumbericPipe;
}());
exports.NumbericPipe = NumbericPipe;
//# sourceMappingURL=numberic.pipe.js.map