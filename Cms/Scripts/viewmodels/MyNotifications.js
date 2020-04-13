var DateFormat = "DD/MM/YYYY";

function MyNotificationsModel() {
    var self = this;

    var filterType = {
        all: 'all',
        readed: 'readed',
        unRead: 'unRead',
        info: 'info',
        warning: 'warning'
    };

    var renderPage = function (currentPage, totalRecord) {
        self.totalPage(Math.ceil(totalRecord / self.recordPerPage()));
        $("#sumaryNotifyPager").text(totalRecord === 0 ? "No notifications" : "Show {0}->{1} of {2} notify".format(((currentPage - 1) * self.recordPerPage() + 1), (currentPage * self.recordPerPage() < totalRecord ? currentPage * self.recordPerPage() : totalRecord), totalRecord));
        $("#notifyPager").pagerNextBackOnly({ pagenumber: currentPage, pagecount: self.totalPage(), totalrecords: totalRecord, buttonClickCallback: self.pageClickSearch });
    };

    self.listNotify = ko.observableArray([]);
    self.IsSearching = ko.observable(false);    
    self.IsLoadingMore = ko.observable(false); // @HenryDo
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);

    self.ShowLoadMore = ko.observable(false);
    self.Keyword = ko.observable('');
    self.checkAllNotification = ko.observable(false);    
    self.currentFilter = ko.observable(filterType.all);
    self.checkedNotifyId = ko.observableArray([]);

    self.filterTitle = ko.computed(function () {
        return self.currentFilter() === "all" ? "All"
            : self.currentFilter() === "readed" ? "Đã đọc"
            : self.currentFilter() === "unRead" ? "Chưa đọc"
            : self.currentFilter() === "info" ? "Thông tin"
            : self.currentFilter() === "warning" ? "Cảnh báo"
            : "";
    });
    // @HenryDo: Load more
    self.checkAllNotification = ko.computed({
        read: function () {
            return self.checkedNotifyId().length == self.listNotify().length && self.listNotify().length > 0;
        },
        write: function (value) {
            self.checkedNotifyId([]);
            if (value) {
                ko.utils.arrayForEach(self.listNotify(), function (item) {
                    self.checkedNotifyId.push('' + item.Id);
                });
            }
        }
    });

    // @HenryDo: Update IsRead
    self.UpdateIsRead = function (id) {        
        amplify.request("UpdateIsRead", { id: id });

        if (lastNotificationsModelView) {
            var total = parseInt(lastNotificationsModelView.totalNotifySystemUnRead()) - 1;
            lastNotificationsModelView.totalNotifySystemUnRead(total === 0 ? "" : total);
        }
    }

    self.searchSubmit = function () {        
        self.search(1);
    }

    self.markReaded = function (isRead) {
        if (self.checkedNotifyId().length > 0) {
            var messages = _.filter(self.listNotify(), function (item) {
                return _.some(self.checkedNotifyId(), function (num) { return num === item.Id.toString(); }) && item.IsRead() == !isRead && item.SendTime != '';
            });

            if (messages.length > 0) {
                notificationsModel.markReaded(_.map(messages, 'Id'), isRead, token, function (result) {
                    if (result.length === 0) {
                        toastr.error(resources.internalEmail.message.error);
                    } else {
                        var notifySuccess = _.filter(self.listNotify(), function (item) {
                            return _.some(result, function (num) { return num === item.Id; });
                        });

                        if (notifySuccess.length > 0) {
                            var currentUserId = (window.currentUser).UserId;
                            $.each(notifySuccess, function (i, item) {
                                item.IsRead(isRead);
                                var add = isRead ? -1 : 1;
                            });

                            // Cập nhập lại Sum số lượng notify
                            var totalUnread = isRead ? parseInt(lastNotificationsModelView.totalNotifySystemUnRead()) - messages.length : parseInt(lastNotificationsModelView.totalNotifySystemUnRead()) + messages.length;
                            lastNotificationsModelView.totalNotifySystemUnRead(totalUnread > 0 ? totalUnread : "");
                        }
                        self.checkedNotifyId([]);
                    }
                });
            }
        }
    };

    self.changeCheckboxFilter = function (filter) {
        var ids = [];
        if (filter === filterType.readed) {
            ids = _.map(_.map(_.filter(self.listNotify(), function (item) {
                return item.IsRead();
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.unRead) {
            ids = _.map(_.map(_.filter(self.listNotify(), function (item) {
                return !item.IsRead();
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.info) {
            ids = _.map(_.map(_.filter(self.listNotify(), function (item) {
                return item.Type === 1;
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.warning) {
            ids = _.map(_.map(_.filter(self.listNotify(), function (item) {
                return item.Type === 0;
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.all) {
            ids = _.map(_.map(self.listNotify(), 'Id'), function (id) {
                return id.toString();
            });
        }

        self.checkedNotifyId(ids);
    };

    self.changeFilter = function (filter) {
        if (filter === filterType.readed) {
            //self.filterTitle("Đã đọc");
            self.currentFilter(filterType.readed);
        } else if (filter === filterType.unRead) {
            //self.filterTitle("Chưa đọc");
            self.currentFilter(filterType.unRead);
        } else if (filter === filterType.info) {
            //self.filterTitle("Thông tin");
            self.currentFilter(filterType.info);
        } else if (filter === filterType.warning) {
            //self.filterTitle("Cảnh báo");
            self.currentFilter(filterType.warning);
        } else {
            //self.filterTitle("Tất cả");
            self.currentFilter(filterType.all);
        }

        self.checkAllNotification(false);
        self.checkedNotifyId([]);
        self.search(1);
    };

    self.search = function (pageclickednumber) {
        var isRead, type;
        self.IsSearching(true);
        if (self.currentFilter() === filterType.readed) {
            isRead = true;
            type = null;
        } else if (self.currentFilter() === filterType.unRead) {
            isRead = false;
            type = null;
        } else if (self.currentFilter() === filterType.info) {
            isRead = null;
            type = 1;
        } else if (self.currentFilter() === filterType.warning) {
            isRead = null;
            type = 0;
        }        

        notificationsModel.getMyNotifications(self.Keyword(), isRead, type, pageclickednumber, self.recordPerPage(), function (result) {
            self.IsSearching(false);
            //formatDateInList(result.items);

            _.each(result.items,
                function (it) {
                    it.IsRead = ko.observable(it.IsRead);
                    it.test = ko.observable("noi dung");
                    var sendTime = moment(it.SendTime).format('DD/MM/YYYY hh:mm:ss a');
                    it.SendTime = sendTime == 'Invalid date' ? '' : sendTime;
                });

            self.listNotify(result.items);
            self.currentPage(result.currentPage);

            if (result.totalRecord > (self.recordPerPage() * self.currentPage())) {
                self.ShowLoadMore(true);
            } else {
                self.ShowLoadMore(false);
            }

            renderPage(self.currentPage(), result.totalRecord);
        });
    };

    self.pageClickSearch = function (pageclickednumber) {
        $("#notifyPager").pagerNextBackOnly({ pagenumber: pageclickednumber, pagecount: self.totalPage(), buttonClickCallback: self.pageClickSearch });
        self.search(pageclickednumber);
    };

    $(document).ready(function () {
        _.each(window.notifications.items,
            function (it) {
                it.IsRead = ko.observable(it.IsRead);
                it.test = ko.observable("noi dung");

                var sendTime = moment(it.SendTime).format('DD/MM/YYYY hh:mm:ss a');
                it.SendTime = sendTime == 'Invalid date' ? '' : sendTime;
            });

        self.listNotify(window.notifications.items);
        renderPage(self.currentPage(), window.notifications.totalRecord);

        if (window.notifications.totalNotification > self.recordPerPage()) {
            self.ShowLoadMore(true);
        }
    });
};

//var modelView = new MyNotificationsModel();
//ko.applyBindings(modelView, $('.main-content')[0]);