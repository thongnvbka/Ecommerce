var DetailSourcingViewModel = function (chatViewModel) {
    var self = this;
    self.listOrderService = ko.observableArray([]);
    self.listProductDetail = ko.observableArray([]);
    self.listOrderPackage = ko.observableArray([]);
    self.listOrderComment = ko.observableArray([]);
    self.listSourceSupplier = ko.observableArray([]);
    self.sourcingDetail = ko.observable(new sourcingDetailModel());
    self.sourcingProduct = ko.observable(new sourcingProductModel());
    self.type = ko.observable();

    $(function () {
        var arr = _.split(window.location.href, "sourcingId=");
        if (arr.length > 1) {
            self.detailSourcing(arr[1]);
        }
    });
    //Todo chi tiết đơn hàng tìm nguồn
    self.detailSourcing = function (id) {
        self.listOrderComment([]);
        self.listSourceSupplier([]);
        $.post("/" + window.culture + "/CMS/Order/DetailSourcing",
            { orderId: id },
            function (result) {

                if (result.model.SourceDetailItem != null) {
                    self.mapSourcingDetail(result.model.SourceDetailItem);
                }
                if (result.model.SourceProductItem != null) {
                    self.mapSourcingProduct(result.model.SourceProductItem);
                }
                self.listOrderComment(result.model.ListOrderComment);
                self.listSourceSupplier(result.model.ListSourceSupplier);
                chatViewModel.showChat(self.sourcingDetail().Id(), self.sourcingDetail().Code(), result.typeSourcing);
            });
    }
    
    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Order/Sourcing";
        //self.GetAll();
        //self.templateId("deposit");
    };

    //Map thông tin chi tiết đơn hàng tìm nguồn
    self.mapSourcingDetail = function (data) {
        self.sourcingDetail(new sourcingDetailModel());
        self.sourcingDetail().Id(data.Id);
        self.sourcingDetail().Code(data.Code);
        self.sourcingDetail().Type(data.Type);
        self.sourcingDetail().AnalyticSupplier(data.AnalyticSupplier);
        self.sourcingDetail().CreateDate(data.CreateDate);
        self.sourcingDetail().UpdateDate(data.UpdateDate);
        self.sourcingDetail().Status(data.Status);
        self.sourcingDetail().ServiceMoney(data.ServiceMoney);
        self.sourcingDetail().TypeService(data.TypeService);
        self.sourcingDetail().ServiceType(data.ServiceType);
        self.sourcingDetail().ShipMoney(data.ShipMoney);
        self.sourcingDetail().SourceSupplierId(data.SourceSupplierId);

        self.sourcingDetail().CustomerName(data.CustomerName);

        self.sourcingDetail().CustomerEmail(data.CustomerEmail);
        self.sourcingDetail().CustomerPhone(data.CustomerPhone);
        self.sourcingDetail().CustomerAddress(data.CustomerAddress);
        self.sourcingDetail().TypeServiceName(data.TypeServiceName);
    }

    //Map thông tin sản phẩm tìm nguồn
    self.mapSourcingProduct = function (data) {
        self.sourcingProduct(new sourcingProductModel());
        self.sourcingProduct().Name(data.Name);
        self.sourcingProduct().Quantity(data.Quantity);
        self.sourcingProduct().Link(data.Link);
        self.sourcingProduct().CategoryName(data.CategoryName);

        self.sourcingProduct().Size(data.Size);
        self.sourcingProduct().Color(data.Color);
        self.sourcingProduct().Note(data.Note);
        if (data.ImagePath1.length > 0) {
            self.sourcingProduct().ImagePath1(data.ImagePath1);
        }
        else {
            self.sourcingProduct().ImagePath1('~/Content/images/icon-home/no.jpg');
        }
        if (data.ImagePath2.length > 0) {
            self.sourcingProduct().ImagePath2(data.ImagePath2);
        }
        else {
            self.sourcingProduct().ImagePath2('~/Content/images/icon-home/no.jpg');
        }
        if (data.ImagePath3.length > 0) {
            self.sourcingProduct().ImagePath3(data.ImagePath3);
        }
        else {
            self.sourcingProduct().ImagePath3('~/Content/images/icon-home/no.jpg');
        }
        if (data.ImagePath4.length > 0) {
            self.sourcingProduct().ImagePath4(data.ImagePath4);
        }
        else {
            self.sourcingProduct().ImagePath4('~/Content/images/icon-home/no.jpg');
        }
        
    }

}
//ko.applyBindings(new DetailDepositViewModel());
var chatViewModel = new ChatViewModel();
var detailSourcingViewModel = new DetailSourcingViewModel(chatViewModel);
ko.applyBindings(detailSourcingViewModel, $("#contentTabDetailSourcing")[0]);