function RouteTransportModel(walletDetailModal) {
    var self = this;

    self.items = ko.observableArray([]);
    self.isLoading = ko.observable(false);

    self.show = function (walletId) {
        self.items([]);
        $("#routeWalletModal").modal("show");
        self.getRoute(walletId);
    }

    self.showWalletDetail = function (walletId) {
        if (walletDetailModal) {
            walletDetailModal.showModel(walletId);
            return;
        }
    }

    self.getRoute = function (walletId) {
        self.isLoading(true);
        $.get("/WalletTracker/Route",
            { walletId: walletId },
            function(data) {
                self.isLoading(false);

                var firstDay = null;
                _.each(data,
                    function(it) {
                        it.firstDay = firstDay === null || !moment(it.dispatcherCreated).isSame(firstDay, 'day');
                        firstDay = it.dispatcherCreated;
                    });

                self.items(data);
            });
    }

    self.addForm = ko.observable(null);
    self.update = function (data) {
        if (self.addForm() == null) {
            self.addForm(new DispatcherAddModel(self.callback, walletDetailModal));
            ko.applyBindings(self.addForm(), $("#DispatcherAddModel")[0]);
        }

        $.get("/dispatcher/getdetail",
            { id: data.dispatcherId },
            function (result) {
                self.addForm().setForm(result);
            });
    }
}