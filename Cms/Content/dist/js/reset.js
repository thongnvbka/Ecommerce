function searchKeyPress(e) {

    if (typeof e == 'undefined' && window.event) { e = window.event; }

}
function searchKeyPressTxt(e, idTxt) {

    if (typeof e == 'undefined' && window.event) { e = window.event; }
    if (e.keyCode == 13) {
        document.getElementById(idTxt).click();
    }
}
function doClick(buttonName, e) {
    var key;
    if (window.event)
        key = window.event.keyCode;     //IE
    else
        key = e.which;     //firefox
    if (key == 13) {
        var btn = document.getElementById(buttonName);
        if (btn != null) {
            btn.click();
            event.keyCode = 0
        }
    }
}
function checkOnly(stayChecked, classTable) {

    $(classTable).find("td input").each(function () {
        if ($(this).attr("checked") && $(this).attr("name") != stayChecked.name) $(this).removeAttr("checked");
    });
}
function SelectAllCheckboxes(chk, iClass) {
    $('.' + iClass).find("input:checkbox").each(function () {
        if (this != chk && this.disabled == false) {
            this.checked = chk.checked;
        }
    });
}
function parseDate(str) {
    var mdy = str.split('/');
    return new Date(mdy[2], mdy[1], mdy[0]);
}

function isDate(txtDate) {
    var currVal = txtDate;
    if (currVal == '')
        return false;

    //Declare Regex  
    var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;
    var mdy = txtDate.split('/');
    //Checks for mm/dd/yyyy format.
    dtMonth = mdy[1];
    dtDay = mdy[0];
    dtYear = mdy[2];

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    }
    return true;
}

function isUrl(s) {
    var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
    return regexp.test(s);
}
function isEmail(email) {
    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    return reg.test(email);
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) return false;
    return true;
}
function SelectAllCheckboxes(tblId, chk) {
    var count = 0;
    $('#' + tblId).find("tr").each(function (idx) {
        $(this).find('td').find('input:checkbox').each(function (index) {
            if (index == 0) {
                if (this != chk && this.disabled == false) {
                    this.checked = chk.checked;
                    if (chk.checked) {
                        count = 1;
                    }
                }
                if (this.checked == false && idx == 0) {
                    $("#ltDeleteAll").removeClass("display-inline-block").addClass("display-none");
                }
            }
        });
    });
    if (count != 0) {
        $("#ltDeleteAll").removeClass("display-none").addClass("display-inline-block");
    } else {
        $("#ltDeleteAll").removeClass("display-inline-block").addClass("display-none");
    }
}
function EnableDeleteAll(tblId) {
    var isTrue = false;
    $('#' + tblId).find("tr").each(function (idx) {
        $(this).find('td').find('input:checkbox').each(function (index) {
            if (index == 0) {
                if (this.checked && idx != 0) {
                    isTrue = true;
                }
            }
        });
    });
    if (isTrue) {
        $("#ltDeleteAll").removeClass("display-none").addClass("display-inline-block");
    } else {
        $("#ltDeleteAll").removeClass("display-inline-block").addClass("display-none");
    }
}
function RemoveChecked(tblId) {
    $('#' + tblId).find("input:checkbox").each(function () {
        $(this).removeAttr("checked");
    });
}
function ShowErrorSys(error) {
    $(".showMessage").html("");
    $(".showMessage").append(error);
    $("#showMessage").removeClass("display-none").addClass("display-block");
}

function CountWord(input) {

    $(input).parent().parent().find(".label .count").text("(" + $(input).val().length + ")");
}


function IsMobilePhone(txtPhone, isEmpty) {
    txtPhone = txtPhone.trim();
    var length = txtPhone.length;
    if (length == 0) {
        if (isEmpty == "1") {
            return true;
        }
        else
            return false;
    }
    if (txtPhone.indexOf('84') == 0) {
        txtPhone = txtPhone.substring(2, length - 2);
    } else {
        if (txtPhone.indexOf('84') == 0) {
            txtPhone = txtPhone.substring(3, length - 3);
        }
    }
    if ((txtPhone.indexOf('09') == 0 || txtPhone.indexOf('1') == 0) && txtPhone.length == 10) {
        return true;
    }
    if (txtPhone.indexOf('9') == 0 && txtPhone.length == 9) {
        return true;
    }
    if (txtPhone.indexOf('01') == 0 && txtPhone.length == 11) {
        return true;
    }
    return false;
}


function ShowLoading() {
    var element = $('body');
    if (element.length > 0) {
        element.append("<div class='loading-icon' style='position: fix'></div>");
        element.css("position", "relative");
    }
}

function HideLoading() {
    var element = $('body');
    if (element.length > 0) {
        element.find(".loading-icon").remove();
    }
}

