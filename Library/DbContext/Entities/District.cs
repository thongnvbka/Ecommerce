namespace Library.DbContext.Entities
{
    // District
    
    public partial class District
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public int ProvinceId { get; set; } // ProvinceId
        public string ProvinceName { get; set; } // ProvinceName (length: 300)
        public string Culture { get; set; } // Culture (length: 2)

        public District()
        {
            Culture = "VN";
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
