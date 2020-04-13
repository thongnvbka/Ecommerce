namespace Library.ViewModels.VipLevel
{
    public class VipLevelViewModel
    {
        /// <summary>
        /// % Triết khấu phí mua hàng hộ
        /// </summary>
        public decimal Order { get; set; }

        /// <summary>
        /// % Triết khấu phí vận chuyển TQ-VN
        /// </summary>
        public decimal Ship { get; set; }

        /// <summary>
        /// % Phí đặt cọc tối thiếu theo giá trị tiền hàng
        /// </summary>
        public decimal Deposit { get; set; }
    }
    public class VipCustomerLevel
    {
        /// <summary>
        /// Level Id
        /// </summary>
        public decimal LevelId { get; set; }

        /// <summary>
        /// Level Name
        /// </summary>
        public decimal LevelName { get; set; }
        
    }
}
