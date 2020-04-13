namespace Library.DbContext.Entities
{
    // Ward

    public partial class Ward
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public int ProvinceId { get; set; } // ProvinceId
        public string ProvinceName { get; set; } // ProvinceName (length: 300)
        public int DistrictId { get; set; } // DistrictId
        public string DistrictName { get; set; } // DistrictName (length: 300)
        public string Culture { get; set; } // Culture (length: 2)

        public Ward()
        {
            Culture = "VN";
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
