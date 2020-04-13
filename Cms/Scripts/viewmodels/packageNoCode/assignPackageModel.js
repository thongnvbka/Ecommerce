function AssignPackageModel(callback) {
    var self = this;

    self.isLoading = ko.observable();
    self.id = ko.observable(0);
    self.orderId = ko.observable(null);
    self.transportCode = ko.observable("");
    self.note = ko.observable("");
    self.packageCode = ko.observable("");

    self.resetForm = function () {
        self.id(0);
        self.orderId(null);
        self.transportCode("");
        self.packageCode("");
        self.note("");
    }

    self.setUpdate = function (packageNoCodeId, transportCode, packageCode) {
        self.resetForm();
        resetForm("#assignPackageForm");

        self.id(packageNoCodeId);
        self.transportCode(transportCode);
        self.packageCode(packageCode);

        $("#assignPackageModal").modal("show");
    }


    self.chooseOrder = function (order) {
        if (order) {
            self.orderId(order.id);
            return;
        }
        self.orderId(null);
    }

    self.save = function () {
        if (self.orderId() == null) {
            toastr.error("Orders are compulsory to enter");
            return;
        }

        if ($.trim(self.transportCode()) === "") {
            toastr.error("Transport code are compulsory to enter");
            return;
        }

        var data = {
            packageNoCodeId: self.id(),
            transportCode: self.transportCode(),
            orderId: self.orderId(),
            note: self.note()
        };

        self.isLoading(true);
        $.post("/order/AssignPackage",
            data, function (rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.msg);
                    return;
                }

                toastr.success(rs.msg);
                self.resetForm();
                $("#assignPackageModal").modal("hide");

                if (callback)
                    callback();
            });
    }
}