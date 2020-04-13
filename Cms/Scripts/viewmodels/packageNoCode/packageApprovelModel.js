function PackageNoCodeModel(packageDetailModel, orderDetailModel,  depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.packages = ko.observableArray();

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
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "package", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

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
        $.get("/packageNoCode/search",
            {
            mode: window.modePage,
            tabMode: self.tabMode(),
            status: self.status(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword(),
            currentPage: self.currentPage(),
            recordPerPage: self.recordPerPage()
        }, function(data) {
            self.isLoading(false);

            _.each(data.items,
                function (item) {
                    item.statusText = item.packageNoCodeStatus === 0 ? 'Newly created' : 'Closed';
                    item.statusClass = item.packageNoCodeStatus === 0 ? 'label label-warning' : 'label label-success';
                    item.packageNoCodeCreatedTextNow = item.packageNoCodeCreated ? moment(item.packageNoCodeCreated).fromNow() : '';
                    item.packageNoCodeCreatedText = item.packageNoCodeCreated ? moment(item.packageNoCodeCreated).format('DD/MM/YYYY HH:mm') : '';
                    item.packageNoCodeUpdatedTextNow = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).fromNow() : '';
                    item.packageNoCodeUpdatedText = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).format('DD/MM/YYYY HH:mm') : '';
                    item.packageNoCodeCommentNo = ko.observable(item.packageNoCodeCommentNo);
                    item.images = item.packageNoCodeImageJson === null ? [] : JSON.parse(item.packageNoCodeImageJson);
                });

            self.createdNo(data.createdNo);
            self.approvelNo(data.approvelNo);
            self.allNo(data.allNo);
            self.packages(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
        });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "packageNoCode_" + data.packageNoCodeId;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            $.post("/PackageNoCode/UpdateCommentNo",
                { id: data.packageNoCodeId },
                function(rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    data.packageNoCodeCommentNo(data.packageNoCodeCommentNo() + 1);
                });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle("P" + data.code);
        self.groupCommentBoxModelModal().pageTitle = "package P" + data.code;

        self.groupCommentBoxModelModal().pageUrl = "/PackageNoCode";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    self.addPackageLoseForm = ko.observable(null);
    self.showUpdatePackageLose = function (data) {
        if (self.addPackageLoseForm() == null) {
            self.addPackageLoseForm(new AddPackageLoseModel());
            ko.applyBindings(self.addPackageLoseForm(), $("#addPackageLoseModal")[0]);
        }

        self.addPackageLoseForm().setUpdate(data.packageNoCodeId, data.code, data.transportCode, data.packageNoCodeNote, data.images);
    }

    $('#addPackageLoseModal').on('hide', function () {
        self.search(self.currentPage());
    });


    self.callbackAssignPackage = function () {
        self.search(self.currentPage());
    }
    self.assignPackageForm = ko.observable();
    self.showAddForOrder = function(data) {
        if (self.assignPackageForm() == null) {
            self.assignPackageForm(new AssignPackageModel(self.callbackAssignPackage));
            ko.applyBindings(self.assignPackageForm(), $("#assignPackageModal")[0]);
        }

        self.assignPackageForm().setUpdate(data.packageNoCodeId, data.transportCode, data.code);
    }

    //self.detailModal = ko.observable(null);
    self.showDetail = function(data) {
        if (packageDetailModel) {
            packageDetailModel.showModel(data);
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
var modelView = new PackageNoCodeModel(packageDetailModelView, orderDetailViewModel, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#package")[0]);