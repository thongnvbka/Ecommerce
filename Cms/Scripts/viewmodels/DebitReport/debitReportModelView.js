function DebitReportModel() {
    var self = this;

    self.packageStatus = ko.observable(null);
    self.items = ko.observableArray([]);

    self.show = function (items, status) {
        self.packageStatus(status);
        self.items(items);
        $("#debitReportModal").modal("show");
    }
}

function TabModelView(serviceId, type, services) {
    var self = this;

    self.services = ko.observableArray(services);
    self.isLoading = ko.observable(false);

    self.templateId = ko.observable(null);
    self.serviceId = ko.observable(serviceId);
    self.type = ko.observable(type);
    self.isActive = ko.observable(true);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.data = ko.observable(null);
    self.items = ko.observableArray([]);

    self.tabId = null;
    self.getTabId = function () {
        if (self.tabId)
            return self.tabId;

        self.tabId = "tabs_" + Math.round(new Date().getTime() + (Math.random() * 100)) + "_";

        return self.tabId;
    }

    self.getTabId();

    self.typeName = ko.observable("");
    self.serviceName = ko.observable("");

    self.tabTitle = ko.computed(function () {
        if (self.type() === 1) {
            self.typeName("Estimated revenue");
        }else if (self.type() === 2) {
            self.typeName("Receivable");
        } else if (self.type() === 0) {
            self.typeName("Total revenue");
        } else if (self.type() === 3) {
            self.typeName("Payable");
        }

        if (self.serviceId() === 255) {
            self.serviceName("Cost of goods");
        } else if (self.serviceId() === 254) {
            self.serviceName("Change due");
        } else if (self.serviceId() === 253) {
            self.serviceName("Deposit");
        } else if (self.serviceId() === -1) {
            self.serviceName("Total");
        } else {
            var service = _.find(self.services(), function (s) { return s.serviceId === self.serviceId() });

            self.serviceName(service.serviceName);
        }

        if (self.type() === 0 && self.serviceId() !== -1) {
            self.templateId(0);
            return self.typeName() + " - " + self.serviceName();
        } else if (serviceId === -1 && self.type() !== 0) {
            self.templateId(1);
            return self.serviceName() + " - " + self.typeName();
        } else if (self.type() === 0 && self.serviceId() === -1) {
            self.templateId(2);
            return "Total service revenue";
        } else {
            if (self.serviceId() === 254 || self.serviceId() === 253) {
                self.templateId(4);
            } else {
                self.templateId(3);
            }
            
            return self.serviceName() + " - " + self.typeName();
        }
    }, self);

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumary_" + self.tabId).html(self.totalRecord() === 0 ? "There is not any order" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " order");

        $("#pager_" + self.tabId).pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pager_" + self.tabId).pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);

        self.isLoading(true);
        $.get("/DebitReport/Detail",
            {
                type: self.type(),
                serviceId: self.serviceId(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            },
            function(data) {
                self.isLoading(false);

                self.processData(data);

                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    self.processData = function (data) {
        // Sum thu các dịch vụ đơn lẻ
        if (self.templateId() === 0) {
            _.each(data.orderReports, function (o) {
                if (data['debitFeatureOrders'] && data.debitFeatureOrders[o.id] && data.debitFeatureOrders[o.id][0]) {
                    o['type1_service_' + self.serviceId()] = data.debitFeatureOrders[o.id][0].price;
                } else if (data['debitFeaturePackages'] && data.debitFeaturePackages[o.id] && data.debitFeaturePackages[o.id][0]) {
                    o['packages_type1_service_' + self.serviceId()] = data.debitFeaturePackages[o.id];
                    o['type1_service_' + self.serviceId()] = _.sumBy(data.debitFeaturePackages[o.id], "price");;
                } else {
                    o['type1_service_' + self.serviceId()] = 0;
                }

                if (data['debitRequiredOrders'] && data.debitRequiredOrders[o.id] && data.debitRequiredOrders[o.id][0]) {
                    o['type2_service_' + self.serviceId()] = data.debitRequiredOrders[o.id][0].price;
                } else if (data['debitRequiredPackages'] && data.debitRequiredPackages[o.id] && data.debitRequiredPackages[o.id][0]) {
                    o['packages_type2_service_' + self.serviceId()] = data.debitRequiredPackages[o.id];
                    o['type2_service_' + self.serviceId()] = _.sumBy(data.debitRequiredPackages[o.id], "price");;
                } else {
                    o['type2_service_' + self.serviceId()] = 0;
                }
            });
            self.items(data.orderReports);
        } else if (self.templateId() === 1) {
            _.each(data.orderReports, function (o) {
                o['totalService'] = 0;

                _.each(self.services(),
                    function (s) {
                        var packageServices;
                        var service;
                        if (self.type() === 1) {
                            packageServices = [];
                            service = null;
                            if (data['debitFeatureOrders'] && data.debitFeatureOrders[o.id]) {
                                service = _.find(data.debitFeatureOrders[o.id],
                                    function (ss) { return ss.serviceId === s.serviceId });
                            }

                            if (data['debitFeaturePackages'] && data.debitFeaturePackages[o.id]) {
                                packageServices = _.filter(data.debitFeaturePackages[o.id],
                                    function (ss) { return ss.serviceId === s.serviceId });
                            }

                            if (service) {
                                o['service_' + s.serviceId] = service.price;
                            } else if (packageServices.length > 0) {
                                o['service_' + s.serviceId] = _.sumBy(packageServices, "price");;
                            } else {
                                o['service_' + s.serviceId] = 0;
                            }
                        }

                        if (self.type() === 2) {
                            packageServices = [];
                            service = null;
                            if (data['debitRequiredOrders'] && data.debitRequiredOrders[o.id]) {
                                service = _.find(data.debitRequiredOrders[o.id],
                                    function (ss) { return ss.serviceId === s.serviceId });
                            }

                            if (data['debitRequiredPackages'] && data.debitRequiredPackages[o.id]) {
                                packageServices = _.filter(data.debitRequiredPackages[o.id],
                                    function (ss) { return ss.serviceId === s.serviceId });
                            }

                            if (service) {
                                o['service_' + s.serviceId] = service.price;
                            } else if (packageServices.length > 0) {
                                o['service_' + s.serviceId] = _.sumBy(packageServices, "price");;
                            } else {
                                o['service_' + s.serviceId] = 0;
                            }
                        }

                        o['totalService'] = o['totalService'] + o['service_' + s.serviceId];
                    });
            });
            self.items(data.orderReports);
        } else if (self.templateId() === 2) {
            _.each(data.orderReports, function (o) {
                o['type1_totalService'] = 0;
                o['type2_totalService'] = 0;

                _.each(self.services(),
                    function (s) {
                        var packageServices;
                        var service;
                        packageServices = [];
                        service = null;

                        if (data['debitFeatureOrders'] && data.debitFeatureOrders[o.id]) {
                            service = _.find(data.debitFeatureOrders[o.id],
                                function (ss) { return ss.serviceId === s.serviceId });
                        }

                        if (data['debitFeaturePackages'] && data.debitFeaturePackages[o.id]) {
                            packageServices = _.filter(data.debitFeaturePackages[o.id],
                                function (ss) { return ss.serviceId === s.serviceId });
                        }

                        if (service) {
                            o['type1_service_' + s.serviceId] = service.price;
                        } else if (packageServices.length > 0) {
                            o['type1_service_' + s.serviceId] = _.sumBy(packageServices, "price");;
                        } else {
                            o['type1_service_' + s.serviceId] = 0;
                        }

                        packageServices = [];
                        service = null;
                        if (data['debitRequiredOrders'] && data.debitRequiredOrders[o.id]) {
                            service = _.find(data.debitRequiredOrders[o.id],
                                function (ss) { return ss.serviceId === s.serviceId });
                        }

                        if (data['debitRequiredPackages'] && data.debitRequiredPackages[o.id]) {
                            packageServices = _.filter(data.debitRequiredPackages[o.id],
                                function (ss) { return ss.serviceId === s.serviceId });
                        }

                        if (service) {
                            o['type2_service_' + s.serviceId] = service.price;
                        } else if (packageServices.length > 0) {
                            o['type2_service_' + s.serviceId] = _.sumBy(packageServices, "price");;
                        } else {
                            o['type2_service_' + s.serviceId] = 0;
                        }

                        o['type1_totalService'] = o['type1_totalService'] + o['type1_service_' + s.serviceId];
                        o['type2_totalService'] = o['type2_totalService'] + o['type2_service_' + s.serviceId];
                    });
            });
            self.items(data.orderReports);
        } else if (self.templateId() === 3) {
            _.each(data.orderReports,
                function(o) {
                    if (self.type() === 1) {
                        if (data['debitFeatureOrders'] &&
                            data.debitFeatureOrders[o.id] &&
                            data.debitFeatureOrders[o.id][0]) {
                            o['service_' + self.serviceId()] = data.debitFeatureOrders[o.id][0].price;
                        } else if (data['debitFeaturePackages'] &&
                            data.debitFeaturePackages[o.id] &&
                            data.debitFeaturePackages[o.id][0]) {

                            o['packages_service_' + self.serviceId()] = data.debitFeaturePackages[o.id];
                            o['service_' + self.serviceId()] = _.sumBy(data.debitFeaturePackages[o.id], "price");;
                        } else {
                            o['service_' + self.serviceId()] = 0;
                        }
                    }else if (self.type() === 2) {
                        if (data['debitRequiredOrders'] &&
                            data.debitRequiredOrders[o.id] &&
                            data.debitRequiredOrders[o.id][0]) {
                            o['service_' + self.serviceId()] = data.debitRequiredOrders[o.id][0].price;
                        } else if (data['debitRequiredPackages'] &&
                            data.debitRequiredPackages[o.id] &&
                            data.debitRequiredPackages[o.id][0]) {
                            o['packages_service_' + self.serviceId()] = data.debitRequiredPackages[o.id];
                            o['service_' + self.serviceId()] = _.sumBy(data.debitRequiredPackages[o.id], "price");;
                        } else {
                            o['service_' + self.serviceId()] = 0;
                        }
                    }
                });
            self.items(data.orderReports);
        } else if (self.templateId() === 4) {
            _.each(data.orders,
                function(o) {
                    if (self.serviceId() === 254) // Excess money
                    {
                        o['service_' + self.serviceId()] = (-1) * o.debt;
                    }
                    if (self.serviceId() === 253) // Tiền Deposit
                    {
                        o['service_' + self.serviceId()] = o.totalPayed;
                    }
                });

            self.items(data.orders);
        }
    }

    self.search(1);
}

function DebitReportModelView() {

    var self = this;

    self.isLoading = ko.observable(false);

    self.debitReport = ko.observable(null);

    self.tabs = ko.observableArray([]);
    //Xuân thêm
    self.debitType = ko.observable(0);
    self.debitServiceId = ko.observable(0);

    $(function () {

    });

    self.activeTab = function(tab) {
        _.each(self.tabs(),
            function (t) {
                t.isActive(false);
            });

        //Xuân thêm
        self.debitType(tab.type);
        self.debitServiceId(tab.serviceId);
        tab.isActive(true);
        tab.renderPager();
    }

    self.getReport = function () {
        self.isLoading(true);
        $.get("/DebitReport/GetReport",
            function (data) {
                self.isLoading(false);
                self.debitReport(data);
            });
    }

    self.showPackage = function (data) {
        
        debitReportViewModel.show(data, _.cloneDeep(self.debitReport().packageStatus));
    }

    self.getReport();

    /**
     * 
     * @param {} serviceId : Id của dịch vụ. Nếu = 0 là tổng tiền dịch vụ: 254: Tiền thừa, 253: Đặt cọc
     * @param {} type : Loại: 0: Tổng dự kiến thu và phải thu, 1: Dự kiến thu, 2: Phải thu, 3 Phải trả
     * @returns {} 
     */
    self.detail = function (serviceId, type) {

        var tab = _.find(self.tabs(), function (t) { return t.type() === type && t.serviceId() === serviceId; });

        var isAddNew = false;
        if (tab == null) {
            isAddNew = true;
            tab = new TabModelView(serviceId, type, self.debitReport().services);
        };
        _.each(self.tabs(),
            function(t) {
                t.isActive(false);
            });

        tab.isActive(true);
        tab.renderPager();

        if (isAddNew)
            self.tabs.push(tab);
        //Xuân thêm
        self.debitType(type);
        self.debitServiceId(serviceId);
    }

    self.ExportDebit = function () {
        $.redirect("/DebitReport/ExcelDebit", {}, "POST");
    }
    //template() ==0
    self.ExportDebit0 = function () {
        $.redirect("/DebitReport/ExcelDebitServices", { type: self.debitType(), serviceId: self.debitServiceId() }, "POST");
    }
    
}

var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var debitReportViewModel = new DebitReportModel();
ko.applyBindings(debitReportViewModel, $("#debitReportModal")[0]);

var modelView = new DebitReportModelView();
ko.applyBindings(modelView, $("#debitReport")[0]);