var sourcingDetailModel = function () {
    this.Id = ko.observable(0);
    this.Code = ko.observable("");
    this.Type = ko.observable("");
    this.AnalyticSupplier = ko.observable("");
    this.CreateDate = ko.observable("");
    this.UpdateDate = ko.observable("");
    this.Status = ko.observable(0);
    this.ServiceMoney = ko.observable("");
    this.TypeService = ko.observable("");
    this.ServiceType = ko.observable("");
    this.ShipMoney = ko.observable("");
    this.SourceSupplierId = ko.observable("");

    this.CustomerName = ko.observable("");

    this.CustomerEmail = ko.observable("");
    this.CustomerPhone = ko.observable("");
    this.CustomerAddress = ko.observable("");
    this.TypeServiceName = ko.observable("");
}

