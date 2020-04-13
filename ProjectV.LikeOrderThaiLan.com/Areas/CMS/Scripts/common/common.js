Globalize.culture("en-US");
moment.locale("en-US");
//hamf cat chuoi
function substring(str, number) {
    if(str === null || str === undefined || str.length < number || number <= 0){
        return str;
    }
    var rt = str.substring(0, number); 
    if (str.length > number) {
        return   rt + "...";
    } else {

    } 
}
function substring(str, number, strReplace) {
    if (str === null || str === undefined || str.length < number || number <= 0) {
        return str;
    }
    str = str.replace(strReplace, "")
    var rt = str.substring(0, number);
    if (str.length > number) {
        return rt + "...";
    } else {

    }
}

// Định dạng số
function formatNumberic(value, ext) {
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

//format theo chuẩn mỹ
function formatNumbericCN(value, ext) {
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

    var radixPoint = Globalize("en-US").culture().numberFormat['.'];

    value = Globalize("en-US").format(value, ext).toString();
    if (value.split(radixPoint)[1] === "0000" || value.split(radixPoint)[1] === "000" || value.split(radixPoint)[1] === "00" || value.split(radixPoint)[1] === "0") {
        value = value.split(radixPoint)[0];
    }
    return value;
}

function formatNumbericCul(value, ext, cul) {
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
    return value + ' ' + cul;
}

//fomatVietNam
function formatVN(value) {
    if (value) {
        value = value.replace(/,/g, "");
        value = value.replace(".", ",");
        return value;
    }
    return "";
}

var msgType = {
    success: 1,
    info: 2,
    warning: 3,
    error: -1
}

function ShowLoading() {
    var element = $('body');
    if (element.length > 0) {
        element.append("<div class='loading-icon' style='position: fix'><i class=\"fa fa-spinner fa-pulse fa-3x fa-fw red-color\"></i></div>");
        element.css("position", "relative");
    }
}

function HideLoading() {
    var element = $('body');
    if (element.length > 0) {
        element.find(".loading-icon").remove();
    }
}
