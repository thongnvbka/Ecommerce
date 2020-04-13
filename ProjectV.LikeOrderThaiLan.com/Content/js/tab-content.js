//shipby me

$(".show-ship-by-me").hide();
$(".show-ship-by-web").show();
$("#shipByWeb").change(function () {
    if (this.checked) {
        $(".show-ship-by-me").hide();
        $(".show-ship-by-web").show();
    }
});

$("#shipByMe").change(function () {
    if (this.checked) {
        $(".show-ship-by-me").show();
        $(".show-ship-by-web").hide();
    }
});

//format
//them san pham vao gio hang trong phan bao gia link
$("#js-dgCart-list-check").hide();
$("#js-dif-h-check").hide();

$("#js-dgCart-list-uncheck").show();
$("#js-dif-h-uncheck").show();

function addToCart() {
    $("#js-dgCart-list-check").show();
    $("#js-dif-h-check").show();
    $("#js-dgCart-list-uncheck").hide();
    $("#js-dif-h-uncheck").hide();
};
//xoa san pham
function deleteCartProduct() {
    $("#js-dgCart-list-check").hide();
    $("#js-dif-h-check").hide();
    $("#js-dgCart-list-uncheck").show();
    $("#js-dif-h-uncheck").show();
};
//them san pham vao gio hang trong phan bao gia link

//next-preview
//next1
function next1() {
    $("#selectProduct").hide();
    $("#selectStorage").show();
    $("#packing").hide();
    $("#delivery").hide();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:nth-of-type(2)").addClass('lete-active');
}
function next2() {
    $("#selectProduct").hide();
    $("#selectStorage").hide();
    $("#packing").show();
    $("#delivery").hide();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:nth-of-type(3)").addClass('lete-active');
}
function next3() {
    $("#selectProduct").hide();
    $("#selectStorage").hide();
    $("#packing").hide();
    $("#delivery").show();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:nth-of-type(4)").addClass('lete-active');
}
//prev

function prv1() {
    $("#selectProduct").show();
    $("#selectStorage").hide();
    $("#packing").hide();
    $("#delivery").hide();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:first").addClass('lete-active');
}
function prv2() {
    $("#selectProduct").hide();
    $("#selectStorage").show();
    $("#packing").hide();
    $("#delivery").hide();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:nth-of-type(2)").addClass('lete-active');
}
function prv3() {
    $("#selectProduct").hide();
    $("#selectStorage").hide();
    $("#packing").show();
    $("#delivery").hide();
    $("ul#clicktabTicket > li").removeClass('lete-active');//remove toan bo classs
    $("ul#clicktabTicket > li:nth-of-type(3)").addClass('lete-active');
}
 
//tao don hang tim nguon
$("#tab_1").show();
$("#createsourcing").hide();
function create() {
    $("#tab_1").hide();
    $("#createsourcing").show();
};
//tab FAQ
$(".faq-nav-box").on("click", "h1", function () {
    var that = $(this);
    $(this).parent().find("ul").slideToggle(function () {
        if ($(this).css("display") === "none") {
            that.find("span").css("background-position", "0 0");
        } else {
            that.find("span").css("background-position", "-11px 0");
        }
    });
});

//delete notification
$('a.delete-notification').click(function () {
    //lấy giá trị thuộc tính href -#delete-box"
    var loginBox = $(this).attr('href');
    //time
    $(loginBox).fadeIn("slow");
    //add vao the body
    $('body').append('<div id="over"></div>');
    $('#over').fadeIn(300);

    return false;
});

// khi click đóng hộp thoại
$(document).on('click', "button.close-box, #over,#delete-box", function () {
    $('#over, .zhezhao-del').fadeOut(300, function () {
        $('#over').remove();
    });
    return false;
});