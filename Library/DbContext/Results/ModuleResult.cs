using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Results
{
    public class ModuleResult
    {
        ///<summary>
        /// Bảng lưu các module trong App của hệ thống Erp
        ///</summary>
        public short Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string Icon { get; set; } // Icon (length: 50)
        public byte AppId { get; set; } // AppId
        public string Description { get; set; } // Description (length: 500)
        public DateTime Created { get; set; } // Created
        public int OrderNo { get; set; } // OrderNo
        public short? ParentId { get; set; } // ParentId
        public string ParentName { get; set; } // ParentName (length: 300)
        public byte Level { get; set; } // Level
    }
}
