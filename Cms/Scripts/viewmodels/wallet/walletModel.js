function WalletModel(packageDetailModal, orderDetailModal, orderServiceOtherModal, depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);
    self.statesGroupId = _.groupBy(window["states"] ? window.states : [], 'id');
    self.entrepots = ko.observableArray(window["entrepots"] ? window.entrepots : []);

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.type = ko.observable("0");
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.orderServiceId = ko.observable(null);
    self.keyword = ko.observable("");
    self.orderServices = ko.observableArray([]);

    self.timeType = ko.observable(null);
    self.entrepotId = ko.observable(null);
    self.mode = ko.observable(3);
    self.createdNo = ko.observable(0);
    self.inStockNo = ko.observable(0);
    self.waitImportNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.addForm = ko.observable(null);

    self.changeMode = function(mode) {
        if (mode !== self.mode()) {
            self.mode(mode);
            self.search(1);
        }
    }

    self.changeOrderServiceId = function () {
        self.search(1);
    }

    self.changeTimeType = function () {
        self.search(1);
    }

    // Subscribe
    var warehouseIdPathFirst = true;
    self.warehouseIdPath.subscribe(function () {
        if (warehouseIdPathFirst) {
            warehouseIdPathFirst = false;
            return;
        }
        self.search(1);
    });

    var statusFirst = true;
    self.status.subscribe(function () {
        if (statusFirst) {
            statusFirst = false;
            return;
        }
        self.search(1);
    });


    self.type.subscribe(function () {
        self.search(1);
    });

    self.changeEntrepotId = function () {
        self.search(1);
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerWallet").html(self.totalRecord() === 0 ? "There are no packing" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " packing");

        $("#pagerWallet").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerWallet").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);

        $.get("/wallet/search",
            {
                orderServiceId: self.orderServiceId(),
                timeType: self.timeType(),
                entrepotId: self.entrepotId(),
                mode: self.mode(),
                type: self.type(),
                warehouseIdPath: self.warehouseIdPath(),
                userId: self.userId(),
                status: self.status(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                var statesGroup = _.groupBy(window.states, "id");

                _.each(data.items,
                    function (item) {
                        item.statusText = statesGroup[item.status + ''][0].name;
                        item.statusClass = item.status === 1
                            ? 'label label-warning'
                            : item.status === 2
                            ? 'label label-success'
                            : item.status === 3
                            ? 'label label-info'
                            : item.status === 5 ? 'label label-primary' : 'label label-danger';

                        item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");

                        item.orderServiceData = _.filter(self.orderServices(),
                            function(it) {
                                return item.orderServices.indexOf(";" + it.serviceId + ";") >= 0;
                            });

                        item.note = ko.observable(item.note);
                        item.note.subscribe(function (newValue) {
                            self.noteWallet(ko.mapping.toJS(item));
                        });
                    });

                self.createdNo(formatNumberic(data.mode["createdNo"]));
                self.inStockNo(formatNumberic(data.mode["inStockNo"]));
                self.waitImportNo(formatNumberic(data.mode["waitImportNo"]));
                self.allNo(formatNumberic(data.mode["allNo"]));

                self.items(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }
    //Cập nhật note bao
    self.noteWallet = function (data) {
        var isLook = false;
        var input = {
            id: data.id,
            note: data.note
        };
        $.post("/wallet/updateWalletNote",
                input,
                function (rs) {
                    self.isLoading(false);
                    if (rs.status < 0) {
                        toastr.error(rs.msg);
                        return;
                    }

                    toastr.success(rs.msg);
                });
        return isLook;
    }

    self.callback = function () {
        self.search(1);
    }

    self.addBill = function () {
        if (self.addForm() == null) {
            self.addForm(new WalletAddModel(self.callback, packageDetailModal, orderDetailModal, orderServiceOtherModal));
            ko.applyBindings(self.addForm(), $("#walletAddOrEdit")[0]);
        }

        self.addForm().showAddForm();
    }

    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new WalletAddModel(self.callback, packageDetailModal, orderDetailModal, orderServiceOtherModal));
            ko.applyBindings(self.addForm(), $("#walletAddOrEdit")[0]);
        }

        self.addForm().setForm(data);
    }

    self.showRouteWallet = function (walletId) {
        routeWalletModelView.show(walletId);
    }
    //Xuất file Excel cho Kho
    self.ExcelTrackingPackageWallet = function () {

        $.redirect("/Wallet/ExcelTrackingPackageWallet", {
            entrepotId: self.entrepotId(),
            mode: self.mode(),
            type: self.type(),
            warehouseIdPath: self.warehouseIdPath(),
            userId: self.userId(),
            status: self.status(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword()
        },
        "POST");
    }
    $(function () {

        var services = _.map(window["orderServices"] ? window.orderServices : [],
            function(s) {
                s.checked = ko.observable(false);
                return s;
            });
        self.orderServices(services);

        $('#Wallet-date-btn').daterangepicker({
            ranges: {
                'Today' : [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '7 days ago': [moment().subtract(6, 'days'), moment()],
                '30 days ago': [moment().subtract(29, 'days'), moment()],
                'This month': [moment().startOf('month'), moment().endOf('month')],
                'Last month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            },
            startDate: moment().subtract(29, 'days'),
            endDate: moment()
        },
            function (start, end) {
                self.fromDate(start.format());
                self.toDate(end.format());
                self.search(1);
                $('#Wallet-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        self.search(1);
    });
}

// Bind PackageDetail
var packageDetailModelView = new PackageDetail(window.orderPackageStates);
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var orderServiceOtherModelView = new OrderServiceOtherModel();
ko.applyBindings(orderServiceOtherModelView, $("#orderServiceOtherModal")[0]);

var walletDetailModalView = new WalletDetailModel(window.allWarehouse, window.walletStates, window.orderPackageStates, packageDetailModelView, orderDetailViewModel);
ko.applyBindings(walletDetailModalView, $("#walletDetailModal")[0]);

// Cập nhật cân nặng cho bao hàng
var updateWeightModelView = new UpdateWeightModel();
ko.applyBindings(updateWeightModelView, $("#updateWalletWeightModal")[0]);

// Bind PackageDetail
var routeWalletModelView = new RouteTransportModel(walletDetailModalView);
ko.applyBindings(routeWalletModelView, $("#routeWalletModal")[0]);

var modelView = new WalletModel(packageDetailModelView, orderDetailViewModel, orderServiceOtherModelView, depositDetailViewModel, orderCommerceDetailViewModel);

ko.applyBindings(modelView, $("#wallet")[0]);

updateWeightModelView.callback = function () {
    modelView.search(modelView.currentPage());
}