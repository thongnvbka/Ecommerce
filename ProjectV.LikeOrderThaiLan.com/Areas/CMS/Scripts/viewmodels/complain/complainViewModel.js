var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var ComplainViewModel = function () {
    var self = this;
    //Todo Khai báo
    self.active = ko.observable('');
    self.listAllComplain = ko.observableArray([]);
    self.listTypeComplain = ko.observableArray([]);
    self.selectedService = ko.observableArray([]);
    self.data = ko.observable();
    self.titleTicket = ko.observable();
    self.inforTicket = ko.observable();
    self.isRending = ko.observable(false);
    self.isRendingPage = ko.observable(false);
    self.templateId = ko.observable("ticket");
    self.isSubmit = ko.observable(true);
    self.isSuccess = ko.observable(true);

    //==================== Search Object -
    self.Keyword = ko.observable("");
    self.Status = ko.observable(-1);
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    //Todo khai bao bien model show du lieu tren view cua bang status
    self.complainStatusItemModel = ko.observable(new complainStatusItemModel());
    //Todo Object chi tiết status
    self.mapComplainStatusItemModel = function (data) {
        self.complainStatusItemModel(new complainStatusItemModel());
        self.complainStatusItemModel().Wait(data.Wait);
        self.complainStatusItemModel().Process(data.Process);
        self.complainStatusItemModel().Success(data.Success);
        self.complainStatusItemModel().Cancel(data.Cancel);
    };
    //Todo Lấy danh sách TICKET
    self.GetAllComplain = function () {
        self.listAllComplain([]);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/Ticket/GetListComplain", {
            PageIndex: page,
            PageSize: pagesize,
            Keyword: self.Keyword(),
            Status: self.Status(),
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime()
        }, function (data) {
            total = data.Page.Total;
            self.listAllComplain(data.ListItems);
            self.mapComplainStatusItemModel(data.ComplainStatusItem);
            self.paging();
            self.isRending(true);
            self.isRendingPage(true);
        });
    }

    //Todo chi tiết khiếu nại
    self.detaiTicket = function (data) {
        window.location.href = "/" + window.culture + "/CMS/Ticket/DetailTicket?ticketId=" + data.id;
    }

    self.backToList = function () {
        self.GetAllComplain();
        self.templateId("ticket");
    };

    self.listTypeService = function (str) {
        var list = ko.observableArray([]);
        var array = str.split(',');
        _.each(array, function (item) {
            list.push({ Name: window.listTicketType[item].Text });
        });

        return list;
    }

    //Todo==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    self.clickSearch = function () {
        page = 1;
        self.GetAllComplain();
    };

    //Todo /search lúc phân trang
    self.search = function (page) {
        self.GetAllComplain();
    };

    //Todo Hàm khởi tạo phân trang
    self.paging = function () {
        var listPage = [];

        page = page <= 0 ? 1 : page;
        pageTotal = Math.ceil(total / pagesize);
        page > 3 ? self.pageStart(true) : self.pageStart(false);
        page > 4 ? self.pageNext(true) : self.pageNext(false);
        pageTotal - 2 > page ? self.pageEnd(true) : self.pageEnd(false);
        pageTotal - 3 > page ? self.pagePrev(true) : self.pagePrev(false);

        var start = (page - 2) <= 0 ? 1 : (page - 2);
        var end = (page + 2) >= pageTotal ? pageTotal : (page + 2);

        for (var i = start; i <= end; i++) {
            listPage.push({ Page: i });
        }

        self.listPage(listPage);
        self.pageTitle(window.messager.pageList.show + " <b>" + (((page - 1) * pagesize) + 1) +
        "</b> " + window.messager.pageList.to + " <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) +
        "</b> " + window.messager.pageList.of + " <b>" + total + "</b> " + window.messager.pageList.record);
       // self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) + "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> của <b>" + total + "</b> Bản ghi");
    }

    // Search status
    self.searchStatus = function (tmpStatus) {
        page = 1;
        self.Status(tmpStatus);
        self.search(1);
    }
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
        self.Status(-1);
        self.search(1);
    }
    self.SetDate = function () {
        self.StartSearch($('#date_timepicker_start').val());
        self.FinishSearch($('#date_timepicker_end').val());
        var startDate = $('#date_timepicker_start').val();
        if (startDate.length > 0) {
            var arrStart = startDate.split('/');
            if (arrStart.length == 3) {
                startDate = arrStart[1] + '/' + arrStart[0] + '/' + arrStart[2] + ' 00:00';
                self.StartDate(startDate);
            }
        }
        var finishDate = $('#date_timepicker_end').val();
        if (finishDate.length > 0) {
            var arrFinish = finishDate.split('/');
            if (arrFinish.length == 3) {
                finishDate = arrFinish[1] + '/' + arrFinish[0] + '/' + arrFinish[2] + ' 23:00';
                self.FinishDate(finishDate);
            }
        }
        page = 1;
        self.AllTime(1);
        self.Status(-1);
        self.search(1);
    }
    self.FinishSearch.subscribe(function () {
        self.SetDate();
    });
    self.StartSearch.subscribe(function () {
        self.SetDate();
    });

    // Search status
    self.searchKeyword = function () {
        page = 1;
        self.search(1);
    }

    self.onEnter = function (d, e) {
        if (e.which == 13 || e.keyCode == 13) {
            self.Keyword($('#txtKeyword').val());
            self.search(1);
        }
        return true;
    };

    self.ticketnModel = ko.observable(new ComplainModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================

    self.mapTicketModel = function (data) {
        self.ticketnModel(new ComplainModel());

        self.ticketnModel().Id(data.Id);
        self.ticketnModel().Code(data.Code);
        self.ticketnModel().TypeOrder(data.TypeOrder);
        self.ticketnModel().TypeService(data.TypeService);
        self.ticketnModel().TypeServiceName(data.TypeServiceName);
        self.ticketnModel().TypeServiceClose(data.TypeServiceClose);
        self.ticketnModel().TypeServiceCloseName(data.TypeServiceCloseName);
        self.ticketnModel().ImagePath1(data.ImagePath1);
        self.ticketnModel().ImagePath2(data.ImagePath2);
        self.ticketnModel().ImagePath3(data.ImagePath3);
        self.ticketnModel().ImagePath4(data.ImagePath4);
        self.ticketnModel().ImagePath5(data.ImagePath5);
        self.ticketnModel().ImagePath6(data.ImagePath6);
        self.ticketnModel().Content(data.Content);
        self.ticketnModel().OrderId(data.OrderId);
        self.ticketnModel().OrderCode(data.OrderCode);
        self.ticketnModel().OrderType(data.OrderType);
        self.ticketnModel().CustomerId(data.CustomerId);
        self.ticketnModel().CustomerName(data.CustomerName);
        self.ticketnModel().CreateDate(data.CreateDate);
        self.ticketnModel().LastUpdateDate(data.LastUpdateDate);
        self.ticketnModel().SystemId(data.SystemId);
        self.ticketnModel().SystemName(data.SystemName);
        self.ticketnModel().Status(data.Status);
        self.ticketnModel().LastReply(data.LastReply);
        self.ticketnModel().BigMoney(data.BigMoney);
        self.ticketnModel().IsDelete(data.IsDelete);
        self.ticketnModel().RequestMoney(data.RequestMoney);
        self.ticketnModel().Content(data.Content);
    };

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    // Tạo khiếu nại
    // File ảnh
    self.isUpload = ko.observable(true);
    self.DetailImagePath1 = ko.observable("");
    self.DetailImagePath2 = ko.observable("");
    self.DetailImagePath3 = ko.observable("");
    self.DetailImagePath4 = ko.observable("");
    var maxFileLength = 5120000;;
    self.addImage = function () {
        $(".flieuploadImg").fileupload({
            url: "/" + window.culture + "/CMS/Ticket/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + window.messager.errorTicket.errorWidthImg ;
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + window.messager.errorTicket.errorImgNotFormat ;
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error(window.messager.errorTicket.errorFileError);
                    return;
                }

                if (self.DetailImagePath1() !== "" &&
                    self.DetailImagePath2() !== "" &&
                    self.DetailImagePath3() !== "" &&
                    self.DetailImagePath4() !== "") {
                    self.DetailImagePath1(window.location.origin + data.result[0].path);
                } else {
                    if (self.DetailImagePath1() === "") {
                        self.DetailImagePath1(window.location.origin + data.result[0].path);
                    } else {
                        if (self.DetailImagePath2() === "") {
                            self.DetailImagePath2(window.location.origin + data.result[0].path);
                        } else {
                            if (self.DetailImagePath3() === "") {
                                self.DetailImagePath3(window.location.origin + data.result[0].path);
                            } else {
                                if (self.DetailImagePath4() === "") {
                                    self.DetailImagePath4(window.location.origin + data.result[0].path);
                                }
                            }
                        }
                    }
                }

                $("div").removeClass("hover");
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    self.createTicket = function () {
        
        if (self.selectedService() == undefined || self.selectedService() == 0) {
            toastr.error(window.messager.errorTicket.errorTypeTicket);
        }
        else if (self.ticketnModel().OrderId() == "") {
            toastr.error(window.messager.errorTicket.errorOrderCode);
        }
        else if (self.ticketnModel().Content() == "") {
            toastr.error(window.messager.errorTicket.errorContentTicket);
        }
        else {
            self.isSubmit(false);

            self.ticketnModel().ImagePath1(self.DetailImagePath1());
            self.ticketnModel().ImagePath2(self.DetailImagePath2());
            self.ticketnModel().ImagePath3(self.DetailImagePath3());
            self.ticketnModel().ImagePath4(self.DetailImagePath4());
            self.ticketnModel().TypeService(self.selectedService());
            $.post("/" + window.culture + "/CMS/Ticket/CreateTicket", { complain: self.ticketnModel() }, function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                } else {
                    toastr.success(result.msg);
                    window.location.href = "/" + window.culture + "/CMS/Ticket/Ticket";
                    
                }
            });
        }
    }

    self.initInputMark = function () {
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    $(function () {
        self.initInputMark();
        self.selectedService([]);
        self.ticketnModel(new ComplainModel());
        self.GetAllComplain();
        self.listTypeComplain(window.listTicketType);
        if (parseInt(window.orderId) > 0) {
            self.ticketnModel().OrderId(window.orderId);
            self.ticketnModel().OrderCode(window.orderCode);
        }
        $("#effect-7 .img").hover(function () {
            $("#effect-7 .img .overlay").css("width", "100%");
        }
       , function () {
           $("#effect-7 .img .overlay").css("width", "0");
       });
        var urlAuto = "/" + window.culture + "/CMS/Ticket/SearchAutoComplete";
        $("#txtOrderCode").autocomplete({
            source: function (request, response) {
                var obj = {
                    q: request.term,
                    top: 5
                };
                $.ajax({
                    url: urlAuto,
                    type: "GET",
                    data: obj,
                    contentType: "application/json; charset=utf-8",
                    datatype: "jsondata",
                    async: "true",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                value: item.OrderId,
                                label: item.OrderCode,
                                icon: item.ImagePath,
                                productCount: item.ProductCount,
                                totalPrice: item.TotalPrice,
                                totalExchange: item.TotalExchange,
                                created: item.created
                            };
                        }));
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        toastr.error("ไม่มีออเดอร์นี้");
                    }
                });
            },
            create: function (event, ui) {
                $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                    return $('<li>')
                        .append("<a class=\"_k_\" rel=\"ignore\" role=\"button\" id=\"js_bg\"><div class=\"clearfix pvs\"><div class=\"MercuryThreadImage mrm lfloat _ohe\"><div class=\"_55lt\" size=\"50\" style=\"width:50px;height:50px;\" data-reactid=\".st\"><img src=\"" + item.icon + "\" width=\"50\" height=\"50\" class=\"img\" data-reactid=\".st.0\"></div></div><div class=\"_l4\"><div class=\"_l2\"><span aria-hidden=\"true\" class=\"_l1\">Mã đơn: " + item.label + " - " + item.created + "</span></div><div class=\"clearfix\"><div class=\"_l3 fsm fwn fcg\"><span class=\"MercuryRepliedIndicator seenByListener\"></span>Tổng tiền hàng:" + item.totalPrice + "¥ <br />Tổng tiền sau quy đổi: " + item.totalExchange + "</div></div></div></div></a>")
                        .appendTo(ul);
                };
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                self.ticketnModel().OrderId(ui.item.value);
                self.ticketnModel().OrderCode(ui.item.label);
                $("#txtOrderCode").val(ui.item.label);
                return false;
            },
            autoFocus: true,
            focus: function (event, ui) {
                $("#txtOrderCode").val(ui.item.label);
                return false;
            },
        });
        $(".customer-search")
           .select2({
               ajax: {
                   url: "/" + window.culture + "/CMS/Ticket/GetOrderSearch",
                   dataType: 'json',
                   delay: 250,
                   data: function (params) {
                       return {
                           keyword: params.term,
                           page: params.page
                       };
                   },
                   processResults: function (data, params) {
                       params.page = params.page || 1;

                       return {
                           results: data.items,
                           pagination: {
                               more: (params.page * 10) < data.total_count
                           }
                       };
                   },
                   cache: true
               },
               escapeMarkup: function (markup) { return markup; },
               minimumInputLength: 1,
               templateResult: function (repo) {
                   if (repo.loading) return repo.code;
                   var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.code + "</b><br/>\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                   return markup;
               },
               templateSelection: function (repo) {
                   return repo.code;
               },
               placeholder: "",
               allowClear: true,
               language: 'vi'
           });
    });
}

var viewModelTicket = new ComplainViewModel();
//ko.applyBindings(new ComplainViewModel());
ko.applyBindings(viewModelTicket, $("#viewTicketCommon")[0]);