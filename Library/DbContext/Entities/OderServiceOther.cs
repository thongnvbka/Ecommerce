using System;

namespace Library.DbContext.Entities
{
    // AccountantSubject

    public partial class OrderServiceOther
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Giá trị là tiền tệ hay %
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// Thành tiền VND
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Loại dịch vụ phát sinh: 0: Phí vận chuyển, 1: Phí xe nâng, 2: Phí điều chuyển nội bộ
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Id phiếu nhập kho hoặc Id của phiếu Putaway hoặc Id của bao hàng
        ///</summary>
        public int ObjectId { get; set; } // ObjectId

        ///<summary>
        /// Loại objectId: 0: Id phiếu nhập kho, 1: Id phiếu Putaway, 2: Id: Bao hàng
        ///</summary>
        public byte Type { get; set; } // Type
        public string Note { get; set; } // Note
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Nhân viên tạo phiếu
        ///</summary>
        public int CreatedUserId { get; set; } // CreatedUserId

        ///<summary>
        /// Tên nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserFullName { get; set; } // CreatedUserFullName (length: 300)

        ///<summary>
        /// UserName của nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserUserName { get; set; } // CreatedUserUserName (length: 50)

        ///<summary>
        /// Chức vụ của nhân viên tạo phiếu
        ///</summary>
        public int CreatedUserTitleId { get; set; } // CreatedUserTitleId

        ///<summary>
        /// Tên chức vụ của nhân viên tạo phiếu
        ///</summary>
        public string CreatedUserTitleName { get; set; } // CreatedUserTitleName (length: 300)

        ///<summary>
        /// Phòng ban nhân viên tạo phiếu
        ///</summary>
        public int CreatedOfficeId { get; set; } // CreatedOfficeId

        ///<summary>
        /// Tên phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string CreatedOfficeName { get; set; } // CreatedOfficeName (length: 300)

        ///<summary>
        /// IdPath phòng ban của nhân viên tạo phiếu
        ///</summary>
        public string CreatedOfficeIdPath { get; set; } // CreatedOfficeIdPath (length: 500)

        public int PackageNo { get; set; }

        public decimal? TotalWeightActual { get; set; }

        public string PackageCodes { get; set; }

        public string DataJson { get; set; }

        public string UnsignText { get; set; }

        public OrderServiceOther()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
