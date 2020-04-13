var notificationsModel = function () { };

amplify.request.define("GetMyNotifications", "ajax", {
    url: '/Home/GetMyNotifications',
    dataType: 'json',
    type: 'GET'
});

// Edited by @HenryDo: Add Keyword parameter
notificationsModel.getMyNotifications = function (keyword, isRead, type, currentPage, recordPerPage, callback) {
    amplify.request("GetMyNotifications", {
        keyword: keyword,
        isRead: isRead,
        type: type,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

amplify.request.define("GetNotifyDetail", "ajax", {
    url: '/Home/NotifyDetail',
    dataType: 'json',
    type: 'GET'
});

notificationsModel.getNotifyDetail = function (id, callback) {
    amplify.request("GetNotifyDetail", {
        id: id
    }, function (data) {
        if (callback) callback(data);
    });
}; 


amplify.request.define("MarkReadedNotify", "ajax", {
    url: '/Home/NotifyMarkReaded',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

notificationsModel.markReaded = function (ids, isRead, token, callback) {
    amplify.request("MarkReadedNotify", {
        ids: ids,
        isRead: isRead,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};