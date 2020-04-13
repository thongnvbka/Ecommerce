using System;

namespace Library.DbContext.Entities
{
    // OrderService
    
    public partial class OrderService
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
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Kiểu tính, % hay Tiền
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Giá trị là tiền tệ hay %
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// Thành tiền VND
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice
        public string Note { get; set; } // Note (length: 600)

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        ///<summary>
        /// Phân nhóm dịch vụ: 0: Cung cấp, 1: Dịch vụ liên quan
        ///</summary>
        public byte Mode { get; set; } // Mode
        public bool Checked { get; set; } // Checked

        ///<summary>
        /// Trạn thái bị xóa hay không
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Thời gian tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        public OrderService()
        {
            Checked = false;
            IsDelete = false;
            Created = DateTime.Now;
            LastUpdate = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
