//menu toolbar
var set1;
var status = true;
$("body").on("mousemove", function () {
    clearTimeout(set1);
    if (status === "true") {
        $(".lete-toolbar").animate({
            right: "0"
        }, 500);
        status = false;
    }
    set1 = setTimeout(function () {
        $(".lete-toolbar").animate({
            right: "-100%"
        }, 1000);
        status = true;
    }, 2000);
});


//back to top
if ($("#toTop").length) {
    var scrollTrigger = 100, // px
        backToTop = function () {
            var scrollTop = $(window).scrollTop();
            if (scrollTop > scrollTrigger) {
                $("#back-to-top").addClass('lete-toolbar-bottpm');
            } else {
                $("#back-to-top").removeClass('lete-toolbar-bottpm');
            }
        };
    backToTop();
    $(window).on("scroll", function () {
        backToTop();
    });
    $("#toTop").on("click", function (e) {
        e.preventDefault();
        $("html,body").animate({
            scrollTop: 0
        }, 700);
    });
};
if ($("#toTopOnHideToolBar").length) {

    $(window).on("scroll", function () {
    });
    $("#toTopOnHideToolBar").on("click", function (e) {
        e.preventDefault();
        $("html,body").animate({
            scrollTop: 0
        }, 700);
    });
};

//backlink
//Cookies.set("name", "custumerCreateOrderDeposit");

$("#createOrder").addClass("lete-active");
$("#createDeposit").removeClass("lete-active");
//document.getElementById("inputLinkOrder").disabled = false;
//document.getElementById("inputLinkOrder").style.background = '#fff';

$("#createOrder").click(function () {
    $("#createOrder").addClass("lete-active");
    document.getElementById("inputLinkOrder").disabled = false;
    $("#createDeposit").removeClass("lete-active");
    document.getElementById("inputLinkOrder").style.background = '#fff';
    Cookies.set("name", "Order");
    var url = $('#inputLinkOrder').val();
    location.href = "/" + window.culture + "/CMS/Order/CreateOrder?url=" + url;
});
$("#createDeposit").click(function () {
    $("#createOrder").removeClass("lete-active");
    $("#createDeposit").addClass("lete-active");
    Cookies.set("name", "Deposit");
    document.location.href = "/" + window.culture + "/CMS/Order/CreateDeposit";
    document.getElementById("inputLinkOrder").disabled = true;
    document.getElementById("inputLinkOrder").style.background = "#ddd";
    document.getElementById("inputLinkOrder").style.color = "#fff";
    //Cookies.get('name', 'custumerCreateOrderDeposit');
});


//multi language
$(document).ready(function () {
    var path = window.location.pathname;
    var cultureName = path.split("/")[1];
    var html = $("#flag-" + cultureName).html();
    $("#flagShow").html(html);
});

function getFlag(culture) {
    var path = window.location.pathname;
    var pathArray = path.split("/");
    var url = "/";
    var x;
    for (x in pathArray) {
        if (x == 1)
            url += culture + "/";
        else {
            if (pathArray[x] != "")
                url += pathArray[x] + "/";
        }
    }

    window.location.href = url.slice(0, -1);
}
//ADD TOOLTIP VÀ ẨN TOOLTIP
jQuery(function () {
    var firstInput = jQuery(".infor-order-deposit").find("input[name='ProductName']");
    //jQuery("#js-name-tips").css("left", (parseInt(jQuery(firstInput).offset().left)) + "px");
    //jQuery("#js-name-tips").css("top", (parseInt(jQuery(firstInput).offset().top)) + "px");
    jQuery("#js-name-tips").show();

    jQuery(firstInput).focus(function () {
        jQuery("#js-name-tips").hide();
    });

    if (Cookies('name') == 'Deposit') {
        $("#createOrder").removeClass("lete-active");
        $("#createDeposit").addClass("lete-active");
        document.getElementById("inputLinkOrder").disabled = true;
        document.getElementById("inputLinkOrder").style.background = "#ddd";
        document.getElementById("inputLinkOrder").style.color = "#fff";
    }

    if (Cookies('name') == 'Order') {
        $("#createOrder").addClass("lete-active");
        document.getElementById("inputLinkOrder").disabled = false;
        $("#createDeposit").removeClass("lete-active");
        document.getElementById("inputLinkOrder").style.background = '#fff';
    }

});//slide show huong dan tao don ky gui

//disabe f12 +ctrl shift i
//$(document).keydown(function (event) {
//    if (event.keyCode === 123) {
//        return false;
//    }
//    else if (event.ctrlKey && event.shiftKey && event.keyCode === 73) {
//        return false;
//    }
//    return false;
//});

//$(document).on("contextmenu", function (e) {
//    e.preventDefault();
//});
