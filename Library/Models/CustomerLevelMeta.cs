using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class CustomerLevelMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Please enter a customer vip level name")]
        [StringLength(255, ErrorMessage = "Customer level name cannot be more than 300 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a customer level description")]
        [StringLength(500, ErrorMessage = "Customer level description cannot be more than 500 characters ")]
        public string Description { get; set; }

        public bool Status { get; set; } // Status

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; } // CreateDate

        public DateTime? UpdateDate { get; set; } // UpdateDate
            
        public decimal StartMoney { get; set; } // StartMoney
        
        public decimal EndMoney { get; set; } // EndMoney

        [Required(ErrorMessage = "Please enter a percentage of your offer creation")]
        [Range(1, 100)]
        public int PercentDeposit { get; set; } // PercentDeposit

        public bool IsDelete { get; set; } // IsDelete
    }
}