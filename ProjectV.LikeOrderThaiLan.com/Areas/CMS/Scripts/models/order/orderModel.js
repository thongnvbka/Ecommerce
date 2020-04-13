var orderModel = function () {
    this.Id = ko.observable(0);
    this.Code = ko.observable("");
    this.Type = ko.observable("");
    this.WebsiteName = ko.observable("");
    this.ShopId = ko.observable("");
    this.ShopName = ko.observable("");
    this.ShopLink = ko.observable("");
    this.ProductNo = ko.observable("");
    this.PackageNo = ko.observable("");
    this.ContractCode = ko.observable("");
    this.ContractCodes = ko.observable("");
    this.LevelId = ko.observable("");
    this.LevelName = ko.observable("");
    this.TotalWeight = ko.observable("");
    this.DiscountType = ko.observable("");
    this.DiscountValue = ko.observable("");
    this.GiftCode = ko.observable("");
    this.CreatedTool = ko.observable("");
    this.Currency = ko.observable(0);
    this.ExchangeRate = ko.observable(0);
    
    this.HashTag = ko.observable("");
    this.WarehouseId = ko.observable("");
    this.WarehouseName = ko.observable("");
    this.CustomerId = ko.observable("");
    this.CustomerName = ko.observable("");
    this.CustomerEmail = ko.observable("");
    this.CustomerPhone = ko.observable("");
    this.CustomerAddress = ko.observable("");
    this.Status = ko.observable(0);
    this.StatusName = ko.observable("");
    this.StatusClass = ko.observable("");
    this.UserId = ko.observable("");
    this.UserFullName = ko.observable("");
    this.OfficeId = ko.observable("");
    this.OfficeName = ko.observable("");
    this.OfficeIdPath = ko.observable("");
    this.CreatedOfficeIdPath = ko.observable("");
    this.CreatedUserId = ko.observable("");
    this.CreatedUserFullName = ko.observable("");
    this.CreatedOfficeId = ko.observable("");
    this.CreatedOfficeName = ko.observable("");
    this.OrderInfoId = ko.observable("");
    this.FromAddressId = ko.observable("");
    this.ToAddressId = ko.observable("");
    this.SystemId = ko.observable("");
    this.SystemName = ko.observable("");
    this.ServiceType = ko.observable("");
    this.Note = ko.observable("");
    this.PrivateNote = ko.observable("");
    this.LinkNo = ko.observable("");
    this.IsDelete = ko.observable("");
    this.Created = ko.observable("");
    this.LastUpdate = ko.observable("");
    this.ListDetail = ko.observableArray([]);
    this.ListOrderService = ko.observableArray([]);
    this.ReasonCancel = ko.observable("");
    this.PriceBargain = ko.observable(0);
    this.ExpectedDate = ko.observable("");
    this.PaidShop = ko.observable(0);
    this.FeeShip = ko.observable(0);
    this.FeeShipBargain = ko.observable(0);
    this.IsPayWarehouseShip = ko.observable(false);
    this.UserNote = ko.observable("");
    this.WarehouseDeliveryId = ko.observable("");
    this.WarehouseDeliveryName = ko.observable("");

    //Tổng tiền hàng của khách
    this.TotalExchange = ko.observable(0);

    this.Total = ko.observable(0);

    //Tiền hàng của khách
    this.TotalPrice = ko.observable(0);
    //Tiền hàng công ty thanh toán
    this.TotalCompanyPrice = ko.observable(0);
    //Tiền khách phải trả NDT
    this.TotalPriceCustomer = ko.observable(0);
    //Tiền phải thanh toán với shop NDT
    this.TotalShop = ko.observable(0);
    //Tiền khách nợ
    this.CashShortage = ko.observable(0);
};