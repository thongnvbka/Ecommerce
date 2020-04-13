using System;

namespace Library.DbContext.Results
{
    public class UserOfficeResult
    {
        public int Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string FullName { get; set; } // FullName (length: 100)
        public string Email { get; set; } // Email (length: 50)
        public string Phone { get; set; }
        public string Avatar { get; set; } // Avatar (length: 2000)

        public int TitleId { get; set; } // TitleId (Primary key)
        public int OfficeId { get; set; } // OfficeId (Primary key)
        public string TitleName { get; set; } // TitleName (length: 300)
        public string OfficeName { get; set; } // OfficeName (length: 300)
        public byte Type { get; set; } // Type
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 500)
        public string OfficeNamePath { get; set; } // OfficeNamePath (length: 2000)
        public short? LevelId { get; set; } // LevelId
        public string LevelName { get; set; } // LevelName (length: 300)
        public DateTime? Birthday { get; set; } // Birthday
        public int? TypeId { get; set; } // TypeId

        //Tên đối tượng người dùng trên  hệ thống
        
        public string TypeName { get; set; } // TypeName

        public DateTime? StartDate { get; set; } // StartDate
        public DateTime Created { get; set; } // Created
        public byte Gender { get; set; } // Gender
        public string Websites { get; set; }

        public string UserFullNameOffice
        {
            get { return "(" + UserName + ")" + " - " + FullName + " - " + TitleName + " - " + OfficeName; }
        }
    }
}