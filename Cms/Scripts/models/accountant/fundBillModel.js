var fundBillDetailModel = function () {
    this.Id = ko.observable("");
    this.Code = ko.observable("");
    this.Type = ko.observable("");
    this.TypeName = ko.observable("");
    this.TypeClass = ko.observable("");
    this.Status = ko.observable("");
    this.CurrencyFluctuations = ko.observable("");

    this.Increase = ko.observable("");
    this.Diminishe = ko.observable("");

    this.CurencyStart = ko.observable("");
    this.CurencyEnd = ko.observable("");
    this.AccountantSubjectId = ko.observable("");
    this.AccountantSubjectName = ko.observable("");

    this.SubjectId = ko.observable("");
    this.SubjectCode = ko.observable("");
    this.SubjectName = ko.observable("");
    this.SubjectPhone = ko.observable("");
    this.SubjectEmail = ko.observable("");
    this.SubjectAddress = ko.observable("");

    this.FinanceFundId = ko.observable("");
    this.FinanceFundName = ko.observable("");
    this.FinanceFundBankAccountNumber = ko.observable("");
    this.FinanceFundDepartment = ko.observable("");
    this.FinanceFundNameBank = ko.observable("");
    this.FinanceFundUserFullName = ko.observable("");
    this.FinanceFundUserPhone = ko.observable("");
    this.FinanceFundUserEmail = ko.observable("");
    this.IsDelete = ko.observable("");
    this.TreasureId = ko.observable("");
    this.TreasureName = ko.observable("");
    this.Note = ko.observable("");
    this.UserId = ko.observable("");
    this.UserCode = ko.observable("");
    this.UserName = ko.observable("");
    this.UserApprovalId = ko.observable("");
    this.UserApprovalCode = ko.observable("");
    this.UserApprovalName = ko.observable("");

    this.OrderId = ko.observable("");
    this.OrderCode = ko.observable("");
    this.OrderType = ko.observable("");

    this.Created = ko.observable("");
    this.LastUpdated = ko.observable("");
};