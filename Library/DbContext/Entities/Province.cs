namespace Library.DbContext.Entities
{
    // Province
    
    public partial class Province
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string Culture { get; set; } // Culture (length: 2)

        public Province()
        {
            Culture = "VN";
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
