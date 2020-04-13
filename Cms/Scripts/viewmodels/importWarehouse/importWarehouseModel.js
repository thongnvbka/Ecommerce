function ImportWarehouseModel(packageDetailModal, orderDetailModal, walletDetailModal, orderServiceOtherModal) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);
    

    self.warehouseIdPath = ko.observable("");
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.addForm = ko.observable(null);

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

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerImportWarehouse").html(self.totalRecord() === 0 ? "There is not any "+(window.viewMode === 0 ? "goods package" : "goods sack") : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() +(window.viewMode === 0 ? "goods package" : "goods sack"));

        $("#pagerImportWarehouse").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerImportWarehouse").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function(currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/importwarehouse/search",
        {
            warehouseIdPath: self.warehouseIdPath(),
            userId: self.userId(),
            viewMode: window.viewMode,
            status: self.status(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword(),
            currentPage: self.currentPage(),
            recordPerPage: self.recordPerPage()
        }, function(data) {
            self.isLoading(false);

            var statesGroup = _.groupBy(window.states, "id");

            var first = null;
            _.each(data.items,
                function(item) {
                    item.isFirst = first !== item.id;
                    first = item.id;
                    item.packageForcastDateText = moment(item.packageForcastDate).format("DD/MM/YYYY");
                    item.statusText = statesGroup[item.status + ''][0].name;
                    item.createdText = moment(item.forcastDate).format("DD/MM/YYYY HH:mm:ss");
                });

            self.items(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
        });
    }

    self.callback = function() {
        self.search(1);
    }

    self.addBill = function () {
        if (self.addForm() == null) {
            self.addForm(new ImportWarehouseAddModel(self.callback, packageDetailModal, orderDetailModal, walletDetailModal, orderServiceOtherModal));
            ko.applyBindings(self.addForm(), $("#importWarehouseBind")[0]);
        }

        self.addForm().showAddForm();
    }

    self.addBillV2 = function () {
        if (self.addForm() == null) {
            self.addForm(new ImportWarehouseAddModel(self.callback, packageDetailModal, orderDetailModal, walletDetailModal, orderServiceOtherModal));
            ko.applyBindings(self.addForm(), $("#importWarehouseBind")[0]);
        }

        $.get("/importwarehouse/gettoadd",
            function(data) {
                self.addForm().showUpdateForm(data);
            });
    }

    self.showDetail = function(data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data.packageId);
            return;
        }
    }

    self.showWalletDetail = function(data) {
        if (walletDetailModal) {
            walletDetailModal.showModel(data.walletId);
        }
    }

    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new ImportWarehouseAddModel(self.callback, packageDetailModal, orderDetailModal, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#importWarehouseBind")[0]);
        }

        self.addForm().setForm(data);
    }

    $(function() {
        $('#ImportWarehouse-date-btn').daterangepicker({
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
                $('#ImportWarehouse-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
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

// Bind WalletDetail
//allWarehouse, walletStates, orderPackageStates, packageDetailModal, orderDetailModal
var walletDetailModelView = new WalletDetailModel(window.allWarehouse, window.walletStates, window.orderPackageStates, packageDetailModelView, orderDetailViewModel, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(walletDetailModelView, $("#walletDetailModal")[0]);

// Bind OrderDetail
var orderServiceOtherModelView = new OrderServiceOtherModel();
ko.applyBindings(orderServiceOtherModelView, $("#orderServiceOtherModal")[0]);

var modelView = new ImportWarehouseModel(packageDetailModelView, orderDetailViewModel, walletDetailModelView, orderServiceOtherModelView, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#importWarehouse")[0])