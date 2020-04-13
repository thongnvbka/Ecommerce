using System;
namespace Library.ViewModels.Items
{
    public class DrawItem
    {
        ///<summary>
        /// Mã yêu cầu rút tiền
        ///</summary>
        public int Id { get; set; } // Id (Primary key)
        

        ///<summary>
        /// Tên chủ tài khoản ngân hàng
        ///</summary>
        public string CardName { get; set; } // CardName (length: 50)

        ///<summary>
        /// Số tài khoản
        ///</summary>
        public string CardId { get; set; } // CardId (length: 20)

        ///<summary>
        /// Ngân hàng
        ///</summary>
        public string CardBank { get; set; } // CardBank (length: 255)

        ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string CardBranch { get; set; } // CardBranch (length: 255)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate
        ///<summary>
        /// Trạng thái. 0: chưa xử lý, 1: đã xử lý
        ///</summary>
        public byte Status { get; set; } // Status
        ///<summary>
        /// Ghi chú phiếu yêu cầu rút tiền
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Số tiền yêu cầu rút từ ví điện tử
        ///</summary>
        public decimal AdvanceMoney { get; set; } // AdvanceMoney
    }
}
