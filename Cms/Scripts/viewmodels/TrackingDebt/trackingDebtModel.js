function OrderExchangeDetailModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.orderId = ko.observable(null);
    self.orderCode = ko.observable(null);
    self.mode = ko.observable(null);
    self.items = ko.observableArray([]);

    self.show = function (orderId, orderCode) {
        self.orderId(orderId);
        self.orderCode(orderCode);
        self.isLoading(false);
        self.search();

        $("#orderExchangeDetailModal").modal("show");
    }

    self.changeMode = function(mode) {
        self.mode(mode);
        self.search();
    }

    self.search = function () {
        self.isLoading(true);
        $.get("/TrackingDebt/GetOrderExchange",
            {
                id: self.orderId(),
                mode: self.mode()
            },
            function(data) {
                self.isLoading(false);
                self.items(data.items);
                self.mode(data.mode);
            });
    }
}

function OrderOtherServiceModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.orderId = ko.observable(null);
    self.orderCode = ko.observable(null);
    self.items = ko.observableArray([]);

    self.show = function (orderId, orderCode) {
        self.orderId(orderId);
        self.orderCode(orderCode);
        self.isLoading(false);
        self.search();

        $("#orderOtherServiceModal").modal("show");
    }

    self.search = function () {
        self.isLoading(true);
        $.get("/TrackingDebt/GetOrderServiceOther",
            {
                id: self.orderId()
            },
            function (items) {
                self.isLoading(false);
                self.items(items);
            });
    }
}

