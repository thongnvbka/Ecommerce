var footerHeight = 34;
var miniSidebar = 69;
var sidebarAction = 30;
var mainTitle = 19;
var pager = 39;
var messageOptionBottom = 34;

(function ($) {
    $.NotifycationCenter = {
        defaults: {
            mode: 0,
            id: null,
            send: {
                title: "",
                content: "",
                to: [],
                cc: [],
                bcc: []
            },
            sendOut: false
        },
        show: function (o) {
            var obj = $.extend({}, this.defaults, o || {});

            if (obj.mode === 0) {
                notifyViewModel.MessageSelectPage("inbox");
            }

            notifyViewModel.IsBack(true);
            notifyViewModel.Show(obj.mode);
        },
        showDetail: function (o) {
            var obj = $.extend({}, this.defaults, o || {});

            if (obj.id != null) {
                obj.mode === 0 ? notifyViewModel.Message().getDetail(obj.id) : notifyViewModel.NotifyDetail(obj.id);
            }

            notifyViewModel.IsBack(true);
            notifyViewModel.Show(obj.mode);
        },
        send: function (o) {
            var obj = $.extend({}, this.defaults, o || {});

            if (obj.send.to.length === 0) {
                toastr.error("Recipient list can not be empty");
                return;
            }

            if (obj.send.title === "") {
                toastr.error("Can not leave the title blank.");
                return;
            }

            if (obj.send.content === "") {
                toastr.error("Content can not be empty.");
                return;
            }

            if (obj.sendOut)
                notifyViewModel.Message().IsSendOut(true);

            notifyViewModel.Message().quickCompose();

            notifyViewModel.Message().subject(obj.send.title);
            notifyViewModel.Message().cc(obj.send.cc != undefined ? obj.send.cc.join() : "");
            notifyViewModel.Message().bcc(obj.send.bcc != undefined ? obj.send.bcc.join() : "");
            notifyViewModel.Message().isShowCompose(true);
            notifyViewModel.Message().isCreateNewMessage(true);

            setTimeout(function () {
                notifyViewModel.calculateComposeContent(obj.send.content);
            }, 400);

            notifyViewModel.Message().RebindTagEditor("to", obj.send.to);
            notifyViewModel.Message().to(obj.send.to.join());

            if (obj.send.bcc && obj.send.bcc.length > 0) {
                notifyViewModel.Message().isShowBcc(true);
                notifyViewModel.Message().RebindTagEditor("bcc", obj.send.bcc);
                notifyViewModel.Message().bcc(obj.send.bcc.join());
            }

            if (obj.send.cc && obj.send.cc.length > 0) {
                notifyViewModeMessage().isShowCc(true);
                notifyViewModel.Message().RebindTagEditor("cc", obj.send.cc);
                notifyViewModel.Message().cc(obj.send.cc.join());
            }

            notifyViewModel.IsBack(true);
            notifyViewModel.CurrentMessagePage('create');
            notifyViewModel.Show(obj.mode);
        },
        updateIsRead: function (id) {
            notifyViewModel.MyNotification().UpdateIsRead(id);
        }
    }
})(jQuery);

