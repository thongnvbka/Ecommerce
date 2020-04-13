function OrderCommerceViewModel() {
    var self = this;

    //Thêm hoặc Edit
    self.isAdd = ko.observable(false);
    self.isSubmit = ko.observable(true);
    self.isDetailRending = ko.observable(true);

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

        self.codePackage('');
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

    self.viewOrderAdd = function (id) {

        self.resetForm();
        self.searchCustomer();
        self.searchShop();

        if (id === undefined || id === 0 || id === null || id === '') {
            self.isAdd(true);

            $.post("/OrderCommerce/GetOrderAdd", {}, function (result) {
                self.listOrderService(result.listOrderService);
            });
        } else {
            self.isAdd(false);
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

                _.each(result.listPackageView,
                   function (it) {
                       it.cacheTransportCode = it.TransportCode;
                       it.cacheForcastDate = it.ForcastDate;

                       it.TransportCode = ko.observable(it.TransportCode);
                       it.TransportCode.subscribe(function (newValue) {
                           if (it.cacheTransportCode !== newValue) {
                               self.updatePackage(ko.mapping.toJS(it));
                               it.cacheTransportCode = newValue;
                           }
                       });

                       it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                       it.ForcastDate.subscribe(function (newValue) {
                           if (it.cacheForcastDate !== newValue) {
                               self.updatePackage(ko.mapping.toJS(it));
                               it.cacheForcastDate = newValue;
                           }
                       });
                   });

                self.listPackageView(result.listPackageView);

                self.totalPriceService(_.reduce(result.listOrderService,
                    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    0));

                $(".shop-search")
                    .empty()
                    .append($("<option/>").val(result.order.ShopId).text(result.order.ShopName))
                    .val(result.order.ShopId)
                    .trigger("change");

                $(".customer-search-add-order")
                    .empty()
                    .append($("<option/>").val(result.order.CustomerId).text(result.order.CustomerName))
                    .val(result.order.CustomerId)
                    .trigger("change");


                $(".select-view").select2();

                //Chat
                self.viewBoxChat.showChat(self.Id(), self.Code(), self.Type(), 1);

                self.updateCalculator();

                self.orderCodeWarehouse(self.WarehouseDeliveryId(), self.Code());
            });
        }

        self.isDetailRending(false);
        $('#orderCommerceAddModal').modal();
        $('#orderCommerceAddModal')
               .on('hide.bs.modal',
               function (e) {
                   $(".search-list").trigger('click');
               });
        self.isDetailRending(true);
        $(".select-view").select2();
    }

    //Đặt xong Orders
    self.orderSuccess = function () {
        if (self.WarehouseId() === undefined) {
            toastr.error('Warehouse not selected!');
            return;
        }

        $.post("/OrderCommerce/OrderSuccess", { id: self.Id(), status: self.Status() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                $('#orderCommerceAddModal').modal('hide');
                toastr.success(result.msg);

                $(".search-list").trigger('click');
            }
        });
    };

    //Cài đăt cho Detail Orders

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
    }

    //cập nhật Detail Orders 
    self.editDetailOrder = function (item) {
        var isLook = false;

        item.Price = formatVN(item.Price);
        item.Quantity = formatVN(item.Quantity);

        $.post({
            url: "/OrderCommerce/EditDetailOrder",
            data: { orderDetail: item, status: self.Status() },
            success: function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    isLook = false;
                } else {
                    isLook = true;
                    self.listOrderService(result.listOrderService);

                    self.updateCalculator();
                }
            },
            async: false
        });

        return isLook;
    }

    //Cập nhật dịch vụ
    self.updateService = function (data) {
        if (self.isAdd()) {
            var list = _.filter(self.listOrderService(),
                function (item) {
                    if (item.ServiceId === data.ServiceId) {
                        item.Checked = !item.Checked;
                    }
                    return item;
                });
            self.listOrderService([]);
            self.listOrderService(list);
        } else {
            $('#update').modal();
            $.post("/Order/UpdateService", { id: data.Id }, function (result) {
                $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                    if (result == -1) {
                        toastr.error('Orders: ' + ReturnCode(data.Code) + ' does not exist or has been deleted!');
                    }
                    else {
                        self.listOrderService(result.listOrderService);
                        self.totalPriceService(_.reduce(result.listOrderService,
                            function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                            0));
                    }
                });
                $('#update').modal('hide');
            });
        }
    }

    //Thêm mới Detail Orders
    self.addOrderDetail = function () {
        if (self.isAdd()) {
            var obj = {
                isCheck: ko.observable(false),
                Id: (new Date()).getTime(),
                Image: ko.observable(""),
                Name: ko.observable(""),
                Link: ko.observable(""),
                Color: ko.observable(""),
                Size: ko.observable(""),
                Note: ko.observable(""),
                Quantity: ko.observable(),
                Price: ko.observable(),
                TotalPrice: ko.observable(),
                UserNote: ko.observable(""),
                Status: 0
            };

            obj.cssQuantity = ko.observable();
            obj.isSelectedQuantity = ko.observable(false);
            obj.Quantity.subscribe(function (newValue) {
                obj.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(obj.Quantity()) *
                    Globalize('en-US').parseFloat(obj.Price()),
                    'N2'));

                self.updateCalculator();
            });

            obj.cssPrice = ko.observable();
            obj.isSelectedPrice = ko.observable(false);
            obj.Price.subscribe(function (newValue) {
                obj.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(obj.Quantity()) *
                    Globalize('en-US').parseFloat(obj.Price()),
                    'N2'));

                self.updateCalculator();
            });

            obj.isCheck.subscribe(function (newValue) {
                self.totalPriceIsCheck(formatNumbericCN(_
                    .reduce(self.listDetail(),
                        function (count, item) {
                            return count +
                            (item.isCheck() === false
                                ? 0
                                : (Globalize('en-US').parseFloat(item.Price()) *
                                    Globalize('en-US').parseFloat(item.Quantity())));
                        },
                        0),
                    'N2'));
            });

            self.listDetail.push(obj);
            self.initInputMark();
        } else {
            $.post("/OrderCommerce/AddDetailOrder",
            {
                id: self.Id(),
                status: self.Status()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.setOrderDetai(result);
                }
            });
        }
    };

    self.deleteOrderDetail = function (data) {
        if (self.isAdd()) {
            self.listDetail.remove(data);
        } else {
            $.post("/OrderCommerce/DeleteDetailOrder",
            {
                id: data.Id,
                status: self.Status()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.listDetail.remove(data);
                }
            });
        }
    }

    self.updateCalculator = function () {
        self.TotalPrice(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.Status === 1 ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.Quantity()))); }, 0), 'N2'));

        if (self.isAdd()) {

        } else {
            self.TotalShop(formatNumbericCN(_.reduce(self.listContractCode(), function (count, item) { return count + (item.Status === 1 ? 0 : Globalize('en-US').parseFloat(item.TotalPrice())); }, 0), 'N2'));
            self.TotalShopExchange(formatNumbericCN(_.reduce(self.listContractCode(), function (count, item) { return count + ((item.Status === 1 ? 0 : Globalize('en-US').parseFloat(item.TotalPrice())) * Globalize('en-US').parseFloat(self.ExchangeRate())); }, 0), 'N2'));
        }

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
            self.titleTotalCus('Money the company makes up');
            self.CashShortage(formatNumbericCN(cashShortage * (-1), 'N2'));
        } else {
            self.titleTotalCus('Money company interest');
            self.CashShortage(formatNumbericCN(cashShortage, 'N2'));
        }

        self.initInputMark();

        self.totalPriceIsCheck(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.isCheck() === false ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.Quantity()))); }, 0), 'N2'));

        self.initInputMark();
    }

    self.IsExchangeOther = ko.observable(false);
    self.titleExchangeOther = ko.observable("Detail");
    self.showExchangeOther = function () {
        self.IsExchangeOther(!self.IsExchangeOther());
        self.titleExchangeOther(self.IsExchangeOther() ? "collapse" : "Detail");
    };

    self.saveOrder = function () {
        if (self.checkOrder()) {
            $('#update').modal();

            $.post("/OrderCommerce/AddOrder",
            {
                order: {
                    CustomerId: self.CustomerId(),
                    CustomerName: self.CustomerName(),
                    CustomerPhone: self.CustomerPhone(),
                    CustomerEmail: self.CustomerEmail(),
                    CustomerAddress: self.CustomerAddress(),
                    WebsiteName: self.WebsiteName(),
                    ShopId: self.ShopId(),
                    ShopName: self.ShopName(),
                    ShopLink: self.ShopLink(),
                    ExchangeRate: self.ExchangeRate(),
                    WarehouseDeliveryId: self.WarehouseDeliveryId(),
                    WarehouseDeliveryName: self.WarehouseDeliveryName(),
                    WarehouseId: self.WarehouseId(),
                    WarehouseName: self.WarehouseName(),
                    BargainType: self.BargainType(),
                    Note: self.Note(),
                    UserNote: self.UserNote()
                },
                listDetail: self.listDetail(),
                listOrderService: self.listOrderService(),
                payment: self.Payment()
            },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        toastr.success(result.msg);

                        $("#orderCommerceAddModal").modal('hide');
                        $(".search-list").trigger('click');
                    }
                    $('#update').modal('hide');
                });
        }
    }

    self.checkOrder = function () {
        var check = true;
        if (self.CustomerName() === "") {
            toastr.error("Not yet Select customer!");
            return false;
        }

        if (self.ShopName() === "") {
            toastr.error("Shop Can not be empty!");
            return false;
        }

        if (self.listDetail().length === 0) {
            toastr.error("Detail no record!");
            return false;
        }

        return check;
    }

    self.checkDetail = function (data) {
        if (data.Name() === "") {
            toastr.error("Have Detail Orders to empty the product name!");
            return false;
        }
        if (data.Link() === "") {
            toastr.error("Have Detail Orders to empty the product link!");
            return false;
        }

        if (data.Price() === "") {
            toastr.error("Have Detail Orders to empty the product price");
            return false;
        }
        if (data.Quantity() === "") {
            toastr.error("Have Detail Orders to empty the product number!");
            return false;
        }
        if (data.Image() === "") {
            toastr.error("Have Detail Orders to empty the product image!");
            return false;
        }

        return true;
    }

    //Cập nhật kho hàng
    self.WarehouseDeliveryId.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouseVN(), function (item) { return item.Id === newId; });
        if (warehouse !== undefined) {
            self.WarehouseDeliveryName(warehouse.Name);

            if (self.CacheWarehouseDeliveryId() == newId) {
                return;
            }

            if (!self.isAdd()) {
                $('#update').modal();

                $.post("/Order/UpdateOrderWarehouseVn",
                    {
                        id: self.Id(),
                        warehouseDeliveryId: self.WarehouseDeliveryId(),
                        warehouseDeliveryName: self.WarehouseDeliveryName()
                    },
                    function (result) {
                        if (result.status === msgType.error) {
                            toastr.error(result.msg);
                        } else {
                            self.CacheWarehouseDeliveryId(newId);
                            self.orderCodeWarehouse(self.WarehouseDeliveryId(), self.Code());
                        }
                        $('#update').modal('hide');
                    });
            } else {
                self.CacheWarehouseDeliveryId(newId);
            }
        }
    });

    // Hàm lấy thông tin kho khi đã chọn
    self.WarehouseId.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouse(), function (item) { return item.Id === newId; });

        if (warehouse !== undefined) {
            self.WarehouseName(warehouse.Name);

            if (self.CacheWarehouseId() == newId) {
                return;
            }

            if (!self.isAdd()) {
                $('#update').modal();
                $.post("/Order/UpdateOrderWarehouse",
                    {
                        id: self.Id(),
                        warehouseId: self.WarehouseId(),
                        warehouseName: self.WarehouseName()
                    },
                    function (result) {
                        if (result.status === msgType.error) {
                            toastr.error(result.msg);
                        } else {
                            self.CacheWarehouseId(newId);
                        }
                        $('#update').modal('hide');
                    });
            } else {
                self.CacheWarehouseId(newId);
            }
        }
    });

    //Cập nhật khách hàng
    self.CustomerId.subscribe(function (newId) {
        if (newId == "" || newId == 0 || newId == null || newId == undefined) {
            return;
        }

        if (self.CacheCustomerId() == newId) {
            return;
        }

        console.log(self.CacheCustomerId());
        console.log(newId);

        if (!self.isAdd()) {
            $('#update').modal();
            $.post("/OrderCommerce/UpdateCustomer",
                {
                    id: self.Id(),
                    customerId: newId,
                    status: self.Status()
                },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.CacheCustomerId(newId);
                    }
                    $('#update').modal('hide');
                });
        } else {
            self.CacheCustomerId(newId);
        }
    });

    //cập nhật shop
    self.ShopId.subscribe(function (newId) {
        if (newId == "" || newId == 0 || newId == null || newId == undefined) {
            return;
        }

        if (self.CacheShopId() == newId) {
            return;
        }

        if (!self.isAdd()) {
            $('#update').modal();
            $.post("/OrderCommerce/UpdateShop",
            {
                id: self.Id(),
                shopId: newId,
                status: self.Status()
            },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.CacheShopId(newId);
                    }
                    $('#update').modal('hide');
                });
        } else {
            self.CacheShopId(newId);
        }
    });

    //cập nhật thông tin note
    self.Note.subscribe(function (newValue) {
        if (newValue == null || newValue == undefined) {
            return;
        }

        if (self.CacheNote() == newValue) {
            return;
        }

        if (!self.isAdd()) {
            $.post("/OrderCommerce/UpdateNote",
            {
                id: self.Id(),
                note: self.Note(),
                userNote: self.UserNote(),
                status: self.Status()
            },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.CacheNote(newValue);
                    }
                });
        }
    });

    self.UserNote.subscribe(function (newValue) {
        if (newValue == null || newValue == undefined) {
            return;
        }

        if (self.CacheUserNote() == newValue) {
            return;
        }

        if (!self.isAdd()) {
            $.post("/OrderCommerce/UpdateNote",
            {
                id: self.Id(),
                note: self.Note(),
                userNote: self.UserNote(),
                status: self.Status()
            },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.CacheUserNote(newValue);
                    }
                });
        }
    });

    //Hàm cập nhật khách thanh toán
    self.OrderExchange.subscribe(function (newValue) {
        if (self.CacheOrderExchange() == newValue) {
            return;
        }

        if (newValue == '0' || newValue == '') {
            toastr.error('The input value must be greater 0');
            return;
        }

        $.post("/OrderCommerce/UpdateCustomerPay", { id: self.Id(), price: formatVN(newValue), status: self.Status() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
                self.isSubmit(true);
            } else {
                self.updateCalculator();
                self.isSubmit(true);
            }
        });
    });

    self.showContractCode = function () {
        self.addContractCode();
    }

    self.addContractCode = function () {
        self.isSubmit(false);
        $.post("/OrderCommerce/AddContractCodeOrder", { id: self.Id(), contractCode: '', pice: 0, status: self.Status() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
                self.isSubmit(true);
            } else {
                self.setContractCode(result);
                self.isSubmit(true);
            }
        });
        //self.contractCode('');
        //self.piceContractCode(0);
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
                        toastr.error('Can not be empty');
                    }
                });
            });

        self.listContractCode(result.listContractCode);

        self.updateCalculator();
    }

    //Gủi lại kế toán thanh toán
    self.reviewContractCode = function (data) {
        $.post("/OrderCommerce/ReviewContractCodeOrder", { id: data.Id, status: data.Status }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.setContractCode(result);
            }
        });
    };

    //Hàm xóa Contract code
    self.deleteContractCode = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Contract code "' + data.ContractCode() + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/OrderCommerce/DeleteContractCodeOrder", { id: data.Id, status: data.Status }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.setContractCode(result);
                }
            });
        }, function () { });
    };

    //Hàm Edit Contract code
    self.updateContractCodeOrder = function (data) {
        var isLook = false;
        data.TotalPrice = formatVN(data.TotalPrice);

        $('#update').modal();
        $.post({
            url: "/OrderCommerce/EditContractCodeOrder",
            data: { id: data.Id, code: data.ContractCode, totalPrice: data.TotalPrice, status: data.Status },
            success: function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    isLook = false;
                } else {
                    self.setContractCode(result);
                    isLook = true;
                }
                $('#update').modal('hide');
            },
            async: false
        });

        self.updateCalculator();
        return isLook;
    };

    self.CheckbargainType = function () {
        var isOk = true;

        if (!self.isAdd()) {
            $.post("/Order/UpdateOrderBargainType", { orderId: self.Id, bargainType: self.BargainType }, function (data) {
                if (data.status !== msgType.success) {
                    isOk = false;
                    toastr.error(data.msg);
                }
            });
        }
        return isOk;
    };

    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function () {
        self.isShowHistory(!self.isShowHistory());
    }

    self.isShowLog = ko.observable(false);
    self.checkShowLog = function () {
        self.isShowLog(!self.isShowLog());
    }

    //mã vận đơn
    self.codePackage = ko.observable();

    //Hàm Add the tracking code
    self.addPackage = function () {
        self.isSubmit(false);

        if (self.codePackage() === '' || self.codePackage() === null || self.codePackage() === undefined) {
            toastr.error('Mã vận đơn Can not be empty!');
            self.isSubmit(true);
        } else {
            $.post("/Order/AddContractCode", { id: self.Id(), codePackage: self.codePackage() }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                } else {
                    _.each(result.list,
                        function (it) {
                            it.cacheTransportCode = it.TransportCode;
                            it.cacheForcastDate = it.ForcastDate;

                            it.TransportCode = ko.observable(it.TransportCode);
                            it.TransportCode.subscribe(function (newValue) {
                                if (it.cacheTransportCode !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheTransportCode = newValue;
                                }
                            });

                            it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                            it.ForcastDate.subscribe(function (newValue) {
                                if (it.cacheForcastDate !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheForcastDate = newValue;
                                }
                            });
                        });
                    self.listPackageView(result.list);

                    $('.datepicker')
                        .datepicker({
                            autoclose: true,
                            language: 'en',
                            format: 'dd/mm/yyyy',
                            startDate: new Date()
                        });

                    self.isSubmit(true);
                }
            });
            self.codePackage('');
        }
    };

    self.listPackageView = ko.observableArray([]);

    //Hàm Edit mã vận đơn
    self.updatePackage = function (data) {
        $('#update').modal();
        $.post("/Order/EditContractCode", { packageId: data.Id, packageName: data.TransportCode, date: data.ForcastDate }, function (result) {
            $('#update').modal('hide');
        });
    };

    //Hàm xóa mã vận đơn
    self.deletePackage = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Mã vận đơn "' + data.TransportCode() + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/Order/DeleteContractCode", { id: data.Id }, function (result) {
                if (result === -1) {
                    toastr.error('Orders: ' + ReturnCode(data.OrderCode) + ' does not exist or has been deleted!');
                } else {
                    toastr.success('Successfully deleted transport code "' + data.TransportCode() + '"');
                    self.listPackageView(result.list);
                }
            });
        }, function () { });
    };

    self.cssFeeShipBargain = ko.observable('');
    self.isSelectedFeeShipBargain = ko.observable(false);

    self.cssFeeShip = ko.observable('');
    self.isSelectedFeeShip = ko.observable(false);

    self.updateOrder = function () {
        $('#update').modal();
        $.post("/OrderCommerce/UpdateOrder",
        {
            id: self.Id(),
            feeShip: formatVN(self.FeeShip()),
            status: self.Status()
        },
        function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);

                self.cssFeeShip('error');
                self.isSelectedFeeShip(true);
            } else {
                self.cssFeeShip('');
                self.isSelectedFeeShip(false);
                self.listOrderService(result.listOrderService);

                self.updateCalculator();
            }
            $('#update').modal('hide');
        });
        return true;
    }


    self.initInputMark = function () {
        $('input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.renderedHandler = function (elements, data) {
        $('#editableBtn' + data.Id).click(function (e) {
            e.stopPropagation();
            $('#editableImg' + data.Id).editable('toggle');
        });

        $('#editableBtl' + data.Id).click(function (e) {
            e.stopPropagation();
            $('#editableLink' + data.Id).editable('toggle');
        });
    }

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search-add-order").empty().trigger("change");
        $(".customer-search-add-order")
            .select2({
                ajax: {
                    url: "Customer/GetCustomerSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    if (self.CustomerName() !== repo.text) {
                        self.CustomerName(repo.text);
                        self.CustomerEmail(repo.email);
                        self.CustomerPhone(repo.phone);
                        self.CustomerAddress(repo.address);
                    }

                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'vi'
            });
    };

    //hàm lấy thông tin shop
    self.searchShop = function () {
        $(".shop-search").empty().trigger("change");
        $(".shop-search")
            .select2({
                ajax: {
                    url: "Shop/GetShopSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            " + repo.url + "<br/>\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    if (self.ShopName() !== repo.text) {
                        self.ShopName(repo.text);
                        self.ShopLink(repo.url);
                        self.WebsiteName(repo.website);
                    }

                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'vi'
            });
    };

    //Thêm mới shop
    self.showAddShop = function () {
        $('#shopAdd').modal();
    }

    self.submitShop = function () {
        if (self.ShopName() === "") {
            toastr.error("shop name Can not be empty!");
            return false;
        }

        if (self.ShopLink() === "") {
            toastr.error("Link shop nhận Can not be empty!");
            return false;
        }

        self.isSubmit(false);

        $.post("/Shop/AddFash", {
            name: self.ShopName(),
            link: self.ShopLink()
        }, function (result) {
            if (result.status === msgType.success) {
                toastr.success(result.msg);
                $("#shopAdd").modal('hide');

                self.ShopId(result.shop.Id);
                self.ShopName(result.shop.Name);
                self.ShopLink(result.shop.Url);

                $(".shop-search")
                    .empty()
                    .append($("<option/>").val(result.shop.Id).text(result.shop.Name))
                    .val(result.shop.Id)
                    .trigger("change");

            } else if (result.status === msgType.error) {
                toastr.error(result.msg);
                self.ShopId("");
                self.ShopName("");
                self.ShopLink("");
            } else {
                toastr.warning(result.msg);
                $("#shopAdd").modal('hide');

                self.ShopId(result.shop.Id);
                self.ShopName(result.shop.Name);
                self.ShopLink(result.shop.Url);

                $(".shop-search")
                    .empty()
                    .append($("<option/>").val(result.shop.Id).text(result.shop.Name))
                    .val(result.shop.Id)
                    .trigger("change");
            }

            self.isSubmit(true);
        });
    }

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }
}