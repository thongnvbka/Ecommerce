using System;

namespace Library.DbContext.Results
{
    public class UserResult
    {
        public int Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string Password { get; set; } // Password (length: 50)
        public string FirstName { get; set; } // FirstName (length: 30)
        public string MidleName { get; set; } // MidleName (length: 30)
        public string LastName { get; set; } // LastName (length: 30)
        public string FullName { get; set; } // FullName (length: 100)
        public byte Gender { get; set; } // Gender
        public string Email { get; set; } // Email (length: 50)
        public string Description { get; set; } // Description (length: 500)
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public int LastUpdateUserId { get; set; } // LastUpdateUserId
        public bool IsDelete { get; set; } // IsDelete
        public byte Status { get; set; } // Status
        public DateTime? Birthday { get; set; } // Birthday
        public DateTime? StartDate { get; set; } // StartDate
        public string Avatar { get; set; } // Avatar (length: 2000)

        public int TitleId { get; set; } // TitleId (Primary key)
        public int OfficeId { get; set; } // OfficeId (Primary key)
        public string TitleName { get; set; } // TitleName (length: 300)
        public string OfficeName { get; set; } // OfficeName (length: 300)
        public bool IsDefault { get; set; } // IsDefault
        public byte Type { get; set; } // Type
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 500)
        public string OfficeNamePath { get; set; } // OfficeNamePath (length: 2000)
        public int? DirectUserId { get; set; } // DirectUserId
        public string DirectFullName { get; set; } // DirectFullName (length: 100)
        public int? DirectTitleId { get; set; } // DirectTitleId
        public string DirectTitleName { get; set; } // DirectTitleName (length: 300)
        public int? DirectOfficeId { get; set; } // DirectOfficeId
        public string DirectOfficeName { get; set; } // DirectOfficeName (length: 300)
        public int? ApprovalUserId { get; set; } // ApprovalUserId
        public string ApprovalFullName { get; set; } // ApprovalFullName (length: 100)
        public int? ApprovalTitleId { get; set; } // ApprovalTitleId
        public string ApprovalTitleName { get; set; } // ApprovalTitleName (length: 300)
        public int? ApprovalOfficeId { get; set; } // ApprovalOfficeId
        public string ApprovalOfficeName { get; set; } // ApprovalOfficeName (length: 300)
        public short? LevelId { get; set; } // LevelId
        public string LevelName { get; set; } // LevelName (length: 300)
        public int? GroupPermisionId { get; set; } // GroupPermisionId
        public string GroupPermissionName { get; set; } // GroupPermissionName (length: 300)
    }
}
