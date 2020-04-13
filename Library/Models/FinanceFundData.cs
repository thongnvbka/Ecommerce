using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    //[MetadataType(typeof(FinanceFundData))]
    //public partial class FinanceFund
    //{
    //}
    public class FinanceFundData
    {
        public int Id { get; set; } // Id (Primary key)
    
        public string IdPath { get; set; } // IdPath (length: 300)

        public string NamePath { get; set; } // NamePath (length: 500)

        [Required(ErrorMessage = "Please enter the amount")]
        public decimal Balance { get; set; } // Balance
        [Required(ErrorMessage = "Please enter the name of the fund")]
        [StringLength(500, ErrorMessage = "Fund name must not exceed 500 characters")]
        public string Name { get; set; } // Name (length: 500)
        [Required(ErrorMessage = "Please choose the fund father")]
        public int ParentId { get; set; } // ParentId
        public string ParentName { get; set; } // ParentName (length: 300)
        public byte Status { get; set; } // Status
        [StringLength(500, ErrorMessage = "Fund name must not exceed 500 characters ")]
        public string Description { get; set; } // Description (length: 500)
        public bool IsDelete { get; set; } // IsDelete
        [Required(ErrorMessage = "Please choose a manager")]
        public int UserId { get; set; } // UserId

        public string UserCode { get; set; } // UserCode (length: 100)
        
        public string UserFullName { get; set; } // UserFullName (length: 100)

        public string UserEmail { get; set; } // UserEmail (length: 50)

        public string UserPhone { get; set; } // UserPhone (length: 20)
        ///<summary>
        /// Tên chủ tài khoản ngân hàng
        ///</summary>
        public string CardName { get; set; } // CardName (length: 50)

        ///<summary>
        /// Số tài khoản
        ///</summary>
        public string CardId { get; set; } // CardId (length: 20)

        ///<summary>
        /// Ngân hàng
        ///</summary>
        public string CardBank { get; set; } // CardBank (length: 255)
         ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string CardBranch { get; set; } // CardBranch (length: 255)

        [Required(ErrorMessage = "Please select Currency")]
        public string Currency { get; set; } // Currency
    }
}
