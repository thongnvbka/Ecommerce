namespace Library.DbContext.Results
{
    public class OrderServiceResult
    {
        ///<summary>
        /// Id dịch vụ
        ///</summary>
        public int ServiceId { get; set; } // ServiceId

        ///<summary>
        /// Tên dịch vụ: Kiểm đếm, đóng kiện,...
        ///</summary>
        public string ServiceName { get; set; } // ServiceName (length: 300)
    }
}
