using System;

namespace Library.DbContext.Entities
{
    // Warehouse
    
    public partial class Warehouse
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã kho
        ///</summary>
        public string Code { get; set; } // Code (length: 50)

        ///<summary>
        /// Tên kho
        ///</summary>
        public string Name { get; set; } // Name (length: 300)

        ///<summary>
        /// Địa chỉ kho
        ///</summary>
        public string Address { get; set; } // Address (length: 600)

        ///<summary>
        /// Trạng thái: Sử dụng, Tạm nghưng, Đóng cửa,..
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Thời gian tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật lần cuối
        ///</summary>
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Mô tả
        ///</summary>
        public string Description { get; set; } // Description (length: 600)
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

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

        public string Country { get; set; }
        public string Phone { get; set; }

        public Warehouse()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
