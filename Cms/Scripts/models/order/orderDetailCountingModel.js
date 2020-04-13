var orderDetailCountingModel = function () {
    this.Id = ko.observable("");
    this.OrderId = ko.observable("");
    this.OrderCode = ko.observable("");
    this.OrderType = ko.observable("");
    this.WebsiteName = ko.observable("");
    this.ShopId = ko.observable("");
    this.ShopName = ko.observable("");
    this.ShopLink = ko.observable("");
    this.WarehouseId = ko.observable("");
    this.WarehouseName = ko.observable("");
    this.CustomerId = ko.observable("");
    this.CustomerName = ko.observable("");
    this.CustomerEmail = ko.observable("");
    this.CustomerPhone = ko.observable("");

    this.CustomerAddress = ko.observable("");
    this.OrderDetailId = ko.observable("");
    this.Name = ko.observable("");
    this.Image = ko.observable("");
    this.Note = ko.observable("");
    this.Link = ko.observable("");
    this.Quantity = ko.observable("");
    this.Properties = ko.observable("");
    this.ProductNo = ko.observable("");
    this.BeginAmount = ko.observable("");
    this.Price = ko.observable("");
    this.ExchangePrice = ko.observable("");
    this.ExchangeRate = ko.observable("");
    this.TotalPrice = ko.observable("");


    this.TotalExchange = ko.observable("");
    this.UserId = ko.observable("");
    this.UserFullName = ko.observable("");
    this.OfficeId = ko.observable("");
    this.OfficeName = ko.observable("");
    this.OfficeIdPath = ko.observable("");
    this.QuantityLose = ko.observable("");
    this.Mode = ko.observable("");
    this.Status = ko.observable("");
    this.NotePrivate = ko.observable("");
    this.Created = ko.observable("");
    this.Updated = ko.observable("");
    this.TotalPriceLose = ko.observable("");
    this.TotalExchangeLose = ko.observable("");

    this.TotalPriceShop = ko.observable("");
    this.TotalExchangeShop = ko.observable("");
    this.TotalPriceCustomer = ko.observable("");
    this.IsDelete = ko.observable("");
};