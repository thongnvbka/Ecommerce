var depositDetailModel = function () {
    this.Id = ko.observable(0);
    this.Code = ko.observable("");
    this.Type = ko.observable("");
    this.CreateDate = ko.observable("");
    this.UpdateDate = ko.observable("");
    this.CustomerName = ko.observable("");
    this.CustomerEmail = ko.observable("");
    this.CustomerPhone = ko.observable("");
    this.CustomerAddress = ko.observable("");
    this.Note = ko.observable("");
    this.WarehouseName = ko.observable("");
    
    this.ContactName = ko.observable("");
    this.ContactPhone = ko.observable("");
    this.ContactAddress = ko.observable("");
    this.ContactEmail = ko.observable("");
    this.PacketNumber = ko.observable("");

    this.Status = ko.observable(0);
    this.TotalAdvance = ko.observable("");
    this.ProvisionalMoney = ko.observable("");
    this.ExchangeRate = ko.observable("");
    this.WarehouseDeliveryName = ko.observable("");
    this.Total = ko.observable(0);
    this.TotalPayed = ko.observable(0);
    this.TotalRefunded = ko.observable(0);
    this.Debt = ko.observable(0);
}

