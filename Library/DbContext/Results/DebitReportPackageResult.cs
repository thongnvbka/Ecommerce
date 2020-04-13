using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DbContext.Results
{
    public class DebitReportPackageResult
    {
        public long Id { get; set; } // Id (Primary key)
        public int? PackageId { get; set; } // PackageId
        public string PackageCode { get; set; } // PackageCode (length: 50)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte ServiceId { get; set; } // ServiceId
        public decimal Price { get; set; } // Price
        public int CustomerId { get; set; } // CustomerId
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Trạng thái kiện hàng: 0: Chờ nhập kho, 1: Đang trong kho, 2: Đang điều chuyển, 3: Mất mã, 4: Hoàn thành, 5: Mất hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Mã vận đơn của kiện hàng
        ///</summary>
        public string TransportCode { get; set; } // TransportCode (length: 50)

        ///<summary>
        /// Cân nặng kiện hàng
        ///</summary>
        public decimal? Weight { get; set; } // Weight

        ///<summary>
        /// Cân nặng chuyển đổi
        ///</summary>
        public decimal? WeightConverted { get; set; } // WeightConverted

        public decimal? WeightActual { get; set; } // WeightActual
    }
}
