using System;

namespace Library.DbContext.Entities
{
    // DispatcherDetail

    public partial class DispatcherDetail
    {
        public int Id { get; set; } // Id (Primary key)
        public int DispatcherId { get; set; } // DispatcherId
        public string DispatcherCode { get; set; } // DispatcherCode (length: 50)
        public int? FromDispatcherId { get; set; } // FromDispatcherId
        public string FromDispatcherCode { get; set; } // FromDispatcherCode (length: 50)
        public int? ToDispatcherId { get; set; } // ToDispatcherId
        public string ToDispatcherCode { get; set; } // ToDispatcherCode (length: 50)
        public int TransportPartnerId { get; set; } // TransportPartnerId
        public string TransportPartnerName { get; set; } // TransportPartnerName (length: 300)
        public int TransportMethodId { get; set; } // TransportMethodId
        public string TransportMethodName { get; set; } // TransportMethodName (length: 300)

        ///<summary>
        /// Đích đến điểm trung chuyển
        ///</summary>
        public int? EntrepotId { get; set; } // EntrepotId

        ///<summary>
        /// Tên đích đến điểm trung  chuyển
        ///</summary>
        public string EntrepotName { get; set; } // EntrepotName (length: 300)
        public int WalletId { get; set; } // WalletId
        public string WalletCode { get; set; } // WalletCode (length: 50)

        ///<summary>
        /// 0: Chờ nhận hàng, 1: Đã nhận hàng, 2: Hoàn thành
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Trị giá tiền của bao hàng
        ///</summary>
        public decimal? Amount { get; set; } // Amount

        ///<summary>
        /// Tổng cân nặng hàng hóa trong bao hàng
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Tổng cân nặng thực tế
        ///</summary>
        public decimal? WeightActual { get; set; } // WeightActual

        ///<summary>
        /// Tổng cân nặng quy đổi
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted

        ///<summary>
        /// Thể tích của bao hàng
        ///</summary>
        public decimal? Volume { get; set; } // Volume

        ///<summary>
        /// Thể tích m3 tính tiền của kiện hàng
        ///</summary>
        public decimal? Value { get; set; } // Value

        ///<summary>
        /// Tổng số kiện hàng nằm trong bao
        ///</summary>
        public int PackageNo { get; set; } // PackageNo
        public string Size { get; set; } // Size (length: 300)
        public string Description { get; set; } // Description (length: 500)
        public string Note { get; set; } // Note (length: 500)
        public int? FromTransportPartnerId { get; set; } // FromTransportPartnerId
        public string FromTransportPartnerName { get; set; } // FromTransportPartnerName (length: 300)
        public int? FromTransportMethodId { get; set; } // FromTransportMethodId
        public string FromTransportMethodName { get; set; } // FromTransportMethodName (length: 300)

        ///<summary>
        /// Đích đến điểm trung chuyển
        ///</summary>
        public int? FromEntrepotId { get; set; } // FromEntrepotId

        ///<summary>
        /// Tên đích đến điểm trung  chuyển
        ///</summary>
        public string FromEntrepotName { get; set; } // FromEntrepotName (length: 300)
        public int? ToTransportPartnerId { get; set; } // ToTransportPartnerId
        public string ToTransportPartnerName { get; set; } // ToTransportPartnerName (length: 300)
        public DateTime? ToTransportPartnerTime { get; set; } // ToTransportPartnerTime
        public int? ToTransportMethodId { get; set; } // ToTransportMethodId
        public string ToTransportMethodName { get; set; } // ToTransportMethodName (length: 300)

        ///<summary>
        /// Đích đến điểm trung chuyển
        ///</summary>
        public int? ToEntrepotId { get; set; } // ToEntrepotId

        ///<summary>
        /// Tên đích đến điểm trung  chuyển
        ///</summary>
        public string ToEntrepotName { get; set; } // ToEntrepotName (length: 300)
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        public DispatcherDetail()
        {
            TransportPartnerName = "";
            TransportMethodId = 0;
            TransportMethodName = "";
            Status = 1;
            PackageNo = 0;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
