using System;

namespace Library.DbContext.Entities
{
    // HistorySatus
    
    public partial class HistorySatu
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Thời gian thay đổi
        ///</summary>
        public DateTime Time { get; set; } // Time

        ///<summary>
        /// Trạng thái thay đổi
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Nhân viên thay đổi
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Ghi chú thay đổi
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Id bản ghi được thay đổi
        ///</summary>
        public int RecordId { get; set; } // RecordId

        ///<summary>
        /// Loại của bản ghi được thay đổi: vd: 0: Trạng thái đơn hàng, 1: Trạng thái kiện hàng,...
        ///</summary>
        public byte Mode { get; set; } // Mode
        public string UserFullName { get; set; } // UserFullName (length: 150)
        public string Json { get; set; } // Json

        public HistorySatu()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
