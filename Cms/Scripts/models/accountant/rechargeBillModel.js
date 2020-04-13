var rechargeBillDetailModel = function () {
    this.Id = ko.observable("");
    this.Code = ko.observable("");
    this.Type = ko.observable("");
    this.TypeName = ko.observable("");
    this.TypeClass = ko.observable("");
    this.Status = ko.observable("");
    this.Note = ko.observable("");
    this.CurrencyFluctuations = ko.observable("");
    this.Increase = ko.observable("");
    this.Diminishe = ko.observable("");

    this.CurencyStart = ko.observable("");
    this.CurencyEnd = ko.observable("");
    this.UserId = ko.observable("");
    this.UserCode = ko.observable("");
    this.UserName = ko.observable("");
    this.UserApprovalId = ko.observable("");
    this.UserApprovalCode = ko.observable("");
    this.UserApprovalName = ko.observable("");
    this.CustomerId = ko.observable("");
    this.CustomerCode = ko.observable("");
    this.CustomerName = ko.observable("");
    this.CustomerPhone = ko.observable("");
    this.CustomerEmail = ko.observable("");
    this.CustomerAddress = ko.observable("");
    this.IsDelete = ko.observable("");

    this.TreasureId = ko.observable("");
    this.TreasureName = ko.observable("");
    this.TreasureIdd = ko.observable("");
    this.IsAutomatic = ko.observable("");
    this.OrderId = ko.observable("");
    this.OrderCode = ko.observable("");
    this.OrderType = ko.observable("");

    this.Created = ko.observable("");
    this.LastUpdated = ko.observable("");

};