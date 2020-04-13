using System;

namespace Library.DbContext.Entities
{
    // AccountantSubject
    
    public partial class AccountantSubject
    {
        public int Id { get; set; } // Id (Primary key)
        public int? Idd { get; set; } // Idd
        public string SubjectName { get; set; } // SubjectName (length: 100)
        public string SubjectNote { get; set; } // SubjectNote
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdated { get; set; } // LastUpdated
        public bool IsIdSystem { get; set; } // IsIdSystem

        public AccountantSubject()
        {
            IsDelete = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            IsIdSystem = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
