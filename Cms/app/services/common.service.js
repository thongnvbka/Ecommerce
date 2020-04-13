System.register([], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var CommonService;
    return {
        setters:[],
        execute: function() {
            CommonService = (function () {
                function CommonService() {
                }
                CommonService.stripVietnameseChars = function (input) {
                    var stringBuilder = [];
                    for (var k = 0; k < input.length; k++) {
                        stringBuilder.push(input.charAt(k));
                    }
                    for (var i = 0; i < stringBuilder.length; i++) {
                        for (var j = 0; j < this.strips.length; j++) {
                            if (this.strips[j].indexOf(stringBuilder[i]) > -1) {
                                stringBuilder[i] = this.replacements[j];
                            }
                        }
                    }
                    return stringBuilder.join("");
                };
                ;
                CommonService.romanize = function (num) {
                    if (!+num)
                        return '';
                    var digits = String(+num).split("");
                    var key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
                        "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
                        "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"];
                    var roman = "";
                    var i = 3;
                    while (i--)
                        roman = (key[+digits.pop() + (i * 10)] || "") + roman;
                    return Array(+digits.join("") + 1).join("M") + roman;
                };
                CommonService.strips = [
                    "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "đ", "Đ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ", "íìỉĩị", "ÍÌỈĨỊ",
                    "óòỏõọơớờởỡợôốồổỗộ",
                    "ÓÒỎÕỌƠỚỜỞỠỢÔỐỒỔỖỘ",
                    "ưứừửữựúùủũụ", "ƯỨỪỬỮỰÚÙỦŨỤ", "ýỳỷỹỵ", "ÝỲỶỸỴ"];
                CommonService.replacements = ['a', 'A', 'd', 'D', 'e', 'E', 'i', 'I', 'o', 'O', 'u', 'U', 'y', 'Y'];
                return CommonService;
            }());
            exports_1("CommonService", CommonService);
        }
    }
});
//# sourceMappingURL=common.service.js.map