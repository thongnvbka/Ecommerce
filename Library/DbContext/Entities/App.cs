using System;

namespace Library.DbContext.Entities
{
    // App
    public partial class App
    {
        ///<summary>
        /// Bảng lưu các App trong hệ thống Erp (Hrm, FinSave, FinShip,...)
        ///</summary>
        public byte Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 500)
        public string Icon { get; set; } // Icon (length: 2000)
        public string Description { get; set; } // Description (length: 500)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created
        public int OrderNo { get; set; } // OrderNo
        public string Url { get; set; } // Url (length: 300)

        public App()
        {
            IsDelete = false;
            OrderNo = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
