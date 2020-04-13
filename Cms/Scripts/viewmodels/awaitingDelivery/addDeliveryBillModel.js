function AddDeliveryBillModel() {
    var self = this;
    self.isLoading = ko.observable(false);
    self.isSubmiting = ko.observable(false);

    self.id = ko.observable(0);
    self.customer = ko.observable(null);
    self.packages = ko.observableArray([]);

    self.totalWeight = ko.observable(null);
    self.priceWeight = ko.observable(null);
    self.pricePacking = ko.observable(null);
    self.priceOther = ko.observable(null);
    self.priceStored = ko.observable(null);
    self.priceFast = ko.observable(null);
    self.totalOrder = ko.observable(null);
    self.total = ko.observable(null);
    self.depositMoney = ko.observable(null);
    self.blanceAfter = ko.observable(null);
    self.blanceAvalibale = ko.observable(null);
    self.debit = ko.observable(null);
    self.debitPre = ko.observable(null);
    self.receivable = ko.observable(null);
    self.receivableCache = ko.observable(null);
    self.customerId = ko.observable(null);

    self.priceShip = ko.observable(null);
    self.note = ko.observable("");
    self.addSuccessCallback = null;

    self.resetForm = function() {
        self.id(0);
        self.customer(null);
        self.packages([]);
        self.totalWeight(null);
        self.priceWeight(null);
        self.pricePacking(null);
        self.priceOther(null);
        self.priceStored(null);
        self.priceFast(null);
        self.totalOrder(null);
        self.total(null);
        self.depositMoney(null);
        self.blanceAfter(null);
        self.blanceAvalibale(null);
        self.debit(null);
        self.debitPre(null);
        self.receivable(null);
        self.receivableCache(null);
        self.customerId(null);
        self.priceShip(null);
        self.note("");
    }

    self.showAndLoadData = function (packageIds, customerId, blanceAvalibale) {
        $("#addDeliveryBillModal").modal("show");
        self.resetForm();
        self.blanceAvalibale(blanceAvalibale);
        self.customerId(customerId);
        self.loadData(packageIds, customerId);
        self.initInputMark();
    }

    self.loadData = function (packageIds, customerId) {
        self.isLoading(true);
        $.post("/AwaitingDelivery/GetDataToAddBill",
            { packageIds: packageIds, customerId: customerId },
            function (data) {
                self.isLoading(false);

                //var c = data.customer;
                //c.address = "";
                //if (c.wardsName !== null && c.wardsName !== "") {
                //    c.address = c.address.length > 0 ? c.address + ", " + c.wardsName : c.wardsName;
                //}
                //if (c.districtName !== null && c.districtName !== "") {
                //    c.address = c.address.length > 0 ? c.address + ", " + c.districtName : c.districtName;
                //}
                //if (c.provinceName !== null && c.districtName !== "") {
                //    c.address = c.address.length > 0 ? c.address + ", " + c.provinceName : c.provinceName;
                //}

                self.customer(ko.mapping.fromJS(data.customer));

                var firstOrderId = null;
                _.each(data.packages,
                    function (p) {
                        p.isFirst = firstOrderId !== p.orderId;
                        firstOrderId = p.orderId;
                        p.totalPackageNo = p.isFirst ? _.filter(data.packages,
                            function (p1) { return p1.orderId === p.orderId; }).length : 0;
                        p.order = data.order[p.orderId];
                        p.serviceOrder = data.serviceOrder[p.orderId];
                        p.priceOrder = p.order + p.serviceOrder;
                        p.priceDepositMoney = data.depositMoney[p.orderId + ''];

                        p.priceWeight = data.priceWeigth[p.id + ''];
                        p.priceStored = data.priceStored[p.id + ''];
                        p.vipShip = data.vipShip[p.customerLevelId + ''];
                    });

                self.packages(data.packages);

                self.totalWeight(_.sumBy(data.packages, "weightActual"));
                self.priceWeight(data.thisPriceWeigth);
                self.pricePacking(data.thisPricePacking);
                self.priceOther(data.thisPriceOther);
                self.priceStored(data.thisPriceStored);
                self.totalOrder(data.thisOrder + data.thisServiceOrder);
                self.depositMoney(data.thisDepositMoney);
                self.total(data.thisTotal);
                self.debit(data.debit);
                self.debitPre(data.debitPre);
                self.receivable(data.receivable);
                self.receivableCache(data.receivable);
                //self.blanceAfter(self.blanceAvalibale() - data.debit);
            });
    }

    self.priceShip.subscribe(function(value) {
        var priceShip = Globalize.parseInt(value);
        if (isNaN(priceShip)) {
            self.receivable(self.receivableCache());
        } else {
            self.receivable(self.receivableCache() + priceShip);
        }

        self.blanceAfter(self.blanceAvalibale() - self.total());
    });

    self.remove = function (data) {
        var packageIds = ";" + _.join(_.map(_.filter(self.packages(), function (p) { return p.id !== data.id; }), "id"), ";") + ";";

        self.loadData(packageIds, self.customer().id());
    }

    self.token = $("#addDeliveryBillForm input[name='__RequestVerificationToken']").val();

    self.save = function() {
        if (_.trim(self.priceShip()).length === 0) {
            toastr.warning("You must compensate the shipping fee");
            return;
        }

        if (self.packages().length === 0) {
            toastr.warning("Dispatch note must have at least one goods package");
            return;
        }

        var packageIds = ";" + _.join(_.map(self.packages(), "id"), ";") + ";";

        var data = {
            packageIds: packageIds,
            note: self.note(),
            priceShip: self.priceShip(),
            customerId: self.customerId()
        };
        data["__RequestVerificationToken"] = self.token;

        self.isSubmiting(true);
        $.post("/AwaitingDelivery/AddBill",
            data,
            function(rs) {
                self.isSubmiting(false);

                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }
                $("#addDeliveryBillModal").modal("hide");
                swal(
                    'Created note successfully!',
                    'ID: D' + rs.deliveryCode,
                    'success'
                );

                if (self.addSuccessCallback)
                    self.addSuccessCallback();
            });

    }

    self.initInputMark = function () {
        $('#addDeliveryBillModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }
}