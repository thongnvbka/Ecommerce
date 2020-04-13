function OrderTypeViewModel() {
    var self = this;

    self.showViewOrder = function (id, code) {
        code = code + '';
        var type = 0;
        $.ajax({
            type: 'POST',
            url: "/Purchase/GetType",
            data: { code: code },
            success: function (result) {
                if (result.status === msgType.error) {
                    type = 0;
                } else {
                    type = result.type;
                }
            },
            async: false
        });

        if (type == 1) {
            orderDetailViewModel.viewOrderDetail(id);
        }

        if (type == 0) {
            depositDetailViewModel.showModalDialog(id);
        }

        if (type == 4) {
            orderCommerceDetailViewModel.showModal(id);
        }
    }

    self.showEditOrder = function (id, code) {
        code = code + '';
        var type = 0;
        $.ajax({
            type: 'POST',
            url: "/Purchase/GetType",
            data: { code: code },
            success: function (result) {
                if (result.status === msgType.error) {
                    type = 0;
                } else {
                    type = result.type;
                }
            },
            async: false
        });

        if (type == 1) {
            viewModel.viewEditDetail(id);
        }

        if (type == 0) {
            depositAddOrEditViewModel.showModalDialog(id);
        }

        if (type == 4) {
            orderCommerce.viewOrderAdd(id);
        }
    }

    self.showEditOrderCode = function (code) {
        code = code + '';
        var type = 0;
        var id = 0; 
        $.ajax({
            type: 'POST',
            url: "/Purchase/GetType",
            data: { code: code },
            success: function (result) {
                if (result.status === msgType.error) {
                    type = 0;
                } else {
                    type = result.type;
                    id = result.id;
                }
            },
            async: false
        });

        if (type == 1) {
            if (viewModel.orderWaitViewModel) {
                viewModel.orderWaitViewModel.viewEditDetail(id);
            } else {
                viewModel.viewEditDetail(id);
            }
        }

        if (type == 0) {
            depositAddOrEditViewModel.showModalDialog(id);
        }

        if (type == 4) {
            orderCommerce.viewOrderAdd(id);
        }
    }

    self.showViewOrderCode = function (code) {
        code = code + '';
        var type = 0;
        var id = 0;
        $.ajax({
            type: 'POST',
            url: "/Purchase/GetType",
            data: { code: code },
            success: function (result) {
                if (result.status === msgType.error) {
                    type = 0;
                } else {
                    type = result.type;
                    id = result.id;
                }
            },
            async: false
        });

        if (type == 1) {
            orderDetailViewModel.viewOrderDetail(id);
        }

        if (type == 0) {
            depositDetailViewModel.showModalDialog(id);
        }

        if (type == 4) {
            orderCommerceDetailViewModel.showModal(id);
        }
    }
}

var orderTypeViewModel = new OrderTypeViewModel();
ko.applyBindings(orderTypeViewModel, $("#view-order-type")[0]);