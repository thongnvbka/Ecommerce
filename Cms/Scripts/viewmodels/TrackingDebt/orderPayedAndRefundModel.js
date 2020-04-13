function OrderPayedAndRefundModel(callback) {
    var self = this;

    self.isLoading = ko.observable(false);
    self.orderId = ko.observable(null);
    self.orderCode = ko.observable("");
    self.amount = ko.observable(null);
    self.note = ko.observable("");
    self.mode = ko.observable(0);

    self.resetForm = function() {
        self.isLoading(false);
        self.orderId(null);
        self.orderCode("");
        self.amount(null);
        self.note("");
    }

    self.callback = callback;

    self.getData = function () {
        return {
            orderId: self.orderId(),
            orderCode: self.orderCode(),
            amount: self.amount(),
            note: self.note()
        };
    }

    /**
     * Hiển thị form Refund/thu tiền
     * @param {} mode 0: Hoàn tiền, 1: Thu tiền
     * @param {} orderId Id Orders 
     * @param {} orderCode Orders code
     * @param {} amount số tiền
     * @returns {} 
     */
    self.show = function(mode, orderId, orderCode, amount) {
        self.resetForm();

        self.mode(mode);
        self.orderId(orderId);
        self.orderCode(orderCode);
        self.amount(formatNumberic(amount));

        self.initInputMark();
        $("#orderPayedAndRefundModal").modal("show");
    }

    self.save = function () {
        if ($.trim(self.amount()) === "") {
            toastr.error("Amount is required");
            return;
        }

        if ($.trim(self.note()) === "") {
            toastr.error("Notes are required");
            return;
        }

        var data = self.getData();
        data["__RequestVerificationToken"] = self.token;

        self.isLoading(true);

        // Hoàn tiền
        if (self.mode() === 0) {
            $.post("/TrackingDebt/OrderRefund",
                data,
                function(rs) {
                    self.isLoading(false);

                    if (rs.status < 0) {
                        toastr.error(rs.text);
                        return;
                    } 

                    toastr.success(rs.text);

                    if (self.callback)
                        self.callback();

                    $("#orderPayedAndRefundModal").modal("hide");
                });
        } else { // Thu tiền
            $.post("/TrackingDebt/OrderPayment",
                data,
                function (rs) {
                    self.isLoading(false);

                    if (rs.status < 0) {
                        toastr.error(rs.text);
                        return;
                    } 

                    toastr.success(rs.text);

                    if (self.callback)
                        self.callback();

                    $("#orderPayedAndRefundModal").modal("hide");

                });
        }
    }

    self.token = $("#orderPayedAndRefundModal input[name='__RequestVerificationToken']").val();

    self.initInputMark = function () {
        $('#orderPayedAndRefundModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal",
                {
                    radixPoint: Globalize.culture().numberFormat['.'],
                    autoGroup: true,
                    groupSeparator: Globalize.culture().numberFormat[','],
                    digits: 2,
                    digitsOptional: true,
                    allowPlus: false,
                    allowMinus: false
                });
            }
        });
    };
}