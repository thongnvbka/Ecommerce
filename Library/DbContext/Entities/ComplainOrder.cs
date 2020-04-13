using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Entities
{
    public partial class ComplainOrder
    {
        public long Id { get; set; } // Id (Primary key)
        public long ComplainId { get; set; } // ComplainId (Primary key)
        public int OrderDetailId { get; set; } // OrderDetailId (Primary key)
        public string Note { get; set; } // Note (length: 2000)
        public DateTime? CreateDate { get; set; } // CreateDate
        public int? LinkOrder { get; set; } // LinkOrder (Primary key)
        public ComplainOrder()
        {
            CreateDate = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
