
// type: 0: Nhập kho, 1: Xuất kho, 2: Tồn kho, 3: Mất hàng,
// 4: Tạo phiếu mất mã, 
// 5: Xử lý phiếu mất mã

/**
 * Detail package
 * @param {any} type Loại
 * @param {any} time Thời gian
 * @param {any} items package
 */
function PackageModel(type, time, momentTime, warehouseId, warehouseCulture) {
    var self = this;
    self.type = ko.observable(type);
    self.time = ko.observable(time);
    self.isActive = ko.observable(true);
    self.items = ko.observableArray([]);
    self.packages = ko.observableArray([]);
    self.momentTime = ko.observable(momentTime);
    self.warehouseId = ko.observable(warehouseId);
    self.warehouseCulture = ko.observable(warehouseCulture);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.tabId = null;
    self.getTabId = function () {
        if (self.tabId)
            return self.tabId;

        self.tabId = "tabs_" + Math.round(new Date().getTime() + (Math.random() * 100)) + "_";

        return self.tabId;
    }
    self.getTabId();

    self.tabTitle = ko.computed(function() {
        if (self.type() === "importeds")
            return "import warehouse - " + self.time();

        if (self.type() === "exporteds")
            return "export warehouse - " + self.time();

        if (self.type() === "storeds")
            return "inventory- " + self.time();

        if (self.type() === "loses")
            return "Lost goods - " + self.time();

        if (self.type() === "noCodeCreated")
            return "Create a code loss code - " + self.time();

        if (self.type() === "noCodeUpdated")
            return "Has processed the lost code - " + self.time();

        return "";
    });

    self.search = function (currentPage) {
        self.currentPage(currentPage);

        $.get("/PackageReport/GetPackages",
            {
                time: self.momentTime(),
                type: self.type(),
                warehouseId: self.warehouseId(),
                warehouseCulture: self.warehouseCulture(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            },
            function(data) {
                self.totalRecord(data.totalRecord);

                if (self.type() === "noCodeCreated" || self.type() === "noCodeUpdated") {
                    _.each(data.items,
                        function(item) {
                            item.statusText = item.packageNoCodeStatus === 0 ? 'Newly created' : 'closed';
                            item.statusClass = item.packageNoCodeStatus === 0 ? 'label label-warning' : 'label label-success';
                            item.packageNoCodeCreatedTextNow = item.created ? moment(item.created).fromNow() : '';
                            item.packageNoCodeCreatedText = item.created ? moment(item.created).format('DD/MM/YYYY HH:mm') : '';
                            item.packageNoCodeUpdatedTextNow = item.updated ? moment(item.updated).fromNow() : '';
                            item.packageNoCodeUpdatedText = item.updated ? moment(item.updated).format('DD/MM/YYYY HH:mm') : '';
                            item.packageNoCodeCommentNo = ko.observable(item.commentNo);
                            item.images = item.imageJson === null
                                ? []
                                : JSON.parse(item.imageJson);
                        });
                }

                self.packages(data.items);
                self.renderPager();
            });
    }

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumary_" + self.tabId).html(self.totalRecord() === 0 ? "No package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + "Package");

        $("#pager_" + self.tabId).pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pager_" + self.tabId).pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search(1);
}

