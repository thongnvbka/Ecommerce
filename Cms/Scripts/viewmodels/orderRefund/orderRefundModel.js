function OrderRefundModel(orderDetailModel, refundUpdateModelModal) {
    var self = this;

    self.isLoading = ko.observable(false);

    // Search
    self.status = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.items = ko.observableArray();

    self.mode = ko.observable(window.modePage); // 0: Refund money, 1: Đổi trả
    self.createdNo = ko.observable(0);
    self.approvelNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    self.listStatus = ko.observableArray([{ id: 0, text: 'Newly created' }, { id: 1, text: 'Compeleted' }]);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, self.mode() === 0 ? "Refund money" : "Return the goods", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

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

        $("#sumaryPagerOrderRefund").html(self.totalRecord() === 0 ? "No tracking card " + (self.mode() === 0 ? "Refund money" : "Return the goods") : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + "Tracking card " + (self.mode() === 0 ? "Refund money" : "Return the goods"));

        $("#pagerOrderRefund").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerOrderRefund").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function(currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/OrderRefund/search",
            {
            mode: window.modePage,
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
                    item.status = ko.observable(item.status);
                    item.statusText = item.status() === 0 ? 'Newly created' : 'Closed';
                    item.statusClass = ko.computed(function () {
                        return item.status() === 0 ? 'label label-warning' : 'label label-success';
                    }, self);

                    item.createdTextNow = item.created ? moment(item.created).fromNow() : '';
                    item.createdText = item.created ? moment(item.created).format('DD/MM/YYYY HH:mm') : '';
                    item.commentNo = ko.observable(item.commentNo);

                    //item.packageNoCodeUpdatedTextNow = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).fromNow() : '';
                    //item.packageNoCodeUpdatedText = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).format('DD/MM/YYYY HH:mm') : '';
                    //item.commentNo = ko.observable(item.commentNo);
                    //item.images = item.packageNoCodeImageJson === null ? [] : JSON.parse(item.packageNoCodeImageJson);
                });

            self.items(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
        });
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "Refund_R" + data.code;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            $.post("/orderRefund/UpdateCommentNo",
                { refundId: data.id },
                function(rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    data.commentNo(data.commentNo() + 1);
                });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle( ReturnCode( data.orderCode));
        self.groupCommentBoxModelModal().pageTitle = (self.mode() === 0 ? "Refund money" : "Return the goods") +" " +  ReturnCode(data.orderCode);

        self.groupCommentBoxModelModal().pageUrl = "/orderRefund";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    self.showOrderDetail = function(orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }

    self.showDetail = function (id) {
        refundUpdateModelModal.showDetail(id, self.mode());
    }


    self.updateStatus = function (params) {
        var d = new $.Deferred();

        var data = { status: params.value, refundId: params.pk };
        data["__RequestVerificationToken"] = self.token;

        $.post("/OrderRefund/UpdateStatus",
            data,
            function(rs) {
                if (rs && rs.status > 0) {
                    toastr.success(rs.text);
                    d.resolve();
                    return d.promise();
                    self.search(1);
                } else {
                    toastr.warning(rs.text);
                    d.reject(rs.text);
                }
            });
    }

    self.token = $("#orderRefund input[name='__RequestVerificationToken']").val();

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

// Bind RefundUpdateModel
var refundUpdateModelModelView = new RefundUpdateModel();
ko.applyBindings(refundUpdateModelModelView, $("#refundUpdateModal")[0]);

var modelView = new OrderRefundModel(orderDetailViewModel, refundUpdateModelModelView);
ko.applyBindings(modelView, $("#orderRefund")[0]);