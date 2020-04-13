var ComplainDetailViewModel = function () {
    var self = this;
    //========== Khai báo ListData
    self.listComment = ko.observableArray([]);
    self.complainModel = ko.observable(new complainModel());

    /// Object chi tiết thông báo
    self.mapComplainModel = function (data) {
        self.complainModel(new complainModel());
        self.complainModel().Id(data.Id);
        self.complainModel().Code(data.Code);
        self.complainModel().TypeOrder(data.TypeOrder);
        self.complainModel().TypeService(data.TypeService);
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
    };


    self.viewComplainModal = function (data) {
        $.ajax({
            type: 'GET',
            url: "/Ticket/DetailTicket",
            data: { 'id': data.Id },
            success: function (response) {
                self.mapNotiModel(response.complainDetail);
                self.listSystem(response.listSystem);
            },
            async: false
        });
    }
}