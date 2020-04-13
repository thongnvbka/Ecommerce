using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DbContext.Entities
{

    // Deposit

    public partial class Deposit
    {

        ///<summary>
        /// Mã đơn ký gửi
        ///</summary>
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã yêu cầu
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Họ tên người nhận
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 255)
        public byte LevelId { get; set; } // LevelId
        public string LevelName { get; set; } // LevelName (length: 300)
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Tên liên lạc lấy hàng bên TQ
        ///</summary>
        public string ContactName { get; set; } // ContactName (length: 100)

        ///<summary>
        /// Điện thoại người liên lạc bên TQ
        ///</summary>
        public string ContactPhone { get; set; } // ContactPhone (length: 20)

        ///<summary>
        /// Địa chỉ người liên lạc bên TQ
        ///</summary>
        public string ContactAddress { get; set; } // ContactAddress (length: 255)

        ///<summary>
        /// Điện thoại người nhận
        ///</summary>
        public string ContactEmail { get; set; } // ContactEmail (length: 300)

        ///<summary>
        /// Mã người dùng
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên người dùng
        ///</summary>
        public string UserName { get; set; } // UserName (length: 255)
        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        ///<summary>
        /// Loại đơn hàng order, ký gửi, tìm nguồn
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PacketNumber { get; set; } // PacketNumber

        ///<summary>
        /// Ghi chú đơn ký gửi
        ///</summary>
        public string Description { get; set; } // Description (length: 1000)

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status
        public int SystemId { get; set; } // SystemId
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Mã kho TQ khách gửi hàng
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho TQ khách gửi hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 500)
        /// <summary>
        /// Tổng tiền tạm tính
        /// </summary>
        public decimal ProvisionalMoney { get; set; } // ProvisionalMoney
        public double TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate
        public bool IsDelete { get; set; } // IsDelete
        ///<summary>
        /// Phục vụ tìm kiếm
        ///</summary>
        public string UnsignName { get; set; } // UnsignName (length: 500)
        /// <summary>
        /// Lý do hủy đơn hàng
        /// </summary>
        public string ReasonCancel { get; set; }
        /// <summary>
        /// Loại hàng. 0: không pin, 1: có pin
        /// </summary>
        public int? DepositType { get; set; }
        [NotMapped]
        public int? Chat { get; set; }
        [NotMapped]
        public string ChatContent { get; set; }
        ///<summary>
        /// Mã kho giao hàng
        ///</summary>
        public int? WarehouseDeliveryId { get; set; } // WarehouseDeliveryId

        ///<summary>
        /// Tên kho giao hàng
        ///</summary>
        public string WarehouseDeliveryName { get; set; } // WarehouseDeliveryName (length: 500)
        /// <summary>
        /// Đơn vị tính duyệt
        /// </summary>
        public string ApprovelUnit { get; set; }
        /// <summary>
        /// Giá duyệt
        /// </summary>
        public decimal? ApprovelPrice { get; set; }
        /// <summary>
        /// Khác null là yêu cầu đi công và cần nhập thêm thông tin về để có thể đi công
        /// </summary>
        public int? OrderInfoId { get; set; }

        public Deposit()
        {
            ExchangeRate = 0m;
            UnsignName = "";
            ProvisionalMoney = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
