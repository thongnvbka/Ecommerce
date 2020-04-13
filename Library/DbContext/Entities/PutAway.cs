using System;

namespace Library.DbContext.Entities
{
    // PutAway

    public partial class PutAway
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã code bao hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái bao hàng (0: Mới khởi tạo, 1: Đã duyệt, 2: Trong kho, 3: Đang vận chuyển, 4: Mất, 5: Hoàn thành)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng cân nặng hàng hóa trong bao hàng
        ///</summary>
        public decimal? TotalWeight { get; set; } // TotalWeight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? TotalActualWeight { get; set; } // TotalActualWeight

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? TotalConversionWeight { get; set; } // TotalConversionWeight

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int PackageNo { get; set; } // PackageNo

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Id Kho hàng tạo bao
        ///</summary>
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Tên kho hàng đã tạo bao
        ///</summary>
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 500)

        ///<summary>
        /// Id Nhân viên tạo phiếu nhập bao
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên thực hiện tạo bao
        ///</summary>
        public string UserName { get; set; } // UserName (length: 50)

        ///<summary>
        /// Mã Code nhân viên thực hiện tạo phiếu
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Ngày bao được tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian sửa - Cập nhật bao hàng
        ///</summary>
        public DateTime Updated { get; set; } // Updated
        public string UnsignedText { get; set; } // UnsignedText (length: 500)
        public string Note { get; set; } // Note (length: 500)
        public bool IsDelete { get; set; } // IsDelete

        public PutAway()
        {
            PackageNo = 0;
            WarehouseIdPath = "";
            WarehouseAddress = "";
            UnsignedText = "";
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
