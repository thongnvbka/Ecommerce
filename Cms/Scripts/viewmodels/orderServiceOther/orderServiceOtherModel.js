function OrderServiceOtherModel() {
    var self = this;

    self.isReadOnly = ko.observable(false);
    self.items = ko.observableArray([]);

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
        self.items.remove(data);
    }

    self.getData = function () {
        return ko.mapping.toJS(self.items());
    }

    self.resetValue = function () {
        self.items.removeAll();
        self.isReadOnly(false);
    }

    self.show = function() {
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