var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var DetailOrderViewModel = function (orderPopupModel, chatViewModel) {
    var self = this;
    self.isRending = ko.observable(false);
    self.totalPrice = ko.observable();
    self.orderDetail = ko.observableArray([]);
    self.orderAddress = ko.observableArray([]);
    self.orderProducts = ko.observableArray([]);
    self.orderServices = ko.observableArray([]);
    self.orderServicesView = ko.observableArray([]);
    self.orderExchange = ko.observableArray([]);
    self.orderPackage = ko.observableArray([]);
    self.recharge = ko.observableArray([]);
    self.complains = ko.observableArray([]);
    //complain id
    self.isComplain = ko.observable();

    self.listOrderService = ko.observableArray([]);
    self.listProductDetail = ko.observableArray([]);
    self.listOrderPackage = ko.observableArray([]);
    self.listOrderComment = ko.observableArray([]);
    self.listOrderDetail = ko.observableArray([]);

    self.orderDetail = ko.observable(new orderDetailModel());

    $(function () {
        var arr = _.split(window.location.href, "orderId=");
        if (arr.length > 1) {
            self.detailOrder(arr[1]);
        }
       
    });
    //Cộng lại tiền
    self.renderedHandler = function (elements, data) {
        console.log(data.ServiceId);
        if (data.ServiceId < 6) {
            if (data.Checked === 'True' && data.TotalPrice > 0) {
                self.totalPrice(self.totalPrice() + data.TotalPrice);
            }
        }

    }
    //Todo chi tiết đơn hàng order
    self.detailOrder = function (id) {
        self.orderDetail(new orderDetailModel());
        self.orderAddress([]);
        self.orderProducts([]);
        self.orderServices([]);
        self.orderExchange([]);
        self.orderPackage([]);
        self.recharge([]);
        self.complains([]);
        self.listOrderDetail([]);

        self.orderServicesView([]);

        $.post("/" + window.culture + "/CMS/Order/GetOrderDetails",
            { orderId: id },
            function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                } else {
                    self.mapOrderDetail(result.orderDetail);

                    if (result.orderAddress !== null) {
                        self.orderAddress(result.orderAddress);
                    }
                    if (result.orderServices !== null) {
                        self.orderServices(result.orderServices);
                    }
                    if (result.listOrderDetail !== null) {
                        self.listOrderDetail(_.orderBy(result.listOrderDetail, ['Id'], ['asc']));
                    }
                    self.orderProducts(result.orderProducts);
                    self.orderServicesView(result.orderServicesView);
                    self.orderExchange(result.orderExchange);
                    self.orderPackage(result.orderPackage);
                    self.recharge(result.recharge);
                    self.complains(result.complains);
                    self.isComplain(result.isComplain);
                    chatViewModel.showChat(self.orderDetail().Id(), self.orderDetail().Code(), self.orderDetail().Type());
                    self.getServicesOther(id);
                    self.isRending(true);
                    
                }
            });
    }
    self.getServicesOther = function (id) {
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/GetServiceOther",
            type: 'Get',
            data: { 'id': id },
            success: function (result) {
                $('#service-other').attr('data-content', result);
            }
        });
        $('[data-toggle="popover"]').popover({
            placement: 'right'
        });
    }
    self.renderedPackageHandler = function (elements, data) {
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/GetPackageHistory",
            type: 'Get',
            data: { 'id': data.Id },
            success: function (result) {
                $('#package-history-' + data.Id).attr('data-content', result);
            },
            asyc: false
        });
        $.ajax({
            url: "/" + window.culture + "/CMS/Order/GetPackageWallet",
            type: 'Get',
            data: { 'id': data.Id },
            success: function (result) {
                $('#package-wallet-' + data.Id).attr('data-content', result);
            },
            asyc: false
        });
        $('[data-toggle="popover"]').popover({
            placement: 'right'
        });
    }

    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Order/BuyOrder";
    };

    //Cập nhật thông tin ghi chú
    self.updateNote = function () {
        var obj = {
            orderId: self.orderDetail().Id(),
            note: self.orderDetail().Note()
        }
        $.ajax({
            type: "POST",
            url: "/" + window.culture + "/CMS/Order/UpdateNote",
            data: obj,
            success: function (data) {
                if (data > 0) {
                    toastr.success(window.messager.detailOrder.updateNoteSuccess);
                }
                else {
                    toastr.error(window.messager.detailOrder.errorupdateNote );
                }
            },
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                HideLoading();
            }
        });
    };
    //Cập nhật thông tin ghi chú riêng
    self.updatePrivateNote = function (data) {
        var obj = {
            detailId: data.Id,
            note: data.PrivateNote
        }
        $.ajax({
            type: "POST",
            url: "/" + window.culture + "/CMS/Order/UpdatePrivateNote",
            data: obj,
            success: function (data) {
                if (data > 0) {
                    toastr.success(window.messager.detailOrder.updateNoteLinkSuccess);
                }
                else {
                    toastr.error(window.messager.detailOrder.errorupdateNoteLinkSuccess);
                }
            },
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                HideLoading();
            }
        });
    };
    //Cập nhật dịch vụ
    self.updateService = function (data) {
        var obj = {
            orderId: self.orderDetail().Id(),
            serviceId: data.ServiceId,
            isCheck: data.Checked
        }
        $.ajax({
            type: "POST",
            url: "/" + window.culture + "/CMS/Order/UpdateService",
            data: obj,
            success: function (data) {
                if (data > 0) {
                    var arr = _.split(window.location.href, "orderId=");
                    if (arr.length > 1) {
                        self.detailOrder(arr[1]);
                    }
                    toastr.success(window.messager.detailOrder.updateServiceSuccess);
                }
                else {
                    toastr.error(window.messager.detailOrder.errorupdateServiceSuccess);
                }
            },
            beforeSend: function () {
                ShowLoading();
            },
            complete: function () {
                HideLoading();
            }
        });
    }
    //================================== Hàm map dữ liệu =======================================
    //Map thông tin chi tiết đơn hàng order
    self.mapOrderDetail = function (data) {
        self.orderDetail(new orderDetailModel());

        self.orderDetail().Id(data.Id);
        self.orderDetail().Code(data.Code);
        self.orderDetail().Type(data.Type);
        self.orderDetail().WebsiteName(data.WebsiteName);
        self.orderDetail().ShopId(data.ShopId);
        self.orderDetail().ShopName(data.ShopName);
        self.orderDetail().ShopLink(data.ShopLink);
        self.orderDetail().ProductNo(data.ProductNo);
        self.orderDetail().PackageNo(data.PackageNo);
        self.orderDetail().TotalWeight(data.TotalWeight);
        self.orderDetail().ExchangeRate(data.ExchangeRate);
        self.orderDetail().Total(data.Total);
        self.orderDetail().TotalPayed(data.TotalPayed);
        self.orderDetail().WarehouseName(data.WarehouseName);
        self.orderDetail().CustomerId(data.CustomerId);
        self.orderDetail().CustomerName(data.CustomerName);
        self.orderDetail().CustomerEmail(data.CustomerEmail);
        self.orderDetail().CustomerPhone(data.CustomerPhone);
        self.orderDetail().CustomerAddress(data.CustomerAddress);
        self.orderDetail().Status(data.Status);
        self.orderDetail().StatusName(data.StatusName);
        self.orderDetail().StatusClass(data.StatusClass);
        self.orderDetail().OrderInfoId(data.OrderInfoId);
        self.orderDetail().FromAddressId(data.FromAddressId);
        self.orderDetail().ToAddressId(data.ToAddressId);
        self.orderDetail().ServiceType(data.ServiceType);
        self.orderDetail().Note(data.Note);
        self.orderDetail().LinkNo(data.LinkNo);
        self.orderDetail().Created(data.Created);
        self.orderDetail().LastUpdate(data.LastUpdate);
        self.orderDetail().ReasonCancel(data.ReasonCancel);
        self.orderDetail().PriceBargain(data.PriceBargain);
        self.orderDetail().ExpectedDate(data.ExpectedDate);
        self.orderDetail().UserNote(data.UserNote);
        self.orderDetail().WarehouseDeliveryId(data.WarehouseDeliveryId);
        self.orderDetail().WarehouseDeliveryName(data.WarehouseDeliveryName);

        //% lúc đặt cọc đơn hàng
        self.orderDetail().DepositPercent(data.DepositPercent);
        //Tổng tiền hàng của khách
        self.orderDetail().TotalExchange(data.TotalExchange);

        //Tổng tiền tạm tính đến thời điểm hiện tại
        self.orderDetail().Total(data.Total);

        //Tiền hàng của khách
        self.orderDetail().TotalPrice(data.TotalPrice);
        //Tiền khách phải trả NDT
        self.orderDetail().TotalPriceCustomer(data.TotalPriceCustomer);
        //Tiền khách nợ
        self.orderDetail().CashShortage(data.CashShortage);

        self.orderDetail().Name(statusApp.order[self.orderDetail().Status()].Name);
        self.orderDetail().Class(statusApp.order[self.orderDetail().Status()].Class);

        self.orderDetail().TotalRefunded(data.TotalRefunded);
        self.orderDetail().Debt(data.Debt);

    }
}
//ko.applyBindings(new DetailDepositViewModel());
var orderPopupModel = new orderPopupModel();

var chatViewModel = new ChatViewModel();
var detailOrderViewModel = new DetailOrderViewModel(orderPopupModel, chatViewModel);
orderPopupModel.parent = detailOrderViewModel;
ko.applyBindings(detailOrderViewModel, $("#contentMain")[0]);