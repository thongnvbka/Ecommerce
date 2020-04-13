function TrackingPackageModel(orderDetailModal, packageDetailModal, walletDetailModal) {
    var self = this;

    self.isLoading = ko.observable(false);

    // filter
    self.listWarehouse = ko.observableArray([]);
    self.listOrderStatus = ko.observableArray([]);
    self.listDepositStatus = ko.observableArray([]);
    self.listPackageStatus = ko.observableArray([]);
    self.listTimeType = ko.observableArray([]);
    self.listOrderType = ko.observableArray([]);
    self.listOrderService = ko.observableArray([]);
    self.warehouse = ko.observable(null);
    self.orderStatus = ko.observable(null);
    self.depositStatus = ko.observable(null);
    self.packageStatus = ko.observable(null);
    self.packageNoteMode = ko.observable(null);
    self.orderType = ko.observable(null);
    self.searchType = ko.observable(null);
    self.searchTypeText = ko.observable('Select');
    self.searchPlaceholder = ko.observable('Enter keywords..');
    self.searchTypes = ko.observableArray([{ id: null, name: 'Select', placeholder: 'Enter keywords..' }, { id: 0, name: 'Bill of lading', placeholder: 'Enter the bill of lading..' }, { id: 1, name: 'Single code', placeholder: 'Enter single code..' }, { id: 2, name: 'Item code', placeholder: 'Enter the event code..' }, { id: 3, name: 'Email', placeholder: 'Enter email..' }]);

    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);

    self.packages = ko.observableArray([]);
    self.keyword = ko.observable("");

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.changeCheck = function(data) {
        data.checked(!data.checked());
        self.search(1);
    }

    self.changeSearchType = function(data) {
        self.searchType(data.id);
        self.searchPlaceholder(data.placeholder);
        self.searchTypeText(data.name);
        self.search(1);
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerTrackingPackage").html(self.totalRecord() === 0 ? "There are no orders" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " packing");

        $("#pagerTrackingPackage").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerTrackingPackage").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage, isFirstRequest) {
        self.currentPage(currentPage);
        self.isLoading(true);

        var statusTexts = _.map(_.filter(self.listPackageStatus(), function (it) { return it.checked() }), "id");
        var statusText = statusTexts.length === 0 ? "" : ';' + _.join(statusTexts, ';') + ';';

        var statusDepositTexts = _.map(_.filter(self.listDepositStatus(), function (it) { return it.checked() }), "id");
        var statusDepositText = statusDepositTexts.length === 0 ? "" : ';' + _.join(statusDepositTexts, ';') + ';';

        var timeTypeTexts = _.map(_.filter(self.listTimeType(), function (it) { return it.checked() }), "id");
        var timeTypeText = timeTypeTexts.length === 0 ? "" : ';' + _.join(timeTypeTexts, ';') + ';';

        var orderStatusTexts = _.map(_.filter(self.listOrderStatus(), function (it) { return it.checked() }), "id");
        var orderStatusText = orderStatusTexts.length === 0 ? "" : ';' + _.join(orderStatusTexts, ';') + ';';

        var warehousesTexts = _.map(_.filter(self.listWarehouse(), function (it) { return it.checked() }), "id");
        var warehouseIdText = warehousesTexts.length === 0 ? "" : ';' + _.join(warehousesTexts, ';') + ';';

        var orderTypeTexts = _.map(_.filter(self.listOrderType(), function (it) { return it.checked() }), "id");
        var orderTypeText = orderTypeTexts.length === 0 ? "" : ';' + _.join(orderTypeTexts, ';') + ';';

        var orderServiceTexts = _.map(_.filter(self.listOrderService(), function (it) { return it.checked() }), "id");
        var orderServiceText = orderServiceTexts.length === 0 ? "" : ';' + _.join(orderServiceTexts, ';') + ';';

        $.get("/TrackingPackage/Search",
            {
                keyword: self.keyword(),
                searchType: self.searchType(),
                statusText: statusText,
                timeTypeText: timeTypeText,
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                orderStatusText: orderStatusText,
                statusDepositText: statusDepositText,
                warehouseIdText: warehouseIdText,
                isFirstRequest: isFirstRequest,
                orderTypeText: orderTypeText,
                orderServiceText: orderServiceText,
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                if (isFirstRequest) {
                    _.each(data.orderTypes,
                        function(t) {
                            t.checked = ko.observable(t.checked);
                        });

                    _.each(data.services,
                        function (t) {
                            t.checked = ko.observable(t.checked);
                        });

                    _.each(data.warehouses,
                        function(t) {
                            t.checked = ko.observable(t.checked);
                        });

                    _.each(data.orderStatuss,
                        function(t) {
                            t.checked = ko.observable(t.checked);
                        });

                    _.each(data.depositStatuss,
                        function (t) {
                            t.checked = ko.observable(t.checked);
                        });

                    var packageStatuss = data.packageStatuss;
                    var listTimeType = _.cloneDeep(data.packageStatuss);

                    _.each(packageStatuss,
                        function(t) {
                            t.checked = ko.observable(t.checked);
                        });

                    _.each(listTimeType,
                        function (t) {
                            t.checked = ko.observable(t.checked);
                        });

                    self.listOrderType(data.orderTypes);
                    self.listOrderService(data.services);
                    self.listWarehouse(data.warehouses);
                    self.listOrderStatus(data.orderStatuss);
                    self.listDepositStatus(data.depositStatuss);
                    self.listPackageStatus(packageStatuss);
                    self.listTimeType(listTimeType);
                    
                    self.warehouse(data.warehouse);
                    self.orderStatus(data.orderStatus);
                    self.depositStatus(data.depositStatus);
                    self.packageStatus(data.packageStatus);
                    self.orderType(data.orderType);
                  
                    self.packageNoteMode(data.packageNoteMode);
                }

                _.each(data.packages,
                    function (p) {
                        p.walletId = data.walletPackages[p.id + '_' + 0] ? data.walletPackages[p.id + '_' + 0][0].id : null;
                        p.walletCode = data.walletPackages[p.id + '_' + 0] ? data.walletPackages[p.id + '_' + 0][0].code : null;
                        p.walletPackageId = data.walletPackages[p.id + '_' + 1] ? data.walletPackages[p.id + '_' + 1][0].id : null;
                        p.walletPackageCode = data.walletPackages[p.id + '_' + 1] ?  data.walletPackages[p.id + '_' + 1][0].code : null;
                        p.walletPackagePackageNo = data.walletPackages[p.id + '_' + 1] ? data.walletPackages[p.id + '_' + 1][0].packageNo : null;

                        p['packageNoteMode'] = self.packageNoteMode();

                    _.each(data.orderServices[p.orderId + ''],
                        function(s) {
                            p['service_'+s.serviceId] = s;
                        });

                    _.each(data.packageHistories[p.id + ''],
                        function (h) {
                            if (h.jsonData) {
                                h.data = JSON.parse(h.jsonData);
                            }
                            
                            p['status_' + h.status] = h;
                        });
                        p['packageNotes'] = data.packageNotes[p.id + ''];
                    });

                self.packages(data.packages);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailModal) {
            orderDetailModal.viewOrderDetail(orderId);
            return;
        }
    }

    self.showPackageDetail = function (data) {
        if (packageDetailModal) {
            packageDetailModal.showModel(data.id);
            return;
        }
    }

    self.showWalletDetail = function (walletId) {
        if (walletDetailModal) {
            walletDetailModal.showModel(walletId);
            return;
        }
    }

    self.transferPackageForm = ko.observable(null);
    self.showTransferPackage = function (data) {
        if (self.transferPackageForm() == null) {
            self.transferPackageForm(new TransferPackageModel());
            ko.applyBindings(self.transferPackageForm(), $("#transferPackageModal")[0]);
        }

        self.transferPackageForm().show(data.transportCode, data.id, data.code, data.customerUserName, function () {
            self.search(self.currentPage());
        });
    }

    self.showUpdateWeight = function(data) {
        $("#updatePackageWeightModal").modal("show");
    }

    $(function () {
        $('#forcastDate-btn').daterangepicker({
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
                $('#forcastDate-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

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

    //Xuất file Excel cho Kho
    self.ExcelTrackingPackage = function (data) {

        var statusTexts = _.map(_.filter(self.listPackageStatus(), function (it) { return it.checked() }), "id");
        var statusText = statusTexts.length === 0 ? "" : ';' + _.join(statusTexts, ';') + ';';

        var statusDepositTexts = _.map(_.filter(self.listDepositStatus(), function (it) { return it.checked() }), "id");
        var statusDepositText = statusDepositTexts.length === 0 ? "" : ';' + _.join(statusDepositTexts, ';') + ';';

        var timeTypeTexts = _.map(_.filter(self.listTimeType(), function (it) { return it.checked() }), "id");
        var timeTypeText = timeTypeTexts.length === 0 ? "" : ';' + _.join(timeTypeTexts, ';') + ';';

        var orderStatusTexts = _.map(_.filter(self.listOrderStatus(), function (it) { return it.checked() }), "id");
        var orderStatusText = orderStatusTexts.length === 0 ? "" : ';' + _.join(orderStatusTexts, ';') + ';';

        var warehousesTexts = _.map(_.filter(self.listWarehouse(), function (it) { return it.checked() }), "id");
        var warehouseIdText = warehousesTexts.length === 0 ? "" : ';' + _.join(warehousesTexts, ';') + ';';

        var orderTypeTexts = _.map(_.filter(self.listOrderType(), function (it) { return it.checked() }), "id");
        var orderTypeText = orderTypeTexts.length === 0 ? "" : ';' + _.join(orderTypeTexts, ';') + ';';

        var orderServiceTexts = _.map(_.filter(self.listOrderService(), function (it) { return it.checked() }), "id");
        var orderServiceText = orderServiceTexts.length === 0 ? "" : ';' + _.join(orderServiceTexts, ';') + ';';

        $.redirect("/TrackingPackage/ExcelTrackingPackage", {
            keyword: self.keyword(),
            timeTypeText: timeTypeText,
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            statusText: statusText,
            orderStatusText: orderStatusText,
            statusDepositText: statusDepositText,
            warehouseIdText: warehouseIdText,
            orderTypeText: orderTypeText,
            orderServiceText: orderServiceText
        },
        "POST");
    }
}