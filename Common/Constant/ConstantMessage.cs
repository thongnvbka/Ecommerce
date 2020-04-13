namespace Common.Constant
{
    public class ConstantMessage
    {
        public static string DataIsNotValid = "Not enough data entered, or data were incorrect !";
        public static string SystemError = "[Error] The system has not performed your operation, please contact technician !";
        public static string CurrencyRealIsNotImpossible = "The amount deducted is greater than the amount receivable !";
        public static string BillIsApproval = "This transaction note has been approved !";
        public static string ClaimForRefundApproval = "This refund request has been completed !";

        public static string FundBillIsNotValid = "Deposit slip/Withdrawing fund slip does not exist !";
        public static string FundBillIsSuccess = "Deposit/Withdrawing fund slip has been approved!";

        public static string TicketIsNotValid = "Ticket does not exist or has been deleted !";

        public static string FinanceFundIsSuccess = "Fund does not exist or has been deleted !";
        public static string AccountantSubjectIsSuccess = "Type of account entry does not exist or has been deleted !";

        public static string VictimIsNotValid = "Subject does not exist or has been deleted !";

        public static string EmailCustomerIsValid = "Email of this potential customer has already existed !";
        public static string CustomerIsValid = "The customer of this email has been an official customer !";
        public static string SystemIsNotValid = "Please select the system before performing the action !";
        public static string WarehouseIsNotValid = "Please select a warehouse before performing the action !";
        public static string StaffIsNotValid = "Please select the staff in charge before performing the action !";
        public static string CustomerStypeIsValid = "Please select the type of customer before performing the action !";

        //public static string CustomerIsNotValid = "Khách hàng không tồn tại !";
        public static string AccountantSubjectIsNotValid = "Please select the accounting subject before performing the action !";
        public static string TreasureIsNotValid = "Please select the type of entry before performing the action !";

        #region Customer
        public static string CustomerIsNotValid = "Please choose customer before performing this action !";
        public static string EditCustomerSuccess = "Customer information edited successfully !";
        public static string CreateNewCustomerSuccess = "Create new customer successfully !";
        #endregion

        #region Order
        public static string OrderIsNotValid = "Order does not exist !";
        #endregion

        #region potentialCustomer
        public static string PotentialCustomerIsNotValid = "Customer does not exist or has been deleted !";
        public static string EditPotentialCustomerSuccess = "Edit customer information successfully !";
        public static string CreateNewPotentialCustomerSuccess = "Create potential customers successfully !";

        public static string CustomerTypeIsNotValid = "Customer type does not exist or has been deleted !";
        #endregion

        #region User
        public static string UserIsNotValid = "Employee does not exist or has been deleted !";
        #endregion

        #region RechargeBill
        public static string CreateNewRechargeBillIsSuccess = "E-wallet transactional slip has been created successfully !";
        public static string ApprovalRechargeBillIsSuccess = "E-wallet money adding/deducting slip has been approved successfully! !";
        public static string EditRechargeBillIsSuccess = "E-wallet money adding/deducting slip has been edited successfully!";
        public static string EditRechargeBillStatusIsNotImpossible = "Unable to edit already approved transaction !";
        public static string DeleteRechargeBillIsSuccess = "Transaction slip deleted successfully !";
        public static string CurrencyFluctuationsImpossible = "The amount you want to withdraw is greater than the available balance !";
        #endregion

        #region FundBill
        public static string ApprovalFundBillIsSuccess = "Fund deposit/withdrawal slip approved successfully !";
        public static string CreateFundBillIsSuccess = "Fund deposit/withdrawal slip created successfully !";
        public static string EditFundBillIsSuccess = "Fund deposit/withdrawal slip edited successfully !";
        public static string DeleteFundBillIsSuccess = "Fund deposit/withdrawal transaction deleted successfully !";
        #endregion

        #region Ticket

        #endregion

        #region ClaimForRefund
        public static string CreateNewClaimForRefundIsSuccess = "Complaint handling refund request created succesfully !";
        public static string UpdateClaimForRefundIsSuccess = "Complaint handling refund request updated successfully !";
        public static string DeleteClaimForRefundIsDetail = "Complaint handling refund request detail deleted successfully !";
        public static string ClaimForRefundIsNotValid = "Refund request slip does not exist !";
        public static string ClaimForRefundIsNotWating = "Refund request has been thoroughly processed, or does not exist !";
        public static string ExecuteClaimForRefundIsSuccess = "Confirm that customer refund has been successful !";
        public static string DeleteClaimForRefundIsSuccess = "Complaint refund request transaction has been deleted successfully!";
        public static string ClaimForRefundIsCancel = "Refund request from complaint handling was cancelled successfully !";

        #endregion

        #region MustCollect
        public static string CreateNewMustCollectIsSuccess = "Creating receivable note successfully !";
        public static string EditMustCollectStatusIsNotImpossible = "Cannot change already approved liabilities note !";
        public static string EditMustCollectIsSuccess = "Liabilities note edited successfully !";
        public static string MustCollectIsApproval = "This liabilities note has been approved !";
        public static string DeleteMustCollectIsSuccess = "Liabilities note deleted successfully !";
        public static string ApprovalMustCollectIsSuccess = "Receivables approved sucessfully! Money has been deposited to fund !";
        #endregion

        #region MustReturn
        public static string CreateNewMustReturnIsSuccess = "Creating payable note successfully !";
        public static string EditMustReturnStatusIsNotImpossible = "Cannot change already approved liabilities note !";
        public static string EditMustReturnIsSuccess = "Liabilities note edited successfully !";
        public static string MustReturnIsApproval = "This liabilities note has been approved !";
        public static string DeleteMustReturnIsSuccess = "Liabilities note deleted successfully !";
        public static string ApprovalMustReturnIsSuccess = "Account payable approved successfully! Money has been deposited to fund !";
        #endregion

        #region FinanceFund
        public static string FinanceFundIsNotValid = "Information of fund collecting money does not exist or has been deleted !";
        #endregion

        #region WithDrawal
        public static string WithDrawalIsApproval = "E-wallet withdrawal request has been processed !";
        public static string DeleteWithDrawalIsSuccess = "Deleted debt slip successfully !";
        #endregion

        #region Complain
        public static string ComplainReceived = " Has been accepted to handle by you!";
        public static string ComplainAvailable = " Does not exist or has been deleted!";
        public static string ComplainDiferent = "  Currently att other processing stages!";
        public static string ComplainPosenReceived = "  Someone else has accepted to handle!";
        public static string ComplainCanNot = "  Cannot accept to handle!";
        public static string ComplainNoCancel = "  Cannot cancel!";
        public static string ComplainCancelSuccess = "  Successfully cancelled!";
        public static string ComplainAssigned = "  Assigned to employees!";
        public static string ComplainOver = "  Exceeding the number of tickets allowed to process!";
        public static string ComplainNoAssign = "  Accept to handle!";
        public static string ComplainSupport = "  Add supporters successfully!";
        public static string ComplainLimit = "  Only one supporter is allowed to be added!";//Chỉ cho phép thêm 1 người hỗ trợ

        public static string ComplainIsSuccess = "  Complaints have been processed completely!";
        public static string CreateComplainIsSuccess = " Add ticket successfully!";

        #endregion

    }
}
