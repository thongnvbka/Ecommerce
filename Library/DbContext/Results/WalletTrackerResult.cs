using System;

namespace Library.DbContext.Results
{
    public class WalletTrackerResult
    {
        public int WalletId { get; set; } // Id (Primary key)
        public string WalletCode { get; set; } // Code (length: 20)
        public decimal? WalletWidth { get; set; } // Width
        public decimal? WalletLength { get; set; } // Length
        public decimal? WalletHeight { get; set; } // Height
        public string WalletSize { get; set; } // Size (length: 500)
        public decimal? WalletTotalWeight { get; set; } // TotalWeight
        public decimal? WalletTotalWeightConverted { get; set; } // TotalWeightConverted
        public decimal? WalletTotalWeightActual { get; set; } // TotalWeightActual
        public decimal? WalletTotalVolume { get; set; } // TotalVolume
        public decimal? WalletWeight { get; set; } // Weight
        public decimal? WalletWeightConverted { get; set; } // WeightConverted
        public decimal? WalletWeightActual { get; set; } // WeightActual
        public decimal? WalletVolume { get; set; } // Volume
        public decimal? WalletTotalValue { get; set; } // TotalValue
        public int WalletPackageNo { get; set; } // PackageNo
        public int WalletCreatedWarehouseId { get; set; } // CreatedWarehouseId
        public string WalletCreatedWarehouseIdPath { get; set; } // CreatedWarehouseIdPath (length: 300)
        public string WalletCreatedWarehouseName { get; set; } // CreatedWarehouseName (length: 300)
        public string WalletCreatedWarehouseAddress { get; set; } // CreatedWarehouseAddress (length: 500)
        public int? WalletCurrentWarehouseId { get; set; } // CurrentWarehouseId
        public string WalletCurrentWarehouseIdPath { get; set; } // CurrentWarehouseIdPath (length: 300)
        public string WalletCurrentWarehouseName { get; set; } // CurrentWarehouseName (length: 300)
        public string WalletCurrentWarehouseAddress { get; set; } // CurrentWarehouseAddress (length: 500)
        public int? WalletTargetWarehouseId { get; set; } // TargetWarehouseId
        public string WalletTargetWarehouseIdPath { get; set; } // TargetWarehouseIdPath (length: 300)
        public string WalletTargetWarehouseName { get; set; } // TargetWarehouseName (length: 300)
        public string WalletTargetWarehouseAddress { get; set; } // TargetWarehouseAddress (length: 500)
        public int WalletUserId { get; set; } // UserId
        public string WalletUserName { get; set; } // UserName (length: 50)
        public int? WalletEntrepotId { get; set; } // EntrepotId
        public string WalletEntrepotName { get; set; } // UserName (length: 50)
        public string WalletUserFullName { get; set; } // UserFullName (length: 300)
        public DateTime WalletCreated { get; set; } // Created
        public string WalletNote { get; set; }
        public int? WalletPartnerId { get; set; } // PartnerId
        public string WalletPartnerName { get; set; } // PartnerName (length: 300)
        public DateTime? WalletPartnerUpdate { get; set; } // PartnerUpdate
        public int? DispatcherEntrepotId { get; set; }
        public string DispatcherEntrepotName { get; set; }
        public int? DispatcherDetailId { get; set; } // Status
        public byte? DispatcherDetailStatus { get; set; } // Status
        public decimal? DispatcherDetailWeight { get; set; } // Weight
        public decimal? DispatcherDetailActualWeight { get; set; } // ActualWeight
        public decimal? DispatcherDetailConvertedWeight { get; set; } // ConvertedWeight
        public string DispatcherDetailDescription { get; set; } // Description (length: 500)
        public decimal? DispatcherDetailValue { get; set; }
        public string DispatcherDetailNote { get; set; } // Note (length: 500)
        public decimal? DispatcherDetailVolume { get; set; } // 
        public int? TransportPartnerId { get; set; } // TransportPartnerId
        public string TransportPartnerName { get; set; } // TransportPartnerName (length: 300)
        public int? TransportMethodId { get; set; } // TransportMethodId
        public string TransportMethodName { get; set; } // TransportMethodName (length: 300)
        public int? DispatcherId { get; set; }
        public string DispatcherCode { get; set; }
        public int? DispatcherCreatedUserId { get; set; } // UserId
        public string DispatcherCreatedUserName { get; set; } // UserName (length: 50)
        public string DispatcherCreatedUserFullName { get; set; } // UserFullName (length: 300)
        public byte? DispatcherPriceType { get; set; } // PriceType
        public decimal? DispatcherValue { get; set; } // Value
        public DateTime? DispatcherCreated { get; set; } // Created


        public int? FromTransportPartnerId { get; set; } // FromTransportPartnerId
        public string FromTransportPartnerName { get; set; } // FromTransportPartnerName (length: 300)
        public int? FromTransportMethodId { get; set; } // FromTransportMethodId
        public string FromTransportMethodName { get; set; } // FromTransportMethodName (length: 300)
        public int? FromEntrepotId { get; set; } // FromEntrepotId
        public string FromEntrepotName { get; set; } // FromEntrepotName (length: 300)
        public int? ToTransportPartnerId { get; set; } // ToTransportPartnerId
        public string ToTransportPartnerName { get; set; } // ToTransportPartnerName (length: 300)
        public DateTime? ToTransportPartnerTime { get; set; } // ToTransportPartnerTime
        public int? ToTransportMethodId { get; set; } // ToTransportMethodId
        public string ToTransportMethodName { get; set; } // ToTransportMethodName (length: 300)
        public int? ToEntrepotId { get; set; } // ToEntrepotId
        public string ToEntrepotName { get; set; } // ToEntrepotName (length: 300)

        public int? FromDispatcherId { get; set; } // FromDispatcherId
        public string FromDispatcherCode { get; set; } // FromDispatcherCode (length: 50)
        public int? ToDispatcherId { get; set; } // ToDispatcherId
        public string ToDispatcherCode { get; set; } // ToDispatcherCode (length: 50)
        public int? EntrepotId { get; set; } // EntrepotId
        public string EntrepotName { get; set; } // EntrepotName (length: 300)
    }
}
