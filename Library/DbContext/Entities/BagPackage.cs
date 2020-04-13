using System;

namespace Library.DbContext.Entities
{
    // BagPackage
    
    public partial class BagPackage
    {
        public int Id { get; set; } // Id (Primary key)
        public int BagId { get; set; } // BagId
        public int PackageId { get; set; } // PackageId
        public DateTime Created { get; set; } // Created

        public BagPackage()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
