function PackageModel(packageDetailModel, orderDetailModel, depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.systems = ko.observableArray(window["systems"] ? window.systems : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);

    self.warehouseIdPath = ko.observable("");
    self.systemId = ko.observable(null);
    self.userId = ko.observable(null);
    self.status = ko.observable(null);
    self.day = ko.observable(null);
    self.orderType = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.packages = ko.observableArray();

    self.mode = ko.observable(2);
    self.inStockNo = ko.observable(0);
    self.waitImportNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Order", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

    self.changeMode = function (mode) {
        if (mode !== self.mode()) {
            self.mode(mode);
            self.search(1);
        }
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

    var systemIdFirst = true;
    self.systemId.subscribe(function () {
        if (systemIdFirst) {
            systemIdFirst = false;
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

    var dayFirst = true;
    self.day.subscribe(function () {
        if (dayFirst) {
            dayFirst = false;
            return;
        }
        self.search(1);
    });

    var orderTypeFirst = true;
    self.orderType.subscribe(function () {
        if (orderTypeFirst) {
            orderTypeFirst = false;
            return;
        }
        self.search(1);
    });

    self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerPackage").html(self.totalRecord() === 0 ? "No package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " package");

        $("#pagerPackage").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPackage").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function(currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/package/search",
            {
            orderType: self.orderType(),
            mode: self.mode(),
            warehouseIdPath: self.warehouseIdPath(),
            systemId: self.systemId(),
            userId: self.userId(),
            status: self.status(),
            day: self.day(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword(),
            currentPage: self.currentPage(),
            recordPerPage: self.recordPerPage()
        }, function(data) {
            self.isLoading(false);

            var statesGroup = _.groupBy(window.states, "id");

            var orderFirst = null;
            _.each(data.items,
                function (item) {
                    item.isFirst = item.orderCode !== orderFirst;
                    orderFirst = item.orderCode;

                    item.statusText = statesGroup[item.status + ''][0].name;

                    item.statusClass = item.status === 0
                        ? 'label label-warning'
                        : item.status === 1
                        ? 'label label-success'
                        : item.status === 2
                        ? 'label label-info'
                        : item.status === 4 ? 'label label-primary' : 'label label-danger';

                    item.forcastDateText = item.forcastDate ? moment(item.forcastDate).fromNow() : '';
                });

            self.inStockNo(data.mode.inStockNo);
            self.waitImportNo(data.mode.waitImportNo);
            self.allNo(data.mode.allNo);
            self.packages(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
        });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "order_" + data.orderCode;
        self.groupCommentBoxModelModal().callback = function () {
            //PlanRepo.UpdateCommentPending(data.Id, function () { });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle( ReturnCode( data.orderCode));
        self.groupCommentBoxModelModal().pageTitle = Order +  ReturnCode(data.orderCode);

        // todo: Henry bổ xung xin khi click vào thông báo
        //self.groupCommentBoxModelModal().pageUrl = "/";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    //self.detailModal = ko.observable(null);
    self.showDetail = function(data) {
        if (packageDetailModel) {
            packageDetailModel.showModel(data.id);
            return;
        }
    }

    self.showOrderDetail = function(orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }

    $(function() {
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

        self.search(1);
    });
}

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

// Bind PackageModel
var modelView = new PackageModel(packageDetailModelView, orderDetailViewModel, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#package")[0]);