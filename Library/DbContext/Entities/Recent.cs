namespace Library.DbContext.Entities
{
    // Recent
    
    public partial class Recent
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id bàn ghi
        ///</summary>
        public int RecordId { get; set; } // RecordId

        ///<summary>
        /// Mode
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Số lần sử dụng
        ///</summary>
        public int CountNo { get; set; } // CountNo

        ///<summary>
        /// UserId
        ///</summary>
        public int UserId { get; set; } // UserId

        public Recent()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
