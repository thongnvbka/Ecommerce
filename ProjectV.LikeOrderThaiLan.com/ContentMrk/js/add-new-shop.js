
var LAST_STORE_COOKIE_NAME = "last_store";

$(function () {
    var lastStore = getCookie(LAST_STORE_COOKIE_NAME);
    if (lastStore !== null && lastStore !== "") {
        $(".subdomain").removeClass("hide");
        $("#login-form input[name=Subdomain]").val(lastStore);
        $("#login-form input[name=Subdomain]").attr("aria-required", "true");
        $("#login-form input[name=Subdomain]").attr("data-val-required", "Nhập vào đường dẫn website");
        $("#login-form").removeData("validator");
        $("#login-form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($("#login-form"));
    }
});

$(document).ready(function () {
    //luu thong tin tracking vao cookie
    var kd = getParameterByName("kd");
    if (kd !== null && kd !== "")
        setCookie("kd", kd, 30);

    var ref = getParameterByName("ref");
    if (ref !== null && ref !== "")
        setCookie("ref", ref, 30);

    var campaign = getParameterByName("campaign");
    if (campaign !== null && campaign !== "")
        setCookie("campaign", campaign, 30);

    if (document.referrer && document.referrer != '') {
        if (document.referrer.indexOf("www.bizweb.vn") == -1) {
            setCookie("referral", document.referrer, 30);
        }
    }

    var landingPage = getCookie("landing_page");
    if (landingPage == null || landingPage == "") {
        setCookie("landing_page", document.location.href, 0.0115);
    }

    var startTime = getCookie("start_time");
    if (startTime == null || startTime == "") {
        setCookie("start_time", "07/02/2017 2:20:13 CH", 0.0115);
    }

    var pageview = getCookie("pageview");
    if (pageview == null || pageview == "") {
        setCookie("pageview", 1, 0.0115);
    }
    else {
        setCookie("pageview", parseInt(pageview) + 1, 0.0115);
    }

    var updateCookie = setInterval(renewFirstPageCookie, 15 * 60 * 1000);

    var source = getParameterByName("source");
    var cookieName = "_bizweb_src";
    var sessionStorageData = getSessionStorage(cookieName);
    var cookieData = getCookie(cookieName);
    var cookieValue = cookieData || sessionStorageData || guid();
    if (!cookieData) {
        if (source)
            setCookie(cookieName, cookieValue, 30);
    }

    if (!sessionStorageData) {
        if (source)
            setSessionStorage(cookieName, cookieValue);
    }
    if (!cookieData && !sessionStorageData) {
        if (source) {
            var req = "/visit/record.gif?url=" + window.location.href + "&r=" + document.referrer + "&guid=" + cookieValue + "&code=" + source + "&ua=" + navigator.userAgent;
            new Image().src = req;
        }

    }

    $(document).on('click', '.forgot-password', function (e) {
        //$('#forget-email').val(' ');
        $('#forget-email').attr('placeholder', 'Nhập tên website của bạn').val("").focus().blur();
        $('#forget-div .error-email').hide();
        e.preventDefault();
        $.fancybox.close();
        $.fancybox.open({
            fitToView: false,
            closeClick: false,
            openEffect: 'fade',
            closeEffect: 'fade',
            href: "#forget-div",
            autoSize: true,
            margin: [20, 0, 20, 0]
        });
    });

    $(document).on('click', '.click-forget-btn', function (e) {
        if ($('#forget-email').val().length == 0) {
            $('#forget-email').focus();
            return false;
        }
        else if ($('#forget-email').val().length > 255) {
            $('.error-email span').text('Tên website không được vượt quá 255 ký tự');
            $('.error-email').show();
            return false;
        }
        else {
            var specialCharacter = "#!~$%^&*()[]{}\|/?";
            for (var i = 0; i < specialCharacter.length; i++) {
                if ($('#forget-email').val().indexOf(specialCharacter[i]) >= 0) {
                    $('.error-email span').text('Tên website không được chứa ký tự đặc biệt');
                    $('.error-email').show();
                    return false;
                }
            }
            var m = $('#forget-email').val();
            m = bodauTiengViet(m);
            var url = "";
            if (m.toLowerCase().indexOf("bizwebvietnam.net") >= 0) {
                url = "https://" + m.replace(/\s/g, '-') + "/admin/authorization/recover";
            }
            else {
                url = "https://" + m.replace(/\s/g, '-') + ".bizwebvietnam.net/admin/authorization/recover";
            }
            e.stopPropagation();
            e.preventDefault();
            window.location.href = url;
            return false;
        }
    });

    $("input[name=StoreName]").blur(function () {
        $("input[name=StoreAlias]").val(generateAlias($("input[name=StoreName]").val()));
    });

    var createStore = getParameterByName("createStore");
    if (createStore) {
        var storeName = getParameterByName("storeName");

        openCreateStorePopup(document, storeName);
    }
});

function renewFirstPageCookie() {
    var landingPage = getCookie("landing_page");
    if (landingPage !== null && landingPage !== "") {
        setCookie("landing_page", landingPage, 0.0115);
    }

    var startTime = getCookie("start_time");
    if (startTime !== null && startTime !== "") {
        setCookie("start_time", startTime, 0.0115);
    }

    var pageview = getCookie("pageview");
    if (pageview !== null || pageview !== "") {
        setCookie("pageview", pageview, 0.0115);
    }
}

function bodauTiengViet(str) {
    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    return str;
}

var mobile = false;
$(window).resize(function () {
    var ww = $(window).width();
    //if (ww > 855) {
    //    mobile = false;
    //    $('.fancybox-item.fancybox-close').addClass('fancybox-close-reg-web');
    //}
    //else {
    //    mobile = true;
    //    $('.fancybox-item.fancybox-close').removeClass('fancybox-close-reg-web');
    //}

    if (ww < 768) {
        if (!$('#login-div br').length > 0)
            $('#login-div .popup-login-text').before('<br/>');
    }
    else {
        $('#login-div br').remove();
    }
})
$(window).trigger('resize');

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function onInputStoreName(e, element) {
    if (e.keyCode == 13) {
        openCreateStorePopup(element);
        return false;
    }
}

function openNewCreateStorePopup(e, storeName, mpage, sourcename) {
    mpage = typeof mpage !== 'undefined' ? mpage : false;

    var iframeSrc = ' ';
    if (mpage == false) {
        iframeSrc = 'https://sendo-app.bizwebvietnam.net/admin/createstore';
    }
    else {
        iframeSrc = 'https://store.bizwebvietnam.net/admin/createstore/registermpage';
    }

    iframeSrc = setParameter(iframeSrc, "registerstep", "2step");

    if (!storeName)
        //storeName = $(e).closest(".quick-registration").find(".input-site-name").val();
        storeName = $(e).parent().find(".input-site-name").val();

    if (storeName != null && storeName != "") {
        iframeSrc = setParameter(iframeSrc, "storeName", storeName);
        iframeSrc = setParameter(iframeSrc, "storeAlias", generateAlias(storeName));
    }

    var sourcePar = getParameterByName('source');

    if (!sourcePar.length) {
        sourcename = typeof sourcename !== 'undefined' ? sourcename : '';
        if (sourcename != '') {
            iframeSrc = setParameter(iframeSrc, "source", sourcename);
        }
    }
    else {
        iframeSrc = setParameter(iframeSrc, "source", sourcePar);
    }
    showCreateStorePopup(iframeSrc);
}

function openCreateStorePopup(e, storeName, mpage, sourcename) {
    mpage = typeof mpage !== 'undefined' ? mpage : false;

    var iframeSrc = ' ';
    if (mpage == false) {
        iframeSrc = 'https://store.bizwebvietnam.net/admin/createstore/quickregister';
    }
    else {
        iframeSrc = 'https://store.bizwebvietnam.net/admin/createstore/registermpage';
    }
    iframeSrc = setParameter(iframeSrc, "registerstep", "1step");
    if (!storeName)
        //storeName = $(e).closest(".quick-registration").find(".input-site-name").val();
        storeName = $(e).parent().find(".input-site-name").val();

    if (storeName != null && storeName != "") {
        iframeSrc = setParameter(iframeSrc, "storeName", storeName);
        iframeSrc = setParameter(iframeSrc, "storeAlias", generateAlias(storeName));
    }

    //var sourcePar = getParameterByName('source');

    //if (!sourcePar.length) {
    //    sourcename = typeof sourcename !== 'undefined' ? sourcename : '';
    //    if (sourcename != '') {
    //        iframeSrc = setParameter(iframeSrc, "source", sourcename);
    //    }
    //}
    //else {
    //    iframeSrc = setParameter(iframeSrc, "source", sourcePar);
    //}

    showCreateStorePopup(iframeSrc);

}
function getMobileOperatingSystem() {

}

function showCreateStorePopup(iframeSrc) {
    var kd = getParameterByName("kd");
    if (kd !== null && kd !== "")
        setCookie("kd", kd, 30);

    kd = getCookie("kd");
    if (kd !== null && kd !== "")
        iframeSrc = setParameter(iframeSrc, "kd", kd);

    var ref = getParameterByName("ref");
    if (ref !== null && ref !== "")
        setCookie("ref", ref, 30);

    ref = getCookie("ref");
    if (ref !== null && ref !== "")
        iframeSrc = setParameter(iframeSrc, "ref", ref);

    if (window.location.href && window.location.href != '') {
        iframeSrc = setParameter(iframeSrc, "source", encodeURIComponent(window.location.href));
    }

    var referral = getCookie("referral");
    if (referral !== null && referral !== "")
        iframeSrc = setParameter(iframeSrc, "referral", encodeURIComponent(referral));

    var campaign = getParameterByName("campaign");
    if (campaign !== null && campaign !== "")
        setCookie("campaign", campaign, 30);

    campaign = getCookie("campaign");
    if (campaign !== null && campaign !== "")
        iframeSrc = setParameter(iframeSrc, "campaign", campaign);

    var landingPage = getCookie("landing_page");
    if (landingPage !== null && landingPage !== "")
        iframeSrc = setParameter(iframeSrc, "landing_page", encodeURIComponent(landingPage));

    var startTime = getCookie("start_time");
    if (startTime !== null && startTime !== "")
        iframeSrc = setParameter(iframeSrc, "start_time", encodeURIComponent(startTime));

    var endTime = "07/02/2017 2:20:13 CH";
    if (endTime !== null && endTime !== "")
        iframeSrc = setParameter(iframeSrc, "end_time", encodeURIComponent(endTime));

    var pageview = getCookie("pageview");
    if (pageview !== null && pageview !== "")
        iframeSrc = setParameter(iframeSrc, "pageview", pageview);

    $('#registration iframe').attr('src', iframeSrc);

    $.fancybox.open({
        fitToView: false,
        closeClick: false,
        openEffect: 'fade',
        closeEffect: 'fade',
        href: "#registration",
        width: "100%",
        maxWidth: 847,
        autoSize: false,
        autoHeight: true,
        margin: [20, 0, 20, 0],
        //'afterLoad': getMobileOperatingSystem(),
        'afterShow': function () {
            $('.fancybox-item.fancybox-close').addClass('new-fancybox-close');
            $('.fancybox-item.fancybox-close').parent().addClass('new-fancybox-wrapper');
            $('#container').addClass('blur-function');
            var userAgent = navigator.userAgent || navigator.vendor || window.opera;
            if (!(/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream)) {
                if ($(window).width() < 768) {
                    $('.fancybox-inner').addClass('ios-fancybox');
                }
            }

        },
        'afterClose': function () {
            $('.fancybox-item.fancybox-close').removeClass('fancybox-close-reg');
            $('.fancybox-item.fancybox-close').removeClass('fancybox-close-reg-web');
            $('#container').removeClass('blur-function');
        }
    });
}

function generateAlias(text) {
    text = text.toLowerCase();
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    text = text.replace(/'|\"|\(|\)|\[|\]/g, "");
    text = text.replace(/\W+/g, "-");
    if (text.slice(-1) === "-")
        text = text.replace(/-+$/, "");

    if (text.slice(0, 1) === "-")
        text = text.replace(/^-+/, "");

    return text;
}

function openLoginPopup() {
    //$('#login-div #Email-error').hide();
    //$('#login-div #Password-error').hide();
    //$('#login-div .subdomain').hide();
    //$('#login-div .subdomain .form-input').attr('placeholder', 'Đường dẫn website').val("").focus().blur();
    //$("#login-form .errors ul").html("");
    $('#login-div #Email').attr('placeholder', 'Địa chỉ email').val("").focus().blur();
    $('#login-div input[name=Password]').attr('placeholder', 'Mật khẩu').val("").focus().blur();
    $.fancybox.open({
        fitToView: false,
        closeClick: false,
        openEffect: 'fade',
        closeEffect: 'fade',
        href: "#login-div",
        autoSize: true,
        margin: [20, 0, 20, 0]
    });
}

function login() {
    var bizwebDomain = 'https://store.bizwebvietnam.net';
    var findSubdomainSrc = bizwebDomain + '/admin/services/findsubdomain';

    $("#login-form .errors ul").html("");

    if ($("#login-form").valid()) {
        $.ajax({
            url: findSubdomainSrc,
            type: "POST",
            data: $("#login-form").serialize(),
            success: function (data) {
                var loginSubdomain = $("#login-form input[name=Subdomain]").val();
                if (data != null && (loginSubdomain === null || loginSubdomain === ""))
                    loginSubdomain = data.subdomain;

                var action = bizwebDomain.replace("store", loginSubdomain) + "/admin/authorization/login";

                setCookie(LAST_STORE_COOKIE_NAME, loginSubdomain, 365);

                $("#login-form").attr("action", action);
                $("#login-form").submit();
            },
            error: function (data) {
                if (data.responseJSON != null && data.responseJSON.error != null) {
                    if (data.responseJSON.error) {
                        $("#login-form .errors ul").append("<li>Bạn nhập sai email hoặc mật khẩu.</li>");
                    }
                }
                else if (data.responseJSON != null && data.responseJSON.suggest != null) {
                    var message = "Có phải ý bạn là: '" + data.responseJSON.suggest + "' ?";
                    $(".subdomain .help-block .help-block").append("<span id='Subdomain-error' class=''>" + message + "</span>");
                }
                else if (data.responseJSON != null && data.responseJSON.multiple != null) {
                    $(".subdomain").removeClass("hide");
                    $(".subdomain .help-block .help-block").append("<span id='Subdomain-error' class=''>Có nhiều hơn một cửa hàng đăng ký dưới email này. Xin hãy nhập đường dẫn website để đăng nhập.</span>");
                }
                else {
                    $("#login-form .errors ul").append("<li>Bạn nhập sai email hoặc mật khẩu.</li>");
                }
            }
        });
    }

    return false;
}



function setCookie(cname, cvalue, exdays) {
    if (!exdays)
        exdays = 30;

    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires + ";domain=.bizweb.vn;path=/";
}

function newSetCookie(cname, cvalue) {
    document.cookie = cname + "=" + cvalue;
}

function getUrlWithoutDomain(url) {
    return url.replace(/^.*\/\/[^\/]+/, '');
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return null;
}

//function setCookie(cname, cvalue, exdays) {
//    var d = new Date();
//    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
//    var expires = "expires=" + d.toUTCString();
//    document.cookie = cname + "=" + cvalue + ";path=/";
//}

function getSessionStorage(sname) {
    return window.sessionStorage.getItem(sname);
}

function setSessionStorage(sname, svalue) {
    window.sessionStorage.setItem(sname, svalue);
}

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    }).toUpperCase();
}



function setParameter(url, paramName, paramValue) {
    if (url.indexOf(paramName + "=") >= 0) {
        var prefix = url.substring(0, url.indexOf(paramName));
        var suffix = url.substring(url.indexOf(paramName));
        suffix = suffix.substring(suffix.indexOf("=") + 1);
        suffix = (suffix.indexOf("&") >= 0) ? suffix.substring(suffix.indexOf("&")) : "";
        url = prefix + paramName + "=" + paramValue + suffix;
    }
    else {
        if (url.indexOf("?") < 0)
            url += "?" + paramName + "=" + paramValue;
        else
            url += "&" + paramName + "=" + paramValue;
    }

    return url;
}
