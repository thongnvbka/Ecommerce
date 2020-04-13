$(function () {
    var isChrome = true;
    // request permission on page load
    document.addEventListener('DOMContentLoaded', function () {
        if (!Notification) {
            isChrome = false;
            return;
        }

        if (Notification.permission !== "granted")
            Notification.requestPermission();
    });

    function notifyChrome(title, body) {
        if (Notification.permission !== "granted")
            Notification.requestPermission();
        else {
            var notification = new Notification(title,
            {
                body: body
            });

            //notification.onclick = function () {
            //    window.open("http://stackoverflow.com/a/13328397/1269037");
            //};
        }
    }

    $.connection.notifyHub.client.notifySystem = function (notifyId, title, type, sendTime) {
        if (lastNotificationsModelView.listNotify().length === 20) {
            lastNotificationsModelView.listNotify.pop();
        }
        lastNotificationsModelView.listNotify.unshift({
            ID: notifyId,
            Title: title,
            Type: type,
            SendTime: sendTime,
            FromNow: 'Vừa xong',
            IsRead: false
        });
        lastNotificationsModelView.totalNotifySystemUnRead(lastNotificationsModelView.totalNotifySystemUnRead() + 1);
        if (isChrome) {
            notifyChrome("Notify", title);
        } else {
            toastr.info(title, 'Notify', {
                positionClass: "toast-bottom-left", onclick: function () {
                    document.location = '/Home/NotifyDetail/' + notifyId;
                }
            });

            ion.sound({
                sounds: [
                    { name: "water_droplet_2" }
                ],

                // main config
                path: "/Content/plugins/soundPlugin/sound/",
                preload: true,
                multiplay: true,
                volume: 1
            });

            ion.sound.play("water_droplet_2");
        }

        // Nếu đang ở trang thông báo của tôi
        if (typeof notifyViewModel !== 'undefined') {
            notifyViewModel.Message().search(1);
        }
    };

    $.connection.notifyHub.client.notifyMessage = function (notifyId, fullName, title, avatar, sendTime) {
        if (lastNotificationsModelView.listMessage().length === 20) {
            lastNotificationsModelView.listMessage.pop();
        }
        lastNotificationsModelView.listMessage.unshift({
            ID: notifyId,
            FullName: fullName,
            FromUser: '{}',
            Title: title,
            FromAvatar: avatar,
            SendTime: sendTime,
            FromNow: ko.observable('Already done')
        });
        lastNotificationsModelView.totalMessageUnRead(lastNotificationsModelView.totalMessageUnRead() + 1);
        if (isChrome) {
            notifyChrome("Message", title);
        } else {
            toastr.info(title, 'Message', {
                positionClass: "toast-bottom-left", onclick: function () {
                    // Mở trang Detail tin nhắn
                    document.location = '/Message/DetailMessage/' + notifyId;
                }
            });
        }
        // Nếu đang ở trang tin nhắn đến thì refresh lại trang
        if (typeof notifyViewModel !== 'undefined') {
            if (notifyViewModel.Message().listType() === 'inbox') {
                notifyViewModel.Message().totalInboxUnread(notifyViewModel.Message().totalInboxUnread() + 1);
                notifyViewModel.Message().search(1);
            }
        }
    };

    $.connection.notifyHub.client.updateTotalNotifySystemUnread = function (notifyIdReaded) {
        lastNotificationsModelView.totalNotifySystemUnRead(lastNotificationsModelView.totalNotifySystemUnRead() - 1);
        lastNotificationsModelView.listNotify.remove(function (item) { return item.Id === notifyIdReaded });

        // Nếu đang ở trang thông báo của tôi
        if (typeof notifyViewModel !== 'undefined') {
            var list = notifyViewModel.MyNotification().listNotify();
            for (var i = 0; i < list.length; i++) {
                if (list[i].Id !== notifyIdReaded) {
                    continue;
                }
                list[i].IsRead(true);
                break;
            }
        }
    };

    $.connection.notifyHub.client.updateTotalMessageUnread = function (notifyIdReaded) {
        lastNotificationsModelView.totalMessageUnRead(lastNotificationsModelView.totalMessageUnRead() - 1);
        lastNotificationsModelView.listMessage.remove(function (item) { return item.Id === notifyIdReaded });
        // Nếu đang ở trang tin nhắn của tôi
    };

    $.connection.notifyHub.client.WarningWhenEndSessionLogin = function () {
        alert('Your session has ended because your account has been signed in somewhere else. \n\n Do you want to login again?');
        document.location = '/Account/Logout';
    };

    $.connection.hub.start().done(function () { });
});
