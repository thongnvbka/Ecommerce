var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var NotifiViewModel = function () {
    var self = this;
    //Todo Khai báo ListData đổ dữ liệu danh sách
    self.listAllNotification = ko.observableArray([]);
    self.isRending = ko.observable(false);

    self.isRendingPage = ko.observable(false);

    //Todo khai báo template
    self.active = ko.observable('notification');
    self.templateId = ko.observable("notification");
    self.data = ko.observable();
    self.titleNotification = ko.observable("");
    self.totalNotifi = ko.observable("");
    self.totalNotifiNoRead = ko.observable("");
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    //Todo Lấy danh sách notification
    self.GetAllNotification = function () {
        self.listAllNotification([]);
        self.GetInit();
        self.isRending(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListNotification", {
            page: page,
            pageSize: pagesize,
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime()
        }, function (data) {
            total = data.totalRecord;
            self.listAllNotification(data.notiCustomer);
            self.paging();
            self.isRending(true);
        });
    }

    self.GetInit = function () {
        self.isRendingPage(true);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetInit", function (data) {
            self.totalNotifi(data.totalNotification);
            self.totalNotifiNoRead(data.totalNotificationNoRead);
        });
    };

    //Todo xoa du lieu notificommon
    self.removeNotifi = function (data) {
        swal({
            title: 'Bạn chắc chắn muốn xóa bản ghi đã chọn xóa?',
            type: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Không',
            confirmButtonText: 'Xóa'
        }).then(function () {
            $.post("/" + window.culture + "/CMS/AccountCMS/DeleteNotitfiCation", { notiId: data.Id }, function () {
                self.GetAllNotification();
                toastr.success("Xóa thành công");
            });
        }, function () {
            toastr.warning("Thông báo hệ thống không tồn tại hoặc đã bị xóa");
        });
    };
    //Todo chi tiết khiếu nại
    self.detailNotification = function (data) {
        window.location.href = "/" + window.culture + "/CMS/AccountCMS/DetailNotitication?notifiId=" + data.Id;
    }
    ////Todo chi tiết thông báo
    //self.detailNotification = function (data) {
    //    self.titleNotification("Thông báo");
    //    self.templateId("detailNotification");
    //    self.mapnotificationModel(data);
    //    self.data(data);
    //    self.isRending(false);
    //    $.post("/" + window.culture + "/CMS/AccountCMS/DetailNotitication/",
    //        { notificationId: data.Id },
    //        function (result) {
    //            self.mapnotificationModel(result.notification);
    //            self.isRending(true);
    //        });
    //}
    //self.backToList = function () {
    //    self.GetAllNotification();
    //    self.templateId("notification");
    //};

    //Todo khai bao bien model show du lieu tren view cua bang notification
    self.notificationModel = ko.observable(new NotifiModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================
    //Todo Object chi tiết notificommon
    self.mapnotificationModel = function (data) {
        self.notificationModel(new NotifiModel());

        self.notificationModel().Id(data.Id);
        self.notificationModel().SystemId(data.SystemId);
        self.notificationModel().SystemName(data.SystemName);
        self.notificationModel().Description(data.Description);
        self.notificationModel().CreateDate(data.CreateDate);
        self.notificationModel().UpdateDate(data.UpdateDate);
        self.notificationModel().OrderId(data.OrderId);
        self.notificationModel().OrderType(data.OrderType);
        self.notificationModel().CustomerId(data.CustomerId);
        self.notificationModel().CustomerName(data.CustomerName);
        self.notificationModel().IsRead(data.IsRead);
        self.notificationModel().Title(data.Title);
        self.notificationModel().UserId(data.UserId);
        self.notificationModel().UserName(data.UserName);
    };

    //Todo==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    //Todo /tìm kiếm
    self.search = function (page) {
        self.listAllNotification([]); 
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListNotification", {
            page: page, pageSize: pagesize,
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime()
        }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                self.listAllNotification(response.notiCustomer);
                self.paging();
                total = response.totalRecord;
                self.isRendingPage(true); 
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
        self.pageTitle(window.messager.pageList.show + " <b>" + (((page - 1) * pagesize) + 1) + "</b> " + window.messager.pageList.to + " <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> " + window.messager.pageList.of + " <b>" + total + "</b> " + window.messager.pageList.record);
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    self.clickMenu = function (name) {
        page = 1;
        self.active(name);
        self.templateId("notification");
        if (name === 'notificationCommon') {
            self.GetNotificationCommon();
        };
        if (name === 'notification') {
            self.GetAllNotification();
        };
    }

    self.GetNotificationCommon = function () {
        self.listAllNotification([]);
        self.GetInit();
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListNotificationCommon", {
            page: page, pageSize: pagesize
        }, function (data) {
            total = data.totalRecord;
            self.listAllNotification(data.notiCustomer);
            self.paging();
        });
    }
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
        self.search(1);
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
        self.search(1);
    }
    self.FinishSearch.subscribe(function () {
        self.SetDate();
    });
    self.StartSearch.subscribe(function () {
        self.SetDate();
    });
    $(function () {
        self.GetAllNotification();
        self.GetInit();
    });
}

ko.applyBindings(new NotifiViewModel());