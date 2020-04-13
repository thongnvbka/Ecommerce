using System;

namespace Library.DbContext.Results
{
    public class PutAwayDetailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int PutAwayId { get; set; } // PutAwayId
        public string PutAwayCode { get; set; } // PutAwayCode (length: 30)
        public int PackageId { get; set; } // PackageId
        public int OrderId { get; set; } // OrderId
        public byte OrderType { get; set; } // OrderType
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public int OrderPackageNo { get; set; } // OrderPackageNo
        public string TransportCode { get; set; } // TransportCode (length: 50)
        public string PackageCode { get; set; } // TransportCode (length: 50)
        public string Note { get; set; } // Note (length: 500)
        public byte Status { get; set; } // Status
        public decimal Length { get; set; } // Length
        public decimal Weight { get; set; } // Weight
        public decimal Width { get; set; } // Width
        public decimal Height { get; set; } // Height
        public decimal ConvertedWeight { get; set; } // ConvertedWeight
        public decimal ActualWeight { get; set; } // ActualWeight
        public int? LayoutId { get; set; }
        public string LayoutName { get; set; }
        public string LayoutIdPath { get; set; }
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerUserName { get; set; }

        public PutAwayDetailResult()
        {
            OrderPackageNo = 0;
            Status = 1;
        }
    }
}
