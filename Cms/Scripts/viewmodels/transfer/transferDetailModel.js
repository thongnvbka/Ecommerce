function TransferDetailModel(allWarehouse, transferStates) {
    var self = this;

    self.allWarehouse = ko.observableArray(allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.suggetionType = ko.observable(0); // 0: package, 1: Bao hàng
    self.statesGroupId = _.groupBy(transferStates, 'id');

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(null);
    self.code = ko.observable(null);
    self.status = ko.observable(null);
    self.totalWeight = ko.observable(null);
    self.totalWeightConverted = ko.observable(null);
    self.totalWeightActual = ko.observable(null);
    self.walletNo = ko.observable(null);
    self.packageNo = ko.observable(null);
    self.note = ko.observable(null);
    self.fromUserId = ko.observable(null);
    self.fromUserFullName = ko.observable(null);
    self.fromUserUserName = ko.observable(null);
    self.fromUserTitleId = ko.observable(null);
    self.fromUserTitleName = ko.observable(null);
    self.fromWarehouseId = ko.observable(null);
    self.fromWarehouseName = ko.observable(null);
    self.fromWarehouseIdPath = ko.observable(null);
    self.fromTime = ko.observable(null);
    self.toUserId = ko.observable(null);
    self.toUserFullName = ko.observable(null);
    self.toUserUserName = ko.observable(null);
    self.toUserTitleId = ko.observable(null);
    self.toUserTitleName = ko.observable(null);
    self.toWarehouseId = ko.observable(null);
    self.toWarehouseName = ko.observable(null);
    self.toWarehouseIdPath = ko.observable(null);
    self.toTime = ko.observable(null);
    self.priceShip = ko.observable(null);

    self.restValue = function () {
        self.id(null);
        self.code(null);
        self.status(null);
        self.totalWeight(null);
        self.totalWeightConverted(null);
        self.totalWeightActual(null);
        self.walletNo(null);
        self.packageNo(null);
        self.note(null);
        self.fromUserId(null);
        self.fromUserFullName(null);
        self.fromUserUserName(null);
        self.fromUserTitleId(null);
        self.fromUserTitleName(null);
        self.fromWarehouseId(null);
        self.fromWarehouseName(null);
        self.fromWarehouseIdPath(null);
        self.fromTime(null);
        self.toUserId(null);
        self.toUserFullName(null);
        self.toUserUserName(null);
        self.toUserTitleId(null);
        self.toUserTitleName(null);
        self.toWarehouseId(null);
        self.toWarehouseName(null);
        self.toWarehouseIdPath(null);
        self.toTime(null);
        self.priceShip(null);

        self.isLoadingItems(false);
        self.isLoading(false);

        self.items.removeAll();
    }

    self.setValue = function (data) {
        self.id(data.id);
        self.code(data.code);
        self.status(data.status);
        self.totalWeight(data.totalWeight);
        self.totalWeightConverted(data.totalWeightConverted);
        self.totalWeightActual(data.totalWeightActual);
        self.walletNo(data.walletNo);
        self.packageNo(data.packageNo);
        self.note(data.note);
        self.fromUserId(data.fromUserId);
        self.fromUserFullName(data.fromUserFullName);
        self.fromUserUserName(data.fromUserUserName);
        self.fromUserTitleId(data.fromUserTitleId);
        self.fromUserTitleName(data.fromUserTitleName);
        self.fromWarehouseId(data.fromWarehouseId);
        self.fromWarehouseName(data.fromWarehouseName);
        self.fromWarehouseIdPath(data.fromWarehouseIdPath);
        self.fromTime(data.fromTime);
        self.toUserId(data.toUserId);
        self.toUserFullName(data.toUserFullName);
        self.toUserUserName(data.toUserUserName);
        self.toUserTitleId(data.toUserTitleId);
        self.toUserTitleName(data.toUserTitleName);
        self.toWarehouseId(data.toWarehouseId);
        self.toWarehouseName(data.toWarehouseName);
        self.toWarehouseIdPath(data.toWarehouseIdPath);
        self.toTime(data.toTime);
        self.priceShip(data.priceShip);

        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/transfer/getpackages", { id: self.id() }, function (data) {
            self.isLoadingItems(false);

            var firstOrderCode;
            _.each(data, function (it) {
                it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
                firstOrderCode = it.orderCode;

                if (it.isFirst) {
                    it.packageNoInTransfer = _.filter(data, function (d) {
                        return d.orderCode === it.orderCode;
                    }).length;
                }
            });

            self.items(data);
        });
    }

    //self.showOrderDetail = function (orderId){
    //    if (orderDetailModal) {
    //        orderDetailModal.viewOrderDetail(orderId);
    //    }
    //}

    //self.showPackageDetail = function(packageId) {
    //    if (packageDetailModal) {
    //        packageDetailModal.showModel(packageId);
    //    }
    //}

    self.showModel = function (data) {
        self.restValue();

        // Tham so truyen vao la object data show luon model
        if (typeof data === "object") {
            self.setValue(data);
            $("#transferDetailModal").modal("show");
            return;
        }

        // Tham so truyen vao la id hoac la url cua link get data
        var url = typeof data === "string" ? data : "/transfer/getdetail/" + data;

        self.isLoading(true);
        $.get(url, function (rs) {
            self.isLoading(false);
            self.setValue(rs);
        });

        $("#transferDetailModal").modal("show");
    }

    
    // Xem Detail Kiện
    self.showDetailPackage = function (data) {
        if (window.hasOwnProperty("packageDetailModelView")) {
            packageDetailModelView.showModel(data);
            return;
        }
    }

    // Xem Detail Orders
    self.showOrderDetail = function (orderId) {
        if (window.hasOwnProperty("orderDetailViewModel")) {
            orderDetailViewModel.viewOrderDetail(orderId);
            return;
        }
    }

    self.showOrderDispositDetail = function (orderId) {
        if (window.hasOwnProperty("depositDetailViewModel")) {
            window.depositDetailViewModel.showModalDialog(orderId);
            return;
        }
    }

    self.showOrderCareDetail = function (orderId) {
        if (window.hasOwnProperty("orderCommerce")) {
            window.orderCommerce.viewOrderAdd(orderId);
            return;
        }
    }

    //self.showAddForm = function () {
    //    self.resetForm();
    //    resetForm("#transferForm");
    //    $("#transferAddOrEdit").modal("show");
    //}

    self.loadingItems = function (transferId) {
        $.get("/transfer/getpackages", { id: transferId }, function (data) {
            self.items(data);
        });
    }
}