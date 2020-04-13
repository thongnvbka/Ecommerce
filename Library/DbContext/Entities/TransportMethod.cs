namespace Library.DbContext.Entities
{
    // TransportMethod
    
    public partial class TransportMethod
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// 0: Mới, 1: Đang sử dụng, 3: Cũ
        ///</summary>
        public byte Status { get; set; } // Status

        public TransportMethod()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
