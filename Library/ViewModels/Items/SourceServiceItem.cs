namespace Library.ViewModels.Items
{
    public class SourceServiceItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên gói dịch vụ tìm nguồn
        ///</summary>
        public string Name { get; set; } // Name (length: 255)

        ///<summary>
        /// Mô tả gói dịch vụ
        ///</summary>
        public string Description { get; set; } // Description
        ///<summary>
        /// Phí duy trì trên tháng
        ///</summary>
        public decimal Price { get; set; } // Price
        ///<summary>
        /// Lần đầu  sử dụng dịch vụ
        ///</summary>
        public bool IsFirst { get; set; } // IsFirst
    }
}
