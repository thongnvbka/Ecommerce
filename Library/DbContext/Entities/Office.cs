using System;

namespace Library.DbContext.Entities
{
    // Office
    
    public partial class Office
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn vị
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Tên đơn vị
        ///</summary>
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 600)

        ///<summary>
        /// Tên viết tắt của đơn vị
        ///</summary>
        public string ShortName { get; set; } // ShortName (length: 50)

        ///<summary>
        /// IdPath đơn vị
        ///</summary>
        public string IdPath { get; set; } // IdPath (length: 400)

        ///<summary>
        /// Path Name của đơn vị theo đươn vị cha (....Name Parent 1/Name Parent 2 / My Name)
        ///</summary>
        public string NamePath { get; set; } // NamePath (length: 2000)

        ///<summary>
        /// 0: Mới tạo, 1 Đang sử dụng, 2 Đơn vị cũ
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Loại đơn vị: 0: Bình thường, 1: Kho, 2: Nhân sự, 3: Kế toán, 4 Ban giám đốc
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Mã đơn vị cha
        ///</summary>
        public int? ParentId { get; set; } // ParentId

        ///<summary>
        /// Tên đươn vị cha
        ///</summary>
        public string ParentName { get; set; } // ParentName (length: 300)

        ///<summary>
        /// Mô tả về đơn vị
        ///</summary>
        public string Description { get; set; } // Description (length: 500)
        public string Address { get; set; } // Description (length: 500)

        ///<summary>
        /// Số nhân viên trong đơn vị
        ///</summary>
        public int UserNo { get; set; } // UserNo

        ///<summary>
        /// Số lượng chức vụ trong đơn vị này
        ///</summary>
        public int TitleNo { get; set; } // TitleNo

        ///<summary>
        /// Số đơn vị bên dưới
        ///</summary>
        public int ChildNo { get; set; } // ChildNo
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public int LastUpdateUserId { get; set; } // LastUpdateUserId
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public  string Culture { get; set; }

        ///<summary>
        /// Đơn vị đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        public Office()
        {
            Status = 1;
            Type = 0;
            TitleNo = 0;
            ChildNo = 0;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
