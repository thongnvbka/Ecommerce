using System;

namespace Library.ViewModels.Report
{
    public class ReportBusinessItem
    {
        public int OrderId { get; set; } // OrderProductNo

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte OrderType { get; set; } // OrderType

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int OrderProductNo { get; set; } // OrderProductNo

        ///<summary>
        /// Số lượng kiện hàng
        ///</summary>
        public int OrderPackageNo { get; set; } // OrderPackageNo

        ///<summary>
        /// Tổng số cân nặng của kiện hàng
        ///</summary>
        public decimal OrderTotalWeight { get; set; } // OrderTotalWeight

        ///<summary>
        /// Tổng tiền VND
        ///</summary>
        public decimal OrderTotalExchange { get; set; } // OrderTotalExchange

        ///<summary>
        /// Tổng tiền ngoại tệ
        ///</summary>
        public decimal OrderTotalPrice { get; set; } // OrderTotalPrice

        ///<summary>
        /// Tổng giá trị tiền + tiền dịch vụ và sau khi giảm giá VND
        ///</summary>
        public decimal OrderTotal { get; set; } // OrderTotal

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte OrderStatus { get; set; } // Status

        ///<summary>
        /// Loại dịch vụ: 0: Gói kinh doanh, 1: Gói tiêu dùng
        ///</summary>
        public byte OrderServiceType { get; set; } // ServiceType

        ///<summary>
        /// Ngày chốt đơn
        ///</summary>
        public DateTime OrderFinishDate { get; set; } // OrderFinishDate

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên xử lý đặt hàng
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

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
        /// Phí dịch vụ mua hàng
        ///</summary>
        public decimal ServicePurchase { get; set; } // ServicePurchase

        ///<summary>
        /// Phí dịch vụ kiểm đếm
        ///</summary>
        public decimal ServiceTally { get; set; } // ServiceTally

        ///<summary>
        /// Phí dịch vụ đóng kiện
        ///</summary>
        public decimal ServiceBaled { get; set; } // ServiceBaled

        ///<summary>
        /// Tổng tiền mặc cả đơn hàng
        ///</summary>
        public decimal OrderBargain { get; set; } // OrderBargain

        ///<summary>
        /// Id nhân viên kinh doanh phụ trách
        ///</summary>
        public int? EmployeeId { get; set; } // EmployeeId

        ///<summary>
        /// Tên nhân viên kinh doanh phụ trách
        ///</summary>
        public string EmployeeFullName { get; set; } // EmployeeFullName (length: 150)

        ///<summary>
        /// Mã nhân viên kinh doanh phụ trách
        ///</summary>
        public string EmployeeCode { get; set; } // EmployeeCode (length: 150)

        ///<summary>
        /// Phí cân nặng = phí vận chuyển về Việt Nam
        ///</summary>
        public decimal ServiceShip { get; set; } // ServiceShip

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Giá duyệt
        ///</summary>
        public decimal ApprovelPrice { get; set; } // ApprovelPrice
    }
}