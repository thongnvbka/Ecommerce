var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var packageViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    //Todo Khai báo ListData đổ dữ liệu danh sách order
    self.listAll = ko.observableArray([]);
    self.isRending = ko.observable(false);
    self.isRendingPage = ko.observable(false);
    //==================== Search Object -
    self.Keyword = ko.observable("");
    self.Status = ko.observable(-1);
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    self.StartStatus = ko.observable(0);
    self.EndStatus = ko.observable(13);
    self.StartStatus1 = ko.observable(0);
    self.EndStatus1 = ko.observable(0);
    self.StartStatus2 = ko.observable(1);
    self.EndStatus2 = ko.observable(1);
    self.StartStatus3 = ko.observable(2);
    self.EndStatus3 = ko.observable(2);
    self.StartStatus4 = ko.observable(3);
    self.EndStatus4 = ko.observable(3);
    self.StartStatus5 = ko.observable(4);
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

    self.StartStatus11 = ko.observable(11);
    self.EndStatus11 = ko.observable(13);
    //Todo khai bao bien model show du lieu tren view cua bang status
    self.orderPackageStatusItemModel = ko.observable(new orderPackageStatusItemModel());
    //Todo Object chi tiết status
    self.mapOrderPackageStatusItemModel = function (data) {
        self.orderPackageStatusItemModel(new orderPackageStatusItemModel());
        self.orderPackageStatusItemModel().shopTqPhatHang(data.shopTqPhatHang);
        self.orderPackageStatusItemModel().khoTqNhanHang(data.khoTqNhanHang);
        self.orderPackageStatusItemModel().trongKhoTq(data.trongKhoTq);
        self.orderPackageStatusItemModel().xuatKhoTq(data.xuatKhoTq);
        self.orderPackageStatusItemModel().trenDuongVe(data.trenDuongVe);
        self.orderPackageStatusItemModel().trongKhoVn(data.trongKhoVn);

        self.orderPackageStatusItemModel().dangVanChuyen(data.dangVanChuyen);
        self.orderPackageStatusItemModel().choGiaoHang(data.choGiaoHang);

        self.orderPackageStatusItemModel().dangGiaoHang(data.dangGiaoHang);
        self.orderPackageStatusItemModel().daTraHang(data.daTraHang);
        self.orderPackageStatusItemModel().hangBiMat(data.hangBiMat);
    };
    //Todo Lấy danh sách order package
    self.GetAll = function () {
        self.listAll([]);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/Transport/GetAllOrderPackage",
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
            EndStatus10: self.EndStatus10(),
            StartStatus11: self.StartStatus11(),
            EndStatus11: self.EndStatus11(),
        }, function (data) {
            total = data.Page.Total;
            self.listAll(data.ListItems);
            self.mapOrderPackageStatusItemModel(data.OrderStatusItem);
            self.paging();
            self.isRending(true);
            self.isRendingPage(true);
            self.GetWallet();
            $('[data-toggle="popover"]').popover({
                placement: 'right'
            });
        });
    };
    self.GetWallet = function () {
        $(".table-product .code-package").each(function () {
            var objs = $(this);
            var id = $(objs).attr('idata');
            $.ajax({
                url: "/" + window.culture + "/CMS/Transport/SetTextWallet",
                type: 'Get',
                data: { 'packageId': parseInt(id) },
                success: function (result) {
                    console.log(result);
                    $(objs).text($(objs).text() + result);
                },
                asyc: false
            });
        });
        
    }
    self.ExcelReport = function () {
        $.redirect("/" + window.culture + "/CMS/Transport/ExportPackage",
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
                EndStatus10: self.EndStatus10(),
                StartStatus11: self.StartStatus11(),
                EndStatus11: self.EndStatus11(),
            },
            "POST");
    }
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
            if (arrStart.length === 3) {
                startDate = arrStart[1] + '/' + arrStart[0] + '/' + arrStart[2] + ' 00:00';
                self.StartDate(startDate);
            }
        }
        var finishDate = $('#date_timepicker_end').val();
        if (finishDate.length > 0) {
            var arrFinish = finishDate.split('/');
            if (arrFinish.length === 3) {
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
    self.returnAtt = function (start, end, start1, end1) {
        if (start === start1 && end === end1) {
            return "outOfStock";
        }
        else {
            return "";
        }
    }
    // Search status
    self.searchKeyword = function () {
        page = 1;
        self.search(1);
    }

    self.onEnter = function (d, e) {
        if (e.which === 13 || e.keyCode === 13) {
            self.Keyword($('#txtKeyword').val());
            self.search(1);
        }
        return true;
    };

    $(function () {
        self.GetAll();
    });
}

ko.applyBindings(new packageViewModel(), $('#tab-package')[0]);