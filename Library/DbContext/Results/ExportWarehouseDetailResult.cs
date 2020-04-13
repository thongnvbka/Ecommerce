using System;

namespace Library.DbContext.Results
{
    public class ExportWarehouseDetailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int ExportWarehouseId { get; set; } // ExportWarehouseId
        public string ExportWarehouseCode { get; set; } // ExportWarehouseCode (length: 50)
        public int PackageId { get; set; } // PackageId
        public string PackageCode { get; set; } // PackageCode (length: 50)
        public decimal PackageWeight { get; set; } // PackageWeight
        public decimal PackageWeightConverted { get; set; } // PackageWeightConverted
        public decimal PackageWeightActual { get; set; } // PackageWeightActual
        public string PackageTransportCode { get; set; } // PackageTransportCode (length: 50)
        public string Note { get; set; } // Note (length: 500)
        public string PackageSize { get; set; } // PackageSize (length: 500)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public decimal OrderWeight { get; set; } // OrderWeight
        public decimal OrderWeightConverted { get; set; } // OrderWeightConverted
        public decimal OrderWeightActual { get; set; } // OrderWeightActual
        public decimal OrderShip { get; set; } // OrderShip
        public decimal OrderShipActual { get; set; } // OrderShipActual
        public int OrderPackageNo { get; set; } // OrderPackageNo
        public int OrderTotalPackageNo { get; set; } // OrderTotalPackageNo
        public string OrderNote { get; set; } // OrderNote
        public int CustomerId { get; set; } // CustomerId
        public string CustomerUserName { get; set; } // CustomerUserName (length: 300)
        public string CustomerFullName { get; set; } // CustomerFullName (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)
        public int CustomerOrderNo { get; set; } // CustomerOrderNo
        public decimal CustomerDistance { get; set; } // CustomerDistance
        public decimal CustomerWeight { get; set; } // CustomerWeight
        public decimal CustomerWeightConverted { get; set; } // CustomerWeightConverted
        public decimal CustomerWeightActual { get; set; } // CustomerWeightActual
        public byte Status { get; set; } // IsDelete
        public decimal Total { get; set; }
        public decimal TotalProductPrice { get; set; }
        public decimal PayedPrice { get; set; }
        public decimal TotalServicePrice { get; set; }
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated
    }
}
