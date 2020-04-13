var total = 0;
var page = 1;
var pagesize =10;
var pageTotal = 0;

var DrawViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    //Todo Khai báo ListData đổ dữ liệu danh sách
    self.listRecharge= ko.observableArray([]);
    self.totalRecharge = ko.observable();//TỔNG SỐ

    //Todo Lấy danh sách notification
    self.GetListRecharge = function () {
        self.listRecharge([]);

        $.post("/"+ window.culture+"/CMS/AccountCMS/GetListRecharge", {
            page: page, pageSize: pagesize
        }, function (data) {
            total = data.totalRecord;
            self.listRecharge(data.rechargeModal);
            self.paging();
        });
    }
    
    //Todo==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    //Todo /tìm kiếm
    self.search = function (page) {
        self.listRecharge([]);

        $.post("/"+ window.culture+"/CMS/AccountCMS/GetListRecharge", {
            page: page, pageSize: pagesize
        }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else { 
                self.listRecharge(response.rechargeModal);
                self.paging();
                total = response.totalRecord;
            }
        });
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
        self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) + "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    $(function () {
        self.GetListRecharge();
        self.GetInit();
    });


    self.GetInit = function () {
        $.post("/"+ window.culture+"/CMS/AccountCMS/GetInit", function (data) {
            self.totalRecharge(data.totalRecharge);
        });
    };
}

ko.applyBindings(new DrawViewModel());