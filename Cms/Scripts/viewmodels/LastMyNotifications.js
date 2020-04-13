var DateFormat = "DD/MM/YYYY";

amplify.request.define("GetLastNotificationsAndMessages", "ajax", {
    url: '/Home/GetLastNotificationsAndMessages',
    dataType: 'json',
    type: 'GET'
});

// Tìm kiếm user (My task)
amplify.request.define('SearchTaskByUserNotify', 'ajax', {
    url: '/Task/SearchByUserNotify',
    dataType: 'json',
    type: 'post'
});

// Tìm kiếm user (My task)
amplify.request.define('UpdateIsRead', 'ajax', {
    url: '/Home/NotifyUpdateIsRead',
    dataType: 'json',
    type: 'POST'
});

var LastMyNotificationsModel = function () {
    var self = this;

    var formatDateInList = function (list) {
        $.each(list, function (index, item) {
            item.FromNow = moment(item.SendTime).fromNow();
            var sendTime = moment(item.SendTime).format('DD/MM/YYYY hh:mm:ss a');
            item.SendTime = sendTime == 'Invalid date' ? '' : sendTime;
        });
    };

    var formatMessageInList = function (list) {
        $.each(list, function (index, item) {
            item.FromNow = ko.observable(moment(item.SendTime).fromNow());
            var sendTime = moment(item.SendTime).format('DD/MM/YYYY hh:mm:ss a');
            item.SendTime = sendTime == 'Invalid date' ? '' : sendTime;
            item.FullName = JSON.parse(item.FromUser).fullName;
        });
    };

    self.totalNotifySystemUnRead = ko.observable(0);

    self.totalMessageUnRead = ko.observable(0);

    self.TotalNotification = ko.observable('');

    self.listNotify = ko.observableArray([]);

    self.listMessage = ko.observableArray([]);

    self.detailNotify = function (item) {
        if (item.Url !== "" && item.Url != undefined) {
            $.NotifycationCenter.updateIsRead(item.Id);
            document.location = item.Url;
        } else {
            $.NotifycationCenter.showDetail({ mode: 1, id: item.Id });
        }
    };

    self.detailMessage = function (item) {
        //document.location = '/Message/DetailMessage/' + item.Id;
        $.NotifycationCenter.showDetail({ mode: 0, id: item.Id });
    };

    // @HenryDo: Show all notification in notification center
    self.ShowAllNotification = function (mode, data) {
        $.NotifycationCenter.show({ mode: mode });
    }

    self.ShowListUnReadNotify = function () {
        // Lấy về dánh sách thông báo chưa đọc

    };

    self.listTask = ko.observableArray([]);
    self.getMyTask = function () {
        var curentUser = window.currentUser;
        amplify.request("SearchTaskByUserNotify", {
            name: '',
            parrentId: 0,
            officeId: 0,
            userId: curentUser.UserId,
            startDate: '',
            endDate: '',
            orderby: 'startdate',
            orderType: 'asc'
        }, function (data) {
            if (data.items != undefined) {
                self.listTask(data.items);
            }
        });
    };

    //self.getMyTask();

    self.getLastNotificationsAndMessages = function () {
        amplify.request("GetLastNotificationsAndMessages", {
        }, function (result) {
            formatDateInList(result.notifications);

            formatMessageInList(result.messages);

            self.totalNotifySystemUnRead(result.totalRecordNotifySystemUnread);
            self.totalMessageUnRead(result.totalMessageUnRead);
            self.listNotify(result.notifications);
            self.listMessage(result.messages);
        });
    };
};

var lastNotificationsModelView = new LastMyNotificationsModel();

lastNotificationsModelView.getLastNotificationsAndMessages();

ko.applyBindings(lastNotificationsModelView, $('#notifications')[0]);