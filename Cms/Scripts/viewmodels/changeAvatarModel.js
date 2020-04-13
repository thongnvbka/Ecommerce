function ChangeAvatarModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.avatar = ko.observable("");

    self.token = $("#changeAvatarForm input[name='__RequestVerificationToken']").val();

    self.save = function () {
        if (self.avatar() === "") {
            toastr.warning("You have not uploaded a avatar yet");
            return;
        }

        var data = { avatar: self.avatar() };
        data["__RequestVerificationToken"] = self.token;

        self.isLoading(true);
        $.post("/User/ChangeAvatar",
            data,
            function(rs) {
                self.isLoading(false);
                if (rs && rs.status <= 0) {
                    toastr.warning(rs.text);
                    return;
                }

                toastr.success(rs.text);
                location.reload();
            });
    }

    $(function() {
        $("#FlieuploadAvatar").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (window.maxFileLength && file.size > window.maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Size is too large";
                } else if (window.validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Not in correct format";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("The file is not allowed");
                    return;
                }

                self.avatar(data.result[0].url);
                $("#imgAvatar").attr("src", "/images/" + data.result[0].url + "_50x50_1");
                $("#loadingUpload").hide();
            },
            send: function () {
                $("#loadingUpload").show();
            }, fail: function () {
                $("#loadingUpload").hide();
            }
        });
    });
}

var modelView = new ChangeAvatarModel();
ko.applyBindings(modelView, $("#changeAvatar")[0]);