export class CommonService {
    static strips = [
    "áàảãạăắằẳẵặâấầẩẫậ", "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ", "đ", "Đ", "éèẻẽẹêếềểễệ", "ÉÈẺẼẸÊẾỀỂỄỆ", "íìỉĩị", "ÍÌỈĨỊ",
    "óòỏõọơớờởỡợôốồổỗộ",
    "ÓÒỎÕỌƠỚỜỞỠỢÔỐỒỔỖỘ",
    "ưứừửữựúùủũụ", "ƯỨỪỬỮỰÚÙỦŨỤ", "ýỳỷỹỵ", "ÝỲỶỸỴ"];

    static replacements = ['a', 'A', 'd', 'D', 'e', 'E', 'i', 'I', 'o', 'O', 'u', 'U', 'y', 'Y'];

    static stripVietnameseChars(input) {
        const stringBuilder = [];
        for (let k = 0; k < input.length; k++) {
            stringBuilder.push(input.charAt(k));
        }
        for (let i = 0; i < stringBuilder.length; i++) {
            for (let j = 0; j < this.strips.length; j++) {
                if (this.strips[j].indexOf(stringBuilder[i]) > -1) {
                    stringBuilder[i] = this.replacements[j];
                }
            }
        }
        return stringBuilder.join("");
    };

    static romanize(num) {
        if (!+num)
            return '';
        const digits = String(+num).split("");
        const key = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
            "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
            "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"];
        let roman = "";
        let i = 3;
        while (i--)
            roman = (key[+digits.pop() + (i * 10)] || "") + roman;
        return Array(+digits.join("") + 1).join("M") + roman;
    }
}