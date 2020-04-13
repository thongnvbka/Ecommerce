function AddPackageLoseModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.isUploading = ko.observable(false);
    self.id = ko.observable(0);
    self.packageCode = ko.observable("");
    self.tranportCode = ko.observable("");
    self.note = ko.observable("");
    self.packageId = ko.observable(null);
    self.images = ko.observableArray([]);
    self.callback = null;

    self.resetForm = function () {
        self.id(0);
        self.tranportCode("");
        self.packageCode("");
        self.note("");
        self.images([]);
        self.packageId(null);
        self.callback = null;
    }

    self.setUpdate = function (id, packageCode, transportCode, note, images) {
        self.resetForm();
        resetForm("#addPackageLoseForm");

        self.id(id);
        self.tranportCode(transportCode);
        self.packageCode(packageCode);
        self.note(note);
        self.images(images);

        $("#addPackageLoseModal").modal("show");
    }

    self.show = function (tranportCode, callback, packageId) {
        self.resetForm();
        resetForm("#addPackageLoseForm");
        $("#addPackageLoseModal").modal("show");

        if (tranportCode)
            self.tranportCode(tranportCode);

        if (packageId)
            self.packageId(packageId);

        if (callback)
            self.callback = callback;
    }

    self.remove = function (data) {
        self.images.remove(data);
    }

    self.save = function () {
        if ($.trim(self.note()) === "") {
            toastr.error("Entering description is compulsory");
            return;
        }

        var data = {
            id: self.id(),
            imageJson: JSON.stringify(self.images()),
            note: self.note(),
            packageId: self.packageId(),
            tranportCode: self.tranportCode()
        };

        self.isLoading(true);
        if (self.id() <= 0) {
            $.post("/order/addpackagenocode",
                data,
                function (rs) {
                    self.isLoading(false);
                    if (rs.status < 0) {
                        toastr.error(rs.msg);
                        return;
                    }

                    toastr.success(rs.msg);
                    self.note("");
                    self.tranportCode("");
                    self.images([]);
                    if (self.callback)
                        self.callback();
                });
        } else {
            $.post("/order/updatepackagenocode",
                data,
                function (rs) {
                    self.isLoading(false);
                    if (rs.status < 0) {
                        toastr.error(rs.msg);
                        return;
                    }

                    toastr.success(rs.msg);
                });
        }
    }

    self.imageViewModal = ko.observable();
    self.showImage = function (image) {
        if (self.imageViewModal() == null) {
            self.imageViewModal(new ViewImageModel());
            ko.applyBindings(self.imageViewModal(), $("#viewImageModal")[0]);
        }

        self.imageViewModal().show(self.images(), image);
    }

    $(function () {
        // Init jquery upload ajax
        $("#addImageForPackageLose").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (window.maxFileLength && file.size > window.maxFileLength) {
                    msg += file.name + ": Size is too large";
                } else if (window.validateBlackListExtensions(file.name)) {
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
                self.images.push(data.result[0].url);
                self.isUploading(false);
            },
            send: function () {
                self.isUploading(true);
            }, fail: function () {
                self.isUploading(false);
            }
        });
    });
}