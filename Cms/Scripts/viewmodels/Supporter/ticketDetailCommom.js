/// <reference path="../../bootstrap.js" />
function TicketDetailViewModel(modelId) {
    var self = this;
    //khai báo biến sử dụng trong ticketDetail
    self.isUpload = ko.observable(true);
    self.complainuser1 = ko.observable([]);
    self.customerEmail = ko.observable();
    self.customerPhone = ko.observable();
    self.customerAddress = ko.observable();
    self.customerLevel = ko.observable();
    self.complainuser = ko.observable([]);
    self.count = ko.observable();
    self.titleCustomer = ko.observable();
    self.titleTicket = ko.observable();
    self.complainModel = ko.observable(new complainModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    self.isDetailRending = ko.observable(false);
    self.listUserDetail = ko.observableArray([]);
    self.contentUser = ko.observable();
    self.contentCustomer = ko.observable();
    self.complainuserinternallist = ko.observable([]);
    self.listComplainOrder = ko.observableArray([]);


    // Tìm kiếm nhân viên hỗ trợ khiếu nại
    self.GetUser = function () {
        self.listUserDetail([]);
        $.post("/Ticket/GetAllUser", {}, function (data) {
            self.listUserDetail(data);
        });
    }

    ///Detail Ticket
    self.viewTicketDetail = function (data) {
        self.GetTicketDetailCommon(data.Id);
    }
    self.GetTicketDetailCommon = function (ticketId) {
        self.titleCustomer('Customer details');
        self.customerEmail("");
        self.customerPhone("");
        self.customerAddress("");
        self.customerLevel("");
        self.complainuser1([]);
        self.complainuser([]);
        self.complainuserinternallist([]);
        self.complainModel(new complainModel());
        self.complainDetailModel(new complainDetailModel());
        self.isDetailRending(false);
        self.GetUser();
        self.isUpload(false);
        $.post("/Ticket/GetTicketDetail", { ticketId: ticketId }, function (result) {
            self.isDetailRending(true);
            if (result.ticketModal != null) {
                self.mapComplainModel(result.ticketModal);
                self.customerEmail(result.customer.Email);
                self.customerPhone(result.customer.Phone);
                self.customerAddress(result.customer.Address);
                self.customerLevel(result.customer.LevelName);
                self.complainuser1(result.list);
                if (result.complainusermain != null) {
                    self.mapcomplainDetailModel(result.complainusermain);
                }
                self.listComplainOrder(result.listComplainOrder);
                var list = self.ChatView(result.complainuserinternallist);
                self.complainuserinternallist(list);

                list = self.ChatView(result.complainuserlist);
                self.complainuser(list);

                $('#ticketDetailModal').modal();
            }
            else {
                $('#ticketDetailModalMini').modal();
            }
        });
        self.interval();
    }
    self.GetTicketDetailView = function (ticketId) {
        self.GetTicketDetailCommon(ticketId);
    }
    //trao đổi với khách hàng
    self.feedbackComplainModal = function () {
        $('#commentForCustomer').modal();
    }

    //hàm phản hồi cho khách hàng
    self.feedbackComplain = function () {
        if (self.contentCustomer() === "" || self.contentCustomer() == null) {
            $('#requireCustomer').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: self.contentCustomer(), objectChat: false }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    self.contentCustomer("");
                    self.GetTicketDetail(self.complainModel().Id());
                    
                }
            });
        }
    };
    var maxFileLength = 5120000;
    self.addImageDetail = function (data1) {

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
                if (data1 == 1)
                {
                    $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: window.location.origin + data.result[0].path, type: 1, objectChat: false }, function (result) {
                        self.GetTicketDetail(self.complainModel().Id());
                        self.contentCustomer("");
                    });
                }
                else {
                    $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: window.location.origin + data.result[0].path, type: 1, objectChat: true }, function (response) {
                        if (!response.status) {
                            toastr.error(response.msg);
                        } else {
                            //toastr.success(response.msg);
                            self.GetTicketDetail(self.complainModel().Id());
                            self.contentUser("");
                        }
                    });

                }
                
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };

    //hàm trao đổi giữa các nhân viên
    self.feedbackUser = function () {
        if (self.contentUser() === "" || self.contentUser() == null) {
            $('#requireUser').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: self.contentUser(), objectChat: true }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    self.GetTicketDetail(self.complainModel().Id());
                    self.contentUser("");
                }
            });
        }
    };
    // hàm huy ticket
    self.requestCancel = function () {

    };
    self.cancelComplain = function () { };

    // hàm hoàn thành ticket
    self.request = function () {

    };
    self.finishComplain = function () {

    };
    //Thêm người hỗ trợ
    self.AddUserSupport = function () {
    }

    //Xóa hỗ trợ
    self.deleteSupport = function (userId) {
    }

    //hàm phản hồi cho khách hàng
    //self.feedbackComplain = function () { }
    self.receiveFixTicket = function (data) { }

    // Hiển thị nội dung trao đổi về khiếu nại khách hàng để chỉnh Edit
    self.contentEdit = ko.observable();
    self.contentEditId = ko.observable();

    self.editContentUser = function (data) {
        self.contentEditId();
        self.contentEdit();
        self.contentEdit(data.Content);
        self.contentEditId(data.Id);
        $('#commentEdit').modal();
    }

    self.deleteContentUser = function (data) {
        self.contentEditId();
        self.contentEditId(data.Id);
        $('#commentDelete').modal();
    }

    // Cập nhật nội dung trao đổi về khiếu nại khách hàng
    self.updateContent = function () {
        $.post("/Ticket/UpdateContent", { complainDetailId: self.contentEditId(), complainContent: self.contentEdit() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                //toastr.success(response.msg);
                //self.getInit();
                self.GetTicketDetail(self.complainModel().Id());
            }
        });
    }

    // Xóa nội dung trao đổi về khiếu nại khách hàng
    self.deleteContent = function () {
        $.post("/Ticket/DeleteContent", { complainDetailId: self.contentEditId() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                //toastr.success(response.msg);
                //self.getInit();
                self.GetTicketDetail(self.complainModel().Id());
            }
        });
    }
    self.interval = function () {
        var interval = setInterval(function () {
            self.renderedHandler();
            var objDiv = $("div.customercomplain");
            _.forEach(objDiv, function (value) {
                var height = value.scrollHeight;
                if (height >= 400) {
                    clearInterval(interval);
                }
            });
        }, 100);
    }
    //Cho thanh cuộn về đầu trang
    self.renderedHandler = function () {
        var objDiv = $("div.customercomplain");
        _.forEach(objDiv, function (value) {
            objDiv.scrollTop(value.scrollHeight);
        });
    }
    // Nhận ticket nhận về xử lý
    self.receiveTicket = function (data) {
    }
    self.GetTicketDetail = function (ticketId) {
        self.customerEmail("");
        self.customerPhone("");
        self.customerAddress("");
        self.customerLevel("");
        self.complainuser1([]);
        self.complainuser([]);
        self.complainuserinternallist([]);
        self.complainModel(new complainModel());
        self.complainDetailModel(new complainDetailModel());
        self.isUpload(false);
        $.post("/Ticket/GetTicketDetail", { ticketId: ticketId }, function (result) {
            self.isDetailRending(true);
            if (result.ticketModal != null) {
                self.mapComplainModel(result.ticketModal);
                self.customerEmail(result.customer.Email);
                self.customerPhone(result.customer.Phone);
                self.customerAddress(result.customer.Address);
                self.customerLevel(result.customer.LevelName);
                self.complainuser1(result.list);
                //self.complainuser(result.complainuserlist);
                //self.complainuserinternallist(result.complainuserinternallist);
                if (result.complainusermain != null) {
                    self.mapcomplainDetailModel(result.complainusermain);
                }
                self.listComplainOrder(result.listComplainOrder);
                var list = self.ChatView(result.complainuserinternallist);
                self.complainuserinternallist(list);

                list = self.ChatView(result.complainuserlist);
                self.complainuser(list);

            }
            self.interval();
        });
        
    }

    self.ChatView = function (complainuserlist) {
        var list = [];

        var group = _.groupBy(complainuserlist, function (obj) {
            return obj.GroupId;
        });

        var array = $.map(group, function (value) {
            return [value];
        });
        
        _.forEach(array, function (listObj) {
            var item = {
                Id: _.last(listObj).Id,
                ListObj: [],
                UserId: listObj[0].UserId,
                UserName: listObj[0].UserName,
                Content: listObj[0].Content,
                ComplainId: listObj[0].ComplainId,
                CreateDate: listObj[0].CreateDate,
                CustomerId: listObj[0].CustomerId,
                CustomerName: listObj[0].CustomerName,
                IsInHouse: listObj[0].IsInHouse,
                IsRead: listObj[0].IsRead,
                CommentType: listObj[0].CommentType,
                SystemId: listObj[0].SystemId,
                SystemName: listObj[0].SystemName,
                UserPosition: listObj[0].UserPosition,
                GroupId: listObj[0].GroupId
            }
            _.forEach(listObj, function (obj) {
                item.ListObj.push(obj);
            });

            list.push(item);
        });

        //self.complainuser(list);
        self.isUpload(true);
        return list;
    };

    // Object Detail Khiếu nại
    self.mapComplainModel = function (data) {
        self.complainModel(new complainModel());

        self.complainModel().Id(data.Id);
        self.complainModel().Code(data.Code);
        self.complainModel().TypeOrder(data.TypeOrder);
        self.complainModel().TypeService(data.TypeService);
        self.complainModel().TypeServiceName(data.TypeServiceName);
        self.complainModel().TypeServiceClose(data.TypeServiceClose);
        self.complainModel().TypeServiceCloseName(data.TypeServiceCloseName);
        self.complainModel().ImagePath1(data.ImagePath1);
        self.complainModel().ImagePath2(data.ImagePath2);
        self.complainModel().ImagePath3(data.ImagePath3);
        self.complainModel().ImagePath4(data.ImagePath4);
        self.complainModel().ImagePath5(data.ImagePath5);
        self.complainModel().ImagePath6(data.ImagePath6);
        self.complainModel().Content(data.Content);
        self.complainModel().OrderId(data.OrderId);
        self.complainModel().OrderCode(data.OrderCode);
        self.complainModel().OrderType(data.OrderType);
        self.complainModel().CustomerId(data.CustomerId);
        self.complainModel().CustomerName(data.CustomerName);
        self.complainModel().CreateDate(data.CreateDate);
        self.complainModel().LastUpdateDate(data.LastUpdateDate);
        self.complainModel().SystemId(data.SystemId);
        self.complainModel().SystemName(data.SystemName);
        self.complainModel().Status(data.Status);
        self.complainModel().LastReply(data.LastReply);
        self.complainModel().BigMoney(data.BigMoney);
        self.complainModel().IsDelete(data.IsDelete);
        self.complainModel().RequestMoney(data.RequestMoney);
        self.complainModel().ContentInternal(data.ContentInternal);

    };

    self.mapcomplainDetailModel = function (data) {
        self.complainDetailModel(new complainDetailModel());
        self.complainDetailModel().Id(data.Id);
        self.complainDetailModel().ComplainId(data.ComplainId);
        self.complainDetailModel().UserId(data.UserId);
        self.complainDetailModel().Content(data.Content);
        self.complainDetailModel().AttachFile(data.AttachFile);
        self.complainDetailModel().CreateDate(data.CreateDate);
        self.complainDetailModel().UpdateDate(data.UpdateDate);
        self.complainDetailModel().UserRequestId(data.UserRequestId);
        self.complainDetailModel().UserRequestName(data.UserRequestName);
        self.complainDetailModel().CustomerId(data.CustomerId);
        self.complainDetailModel().CustomerName(data.CustomerName);
        self.complainDetailModel().UserName(data.UserName);
        self.complainDetailModel().IsRead(data.IsRead);
        self.complainDetailModel().UserPosition(data.UserPosition);
    };
    self.NoteClose = function () {

    }
}