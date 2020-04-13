using System;

namespace Library.DbContext.Entities
{

    // HistoryPackage

    public partial class HistoryPackage
    {
        public long Id { get; set; } // Id (Primary key)
        public int? OrderId { get; set; } // OrderId
        public int? OrderPackage { get; set; } // OrderPackage
        public DateTime? CreateDate { get; set; } // CreateDate
        public DateTime? UpdateDate { get; set; } // UpdateDate
        public string Note { get; set; } // Note (length: 500)

        public HistoryPackage()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
