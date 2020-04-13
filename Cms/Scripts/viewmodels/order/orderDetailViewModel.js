function OrderDetailViewModel(modelId) {
    var self = this;

    self.isDetailRending = ko.observable(false);

    self.order = ko.observableArray([]);
    self.customerToAddress = ko.observableArray([]);
    self.customerFormAddress = ko.observableArray([]);
    self.listWarehouseOrder = ko.observableArray([]);
    self.listPackageView = ko.observableArray([]);
    self.listDetail = ko.observableArray([]);
    self.warehouseIdOrder = ko.observableArray([]);
    self.listOrderService = ko.observableArray([]);
    self.customer = ko.observableArray([]);
    self.orderExchange = ko.observableArray([]);
    self.userOrder = ko.observableArray([]);
    self.totalPriceService = ko.observableArray([]);
    self.listContractCode = ko.observableArray([]);
    self.viewBoxChat = new ChatViewModel();
    self.cashShortage = ko.observable();
    self.totalPriceCustomer = ko.observable();
    self.totalShop = ko.observable();
    self.listHistory = ko.observableArray([]);
    self.listOrderExchage = ko.observableArray([]);
    self.listLog = ko.observableArray([]);
    self.mess = ko.observable();
    self.orderReason = ko.observable();
    self.listOrderShop = ko.observableArray([]);

    //Hàm hiển thị Detail Orders
    self.viewOrderDetail = function (id) {
        self.isDetailRending(false);
        self.isShowHistory(false);
        self.isShowLog(false);

        self.order([]);
        self.customerToAddress([]);
        self.customerFormAddress([]);
        self.listWarehouseOrder([]);
        self.listPackageView([]);
        self.listDetail([]);
        self.warehouseIdOrder([]);
        self.listOrderService([]);
        self.customer([]);
        self.orderExchange([]);
        self.userOrder([]);
        self.totalPriceService([]);
        self.listContractCode([]);
        self.cashShortage(0);
        self.totalPriceCustomer(0);
        self.totalShop(0);
        self.orderReason(null);
        self.listOrderShop([]);

        if (typeof id === "object") {
            toastr.error('Incorrect input data');
            return;
        }

        self.titleTotalCus = ko.observable('Customer must pay');

        $.post("/Order/GetOrderDetail", { orderId: id }, function (result) {
            if (result.status !== msgType.error) {
                self.order(ko.mapping.toJS(result.order));

                self.customerToAddress(result.toAddress);
                self.customerFormAddress(result.formAddress);
                self.listWarehouseOrder(result.listWarehouse);

                //Lấy danh sách kiên hàng
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderListPackage",
                    data: { orderId: id },
                    success: function (resultListPackage) {
                        _.each(resultListPackage.listPackageView,
                           function (it) {
                               it.cacheTransportCode = it.TransportCode;
                               it.cacheForcastDate = it.ForcastDate;
                               it.cacheNote = it.Note;

                               it.TransportCode = ko.observable(it.TransportCode);
                               it.TransportCode.subscribe(function (newValue) {
                                   if (it.cacheTransportCode !== newValue) {
                                       self.updatePackage(ko.mapping.toJS(it));
                                       it.cacheTransportCode = newValue;
                                   }
                               });

                               it.ForcastDate = ko
                                   .observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                               it.ForcastDate.subscribe(function (newValue) {
                                   if (it.cacheForcastDate !== newValue) {
                                       self.updatePackage(ko.mapping.toJS(it));
                                       it.cacheForcastDate = newValue;
                                   }
                               });

                               it.Note = ko.observable(it.Note);
                               it.Note.subscribe(function (newValue) {
                                   if (it.cacheNote !== newValue) {
                                       self.updatePackage(ko.mapping.toJS(it));
                                       it.cacheNote = newValue;
                                   }
                               });
                           });

                        self.listPackageView(resultListPackage.listPackageView);
                    },
                    async: false
                });

                //Lấy danh sách Detail Orders
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderListDetail",
                    data: { orderId: id },
                    success: function (resultListDetail) {
                        self.listDetail(resultListDetail.listOrderDetail);
                    },
                    async: false
                });

                //Lấy danh sách lịch sử và log
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderHistory",
                    data: { orderId: id },
                    success: function (resultHistory) {
                        self.listLog(resultHistory.listLog);
                        self.listHistory(resultHistory.listHistory);
                    },
                    async: false
                });
               
                self.listContractCode(result.listContractCode);
                self.listOrderExchage(result.listOrderExchage);
                self.orderExchangeOther(_.reduce(self.listOrderExchage(), function (count, item) { return count + item.TotalPrice; }, 0));

                self.warehouseIdOrder(result.order.WarehouseId);
                self.listOrderService(result.listOrderService);
                self.customer(result.customer);
                self.orderExchange(result.orderExchange);
                self.userOrder(result.userOrder);
                self.mess(result.mess);
                self.orderReason(result.orderReason);
                self.listOrderShop(result.listOrderShop);

                self.order().PriceBargain = formatNumbericCN(result.order.PriceBargain === null ? 0 : result.order.PriceBargain, 'N2');
                self.order().PaidShop = formatNumbericCN(result.order.PaidShop === null ? 0 : result.order.PaidShop, 'N2');
                self.order().FeeShip = formatNumbericCN(result.order.FeeShip === null ? 0 : result.order.FeeShip, 'N2');
                self.order().FeeShipBargain = formatNumbericCN(result.order.FeeShipBargain === null ? 0 : result.order.FeeShipBargain, 'N2');
                self.totalPriceCustomer(formatNumbericCN(self.order().TotalPrice + Globalize("en-US").parseFloat(self.order().FeeShip), 'N2'));
                self.totalShop(formatNumbericCN(Globalize("en-US").parseFloat(self.order().PaidShop) + Globalize("en-US").parseFloat(self.order().FeeShipBargain), 'N2'));

                self.totalPriceService(_.reduce(result.listOrderService,
                   function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                   0));

                if (self.orderExchange() !== null) {
                    //var cashShortage  = self.order().Total - self.orderExchange().TotalPrice - self.orderExchangeOther();

                    //if (cashShortage < 0) {
                    //    self.titleTotalCus('Money to pay customer');
                    //    self.cashShortage(formatNumbericCN(cashShortage * (-1), 'N2'));
                    //} else {
                    //    self.titleTotalCus('Customer must pay');
                    //    self.cashShortage(formatNumbericCN(cashShortage, 'N2'));
                    //}


                    if (self.order().Debt < 0) {
                        self.titleTotalCus('Money to pay customer');
                        self.cashShortage(formatNumbericCN(self.order().Debt * (-1), 'N2'));
                    } else {
                        self.titleTotalCus('Customer must pay');
                        self.cashShortage(formatNumbericCN(self.order().Debt, 'N2'));
                    }
                }

                if (result.status === msgType.warning) {
                    toastr.warning(result.msg);
                }

                $(modelId === undefined ? '#orderDetailModal' : "#" + modelId).modal();
            }
            else {
                toastr.error(result.msg);
            }
            self.isDetailRending(true);

            self.viewBoxChat.showChat(self.order().Id, self.order().Code, self.order().Type, 1);

            self.orderCodeWarehouse(self.order().WarehouseDeliveryId, self.order().Code);
        });
    };

    self.showOrderShop = function() {
        $('#orderShopView').modal();
    }

    self.orderExchangeOther = ko.observable(0); //Tiền khác khách hàng đã thanh toán
    self.IsExchangeOther = ko.observable(false);
    self.titleExchangeOther = ko.observable("Detail");
    self.showExchangeOther = function () {
        self.IsExchangeOther(!self.IsExchangeOther());
        self.titleExchangeOther(self.IsExchangeOther() ? "collapse" : "Detail");
    };

    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function() {
        self.isShowHistory(!self.isShowHistory());
    }

    self.isShowLog = ko.observable(false);
    self.checkShowLog = function () {
        self.isShowLog(!self.isShowLog());
    }

    self.IsExchangeOther = ko.observable(false);
    self.titleExchangeOther = ko.observable("Detail");
    self.showExchangeOther = function () {
        self.IsExchangeOther(!self.IsExchangeOther());
        self.titleExchangeOther(self.IsExchangeOther() ? "collapse" : "Detail");
    };

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }
}