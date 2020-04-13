namespace Library.DbContext.Entities
{
    // Partner
    
    public partial class Partner
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 50)
        public string Name { get; set; } // Name (length: 300)
        public string Description { get; set; } // Description (length: 500)
        public string Note { get; set; } // Note (length: 500)
        public string UnsignName { get; set; } // Note (length: MAX)
        public bool IsDelete { get; set; } // IsDelete
        public int PriorityNo { get; set; } // PriorityNo ()
        ///<summary>
        /// 0: Đối tác mới, 1 Đối tác hiện tại, 2: Đối tác cũ
        ///</summary>
        public byte Status { get; set; } // Status

        public Partner()
        {
            IsDelete = false;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
