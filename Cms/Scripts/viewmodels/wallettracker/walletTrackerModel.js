function WalletTrackerModel(packageDetailModal, orderDetailModal, walletDetailModal, routeWalletModelView, depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.allWarehouses = ko.observableArray(window["allWarehouses"] ? window.allWarehouses : []);
    self.states = ko.observableArray([]);
    self.entrepots = ko.observableArray(window["entrepots"] ? window.entrepots : []);
    self.statesGroupId = _.groupBy(window["states"] ? window.states : [], 'id');
    self.transportPartners = ko.observableArray(window.transportPartners);
    self.warehouseIdPath = ko.observable("");
    self.targetWarehouseId = ko.observable(null);
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    self.mode = ko.observable(3);
    self.tabId = ko.observable(null);
    self.entrepotId = ko.observable(null);
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

    self.isCheckedAll = ko.observable(false);
    self.hasItemChecked = ko.observable(false);

    self.totalWeight = ko.observable(0);
    self.totalWeightConverted = ko.observable(0);
    self.totalVolume = ko.observable(0);
    self.totalPackNo = ko.observable(0);
    self.totalWallet = ko.observable(0);
    self.totalWalletReceived = ko.observable(0);
    self.totalWalletCompleted = ko.observable(0);

    self.changeMode = function(mode) {
        if (mode !== self.mode()) {
            self.mode(mode);
            self.search(1);
        }
    }

    self.showWalletDetail = function (walletId) {
        if (walletDetailModal) {
            walletDetailModal.showModel(walletId);
            return;
        }
    }

    self.showRouteWallet = function(walletId) {
        routeWalletModelView.show(walletId);
    }

    self.changeIsCheckedAll = function() {
        self.isCheckedAll(!self.isCheckedAll());

        _.each(_.filter(self.items(), function(i) { return i.disableCheck === false; }), function(it) {
                it.isChecked(self.isCheckedAll());
            });

        self.hasItemChecked(self.isCheckedAll());
    }

    self.changeIsChecked = function(item) {
        item.isChecked(!item.isChecked());
    
        var count = _.countBy(_.filter(self.items(), function(i) { return i.disableCheck === false; }), function(it){ return it.isChecked(); });

        if (count["true"] === undefined) {
            self.isCheckedAll(false);
        }else if (count["false"] === undefined) {
            self.isCheckedAll(true);
        }else if (count["true"] && count["false"]) {
            self.isCheckedAll(false);
        } else {
            self.isCheckedAll(true);
        }

        self.hasItemChecked(count["true"] !== undefined);
    }

    self.changeTab = function(tabId) {
        if (tabId !== self.tabId()) {
            self.items([]);
            self.tabId(tabId);
            
            if (tabId === 0) {
                self.states([{ id: 0, name: "Newly created" }, { id: 1, name: "In transit" }, { id: 2, name: "Completed" }]);
                self.status(null);
                self.totalWeight(0);
                self.totalWeightConverted(0);
                self.totalVolume(0);
                self.totalPackNo(0);
                
            }else if (tabId > 0) {
                self.states([
                    { id: 0, name: "Waiting to receive goods" }, { id: 1, name: "In transit" },
                    { id: 2, name: "Completed" }
                ]);
                self.status(null);
                self.totalWeight(0);
                self.totalWeightConverted(0);
                self.totalVolume(0);
                self.totalPackNo(0);
                self.totalWallet(0);
                self.totalWalletReceived(0);
                self.totalWalletCompleted(0);
            } else {
                 self.states([{ id: 0, name: "In transit" }, { id: 1, name: "Stocked" }]);
                self.status(null);
                self.targetWarehouseId(null);
                self.totalWeight(0);
                self.totalWeightConverted(0);
                self.totalVolume(0);
                self.totalPackNo(0);
                self.totalWallet(0);
                self.totalWalletReceived(0);
                self.totalWalletCompleted(0);
            }
        }
    }

    self.changeWarehouse = function() {
        self.search(1);
    }

    self.changeStatus = function() {
        self.search(1);
    }

    self.changeTargetWarehouseId = function() {
        self.search(1);
    }

    self.changeEntrepotId = function() {
        self.search(1);
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerWalletTracker").html(self.totalRecord() === 0 ? "There are no packing" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " packing");

        $("#pagerWalletTracker").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerWalletTracker").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.exportParam = ko.computed(function() {
        return $.param({
            tabId: self.tabId(),
            entrepotId: self.entrepotId(),
            mode: self.mode(),
            warehouseIdPath: self.warehouseIdPath(),
            targetWarehouseId: self.targetWarehouseId(),
            userId: self.userId(),
            status: self.status(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword()
        });
    }, this);

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/wallettracker/search",
            {
                tabId: self.tabId(),
                entrepotId: self.entrepotId(),
                mode: self.mode(),
                warehouseIdPath: self.warehouseIdPath(),
                targetWarehouseId: self.targetWarehouseId(),
                userId: self.userId(),
                status: self.status(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                if (self.tabId() <= 0) {
                    self.totalWeight(data.tab["totalWeight"]);
                    self.totalWeightConverted(data.tab["totalWeightConverted"]);
                    self.totalVolume(data.tab["totalVolume"]);
                    self.totalPackNo(data.tab["totalPackNo"]);
                    self.hasItemChecked(false);
                    self.isCheckedAll(false);
                    _.each(data.items,
                        function(it) {
                            it.isChecked = ko.observable(false);
                            it.status =  it.dispatcherId === null
                                ? 0
                                : it.walletTargetWarehouseId === it.walletCurrentWarehouseId ? 2 :
                                 it.walletPartnerId !== null
                                ? 1 : -1;
                            it.disableCheck = it.status !== 0;
                        });
                }else if (self.tabId() > 0) {
                    self.totalWallet(data.tab["totalWallet"]);
                    self.totalWalletReceived(data.tab["totalWalletReceived"]);
                    self.totalWalletCompleted(data.tab["totalWalletCompleted"]);
                    self.totalWeight(data.tab["totalWeight"]);
                    self.totalWeightConverted(data.tab["totalWeightConverted"]);
                    self.totalVolume(data.tab["totalVolume"]);
                    self.totalPackNo(data.tab["totalPackNo"]);
                    self.hasItemChecked(false);
                    self.isCheckedAll(false);

                    var firstDispatcherId = null;
                    _.each(data.items,
                        function(it) {
                            it.isChecked = ko.observable(false);
                            it.disableCheck = it.toDispatcherId !== null || it.dispatcherDetailStatus !== 1;
                            it.isFirst = firstDispatcherId !== it.dispatcherId;
                            firstDispatcherId = it.dispatcherId;
                            it.status = ko.observable(it.dispatcherDetailStatus);
                            it.dispatcherDetailValueCache = it.dispatcherDetailValue;
                            it.dispatcherDetailValue = ko.observable(formatNumberic(it.dispatcherDetailValue, 'N2'));
                            it.dispatcherDetailValue.subscribe(function(newValue) {
                                var value = Globalize.parseFloat(newValue);
                                if (isNaN(value) || value === it.dispatcherDetailValueCache) {
                                    return;
                                }
                                it.dispatcherDetailValueCache = value;
                                self.updateValue(it);
                            });

                            it.dispatcherDetailDescriptionCache = it.dispatcherDetailDescription;
                            it.dispatcherDetailDescription = ko.observable(it.dispatcherDetailDescription);
                            it.dispatcherDetailDescription.subscribe(function(newValue) {
                                if (newValue === it.dispatcherDetailDescriptionCache) {
                                    return;
                                }
                                it.dispatcherDetailDescriptionCache = newValue;
                                self.updateDescription(it);
                            });
                            
                        });
                    
                }
            self.items(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
            self.initInputMark();
        });
    }

    self.callback = function () {
        self.search(self.currentPage());
    }

    self.addBill = function () {
        if (self.addForm() == null) {
            self.addForm(new DispatcherAddModel(self.callback, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#DispatcherAddModel")[0]);
        }

        var walletCodes = ";" +
            _.join(_.map(_.filter(self.items(), function(it) { return it.isChecked(); }), "walletCode"), ";") +
            ";";

        self.addForm().showAddForm(walletCodes);
    }

    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new DispatcherAddModel(self.callback, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#DispatcherAddModel")[0]);
        }

        $.get("/dispatcher/getdetail",
            { id: data.dispatcherId },
            function(result) {
                self.addForm().setForm(result);
            });
    }

    self.token = $("#walletTracker input[name='__RequestVerificationToken']").val();

    self.updateValue = function(item) {
        var data = { dispatcherDetailId: item.dispatcherDetailId, value: item.dispatcherDetailValue() };
        data["__RequestVerificationToken"] = self.token;

        if ($.trim(data.value) === "") {
            toastr.warning("Shipping latch value must not be left blank");
            return;
        }

        $.post("/dispatcher/updateValue", data,
            function(rs) {
                if (rs.status >= 0) {
                    item.status(rs.status);
                    item.dispatcherDetailStatus = item.status();
                    item.disableCheck = item.dispatcherDetailStatus !== 1;
                    return;
                }

                toastr.warning(rs.text);
            });
    }

    self.updateDescription = function(item) {
        var data = { dispatcherDetailId: item.dispatcherDetailId, description: item.dispatcherDetailDescription() };
        data["__RequestVerificationToken"] = self.token;

        if ($.trim(data.description) === "") {
            return;
        }

        $.post("/dispatcher/updateDescription", data,
            function(rs) {
                if (rs.status >= 0) {
                    return;
                }

                toastr.warning(rs.text);
            });
    }

    self.confirm = function(item) {
        var value = item.priceType === 0
            ? formatNumberic(item.walletWeight, 'N2')
            : formatNumberic(item.walletVolume, 'N2');

        item.dispatcherDetailValue(value);
    }

    self.initInputMark = function() {
        $('#walletTracker input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    $(function () {
        $('#WalletTracker-date-btn').daterangepicker({
            ranges: {
                'Today' : [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '7 days ago': [moment().subtract(6, 'days'), moment()],
                '30 days ago': [moment().subtract(29, 'days'), moment()],
                'This month': [moment().startOf('month'), moment().endOf('month')],
                'Last month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            },
            startDate: moment(),
            endDate: moment()
        },
            function (start, end) {
                self.fromDate(start.format());
                self.toDate(end.format());
                self.search(1);

                if (moment().diff(start, 'days') === 0 && moment().diff(end, 'days') === 0) {
                    $('#WalletTracker-date-btn span')
                        .html('Today' );
                } else if (moment().subtract(1, 'days').diff(start, 'days') === 0 && moment().subtract(1, 'days').diff(end, 'days') === 0) {
                    $('#WalletTracker-date-btn span')
                        .html('Yesterday');
                }
                else if (moment().subtract(6, 'days').diff(start, 'days') === 0 && moment().diff(end, 'days') === 0) {
                    $('#WalletTracker-date-btn span')
                        .html('7 days ago');
                }
                else if (moment().subtract(29, 'days').diff(start, 'days') === 0 && moment().diff(end, 'days') === 0) {
                     $('#WalletTracker-date-btn span')
                        .html('30 days ago');
                }
                else if (moment().startOf('month').diff(start, 'days') === 0 && moment().endOf('month').diff(end, 'days') === 0) {
                    $('#WalletTracker-date-btn span')
                        .html('This month');
                }
                else if (moment().subtract(1, 'month').startOf('month').diff(start, 'days') === 0 && moment().subtract(1, 'month').endOf('month').diff(end, 'days') === 0) {
                    $('#WalletTracker-date-btn span')
                        .html('Last month');
                }
                else {
                    $('#WalletTracker-date-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                }
            });

        $('#WalletTracker-date-btn span').html('Today' );

        self.fromDate(moment().startOf('day').format());
        self.toDate( moment().endOf('day').format());

        self.changeTab(0);
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

// allWarehouse, walletStates, orderPackageStates, packageDetailModal, orderDetailModal
var walletDetailModalView = new WalletDetailModel(window.allWarehouse, window.walletStates, window.orderPackageStates, packageDetailModelView, orderDetailViewModel);
ko.applyBindings(walletDetailModalView, $("#walletDetailModal")[0]);

// Bind PackageDetail
var routeWalletModelView = new RouteTransportModel(walletDetailModalView);
ko.applyBindings(routeWalletModelView, $("#routeWalletModal")[0]);

var modelView = new WalletTrackerModel(packageDetailModelView, orderDetailViewModel, walletDetailModalView, routeWalletModelView, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#walletTracker")[0]);