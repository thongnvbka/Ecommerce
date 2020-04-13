using System;

namespace Library.DbContext.Entities
{
    // Title
    
    public partial class Title
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã chức vụ
        ///</summary>
        public string Code { get; set; } // Code (length: 50)

        ///<summary>
        /// Tên chức vụ
        ///</summary>
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 600)

        ///<summary>
        /// Tên viết tắt của chức vụ
        ///</summary>
        public string ShortName { get; set; } // ShortName (length: 50)

        ///<summary>
        /// 0: Mới tạo, 1 Đang sử dụng, 2 cũ
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Mô tả về chức vụ
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Cập nhật gần nhất
        ///</summary>
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// UserId cập nhật gần nhất
        ///</summary>
        public long LastUpdateUserId { get; set; } // LastUpdateUserId

        ///<summary>
        /// Đơn vị đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Số lượng phòng ban đang có chức vụ này
        ///</summary>
        public int OfficeNo { get; set; } // OfficeNo

        public Title()
        {
            Status = 1;
            IsDelete = false;
            OfficeNo = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
