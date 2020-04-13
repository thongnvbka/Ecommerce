function RequestAccountingLoseModel(imageViewModel) {
    var self = this;
    self.isLoading = ko.observable(false);
    self.orderCode = ko.observable("");
    self.orderId = ko.observable(null);
    self.items = ko.observableArray([]);
    // Search

    self.show = function(orderId, orderCode) {
        self.resetValue();
        self.orderId(orderId);
        self.orderCode(orderCode);

        $("#requestAcoutingLoseModal").modal("show");
        self.isLoading(true);

        self.getDetail();
    }

    self.getDetail = function() {
        $.get("/AcountingLose/GetByOrderId",
            { orderId: self.orderId() },
            function (data) {
                self.isLoading(false);

                _.each(data,
                    function (it) {
                        if (it.imageJson) {
                            it.images = ko.observableArray(JSON.parse(it.imageJson));
                        }
                    });

                self.items(data);
            });
    }

    self.resetValue = function() {
        self.isLoading(false);
        self.orderCode("");
        self.orderId(null);
    }

    self.hide = function() {
        self.resetValue();
        $("#requestAcoutingLoseModal").modal("hide");
    }

    self.quantityLoseRecived = ko.observable(null);
    self.dataCache = ko.observable(null);
    self.isLoadingRecived = ko.observable(false);

    self.showUpdate = function (data) {
        self.dataCache(data);
        self.quantityLoseRecived("");
        self.initInputMark();
        $("#requestAcoutingLoseUpdateModal").modal("show");
    }

    self.updateQuantityLose = function() {
        if ($.trim(self.quantityLoseRecived()).length === 0) {
            toastr.warning("The number of products received by customer cannot be empty!");
            return;
        }

        var number = Globalize.parseInt(self.quantityLoseRecived());
        if (number <= 0) {
            toastr.warning("The number of products received by customer cannot be less than 0!");
            return;  
        }

        if (number > self.dataCache().quantityLose) {
            toastr.warning("Number of products received by customer is not greater than the total number of wrong products!");
            return;
        }

        self.isLoadingRecived(true);

        var data = {
            orderDetailId: self.dataCache().id,
            quantityRecived: number
        };

        data["__RequestVerificationToken"] = self.token;

        $.post("/AcountingLose/UpdateAcountingLose", data,
            function(rs) {
                self.isLoadingRecived(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
                $("#requestAcoutingLoseUpdateModal").modal("hide");
                self.getDetail();
            });
    }

    self.token = $("#requestAcoutingLoseUpdateModal input[name='__RequestVerificationToken']").val();

    self.initInputMark = function () {
        $('#requestAcoutingLoseUpdateModal input.integer').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.viewImage = function(data) {
        imageViewModel.show(data.images(), data.images()[0]);
    }

    self.viewImage2 = function (data) {
        imageViewModel.show([data.image], data.image, true);
    }
}