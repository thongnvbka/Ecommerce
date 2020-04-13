function OrderCommerceDetailViewModel(modelId) {
    var self = this;

    self.isDetailRending = ko.observable(true);
    self.viewBoxChat = new ChatViewModel();

    //Các biến dữ liệu
    self.listOrderService = ko.observableArray([]);
    self.listDetail = ko.observableArray([]);
    self.listWarehouse = ko.observableArray(window.listWarehouse);
    self.listWarehouseVN = ko.observableArray(window.listWarehouseVN);
    self.listContractCode = ko.observableArray([]);
    self.listBargainType = ko.observableArray(window.listBargainType);
    self.listHistory = ko.observableArray([]);
    self.listOrderExchage = ko.observableArray([]);
    self.listLog = ko.observableArray([]);
    self.mess = ko.observable();
    self.totalPriceIsCheck = ko.observable(0);
    self.isDisableFeeShipBargain = ko.observable(true);
    self.totalPriceService = ko.observable();
    self.listPackageView = ko.observableArray([]);

    //Các biến Orders
    self.Id = ko.observable();
    self.Code = ko.observable("");
    self.Status = ko.observable(0);
    self.Type = ko.observable(0);
    self.Created = ko.observable(new Date());
    self.LastUpdate = ko.observable("");
    self.Note = ko.observable("");
    self.UserNote = ko.observable("");
    self.ExchangeRate = ko.observable(0);
    self.WebsiteName = ko.observable("");
    self.ShopId = ko.observable();
    self.ShopName = ko.observable("");
    self.ShopLink = ko.observable("");
    self.LevelName = ko.observable("");

    self.TotalPrice = ko.observable(0);
    self.PaidShop = ko.observable(0);
    self.FeeShip = ko.observable(0);
    self.FeeShipBargain = ko.observable(0);
    self.TotalPriceCustomer = ko.observable(0);
    self.TotalShop = ko.observable(0);
    self.PriceBargain = ko.observable(0);
    self.TotalExchange = ko.observable(0);
    self.CacheOrderExchange = ko.observable("0");
    self.OrderExchange = ko.observable("0");
    self.Total = ko.observable(0);
    self.titleTotalCus = ko.observable(0);
    self.CashShortage = ko.observable(0);
    self.orderExchangeOther = ko.observable(0);
    self.TotalShopExchange = ko.observable(0);

    self.CustomerId = ko.observable();
    self.CustomerName = ko.observable("");
    self.CustomerPhone = ko.observable("");
    self.CustomerEmail = ko.observable("");
    self.CustomerAddress = ko.observable("");

    self.WarehouseDeliveryId = ko.observable("");
    self.WarehouseDeliveryName = ko.observable("");
    self.WarehouseId = ko.observable("");
    self.WarehouseName = ko.observable("");
    self.BargainType = ko.observable("0");
    self.Payment = ko.observable();

    self.CacheWarehouseDeliveryId = ko.observable("");
    self.CacheWarehouseId = ko.observable("");
    self.CacheCustomerId = ko.observable("");
    self.CacheShopId = ko.observable("");
    self.CacheNote = ko.observable("");
    self.CacheUserNote = ko.observable("");

    self.resetForm = function () {
        self.ExchangeRate(formatNumbericCN(window.exchangeRate));
        self.listOrderService([]);
        self.listDetail([]);
        self.listContractCode([]);
        self.listHistory([]);
        self.listOrderExchage([]);
        self.listLog([]);
        self.listOrderService([]);
        self.mess('');
        self.totalPriceIsCheck(0);
        self.totalPriceService(0);

        self.CacheWarehouseDeliveryId("");
        self.CacheWarehouseId("");
        self.CacheCustomerId("");
        self.CacheShopId("");
        self.CacheNote("");
        self.CacheUserNote("");
        self.CacheOrderExchange("0");

        self.Id();
        self.Code("");
        self.Status(0);
        self.Type(0);
        self.Created(new Date());
        self.LastUpdate("");
        self.Note("");
        self.UserNote("");
        self.WebsiteName("");
        self.ShopId();
        self.ShopName("");
        self.ShopLink("");
        self.LevelName("");

        self.TotalPrice(0);
        self.PaidShop(0);
        self.FeeShip(0);
        self.FeeShipBargain(0);
        self.TotalPriceCustomer(0);
        self.TotalShop(0);
        self.PriceBargain(0);
        self.TotalExchange(0);
        self.OrderExchange("0");
        self.Total(0);
        self.titleTotalCus(0);
        self.CashShortage(0);
        self.orderExchangeOther(0);
        self.TotalShopExchange(0);

        self.CustomerId();
        self.CustomerName("");
        self.CustomerPhone("");
        self.CustomerEmail("");
        self.CustomerAddress("");

        self.WarehouseDeliveryId("");
        self.WarehouseDeliveryName("");
        self.WarehouseId("");
        self.WarehouseName("");
        self.BargainType("0");
        self.Payment();
    }

    self.setForm = function (data) {
        self.CacheWarehouseDeliveryId(data.WarehouseDeliveryId);
        self.CacheWarehouseId(data.WarehouseId);
        self.CacheCustomerId(data.CustomerId);
        self.CacheShopId(data.ShopId);
        self.CacheNote(data.Note);
        self.CacheUserNote(data.UserNote);

        self.Id(data.Id);
        self.Code(data.Code);
        self.Status(data.Status);
        self.Type(data.Type);
        self.Created(data.Created);
        self.LastUpdate(data.LastUpdate);
        self.Note(data.Note);
        self.UserNote(data.UserNote);
        self.ExchangeRate(formatNumbericCN(data.ExchangeRate));
        self.WebsiteName(data.WebsiteName);
        self.ShopId(data.ShopId);
        self.ShopName(data.ShopName);
        self.ShopLink(data.ShopLink);
        self.LevelName(data.LevelName);

        self.CustomerId(data.CustomerId);
        self.CustomerName(data.CustomerName);
        self.CustomerPhone(data.CustomerPhone);
        self.CustomerEmail(data.CustomerEmail);
        self.CustomerAddress(data.CustomerAddress);

        self.WarehouseDeliveryId(data.WarehouseDeliveryId);
        self.WarehouseDeliveryName(data.WarehouseDeliveryName);
        self.WarehouseId(data.WarehouseId);
        self.WarehouseName(data.WarehouseName);
        self.BargainType(data.BargainType + '');

        self.FeeShip(data.FeeShip);
        self.FeeShipBargain(data.FeeShipBargain);
    }

    //Hàm hiển thị Detail Orders
    self.showModal = function (id) {
        self.resetForm();

        $.post("/OrderCommerce/GetOrderDetail", { id: id }, function (result) {
            self.setForm(result.order);
            self.listOrderService(result.listOrderService);
            self.setOrderDetai(result);
            self.listHistory(result.listHistory);
            self.listOrderExchage(result.listOrderExchage);
            self.listLog(result.listLog);
            self.mess(result.mess);
            self.setContractCode(result);
            self.CacheOrderExchange(formatNumbericCN(result.orderExchange));
            self.OrderExchange(formatNumbericCN(result.orderExchange));
            self.listPackageView(result.listPackageView);

            self.totalPriceService(_.reduce(result.listOrderService,
                function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                0));

            self.updateCalculator();

            self.viewBoxChat.showChat(result.order.Id, result.order.Code, result.order.Type, 1);
        });

        self.isDetailRending(false);
        $('#orderCommerceDetailModal').modal();
        self.isDetailRending(true);
    }

    self.updateCalculator = function () {
        self.TotalPrice(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.Status === 1 ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.Quantity()))); }, 0), 'N2'));
        
        self.TotalShop(formatNumbericCN(_.reduce(self.listContractCode(), function (count, item) { return count + (item.Status === 1 ? 0 : Globalize('en-US').parseFloat(item.TotalPrice())); }, 0), 'N2'));
        self.TotalShopExchange(formatNumbericCN(_.reduce(self.listContractCode(), function (count, item) { return count + ((item.Status === 1 ? 0 : Globalize('en-US').parseFloat(item.TotalPrice())) * Globalize('en-US').parseFloat(self.ExchangeRate())); }, 0), 'N2'));

        self.orderExchangeOther(_.reduce(self.listOrderExchage(), function (count, item) { return count + item.TotalPrice; }, 0));

        //Tiền khách
        var priceCustomer = Globalize('en-US').parseFloat(self.TotalPrice());
        var shipCustomer = Globalize('en-US').parseFloat(self.FeeShip());
        var totalCustomer = priceCustomer + shipCustomer;
        self.TotalPriceCustomer(formatNumbericCN(totalCustomer, 'N2'));

        //Tiền công ty
        var totalCompany = Globalize('en-US').parseFloat(self.TotalShop());
        var shipCompany = Globalize('en-US').parseFloat(self.FeeShipBargain());
        if (totalCompany !== 0) {
            if (totalCompany < shipCompany) {
                self.FeeShipBargain(0);
                shipCompany = 0;
            }
            var priceCompany = totalCompany - shipCompany;

            if (priceCompany > priceCustomer) {
                shipCompany += priceCompany - priceCustomer;
                self.FeeShipBargain(formatNumbericCN(shipCompany, 'N2'));
                priceCompany = priceCustomer;
            }

            self.PaidShop(formatNumbericCN(priceCompany, 'N2'));
        } else {
            self.TotalShop(0);
            self.PaidShop(0);
            self.FeeShipBargain(0);
        }

        //Tiền bargain được
        var priceBargain = totalCustomer - totalCompany;
        if (priceBargain < 0) {
            priceBargain = 0;
        }

        self.PriceBargain(formatNumbericCN(priceBargain, 'N2'));

        //Tính lại tiền việt nam đồng
        var totalPrice = Globalize('en-US').parseFloat(self.TotalPrice());
        var exchangeRate = Globalize('en-US').parseFloat(self.ExchangeRate());

        self.TotalExchange(formatNumbericCN((totalPrice * exchangeRate), 'N2'));
        self.Total(formatNumbericCN((totalPrice * exchangeRate + self.totalPriceService()), 'N2'));

        var cashShortage = (Globalize('en-US').parseFloat(self.OrderExchange()) - (Globalize('en-US').parseFloat(self.TotalShopExchange()) + self.orderExchangeOther()));
        if (cashShortage < 0) {
            self.titleTotalCus('Company money compensated');
            self.CashShortage(formatNumbericCN(cashShortage * (-1), 'N2'));
        } else {
            self.titleTotalCus('Money company words');
            self.CashShortage(formatNumbericCN(cashShortage, 'N2'));
        }

        self.totalPriceIsCheck(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.isCheck() === false ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.Quantity()))); }, 0), 'N2'));
    }

    self.setOrderDetai = function (result) {
        var list = [];
        _.each(result.listOrderDetail,
            function (item) {
                item.isCheck = ko.observable(false);

                item.cachePrice = item.Price;
                item.cacheQuantityBooked = item.QuantityBooked;

                item.Quantity = ko.observable(formatNumbericCN(item.Quantity, 'N0'));
                item.QuantityBooked = ko.observable(formatNumbericCN(item.QuantityBooked, 'N0'));
                item.QuantityActuallyReceived = ko.observable(formatNumbericCN(item.QuantityActuallyReceived, 'N0'));
                item.Price = ko.observable(formatNumbericCN(item.Price, 'N2'));
                item.ExchangeRate = ko.observable(formatNumbericCN(item.ExchangeRate, 'N2'));
                item.ExchangePrice = ko.observable(formatNumbericCN(item.ExchangePrice, 'N2'));
                item.TotalPrice = ko.observable(formatNumbericCN(item.TotalPrice, 'N2'));
                item.TotalExchange = ko.observable(formatNumbericCN(item.TotalExchange, 'N2'));
                item.UserNote = ko.observable(item.UserNote);
                item.Image = ko.observable(item.Image),
                item.Name = ko.observable(item.Name),
                item.Link = ko.observable(item.Link),
                item.Color = ko.observable(item.Color),
                item.Size = ko.observable(item.Size),
                item.Note = ko.observable(item.Note),

                item.cssQuantity = ko.observable();
                item.isSelectedQuantity = ko.observable(false);
                item.Quantity.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        item.isSelectedQuantity(true);
                        item.cssQuantity('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (item.cacheQuantityBooked !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.editDetailOrder(ko.mapping.toJS(item))) {
                                item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.Quantity()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                                item.cacheQuantityBooked = Globalize('en-US').parseFloat(newValue);
                            } else {
                                item.isSelectedQuantity(true);
                                item.cssQuantity('error');
                            }
                        } else {
                            item.isSelectedQuantity(false);
                            item.cssQuantity('');
                            item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.Quantity()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                        }
                    }
                });

                item.cssPrice = ko.observable();
                item.isSelectedPrice = ko.observable(false);

                item.Price.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        item.isSelectedPrice(true);
                        item.cssPrice('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (item.cachePrice !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.editDetailOrder(ko.mapping.toJS(item))) {
                                item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.Quantity()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                                item.cachePrice = Globalize('en-US').parseFloat(newValue);
                            } else {
                                item.isSelectedPrice(true);
                                item.cssPrice('error');
                            }
                        } else {
                            item.isSelectedPrice(false);
                            item.cssPrice('');
                            item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.Quantity()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                        }
                    }
                });

                item.UserNote.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.isCheck.subscribe(function (newValue) {
                    self.totalPriceIsCheck(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.isCheck() === false ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.QuantityBooked()))); }, 0), 'N2'));
                });

                item.Note.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.Name.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.Link.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.Image.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.Color.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                item.Size.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                list.push(item);
            });

        self.listDetail(list);
        self.updateCalculator();

        self.orderCodeWarehouse(self.WarehouseDeliveryId(), self.Code());
    }

    self.setContractCode = function (result) {
        if (result.listContractCode.length === 0) {
            self.isDisableFeeShipBargain(true);
        }
        // danh sách Contract
        _.each(result.listContractCode,
            function (it) {
                it.cacheContractCode = it.ContractCode;
                it.cacheTotalPrice = it.TotalPrice;
                if (it.TotalPrice > 0) {
                    self.isDisableFeeShipBargain(false);
                }

                //định nghĩa cho biến knockout
                it.ContractCode = ko.observable(it.ContractCode);
                it.TotalPrice = ko.observable(formatNumbericCN(it.TotalPrice, 'N2'));

                it.cssTotalPrice = ko.observable();
                it.isSelectedTotalPrice = ko.observable(false);
                it.isDisableTotalPrice = ko.observable(true);

                if (it.ContractCode()) {
                    it.isDisableTotalPrice(false);
                }

                it.TotalPrice.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        it.isSelectedTotalPrice(true);
                        it.cssTotalPrice('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (it.cacheTotalPrice !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.updateContractCodeOrder(ko.mapping.toJS(it))) {
                                it.cacheTotalPrice = Globalize('en-US').parseFloat(newValue);
                            } else {
                                it.isSelectedTotalPrice(true);
                                it.cssTotalPrice('error');
                            }
                        } else {
                            it.isSelectedTotalPrice(false);
                            it.cssTotalPrice('');
                        }
                    }
                });

                it.cssContractCode = ko.observable();
                it.isSelectedContractCode = ko.observable(false);
                it.ContractCode.subscribe(function (newValue) {
                    if (newValue) {
                        it.isDisableTotalPrice(false);
                        if (it.cacheContractCode !== newValue) {
                            if (self.updateContractCodeOrder(ko.mapping.toJS(it))) {
                                it.cacheContractCode = newValue;
                            } else {
                                it.isDisableTotalPrice(true);
                                it.isSelectedContractCode(true);
                                it.cssContractCode('error');
                            }
                        } else {
                            it.isDisableTotalPrice(false);
                            it.isSelectedContractCode(false);
                            it.cssContractCode('');
                        }
                    } else {
                        it.isSelectedContractCode(true);
                        it.isDisableTotalPrice(true);
                        it.cssContractCode('error');
                        toastr.error('cannot be empty');
                    }
                });
            });

        self.listContractCode(result.listContractCode);

        self.updateCalculator();
    }

    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function () {
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
        self.titleExchangeOther(self.IsExchangeOther() ? "cosapse" : "Detail");
    };

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }
}