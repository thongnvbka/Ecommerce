var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var WalletViewModel = function () {
    var self = this;
    self.listWallet = ko.observableArray([]);
    self.listAll = ko.observableArray([]);
    self.Keyword = ko.observable("");
    self.Status = ko.observable(-1);
    self.StartDate = ko.observable("");
    self.WalletIds = ko.observable("-1");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);

    self.active = ko.observable('');
    self.templateId = ko.observable("templateRecharge");
    self.AdvanceMoney = ko.observable("");
    self.data = ko.observable("");
    self.isRending = ko.observable(false);
    self.isRendingPage = ko.observable(false);
    self.Keyword = ko.observable("");
    self.ShowMessagerDeposit = function () {
        var error = 0;
        if ($("#AdvanceMoney").val().trim().length === 0) {
            toastr.error(window.messager.wallet.errorNullMoneyTotal);
            error = 1;
        }
        if ($("#CardName").val().trim().length === "0") {
            toastr.error(window.messager.wallet.errorNullAcountBank);
            error = 1;
        }
        if ($("#CardId").val().trim().length === 0) {
            toastr.error(window.messager.wallet.errorNullNumberAcountBank);
            error = 1;
        }
        if ($("#CardBank").val().trim().length === "0") {
            toastr.error(window.messager.wallet.errorNullBankName);
            error = 1;
        }
        if ($("#CardBranch").val().trim().length === "0") {
            toastr.error(window.messager.wallet.errorNullBranchBank);
            error = 1;
        }
        if (error === 1) {
            return false;
        } else {
            $("#dialog_deposit_ok").modal('show');
            return true;
        }
    }
    self.initInputMark = function () {
        $("input.decimal").each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", {
                    radixPoint: Globalize.culture().numberFormat["."],
                    autoGroup: true, groupSeparator: Globalize.culture().numberFormat[","],
                    digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false
                });
            }


            //if (!$(this).data()._inputmask) {
            //    $(this).inputmask("decimal", {
            //        //radixPoint: Globalize.culture().numberFormat['.'],
            //        //autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','],
            //        //digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false
            //        rightAlign: true,
            //        'groupSeparator': '.',
            //        'autoGroup': true
            //    });
            //}
        });
    }

    //todo lay ra danh sach yeu cau rut tien
    self.GetClaim = function () {
        self.templateId("templateClaim");
        self.listData([]);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListDraw", {
            page: page, pageSize: pagesize
        }, function (data) {
            total = data.totalRecord;
            self.listData(data.listItems);
            self.paging();
            self.isRending(true);
            self.isRendingPage(true);
        });

        self.initInputMark();
        $('#btnCommand').click(function () {
            self.ShowMessagerDeposit();
        });
    };

    //Todo Khai báo ListData đổ dữ liệu danh sách
    self.listRecharge = ko.observableArray([]);
    self.listData = ko.observableArray([]);
    self.totalRecharge = ko.observable();//TỔNG SỐ

    //Todo Lấy danh sách notification
    self.GetListRecharge = function () {
        self.listRecharge([]);
        self.isRendingPage(false);
        self.templateId("templateRecharge");
        var obj = {
            PageIndex: page,
            PageSize: pagesize,
            Keyword: self.Keyword(),
            Status: self.Status(),
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime(),
            WalletIds: self.WalletIds()
        };

        $.post("/" + window.culture + "/CMS/Wallet/GetListRecharge", obj, function (data) {
            total = data.Page.Total;
            self.listAll(data.ListItems);
            self.listWallet(data.Wallets);
            self.paging();
            self.isRending(true);
            self.isRendingPage(true);
        });
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
        self.GetListRecharge();
    };
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
        self.GetListRecharge();
    }
    
    self.SetDate = function () {
        self.StartSearch($("#date_timepicker_start").val());
        self.FinishSearch($("#date_timepicker_end").val());
        var startDate = $("#date_timepicker_start").val();
        if (startDate.length > 0) {
            var arrStart = startDate.split('/');
            if (arrStart.length === 3) {
                startDate = arrStart[1] + "/" + arrStart[0] + "/" + arrStart[2] + " 00:00";
                self.StartDate(startDate);
            }
        }
        var finishDate = $("#date_timepicker_end").val();
        if (finishDate.length > 0) {
            var arrFinish = finishDate.split('/');
            if (arrFinish.length === 3) {
                finishDate = arrFinish[1] + "/" + arrFinish[0] + "/" + arrFinish[2] + " 23:00";
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
    //Todo /tìm kiếm
    self.search = function (page) {
        page = 1;
        if (self.templateId() === 'templateRecharge') {
            self.GetListRecharge();
        }

        if (self.templateId() === 'templateClaim') {
            self.listData([]);
            //self.isRending(false);        
            self.isRendingPage(false);
            $.post("/" + window.culture + "/CMS/AccountCMS/GetListDraw", {
                page: page, pageSize: pagesize
            }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    self.listData(response.listItems);
                    self.paging();
                    total = response.totalRecord;
                    self.isRending(true);
                    self.isRendingPage(true);
                }
            });
        }
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
        //self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) + "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    $(function () {
        self.initInputMark();
        self.GetListRecharge();
        self.GetInit();
    });

    self.GetInit = function () {
        $.post("/" + window.culture + "/CMS/AccountCMS/GetInit", function (data) {
            self.totalRecharge(data.totalRecharge);
        });
    };

    //mapobject
    //Todo khai bao bien model show du lieu tren view cua bang notification
    self.claimModel = ko.observable(new drawItemModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================
    //Todo Object chi tiết notificommon
    self.mapClaimModel = function (data) {
        self.claimModel(new drawItemModel());

        self.claimModel().Id(data.Id);
        self.claimModel().CardName(data.CardName);
        self.claimModel().CardId(data.CardId);
        self.claimModel().CardBank(data.CardBank);
        self.claimModel().CardBranch(data.CardBranch);
        self.claimModel().CreateDate(data.CreateDate);
        self.claimModel().Status(data.Status);
        self.claimModel().Note(data.Note);
        self.claimModel().AdvanceMoney(data.AdvanceMoney);
    };
    self.onEnter = function (d, e) {
        if (e.which === 13 || e.keyCode === 13) {
            self.Keyword($("#txtKeyword").val());
            self.search(1);
        }
        return true;
    };
    //export
    self.ExcelReport = function () {
        $.redirect("/" + window.culture + "/CMS/Wallet/ExportRecharge", 
        {
            PageIndex: 1,
            PageSize: 1000000000,
            Keyword: self.Keyword(),
            Status: self.Status(),
            StartDate: self.StartDate(),
            FinishDate: self.FinishDate(),
            AllTime: self.AllTime(),
            WalletIds: self.WalletIds()
        }
        , "POST");
    }
}

ko.applyBindings(new WalletViewModel());