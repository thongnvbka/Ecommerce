function TransferPackageModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.orderId = ko.observable(null);
    self.transportCode = ko.observable("");
    self.customerEmail = ko.observable("");
    self.packageId = ko.observable(null);
    self.note = ko.observable("");
    self.code = ko.observable("");
    self.callback = null;

    self.resetForm = function () {
        self.isLoading(false);
        self.orderId(null);
        self.transportCode("");
        self.customerEmail("");
        self.packageId(null);
        self.code("");
        self.note("");
        self.callback = null;
    }

    self.show = function (transportCode, packageId, packagecode, customerEmail, callback) {
        self.resetForm();
        resetForm("#transferPackageForm");
        $("#transferPackageModal").modal("show");

        self.transportCode(transportCode);
        self.customerEmail(customerEmail);
        self.packageId(packageId);
        self.code(packagecode);

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

    self.token = $("#transferPackageModal input[name = '__RequestVerificationToken']").val();

    self.save = function () {
        if (self.orderId() == null) {
            toastr.error("Orders are compulsory to enter");
            return;
        }

        if (self.packageId() == null) {
            toastr.error("Package is required");
            return;
        }

        if ($.trim(self.note()) === "") {
            toastr.error("Note is required");
            return;
        }

        var data = {
            orderId: self.orderId(),
            note: self.note(),
            packageId: self.packageId()
        };
        data['__RequestVerificationToken'] = self.token;

        self.isLoading(true);
        $.post("/Package/Transfer",
            data, function (rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);

                if (self.callback)
                    self.callback();
            });
    }
}