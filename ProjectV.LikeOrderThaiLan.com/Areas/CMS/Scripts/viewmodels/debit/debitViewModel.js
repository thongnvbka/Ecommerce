var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var debitViewModel = function () {
    var self = this;
    //Todo Khai báo ListData đổ dữ liệu danh sách order
    self.listAll = ko.observableArray([]);
    self.listAllDebitHistory = ko.observableArray([]);
    self.isRending = ko.observable(false);
    //==================== Search Object -
    self.Keyword = ko.observable("");
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    
    //Todo Lấy danh sách cong no
    self.GetAll = function () {
        self.listAll([]);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetAllDebitList",
        {
            PageIndex: page,
            PageSize: pagesize,
            Keyword: self.Keyword(),
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime()
        }, function (data) {
            total = data.Page.Total;

            _.each(data.ListItems, function (item) {
                item.ListDebitHistory = ko.observableArray([]);
            });
            self.listAll(data.ListItems);
            self.paging();
            self.isRending(true);
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
        self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) +
            "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) +
            "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
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
    //show more
    self.showChil = function (data) {
        $.post("/" + window.culture + "/CMS/AccountCMS/GetAllDebitHistoryList", { debitId: data.DebitId }, function (result) {
            data.ListDebitHistory(result.debitHistoryList);
        });
        $('.chil' + data.DebitId).toggle();
    }
    self.onEnter = function (d, e) {
        if (e.which == 13 || e.keyCode == 13) {
            self.Keyword($('#txtKeyword').val());
            self.search(1);
        }
        return true;
    };
    $(function () {
        self.GetAll();
        $("#effect-7 .img").hover(function () {
            $("#effect-7 .img .overlay").css("width", "100%");
        }
       , function () {
           $("#effect-7 .img .overlay").css("width", "0");
       });
    });
}

ko.applyBindings(new debitViewModel());