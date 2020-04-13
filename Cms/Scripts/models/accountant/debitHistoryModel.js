var debitHistoryModel = function () {
    this.Id = ko.observable("");
    this.Code = ko.observable("");
    this.Status = ko.observable("");
    this.Note = ko.observable("");
    this.DebitId = ko.observable("");
    this.DebitType = ko.observable("");
    this.DebitCode = ko.observable("");
    this.Money = ko.observable("");
    this.OrderId = ko.observable("");
    this.OrderType = ko.observable("");
    this.OrderCode = ko.observable("");
    this.PayReceivableId = ko.observable("");
    this.PayReceivableIdd = ko.observable("");
    this.PayReceivableIName = ko.observable("");

    this.IsSystem = ko.observable("");
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
    this.TreasureId = ko.observable("");
    this.TreasureName = ko.observable("");


    this.Created = ko.observable("");
    this.LastUpdated = ko.observable("");

    this.StatusHistoryName = ko.observable("");
    this.StatusHistoryClass = ko.observable("");
    this.TypeHistoryName = ko.observable("");
    this.TypeHistoryClass = ko.observable("");
}