function NotificationViewModel() {
    var self = this;

    self.CurrentMode = ko.observable(0); // 0 Là thư - 1 là thông báo
    self.IsShowSidebar = ko.observable(false);
    self.NotificationQuantity = ko.observable(0);
    self.Title = ko.observable("Hộp thư");
    self.MyNotification = ko.observable(new MyNotificationsModel());
    self.Message = ko.observable(new MyMessageModel());
    self.CurrentMessagePage = ko.observable('inbox'); // create: Soạn Thư - inbox:  Inbox - star: Thư gắn sao - draft: Thư nháp - sent: Thư đã gửi - trash: Thùng rác

    self.CurrentNotification = ko.observable({});
    self.CurrentMessage = ko.observable({});
    self.IsMobile = ko.observable(false);
    self.IsBack = ko.observable(true);
    self.messageMinHeight = ko.observable(400);
    self.notifyMinHeight = ko.observable(400);

    self.ChangeMode = function (mode, data) {
        self.CurrentMode(mode);

        if (self.CurrentMode() === 0) {
            self.Message().listType('inbox');
            self.Message().search(1);
        }

        mode === 0 ? self.Title("Mailbox") : self.Title("Notify");
    }

    self.ShowNotificationDetail = function (data) {
        if (!data.IsRead()) {
            data.IsRead(true);
            self.NotificationQuantity(self.NotificationQuantity() - 1);
            self.MyNotification().UpdateIsRead(data.Id);
        }

        self.IsBack(!self.IsBack());
        self.CurrentNotification(data);
    }

    self.DiscardMessage = function () {
        self.Message().discard();
        //self.CurrentMessagePage("inbox");
    }

    self.ChangeCheckAllMessage = function () {
        self.Message().checkAllMessage(!self.Message().checkAllMessage());
    };

    self.ChangeCheckAllNotification = function () {
        self.MyNotification().checkAllNotification(!self.MyNotification().checkAllNotification());
    }

    self.NotifyDetail = function (id) {
        var notify = _.find(self.MyNotification().listNotify(), function (nt) {
            return nt.Id === id;
        });

        if (notify != undefined) {
            self.CurrentNotification(notify);
            if (!notify.IsRead()) {
                notify.IsRead(true);
                notifyViewModel.MyNotification().UpdateIsRead(notify.Id);
            }
        }
    }

    self.ChangeShowCc = function () {
        self.Message().changeShowCc();
    }

    self.ChangeShowBcc = function () {
        self.Message().changeShowBcc();
    }

    self.MessageSelectPage = function (page, data) {
        self.CurrentMessagePage(page);

        if (page != 'create') {
            self.Message().ShowMessageContent(false);
            self.Message().listType(page);
            self.Message().search(1);
        } else {
            if (self.IsMobile()) {
                self.IsBack(!self.IsBack());
            }

            setTimeout(function () {
                self.calculateComposeContent();
            }, 200);

            self.Message().listMessage([]);
            self.Message().isShowCompose(true);
            self.Message().ShowMessageContent(false);
            self.Message().compose();
        }
    }

    self.ShowSidebar = function () {
        self.IsShowSidebar(!self.IsShowSidebar());
    }

    self.Show = function (mode) {
        self.CurrentMode(mode);
        $("#NotificationCenter").modal("show");

        $.get("/Home/GetMyNotifications", {
            keyword: "",
            isRead: false,
            type: null
        }, function (result) {
            _.each(result.items, function (item) {
                item.IsRead = ko.observable(item.IsRead);
            });

            self.MyNotification().currentFilter("all");
            self.MyNotification().search(1);
        });
    }

    self.DeleteMessage = function (check, data) {
        if (self.IsMobile() && !self.IsBack())
            self.IsBack(!self.IsBack());

        self.Message().checkedMessageId([]);
        self.Message().checkedMessageId.push(data.Id);
        self.Message().deleteMessage(check);
    }

    self.Back = function () {
        self.IsBack(true);
        if (self.CurrentMode() === 0 && self.CurrentMessagePage() != 'create') self.Message().search(1);
    }

    self.Detail = function (item) {
        if (self.IsMobile()) {
            self.IsBack(!self.IsBack());
        }

        self.CurrentMessagePage('inbox');
        self.Message().detail(item);

        setTimeout(function () {
            $("#content").css("height", $("#sidebar").height());
            $(".wrapper-message").css("height", $("#content").height() - 75);
        }, 50);
    }

    self.ComposeContinue = function (item) {
        if (self.IsMobile()) {
            self.IsBack(!self.IsBack());
        }

        self.Message().composeContinue(item);
    }

    $(function () {
        var messageHeight = $(window).innerHeight() - ($(".modal-footer").height() + $(".mini-sidebar").height());

        self.messageMinHeight(messageHeight);

        if (notifications != undefined) {
            self.NotificationQuantity(notifications.totalNotification);
        }

        $("#NotificationCenter").on('hidden', function () {
            //window.onbeforeunload = null;
            self.Message().isShowCompose(false);
            if (CKEDITOR.instances.messageContent != undefined) {
                CKEDITOR.instances.messageContent.destroy();
            }

            self.Message().cc([]);
            self.Message().to([]);
            self.Message().bcc([]);
        });

        $(window).resize(function () {
            $(this).width() < 768 ? self.IsMobile(true) : self.IsMobile(false);
        });

        $(window).width() < 768 ? self.IsMobile(true) : self.IsMobile(false);

        // Tính độ cao nội dung của notification center
        $("#NotificationCenter").on('show', function () {
            var windowHeight = $(window).innerHeight();
            var listMessageHeight = windowHeight - (footerHeight + miniSidebar + sidebarAction + mainTitle + 80 + pager); // 80 phần thừa trên vào dưới            
            $("#NotificationCenter .wrapper-list-message").css({ "height": listMessageHeight });
        });
    });

    self.calculateComposeContent = function (content) {
        var toHeight = $(".group-to").height();
        var ccHeight = $(".group-cc").height();
        var bccHeight = $(".group-bcc").height();
        var subjectHeight = $(".group-subject").height();
        var windowHeight = $(window).innerHeight();

        if (ccHeight == 99)
            ccHeight = 0;

        if (bccHeight == 99)
            bccHeight = 0;

        var contentHeight = windowHeight - (footerHeight + messageOptionBottom + toHeight + ccHeight + bccHeight + subjectHeight + 170);

        var editor = CKEDITOR.instances.messageContent;


        setTimeout(function () {
            if (editor == undefined) {
                if (!CKEDITOR.instances.messageContent) {
                    CKEDITOR.replace('messageContent', {
                        extraPlugins: 'autogrow',
                        removePlugins: 'resize', height: contentHeight,
                        toolbarLocation: 'bottom', uiColor: '#FAFAFA',
                        autoGrow_onStartup: false,
                        autoGrow_minHeight: contentHeight
                    });
                }

                CKEDITOR.instances.messageContent.updateElement();
                CKEDITOR.instances.messageContent.setData(content ? content : "");
            }
        }, 200);

        $("#content").css("height", $("#sidebar").height());
        $(".wrapper-message").css("height", $("#content").height() - 75);
    }

    function calculateMessageContent() {
        var messageSubject = $(".message-detail .message-subject").height();
        var messageTo = $("#detailUserSend").height();
        var messageFrom = $(".message-detail .message-from").height();
        var messageCc = $(".message-detail .message-cc").height();
        var messageTime = $(".message-detail .message-time").height();

        var windowHeight = $(window).height();

        var contentHeight = windowHeight - (footerHeight + 30 + messageOptionBottom + messageSubject + messageTo + messageTime + messageCc + messageFrom + 70);

        $(".message-detail .message-content").css("height", contentHeight);
    }
}

var notifyViewModel = new NotificationViewModel();
var token = $("input[name=__RequestVerificationToken]").val();

ko.applyBindings(notifyViewModel, $(".wrapper-notification-center")[0]);