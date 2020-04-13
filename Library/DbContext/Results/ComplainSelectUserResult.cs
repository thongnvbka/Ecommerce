using System;

namespace Library.DbContext.Results
{
    public partial class ComplainSelectUserResult
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public byte TypeOrder { get; set; }
        public int TypeService { get; set; }
        public string TypeServiceName { get; set; }
        public int? TypeServiceClose { get; set; } 
        public string TypeServiceCloseName { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }
        public string ImagePath4 { get; set; }
        public string ImagePath5 { get; set; }
        public string ImagePath6 { get; set; }
        public string Content { get; set; }
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? SystemId { get; set; }
        public string SystemName { get; set; }
        public byte? Status { get; set; }
        public string LastReply { get; set; }
        public decimal? BigMoney { get; set; }
        public bool IsDelete { get; set; }
        public decimal? RequestMoney { get; set; }
        public string FullName { get; set; }
    }
}