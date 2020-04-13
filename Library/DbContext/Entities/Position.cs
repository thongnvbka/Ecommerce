using System;

namespace Library.DbContext.Entities
{
    // Position
    
    public partial class Position
    {
        public int OfficeId { get; set; } // OfficeId (Primary key)
        public string OfficeName { get; set; } // OfficeName (length: 300)
        public int TitleId { get; set; } // TitleId (Primary key)
        public string TitleName { get; set; } // TitleName (length: 300)
        public DateTime Created { get; set; }
        public Position()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
