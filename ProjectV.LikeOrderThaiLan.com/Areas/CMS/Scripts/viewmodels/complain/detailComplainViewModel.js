var DetailComplainViewModel = function (chatViewModel) {
    var self = this;
    self.titleTicket = ko.observable();
    self.inforTicket = ko.observable();
    self.listTicketComment = ko.observableArray([]);

    $(function () {
        var arr = _.split(window.location.href, "ticketId=");
        if (arr.length > 1) {
            self.detaiTicket(arr[1]);
        }
    });
    //Todo chi tiết khiếu nại
    self.detaiTicket = function (id) {
        self.titleTicket("ข้อมูลการร้องเรียน");
        self.inforTicket("ข้อมูลของออเดอร์");
        //self.mapTicketModel(data);
        self.ticketnModel(new ComplainModel());
        self.listTicketComment([]);
        //self.data(data);
        $.post("/" + window.culture + "/CMS/Ticket/DetailTicket",
            { ticketId: id },
            function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                } else {
                    self.mapTicketModel(result.complainDetail);
                    self.listTicketComment(result.comments);
                    chatViewModel.showChat(id);
                }
            });
    }
    
    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Ticket/Ticket";
        //self.GetAll();
        //self.templateId("deposit");
    };

    self.ticketnModel = ko.observable(new ComplainModel());
    //Todo==================== Object Map dữ liệu trả về View =========================================
    
    self.mapTicketModel = function (data) {
        self.ticketnModel(new ComplainModel());

        self.ticketnModel().Id(data.Id);
        self.ticketnModel().Code(data.Code);
        self.ticketnModel().TypeOrder(data.TypeOrder);
        self.ticketnModel().TypeService(data.TypeService);
        self.ticketnModel().TypeServiceName(data.TypeServiceName);
        self.ticketnModel().TypeServiceClose(data.TypeServiceClose);
        self.ticketnModel().TypeServiceCloseName(data.TypeServiceCloseName);
        self.ticketnModel().ImagePath1(data.ImagePath1);
        self.ticketnModel().ImagePath2(data.ImagePath2);
        self.ticketnModel().ImagePath3(data.ImagePath3);
        self.ticketnModel().ImagePath4(data.ImagePath4);
        self.ticketnModel().ImagePath5(data.ImagePath5);
        self.ticketnModel().ImagePath6(data.ImagePath6);
        self.ticketnModel().Content(data.Content);
        self.ticketnModel().OrderId(data.OrderId);
        self.ticketnModel().OrderCode(data.OrderCode);
        self.ticketnModel().OrderType(data.OrderType);
        self.ticketnModel().CustomerId(data.CustomerId);
        self.ticketnModel().CustomerName(data.CustomerName);
        self.ticketnModel().CreateDate(data.CreateDate);
        self.ticketnModel().LastUpdateDate(data.LastUpdateDate);
        self.ticketnModel().SystemId(data.SystemId);
        self.ticketnModel().SystemName(data.SystemName);
        self.ticketnModel().Status(data.Status);
        self.ticketnModel().LastReply(data.LastReply);
        self.ticketnModel().BigMoney(data.BigMoney);
        self.ticketnModel().IsDelete(data.IsDelete);
        self.ticketnModel().RequestMoney(data.RequestMoney);
        self.ticketnModel().Content(data.Content);
    };
}
//ko.applyBindings(new DetailDepositViewModel());
var chatViewModel = new ChatTicketViewModel();
var detailComplainViewModel = new DetailComplainViewModel(chatViewModel);
ko.applyBindings(detailComplainViewModel, $("#contentTabDetailComplain")[0]);

