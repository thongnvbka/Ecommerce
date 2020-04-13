using System;

namespace Library.DbContext.Entities
{
    // Bag
    
    public partial class Bag
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Ngày tạo bao
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Id nhân viên tạo bao
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên đầy đủ nhân viên
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Trạng thái bao: Mới tạo, Chờ nhập kho VN,..
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Mã bao (Xây dựng quy tắc tạo mã bao)
        ///</summary>
        public string Code { get; set; } // Code (length: 50)

        ///<summary>
        /// Id kho bao đang ở trong
        ///</summary>
        public int? WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Kiện tên kho bao đang ở trong
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Kho trước đây của bao
        ///</summary>
        public int? OdlWarehouseId { get; set; } // OdlWarehouseId

        ///<summary>
        /// Tên kho trước đây của bao
        ///</summary>
        public string OldWarehouseName { get; set; } // OldWarehouseName (length: 300)

        public Bag()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
