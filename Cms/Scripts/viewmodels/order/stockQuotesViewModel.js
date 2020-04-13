function StockQuotesViewModel() {
    var self = this;

    self.customer = ko.observableArray([]);
    self.isDetailRending = ko.observable(true);
    self.listDetail = ko.observableArray([]);
    self.listSupplier = ko.observableArray([]);
    self.exchangeRate = ko.observable();
    self.source = ko.observableArray([]);
    self.viewBoxChat = new ChatViewModel();
    
    self.showModalDialog = function (id) {

        self.listDetail([]);
        self.listSupplier([]);
        self.userOrder([]);

        self.isDetailRending(false);
        self.isShowHistory(false);

        $.post("/Source/GetData", function (result) {
            if (result.status === msgType.success) {
                self.exchangeRate(result.exchangeRate);
            } else {
                toastr.error(result.msg);
                self.isDetailRending(true);
            }
        });

        if (id > 0) {
            $(".view-chat-box").show();

            $.post("/Source/GetSourceDetail", { id: id }, function (result) {
                if (result.status === msgType.success) {
                    self.source(result.source);
                    self.customer(result.customer);
                    self.listDetail(result.listDetail);
                    self.listSupplier(result.listSupplier);
                    self.listHistory(result.listHistory);
                    self.userOrder(result.userOrder);

                    self.isDetailRending(true);
                    $("#stockQuotesView").modal();

                    self.viewBoxChat.showChat(result.source.Id, result.source.Code, result.source.Type, 1);

                } else {
                    toastr.error(result.msg);
                    self.isDetailRending(true);
                }
            });
        } else {
            toastr.error("Can not display");
        }
    }

    self.listHistory  = ko.observableArray([]);
    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function() {
        self.isShowHistory(!self.isShowHistory());
    }
    self.userOrder = ko.observableArray([]);
}