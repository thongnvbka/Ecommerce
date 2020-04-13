var DetailDepositViewModel = function (depositPopupModel,chatViewModel) {
    var self = this;
    self.listOrderService = ko.observableArray([]);
    self.listProductDetail = ko.observableArray([]);
    self.listOrderPackage = ko.observableArray([]);
    self.listOrderComment = ko.observableArray([]);
    self.listSourceSupplier = ko.observableArray([]);
    self.orderExchange = ko.observableArray([]);
    self.depositDetail = ko.observable(new depositDetailModel());
    self.isRending = ko.observable(false);
    self.selectedService = ko.observableArray([]);
    self.recharge = ko.observableArray([]);
    self.complains = ko.observableArray([]);
    self.orderPackage = ko.observableArray([]);
    //complain id
    self.isComplain = ko.observable();

    $(function () {
        var arr = _.split(window.location.href, "depositId=");
        if (arr.length > 1) {
            self.detailDeposit(arr[1]);
        }
    });

    self.selectedService.subscribe(function (newValue) {
        $.post("/" + window.culture + "/CMS/Order/UpdateService",
          { selectedService: self.selectedService() },
          function (result) {

          });
    });
    //Cập nhật dịch vụ
    self.updateService = function (data) {
        var obj = {
            orderId: self.depositDetail().Id(),
            serviceId: data.ServiceId,
            isCheck: data.Checked
        }
        $.ajax({
            type: "POST",
            url: "/" + window.culture + "/CMS/Order/UpdateService",
            data: obj,
            success: function (data) {
                if (data > 0) {
                    toastr.success('Cập nhật dịch vụ đơn hàng thành công');
                    window.location.reload();
                }
                else {
                    toastr.error('Cập nhật dịch vụ đơn hàng không thành công. Bạn vui lòng thao tác lại trong giây lát');
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
    //Todo chi tiết đơn hàng ký gửi
    self.detailDeposit = function (id) {
        self.listOrderService([]);
        self.listProductDetail([]);
        self.listOrderComment([]);
        self.orderExchange([]);
        self.orderPackage([]);
        self.recharge([]);
        self.complains([]);
        self.isRending(false);
        $.post("/" + window.culture + "/CMS/Order/DetailDeposit",
            { orderId: id },
            function (result) {
                if (result.model.DepositViewItem != null) {
                    self.mapDepositDetail(result.model.DepositViewItem);
                }
                self.listOrderService(result.model.ListOrderService);
                self.listProductDetail(result.model.ListDetail);
                self.listOrderComment(result.model.ListOrderComment);
                self.orderExchange(result.model.OrderExchange);
                self.orderPackage(result.model.OrderPackages);
                self.recharge(result.model.RechargeBill);
                self.complains(result.model.Complains);
                self.isComplain(result.isComplain);
                chatViewModel.showChat(self.depositDetail().Id(), self.depositDetail().Code(), self.depositDetail().Type());
                self.getServicesOther(id);
                self.isRending(true);
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
        window.location.href = "/" + window.culture + "/CMS/Order/DepositOrder";
        //self.GetAll();
        //self.templateId("deposit");
    };

    //Cập nhật thông tin ghi chú
    self.updateNote = function () {
        var id = self.depositDetail().Id();
        var note = self.depositDetail().Note();
        $.post("/" + window.culture + "/CMS/Order/UpdateNote",
           { orderId: id, note: note },
           function (result) {
              
           });

    };

    //Map thông tin chi tiết đơn hàng ký gửi
    self.mapDepositDetail = function (data) {
        self.depositDetail(new depositDetailModel());
        self.depositDetail().Id(data.Id);
        self.depositDetail().Code(data.Code);
        self.depositDetail().Type(data.Type);
        self.depositDetail().CreateDate(data.CreateDate);
        self.depositDetail().UpdateDate(data.UpdateDate);
        self.depositDetail().CustomerName(data.CustomerName);
        self.depositDetail().CustomerEmail(data.CustomerEmail);
        self.depositDetail().CustomerPhone(data.CustomerPhone);
        self.depositDetail().CustomerAddress(data.CustomerAddress);
        self.depositDetail().Note(data.Note);
        self.depositDetail().WarehouseName(data.WarehouseName);

        self.depositDetail().ContactName(data.ContactName);
        self.depositDetail().ContactPhone(data.ContactPhone);
        self.depositDetail().ContactAddress(data.ContactAddress);
        self.depositDetail().ContactEmail(data.ContactEmail);
        self.depositDetail().ContactEmail(data.ContactEmail);

        self.depositDetail().Status(data.Status);
        self.depositDetail().TotalAdvance(data.TotalAdvance);
        self.depositDetail().ProvisionalMoney(data.ProvisionalMoney);
        self.depositDetail().Total(data.Total);
        self.depositDetail().TotalPayed(data.TotalPayed);
        self.depositDetail().ExchangeRate(data.ExchangeRate);
        self.depositDetail().WarehouseDeliveryName(data.WarehouseDeliveryName);
        self.depositDetail().TotalRefunded(data.TotalRefunded);
        self.depositDetail().Debt(data.Debt);
    }

}
//ko.applyBindings(new DetailDepositViewModel());
var chatViewModel = new ChatViewModel();
var depositPopupModel = new depositPopupModel();
var detailDepositViewModel = new DetailDepositViewModel(depositPopupModel, chatViewModel);
depositPopupModel.parent = detailDepositViewModel;

ko.applyBindings(detailDepositViewModel, $("#contentTabDetailOrder")[0]);