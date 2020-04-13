using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class DrawMeta
    {
        ///<summary>
        /// Mã yêu cầu rút tiền
        ///</summary>
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string CustomerCode { get; set; } // CustomerCode (length: 20)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 100)

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
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật cuối cùng
        ///</summary>
        public DateTime? LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Mã nhân viên tiếp nhận xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên tiếp nhận
        ///</summary>
        public string UserName { get; set; } // UserName (length: 255)

        ///<summary>
        /// Trạng thái. 0: chưa xử lý, 1: đã xử lý
        ///</summary>
        public byte? Status { get; set; } // Status

        ///<summary>
        /// Ghi chú phiếu yêu cầu rút tiền
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Số tiền yêu cầu rút từ ví điện tử
        ///</summary>
        public decimal AdvanceMoney { get; set; } // AdvanceMoney

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)
    }
}
