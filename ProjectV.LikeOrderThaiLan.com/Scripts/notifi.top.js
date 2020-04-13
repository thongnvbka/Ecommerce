function GetNotifiTopCard() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/NotifiTopOrder',
        success: function (data) {
            if (data.length > 0) {
                $('#notifi-card').html(data);
            }
        }
    });
}
function GetNotifiTopCommon() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/NotifiTopCommon',
        success: function (data) {
            if (data.length > 0) {
                $('#notifi-common').html(data);
            }
        }
    });
}
function UpdateNotifiTopCommon() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/UpdateNotifiTopCommon',
        success: function (data) {
            $('#notifi-common .show-data').hide();
        }
    });
}
function GetNotifiTop() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/OrderNotifiTop',
        success: function (data) {
            if (data.length > 0) {
                $('#notifi-private').html(data);
            }
        }
    });
}
function UpdateNotifiTop() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/UpdateNotifiTopOrder',
        success: function (data) {
            $('#notifi-private .show-data').hide();
        }
    });
}
function GetWallet() {
    $.ajax({
        type: "GET",
        url: '/' + window.culture + '/CMS/Notifi/GetWallet',
        success: function (data) {
            if (data.length > 0) {
                if ($('#toolbar-money').length > 0) {
                    $('#toolbar-money').text(data);
                }
            }
        }
    });
}
$(document).ready(function () {
    setTimeout(GetNotifiTopCard(), 3 * 60 * 1000);
    $('#notifi-private').click(function () {
        if ($('#notifi-private .show-data').text() == "0") {
            GetNotifiTopCard();
        }
    });
    setTimeout(GetNotifiTop(), 3 * 60 * 1000);
    $('#notifi-card').click(function () {
        if ($('#notifi-card .show-data').text() == "0") {
            GetNotifiTop();
        }
    });
    GetWallet();
});