var DetailNotifiViewModel = function () {
    var self = this;
    self.data = ko.observable();
    self.titleNotification = ko.observable("");
    self.totalNotifi = ko.observable("");
    self.totalNotifiNoRead = ko.observable("");
    self.isRending = ko.observable(false);

    $(function () {
        var arr = _.split(window.location.href, "notifiId=");
        if (arr.length > 1) {
            self.detailNotification(arr[1]);
        }
    });

    //Todo chi tiết thông báo
    self.detailNotification = function (id) {
        self.titleNotification("Thông báo");
        //self.mapnotificationModel(data);
        //self.data(data);
        self.isRending(false);
        $.post("/" + window.culture + "/CMS/AccountCMS/DetailNotitication/",
            { notificationId:id },
            function (result) {
                self.mapnotificationModel(result.notification);
                self.isRending(true);
            });
    }
    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/AccountCMS/Notification";
    };

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
}
ko.applyBindings(new DetailNotifiViewModel());