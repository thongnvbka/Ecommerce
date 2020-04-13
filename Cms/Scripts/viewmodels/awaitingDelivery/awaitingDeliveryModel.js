function AwaitingDeliveryModel(packageDetailModel, orderDetailModel, addDeliveryBillModal) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.warehouses = ko.observableArray(window["warehouses"] ? window.warehouses : []);
    self.systems = ko.observableArray(window["systems"] ? window.systems : []);
    self.states = ko.observableArray(window["states"] ? window.states : []);

    self.systemId = ko.observable(null);
    self.userId = ko.observable(null);
    self.orderType = ko.observable(null);
    self.keyword = ko.observable("");

    // list data
    self.items = ko.observableArray();

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Order", { isShowNotify: true, listUserTag: "/user/searchusertag" }));
    
    var systemIdFirst = true;
    self.systemId.subscribe(function () {
        if (systemIdFirst) {
            systemIdFirst = false;
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

        $("#sumaryPagerAwaitingDelivery").html(self.totalRecord() === 0 ? "There is not any customer" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " Customer");

        $("#pagerAwaitingDelivery").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerAwaitingDelivery").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function (currentPage) {
        self.currentPage(currentPage);
        self.isLoading(true);
        $.get("/AwaitingDelivery/search",
            {
                orderType: self.orderType(),
                systemId: self.systemId(),
                keyword: self.keyword(),
                currentPage: self.currentPage(),
                recordPerPage: self.recordPerPage()
            }, function (data) {
                self.isLoading(false);

            _.each(data.customers,
                function(c) {
                    var packages = _.filter(data.packages, function(p) { return p.customerId === c.id; });
                    c.packages = ko.observableArray(packages);

                    c.addressDetail = "";
                    
                    if (c.wardsName !== null && c.wardsName !== "") {
                        c.addressDetail = c.addressDetail.length > 0 ? c.addressDetail + ", " + c.wardsName : c.wardsName;
                    }
                    if (c.districtName !== null && c.districtName !== "") {
                        c.addressDetail = c.addressDetail.length > 0 ? c.addressDetail + ", " + c.districtName : c.districtName;
                    }
                    if (c.provinceName !== null && c.districtName !== "") {
                        c.addressDetail = c.addressDetail.length > 0 ? c.addressDetail + ", " + c.provinceName : c.provinceName;
                    }

                    c.weight = _.sumBy(packages, 'weight');
                    c.weightConverted = _.sumBy(packages, 'weightConverted');
                    c.weightActual = _.sumBy(packages, 'weightActual');

                    var callHistory = _.find(data.callHistories, function (ch) { return ch.customerId === c.id; });

                    if (callHistory) {
                        callHistory.created = moment(callHistory.created);
                        callHistory.createdText = callHistory.created.format("DD/MM/YYYY HH:ss");
                        callHistory.createdFromNow = callHistory.created.fromNow();
                    }

                    c.callHistory = ko.observable(callHistory);
                    c.debit = ko.observable(null);
                    c.isLoading = ko.observable(false);
                });

            self.items(data.customers);

            self.totalRecord(data.totalRecord);
            
            self.renderPager();
        });
    }

    self.AddCall = function (data) {
        swal({
            title: 'Confirm call for customer',
            inputPlaceholder: 'Calling content',
            confirmButtonText: 'Confirm',
           cancelButtonText: 'Cancel',
            input: 'textarea',
            showCancelButton: true,
            showLoaderOnConfirm: true,
            allowOutsideClick: false
        }).then(function(note) {
            $.post("/AwaitingDelivery/AddCustomerCallHistory",
                { customerId: data.id, note: note },
                function(rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    toastr.success(rs.text);

                    rs.data.created = moment(rs.data.created);
                    rs.data.createdText = rs.data.created.format("DD/MM/YYYY HH:ss");
                    rs.data.createdFromNow = rs.data.created.fromNow();

                    data.callHistory(rs.data);
                });
        }, function(){});
    }

    self.checkDebit = function (data) {
        var packageIds = ";" + _.join(_.map(data.packages(), "id"), ";") + ";";

        data.isLoading(true);
        $.post("/AwaitingDelivery/CheckDebit",
            { packageIds: packageIds },
            function (debit) {
                data.isLoading(false);
                data.debit(debit);
            });
    }

    self.addDeliveryBill = function(data) {
        var packageIds = ";" + _.join(_.map(data.packages(), "id"), ";") + ";";

        addDeliveryBillModal.showAndLoadData(packageIds, data.id, data.balanceAvalible);
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "order_" + data.orderCode;
        self.groupCommentBoxModelModal().callback = function () {
            //PlanRepo.UpdateCommentPending(data.Id, function () { });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle(ReturnCode(data.orderCode));
        self.groupCommentBoxModelModal().pageTitle = Order + ReturnCode(data.orderCode);

        // todo: Henry bổ xung xin khi click vào thông báo
        //self.groupCommentBoxModelModal().pageUrl = "/";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    //self.detailModal = ko.observable(null);
    self.showDetail = function (data) {
        if (packageDetailModel) {
            packageDetailModel.showModel(data);
            return;
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }

    $(function() {
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

// Bind AddDeliveryBill
var addDeliveryBillModelView = new AddDeliveryBillModel();
ko.applyBindings(addDeliveryBillModelView, $("#addDeliveryBillModal")[0]);

// Bind PackageModel
var modelView = new AwaitingDeliveryModel(packageDetailModelView, orderDetailViewModel, addDeliveryBillModelView);
ko.applyBindings(modelView, $("#awaitingDelivery")[0]);


addDeliveryBillModelView.addSuccessCallback = function() {
    modelView.search(modelView.currentPage());
}
