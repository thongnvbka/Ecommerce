var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var DepositViewModel = function (depositPopupModel) {
    var self = this;
    self.active = ko.observable('');
    //Todo Khai báo ListData đổ dữ liệu danh sách order
    self.listAll = ko.observableArray([]);
    self.listOrderService = ko.observableArray([]);
    self.listProductDetail = ko.observableArray([]);
    self.listOrderPackage = ko.observableArray([]);
    self.listOrderComment = ko.observableArray([]);
    self.listSourceSupplier = ko.observableArray([]);
    self.isRending = ko.observable(false);
    self.isRendingPage = ko.observable(false);
    self.depositDetail = ko.observable(new depositDetailModel());

    self.templateId = ko.observable("deposit");
    //==================== Search Object -
    self.Keyword = ko.observable("");
    self.Status = ko.observable(-1);
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    self.StartStatus = ko.observable(0);
    self.EndStatus = ko.observable(10);
    self.StartStatus1 = ko.observable(0);
    self.EndStatus1 = ko.observable(0);
    self.StartStatus2 = ko.observable(1);
    self.EndStatus2 = ko.observable(2);
    self.StartStatus3 = ko.observable(3);
    self.EndStatus3 = ko.observable(3);
    self.StartStatus4 = ko.observable(4);
    self.EndStatus4 = ko.observable(4);
    self.StartStatus5 = ko.observable(5);
    self.EndStatus5 = ko.observable(5);

    self.StartStatus6 = ko.observable(6);
    self.EndStatus6 = ko.observable(6);
    self.StartStatus7 = ko.observable(7);
    self.EndStatus7 = ko.observable(7);
    self.StartStatus8 = ko.observable(8);
    self.EndStatus8 = ko.observable(8);
    self.StartStatus9 = ko.observable(9);
    self.EndStatus9 = ko.observable(9);
    self.StartStatus10 = ko.observable(10);
    self.EndStatus10 = ko.observable(10);
    //Todo khai bao bien model show du lieu tren view cua bang status
    self.depositStatusItemModel = ko.observable(new depositStatusItemModel());
    //Todo Object chi tiết status
    self.mapDepositStatusItemModel = function (data) {
        self.depositStatusItemModel(new depositStatusItemModel());
        self.depositStatusItemModel().choBaoGia(data.choBaoGia);
        self.depositStatusItemModel().choXuLy(data.choXuLy);
        self.depositStatusItemModel().choKetDon(data.choKetDon);
        self.depositStatusItemModel().choXuatKho(data.choXuatKho);
        self.depositStatusItemModel().hangTrongKho(data.hangTrongKho);
        self.depositStatusItemModel().dangVanChuyen(data.dangVanChuyen);
        self.depositStatusItemModel().choGiaoHang(data.choGiaoHang);
        self.depositStatusItemModel().daGiaoHang(data.daGiaoHang);
        self.depositStatusItemModel().hoanThanh(data.hoanThanh);
        self.depositStatusItemModel().huy(data.huy);
    };
    //Todo Lấy danh sách deposit
    self.GetAll = function () {
        self.listAll([]);
        //self.isRending(false);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/Order/GetAllDepositList",
        {
            PageIndex: page,
            PageSize: pagesize,
            Keyword: self.Keyword(),
            Status: self.Status(),
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime(),
            StartStatus: self.StartStatus(),
            EndStatus: self.EndStatus(),
            StartStatus1: self.StartStatus1(),
            EndStatus1: self.EndStatus1(),
            StartStatus2: self.StartStatus2(),
            EndStatus2: self.EndStatus2(),
            StartStatus3: self.StartStatus3(),
            EndStatus3: self.EndStatus3(),
            StartStatus4: self.StartStatus4(),
            EndStatus4: self.EndStatus4(),
            StartStatus5: self.StartStatus5(),
            EndStatus5: self.EndStatus5(),
            StartStatus6: self.StartStatus6(),
            EndStatus6: self.EndStatus6(),
            StartStatus7: self.StartStatus7(),
            EndStatus7: self.EndStatus7(),
            StartStatus8: self.StartStatus8(),
            EndStatus8: self.EndStatus8(),
            StartStatus9: self.StartStatus9(),
            EndStatus9: self.EndStatus9(),
            StartStatus10: self.StartStatus10(),
            EndStatus10: self.EndStatus10()
        }, function (data) {
            total = data.Page.Total;
            self.listAll(data.ListItems);
            self.mapDepositStatusItemModel(data.DepositStatusItem);
            self.paging();
            $('.table-product tbody .option-order').each(function () {
                $(this).find('span:eq(0)').remove();
            });
            self.isRending(true);
            self.isRendingPage(true);
        });
    };

    self.clickSearch = function () {
        page = 1;
        self.GetAll();
    };
    //Todo /tìm kiếm
    self.search = function (page) {
        self.GetAll();
    };

    //Todo ==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

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
        //self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) +
        //    "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) +
        //    "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }
    // Search status
    self.searchStatus = function (tmpStatus, start, end) {
        page = 1;
        self.StartStatus(start);
        self.EndStatus(end);
        self.Status(tmpStatus);
        self.search(1);
    }
    //Search time
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
        self.Status(-1);
        self.search(1);
    }
    self.returnAtt = function (start, end, start1, end1) {
        if (start == start1 && end == end1) {
            return "outOfStock";
        }
        else {
            return "";
        }
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
    //export
    self.ExcelReport = function () {
        $.redirect("/" + window.culture + "/CMS/Order/ExportDeposit",
            {
                PageIndex: 1,
                PageSize: 1000000000,
                Keyword: self.Keyword(),
                Status: self.Status(),
                StartDateS: self.StartDate(),
                FinishDateS: self.FinishDate(),
                AllTime: self.AllTime(),
                StartStatus: self.StartStatus(),
                EndStatus: self.EndStatus(),
                StartStatus1: self.StartStatus1(),
                EndStatus1: self.EndStatus1(),
                StartStatus2: self.StartStatus2(),
                EndStatus2: self.EndStatus2(),
                StartStatus3: self.StartStatus3(),
                EndStatus3: self.EndStatus3(),
                StartStatus4: self.StartStatus4(),
                EndStatus4: self.EndStatus4(),
                StartStatus5: self.StartStatus5(),
                EndStatus5: self.EndStatus5(),
                StartStatus6: self.StartStatus6(),
                EndStatus6: self.EndStatus6(),
                StartStatus7: self.StartStatus7(),
                EndStatus7: self.EndStatus7(),
                StartStatus8: self.StartStatus8(),
                EndStatus8: self.EndStatus8(),
                StartStatus9: self.StartStatus9(),
                EndStatus9: self.EndStatus9(),
                StartStatus10: self.StartStatus10(),
                EndStatus10: self.EndStatus10()
            },
            "POST");
    }

    self.onEnter = function (d, e) {
        if (e.which == 13 || e.keyCode == 13) {
            self.Keyword($('#txtKeyword').val());
            self.search(1);
        }
        return true;
    };

    //Todo chi tiết đơn hàng ký gửi
    self.detailDeposit = function (data) {
        //self.templateId("detailDeposit");
        //self.listOrderService([]);
        //self.listProductDetail([]);
        //self.listOrderComment([]);
        //$.post("/" + window.culture + "/CMS/Order/DetailDeposit/",
        //    { orderId: data.Id },
        //    function (result) {
        //        if (result.model.DepositViewItem != null) {
        //            self.mapDepositDetail(result.model.DepositViewItem);
        //        }
        //        self.listOrderService(result.model.ListOrderService);
        //        self.listProductDetail(result.model.ListDetail);
        //        self.listOrderComment(result.model.ListOrderComment);
        //        window.location.href = "/" + window.culture + "/CMS/Order/DetailDeposit";
        //    });
        window.location.href = "/" + window.culture + "/CMS/Order/DetailDeposit?depositId=" + data.Id;
    }

    self.backToList = function () {
        self.GetAll();
        self.templateId("deposit");
    };
    //Map thông tin chi tiết đơn hàng ký gửi
    self.mapDepositDetail = function (data) {
        self.depositDetail(new depositDetailModel());
        self.depositDetail().Id(data.Id);
        self.depositDetail().Code(data.Code);
        self.depositDetail().CreateDate(data.CreateDate);
        self.depositDetail().UpdateDate(data.UpdateDate);
        self.depositDetail().CustomerName(data.CustomerName);
        self.depositDetail().CustomerEmail(data.CustomerEmail);
        self.depositDetail().CustomerPhone(data.CustomerPhone);
        self.depositDetail().CustomerAddress(data.CustomerAddress);
        self.depositDetail().Note(data.Note);
        self.depositDetail().WarehouseName(data.WarehouseName);

        self.depositDetail().ContactName(data.ContactName);
        self.depositDetail().ContactPhone(data.ContactPhone);
        self.depositDetail().ContactAddress(data.ContactAddress);
        self.depositDetail().ContactEmail(data.ContactEmail);
        self.depositDetail().ContactEmail(data.ContactEmail);

        self.depositDetail().Status(data.Status);
        self.depositDetail().TotalAdvance(data.TotalAdvance);
        self.depositDetail().ProvisionalMoney(data.ProvisionalMoney);
    }
    $(function () {
        self.GetAll();
    });
}
var depositPopupModel = new depositPopupModel();
var depositViewModel = new DepositViewModel(depositPopupModel);
depositPopupModel.parent = depositViewModel;
ko.applyBindings(depositViewModel);