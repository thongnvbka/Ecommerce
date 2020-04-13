using System;

namespace Library.DbContext.Entities
{
    // DeliverySpend
    
    public partial class DeliverySpend
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã phiếu vận chuyển
        ///</summary>
        public string DeliveryCode { get; set; } // DeliveryCode (length: 50)

        ///<summary>
        /// Id phiếu vận chuyển
        ///</summary>
        public int DeliveryId { get; set; } // DeliveryId

        ///<summary>
        /// Tên khoản chi
        ///</summary>
        public string SpendName { get; set; } // SpendName (length: 300)

        ///<summary>
        /// Id khoản chi
        ///</summary>
        public byte SpendId { get; set; } // SpendId

        ///<summary>
        /// Giá trị khoản chi
        ///</summary>
        public decimal Value { get; set; } // Value

        ///<summary>
        /// 0: Chi ra, 1: Thu vào
        ///</summary>
        public byte Mode { get; set; } // Mode
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        public DeliverySpend()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
