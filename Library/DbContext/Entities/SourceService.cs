using System;

namespace Library.DbContext.Entities
{

    // SourceService

    public partial class SourceService
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên gói dịch vụ tìm nguồn
        ///</summary>
        public string Name { get; set; } // Name (length: 255)

        ///<summary>
        /// Mô tả gói dịch vụ
        ///</summary>
        public string Description { get; set; } // Description

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật cuối cùng
        ///</summary>
        public DateTime? LastUpdateDate { get; set; } // LastUpdateDate

        ///<summary>
        /// Phí duy trì trên tháng
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Trạng thái gói dịch vụ
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Bản ghi đã xóa hay không
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete
        public int? CreateId { get; set; } // CreateId
        public string CreateName { get; set; } // CreateName (length: 255)
        public int? UpdateId { get; set; } // UpdateId
        public string UpdateName { get; set; } // UpdateName (length: 255)

        ///<summary>
        /// Id cấp độ khách hàng
        ///</summary>
        public byte LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp độ khách hàng
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 300)

        ///<summary>
        /// Lần đầu  sử dụng dịch vụ
        ///</summary>
        public bool IsFirst { get; set; } // IsFirst

        public SourceService()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
