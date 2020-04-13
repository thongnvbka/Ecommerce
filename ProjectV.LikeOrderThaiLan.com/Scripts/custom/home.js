$(document).ready(function () {
    var arr = $(".home-map-box");
    var res = [];
    var len = arr.length;
    for (var i = 0; i < len; i++) {
        var j = Math.floor(Math.random() * arr.length);
        res[i] = arr[j];
        arr.splice(j, 1);
    }
    $(".home-tool-main-tcontent").on("mouseover", "li", function () {
        $(".home-tool-main-tcontent").find("li").removeClass("lete-active").each(function () {
            var id = $(this).attr("data-id");
            $(this).find("img").attr("src", "/Content/images/icon-home-master/home_" + id + "_icon.png");
        });
        $(this).addClass("lete-active");
        var id = $(this).attr("data-id");
        $(this).find("img").attr("src", "/Content/images/icon-home-master/home_" + id + "_icon_s.png");
        $(".home-tool-main-tcontent-box").css("display", "none");
        $("." + id + "_msg").css("display", "block");
    });

    ////TODO [Gioi] Fix Notification
    if ($.cookie('showNotification') !== 'true') {
        var id = '#dialog';
        //Get the screen height and width
        var maskHeight = $(document).height();
        var maskWidth = $(window).width();

        //Set heigth and width to mask to fill up the whole screen
        $('#mask').css({ 'width': maskWidth, 'height': maskHeight });

        //transition effect
        $('#mask').fadeIn(0);
        $('#mask').fadeTo("slow", 0.8);

        //transition effect
        $(id).fadeIn(0);

        //if close button is clicked
        $('.window .close')
            .click(function (e) {
                //Cancel the link behavior
                e.preventDefault();

                $('#mask').hide();
                $('.window').hide();
            });

        //if mask is clicked
        $('#mask')
            .click(function () {
                $(this).hide();
                $('.window').hide();
            });

        $.cookie('showNotification', 'true', { expires: 0.1 });
    }


    //if ($.cookie('showNotification') === 'true') {
    //    $("#createOrder").removeClass("lete-active");
    //    $("#createDeposit").addClass("lete-active");

    //    document.getElementById("inputLinkOrder").disabled = true;
    //    document.getElementById("inputLinkOrder").style.background = '#ddd';
    //    document.getElementById("inputLinkOrder").style.color = '#fff';
    //    //document.location.href = "/" + window.culture + "/CMS/Order/CreateDeposit";

    //    $.cookie('showNotification', 'true', { expires: 0.1 });
    //}
})