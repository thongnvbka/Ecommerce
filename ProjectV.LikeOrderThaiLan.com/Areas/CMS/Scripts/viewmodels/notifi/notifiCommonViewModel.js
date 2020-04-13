var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var NotifiCommonViewModel = function () {
    var self = this;
    //Todo Khai báo ListData đổ dữ liệu danh sách
    self.listAllNotification = ko.observableArray([]); 
    self.isRending = ko.observable(false);
    self.isRendingPage = ko.observable(false);

    //Todo khai báo template
    self.templateId = ko.observable("notificationCommon");
    self.data = ko.observable();
    self.titleNotification = ko.observable("");
    self.totalNotifi = ko.observable("");
    self.totalNotifiNoRead = ko.observable("");
    //self.totalNotifi = ko.observable("");
    self.totalNoRead = ko.observable("");
    self.totalRead = ko.observable("");
    self.GetInit = function () {
        //self.isRendingPage(false);
        self.isRendingPage(true);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetInitCommon", function (data) {
            self.totalNotifi(data.totalNotificationCommon);
            self.totalNotifiNoRead(data.totalNotificationCommonNoRead);
        });
    };

    //Todo xoa du lieu notificommon
    self.removeNotifi = function (data) {
        swal({
            title: 'Do you want to deleted ?',
            type: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/" + window.culture + "/CMS/AccountCMS/DeleteNotificationCommon", { notiId: data.Id }, function () {
                self.GetNotificationCommon();
                toastr.success("Deleted successfully");
            });
        }, function () {
            toastr.warning("Thông báo hệ thống không tồn tại hoặc đã bị xóa");
        });
    };

    //Todo chi tiết thông báo
    self.detailNotification = function (data) {
        self.titleNotification("ประกาศ");
        self.templateId("detailNotificationCommon");
        self.mapnotificationModel(data);
        self.data(data);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/DetailNotificationCommon/",
            { notificationId: data.Id },
            function (result) {
                self.mapnotificationModel(result.notification);
                self.isRendingPage(true);
            });
    }
    self.backToList = function () {
        self.GetNotificationCommon();
        self.templateId("notificationCommon");
    };

    //Todo khai bao bien model show du lieu tren view cua bang notification
    self.notificationModel = ko.observable(new NotifiCommonModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================
    //Todo Object chi tiết notificommon
    self.mapnotificationModel = function (data) {
        self.notificationModel(new NotifiCommonModel());

        self.notificationModel().Id(data.Id);
        self.notificationModel().SystemId(data.SystemId);
        self.notificationModel().SystemName(data.SystemName);
        self.notificationModel().Description(data.Description);
        self.notificationModel().CreateDate(data.CreateDate);
        self.notificationModel().UpdateDate(data.UpdateDate);
        self.notificationModel().IsRead(data.IsRead);
        self.notificationModel().Title(data.Title);
        self.notificationModel().UserId(data.UserId);
        self.notificationModel().UserName(data.UserName);
        self.notificationModel().Url(data.Url);
        self.notificationModel().Status(data.Status);
        self.notificationModel().PublishDate(data.PublishDate);
        self.notificationModel().ImagePath(data.ImagePath);
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
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListNotificationCommon", {
            page: page, pageSize: pagesize
        }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                self.listAllNotification(response.notification);
                self.paging(); 
                //self.totalNoti(response.totalRecord);
                self.totalRead(response.totalRead);
                self.totalNoRead(response.totalNoRead);
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
        //self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) + "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    } 

    self.GetNotificationCommon = function () {
        self.listAllNotification([]);
        self.GetInit();
        self.isRending(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/GetListNotificationCommon", {
            page: page, pageSize: pagesize
        }, function (data) {
            total = data.totalRecord;
            self.listAllNotification(data.notification);
            //self.totalNoti(data.totalNoti);
            self.totalRead(data.totalRead);
            self.totalNoRead(data.totalNoRead);
            self.paging();
            self.isRending(true);
        });
    }

    $(function () {
        self.GetNotificationCommon();
        self.GetInit();
    });
}

ko.applyBindings(new NotifiCommonViewModel());