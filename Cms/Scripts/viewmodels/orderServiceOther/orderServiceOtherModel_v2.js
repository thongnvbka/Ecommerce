function OrderServiceOtherModel() {
    var self = this;

    self.isReadOnly = ko.observable(false);
    self.items = ko.observableArray([]);

    self.mode = ko.observable(0);
    self.orderCode = ko.observable("");
    self.value = ko.observable("");
    self.note = ko.observable("");
    self.importWarehouseId = ko.observable(null);
    self.importWarehouseCode = ko.observable("");
    self.packages = ko.observableArray([]);
    self.isLoading = ko.observable(false);

    self.save = function () {
        var s = {
            mode: self.mode(),
            orderCode: _.trim(self.orderCode()),
            value: _.trim(self.value()),
            note: self.note(),
            importWarehouseId: self.importWarehouseId(),
            importWarehouseCode: self.importWarehouseCode()
        };

        if (s.orderCode.length === 0 || s.value.length === 0) {
            toastr.warning("The order number and the value of money in the resulting charge may not be left blank");
            return;
        }

        if (_.isNil(_.find(self.packages(), function (p) { return p.orderCode === s.orderCode; }))) {
            toastr.warning('Order code "' + s.orderCode + '" Not included in this entry');
            return;
        }

        s['__RequestVerificationToken'] = self.token;

        self.isLoading(true);
        $.post("/ImportWarehouse/AddOtherService",
            s,
            function(rs) {
                self.isLoading(false);
                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                self.resetForm();
                self.getDetail();
            });
    }

    self.resetForm = function() {
        self.mode(0);
        self.orderCode("");
        self.value("");
        self.note("");
    }

    self.getDetail = function () {
        $.get("/ImportWarehouse/GetOtherService",
            { importWarehouseId: self.importWarehouseId() },
            function(data) {
                self.items(data);
            });
    }

    self.token = $("#orderServiceOtherModal input[name='__RequestVerificationToken']").val();

    self.add = function () {
        var data = {
            orderCode: ko.observable(""),
            mode: ko.observable(0),
            value: ko.observable(null),
            note: ko.observable("")
        };
        self.items.push(data);
        self.initInputMark();
    }

    self.remove = function (data) {

        swal({
                title: 'Are you sure you want to delete this item?',
                text: 'This cost incurred',
                type: 'warning',
                showCancelButton: true,
               cancelButtonText: 'Cancel',
                confirmButtonText: 'Delete'
            })
            .then(function() {
                    self.isLoading(true);

                    var item = {
                        importWarehouseId: self.importWarehouseId(),
                        importWarehouseCode: self.importWarehouseCode(),
                        id: data.id
                    };

                    item["__RequestVerificationToken"] = self.token;
                    $.post("/importwarehouse/RemoveOtherService",
                        item,
                        function(rs) {
                            self.isLoading(false);
                            if (rs.status < 0) {
                                toastr.error(rs.text);
                                return;
                            }

                            toastr.success(rs.text);
                            self.getDetail();
                        });
                },
                function() {});
    }

    self.getData = function () {
        return ko.mapping.toJS(self.items());
    }

    self.resetValue = function () {
        self.items.removeAll();
        self.isReadOnly(false);
    }

    self.show = function (importWarehouseId, importWarehouseCode, packages) {
        self.importWarehouseId(importWarehouseId);
        self.importWarehouseCode(importWarehouseCode);
        self.packages(packages);

        $("#orderServiceOtherModal").modal("show");
    }

    self.initInputMark = function () {
        $('#orderServiceOtherModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", {
                    radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true,
                    groupSeparator: Globalize.culture().numberFormat[','], digits: 2,
                    digitsOptional: true, allowPlus: false, allowMinus: false
                });
            }
        });
    }
}