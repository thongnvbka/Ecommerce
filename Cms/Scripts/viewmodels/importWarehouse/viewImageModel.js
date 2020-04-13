function ViewImageModel() {
    var self = this;

    self.isLoading = ko.observable(false);
    self.images = ko.observableArray([]);
    self.image = ko.observable(null);
    self.currentNo = ko.observable(null);
    self.isUrlAbsolute = ko.observable(false);

    self.show = function (images, image, isUrlAbsolute) {
        self.images(images);
        self.image(image);
        var idx = _.indexOf(self.images(), self.image());
        self.currentNo(idx + 1);

        if (isUrlAbsolute)
            self.isUrlAbsolute(true);

        $("#viewImageModal").modal("show");
    }

    self.next = function() {
        var idx = _.indexOf(self.images(), self.image());

        if (idx === (self.images().length - 1)) {
            self.image(self.images()[0]);
            self.currentNo(1);
        } else {
            self.image(self.images()[idx + 1]);
            self.currentNo(idx + 2);
        }
    }

    self.previous = function() {
        var idx = _.indexOf(self.images(), self.image());

        if (idx === 0) {
            self.image(self.images()[self.images().length - 1]);
            self.currentNo(self.images().length);
        } else {
            self.image(self.images()[idx - 1]);
            self.currentNo(idx);
        }
    }
}