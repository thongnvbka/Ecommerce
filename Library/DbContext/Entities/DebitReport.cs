namespace Library.DbContext.Entities
{
    // DebitReport
    
    public partial class DebitReport
    {
        public long Id { get; set; } // Id (Primary key)
        public int? PackageId { get; set; } // PackageId
        public string PackageCode { get; set; } // PackageCode (length: 50)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte ServiceId { get; set; } // ServiceId
        public decimal Price { get; set; } // Price
        public int CustomerId { get; set; } // CustomerId
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        public DebitReport()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
