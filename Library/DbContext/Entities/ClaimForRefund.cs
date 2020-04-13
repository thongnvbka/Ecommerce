using System;

namespace Library.DbContext.Entities
{
    // ClaimForRefund
    public partial class ClaimForRefund
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã code phiếu yêu cầu hoàn tiền khách hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Id đơn hàng cần xử lý hoàn bồi
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã đơn hàng cần xử lý hoàn bồi
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 30)

        ///<summary>
        /// Loại đơn hàng cần xử lý hoàn bồi
        ///</summary>
        public int? OrderType { get; set; } // OrderType

        ///<summary>
        /// Trạng thái yêu cầu hoàn tiền (0: Chờ xử lý, 1: Hoàn tất, 2: Hủy hoàn bồi)
        ///</summary>
        public int Status { get; set; } // Status
        public int? TicketId { get; set; } // TicketId
        public string TicketCode { get; set; } // TicketCode (length: 30)
        public DateTime? TicketCreated { get; set; } // TicketCreated

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Code khách hàng
        ///</summary>
        public string CustomerCode { get; set; } // CustomerCode (length: 20)

        ///<summary>
        /// Họ tên khách hàng
        ///</summary>
        public string CustomerFullName { get; set; } // CustomerFullName (length: 200)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 100)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Địa chỉ khách hàng
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)
        public int? CustomerOfficeId { get; set; } // CustomerOfficeId
        public string CustomerOfficeName { get; set; } // CustomerOfficeName (length: 300)
        public string CustomerOfficePath { get; set; } // CustomerOfficePath (length: 300)

        ///<summary>
        /// Id nhân viên đặt hàng
        ///</summary>
        public int? OrderUserId { get; set; } // OrderUserId

        ///<summary>
        /// Mã nhân viên đặt hàng
        ///</summary>
        public string OrderUserCode { get; set; } // OrderUserCode (length: 30)

        ///<summary>
        /// Họ tên nhân viên đặt hàng
        ///</summary>
        public string OrderUserFullName { get; set; } // OrderUserFullName (length: 200)

        ///<summary>
        /// Email nhân viên đặt hàng
        ///</summary>
        public string OrderUserEmail { get; set; } // OrderUserEmail (length: 300)

        ///<summary>
        /// Điện thoại nhân viên đặt hàng
        ///</summary>
        public string OrderUserPhone { get; set; } // OrderUserPhone (length: 100)

        ///<summary>
        /// Id phòng nhân viên đặt hàng
        ///</summary>
        public int? OrderUserOfficeId { get; set; } // OrderUserOfficeId

        ///<summary>
        /// Tên phòng ban của nhân viên đặt hàng
        ///</summary>
        public string OrderUserOfficeName { get; set; } // OrderUserOfficeName (length: 300)

        ///<summary>
        /// Path phòng ban của nhân viên đặt hàng
        ///</summary>
        public string OrderUserOfficePath { get; set; } // OrderUserOfficePath (length: 300)

        ///<summary>
        /// Id nhân viên CSKH
        ///</summary>
        public int? SupportId { get; set; } // SupportId

        ///<summary>
        /// Mã Code nhân viên CSKH
        ///</summary>
        public string SupportCode { get; set; } // SupportCode (length: 30)

        ///<summary>
        /// Họ tên nhân viên CSKH
        ///</summary>
        public string SupportFullName { get; set; } // SupportFullName (length: 200)

        ///<summary>
        /// Email nhân viên CSKH
        ///</summary>
        public string SupportEmail { get; set; } // SupportEmail (length: 300)
        public int? AccountantId { get; set; } // AccountantId
        public string AccountantCode { get; set; } // AccountantCode (length: 30)
        public string AccountantFullName { get; set; } // AccountantFullName (length: 200)
        public string AccountantEmail { get; set; } // AccountantEmail (length: 300)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Id nhân viên tạo phiếu
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Mã nhân viên tạo phiếu
        ///</summary>
        public string UserCode { get; set; } // UserCode (length: 20)

        ///<summary>
        /// Tên nhân viên tạo phiếu
        ///</summary>
        public string UserName { get; set; } // UserName (length: 200)

        ///<summary>
        /// Email nhân viên tạo phiếu
        ///</summary>
        public string UserEmail { get; set; } // UserEmail (length: 300)

        ///<summary>
        /// Điện thoại nhân viên tạo phiếu
        ///</summary>
        public string UserPhone { get; set; } // UserPhone (length: 100)

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        ///<summary>
        /// 0: chưa xóa. 1: đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày tạo yêu cầu hoàn tiền
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Ngày cập nhật yêu cầu hoàn tiền
        ///</summary>
        public DateTime LastUpdated { get; set; } // LastUpdated

        ///<summary>
        /// Tổng tiền dự kiến trả cho khách
        ///</summary>
        public decimal? MoneyRefund { get; set; } // MoneyRefund

        ///<summary>
        /// Tổng tiền thực tế trả khách
        ///</summary>
        public decimal? RealTotalRefund { get; set; } // RealTotalRefund

        ///<summary>
        /// Số tiền thực tế đặt hàng đòi lại được từ Shop
        ///</summary>
        public decimal? MoneyOrderRefund { get; set; } // MoneyOrderRefund
        public decimal? MoneyOrderRefundDicker { get; set; } // MoneyOrderRefundDicker

        ///<summary>
        /// Số tiền CSKH yêu cầu trả khách
        ///</summary>
        public decimal? SupporterMoneyRequest { get; set; } // SupporterMoneyRequest

        ///<summary>
        /// Số tiền giảm trừ trả khách
        ///</summary>
        public decimal? CurrencyDiscount { get; set; } // CurrencyDiscount

        ///<summary>
        /// Ghi chú cho phiếu lỗi
        ///</summary>
        public string NoteOrderer { get; set; } // NoteOrderer
        public string NoteSupporter { get; set; } // NoteSupporter
        public string NoteAccountanter { get; set; } // NoteAccountanter

        ///<summary>
        /// Mã Người phê duyệt hoàn tiền khiếu nại
        ///</summary>
        public int? ApproverId { get; set; } // ApproverId

        ///<summary>
        /// Tên Người phê duyệt hoàn tiền khiếu nại
        ///</summary>
        public string ApproverName { get; set; } // ApproverName (length: 200)
        ///<summary>
        /// Số tiền khác
        ///</summary>
        public decimal? MoneyOther { get; set; } // MoneyOther

        ///<summary>
        /// Lý do hủy hoàn bồi
        ///</summary>
        public string ReasonCancel { get; set; } // ReasonCancel 

        public ClaimForRefund()
        {
            ExchangeRate = 0m;
            IsDelete = false;
            Created = DateTime.Now;
            LastUpdated = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
