namespace Library.DbContext.Entities
{
    // OrderAddress
    
    public partial class OrderAddress
    {
        public int Id { get; set; } // Id (Primary key)
        public int? ProvinceId { get; set; } // ProvinceId
        public int? DistrictId { get; set; } // DistrictId
        public int? WardId { get; set; } // WardId
        public string Address { get; set; } // Address (length: 600)
        public string ProvinceName { get; set; } // ProvinceName (length: 300)
        public string DistrictName { get; set; } // DistrictName (length: 300)
        public string WardName { get; set; } // WardName (length: 300)
        public string Phone { get; set; } // Phone (length: 300)
        public string FullName { get; set; } // FullName (length: 150)

        public OrderAddress()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
