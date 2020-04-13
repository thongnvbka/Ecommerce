namespace Library.DbContext.Results
{
    public class UserSuggetionResult
    {
        public int Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string FullName { get; set; } // FullName (length: 100)
        public byte Gender { get; set; } // Gender
        public string Email { get; set; } // Email (length: 50)
        public byte Status { get; set; } // Status
        public string Avatar { get; set; } // Avatar (length: 2000)
        public int TitleId { get; set; } // TitleId (Primary key)
        public int OfficeId { get; set; } // OfficeId (Primary key)
        public string TitleName { get; set; } // TitleName (length: 300)
        public string OfficeName { get; set; } // OfficeName (length: 300)
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 500)
        public short? LevelId { get; set; } // LevelId
        public string LevelName { get; set; } // LevelName (length: 300)
    }
}