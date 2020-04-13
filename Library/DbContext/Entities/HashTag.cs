namespace Library.DbContext.Entities
{
    // HashTag
    
    public partial class HashTag
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên Hasgtag
        ///</summary>
        public string Name { get; set; } // Name (length: 150)

        ///<summary>
        /// Mô tả về HashTag
        ///</summary>
        public string Description { get; set; } // Description (length: 300)
        public bool IsDelete { get; set; } // IsDelete

        public HashTag()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
