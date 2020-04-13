using System;

namespace Library.DbContext.Entities
{
    // Page
    
    public partial class Page
    {

        ///<summary>
        /// Lưu các page được khai báo trong hệ thống
        ///</summary>
        public short Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 500)
        public string Description { get; set; } // Description (length: 500)
        public short ModuleId { get; set; } // ModuleId
        public string ModuleName { get; set; } // ModuleName (length: 300)
        public byte AppId { get; set; } // AppId
        public bool ShowInMenu { get; set; } // ShowInMenu
        public string AppName { get; set; } // AppName (length: 300)

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created
        public int OrderNo { get; set; } // OrderNo
        public bool IsDelete { get; set; } // IsDelete
        public string Url { get; set; } // Url (length: 300)
        public string Icon { get; set; } // Url (length: 300)

        public Page()
        {
            ShowInMenu = true;
            OrderNo = 0;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
