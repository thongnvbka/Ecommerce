function UpdateWeightModel() {
    var self = this;

    self.isLoading = ko.observable(false);

    self.walletCode = ko.observable("");
    self.note = ko.observable("");
    self.walletId = ko.observable(0);
    self.weight = ko.observable(null);
    self.width = ko.observable(null);
    self.height = ko.observable(null);
    self.length = ko.observable(null);
    self.widthConverted = ko.computed(function () {
        var width = Globalize.parseFloat(self.width());
        var length = Globalize.parseFloat(self.length());
        var height = Globalize.parseFloat(self.height());

        return width && length && height ? width * length * height / 5000 : 0;
    }, this);

    self.callback = null;

    self.resetForm = function() {
        self.isLoading(false);

        self.width(null);
        self.length(null);
        self.height(null);
        self.weight(null);
        self.walletCode("");
        self.note("");
        self.walletId(0);

        self.initInputMark();
        self.removeRules();
    }

    self.removeRules = function () {
        $("#updateWalletWeightForm #Width").rules("remove", "number");
        $("#updateWalletWeightForm #Length").rules("remove", "number");
        $("#updateWalletWeightForm #Height").rules("remove", "number");
        $("#updateWalletWeightForm #Weight").rules("remove", "number");
    }

    self.setForm = function (data) {
        self.walletId(data.id);
        self.walletCode(data.code);
        self.weight(formatNumberic(data.weight, 'N2'));
        self.width(formatNumberic(data.width, 'N2'));
        self.length(formatNumberic(data.length, 'N2'));
        self.height(formatNumberic(data.height, 'N2'));
    }

    self.getForm = function() {
        return {
            width: self.width(),
            length: self.length(),
            height: self.height(),
            weight: self.weight(),
            walletId: self.walletId(),
            walletCode: self.walletCode(),
            note: self.note()
        };
    }

    self.initInputMark = function () {
        $('#updateWalletWeightForm input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.showUpdateForm = function(data) {
        self.resetForm();
        resetForm("#updateWalletWeightForm");

        self.setForm(data);

        $("#updateWalletWeightModal").modal("show");
    }

    self.token = $("#updateWalletWeightModal input[name = '__RequestVerificationToken']").val();

    self.save = function() {
        var data = self.getForm();
        data['__RequestVerificationToken'] = self.token;

        if (!$("#updateWalletWeightForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        if ($.trim(data.note) === "") {
            toastr.error("The note is required");
            return;
        }
         

        self.isLoading(true);
        $.post("/wallet/UpdateWeight",
            data,
            function(rs) {
                self.isLoading(false);

                if (rs.status < 0) {
                    toastr.error(rs.text);
                    return;
                }

                toastr.success(rs.text);
                // reset form
                self.resetForm();
                resetForm("#updateWalletWeightForm");

                $("#updateWalletWeightModal").modal("hide");

                if (self.callback)
                    self.callback();
            });
    }
}