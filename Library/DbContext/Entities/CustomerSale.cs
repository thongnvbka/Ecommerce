using System;

namespace Library.DbContext.Entities
{

    // CustomerSale
    
    public partial class CustomerSale
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Đường dẫn ảnh
        ///</summary>
        public string ImagePath { get; set; } // ImagePath (length: 600)

        ///<summary>
        /// Mã tài khoản
        ///</summary>
        public string CardId { get; set; } // CardId (length: 50)

        ///<summary>
        /// Tên tài khoản
        ///</summary>
        public string CardNumber { get; set; } // CardNumber (length: 300)

        ///<summary>
        /// Ưu đãi "Phí mua hàng"
        ///</summary>
        public int SaleShoping { get; set; } // SaleShoping

        ///<summary>
        /// Ưu đãi "Phí vận chuyển"
        ///</summary>
        public int SaleShiping { get; set; } // SaleShiping

        ///<summary>
        /// Trạng thái
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Có xóa hay không
        ///</summary>
        public bool? IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Mã hệ thống
        ///</summary>
        public int? SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Cấp độ tài khoản
        ///</summary>
        public int? LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp độ
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 255)

        ///<summary>
        /// Mã nhân viên tạo
        ///</summary>
        public int? UserCreateId { get; set; } // UserCreateId

        ///<summary>
        /// Tên nhân viên tạo
        ///</summary>
        public string UserCreateName { get; set; } // UserCreateName (length: 255)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật gần nhất
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Mã nhân viên cập nhật gần nhất
        ///</summary>
        public int? UserUpdateId { get; set; } // UserUpdateId

        ///<summary>
        /// Tên nhân viên cập nhật gần nhất
        ///</summary>
        public string UserUpdateName { get; set; } // UserUpdateName (length: 255)

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)

        public CustomerSale()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
