using System.Collections.Generic;
using AutoMapper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.ViewModels.Account;
using Newtonsoft.Json;

namespace Library.AutoMappers
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                // Khai bao map object tai day
                cfg.CreateMap<TitleMeta, Title>();
                cfg.CreateMap<Title, TitleMeta>();
                cfg.CreateMap<Title, TitleSuggetionResult>();
                cfg.CreateMap<Office, OfficeMeta>();
                cfg.CreateMap<OfficeMeta, Office>();
                cfg.CreateMap<OfficeMeta, Office>();
                cfg.CreateMap<Office, OfficeDropdownResult>();
                cfg.CreateMap<Category, CategoryMeta>();
                cfg.CreateMap<User, UserMeta>();
                cfg.CreateMap<UserMeta, User>();
                cfg.CreateMap<User, UserResult>();
                cfg.CreateMap<QuickAddUserMeta, User>();
                cfg.CreateMap<CustomerRegisterMeta, Customer>();
                cfg.CreateMap<Warehouse, WarehouseMeta>();
                cfg.CreateMap<WarehouseMeta, Warehouse>();
                cfg.CreateMap<Order, ShopingCartViewModel>();
                cfg.CreateMap<OrderDetail, Product>()
                    .ForMember(dest => dest.Propeties,
                        opts => opts.MapFrom(src => JsonConvert.DeserializeObject<List<Propety>>(src.Properties)))
                    .ForMember(dest => dest.Prices,
                        opts => opts.MapFrom(src => JsonConvert.DeserializeObject<List<PriceMeta>>(src.Prices)));
                cfg.CreateMap<OrderService, Service>();
                cfg.CreateMap<PageMeta, Page>();
                cfg.CreateMap<Page, PageResult>();
                cfg.CreateMap<GroupPermissionMeta, GroupPermision>();
                cfg.CreateMap<GroupPermision, GroupPermissionResult>();
                cfg.CreateMap<CustumerTypeMeta, CustomerType>();
                cfg.CreateMap<CustomerType, CustumerTypeMeta> ();
                cfg.CreateMap<CustomerMeta, Customer>();
                cfg.CreateMap<Customer, CustomerMeta>();
                cfg.CreateMap<CustomerLevelMeta, CustomerLevel>();
                cfg.CreateMap<CustomerLevel, CustomerLevelMeta>();
                cfg.CreateMap<Treasure, TreasureMeta>();
                cfg.CreateMap<TreasureMeta,Treasure > ();
                cfg.CreateMap<FinanceFund, FinanceFundData>();
                cfg.CreateMap<FinanceFundData, FinanceFund>();
                cfg.CreateMap<ComplainSelectUserResult, Complain>();
                cfg.CreateMap<AccountantSubject, AccountantSubjectMeta>();
                cfg.CreateMap<AccountantSubjectMeta, AccountantSubject>();
                cfg.CreateMap<Complain, ComplainSelectUserResult>();

                cfg.CreateMap<RechargeBill, RechargeBillMeta>();
                cfg.CreateMap<RechargeBillMeta, RechargeBill>();

                cfg.CreateMap<FundBill, FundBillMeta>();
                cfg.CreateMap<FundBillMeta, FundBill>();
                cfg.CreateMap<OrderPackage, OrderPackageForDeliveryResult>();
                cfg.CreateMap<PutAwayDetail, PutAwayDetailResult>();

                cfg.CreateMap<PotentialCustomerMeta, PotentialCustomer>();
                cfg.CreateMap<PotentialCustomer, PotentialCustomerMeta>();
                cfg.CreateMap<ExportWarehouseDetail, ExportWarehouseDetailMeta>();
                cfg.CreateMap<ExportWarehouseDetailMeta, ExportWarehouseDetail>();

                cfg.CreateMap<ClaimForRefund, ClaimForRefund>();
                cfg.CreateMap<LayoutMeta, Layout>();
                cfg.CreateMap<Layout, LayoutMeta>();

                cfg.CreateMap<ClaimForRefundDetail, ClaimForRefundDetail>();
                cfg.CreateMap<Complain, Complain>();
                cfg.CreateMap<CustomerWallet, CustomerWallet>();
                cfg.CreateMap<CustomerWallet, CustomerWallitMeta>();
                cfg.CreateMap<CustomerWallitMeta, CustomerWallet>();
                cfg.CreateMap<PayReceivable, PayReceivablesMeta>();
                cfg.CreateMap<PayReceivablesMeta, PayReceivable>();
                cfg.CreateMap<DebitHistory, DebitHistoryMeta>();
                cfg.CreateMap<DebitHistoryMeta, DebitHistory>();
                cfg.CreateMap<ComplainType, ComplainTypeMeta>();
                cfg.CreateMap<ComplainTypeMeta, ComplainType>();
            });
        }
    }
}
