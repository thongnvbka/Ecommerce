var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

function autoresize() {
    setTimeout(function () {
        autosize(document.querySelectorAll('textarea'));
    },
        300);
}

function OrderWaitViewModel(orderDetailViewModel, chatViewModel, depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;

    //============================================================= Tìm kiếm ===================================================
    //các biến tìm kiếm
    self.keyword = ko.observable('');
    self.status = ko.observable();
    self.CountTicket = ko.observable();
    self.systemId = ko.observable();
    self.dateStart = ko.observable();
    self.dateEnd = ko.observable();
    self.userId = ko.observable();
    self.customerId = ko.observable();
    self.active = ko.observable();
    self.checkExactCode = ko.observable(false);
    self.checkRetail = ko.observable(false);

    self.listStatus = ko.observableArray([]);
    self.listSystem = ko.observableArray([]);
    self.listSystemRender = ko.observableArray([]);
    self.listOrder = ko.observableArray([]);
    self.listUser = ko.observableArray([]);
    self.listHistory = ko.observableArray([]);
    self.listUserOffice = ko.observableArray([]);
    self.listOrderService = ko.observableArray([]);
    self.listDetail = ko.observableArray([]);
    //self.listShop = ko.observableArray([]);
    self.listWarehouseVN = ko.observableArray([]);

    self.customerToAddress = ko.observableArray([]);
    self.customerFormAddress = ko.observableArray([]);


    self.warehouseVNId = ko.observable();

    // Hàm lấy thông tin kho khi đã chọn
    self.warehouseVNId.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouseVN(), function (item) { return item.Id === newId; });
        if (warehouse !== undefined) {

            if (self.warehouseVNId() === self.WarehouseDeliveryId()) {
                return;
            }
            $('#update').modal();
            self.WarehouseDeliveryId(warehouse.Id);
            self.WarehouseDeliveryName(warehouse.Name);

            $.post("/Order/UpdateOrderWarehouseVn",
                {
                    id: self.Id(),
                    warehouseDeliveryId: warehouse.Id,
                    warehouseDeliveryName: warehouse.Name
                },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        //toastr.success(result.msg);
                    }
                    $('#update').modal('hide');
                });
        }
    });

    //Hàm lấy thông tin nhân viên
    self.searchUser = function () {
        $(".user-search")
            .select2({
                ajax: {
                    url: "User/GetUserSearch",
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
                                        <img class='w-40 mr10 mt5' src='/images/" + repo.avatar + "_50x50_1'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    return repo.text;
                },
                placeholder: "All",
                allowClear: true,
                language: 'en'
            });
    };

    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search")
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
                    return repo.text;
                },
                placeholder: "All",
                allowClear: true,
                language: 'en',
            });
    };

    //hàm lấy thông tin shop
    self.searchShop = function () {
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
                    if (repo.id === "" || repo.id === undefined) {

                    } else {
                        if (self.ShopName() !== repo.text) {
                            self.ShopName(repo.text);
                            self.ShopLink(repo.url);
                            self.WebsiteName(repo.website);
                        }
                    }
                    return repo.text;
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    //Hàm tìm kiếm 
    self.search = function (page) {
        window.page = page;

        self.isSubmit(true);
        self.isRending(false);

        if (self.active() === 'order-wait') {
            $.post("/OrderAwait/GetOrderWait",
                {
                    page: page,
                    pageSize: pagesize,
                    keyword: self.keyword(),
                    status: self.status() == undefined ? -1 : self.status(),
                    systemId: self.systemId(),
                    userId: self.userId(),
                    customerId: self.customerId(),
                    dateStart: self.dateStart(),
                    dateEnd: self.dateEnd(),
                    checkExactCode: self.checkExactCode(),
                    checkRetail: self.checkRetail()
                },
                function (data) {
                    total = data.totalRecord;
                    //self.listOrder(data.listOrder);

                    var list = [];
                    _.each(data.listOrder, function (item) {
                        item.CssRead = ko.observable('no-read');
                        item.CheckRead = function () {
                            item.CssRead('yes-read');
                            return true;
                        }

                        list.push(item);
                    });

                    self.listOrder(list);

                    self.paging();
                    self.isRending(true);

                    self.totalAwait(data.totalAwait);
                    if (data.totalAwait < 2) {
                        self.cogAwait(data.totalAwait);
                    }
                });
        }

        if (self.active() === 'order-wait-new') {
            $.post("/OrderAwait/GetOrderWaitNew",
                {
                    page: page,
                    pageSize: pagesize,
                    keyword: self.keyword(),
                    status: self.status() == undefined ? -1 : self.status(),
                    systemId: self.systemId(),
                    userId: self.userId(),
                    customerId: self.customerId(),
                    dateStart: self.dateStart(),
                    dateEnd: self.dateEnd(),
                    checkExactCode: self.checkExactCode(),
                    checkRetail: self.checkRetail()
                },
                function (data) {
                    total = data.totalRecord;
                    //self.listOrder(data.listOrder);

                    var list = [];
                    _.each(data.listOrder, function (item) {
                        item.CssRead = ko.observable('no-read');
                        item.CheckRead = function () {
                            item.CssRead('yes-read');
                            return true;
                        }

                        list.push(item);
                    });

                    self.listOrder(list);

                    self.paging();
                    self.isRending(true);

                    self.totalAwait(data.totalAwait);
                    if (data.totalAwait < 2) {
                        self.cogAwait(data.totalAwait);
                    }
                });
        }

        if (self.active() === 'order-cus') {
            var status = self.status() == undefined ? -1 : self.status();
            var type = -1;
            if (status != -1) {
                var slipt = status.split('.');

                status = slipt[1];
                type = slipt[0];
            }

            $.post("/OrderAwait/GetOrderCustomerCare",
                {
                    page: page,
                    pageSize: pagesize,
                    keyword: self.keyword(),
                    status: status,
                    type: type,
                    systemId: self.systemId(),
                    userId: self.userId(),
                    customerId: self.customerId(),
                    dateStart: self.dateStart(),
                    dateEnd: self.dateEnd(),
                    checkExactCode: self.checkExactCode(),
                    checkRetail: self.checkRetail()
                },
                function (data) {
                    total = data.totalRecord;

                    var list = [];
                    var complain;
                    var claim;
                    _.each(data.listOrder, function (item) {
                        item.CssRead = ko.observable('no-read');
                        item.CheckRead = function () {
                            item.CssRead('yes-read');
                            return true;
                        }
                        item.CountTicket = 0;
                        item.SumRealTotal = 0;
                        complain = _.find(data.listComplain, function (it) {
                            return it['OrderId'] == item.Id;
                        });
                        claim = _.find(data.listClaim, function (it) {
                            return it['OrderId'] == item.Id;
                        });
                        if (complain != undefined) {
                            item.CountTicket = complain['Count'];
                        }
                        if (claim != undefined) {
                            item.SumRealTotal = claim['RealTotal'];
                        }

                        list.push(item);
                    });

                    self.listOrder(list);

                    //self.listOrder(data.listOrder);
                    self.paging();
                    self.isRending(true);
                });
        }
    };

    //Hàm chọn tab để tìm kiếm
    self.clickSearch = function (data, event) {
        if (event.which) {
            //Actually clicked
            $('#nav-tab' + self.systemId()).click();
        } else {
            //Triggered by code
            self.search(page);
        }
    }

    self.clickTab = function (tab) {
        self.systemId(tab);
        self.search(1);
        $(".select-view").select2();
    }

    self.totalAwait = ko.observable(0); //Sum số Orders chưa nhận
    self.cogAwait = ko.observable(2); //Số Orders được nhận
    //Hàm load lại dữ liệu trên các tab
    self.renderSystem = function (name) {
        self.listSystemRender([]);
        self.listStatus([]);
        self.active(name);

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });

        self.keyword('');
        self.status(-1);
        self.systemId(-1);
        self.dateStart(undefined);
        self.dateEnd(undefined);
        self.userId(null);
        self.customerId(null);
        self.checkExactCode(false);
        self.checkRetail(false);

        $.post("/Purchase/GetRenderSystem", { active: name }, function (data) {
            if (name === 'order-cus') {
                var group = _.groupBy(data.listStatus, function (item) {
                    return item.Type;
                });

                var array = _.values(group).reverse();
                var listStatus = [];

                _.forEach(array, function (item) {
                    if (item[0].Type === -1) {
                        listStatus.push({
                            Label: 'All',
                            Group: item
                        });
                    } else {
                        if (item[0].Type === window.orderType.deposit) {
                            listStatus.push({
                                Label: 'Consignment note',
                                Group: item
                            });
                        } else {
                            listStatus.push({
                                Label: ' Order',
                                Group: item
                            });
                        }
                    }
                });
                self.listStatus(listStatus);

            } else {
                self.listStatus(data.listStatus);
            }
            self.listSystem(data.listSystem);
            self.listSystemRender(data.listSystem);
            $('.nav-tabs').tabdrop();
            $(".select-view").select2();

            $('#daterange-btn').daterangepicker({
                locale: {
                    applyLabel: "Agree",
                    cancelLabel: "All",
                    fromLabel: "From",
                    toLabel: "To",
                    customRangeLabel: "Option",
                    firstDay: 1
                },
                ranges: {
                    'Today' : [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    '7 days ago': [moment().subtract(6, 'days'), moment()],
                    '30 days ago': [moment().subtract(29, 'days'), moment()],
                    'This week': [moment().startOf('week'), moment().endOf('week')],
                    'This month': [moment().subtract(0, 'month').startOf('month'), moment().subtract(0, 'month').endOf('month')]
                },
                startDate: moment().subtract(29, 'days'),
                endDate: moment()
            },
                function (start, end) {
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
                });

            $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
                //do something, like clearing an input
                $('#daterange-btn span').html('Created date');
                self.dateStart(null);
                self.dateEnd(null);
            });

            self.searchCustomer();
            self.searchUser();
            self.searchShop();
            self.search(1);
        });
    }

    //============================================== Các hàm hiển thị Detail, chat ============================================================

    //hiển thị xem Detail Orders
    self.showOrderDetail = function (orderId) {
        if (orderDetailViewModel) {
            orderDetailViewModel.viewOrderDetail(orderId);
            return;
        }
    }

    self.showOrderDepositDetail = function (orderId) {
        if (depositDetailViewModel) {
            depositDetailViewModel.showModalDialog(orderId);
            return;
        }
    }

    self.showOrderCommerceDetail = function (orderId) {
        if (orderCommerceDetailViewModel) {
            orderCommerceDetailViewModel.showModal(orderId);
            return;
        }
    }

    //hiển thị chat
    self.showChatOrder = function (data) {
        if (chatViewModel) {
            chatViewModel.showChat(data.Id, data.Code, data.Type);

            $('#chatModal')
                .on('hide.bs.modal',
                function (e) {
                    self.search(page);
                });
        }
    }

    //=============================================== Phần xử lý Orders báo giá ===============================================================
    self.isRending = ko.observable(false);
    self.isSubmit = ko.observable(true);
    self.isDetailRending = ko.observable(false);

    self.mess = ko.observable();
    self.totalPriceService = ko.observable(0);

    //model
    //Thông tin chung
    self.Id = ko.observable();
    self.Code = ko.observable();
    self.Status = ko.observable(0);
    self.Type = ko.observable();
    self.CustomerCareUserId = ko.observable();
    self.Created = ko.observable(new Date());
    self.LastUpdate = ko.observable();
    self.CacheIsRetail = ko.observable(false);
    self.IsRetail = ko.observable(false);
    //Tính toán
    self.TotalPrice = ko.observable(0);
    self.FeeShip = ko.observable(0);
    self.CacheFeeShip = ko.observable(0);
    self.TotalPriceCustomer = ko.observable(0);
    self.PaidShop = ko.observable(0);
    self.TotalExchange = ko.observable(0);
    self.CashShortage = ko.observable(0);
    self.PriceBargain = ko.observable(0);
    self.Total = ko.observable(0);

    //khách hàng
    self.LevelName = ko.observable();
    self.CustomerId = ko.observable();
    self.CustomerName = ko.observable();
    self.CustomerEmail = ko.observable();
    self.CustomerPhone = ko.observable();
    //thông tin thêm
    self.WebsiteName = ko.observable();
    self.CacheShopId = ko.observable();
    self.ShopId = ko.observable();
    self.ShopName = ko.observable();
    self.ShopLink = ko.observable();
    self.ExchangeRate = ko.observable(0);
    self.WarehouseDeliveryId = ko.observable();
    self.WarehouseDeliveryName = ko.observable();
    self.ProductNo = ko.observable(0);
    self.LinkNo = ko.observable(0);
    self.DepositPercent = ko.observable(0);

    //note
    self.Note = ko.observable();
    self.UserNote = ko.observable();

    self.ShopId.subscribe(function (newId) {
        if (self.ShopId() == self.CacheShopId() || self.ShopId() === '' || self.ShopId() === 0 || self.ShopId() === undefined || self.Id() === '' || self.Id() === 0 || self.Id() === undefined) {
            return;
        }

        $('#update').modal();

        $.post("/OrderAwait/UpdateShop",
            {
                id: self.Id(),
                shopId: self.ShopId()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    //toastr.success(result.msg);
                    self.CacheShopId(self.ShopId());
                }
                $('#update').modal('hide');
            });
    });

    self.setModel = function (data) {
        self.Id(data.Id);
        self.Code(data.Code);
        self.Status(data.Status);
        self.Type(data.Type + '');
        self.CustomerCareUserId(data.CustomerCareUserId);
        self.Created(data.Created);
        self.LastUpdate(data.LastUpdate);
        self.LevelName(data.LevelName);
        self.CustomerId(data.CustomerId);
        self.CustomerName(data.CustomerName);
        self.CustomerEmail(data.CustomerEmail);
        self.CustomerPhone(data.CustomerPhone);
        self.WebsiteName(data.WebsiteName);
        self.CacheShopId(data.ShopId);
        self.ShopId(data.ShopId);
        self.ShopName(data.ShopName);
        self.ShopLink(data.ShopLink);
        self.ExchangeRate(data.ExchangeRate);
        self.Note(data.Note);
        self.UserNote(data.UserNote);
        self.WarehouseDeliveryId(data.WarehouseDeliveryId);
        self.WarehouseDeliveryName(data.WarehouseDeliveryName);
        self.warehouseVNId(data.WarehouseDeliveryId);
        self.ProductNo(data.ProductNo);
        self.LinkNo(data.LinkNo);
        self.DepositPercent(data.DepositPercent);

        self.TotalPrice(data.TotalPrice);
        self.CacheFeeShip(data.FeeShip);
        self.FeeShip(data.FeeShip);
        self.PaidShop(data.PaidShop);
        self.TotalExchange(data.TotalExchange);
        self.Total(data.Total);
        self.CacheIsRetail(data.IsRetail);
        self.IsRetail(data.IsRetail);

        $(".select-view").select2();
    }

    self.resetModel = function () {
        self.Id("");
        self.Code("");
        self.Status(0);
        self.Type("");
        self.CustomerCareUserId("");
        self.Created(new Date());
        self.LastUpdate("");
        self.LevelName("");
        self.CustomerId("");
        self.CustomerName("");
        self.CustomerEmail("");
        self.CustomerPhone("");
        self.WebsiteName("");
        self.CacheShopId("");
        self.ShopId("");
        self.ShopName("");
        self.ShopLink("");
        self.ExchangeRate(0);
        self.Note("");
        self.UserNote("");
        self.WarehouseDeliveryId("");
        self.WarehouseDeliveryName("");
        self.CheckValue("");
        self.linkData([]);

        self.TotalPrice(0);
        self.CacheFeeShip(0);
        self.FeeShip(0);
        self.TotalPriceCustomer(0);
        self.PaidShop(0);
        self.TotalExchange(0);
        self.CashShortage(0);
        self.PriceBargain(0);
        self.Total(0);
        self.ProductNo(0);
        self.LinkNo(0);
        self.DepositPercent(0);
        self.CacheIsRetail(false);
        self.IsRetail(false);

        self.customerToAddress([]);
        self.customerFormAddress([]);
        self.listOrderService([]);
        self.listDetail([]);
        self.listOrderView([]);

        $(".shop-search").empty();
    }

    self.IsRetail.subscribe(function (newValue) {
        if (newValue != self.CacheIsRetail()) {
            $.ajax({
                type: 'POST',
                url: "/OrderAwait/UpdateRetail",
                data: { orderId: self.Id(), isRetail: newValue },
                success: function (result) {
                    $.post("/Order/GetOrderDetail", { orderId: self.Id() }, function (result) {
                        if (result == -1) {
                            //toastr.error('Orders: ' + ReturnCode(data.Code) + ' does not exist or has been deleted!');
                        }
                        else {
                            self.listOrderService(result.listOrderService);
                            self.totalPriceService(_.reduce(result.listOrderService,
                                function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                                0));

                            self.initInputMark();

                            self.calculatePrice();
                        }
                    });
                },
                async: false
            });

            self.CacheIsRetail(newValue);
        }
    });

    //Hàm xử lý Orders báo giá
    self.successDetail = ko.observable(0);
    self.viewEditDetail = function (id) {
        self.isDetailRending(false);
        self.resetModel();
        autoresize();

        self.listWarehouseVN(window.listWarehouseVN);

        $.post("/Order/GetOrderDetail", { orderId: id }, function (result) {
            self.successDetail(0);

            if (result.status !== msgType.error) {
                self.setModel(result.order);
                self.customerToAddress(result.toAddress);
                self.customerFormAddress(result.formAddress);
                //self.listShop(result.listShop);
                self.mess(result.mess);

                //Lấy danh sách Detail Orders
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderListDetail",
                    data: { orderId: id },
                    success: function (resultListDetail) {
                        var list = self.bindingOrderDetail(resultListDetail);
                        self.listDetail(list);
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

                self.listOrderService(result.listOrderService);
                self.totalPriceService(_.reduce(result.listOrderService,
                    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    0));

                self.initInputMark();

                self.calculatePrice();

                //Chat
                self.viewBoxChat.showChat(self.Id(), self.Code(), self.Type(), 1);

                $(".shop-search").empty().append($("<option/>").val(result.order.ShopId).text(result.order.ShopName)).val(result.order.ShopId).trigger("change");
            }
            else {
                toastr.error(result.msg);
                return;
            }
            self.isDetailRending(true);

            $('.datepicker')
                .datepicker({
                    autoclose: true,
                    language: 'en',
                    format: 'dd/mm/yyyy',
                    startDate: new Date()
                });

            $('#orderAddOrEditAwaitModal').modal();

            console.log(3);

            $('#orderAddOrEditAwaitModal')
                .on('hide.bs.modal',
                function (e) {
                    self.search(page);
                });
        });
    };

    //Cập nhật thông phí ship nội địa trung quốc
    self.updateFeeShip = function () {
        if (self.FeeShip() !== null && self.FeeShip() !== '' && self.FeeShip() !== undefined && self.Id() !== '') {
            $.post("/OrderAwait/UpdateFeeShip",
                {
                    id: self.Id(),
                    value: formatVN(self.FeeShip() + '')
                },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.listOrderService(result.listOrderService);
                        self.totalPriceService(_.reduce(result.listOrderService,
                            function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                            0));
                        self.calculatePrice();
                    }
                });
        }
    }


    //Hàm gửi báo giá cho khách hàng
    self.orderWait = function () {
        $.post("/OrderAwait/OrderWait", { id: self.Id(), type: self.Type() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                $('#orderAddOrEditAwaitModal').modal('hide');
                toastr.success(result.msg);
                $(".search-list").trigger('click');
            }
        });
    };

    self.initInputMark = function () {
        $('#orderAddOrEditAwaitModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('#contractCodeModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });

        $('#orderAddOrEditAwaitModal input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('#contractCodeModal input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.calculatePrice = function () {
        self.TotalPrice(formatNumbericCN(_.reduce(self.listDetail(), function (count, item) { return count + (item.Status !== 0 ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.QuantityBooked()))); }, 0), 'N2'));

        //Tiền khách
        var priceCustomer = Globalize('en-US').parseFloat(self.TotalPrice());
        var shipCustomer = Globalize('en-US').parseFloat(self.FeeShip());
        var totalCustomer = priceCustomer + (shipCustomer === '' ? 0 : shipCustomer);
        self.TotalPriceCustomer(formatNumbericCN(totalCustomer, 'N2'));

        //Tiền công ty
        self.PaidShop(0);

        //Tiền bargain được
        self.PriceBargain(formatNumbericCN((totalCustomer), 'N2'));

        //Tính lại tiền việt nam đồng
        var totalPrice = Globalize('en-US').parseFloat(self.TotalPrice());
        var exchangeRate = Globalize('en-US').parseFloat(self.ExchangeRate());

        self.TotalExchange(formatNumbericCN((totalPrice * exchangeRate), 'N2'));
        self.Total(formatNumbericCN((totalPrice * exchangeRate + self.totalPriceService()), 'N2'));
        self.CashShortage(formatNumbericCN((totalPrice * exchangeRate + self.totalPriceService()), 'N2'));
    }

    //=============================================== Detail Orders =========================================================================
    //Cập nhật Detail Orders
    self.editDetailOrder = function (data) {
        data.Price = formatVN(data.Price + '');
        data.QuantityBooked = formatVN(data.QuantityBooked + '');
        data.Quantity = formatVN(data.Quantity + '');

        $('#update').modal();
        var check = false;
        $.ajax({
            type: 'POST',
            url: "/Order/UpdateOrderDetailAwait",
            data: { orderDetailMode: data },
            success: function (result) {
                if (result.status === msgType.success) {
                    self.listOrderService(result.listOrderService);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    self.calculatePrice();
                    check = true;
                }
                else {
                    toastr.error(result.msg);
                }
            },
            async: false
        });

        $('#update').modal('hide');
        return check;
    };

    //Đặt được hàng trong Detail Orders
    self.orderDetailSuccess = function (data) {
        self.isDetailRending(false);

        $.post("/Order/OrderDetailSuccess", { id: data.Id }, function (result) {
            if (result.status !== msgType.success) {
                toastr.error(result.msg);
            }
            else {
                self.listOrderService(result.listOrderService);
                self.totalPriceService(_.reduce(result.listOrderService,
                    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    0));


                $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (resultReset) {
                    self.successDetail(0);

                    //Lấy danh sách Detail Orders
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderListDetail",
                        data: { orderId: data.OrderId },
                        success: function (resultListDetail) {
                            var list = self.bindingOrderDetail(resultListDetail);
                            self.listDetail(list);
                        },
                        async: false
                    });

                    self.listOrderService(result.listOrderService);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    self.calculatePrice();
                    self.isDetailRending(true);
                });

                toastr.success(result.msg);
            }
            self.isDetailRending(true);
        });
    };

    self.FeeShip.subscribe(function (newValue) {
        if (self.CacheFeeShip() != newValue) {
            self.updateFeeShip();
            self.CacheFeeShip(newValue);
        }
    });

    //Hủy đặt hàng trong Detail Orders
    self.orderDetailCancel = function (data) {
        self.isDetailRending(false);

        $.post("/Order/OrderDetailCancel", { id: data.Id }, function (result) {

            if (result.status !== msgType.success) {
                toastr.error(result.msg);
            }
            else {
                self.listOrderService(result.listOrderService);
                self.totalPriceService(_.reduce(result.listOrderService,
                    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    0));

                $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                    self.successDetail(0);

                    //Lấy danh sách Detail Orders
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderListDetail",
                        data: { orderId: data.OrderId },
                        success: function (resultListDetail) {
                            var list = self.bindingOrderDetail(resultListDetail);
                            self.listDetail(list);
                        },
                        async: false
                    });

                    self.listOrderService(result.listOrderService);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    self.calculatePrice();

                    self.isDetailRending(true);
                });
                toastr.success(result.msg);
            }
            self.isDetailRending(true);
        });
    };

    self.bindingOrderDetail = function (result) {
        var list = [];
        _.each(result.listOrderDetail, function (item) {
            if (item.Status === 0) {
                self.successDetail(self.successDetail() + 1);
            }

            item.IsView = ko.observable(item.IsView);
            item.clickRead = function () {
                item.IsView(true);

                $.post("/OrderAwait/IsView", { id: item.Id }, function (result) { });
                return true;
            }

            item.CacheQuantityBooked = ko.observable(formatNumbericCN(item.QuantityBooked, 'N0'));
            item.CacheQuantity = ko.observable(formatNumbericCN(item.Quantity, 'N0'));
            item.CachePrice = ko.observable(formatNumbericCN(item.Price, 'N2'));

            item.Quantity = ko.observable(formatNumbericCN(item.Quantity, 'N0'));
            item.QuantityBooked = ko.observable(formatNumbericCN(item.QuantityBooked, 'N0'));
            item.Price = ko.observable(formatNumbericCN(item.Price, 'N2'));
            item.ExchangeRate = ko.observable(formatNumbericCN(item.ExchangeRate, 'N2'));
            item.ExchangePrice = ko.observable(formatNumbericCN(item.ExchangePrice, 'N2'));
            item.TotalPrice = ko.observable(formatNumbericCN(item.TotalPrice, 'N2'));
            item.TotalExchange = ko.observable(formatNumbericCN(item.TotalExchange, 'N2'));
            item.UserNote = ko.observable(item.UserNote);
            item.Note = ko.observable(item.Note);
            item.Name = ko.observable(item.Name);
            item.Size = ko.observable(item.Size);
            item.Color = ko.observable(item.Color);
            item.ShopName = ko.observable(item.ShopName);
            item.Link = ko.observable(item.Link);
            item.Image = ko.observable(item.Image);

            item.QuantityBooked.subscribe(function (newValue) {
                if (newValue != item.CacheQuantityBooked()) {

                    if (self.editDetailOrder(ko.mapping.toJS(item))) {
                        item.CacheQuantityBooked(newValue);
                        item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                    } else {
                        item.QuantityBooked(item.CacheQuantityBooked());
                    }
                }
            });

            item.Quantity.subscribe(function (newValue) {
                if (newValue != item.CacheQuantity()) {
                    //if (Globalize('en-US').parseFloat(item.QuantityBooked()) > Globalize('en-US').parseFloat(item.Quantity())) {
                    item.CacheQuantityBooked(newValue);
                    item.QuantityBooked(newValue);
                    //}

                    if (self.editDetailOrder(ko.mapping.toJS(item))) {
                        item.CacheQuantity(newValue);
                        item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                    } else {
                        item.Quantity(item.CacheQuantity());
                    }
                }
            });

            item.Price.subscribe(function (newValue) {
                if (newValue != item.CachePrice()) {
                    if (self.editDetailOrder(ko.mapping.toJS(item))) {
                        item.CachePrice(newValue);
                        item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) *
                            Globalize('en-US').parseFloat(item.Price()),
                            'N2'));
                    } else {
                        item.Price(item.CachePrice());
                    }
                }
            });

            item.UserNote.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Note.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Size.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Color.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Name.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.ShopName.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Link.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            item.Image.subscribe(function (newValue) {
                self.editDetailOrder(ko.mapping.toJS(item));
            });

            list.push(item);
        });

        return list;
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

    self.listOrderView = ko.observableArray([]);
    //Tách Orders
    self.separationOrder = function () {
        $('#update').modal();

        $.post("/OrderAwait/SeparationOrder", { id: self.Id() }, function (result) {
            if (result.status === msgType.success) {
                toastr.success(result.msg);

                self.listOrderView(result.listOrderView);

                //$('#orderAddOrEditAwaitModal').modal('hide');
                $(".search-list").trigger('click');
            }
            else {
                toastr.error(result.msg);
            }

            $('#update').modal('hide');
        });
    }

    //=============================================== Thêm shop =================================================================================
    //Thêm mới shop
    self.showAddShop = function () {
        $('#shopAdd').modal();
    }

    self.submitShop = function () {
        if (self.ShopName() === "") {
            toastr.error("Shop name can not be empty!");
            return false;
        }

        if (self.ShopLink() === "") {
            toastr.error("Link shop cannot be empty!");
            return false;
        }

        self.isSubmit(false);

        $.post("/Shop/AddFash",
            {
                name: self.ShopName(),
                link: self.ShopLink()
            },
            function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#shopAdd").modal('hide');

                    self.ShopId(result.shop.Id);
                    self.ShopName(result.shop.Name);
                    self.ShopLink(result.shop.Url);
                    self.WebsiteName(result.shop.Website);
                    //self.listShop(result.listShop);

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
                    self.WebsiteName("");
                } else {
                    toastr.warning(result.msg);
                    $("#shopAdd").modal('hide');

                    self.ShopId(result.shop.Id);
                    self.ShopName(result.shop.Name);
                    self.ShopLink(result.shop.Url);
                    self.WebsiteName(result.shop.Website);

                    $(".shop-search")
                        .empty()
                        .append($("<option/>").val(result.shop.Id).text(result.shop.Name))
                        .val(result.shop.Id)
                        .trigger("change");
                }

                self.isSubmit(true);
            });
    };


    self.imgView = ko.observable();
    self.showImg = function (data) {
        self.imgView(data.Image());
        $("#shopImg").modal();
    }

    //Hàm show modal Cancel Orders
    self.orderCloseDetailView = function () {
        self.orderCloseDetailCommon(self.Id(), self.Type(), self.Code(), self.Status());
    };

    //Hàm show modal Cancel Orders
    self.orderCloseDetailCommon = function (id, type, code, status) {
        self.isSubmit(true);

        self.reasonId(id);
        self.reasonStatus(status);
        self.reasonCode(ReturnCode(code));
        self.reasonType(type);
        $("#orderCloseModal").modal();
    };

    //=============================================== Nhận đơn báo giá, phân đơn báo giá, hủy đơn báo giá =======================================

    //hàm hủy đơn báo giá
    self.reasonId = ko.observable();
    self.reasonStatus = ko.observable();
    self.reasonCode = ko.observable();
    self.reasonType = ko.observable();
    self.reasonNote = ko.observable();

    self.orderClose = function (data) {

        console.log(data);
        self.isSubmit(true);

        self.reasonId("");
        self.reasonStatus("");
        self.reasonCode("");
        self.reasonType("");
        self.reasonNote("");

        self.reasonId(data.Id);
        self.reasonStatus(data.Status);
        self.reasonCode(ReturnCode(data.Code));
        self.reasonType(data.Type);
        $("#orderCloseModal").modal();
    };

    //Hàm hủy đơn báo giá
    self.submitCancelOrder = function () {
        self.isSubmit(false);
        if (self.reasonNote() == undefined || self.reasonNote() === "") {
            toastr.error("Reason for cancellation Orders cannot be empty");
            self.isSubmit(true);
        }
        else if (self.reasonNote().length < 10) {
            toastr.error("Reason for cancellation Orders must not be less than 10 characters ");
            self.isSubmit(true);
        }
        else {
            $.post("/OrderAwait/OrderCancelCustomerCase", { id: self.reasonId(), type: self.reasonType(), note: self.reasonNote(), status: self.reasonStatus() }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#orderCloseModal").modal("hide");
                    $("#orderAddOrEditAwaitModal").modal("hide");
                    $(".search-list").trigger('click');
                    self.search(1);
                    self.isSubmit(true);
                }
                else {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                }
            });
        }
    };

    //Hàm lấy nhiều Orders về xử lý
    self.receivePurchaseMultiOrder = function () {
        self.isSubmit(false);
        $.post("/OrderAwait/ReceivePurchaseMultiOrderCustomerCase", {}, function (result) {
            if (result.status === msgType.success) {
                self.isSubmit(true);

                var str = '';
                var p = '';
                _.each(result.listOrderCode, function (item) {
                    str += p + ReturnCode(item);
                    p = ', ';
                });

                toastr.success(result.msg);
                $(".search-list").trigger('click');
                self.search(page);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });
    }

    //Hàm show modal Divide Orders for staff
    self.assignedId = ko.observable();
    self.assignedStatus = ko.observable();
    self.assignedType = ko.observable();
    self.assignedCode = ko.observable();
    self.assignedUserId = ko.observable();

    self.assignedOrder = function (data) {
        self.isSubmit(true);
        self.assignedId(data.Id);
        self.assignedStatus(data.Status);
        self.assignedType(data.Type);

        self.assignedCode(ReturnCode(data.Code));

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });
        $("#orderAssignedModal").modal();
    };

    //Hàm phân Orders cho Staff handling
    self.submitAssignedOrder = function () {
        self.isSubmit(false);

        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.assignedUserId(); });

        $.post("/OrderAwait/AssignedOrderCustomerCase", { orderId: self.assignedId(), orderType: self.assignedType(), user: userSelect, status: self.assignedStatus() }, function (result) {
            if (result.status === msgType.success) {
                $("#orderAssignedModal").modal("hide");
                toastr.success(result.msg);
                $(".search-list").trigger('click');
                self.search(page);
                self.isSubmit(true);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });
    };

    //Hàm show modal chuyển Orders cho nhân viên khác
    self.orderReplyModal = function (data) {
        self.isSubmit(true);

        self.setModel(data);

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });
        $('#orderReplyModal').modal();
    };

    //Hàm chuyển Orders cho nhân viên khác
    self.submitOrderReplyModal = function () {
        self.isSubmit(false);

        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.CustomerCareUserId(); });

        $.post("/OrderAwait/OrderReplyCustomerCase", { orderId: self.Id(), user: userSelect }, function (result) {
            if (result.status === msgType.success) {
                $('#orderReplyModal').modal('hide');
                toastr.success(result.msg);
                $(".search-list").trigger('click');
                self.isSubmit(true);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });
    };

    self.updateService = function (data) {
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

                    self.initInputMark();

                    self.calculatePrice();
                }
            });
        });
    }

    //chuyền link cho Orders khác

    self.listOrderForward = ko.observableArray([]);
    self.listOrderForwardAll = ko.observableArray([]);
    self.CheckValue = ko.observable();
    self.linkData = ko.observableArray([]);
    self.searchLink = ko.observable();

    self.searchLink.subscribe(function (newValue) {
        self.CheckValue('');
        newValue = newValue.trim().toUpperCase().replace("ORD", "");

        var list = [];
        _.each(self.listOrderForwardAll(),
            function (item) {
                if (item.Code.includes(newValue)) {
                    list.push(item);
                }
            });

        self.listOrderForward(list);
    });

    self.forwardLink = function (data) {
        self.searchLink('');
        self.CheckValue('');
        self.listOrderForwardAll([]);

        $.post("/OrderAwait/GetOrderForward", { customerId: self.CustomerId(), orderId: data.OrderId }, function (result) {
            self.linkData([]);
            self.linkData.push(data);
            self.listOrderForward(result.listOrder);
            self.listOrderForwardAll(result.listOrder);
        });
        $('#forwardLink').modal();
    }

    self.submitForwardLink = function () {
        self.isSubmit(false);
        if (self.CheckValue() == null || self.CheckValue() == '' || self.CheckValue() == undefined) {
            self.isSubmit(true);
            toastr.error('You have not selected Orders to transfer link!');
        } else {
            $.post("/OrderAwait/ForwardLink", { orderOldId: self.Id(), orderNewId: self.CheckValue(), orderDetailId: self.linkData()[0].Id }, function (result) {
                if (result.status !== msgType.success) {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                } else {
                    toastr.success(result.msg);

                    if (result.checkCancel) {
                        toastr.error('Orders ' + ReturnCode(self.Code()) + ' Canceled due to no link in Orders');

                        $('#forwardLink').modal('hide');
                        $('#orderAddOrEditAwaitModal').modal('hide');
                    } else {
                        self.isDetailRending(false);
                        $.post("/Order/GetOrderDetail", { orderId: self.Id() }, function (result) {

                            //Lấy danh sách Detail Orders
                            $.ajax({
                                type: 'POST',
                                url: "/Order/GetOrderListDetail",
                                data: { orderId: self.Id() },
                                success: function (resultListDetail) {
                                    var list = self.bindingOrderDetail(resultListDetail);
                                    self.listDetail(list);
                                },
                                async: false
                            });

                            self.listOrderService(result.listOrderService);
                            self.totalPriceService(_.reduce(result.listOrderService,
                                function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                                0));
                            self.calculatePrice();
                            self.isDetailRending(true);
                            self.isSubmit(true);
                            $('#forwardLink').modal('hide');
                        });
                    }
                }
            });
        }
    }

    //Export Excel danh sách Orders
    self.ExcelGetOrderCustomerCare = function () {
        var status = self.status() == undefined ? -1 : self.status();
        var type = -1;
        if (status != -1) {
            var slipt = status.split('.');

            status = slipt[1];
            type = slipt[0];
        }
        $.redirect("/OrderAwait/ExcelGetOrderCustomerCare",
            {
                page: page,
                pageSize: pagesize,
                keyword: self.keyword(),
                status: status,
                type: type,
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd(),
                checkExactCode: self.checkExactCode(),
                checkRetail: self.checkRetail()
            }
            , "POST");
    }

    //============================================================= Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    //Hàm khởi tạo phân trang
    self.paging = function () {

        var listPage = [];

        page = page <= 0 ? 1 : page;
        pageTotal = Math.ceil(total / pagesize);
        page > 3 ? self.pageStart(true) : self.pageStart(false);
        page > 4 ? self.pageNext(true) : self.pageNext(false);
        pageTotal - 2 > page ? self.pageEnd(true) : self.pageEnd(false);
        pageTotal - 3 > page ? self.pagePrev(true) : self.pagePrev(false);

        var start = (page - 2) <= 0 ? 1 : (page - 2);
        var end = (page + 2) >= pageTotal ? pageTotal : (page + 2);

        for (var i = start; i <= end; i++) {
            listPage.push({ Page: i });
        }

        self.listPage(listPage);
        self.pageTitle("Show <b>" + (((page - 1) * pagesize) + 1) + "</b> to <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> of <b>" + total + "</b> Record" );
    }

    //Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        } 
    }

    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function () {
        self.isShowHistory(!self.isShowHistory());
    }

    self.isShowLog = ko.observable(false);
    self.listLog = ko.observableArray([]);
    self.checkShowLog = function () {
        self.isShowLog(!self.isShowLog());
    }
}