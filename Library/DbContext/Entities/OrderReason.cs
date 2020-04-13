using System;

namespace Library.DbContext.Entities
{
    public partial class OrderReason
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderId { get; set; } // OrderId
        public byte ReasonId { get; set; } // ReasonId
        public string Reason { get; set; } // Reason (length: 500)
        public DateTime? CreateDate { get; set; } // CreateDate
        public byte Type { get; set; } // Type

        public OrderReason()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
