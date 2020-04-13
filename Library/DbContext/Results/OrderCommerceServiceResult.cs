using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Results
{
    public class OrderCommerceServiceResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Id dịch vụ
        ///</summary>
        public int ServiceId { get; set; } // ServiceId

        ///<summary>
        /// Tên dịch vụ: Kiểm đếm, đóng kiện,...
        ///</summary>
        public string ServiceName { get; set; } // ServiceName (length: 300)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate
        
        ///<summary>
        /// Giá trị là tiền tệ hay %
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// Thành tiền VND
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice
        
        public bool Checked { get; set; } // Checked

        ///<summary>
        /// Phân nhóm dịch vụ: 0: Cung cấp, 1: Dịch vụ liên quan
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Kiểu tính, % hay Tiền
        ///</summary>
        public byte Type { get; set; } // Type
    }
}
