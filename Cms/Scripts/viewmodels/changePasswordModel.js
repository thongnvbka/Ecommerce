function ChangePasswordModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.oldPassword = ko.observable("");
    self.newPassword = ko.observable("");
    self.reNewPassword = ko.observable("");

    self.resetForm = function() {
        self.isLoading(false);
        self.oldPassword("");
        self.newPassword("");
        self.reNewPassword("");
    }

    self.getValue = function() {
        return {
            oldPassword: self.oldPassword(),
            newPassword: self.newPassword(),
            reNewPassword: self.reNewPassword()
        }
    }

    self.resetForm();

    self.token = $("#changePasswordForm input[name='__RequestVerificationToken']").val();

    self.save = function() {
        if (!$("#changePasswordForm").valid()) {
            toastr.error("Check the entered fields!");
            $(".input-validation-error:first").focus();
            return;
        }

        var data = self.getValue();
        data["__RequestVerificationToken"] = self.token;

        self.isLoading(true);
        $.post("/User/ChangePassword",
            data,
            function(rs) {
                self.isLoading(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
                self.resetForm();
            });
    }
}

var modelView = new ChangePasswordModel();
ko.applyBindings(modelView, $("#changePassword")[0]);