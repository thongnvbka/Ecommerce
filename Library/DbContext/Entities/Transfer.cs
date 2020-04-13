using System;

namespace Library.DbContext.Entities
{
    // Transfer
    
    public partial class Transfer
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái: 0: Mới tạo, 1: Đã hoàn thành
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng số cân nặng của kiện hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng chuyển đổi
        ///</summary>
        public decimal? TotalWeightConverted { get; set; } // TotalWeightConverted

        ///<summary>
        /// Tổng cân nặng tính tiền
        ///</summary>
        public decimal? TotalWeightActual { get; set; } // TotalWeightActual

        ///<summary>
        /// Tổng số bao
        ///</summary>
        public int WalletNo { get; set; } // WalletNo

        ///<summary>
        /// Tổng số kiện hàng
        ///</summary>
        public int PackageNo { get; set; } // PackageNo
        public string UnsignedText { get; set; } // UnsignedText

        ///<summary>
        /// Ghi chú
        ///</summary>
        public string Note { get; set; } // Note (length: 500)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Nhân viên tạo
        ///</summary>
        public int FromUserId { get; set; } // FromUserId

        ///<summary>
        /// Từ
        ///</summary>
        public string FromUserFullName { get; set; } // FromUserFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên tạo phiếu
        ///</summary>
        public string FromUserUserName { get; set; } // FromUserUserName (length: 50)

        ///<summary>
        /// Chức vụ của nhân viên tạo phiếu
        ///</summary>
        public int FromUserTitleId { get; set; } // FromUserTitleId

        ///<summary>
        /// Tên chức vụ của nhân viên tạo phiếu
        ///</summary>
        public string FromUserTitleName { get; set; } // FromUserTitleName (length: 300)

        ///<summary>
        /// Phòng ban nhân viên tạo phiếu
        ///</summary>
        public int FromWarehouseId { get; set; } // FromWarehouseId

        ///<summary>
        /// Tên phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string FromWarehouseName { get; set; } // FromWarehouseName (length: 300)

        ///<summary>
        /// IdPath phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string FromWarehouseIdPath { get; set; } // FromWarehouseIdPath (length: 500)

        ///<summary>
        /// Ngày khởi tạo
        ///</summary>
        public DateTime FromTime { get; set; } // FromTime

        ///<summary>
        /// Nhân viên xác thực
        ///</summary>
        public int? ToUserId { get; set; } // ToUserId

        ///<summary>
        /// Tên đầy đủ nhân viên xác thực
        ///</summary>
        public string ToUserFullName { get; set; } // ToUserFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên xác thực
        ///</summary>
        public string ToUserUserName { get; set; } // ToUserUserName (length: 50)

        ///<summary>
        /// Chức vụ nhân viên xác thực
        ///</summary>
        public int? ToUserTitleId { get; set; } // ToUserTitleId

        ///<summary>
        /// Tiêu đề của nhân viên xác thực
        ///</summary>
        public string ToUserTitleName { get; set; } // ToUserTitleName (length: 300)

        ///<summary>
        /// Id đơn vị của nhân viên xác thực
        ///</summary>
        public int ToWarehouseId { get; set; } // ToWarehouseId

        ///<summary>
        /// PHòng ban của nhân viên xác thực
        ///</summary>
        public string ToWarehouseName { get; set; } // ToWarehouseName (length: 300)

        ///<summary>
        /// IdPath đơn vị của nhân viên xác thực
        ///</summary>
        public string ToWarehouseIdPath { get; set; } // ToWarehouseIdPath (length: 500)

        ///<summary>
        /// IdPath đơn vị của nhân viên xác thực
        ///</summary>
        public DateTime? ToTime { get; set; } // ToTime
        public decimal? PriceShip { get; set; } // PriceShip

        public Transfer()
        {
            WalletNo = 0;
            PackageNo = 0;
            UnsignedText = "";
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
