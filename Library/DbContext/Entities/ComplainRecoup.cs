using System;

namespace Library.DbContext.Entities
{
    // ComplainRecoup

    public partial class ComplainRecoup
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id khiếu nại
        ///</summary>
        public long ComplainId { get; set; } // ComplainId

        ///<summary>
        /// Mã khiếu nại
        ///</summary>
        public string ComplainCode { get; set; } // ComplainCode (length: 50)

        ///<summary>
        /// Lý do bồi thường
        ///</summary>
        public string RecoupCause { get; set; } // RecoupCause

        ///<summary>
        /// Phương án bồi thường
        ///</summary>
        public string RecoupSolution { get; set; } // RecoupSolution

        ///<summary>
        /// Tiền công ty bồi thường
        ///</summary>
        public decimal? CompanyMoney { get; set; } // CompanyMoney

        ///<summary>
        /// Tiền công ty mặc cả
        ///</summary>
        public decimal? CompanyBargain { get; set; } // CompanyBargain

        ///<summary>
        /// Tiền nhân viên bồi thường
        ///</summary>
        public decimal? EmployeeMoney { get; set; } // EmployeeMoney

        ///<summary>
        /// Tiền shop bồi thường
        ///</summary>
        public decimal? ShopMoney { get; set; } // ShopMoney

        ///<summary>
        /// 0: mất hàng do vận chuyển, 1: hàng vỡ méo hỏng do vận chuyển, 2: Hàng bị ngấm nước mưa, 3: hỗ trợ cước cân nặng do quy đổi, 4: hoàn tiền đóng kiện gỗ, 5: hoàn tiền kiểm đếm, 6: tiền cân nặng phải miễn phí cho khách không thu được, 7: hoàn tiền mặc cả, tiền hàng, 8: cty nhận hàng về để thanh lý, 9: nhầm do shop đóng chung kiện hàng, 10: hỗ trợ cho khách để giải quyết dứt điểm, 11: chuyển hàng nhầm kho, 12: Shop không uy tín gửi hàng kém chất lượng hoặc thiếu hàng, 13: hàng sai mẫu, hàng thiếu hỗ trợ để khách nhận hàng
        ///</summary>
        public byte TypeCause { get; set; } // TypeCause

        ///<summary>
        /// Lý do khiếu nại
        ///</summary>
        public byte? TypeTicket { get; set; } // TypeTicket

        public ComplainRecoup()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
