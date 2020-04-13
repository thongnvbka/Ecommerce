function AccountingLoseModel(orderDetailModel, requestAccountingLoseModal, changeStatusModal, refundUpdateModelModal,  depositDetailViewModel, orderCommerceDetailViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.orders = ko.observableArray();

    self.tabMode = ko.observable(0);
    self.createdNo = ko.observable(0);
    self.handleNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Order", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

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

        $("#sumaryPagerPackage").html(self.totalRecord() === 0 ? "No package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " đến " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " của " + self.totalRecord() + " package");

        $("#pagerPackage").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPackage").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/acountinglose/search",
            {
                tabMode: self.tabMode(),
                status: self.status(),
                fromDate: self.fromDate(),
                toDate: self.toDate(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

                _.each(data.items,
                    function (item) {
                        item.statusText = item.requestStatus === 0 ? 'Newly created' : item.requestStatus === 1 ? 'Being processed' : 'Closed';
                       item.statusClass = item.requestStatus === 0 ? 'label label-danger' : item.requestStatus === 1 ? 'label label-warning' : 'label label-success';
                       item.createdTextNow = item.created ? moment(item.created).fromNow() : '';
                       item.createdText = item.created ? moment(item.created).format('DD/MM/YYYY HH:mm') : '';
                       item.commentNo = ko.observable(item.commentNo);
                   });

                self.createdNo(data.createdNo);
                self.handleNo(data.handleNo);
                self.allNo(data.allNo);
                self.orders(data.items);
                self.totalRecord(data.totalRecord);
                self.renderPager();
            });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "orderCoutingLose_" + data.id;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            $.post("/AcountingLose/UpdateCommentNo",
                { orderId: data.id },
                function (rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    data.commentNo(data.commentNo() + 1);
                });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle(ReturnCode(data.code));
        self.groupCommentBoxModelModal().pageTitle = Order + ReturnCode(data.code);

        self.groupCommentBoxModelModal().pageUrl = "/AcountingLose";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    self.showOrderDetail = function (orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }

    self.closeRequest = function(data) {
        changeStatusModal.show(data.id, data.code, data.requestStatus, data.noteProcess);
        changeStatusModal.callback = function() {
            self.search(1);
        };
    }

    self.addRefund = function(orderId, mode) {
        refundUpdateModelModal.showAdd(orderId, mode);
    }

    self.showRequest = function(data) {
        requestAccountingLoseModal.show(data.id, data.code);
    }

    $(function () {
        $('#forcastDate-btn').daterangepicker({
            ranges: {
                'Today': [moment(), moment()],
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

var imageViewModel = new ViewImageModel();
ko.applyBindings(imageViewModel, $("#viewImageModal")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind requestAcoutingLose
var requestAccountingLoseModelView = new RequestAccountingLoseModel(imageViewModel);
ko.applyBindings(requestAccountingLoseModelView, $("#requestAcoutingLoseModalBinding")[0]);

// Bind changeStatus
var changeStatusModelView = new ChangeStatusModel();
ko.applyBindings(changeStatusModelView, $("#changeStatusModal")[0]);

// Bind RefundUpdateModel
var refundUpdateModelModelView = new RefundUpdateModel();
ko.applyBindings(refundUpdateModelModelView, $("#refundUpdateModal")[0]);

// Bind PackageModel
var modelView = new AccountingLoseModel(orderDetailViewModel, requestAccountingLoseModelView, changeStatusModelView, refundUpdateModelModelView, depositDetailViewModel, orderCommerceDetailViewModel);
ko.applyBindings(modelView, $("#accountingLose")[0]);