$("#zhuanyun").on("click", function () {
    $("#tatol").attr("disabled", "disabled").val(0)
})
$("#daigou").on("click", function () {
    $("#tatol").removeAttr("disabled").val("")
})
$("#begin").on("click", function () {
    if ($("#tatol").val() == "") {
        clearTimeout(set1);
        $("#tip_msg").html("请填写商品总价");
        var set1 = setTimeout(function () {
            $("#tip_msg").html("");
        }, 2000);
    } else if (isNaN($("#tatol").val()) || $("#tatol").val() < 0) {
        clearTimeout(set2);
        $("#tip_msg").html("请填写正确商品总价,必须是数字且大于0");
        var set2 = setTimeout(function () {
            $("#tip_msg").html("");
        }, 2000);
    } else if ($("#weight").val() == "") {
        clearTimeout(set3);
        $("#tip_msg").html("请填写商品重量");
        var set3 = setTimeout(function () {
            $("#tip_msg").html("");
        }, 2000);
    } else if (isNaN($("#weight").val()) || $("#weight").val() <= 0) {
        clearTimeout(set4);
        $("#tip_msg").html("请填写正确商品重量,必须是数字且大于0");
        var set4 = setTimeout(function () {
            $("#tip_msg").html("");
        }, 2000);
    } else if ($("#address").attr("data-text") == "") {
        clearTimeout(set5);
        $("#tip_msg").html("请选择收货地址");
        var set5 = setTimeout(function () {
            $("#tip_msg").html("");
        }, 2000);
    } else {
        var typeId;
        $(".feiyonggusuan-guonei-add-box-l-box").find("input[type='radio']").each(function () {
            if (this.checked) {
                typeId = this.value
            }
        });
        var msg = '<tr>'
                    + '<th>运送方式</th>'
                    + '<th>运输特点</th>'
                    + '<th>预计运期</th>'
                    + '<th>商品总价(￥)</th>'
                    + '<th>运费(￥)</th>'
                    + '<th>报关费(￥)</th>'
                    + '<th>服务费(￥)</th>'
                    + '<th>总价(￥)</th>'
                + '</tr>'
                + '<tr><td colspan="8"><img src="themes/lete/img/loading.gif" /></td></tr>';
        $("#ship_list").html(msg)
        $.ajax({
            type: "post",
            url: "ship.php?act=shipping_count",
            async: true,
            data: {
                goods_amount: $("#tatol").val(),
                order_type: typeId,
                weight: $("#weight").val(),
                country: $("#address").attr("data-text")
            },
            success: function (data) {
                var data = JSON.parse(data);
                var str = '<tr>'
                    + '<th>运送方式</th>'
                    + '<th>运输特点</th>'
                    + '<th>预计运期</th>'
                    + '<th>商品总价(￥)</th>'
                    + '<th>运费(￥)</th>'
                    + '<th>报关费(￥)</th>'
                    + '<th>服务费(￥)</th>'
                    + '<th>总价(￥)</th>'
                    + '</tr>';
                for (var i = 0; i < data.length; i++) {
                    var status = true;
                    for (n in data[i]) {
                        if (data[i][n] == "" && n != "goods_amount") {
                            status = false
                        }
                    }
                    if (data[i].name == undefined || data[i].spec == undefined || data[i].time == undefined || data[i].goods_amount == undefined || data[i].price == undefined || data[i].baoguan_fee == undefined || data[i].server_fee == undefined || data[i].tol_fee == undefined) {
                        status = false
                    }
                    if (status) {
                        str += '<tr>'
                    + '<td>' + data[i].name + '</td>'
                    + '<td>' + data[i].spec + '</td>'
                    + '<td>' + data[i].time + '</td>'
                    + '<td>' + data[i].goods_amount + '</td>'
                    + '<td>' + data[i].price + '</td>'
                    + '<td>' + data[i].baoguan_fee + '</td>'
                    + '<td>' + data[i].server_fee + '</td>'
                    + '<td>' + data[i].tol_fee + '</td></tr>';
                    }
                }
                $("#ship_list").html(str)
            }
        });
    }
})
$("#yunfei_tap1").on("click", function () {
    $(this).addClass("lete-active");
    $("#yunfei_tap2").removeClass("lete-active");
    $("#yunfei-content1").show();
    $("#yunfei-content2").hide();
})

$("#yunfei_tap11").on("click", function () {
    $(".yunfei-search-form-main-top11").removeClass("lete-active");
    $(this).addClass("lete-active");
    $(".yunfei-search-form-main-content11").hide();
    $("#yunfei-content11").show();
})
$("#yunfei_tap12").on("click", function () {
    $(".yunfei-search-form-main-top11").removeClass("lete-active");
    $(this).addClass("lete-active");
    $(".yunfei-search-form-main-content11").hide();
    $("#yunfei-content12").show();
})
$("#yunfei_tap13").on("click", function () {
    $(".yunfei-search-form-main-top11").removeClass("lete-active");
    $(this).addClass("lete-active");
    $(".yunfei-search-form-main-content11").hide();
    $("#yunfei-content13").show();
})
$("#yunfei_tap14").on("click", function () {
    $(".yunfei-search-form-main-top11").removeClass("lete-active");
    $(this).addClass("lete-active");
    $(".yunfei-search-form-main-content11").hide();
    $("#yunfei-content14").show();
})
$("#yunfei_tap15").on("click", function () {
    $(".yunfei-search-form-main-top11").removeClass("lete-active");
    $(this).addClass("lete-active");
    $(".yunfei-search-form-main-content11").hide();
    $("#yunfei-content15").show();
})
$(".yunfei-search-form-main").on("click", "a", function () {
    $("#address").val($(this).text());
    $("#address").attr("data-text", $(this).attr("data-text"));
    $(".yunfei-search-form-main").hide();
    $("#address").css("background-position", "right 12px")
})
$("#address").on("click", function () {
    if ($(".yunfei-search-form-main").css("display") == "none") {
        $(".yunfei-search-form-main").show();
        $(this).css("background-position", "right -15px")
    } else {
        $(".yunfei-search-form-main").hide();
        $(this).css("background-position", "right 12px")
    }
})