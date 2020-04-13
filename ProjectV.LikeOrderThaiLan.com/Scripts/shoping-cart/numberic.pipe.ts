import { Pipe, PipeTransform } from '@angular/core';

declare var Globalize;

@Pipe({ name: 'numberic' })

export class NumbericPipe implements PipeTransform {
    transform(value: any, ext?: string): any {
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
    }
}