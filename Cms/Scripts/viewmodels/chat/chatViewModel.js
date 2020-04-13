function ChatViewModel(modalId) {
    var self = this;

    self.orderId = ko.observable();
    self.orderCode = ko.observable();
    self.orderType = ko.observable();
    self.listChat = ko.observableArray([]);
    self.text = ko.observable("");
    self.isSend = ko.observable(true);
    self.isUpload = ko.observable(true);
    self.templateChatId = ko.observable("isEmp");

    self.showChat = function (orderId, orderCode, orderT, type) {
        orderCode = ReturnCode(orderCode);

        if (type === undefined) {
            $("#chatModal").modal();

            self.templateChatId("isEmp");
        } else {
            self.templateChatId("temDivBoxChat");
        }

        self.orderId(orderId);
        self.orderCode(orderCode);
        self.orderType(orderT);

        self.getChat(orderId, orderCode, orderT);
        self.interval();
    };

    self.getChat = function (orderId, orderCode, orderType) {
        if (orderId != null) {
            $.post("/Chat/GetChat", { orderId: orderId, orderType: orderType }, function (result) {
                var list = [];
                var group = _.groupBy(result, function (obj) {
                    return obj.GroupId;
                });

                var array = $.map(group, function (value) {
                    return [value];
                });

                _.forEach(array, function (listObj) {
                    var item = {
                        Id: listObj[0].Id,
                        ListObj: [],
                        OrderId: listObj[0].OrderId,
                        CustomerId: listObj[0].CustomerId,
                        UserId: listObj[0].UserId,
                        CreateDate: listObj[0].CreateDate,
                        IsRead: listObj[0].IsRead,
                        CustomerName: listObj[0].CustomerName,
                        UserName: listObj[0].UserName,
                        SystemId: listObj[0].SystemId,
                        SystemName: listObj[0].SystemName,
                        OrderType: listObj[0].OrderType,
                        CommentType: listObj[0].CommentType,
                        UserFullName: listObj[0].UserFullName,
                        UserOffice: listObj[0].UserOffice,
                        UserPhone: listObj[0].UserPhone,
                        GroupId: listObj[0].GroupId
                    }
                    _.forEach(listObj, function (obj) {
                        item.ListObj.push(obj);
                    });

                    list.push(item);
                });
                self.listChat(list);
            });
        }

    }

    self.submit = function () {
        self.isSend(false);
        if (self.text()) {
            $.post("/Chat/AddChat", { orderId: self.orderId(), orderType: self.orderType(), text: self.text() }, function (result) {
                var list = [];
                if (result.status == msgType.error) {
                    toastr.error(result.msg);
                } else {

                    var group = _.groupBy(result.listChat, function (obj) {
                        return obj.GroupId;
                    });

                    var array = $.map(group, function (value) {
                        return [value];
                    });

                    _.forEach(array, function (listObj) {
                        var item = {
                            Id: listObj[0].Id,
                            ListObj: [],
                            OrderId: listObj[0].OrderId,
                            CustomerId: listObj[0].CustomerId,
                            UserId: listObj[0].UserId,
                            CreateDate: listObj[0].CreateDate,
                            IsRead: listObj[0].IsRead,
                            CustomerName: listObj[0].CustomerName,
                            UserName: listObj[0].UserName,
                            SystemId: listObj[0].SystemId,
                            SystemName: listObj[0].SystemName,
                            OrderType: listObj[0].OrderType,
                            CommentType: listObj[0].CommentType,
                            UserFullName: listObj[0].UserFullName,
                            UserOffice: listObj[0].UserOffice,
                            UserPhone: listObj[0].UserPhone,
                            GroupId: listObj[0].GroupId
                        }
                        _.forEach(listObj, function (obj) {
                            item.ListObj.push(obj);
                        });

                        list.push(item);
                    });

                    self.listChat(list);
                    self.text("");
                    self.isSend(true);
                }
            });
        } else {
            self.isSend(true);
        }
    }
    var maxFileLength = 5120000;;
    self.addImage = function () {
        $(".flieuploadImg").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Size is too large";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Not in correct format";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("The file is not allowed");
                    return;
                }

                self.isUpload(false);

                $.post("/Chat/AddChat", { orderId: self.orderId(), orderType: self.orderType(), text: window.location.origin + data.result[0].path, type: 1 }, function (result) {
                    var list = [];
                    if (result.status == msgType.error) {
                        toastr.error(result.msg);
                    } else {

                        var group = _.groupBy(result.listChat, function (obj) {
                            return obj.GroupId;
                        });

                        var array = $.map(group, function (value) {
                            return [value];
                        });

                        _.forEach(array, function (listObj) {
                            var item = {
                                Id: listObj[0].Id,
                                ListObj: [],
                                OrderId: listObj[0].OrderId,
                                CustomerId: listObj[0].CustomerId,
                                UserId: listObj[0].UserId,
                                CreateDate: listObj[0].CreateDate,
                                IsRead: listObj[0].IsRead,
                                CustomerName: listObj[0].CustomerName,
                                UserName: listObj[0].UserName,
                                SystemId: listObj[0].SystemId,
                                SystemName: listObj[0].SystemName,
                                OrderType: listObj[0].OrderType,
                                CommentType: listObj[0].CommentType,
                                UserFullName: listObj[0].UserFullName,
                                UserOffice: listObj[0].UserOffice,
                                UserPhone: listObj[0].UserPhone,
                                GroupId: listObj[0].GroupId
                            }
                            _.forEach(listObj, function (obj) {
                                item.ListObj.push(obj);
                            });

                            list.push(item);
                        });

                        self.listChat(list);
                        self.isUpload(true);
                    }
                });
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    self.interval = function () {
        var interval = setInterval(function () {
            self.renderedHandler();
            var objDiv = $("div.divBoxChat");
            _.forEach(objDiv, function (value) {
                var height = value.scrollHeight;
                if (height >= 250) {
                    clearInterval(interval);
                }
            });
        }, 0);
    }

    self.renderedHandler = function () {
        var objDiv = $("div.divBoxChat");
        _.forEach(objDiv, function (value) {
            objDiv.scrollTop(value.scrollHeight);
        });
    }

    self.checkKeyUpEnter = function (data, event) {
        if (event.keyCode == 13 && !event.ctrlKey) {
            self.submit();
        }
    };
};

$('#showImgModal').click(function () {
    document.getElementById('showImgModal').style.display = 'none';
});

$('#imgShow').click(function (event) {
    event.stopPropagation();
});