function ChangeFullnameModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.fullname = ko.observable("");

    self.token = $("#changeFullnameForm input[name='__RequestVerificationToken']").val();

    self.saveFullName = function () {
        if (self.fullname() === "") {
            toastr.warning("You have not entered a username yet");
            return;
        }

        var data = { fullname: self.fullname() };
        data["__RequestVerificationToken"] = self.token;

        self.isLoading(true);
        $.post("/User/ChangeFullname",
            data,
            function (rs) {
                self.isLoading(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
                location.reload();
            });
    }

    $(function () {

    });
}

var modelView = new ChangeFullnameModel();
ko.applyBindings(modelView, $("#changeFullname")[0]);