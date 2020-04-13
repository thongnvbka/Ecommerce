using System;

namespace Library.DbContext.Entities
{
    // GroupChat
    
    public partial class GroupChat
    {
        public string Id { get; set; } // Id (Primary key) (length: 100)
        public int CreatorId { get; set; } // CreatorId
        public DateTime CreatedOnDate { get; set; } // CreatedOnDate
        public string GroupName { get; set; } // GroupName (length: 500)
        public string Image { get; set; } // Image (length: 500)
        public string CreatorFullName { get; set; } // CreatorFullName (length: 150)
        public string CreatorTitleName { get; set; } // CreatorTitleName (length: 300)
        public string CreatorOfficeName { get; set; } // CreatorOfficeName (length: 300)
        public string UnsignName { get; set; } // UnsignName (length: 500)
        public bool IsSystem { get; set; } // IsSystem
        public int UserQuantity { get; set; } // UserQuantity
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// 0: Public 1: Protected 2: Private
        ///</summary>
        public byte Status { get; set; } // Status

        public GroupChat()
        {
            CreatedOnDate = DateTime.Now;
            IsSystem = false;
            UserQuantity = 1;
            IsDelete = false;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
