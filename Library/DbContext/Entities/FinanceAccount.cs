using System;
namespace Library.DbContext.Entities
{
    

    // FinanceAccount
    
    public partial class FinanceAccount
    {
        public int AccountId { get; set; } // AccountId (Primary key)
        public string Card { get; set; } // Card (length: 50)
        public string FullName { get; set; } // FullName (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Description { get; set; } // Description (length: 255)
        public bool? Status { get; set; } // Status
        public DateTime? CreateDate { get; set; } // CreateDate
        public DateTime? UpdateDate { get; set; } // UpdateDate
        public DateTime? LastUpdate { get; set; } // LastUpdate
        public decimal? MoneyAvaiable { get; set; } // MoneyAvaiable
        public decimal? MoneyCurrent { get; set; } // MoneyCurrent
        public string DeputyName { get; set; } // DeputyName (length: 50)
        public string DeputyEmail { get; set; } // DeputyEmail (length: 50)
        public string DeputyPhone { get; set; } // DeputyPhone (length: 20)
        public string DeputyCard { get; set; } // DeputyCard (length: 20)
        public string DeputyAddress { get; set; } // DeputyAddress (length: 255)
        public int? FundId { get; set; } // FundId

        public FinanceAccount()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
