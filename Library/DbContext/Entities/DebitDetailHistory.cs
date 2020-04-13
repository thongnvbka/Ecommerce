using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Entities
{
    public class DebitDetailHistory
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public byte Status { get; set; } // Status
        public string Note { get; set; } // Note
        public decimal? MustCollectMoney { get; set; } // MustCollectMoney
        public decimal? MustReturnMoney { get; set; } // MustReturnMoney
        public int? TreasureId { get; set; } // TreasureId
        public int? TreasureIdd { get; set; } // TreasureIdd
        public string TreasureName { get; set; } // TreasureName (length: 300)
        public int? FinanceFundId { get; set; } // FinanceFundId
        public string FinanceFundName { get; set; } // FinanceFundName (length: 500)
        public string FinanceFundBankAccountNumber { get; set; } // FinanceFundBankAccountNumber (length: 100)
        public string FinanceFundDepartment { get; set; } // FinanceFundDepartment (length: 300)
        public string FinanceFundNameBank { get; set; } // FinanceFundNameBank (length: 300)
        public string FinanceFundUserFullName { get; set; } // FinanceFundUserFullName (length: 300)
        public string FinanceFundUserPhone { get; set; } // FinanceFundUserPhone (length: 100)
        public string FinanceFundUserEmail { get; set; } // FinanceFundUserEmail (length: 300)
        public int? SubjectTypeId { get; set; } // SubjectTypeId
        public string SubjectTypeName { get; set; } // SubjectTypeName (length: 300)
        public int? AccountantSubjectId { get; set; } // AccountantSubjectId
        public string AccountantSubjectName { get; set; } // AccountantSubjectName (length: 100)
        public int? SubjectId { get; set; } // SubjectId
        public string SubjectCode { get; set; } // SubjectCode (length: 20)
        public string SubjectName { get; set; } // SubjectName (length: 300)
        public string SubjectPhone { get; set; } // SubjectPhone (length: 100)
        public string SubjectEmail { get; set; } // SubjectEmail (length: 300)
        public string SubjectAddress { get; set; } // SubjectAddress (length: 500)
        public int? OrderId { get; set; } // OrderId
        public byte? OrderType { get; set; } // OrderType
        public string OrderCode { get; set; } // OrderCode (length: 20)
        public int? UserId { get; set; } // UserId
        public string UserCode { get; set; } // UserCode (length: 20
        public string UserName { get; set; } // UserName (length: 300)
        public int? UserApprovalId { get; set; } // UserApprovalId
        public string UserApprovalCode { get; set; } // UserApprovalCode (length: 20)
        public string UserApprovalName { get; set; } // UserApprovalName (length: 300)
        public bool? IsSystem { get; set; } // IsSystem
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdated { get; set; } // LastUpdated
        public bool IsDelete { get; set; } // IsDelete

        public decimal? TotalMustCollectMoney { get; set; } // TotalMustCollectMoney
        public decimal? TotalMustReturnMoney { get; set; } // TotalMustReturnMoney
        public byte? Type { get; set; } // DebitType

        public List<DebitHistory> ListHistory { get; set; }

    }
}
