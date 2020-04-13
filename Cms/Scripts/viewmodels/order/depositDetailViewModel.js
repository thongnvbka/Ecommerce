function DepositDetailViewModel(modelId) {
    var self = this;

    self.isDetailRending = ko.observable(true);
    self.deposit = ko.observableArray([]);
    self.exchangeRate = ko.observable();
    self.listDetail = ko.observableArray([]);
    self.listPackageView = ko.observableArray([]);
    self.viewBoxChat = new ChatViewModel();
    self.listOrderService = ko.observableArray([]);
    self.listOrderServiceCheck = ko.observableArray([]);
    self.listOrderServiceOther = ko.observableArray([]);

    self.showModalDialog = function (id) {
        self.isDetailRending(false);
        self.isShowHistory(false);

        self.deposit([]);
        self.exchangeRate(0);
        self.listDetail([]);
        self.listPackageView([]);
        self.userOrder([]);
        self.listOrderService([]);
        self.listOrderServiceOther([]);

        $.post("/Deposit/GetData", function (result) {
            if (result.status === msgType.success) {
                self.exchangeRate(result.exchangeRate);
            } else {
                toastr.error(result.msg);
                self.isDetailRending(true);
            }
        });

        if (id > 0) {
            $.post("/Deposit/GetDepositDetail",
                { id: id },
                function (result) {
                    if (result.status === msgType.success) {
                        self.deposit(result.deposit);
                        self.listDetail(result.listDetail);
                        self.userOrder(result.userOrder);
                        self.listOrderService(result.listOrderService);
                        self.listOrderServiceOther(result.listOrderServiceOther);

                        self.listPackageView(result.listPackageView);
                        self.listHistory(result.listHistory);

                        self.listOrderServiceCheck([]);
                        _.each(result.listOrderServiceCheck,
                            function (item) {

                                item.Checked = ko.observable(item.Checked);

                                self.listOrderServiceCheck.push(item);
                            });

                        self.isDetailRending(true);
                        $("#orderDepositDetail").modal();

                        self.viewBoxChat.showChat(result.deposit.Id, result.deposit.Code, result.deposit.Type, 1);

                        self.initInputMark();

                        self.orderCodeWarehouse(self.deposit().WarehouseDeliveryId, self.deposit().Code);

                    } else {
                        toastr.error(result.msg);
                        self.isDetailRending(true);
                    }
                });
        }
    }

    self.listHistory  = ko.observableArray([]);
    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function() {
        self.isShowHistory(!self.isShowHistory());
    }
    
    self.userOrder = ko.observableArray([]);

    self.initInputMark = function () {
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }
}
