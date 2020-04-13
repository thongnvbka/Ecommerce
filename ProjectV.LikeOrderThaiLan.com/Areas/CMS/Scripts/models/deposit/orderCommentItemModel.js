var orderCommentItemModel = function () {
    this.Id = ko.observable("");
    this.Description = ko.observable("");
    this.OrderId = ko.observable("");
    this.CustomerId = ko.observable("");
    this.UserId = ko.observable("");
    this.CreateDate = ko.observable("");
    this.IsRead = ko.observable("");
    this.CustomerName = ko.observable("");
    this.UserName = ko.observable("");
    this.SystemId = ko.observable("");
    this.SystemName = ko.observable("");
    this.OrderType = ko.observable("");
    this.CommentType = ko.observable("");
    this.UserOffice = ko.observable("");
    this.UserPhone = ko.observable("");
    this.GroupId = ko.observable("");
};