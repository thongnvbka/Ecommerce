function AddPackageModel() {
    var self = this;

    self.isLoading = ko.observable();
    self.orderId = ko.observable(null);
    self.transportCode = ko.observable("");
    self.packageId = ko.observable(null);
    self.note = ko.observable("");
    self.callback = null;

    self.resetForm = function () {
        self.orderId(null);
        self.transportCode("");
        self.packageId(null);
        self.note("");
        self.callback = null;
    }

    self.show = function (transportCode, callback, packageId) {
        self.resetForm();
        resetForm("#addPackageForm");
        $("#addPackageModal").modal("show");

        if (transportCode)
            self.transportCode(transportCode);

        if (packageId)
            self.packageId(packageId);

        if (callback)
            self.callback = callback;
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
            toastr.error("Entering order is compulsory");
            return;
        }

        if ($.trim(self.transportCode()) === "") {
            toastr.error("Entering waybill code is compulsory");
            return;
        }

        var data = {
            id: self.orderId(),
            codePackage: self.transportCode(),
            note: self.note(),
            packageId: self.packageId(),
            mode: 1
        };

        self.isLoading(true);
        $.post("/order/addcontractcode",
            data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.msg);
                    return;
                }

                toastr.success(rs.msg);
                self.transportCode("");
                self.note("");
                if (self.callback)
                    self.callback();
            });
    }
}