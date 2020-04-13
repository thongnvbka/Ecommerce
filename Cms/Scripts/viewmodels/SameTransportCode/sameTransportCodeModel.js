function SameTransportCodeModel(packageDetailModel, orderDetailModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.status = ko.observable(null);
    self.orderType = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.packages = ko.observableArray();

    self.packageStatus = ko.observable(null);

    self.tabMode = ko.observable(0);
    self.createdNo = ko.observable(0);
    self.approvelNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Duplicate bill of Lading", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

    self.changeTabMode = function (tabMode) {
        if (tabMode !== self.tabMode()) {
            self.tabMode(tabMode);
            self.search(1);
        }
    }

    var statusFirst = true;
    self.status.subscribe(function () {
        if (statusFirst) {
            statusFirst = false;
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

        $("#sumaryPagerPackage").html(self.totalRecord() === 0 ? "There is no duplicate bill of lading code" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " Bill of lading duplicated");

        $("#pagerPackage").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPackage").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/SameTransportCode/search",
            {
                orderType: self.orderType(),
                mode: window.modePage,
                tabMode: self.tabMode(),
                status: self.status(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                var firstTransportCode = null;
                var firstWarehouseId = null;
                var firstOrderId = null;
                self.packageStatus(data.packageStatus);

            var stt = 0;
            _.each(data.packages,
                function (p) {
                    p.sameCodeStatus = ko.observable(p.sameCodeStatus);
                    // Mã vân đơn
                    p.firstTransportCode = firstTransportCode !== p.transportCode;

                    if (p.firstTransportCode) {
                        p.warehouseNo = _.filter(data.packages,
                            function(pp) {
                                return pp.transportCode === p.transportCode;
                            }).length;
                        stt = stt + 1;
                        p.countNo = stt;
                    }

                    // kho hàng
                    p.firstWarehouseId = firstTransportCode !== p.transportCode || firstWarehouseId !== p.customerWarehouseId;

                    if (p.firstWarehouseId)
                        p.orderNo = _.filter(data.packages,
                            function (pp) {
                                return pp.transportCode === p.transportCode &&
                                    pp.customerWarehouseId === p.customerWarehouseId;
                            }).length;

                    // package
                    p.firstOrderId = firstTransportCode !== p.transportCode || firstWarehouseId !== p.customerWarehouseId || firstOrderId !== p.orderId;

                    if (p.firstOrderId)
                        p.packageNo = _.filter(data.packages,
                            function(pp) {
                                return pp.transportCode === p.transportCode &&
                                    pp.customerWarehouseId === p.customerWarehouseId &&
                                    pp.orderId === p.orderId;
                            }).length;

                    firstTransportCode = p.transportCode;
                    firstWarehouseId = p.customerWarehouseId;
                    firstOrderId = p.orderId;
                });

                //self.createdNo(data.createdNo);
                //self.approvelNo(data.approvelNo);
                //self.allNo(data.allNo);
                self.packages(data.packages);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "sameTransportCode_" + data.transportCode;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            //$.post("/PackageNoCode/UpdateCommentNo",
            //    { id: data.packageNoCodeId },
            //    function (rs) {
            //        if (rs && rs.status < 0) {
            //            toastr.warning(rs.text);
            //            return;
            //        }
            //        data.packageNoCodeCommentNo(data.packageNoCodeCommentNo() + 1);
            //    });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle("MVĐ-" +data.transportCode);
        self.groupCommentBoxModelModal().pageTitle = "MVĐ: " + data.transportCode;

        self.groupCommentBoxModelModal().pageUrl = "/SameTransportCode";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    //self.detailModal = ko.observable(null);
    self.showDetail = function (data) {
        if (packageDetailModel) {
            packageDetailModel.showModel(data.id);
            return;
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }
    self.listStatus = ko.observableArray([{ id: 0, text: 'Waiting for progressing' }, { id: 1, text: 'Processed' }]);
    self.updateStatus = function (params) {
        var d = new $.Deferred();

        var data = { status: params.value, transportCode: params.pk };
        data["__RequestVerificationToken"] = self.token;

        $.post("/SameTransportCode/UpdateStatus",
            data,
            function (rs) {
                if (rs && rs.status > 0) {
                    toastr.success(rs.text);
                    d.resolve();
                    return d.promise();
                } else {
                    toastr.warning(rs.text);
                    d.reject(rs.text);
                }
            });
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

        self.search(1);
    });
}

// Bind PackageDetail
var packageDetailModelView = new PackageDetail(window.states);
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// Bind PackageModel
var modelView = new SameTransportCodeModel(packageDetailModelView, orderDetailViewModel);
ko.applyBindings(modelView, $("#sameTransportCode")[0]);