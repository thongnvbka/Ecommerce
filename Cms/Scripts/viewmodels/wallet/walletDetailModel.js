function WalletDetailModel(allWarehouse, walletStates, orderPackageStates, packageDetailModal, orderDetailModal) {
    var self = this;

    self.allWarehouse = ko.observableArray(allWarehouse);
    self.isLoading = ko.observable(false);
    self.isLoadingItems = ko.observable(false);
    self.suggetionType = ko.observable(0); // 0: package, 1: Bao hàng
    self.statesGroupId = _.groupBy(walletStates, 'id');

    self.items = ko.observableArray([]);

    // model
    self.id = ko.observable(0);
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.status = ko.observable(1);
    self.targetWarehouseId = ko.observable(null);
    self.targetWarehouseIdPath = ko.observable("");
    self.targetWarehouseName = ko.observable("");
    self.targetWarehouseAddress = ko.observable("");
    self.userId = ko.observable(0);
    self.userName = ko.observable("");
    self.userFullName = ko.observable("");
    self.created = ko.observable();
    self.updated = ko.observable();
    self.packageNo = ko.observable(0);
    self.totalValue = ko.observable(null);
    self.totalWeight = ko.observable(null);
    self.totalWeightConverted = ko.observable(null);
    self.totalWeightActual = ko.observable(null);
    self.currentWarehouseId = ko.observable(0);
    self.currentWarehouseIdPath = ko.observable("");
    self.currentWarehouseName = ko.observable("");
    self.currentWarehouseAddress = ko.observable("");
    self.createdWarehouseId = ko.observable(0);
    self.createdWarehouseIdPath = ko.observable("");
    self.createdWarehouseName = ko.observable("");
    self.createdWarehouseAddress = ko.observable("");

    self.restValue = function () {
        self.id(0);
        self.note("");
        self.code("");
        self.status(1);
        self.targetWarehouseId(null);
        self.targetWarehouseIdPath("");
        self.targetWarehouseName("");
        self.targetWarehouseAddress("");
        self.userId(0);
        self.userName("");
        self.userFullName("");
        self.created();
        self.updated();
        self.packageNo(0);
        self.totalValue(null);
        self.totalWeight(null);
        self.totalWeightConverted(null);
        self.totalWeightActual(null);
        self.currentWarehouseId(0);
        self.currentWarehouseIdPath("");
        self.currentWarehouseName("");
        self.currentWarehouseAddress("");
        self.createdWarehouseId(0);
        self.createdWarehouseIdPath("");
        self.createdWarehouseName("");
        self.createdWarehouseAddress("");

        self.isLoadingItems(false);
        self.isLoading(false);

        self.items.removeAll();
    }

    self.setValue = function (data) {
        self.id(data.id);
        self.note(data.note);
        self.code(data.code);
        self.status(data.status);
        self.targetWarehouseId(data.targetWarehouseId);
        self.targetWarehouseIdPath(data.targetWarehouseIdPath);
        self.targetWarehouseName(data.targetWarehouseName);
        self.targetWarehouseAddress(data.targetWarehouseAddress);
        self.userId(data.userId);
        self.userName(data.userName);
        self.userFullName(data.userFullName);
        self.created(data.created);
        self.updated(data.updated);
        self.packageNo(data.packageNo);
        self.totalValue(data.totalValue);
        self.totalWeight(data.totalWeight);
        self.totalWeightConverted(data.totalWeightConverted);
        self.totalWeightActual(data.totalWeightActual);
        self.currentWarehouseId(data.currentWarehouseId);
        self.currentWarehouseIdPath(data.currentWarehouseIdPath);
        self.currentWarehouseName(data.currentWarehouseName);
        self.currentWarehouseAddress(data.currentWarehouseAddress);
        self.createdWarehouseId(data.createdWarehouseId);
        self.createdWarehouseIdPath(data.createdWarehouseIdPath);
        self.createdWarehouseName(data.createdWarehouseName);
        self.createdWarehouseAddress(data.createdWarehouseAddress);

        // Get detail của phiếu nhập kho
        self.getDetail();
    }

    self.getDetail = function () {
        self.isLoadingItems(true);
        $.get("/wallet/getpackages", { id: self.id() }, function (data) {
            self.isLoadingItems(false);

            var firstOrderCode;
            _.each(data, function (it) {
                it.isFirst = ko.observable(firstOrderCode !== it.orderCode);
                firstOrderCode = it.orderCode;

                if (it.isFirst) {
                    it.packageNoInWallet = _.filter(data, function (d) {
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
            $("#walletDetailModal").modal("show");
            return;
        }

        // Tham so truyen vao la id hoac la url cua link get data
        var url = typeof data === "string" ? data : "/wallet/getdetail/" + data;

        self.isLoading(true);
        $.get(url, function (rsData) {
            self.isLoading(false);
            self.setValue(rsData);
            $("#walletDetailModal").modal("show");
        });
    }

    
    self.showPackageDetail = function (packageId) {
        if (window.hasOwnProperty("packageDetailModelView")) {
            packageDetailModelView.showModel(packageId);
        }
    }

    // Xem Detail Orders
    self.showOrderDetail = function (orderId) {
        if (window.hasOwnProperty("orderDetailViewModel")) {
            window.orderDetailViewModel.viewOrderDetail(orderId);
            return;
        }
    }

    //self.showAddForm = function () {
    //    self.resetForm();
    //    resetForm("#walletForm");
    //    $("#walletAddOrEdit").modal("show");
    //}

    self.loadingItems = function (walletId) {
        $.get("/wallet/getpackages", { id: walletId }, function (data) {
            self.items(data);
        });
    }
}