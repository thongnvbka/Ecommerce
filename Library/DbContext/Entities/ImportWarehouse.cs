using System;

namespace Library.DbContext.Entities
{
    // ImportWarehouse
    public partial class ImportWarehouse
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu nhập kho
        ///</summary>
        public string Code { get; set; } // Code (length: 20)

        ///<summary>
        /// Trạng thái nhập kho (0: Mới khởi tạo, 1: Đã duyệt)
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PackageNumber { get; set; } // PackageNumber

        ///<summary>
        /// Số bao
        ///</summary>
        public int? WalletNumber { get; set; } // WalletNumber
        public int WarehouseId { get; set; } // WarehouseId
        public string WarehouseIdPath { get; set; } // WarehouseIdPath (length: 300)

        ///<summary>
        /// Tên kho nhập hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        public string WarehouseAddress { get; set; } // WarehouseAddress (length: 300)
        public string ShipperName { get; set; } // ShipperName (length: 300)
        public string ShipperPhone { get; set; } // ShipperPhone (length: 300)
        public string ShipperAddress { get; set; } // ShipperAddress (length: 300)
        public string ShipperEmail { get; set; } // ShipperEmail (length: 300)

        ///<summary>
        /// Id nhân viên tạo phiếu
        ///</summary>
        public int? UserId { get; set; } // UserId
        public string UserName { get; set; } // UserName (length: 30)

        ///<summary>
        /// Họ tên nhân viên tạo phiếu
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Id quản lý kho
        ///</summary>
        public int? WarehouseManagerId { get; set; } // WarehouseManagerId

        ///<summary>
        /// Code quản lý kho
        ///</summary>
        public string WarehouseManagerCode { get; set; } // WarehouseManagerCode (length: 20)

        ///<summary>
        /// Họ tên quản lý kho
        ///</summary>
        public string WarehouseManagerFullName { get; set; } // WarehouseManagerFullName (length: 300)

        ///<summary>
        /// Id kế toán viên
        ///</summary>
        public int? WarehouseAccountantId { get; set; } // WarehouseAccountantId

        ///<summary>
        /// Code kế toán viên
        ///</summary>
        public string WarehouseAccountantCode { get; set; } // WarehouseAccountantCode (length: 20)

        ///<summary>
        /// Họ tên kế toán viên
        ///</summary>
        public string WarehouseAccountantFullName { get; set; } // WarehouseAccountantFullName (length: 300)
        public string UnsignedText { get; set; } // UnsignedText
        public string Note { get; set; } // Note (length: 500)
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public bool IsDelete { get; set; } // IsDelete

        public ImportWarehouse()
        {
            UnsignedText = "";
            Created = DateTime.Now;
            Updated = DateTime.Now;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
