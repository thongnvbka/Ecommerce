function UpdateWeightModel() {
    var self = this;

    self.isLoading = ko.observable(false);

    self.packageCode = ko.observable("");
    self.note = ko.observable("");
    self.packageId = ko.observable(0);
    self.weight = ko.observable(null);
    self.width = ko.observable(null);
    self.height = ko.observable(null);
    self.length = ko.observable(null);
    self.widthConverted = ko.computed(function () {
        var width = Globalize.parseFloat(self.width());
        var length = Globalize.parseFloat(self.length());
        var height = Globalize.parseFloat(self.height());

        return width && length && height ? width * length * height / 6000 : 0;
    }, this);

    self.callback = null;

    self.resetForm = function() {
        self.isLoading(false);

        self.width(null);
        self.length(null);
        self.height(null);
        self.weight(null);
        self.packageCode("");
        self.note("");
        self.packageId(0);

        self.initInputMark();
        self.removeRules();
    }

    self.removeRules = function () {
        $("#updatePackageWeightForm #Width").rules("remove", "number");
        $("#updatePackageWeightForm #Length").rules("remove", "number");
        $("#updatePackageWeightForm #Height").rules("remove", "number");
        $("#updatePackageWeightForm #Weight").rules("remove", "number");
    }

    self.setForm = function (data) {
        self.packageId(data.id);
        self.packageCode(data.code);
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
            packageId: self.packageId(),
            packageCode: self.packageCode(),
            note: self.note()
        };
    }

    self.initInputMark = function () {
        $('#updatePackageWeightForm input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.showUpdateForm = function(data) {
        self.resetForm();
        resetForm("#updatePackageWeightForm");

        self.setForm(data);

        $("#updatePackageWeightModal").modal("show");
    }

    self.token = $("#updatePackageWeightForm input[name = '__RequestVerificationToken']").val();

    self.save = function() {
        var data = self.getForm();
        data['__RequestVerificationToken'] = self.token;

        if (!$("#updatePackageWeightForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        if ($.trim(data.note) === "") {
            toastr.error("Note is required");
            return;
        }
         

        self.isLoading(true);
        $.post("/Package/UpdateWeight",
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
                resetForm("#updatePackageWeightForm");

                $("#updatePackageWeightModal").modal("hide");

                if (self.callback)
                    self.callback();
            });
    }
}