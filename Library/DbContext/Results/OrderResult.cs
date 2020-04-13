namespace Library.DbContext.Results
{
    public class OrderResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte Type { get; set; } // Type

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

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status
    }
}
