namespace Library.DbContext.Entities
{
    // Entrepot
    
    public partial class Entrepot
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// 0: Mới, 1: Hiện tại, 2: Cũ
        ///</summary>
        public byte? Status { get; set; } // Status

        public Entrepot()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
