namespace Library.DbContext.Entities
{
    // UserPosition
    
    public partial class UserPosition
    {
        public int Id { get; set; } // Id (Primary key)
        public int UserId { get; set; } // UserId
        public int TitleId { get; set; } // TitleId
        public int OfficeId { get; set; } // OfficeId
        public string TitleName { get; set; } // TitleName (length: 300)
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// 1: Vị trí chính, 0: Vị trí kiêm nhiệm
        ///</summary>
        public bool IsDefault { get; set; } // IsDefault

        ///<summary>
        /// 0: Nhân viên, 1: Trưởng đơn vị, 2: Phó đơn vị
        ///</summary>
        public byte Type { get; set; } // Type
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 500)
        public string OfficeNamePath { get; set; } // OfficeNamePath (length: 2000)

        ///<summary>
        /// Quản lý trực tiếp UserId
        ///</summary>
        public int? DirectUserId { get; set; } // DirectUserId

        ///<summary>
        /// Quản lý trực tiếp Tên đầy đủ
        ///</summary>
        public string DirectFullName { get; set; } // DirectFullName (length: 100)

        ///<summary>
        /// Quản lý trực tiếp Id chức vụ
        ///</summary>
        public int? DirectTitleId { get; set; } // DirectTitleId
        public string DirectTitleName { get; set; } // DirectTitleName (length: 300)
        public int? DirectOfficeId { get; set; } // DirectOfficeId
        public string DirectOfficeName { get; set; } // DirectOfficeName (length: 300)

        ///<summary>
        /// Quản lý phê duyệt
        ///</summary>
        public int? ApprovalUserId { get; set; } // ApprovalUserId
        public string ApprovalFullName { get; set; } // ApprovalFullName (length: 100)
        public int? ApprovalTitleId { get; set; } // ApprovalTitleId
        public string ApprovalTitleName { get; set; } // ApprovalTitleName (length: 300)
        public int? ApprovalOfficeId { get; set; } // ApprovalOfficeId
        public string ApprovalOfficeName { get; set; } // ApprovalOfficeName (length: 300)

        ///<summary>
        /// Mã cấp bậc
        ///</summary>
        public short? LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp bậc
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 300)
        public short? GroupPermisionId { get; set; } // GroupPermisionId
        public string GroupPermissionName { get; set; } // GroupPermissionName (length: 300)

        public UserPosition()
        {
            IsDefault = true;
            Type = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
