function CustomerLookUp(modelId) {
    var self = this;
    self.fundBillModel = ko.observable(new fundBillDetailModel());
    self.rechargeBillModel = ko.observable(new rechargeBillDetailModel());

    // Lấy Detail nạp/chi tiền quỹ - FundBill
    self.GetFundBillDetail = function (id) {
        $.post("/FundBill/GetFundBillDetail", { funBillId: id }, function (result) {
            console.log(result.fundBillModal);
            self.mapFundBillModel(result.fundBillModal);
            console.log(self.fundBillModel());
        });
    }
    self.viewMoneyFundDetail = function (id) {
        self.GetFundBillDetail(id);
        $('#moneyFundDetailModal').modal();
    }
    // Object FundBill - Detail phiếu nạp/Subtract funds
    self.mapFundBillModel = function (data) {
        self.fundBillModel(new fundBillDetailModel());

        self.fundBillModel().Id(data.Id);
        self.fundBillModel().Code(data.Code);
        self.fundBillModel().Type(data.Type);
        self.fundBillModel().TypeName(statusApp.typeFundBill[data.Type].Name);
        self.fundBillModel().TypeClass(statusApp.typeFundBill[data.Type].Class);
        self.fundBillModel().Status(data.Status);
        self.fundBillModel().CurrencyFluctuations( formatNumberic(data.CurrencyFluctuations, 'N2'));
        self.fundBillModel().CurencyStart(data.CurencyStart);
        self.fundBillModel().CurencyEnd(data.CurencyEnd);
        self.fundBillModel().AccountantSubjectId(data.AccountantSubjectId);
        self.fundBillModel().AccountantSubjectName(data.AccountantSubjectName);

        self.fundBillModel().SubjectId(data.SubjectId);
        self.fundBillModel().SubjectCode(data.SubjectCode);
        self.fundBillModel().SubjectName(data.SubjectName);
        self.fundBillModel().SubjectPhone(data.SubjectPhone);
        self.fundBillModel().SubjectEmail(data.SubjectEmail);
        self.fundBillModel().SubjectAddress(data.SubjectAddress);

        self.fundBillModel().FinanceFundId(data.FinanceFundId);
        self.fundBillModel().FinanceFundName(data.FinanceFundName);
        self.fundBillModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.fundBillModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.fundBillModel().FinanceFundNameBank(data.FinanceFundNameBank);
        self.fundBillModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.fundBillModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.fundBillModel().FinanceFundUserEmail(data.FinanceFundUserEmail);

        self.fundBillModel().IsDelete(data.IsDelete);
        self.fundBillModel().TreasureId(data.TreasureId);
        self.fundBillModel().TreasureName(data.TreasureName);
        self.fundBillModel().Note(data.Note);
        self.fundBillModel().UserId(data.UserId);
        self.fundBillModel().UserCode(data.UserCode);
        self.fundBillModel().UserName(data.UserName);
        self.fundBillModel().UserApprovalId(data.UserApprovalId);
        self.fundBillModel().UserApprovalCode(data.UserApprovalCode);
        self.fundBillModel().UserApprovalName(data.UserApprovalName);
        self.fundBillModel().Created(data.Created);
        self.fundBillModel().LastUpdated(data.LastUpdated);
    }
}