function TrackingDebtModel(orderDetailModal, orderExchangeDetailModal, orderOtherServiceModal) {
    var self = this;

    self.isLoading = ko.observable(false);

    // filter
    self.listWarehouse = ko.observableArray([]);
    self.listOrderStatus = ko.observableArray([]);
    self.listDepositStatus = ko.observableArray([]);
    self.listOrderType = ko.observableArray([]);
    self.moneys = ko.observableArray([]);
    self.warehouse = ko.observable(window.warehouse);
    self.orderStatus = ko.observable(window.orderStatus);
    self.depositStatus = ko.observable(window.depositStatus);
    self.orderType = ko.observable(window.orderType);

    self.orders = ko.observableArray([]);
    self.keyword = ko.observable("");

    self.searchType = ko.observable(null);
    self.searchTypeText = ko.observable('Select');
    self.searchPlaceholder = ko.observable('Enter keyword..');
    self.searchTypes = ko.observableArray([
        { id: null, name: 'Select', placeholder: 'Enter keyword..' },
        { id: 0, name: 'Single code', placeholder: 'Enter single code..' },
        { id: 1, name: 'Guest telephone number', placeholder: 'Enter your phone number..' },
        { id: 2, name: 'Email', placeholder: 'Enter email..' }
    ]);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.changeCheck = function(data) {
        data.checked(!data.checked());
        self.search(1);
    }

    self.changeSearchType = function (data) {
        self.searchType(data.id);
        self.searchPlaceholder(data.placeholder);
        self.searchTypeText(data.name);
        self.search(1);
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerTrackingDebt").html(self.totalRecord() === 0 ? "There is not any order" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " order");

        $("#pagerTrackingDebt").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerTrackingDebt").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);

        var statusTexts = _.map(_.filter(self.listOrderStatus(), function (it) { return it.checked() }), "id");
        var statusText = statusTexts.length === 0 ? "" : ';' + _.join(statusTexts, ';') + ';';

        var statusDepositTexts = _.map(_.filter(self.listDepositStatus(), function (it) { return it.checked() }), "id");
        var statusDepositText = statusDepositTexts.length === 0 ? "" : ';' + _.join(statusDepositTexts, ';') + ';';

        var warehousesTexts = _.map(_.filter(self.listWarehouse(), function (it) { return it.checked() }), "id");
        var warehouseIdText = warehousesTexts.length === 0 ? "" : ';' + _.join(warehousesTexts, ';') + ';';

        var orderTypeTexts = _.map(_.filter(self.listOrderType(), function (it) { return it.checked() }), "id");
        var orderTypeText = orderTypeTexts.length === 0 ? "" : ';' + _.join(orderTypeTexts, ';') + ';';

        var moneyTexts = _.map(_.filter(self.moneys(), function (it) { return it.checked() }), "id");
        var moneyText = moneyTexts.length === 0 ? "" : ';' + _.join(moneyTexts, ';') + ';';

        $.get("/TrackingDebt/Search",
            {
                keyword: self.keyword(),
                statusDepositText: statusDepositText,
                statusText: statusText,
                searchType: self.searchType(),
                moneyText: moneyText,
                warehouseIdText: warehouseIdText,
                orderTypeText: orderTypeText,
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

            _.each(data.orders,
                function (o) {
                    _.each(data.services[o.id + ''], function(s) {
                        o[s.serviceId] = s;
                    });
                });

                self.orders(data.orders);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    self.showOrderDetail = function (data) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(data.id);
            return;
        }
    }

    self.showOrderExchange = function (id, orderCode) {
        if (orderExchangeDetailModal) {
            orderExchangeDetailModal.show(id, orderCode);
            return;
        }
    }

    self.showOrderOtherService = function (id, orderCode) {
        if (orderOtherServiceModal) {
            orderOtherServiceModal.show(id, orderCode);
            return;
        }
    }

    self.search(1);

    self.changeCheckMoney = function(data) {
        data.checked(!data.checked());

        // Đang nợ tiền khách
        if (data.id === 2 && data.checked()) {
            self.moneys()[3].checked(false);
        } else if (data.id === 3 && data.checked()) {
            self.moneys()[2].checked(false);
        }

        self.search(1);
    }

    // Hoàn tiền Orders
    self.refund = function (data) {
        console.log(data);
        var amount = 0;
        if (data.debt < 0)
            amount = Math.abs(data.debt);

        orderPayedAndRefundViewModel.show(0, data.id, data.code, amount);
    }

    // Thu thêm tiền Orders
    self.payed = function (data) {
        var amount = 0;
        if (data.debt > 0)
            amount = data.debt;

        orderPayedAndRefundViewModel.show(1, data.id, data.code, amount);
    }

    $(function () {
        _.each(window.orderTypes,
            function (t) {
                t.checked = ko.observable(t.checked);
            });
        _.each(window.warehouses,
            function (t) {
                t.checked = ko.observable(t.checked);
            });
        _.each(window.orderStatuss,
            function (t) {
                t.checked = ko.observable(t.checked);
            });

        _.each(window.depositStatuss,
            function (t) {
                t.checked = ko.observable(t.checked);
            });

        self.moneys([
            {
                id: 0,
                name: "Guests have money",
                checked: ko.observable(false)
            },
            {
                id: 1,
                name: "There are refunds for guests",
                checked: ko.observable(false)
            },
            {
                id: 2,
                name: "Owe money",
                checked: ko.observable(false)
            },
            {
                id: 3,
                name: "Guests are owed the company",
                checked: ko.observable(false)
            }
        ]);

        self.listWarehouse(window.warehouses);
        self.listOrderStatus(window.orderStatuss);
        self.listDepositStatus(window.depositStatuss);
        self.listOrderType(window.orderTypes);

        //var drop = null;
        $('.dropdown-toggle-custome').click(function () {
            var $this = $(this);
            var drop = $this.parent().find(".dropdown-menu");
            drop.slideToggle(10);

            $(document).click(function (e) {
                if (drop && $(e.target).parents().filter($this).length === 0 && $(e.toElement).filter($this).length === 0
                    && $(e.target).parents().filter('.size18.isCheckbox').length === 0 && $(e.toElement).filter(('.size18.isCheckbox')).length === 0) {
                    drop.slideUp(10);
                }
            });
        });
    });
}

// Bind OrderExchangeDetailModel
var orderExchangeDetailModelView = new OrderExchangeDetailModel();
ko.applyBindings(orderExchangeDetailModelView, $("#orderExchangeDetailModal")[0]);

// Bind OrderOtherServiceModel
var orderOtherServiceModelView = new OrderOtherServiceModel();
ko.applyBindings(orderOtherServiceModelView, $("#orderOtherServiceModal")[0]);

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// Bind OrderPayedAndRefundModel
var orderPayedAndRefundViewModel = new OrderPayedAndRefundModel();
ko.applyBindings(orderPayedAndRefundViewModel, $("#orderPayedAndRefundModal")[0]);

var modelView = new TrackingDebtModel(orderDetailViewModel, orderExchangeDetailModelView, orderOtherServiceModelView);
ko.applyBindings(modelView, $("#trackingDebt")[0]);

orderPayedAndRefundViewModel.callback = function() {
    modelView.search(modelView.currentPage());
}