function PackageDetail(states) {
    var self = this;
    self.isLoading = ko.observable(false);
    if (states == undefined) {
        $.ajax({
            type: 'POST',
            url: '/Package/GetStatus',
            success: function (result) {
                states = result;
            },
            async: false
        });
    }

    self.statesGroupBy = _.groupBy(states, "id");

    self.code = ko.observable("");
    self.created = ko.observable(null);
    self.currentWarehouseAddress = ko.observable(null);
    self.currentWarehouseId = ko.observable(null);
    self.currentWarehouseIdPath = ko.observable(null);
    self.currentWarehouseName = ko.observable(null);
    self.customerId = ko.observable(null);
    self.customerLevelId = ko.observable(null);
    self.customerLevelName = ko.observable(null);
    self.customerName = ko.observable(null);
    self.customerUserName = ko.observable(null);
    self.customerWarehouseAddress = ko.observable(null);
    self.customerWarehouseId = ko.observable(null);
    self.customerWarehouseIdPath = ko.observable(null);
    self.customerWarehouseName = ko.observable(null);
    self.forcastDate = ko.observable(null);
    self.hashTag = ko.observable(null);
    self.height = ko.observable(null);
    self.id = ko.observable(0);
    self.lastUpdate = ko.observable(null);
    self.length = ko.observable(0);
    self.note = ko.observable("");
    self.orderCode = ko.observable("");
    self.orderId = ko.observable(0);
    self.packageNo = ko.observable(0);
    self.size = ko.observable(null);
    self.status = ko.observable(0);
    self.systemId = ko.observable(null);
    self.systemName = ko.observable(null);
    self.totalPrice = ko.observable(0);
    self.transportCode = ko.observable("");
    self.userFullName = ko.observable("");
    self.userId = ko.observable(0);
    self.warehouseAddress = ko.observable("");
    self.warehouseId = ko.observable("");
    self.warehouseIdPath = ko.observable("");
    self.warehouseName = ko.observable("");
    self.weight = ko.observable(null);
    self.weightActual = ko.observable(null);
    self.weightConverted = ko.observable(null);
    self.width = ko.observable(null);
    self.histories = ko.observableArray([]);
    self.packageNotes = ko.observableArray([]);
    self.packageNoteMode = ko.observable(null);

    self.restValue = function () {
        self.isLoading = ko.observable(false);
        self.code("");
        self.created(null);
        self.currentWarehouseAddress(null);
        self.currentWarehouseId(null);
        self.currentWarehouseIdPath(null);
        self.currentWarehouseName(null);
        self.customerId(null);
        self.customerLevelId(null);
        self.customerLevelName(null);
        self.customerName(null);
        self.customerUserName(null);
        self.customerWarehouseAddress(null);
        self.customerWarehouseId(null);
        self.customerWarehouseIdPath(null);
        self.customerWarehouseName(null);
        self.forcastDate(null);
        self.hashTag(null);
        self.height(null);
        self.id(0);
        self.lastUpdate(null);
        self.length(0);
        self.note("");
        self.orderCode("");
        self.orderId(0);
        self.packageNo(0);
        self.size(null);
        self.status(0);
        self.systemId(null);
        self.systemName(null);
        self.totalPrice(0);
        self.transportCode("");
        self.userFullName("");
        self.userId(0);
        self.warehouseAddress("");
        self.warehouseId("");
        self.warehouseIdPath("");
        self.warehouseName("");
        self.weight(null);
        self.weightActual(null);
        self.weightConverted(null);
        self.width(null);
        self.histories([]);
        self.packageNotes([]);
        self.packageNoteMode(null);
    }

    self.setValue = function (data) {
        self.code(data.code);
        self.created(data.created);
        self.currentWarehouseAddress(data.currentWarehouseAddress);
        self.currentWarehouseId(data.currentWarehouseId);
        self.currentWarehouseIdPath(data.currentWarehouseIdPath);
        self.currentWarehouseName(data.currentWarehouseName);
        self.customerId(data.customerId);
        self.customerLevelId(data.customerLevelId);
        self.customerLevelName(data.customerLevelName);
        self.customerName(data.customerName);
        self.customerUserName(data.customerUserName);
        self.customerWarehouseAddress(data.customerWarehouseAddress);
        self.customerWarehouseId(data.customerWarehouseId);
        self.customerWarehouseIdPath(data.customerWarehouseIdPath);
        self.customerWarehouseName(data.customerWarehouseName);
        self.forcastDate(data.forcastDate);
        self.hashTag(data.hashTag);
        self.height(data.height);
        self.id(data.id);
        self.lastUpdate(data.lastUpdate);
        self.length(data.length);
        self.note(data.note);
        self.orderCode(data.orderCode);
        self.orderId(data.orderId);
        self.packageNo(data.packageNo);
        self.size(data.size);
        self.status(data.status);
        self.systemId(data.systemId);
        self.systemName(data.systemName);
        self.totalPrice(data.totalPrice);
        self.transportCode(data.transportCode);
        self.userFullName(data.userFullName);
        self.userId(data.userId);
        self.warehouseAddress(data.warehouseAddress);
        self.warehouseId(data.warehouseId);
        self.warehouseIdPath(data.warehouseIdPath);
        self.warehouseName(data.warehouseName);
        self.weight(data.weight);
        self.weightActual(data.weightActual);
        self.weightConverted(data.weightConverted);
        self.width(data.width);
    }

    self.showModel = function (data) {
        self.restValue();

        // Tham so truyen vao la object data show luon model
        if (typeof data === "object") {
            self.setValue(data);
            $("#packageDetail").modal("show");
            return;
        }

        // Tham so truyen vao la id hoac la url cua link get data
        var url = typeof data === "string" ? data : "/package/getdetail/" + data;

        self.isLoading(true);
        $.get(url, function (rs) {
            self.isLoading(false);
            if (rs.package) {
                self.histories(rs.packageHistories);
                self.packageNoteMode(rs.packageNoteMode);
                self.packageNotes(rs.packageNotes);
                self.setValue(rs.package);
            }
        });

        $("#packageDetail").modal("show");
    }

    $(function () {
        $("#packageDetail .btn-primary").click(function () {
            $("body").addClass("printModal");
            $("#packageDetail").addClass('modalPrint');
            window.print();
            $("body").removeClass("printModal");
            $("#packageDetail").removeClass('modalPrint');
        });
    });


}