function PackageReportModel() {
    var self = this;

    self.isLoading = ko.observable(false);

    self.warehouseId = ko.observable(null);
    self.warehouseCulture = ko.observable(null);
    self.reportData = ko.observable(null);
    self.cacheData = ko.observable(null);
    self.reportTitle = ko.observable("Daily report " + moment().format('DD/MM/YYYY'));
   
    
    self.titleToday = ko.observable("to day");
    self.selectDateReport = ko.observable("day");
    self.reportDate = ko.observable(moment());
    self.reportDateStart = ko.observable(moment().startOf('day'));
    self.reportDateEnd = ko.observable(moment().endOf('day'));
    self.tabs = ko.observableArray([]);

    self.warehouseId.subscribe(function(warehouseId) {
        var warehouse = _.find(window.warehouses, function (w) { return w.id === warehouseId });
        self.warehouseCulture(warehouse.culture);
    });

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Goods package", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

    self.warehouseCulture.subscribe(function() {
        self.reportMode();
    });

    self.btnNext = function () {
        if (self.selectDateReport() === 'day') {
            self.reportDate(self.reportDate().add(1, 'days'));
        } else if (self.selectDateReport() === 'week') {
            self.reportDate(self.reportDate().add(7, 'days'));
        } else {
            self.reportDate(self.reportDate().add(1, 'months'));
        }
        self.reportMode();
    }

    self.btnPre = function () {
        if (self.selectDateReport() === 'day') {
            self.reportDate(self.reportDate().add(-1, 'days'));
        } else if (self.selectDateReport() === 'week') {
            self.reportDate(self.reportDate().add(-7, 'days'));
        } else {
            self.reportDate(self.reportDate().add(-1, 'months'));
        }
        self.reportMode();
    }

    self.btnToday = function () {
        self.reportDate(moment(new Date()));
        self.reportMode();
    }

    self.btnCalendar = function () {
        document.getElementById("reportDate").focus();
    }

    self.btnSelect = function (name) {
        self.selectDateReport(name);
        self.titleToday(name === 'day' ? "today" : name === 'week' ? 'this week' : 'this month');
        self.reportMode();

        $('.report-date').datepicker("remove");
        if (self.selectDateReport() === 'day') {
            $('.report-date').datepicker({
                autoclose: true,
                language: 'vi'
            });
        } else if (self.selectDateReport() === 'week') {
            $('.report-date').datepicker({
                autoclose: true,
                language: 'vi'
            });
        } else {
            $('.report-date').datepicker({
                autoclose: true,
                minViewMode: 1,
                language: 'vi'
            });
        }
    }

    self.reportMode = function () {
        self.isLoading(true);
        if (self.selectDateReport() === 'day') {
            self.reportTitle("Báo cáo ngày " + self.reportDate().format('DD/MM/YYYY'));

            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
        } else if (self.selectDateReport() === 'week') {
            self.reportTitle("Báo cáo tuần " + self.reportDate().week() + '(' + self.reportDate().startOf('week').isoWeekday(1).format('DD/MM/YYYY') + ' - ' + self.reportDate().endOf('week').format('DD/MM/YYYY') + ')');

            self.reportDateStart(self.reportDate().startOf('week').isoWeekday(1).startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('week').endOf('day').format());
        } else {
            self.reportTitle("Báo cáo tháng " + self.reportDate().format('MM/YYYY'));

            self.reportDateStart(self.reportDate().startOf('month').format());
            self.reportDateEnd(self.reportDate().endOf('month').format());
        }

//        self.viewReportProfit();
//        self.viewReportBargainSituation();

        $.get("/PackageReport/GetPackage",
            {
                fromDate: self.reportDateStart(),
                toDate: self.reportDateEnd(),
                warehouseId: self.warehouseId(),
                warehouseCulture: self.warehouseCulture()
            },
            function (data) {
                self.processData(data);
                //console.log(data);
            });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "packageNoCode_" + data.id;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            $.post("/PackageNoCode/UpdateCommentNo",
                { id: data.id },
                function (rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    data.packageNoCodeCommentNo(data.packageNoCodeCommentNo() + 1);
                });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle("P" + data.packageCode);
        self.groupCommentBoxModelModal().pageTitle = "Package P" + data.packageCode;

        self.groupCommentBoxModelModal().pageUrl = "/PackageNoCode";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        //self.search(self.currentPage());
    });

    self.processData = function(data) {
        var fromDate = moment(self.reportDateStart());
        var toDate = moment(self.reportDateEnd());

        var categories = [];
        var times = {};

        var imported = {
            name: 'import warehouse',
            data: []
        };

        var exported = {
            name: 'export warehouse',
            data: []
        };

        var stored = {
            name: 'inventory',
            data: []
        };

        var lose = {
            name: 'Lost goods',
            data: []
        };

        var noCodeCreated = {
            name: ' create lost code',
            data: []
        };

        var noCodeUpdated = {
            name: 'Has processed code loss',
            data: []
        };

//        data["importedsGroup"]= _.groupBy(data.importeds, function (i) {return moment(i.createDate).format('DD/MM');});
//        data["exportedsGroup"] = _.groupBy(data.exporteds, function (i) {return moment(i.createDate).format('DD/MM');});
//        data["storedsGroup"] = _.groupBy(data.storeds, function (i) {return moment(i.createDate).format('DD/MM');});
//        data["losesGroup"] = _.groupBy(data.loses, function (i) {return moment(i.createDate).format('DD/MM');});

        do {
            var time = fromDate.format('DD/MM');
            var day = fromDate.format('D');
            categories.push(time);
            times[time] = fromDate.format("DD/MM/YYYY");

//            if (data["importedsGroup"][time]) {
//                data["importedsGroup"][time] = _.uniqBy(data["importedsGroup"][time], "id");
//            }
//            if (data["exportedsGroup"][time]) {
//                data["exportedsGroup"][time] = _.uniqBy(data["exportedsGroup"][time], "id");
//            }
//            if (data["storedsGroup"][time]) {
//                data["storedsGroup"][time] = _.uniqBy(data["storedsGroup"][time], "id");
//            }
//            if (data["losesGroup"][time]) {
//                data["losesGroup"][time] = _.uniqBy(data["losesGroup"][time], "id");
//            }

            imported.data.push(data.importeds[day] ? data.importeds[day] : 0);
            exported.data.push(data.exporteds[day] ? data.exporteds[day] : 0);
            stored.data.push(data.storeds[day] ? data.storeds[day] : 0);
            lose.data.push(data.loses[day] ? data.loses[day] : 0);
            noCodeCreated.data.push(data.noCodeCreateds[day] ? data.noCodeCreateds[day] : 0);
            noCodeUpdated.data.push(data.noCodeUpdateds[day] ? data.noCodeUpdateds[day] : 0);

            fromDate = fromDate.add(1, 'day');

        } while (fromDate <= toDate)

        self.reportData({
            times: times,
            categories: categories,
            imported: imported,
            exported: exported,
            stored: stored,
            lose: lose,
            noCodeCreated: noCodeCreated,
            noCodeUpdated: noCodeUpdated
        });

        //self.cacheData(data);

        self.rawChart(categories, [imported, exported, stored, lose, noCodeCreated, noCodeUpdated]);
        self.tabs.removeAll();
        self.isLoading(false);
    }

    self.rawChart = function (cateories, series) {
        window.Highcharts.chart('chartReport', {
            chart: {
                type: 'column'
            },
            title: {
                text: null
            },
//            subtitle: {
//                text: 'Source: WorldClimate.com'
//            },
            xAxis: {
                categories: cateories,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Package'
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.0f} kiện</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: series
        });
    }

    self.activeTab = function (tab) {
        _.each(self.tabs(),
            function (t) {
                t.isActive(false);
            });

        tab.isActive(true);
        tab.renderPager();
    }

    /**
     * 
     * @param {} serviceId : Id của dịch vụ. Nếu = 0 là tổng tiền dịch vụ: 254: Tiền thừa, 253: Đặt cọc
     * @param {} type : Loại: 0: Tổng Estimated revenue và phải thu, 1: Estimated revenue, 2: Phải thu, 3 Phải trả
     * @returns {} 
     */
    self.detail = function (type, time) {

        var tab = _.find(self.tabs(), function (t) { return t.type() === type && t.time() === time; });

        var isAddNew = false;
        if (tab == null) {
            isAddNew = true;
            tab = new PackageModel(type, time, self.reportData().times[time], self.warehouseId(), self.warehouseCulture());
        };
        _.each(self.tabs(),
            function (t) {
                t.isActive(false);
            });

        tab.isActive(true);

        setTimeout(function() { tab.renderPager(); }, 300);
        

        if (isAddNew)
            self.tabs.push(tab);
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


var modelView = new PackageReportModel();
ko.applyBindings(modelView, $("#packageReportModel")[0]);