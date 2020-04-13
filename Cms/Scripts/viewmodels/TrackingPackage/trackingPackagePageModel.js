// Bind PackageDetail
var packageDetailModelView = new PackageDetail(window.states);
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// Cập nhật cân nặng cho package
var updateWeightModelView = new UpdateWeightModel();
ko.applyBindings(updateWeightModelView, $("#updatePackageWeightModal")[0]);

var walletDetailModalView11 = new WalletDetailModel(window.allWarehouse, window.walletStates,
    window.orderPackageStates, packageDetailModelView, orderDetailViewModel);

ko.applyBindings(walletDetailModalView11, $("#walletDetailModal")[0]);

var modelView = new TrackingPackageModel(orderDetailViewModel, packageDetailModelView, walletDetailModalView11);

modelView.search(1, true);

updateWeightModelView.callback = function() {
    modelView.search(modelView.currentPage(), false);
}

ko.applyBindings(modelView, $("#trackingPackage")[0]);