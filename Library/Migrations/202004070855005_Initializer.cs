namespace Library.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initializer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountantSubject",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Idd = c.Int(),
                        SubjectName = c.String(nullable: false, maxLength: 100),
                        SubjectNote = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        IsIdSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.App",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(maxLength: 500),
                        Icon = c.String(maxLength: 2000),
                        Description = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        OrderNo = c.Int(nullable: false),
                        Url = c.String(maxLength: 300, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Attachment_Message",
                c => new
                    {
                        AttachmentId = c.Long(nullable: false),
                        MessageId = c.Long(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        IsCanEdit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.AttachmentId, t.MessageId });
            
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttachmentName = c.String(nullable: false, maxLength: 100),
                        AttachmentPath = c.String(nullable: false, maxLength: 500),
                        Type = c.String(maxLength: 50),
                        TypeEn = c.String(maxLength: 50),
                        Extension = c.String(nullable: false, maxLength: 10, unicode: false),
                        Size = c.Int(nullable: false),
                        SizeString = c.String(maxLength: 20, unicode: false),
                        CreatedOnDate = c.DateTime(nullable: false),
                        UploaderId = c.Long(),
                        UploaderFullName = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BagPackage",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        BagId = c.Int(nullable: false),
                        PackageId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bag",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 150),
                        Status = c.Byte(nullable: false),
                        Code = c.String(nullable: false, maxLength: 50),
                        WarehouseId = c.Int(),
                        WarehouseName = c.String(maxLength: 300),
                        OdlWarehouseId = c.Int(),
                        OldWarehouseName = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        IdPath = c.String(nullable: false, maxLength: 600),
                        NamePath = c.String(nullable: false, maxLength: 200),
                        ParentId = c.Int(),
                        ParentName = c.String(maxLength: 300),
                        Status = c.Int(nullable: false),
                        Description = c.String(maxLength: 600),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        HashTag = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChangePasswordLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        IP = c.String(nullable: false, maxLength: 20),
                        ChangeTime = c.DateTime(nullable: false),
                        OldPassword = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChatFilesAttach",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.String(maxLength: 50, unicode: false),
                        ChatId = c.Long(),
                        FileName = c.String(maxLength: 300),
                        FileUrl = c.String(unicode: false),
                        DownloaderIds = c.String(),
                        FileSize = c.Int(),
                        FileSizeText = c.String(maxLength: 150, unicode: false),
                        ContentType = c.String(maxLength: 50, unicode: false),
                        DownloadCount = c.Int(),
                        IsRemove = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimForRefund",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 30),
                        OrderType = c.Int(),
                        Status = c.Int(nullable: false),
                        TicketId = c.Int(),
                        TicketCode = c.String(maxLength: 30),
                        TicketCreated = c.DateTime(),
                        CustomerId = c.Int(),
                        CustomerCode = c.String(maxLength: 20),
                        CustomerFullName = c.String(maxLength: 200),
                        CustomerPhone = c.String(maxLength: 100),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        CustomerOfficeId = c.Int(),
                        CustomerOfficeName = c.String(maxLength: 300),
                        CustomerOfficePath = c.String(maxLength: 300),
                        OrderUserId = c.Int(),
                        OrderUserCode = c.String(maxLength: 30),
                        OrderUserFullName = c.String(maxLength: 200),
                        OrderUserEmail = c.String(maxLength: 300),
                        OrderUserPhone = c.String(maxLength: 100),
                        OrderUserOfficeId = c.Int(),
                        OrderUserOfficeName = c.String(maxLength: 300),
                        OrderUserOfficePath = c.String(maxLength: 300),
                        SupportId = c.Int(),
                        SupportCode = c.String(maxLength: 30),
                        SupportFullName = c.String(maxLength: 200),
                        SupportEmail = c.String(maxLength: 300),
                        AccountantId = c.Int(),
                        AccountantCode = c.String(maxLength: 30),
                        AccountantFullName = c.String(maxLength: 200),
                        AccountantEmail = c.String(maxLength: 300),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UserId = c.Int(),
                        UserCode = c.String(maxLength: 20),
                        UserName = c.String(maxLength: 200),
                        UserEmail = c.String(maxLength: 300),
                        UserPhone = c.String(maxLength: 100),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        MoneyRefund = c.Decimal(precision: 18, scale: 4),
                        RealTotalRefund = c.Decimal(precision: 18, scale: 4),
                        MoneyOrderRefund = c.Decimal(precision: 18, scale: 4),
                        MoneyOrderRefundDicker = c.Decimal(precision: 18, scale: 4),
                        SupporterMoneyRequest = c.Decimal(precision: 18, scale: 4),
                        CurrencyDiscount = c.Decimal(precision: 18, scale: 4),
                        NoteOrderer = c.String(),
                        NoteSupporter = c.String(),
                        NoteAccountanter = c.String(),
                        ApproverId = c.Int(),
                        ApproverName = c.String(maxLength: 200),
                        MoneyOther = c.Decimal(precision: 18, scale: 4),
                        ReasonCancel = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClaimForRefundDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Long(),
                        Name = c.String(maxLength: 600),
                        Link = c.String(),
                        Image = c.String(),
                        Quantity = c.Int(),
                        Size = c.String(maxLength: 50),
                        Color = c.String(maxLength: 50),
                        Price = c.Decimal(precision: 18, scale: 4),
                        TotalPrice = c.Decimal(precision: 18, scale: 4),
                        TotalExchange = c.Decimal(precision: 18, scale: 4),
                        OrderId = c.Int(),
                        OrderType = c.Byte(),
                        QuantityFailed = c.Int(),
                        TotalQuantityFailed = c.Decimal(precision: 18, scale: 4),
                        Note = c.String(),
                        ClaimId = c.Int(),
                        ClaimCode = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComplainHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ComplainId = c.Long(nullable: false),
                        Status = c.Byte(nullable: false),
                        Content = c.String(nullable: false, maxLength: 1000),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 300),
                        CreateDate = c.DateTime(nullable: false),
                        ClaimForRefundId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComplainOrder",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ComplainId = c.Long(nullable: false),
                        OrderDetailId = c.Int(nullable: false),
                        Note = c.String(maxLength: 2000),
                        CreateDate = c.DateTime(),
                        LinkOrder = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Complain",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(maxLength: 30),
                        TypeOrder = c.Byte(nullable: false),
                        TypeService = c.Int(nullable: false),
                        TypeServiceName = c.String(maxLength: 500),
                        TypeServiceClose = c.Int(),
                        TypeServiceCloseName = c.String(maxLength: 500),
                        ImagePath1 = c.String(maxLength: 255),
                        ImagePath2 = c.String(maxLength: 255),
                        ImagePath3 = c.String(maxLength: 255),
                        ImagePath4 = c.String(maxLength: 255),
                        ImagePath5 = c.String(maxLength: 255),
                        ImagePath6 = c.String(maxLength: 255),
                        Content = c.String(maxLength: 2000),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(maxLength: 30),
                        OrderType = c.Byte(),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(maxLength: 255),
                        CreateDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        SystemId = c.Int(),
                        SystemName = c.String(maxLength: 100),
                        Status = c.Byte(),
                        LastReply = c.String(maxLength: 2000),
                        BigMoney = c.Decimal(storeType: "money"),
                        IsDelete = c.Boolean(nullable: false),
                        RequestMoney = c.Decimal(storeType: "money"),
                        ContentInternal = c.String(),
                        ContentInternalOrder = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComplainType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPath = c.String(nullable: false, maxLength: 300),
                        NamePath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 500),
                        ParentId = c.Int(nullable: false),
                        ParentName = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        IsParent = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 500),
                        Index = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ComplainUser",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ComplainId = c.Long(nullable: false),
                        UserId = c.Int(),
                        Content = c.String(storeType: "ntext"),
                        AttachFile = c.String(maxLength: 255),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        UserRequestId = c.Int(),
                        UserRequestName = c.String(maxLength: 255),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 255),
                        UserName = c.String(maxLength: 255),
                        IsRead = c.Boolean(),
                        IsCare = c.Boolean(),
                        IsInHouse = c.Boolean(),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        GroupId = c.Int(),
                        CommentType = c.Byte(),
                        SystemId = c.Int(),
                        SystemName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfigLoginFailure",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaximumLoginFailure = c.Byte(nullable: false),
                        LockDuration = c.Short(nullable: false),
                        LoginFailureInterval = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerCallHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 300),
                        UserName = c.String(nullable: false, maxLength: 30),
                        TitleId = c.Int(nullable: false),
                        TitleName = c.String(nullable: false, maxLength: 300),
                        OfficeId = c.Int(nullable: false),
                        OfficeName = c.String(nullable: false, maxLength: 300),
                        OfficeIdPath = c.String(nullable: false, maxLength: 600),
                        OfficeNamePath = c.String(nullable: false, maxLength: 600),
                        CustomerId = c.Int(nullable: false),
                        CustomerEmail = c.String(nullable: false, maxLength: 300),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                        CustomerVipId = c.Byte(nullable: false),
                        CustomerVipName = c.String(nullable: false, maxLength: 300),
                        Mode = c.Byte(nullable: false),
                        IsLast = c.Boolean(nullable: false),
                        ObjectId = c.Int(),
                        Content = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerConfigLevel",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CustomerConfigName = c.String(maxLength: 100),
                        TurnoverRate = c.String(maxLength: 300),
                        Description = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CustomerLevel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Description = c.String(maxLength: 500),
                        Status = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        UserName = c.String(maxLength: 50, unicode: false),
                        IsDelete = c.Boolean(nullable: false),
                        StartMoney = c.Decimal(nullable: false, storeType: "money"),
                        EndMoney = c.Decimal(nullable: false, storeType: "money"),
                        PercentDeposit = c.Int(nullable: false),
                        Order = c.Byte(),
                        Ship = c.Byte(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerLogLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(nullable: false, maxLength: 50),
                        UnsignName = c.String(nullable: false, maxLength: 100, unicode: false),
                        LoginTime = c.DateTime(nullable: false),
                        IP = c.String(maxLength: 30),
                        Token = c.String(maxLength: 1000),
                        OS = c.String(maxLength: 300),
                        Browser = c.String(maxLength: 400),
                        Version = c.String(maxLength: 300),
                        LogoutTime = c.DateTime(),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 50, unicode: false),
                        FullName = c.String(nullable: false, maxLength: 50),
                        Type = c.Byte(),
                        DataBefore = c.String(),
                        DataAfter = c.String(),
                        DataType = c.Byte(),
                        LogContent = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        IP = c.String(maxLength: 30),
                        Token = c.String(maxLength: 1000),
                        OS = c.String(maxLength: 300),
                        Browser = c.String(maxLength: 400),
                        Version = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeId = c.Int(),
                        TypeIdd = c.Int(),
                        TypeName = c.String(maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                        FirstName = c.String(maxLength: 30),
                        LastName = c.String(maxLength: 30),
                        MidleName = c.String(maxLength: 30),
                        FullName = c.String(nullable: false, maxLength: 90),
                        Password = c.String(nullable: false, maxLength: 50),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(maxLength: 200),
                        Phone = c.String(nullable: false, maxLength: 50),
                        Avatar = c.String(),
                        Nickname = c.String(nullable: false, maxLength: 150),
                        LevelId = c.Byte(nullable: false),
                        LevelName = c.String(nullable: false, maxLength: 150),
                        Point = c.Int(nullable: false),
                        GenderId = c.Byte(),
                        GenderName = c.String(maxLength: 300),
                        DistrictId = c.Int(),
                        DistrictName = c.String(maxLength: 300),
                        ProvinceId = c.Int(),
                        ProvinceName = c.String(maxLength: 300),
                        WardId = c.Int(),
                        WardsName = c.String(maxLength: 300),
                        Address = c.String(maxLength: 600),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(),
                        LastLockoutDate = c.DateTime(),
                        LockoutToDate = c.DateTime(),
                        FirstLoginFailureDate = c.DateTime(),
                        LoginFailureCount = c.Byte(nullable: false),
                        HashTag = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BalanceAvalible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsActive = c.Boolean(nullable: false),
                        IsLockout = c.Boolean(nullable: false),
                        CodeActive = c.String(maxLength: 20, unicode: false),
                        CreateDateActive = c.DateTime(),
                        DateActive = c.DateTime(),
                        CountryId = c.String(maxLength: 10, unicode: false),
                        Code = c.String(maxLength: 30),
                        Status = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CardName = c.String(maxLength: 50),
                        CardId = c.String(maxLength: 20, unicode: false),
                        CardBank = c.String(maxLength: 255),
                        CardBranch = c.String(maxLength: 255),
                        WarehouseId = c.Int(),
                        WarehouseName = c.String(maxLength: 500),
                        DepositPrice = c.Decimal(precision: 18, scale: 4),
                        Birthday = c.DateTime(storeType: "date"),
                        Url = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerSale",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagePath = c.String(nullable: false, maxLength: 600, unicode: false),
                        CardId = c.String(maxLength: 50, unicode: false),
                        CardNumber = c.String(maxLength: 300),
                        SaleShoping = c.Int(nullable: false),
                        SaleShiping = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        IsDelete = c.Boolean(),
                        SystemId = c.Int(),
                        SystemName = c.String(maxLength: 100),
                        LevelId = c.Int(),
                        LevelName = c.String(maxLength: 255),
                        UserCreateId = c.Int(),
                        UserCreateName = c.String(maxLength: 255),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        UserUpdateId = c.Int(),
                        UserUpdateName = c.String(maxLength: 255),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 600),
                        NameType = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerWallet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Idd = c.Int(),
                        IdPath = c.String(nullable: false, maxLength: 400, unicode: false),
                        NamePath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 300),
                        ParentId = c.Int(nullable: false),
                        ParentName = c.String(nullable: false, maxLength: 300),
                        Operator = c.Boolean(),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                        Description = c.String(maxLength: 600),
                        IsParent = c.Boolean(nullable: false),
                        IsIdSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Debit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        Note = c.String(),
                        MustCollectMoney = c.Decimal(precision: 18, scale: 4),
                        MustReturnMoney = c.Decimal(precision: 18, scale: 4),
                        TreasureId = c.Int(),
                        TreasureIdd = c.Int(),
                        TreasureName = c.String(maxLength: 300),
                        FinanceFundId = c.Int(),
                        FinanceFundName = c.String(maxLength: 500),
                        FinanceFundBankAccountNumber = c.String(maxLength: 100),
                        FinanceFundDepartment = c.String(maxLength: 300),
                        FinanceFundNameBank = c.String(maxLength: 300),
                        FinanceFundUserFullName = c.String(maxLength: 300),
                        FinanceFundUserPhone = c.String(maxLength: 100),
                        FinanceFundUserEmail = c.String(maxLength: 300),
                        SubjectTypeId = c.Int(),
                        SubjectTypeName = c.String(maxLength: 300),
                        AccountantSubjectId = c.Int(),
                        AccountantSubjectName = c.String(maxLength: 100),
                        SubjectId = c.Int(),
                        SubjectCode = c.String(maxLength: 20),
                        SubjectName = c.String(maxLength: 300),
                        SubjectPhone = c.String(maxLength: 100),
                        SubjectEmail = c.String(maxLength: 300),
                        SubjectAddress = c.String(maxLength: 500),
                        OrderId = c.Int(),
                        OrderType = c.Byte(),
                        OrderCode = c.String(maxLength: 20),
                        UserId = c.Int(),
                        UserCode = c.String(maxLength: 20),
                        UserName = c.String(maxLength: 300),
                        UserApprovalId = c.Int(),
                        UserApprovalCode = c.String(maxLength: 20),
                        UserApprovalName = c.String(maxLength: 300),
                        IsSystem = c.Boolean(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DebitHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        DebitId = c.Int(),
                        DebitType = c.Byte(),
                        DebitCode = c.String(maxLength: 20),
                        Status = c.Byte(nullable: false),
                        Note = c.String(),
                        Money = c.Decimal(precision: 18, scale: 4),
                        OrderId = c.Int(),
                        OrderType = c.Byte(),
                        OrderCode = c.String(maxLength: 20),
                        PayReceivableId = c.Int(),
                        PayReceivableIdd = c.Int(),
                        PayReceivableIName = c.String(maxLength: 300),
                        IsSystem = c.Boolean(nullable: false),
                        SubjectId = c.Int(),
                        SubjectCode = c.String(maxLength: 20),
                        SubjectName = c.String(maxLength: 300),
                        SubjectPhone = c.String(maxLength: 100),
                        SubjectEmail = c.String(maxLength: 300),
                        SubjectAddress = c.String(maxLength: 500),
                        FinanceFundId = c.Int(),
                        FinanceFundName = c.String(maxLength: 500),
                        FinanceFundBankAccountNumber = c.String(maxLength: 100),
                        FinanceFundDepartment = c.String(maxLength: 300),
                        FinanceFundNameBank = c.String(maxLength: 300),
                        FinanceFundUserFullName = c.String(maxLength: 300),
                        FinanceFundUserPhone = c.String(maxLength: 100),
                        FinanceFundUserEmail = c.String(maxLength: 300),
                        TreasureId = c.Int(),
                        TreasureName = c.String(maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DebitReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PackageId = c.Int(),
                        PackageCode = c.String(maxLength: 50),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        ServiceId = c.Byte(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CustomerId = c.Int(nullable: false),
                        CustomerEmail = c.String(nullable: false, maxLength: 300),
                        CustomerPhone = c.String(nullable: false, maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Delivery",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        UnsignedText = c.String(nullable: false, maxLength: 4000),
                        Status = c.Byte(nullable: false),
                        OrderNo = c.Int(),
                        PackageNo = c.Int(),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(nullable: false, maxLength: 300),
                        CreatedUserId = c.Int(nullable: false),
                        CreatedUserFullName = c.String(nullable: false, maxLength: 300),
                        CreatedUserUserName = c.String(nullable: false, maxLength: 50),
                        CreatedUserTitleId = c.Int(nullable: false),
                        CreatedUserTitleName = c.String(nullable: false, maxLength: 300),
                        CreatedOfficeId = c.Int(nullable: false),
                        CreatedOfficeName = c.String(nullable: false, maxLength: 300),
                        CreatedOfficeIdPath = c.String(nullable: false, maxLength: 500),
                        CreatedTime = c.DateTime(nullable: false),
                        ExpertiseUserId = c.Int(),
                        ExpertiseUserFullName = c.String(maxLength: 300),
                        ExpertiseUserUserName = c.String(maxLength: 50),
                        ExpertiseUserTitleId = c.Int(),
                        ExpertiseUserTitleName = c.String(maxLength: 300),
                        ExpertiseOfficeId = c.Int(),
                        ExpertiseOfficeName = c.String(maxLength: 300),
                        ExpertiseOfficeIdPath = c.String(maxLength: 500),
                        ExpertiseTime = c.DateTime(),
                        ShipperUserId = c.Int(),
                        ShipperFullName = c.String(maxLength: 300),
                        ShipperUserUserName = c.String(maxLength: 50),
                        ShipperUserTitleId = c.Int(),
                        ShipperUserTitleName = c.String(maxLength: 300),
                        ShipperOfficeId = c.Int(),
                        ShipperOfficeName = c.String(maxLength: 300),
                        ShipperOfficeIdPath = c.String(maxLength: 500),
                        shipperTime = c.DateTime(),
                        ApprovelUserId = c.Int(),
                        ApprovelFullName = c.String(maxLength: 300),
                        ApprovelUserUserName = c.String(maxLength: 50),
                        ApprovelUserTitleId = c.Int(),
                        ApprovelUserTitleName = c.String(maxLength: 300),
                        ApprovelOfficeId = c.Int(),
                        ApprovelOfficeName = c.String(maxLength: 300),
                        ApprovelOfficeIdPath = c.String(maxLength: 500),
                        ApprovelTime = c.DateTime(),
                        AccountantUserId = c.Int(),
                        AccountantFullName = c.String(maxLength: 300),
                        AccountantUserUserName = c.String(maxLength: 50),
                        AccountantUserTitleId = c.Int(),
                        AccountantUserTitleName = c.String(maxLength: 300),
                        AccountantOfficeId = c.Int(),
                        AccountantOfficeName = c.String(maxLength: 300),
                        AccountantOfficeIdPath = c.String(maxLength: 500),
                        AccountantTime = c.DateTime(),
                        Note = c.String(),
                        CarNumber = c.String(maxLength: 10),
                        CarDescription = c.String(maxLength: 10),
                        IsDelete = c.Boolean(nullable: false),
                        IsLast = c.Boolean(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        CustomerCode = c.String(nullable: false, maxLength: 50),
                        CustomerFullName = c.String(nullable: false, maxLength: 300),
                        CustomerEmail = c.String(nullable: false, maxLength: 50),
                        CustomerPhone = c.String(nullable: false, maxLength: 50),
                        CustomerAddress = c.String(nullable: false, maxLength: 500),
                        CustomerVipId = c.Byte(nullable: false),
                        CustomerVipName = c.String(nullable: false, maxLength: 300),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        PriceWeight = c.Decimal(precision: 18, scale: 4),
                        PricePacking = c.Decimal(precision: 18, scale: 4),
                        PriceOrder = c.Decimal(precision: 18, scale: 4),
                        PriceOther = c.Decimal(precision: 18, scale: 4),
                        PriceStored = c.Decimal(precision: 18, scale: 4),
                        PriceShip = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Total = c.Decimal(precision: 18, scale: 4),
                        Debit = c.Decimal(precision: 18, scale: 4),
                        DebitPre = c.Decimal(precision: 18, scale: 4),
                        PricePayed = c.Decimal(precision: 18, scale: 4),
                        Receivable = c.Decimal(precision: 18, scale: 4),
                        BlanceBefo = c.Decimal(precision: 18, scale: 4),
                        BlanceAfter = c.Decimal(precision: 18, scale: 4),
                        DebitAfter = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeliveryId = c.Int(nullable: false),
                        DeliveryCode = c.String(nullable: false, maxLength: 30, unicode: false),
                        PackageId = c.Int(nullable: false),
                        packageCode = c.String(nullable: false, maxLength: 50, unicode: false),
                        OrderPackageNo = c.Int(nullable: false),
                        OrderServices = c.String(maxLength: 500),
                        Note = c.String(maxLength: 500),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        OrderId = c.Int(nullable: false),
                        OrderType = c.Byte(nullable: false),
                        TransportCode = c.String(maxLength: 50),
                        Status = c.Byte(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        WarehouseId = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        PriceWeight = c.Decimal(precision: 18, scale: 4),
                        Price = c.Decimal(precision: 18, scale: 4),
                        PricePacking = c.Decimal(precision: 18, scale: 4),
                        PriceOther = c.Decimal(precision: 18, scale: 4),
                        PriceStored = c.Decimal(precision: 18, scale: 4),
                        PriceOrder = c.Decimal(precision: 18, scale: 4),
                        PricePayed = c.Decimal(precision: 18, scale: 4),
                        Debit = c.Decimal(precision: 18, scale: 4),
                        PriceShip = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LayoutId = c.Int(),
                        LayoutName = c.String(maxLength: 300),
                        WalletId = c.Int(),
                        WalletCode = c.String(maxLength: 50),
                        ShipDiscount = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliverySpend",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DeliveryCode = c.String(nullable: false, maxLength: 50),
                        DeliveryId = c.Int(nullable: false),
                        SpendName = c.String(nullable: false, maxLength: 300),
                        SpendId = c.Byte(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Mode = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DepositDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        DepositId = c.Int(nullable: false),
                        LadingCode = c.String(maxLength: 50, unicode: false),
                        Weight = c.Double(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        CategoryName = c.String(nullable: false, maxLength: 300),
                        ProductName = c.String(nullable: false, maxLength: 300),
                        Quantity = c.Int(nullable: false),
                        Size = c.String(nullable: false, maxLength: 50),
                        Image = c.String(),
                        Note = c.String(maxLength: 600),
                        PacketNumber = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Long = c.Double(nullable: false),
                        Wide = c.Double(nullable: false),
                        High = c.Double(nullable: false),
                        ListCode = c.String(maxLength: 1000),
                        ShipTq = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ContractCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        Code = c.String(),
                        DepositDetail_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DepositDetail", t => t.DepositDetail_Id)
                .Index(t => t.DepositDetail_Id);
            
            CreateTable(
                "dbo.Deposit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 255),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 255),
                        LevelId = c.Byte(nullable: false),
                        LevelName = c.String(nullable: false, maxLength: 300),
                        Note = c.String(maxLength: 500),
                        ContactName = c.String(maxLength: 255),
                        ContactPhone = c.String(maxLength: 300, unicode: false),
                        ContactAddress = c.String(maxLength: 255),
                        ContactEmail = c.String(maxLength: 300),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 255),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        Type = c.Byte(nullable: false),
                        PacketNumber = c.Int(),
                        Description = c.String(maxLength: 1000),
                        Status = c.Byte(nullable: false),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(nullable: false, maxLength: 500),
                        ProvisionalMoney = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalWeight = c.Double(nullable: false),
                        Currency = c.String(nullable: false, maxLength: 50),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        UnsignName = c.String(maxLength: 500),
                        ReasonCancel = c.String(maxLength: 500),
                        DepositType = c.Int(),
                        WarehouseDeliveryId = c.Int(),
                        WarehouseDeliveryName = c.String(maxLength: 500),
                        ApprovelUnit = c.String(maxLength: 50),
                        ApprovelPrice = c.Decimal(precision: 18, scale: 4),
                        OrderInfoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DispatcherDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DispatcherId = c.Int(nullable: false),
                        DispatcherCode = c.String(nullable: false, maxLength: 50),
                        FromDispatcherId = c.Int(),
                        FromDispatcherCode = c.String(maxLength: 50),
                        ToDispatcherId = c.Int(),
                        ToDispatcherCode = c.String(maxLength: 50),
                        TransportPartnerId = c.Int(nullable: false),
                        TransportPartnerName = c.String(nullable: false, maxLength: 300),
                        TransportMethodId = c.Int(nullable: false),
                        TransportMethodName = c.String(nullable: false, maxLength: 300),
                        EntrepotId = c.Int(),
                        EntrepotName = c.String(maxLength: 300),
                        WalletId = c.Int(nullable: false),
                        WalletCode = c.String(nullable: false, maxLength: 50),
                        Status = c.Byte(nullable: false),
                        Amount = c.Decimal(precision: 18, scale: 4),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        Volume = c.Decimal(precision: 18, scale: 4),
                        Value = c.Decimal(precision: 18, scale: 4),
                        PackageNo = c.Int(nullable: false),
                        Size = c.String(maxLength: 300),
                        Description = c.String(maxLength: 500),
                        Note = c.String(maxLength: 500),
                        FromTransportPartnerId = c.Int(),
                        FromTransportPartnerName = c.String(maxLength: 300),
                        FromTransportMethodId = c.Int(),
                        FromTransportMethodName = c.String(maxLength: 300),
                        FromEntrepotId = c.Int(),
                        FromEntrepotName = c.String(maxLength: 300),
                        ToTransportPartnerId = c.Int(),
                        ToTransportPartnerName = c.String(maxLength: 300),
                        ToTransportPartnerTime = c.DateTime(),
                        ToTransportMethodId = c.Int(),
                        ToTransportMethodName = c.String(maxLength: 300),
                        ToEntrepotId = c.Int(),
                        ToEntrepotName = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dispatcher",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        FromWarehouseId = c.Int(),
                        FromWarehouseIdPath = c.String(maxLength: 300),
                        FromWarehouseName = c.String(maxLength: 300),
                        FromWarehouseAddress = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        Amount = c.Decimal(precision: 18, scale: 4),
                        TotalWeight = c.Decimal(precision: 18, scale: 2),
                        TotalWeightActual = c.Decimal(precision: 18, scale: 2),
                        TotalWeightConverted = c.Decimal(precision: 18, scale: 2),
                        TotalVolume = c.Decimal(precision: 18, scale: 4),
                        TotalPackageNo = c.Int(nullable: false),
                        WalletNo = c.Int(nullable: false),
                        PriceType = c.Byte(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        UserFullName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Note = c.String(maxLength: 500),
                        UnsignedText = c.String(nullable: false, maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        ForcastDate = c.DateTime(),
                        ToWarehouseId = c.Int(),
                        ToWarehouseIdPath = c.String(maxLength: 300),
                        ToWarehouseName = c.String(maxLength: 300),
                        ToWarehouseAddress = c.String(maxLength: 500),
                        TransportPartnerId = c.Int(nullable: false),
                        TransportPartnerName = c.String(nullable: false, maxLength: 300),
                        TransportMethodId = c.Int(nullable: false),
                        TransportMethodName = c.String(nullable: false, maxLength: 300),
                        ContactName = c.String(maxLength: 300),
                        ContactPhone = c.String(maxLength: 20),
                        EntrepotId = c.Int(),
                        EntrepotName = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        ProvinceId = c.Int(nullable: false),
                        ProvinceName = c.String(nullable: false, maxLength: 300),
                        Culture = c.String(nullable: false, maxLength: 2, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Draw",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(maxLength: 255),
                        CustomerCode = c.String(maxLength: 20),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 100),
                        CardName = c.String(maxLength: 50),
                        CardId = c.String(maxLength: 20, unicode: false),
                        CardBank = c.String(maxLength: 255),
                        CardBranch = c.String(maxLength: 255),
                        CreateDate = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 255),
                        Status = c.Byte(),
                        Note = c.String(nullable: false, maxLength: 500),
                        AdvanceMoney = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Entrepot",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExportWarehouse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        OrderNo = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 30),
                        UserFullName = c.String(maxLength: 300),
                        UnsignedText = c.String(nullable: false),
                        Note = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExportWarehouseDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExportWarehouseId = c.Int(nullable: false),
                        ExportWarehouseCode = c.String(nullable: false, maxLength: 50),
                        PackageId = c.Int(nullable: false),
                        PackageCode = c.String(nullable: false, maxLength: 50),
                        PackageWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PackageWeightConverted = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PackageWeightActual = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PackageTransportCode = c.String(nullable: false, maxLength: 50),
                        Note = c.String(maxLength: 500),
                        PackageSize = c.String(nullable: false, maxLength: 500),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        OrderType = c.Byte(nullable: false),
                        OrderWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderWeightConverted = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderWeightActual = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderShip = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderShipActual = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrderPackageNo = c.Int(nullable: false),
                        OrderTotalPackageNo = c.Int(nullable: false),
                        OrderNote = c.String(),
                        CustomerId = c.Int(nullable: false),
                        CustomerUserName = c.String(nullable: false, maxLength: 300),
                        CustomerFullName = c.String(nullable: false, maxLength: 300),
                        CustomerPhone = c.String(nullable: false, maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        CustomerOrderNo = c.Int(nullable: false),
                        CustomerDistance = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CustomerWeight = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CustomerWeightConverted = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CustomerWeightActual = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FinanceAccount",
                c => new
                    {
                        AccountId = c.Int(nullable: false, identity: true),
                        Card = c.String(maxLength: 50, unicode: false),
                        FullName = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 20, unicode: false),
                        Description = c.String(maxLength: 255),
                        Status = c.Boolean(),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        MoneyAvaiable = c.Decimal(storeType: "money"),
                        MoneyCurrent = c.Decimal(storeType: "money"),
                        DeputyName = c.String(maxLength: 50),
                        DeputyEmail = c.String(maxLength: 50, unicode: false),
                        DeputyPhone = c.String(maxLength: 20, unicode: false),
                        DeputyCard = c.String(maxLength: 20, unicode: false),
                        DeputyAddress = c.String(maxLength: 255),
                        FundId = c.Int(),
                    })
                .PrimaryKey(t => t.AccountId);
            
            CreateTable(
                "dbo.FinanceFund",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPath = c.String(nullable: false, maxLength: 300),
                        NamePath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 500),
                        ParentId = c.Int(nullable: false),
                        ParentName = c.String(nullable: false, maxLength: 300),
                        Status = c.Byte(nullable: false),
                        Description = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserCode = c.String(maxLength: 100),
                        UserFullName = c.String(nullable: false, maxLength: 100),
                        UserEmail = c.String(nullable: false, maxLength: 50),
                        UserPhone = c.String(maxLength: 20),
                        NameBank = c.String(maxLength: 50),
                        Department = c.String(maxLength: 600),
                        BankAccountNumber = c.String(maxLength: 50, unicode: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsParent = c.Boolean(nullable: false),
                        CardName = c.String(maxLength: 50),
                        CardId = c.String(maxLength: 20, unicode: false),
                        CardBank = c.String(maxLength: 255),
                        CardBranch = c.String(maxLength: 255),
                        Maxlength = c.String(maxLength: 4000),
                        Currency = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FundBill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Type = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        CurrencyFluctuations = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Increase = c.Decimal(precision: 18, scale: 4),
                        Diminishe = c.Decimal(precision: 18, scale: 4),
                        CurencyStart = c.Decimal(precision: 18, scale: 4),
                        CurencyEnd = c.Decimal(precision: 18, scale: 4),
                        AccountantSubjectId = c.Int(),
                        AccountantSubjectName = c.String(maxLength: 100),
                        SubjectId = c.Int(),
                        SubjectCode = c.String(maxLength: 20),
                        SubjectName = c.String(maxLength: 300),
                        SubjectPhone = c.String(maxLength: 100),
                        SubjectEmail = c.String(maxLength: 300),
                        FinanceFundId = c.Int(),
                        FinanceFundName = c.String(maxLength: 500),
                        FinanceFundBankAccountNumber = c.String(maxLength: 100),
                        FinanceFundDepartment = c.String(maxLength: 300),
                        FinanceFundNameBank = c.String(maxLength: 300),
                        FinanceFundUserFullName = c.String(maxLength: 300),
                        FinanceFundUserPhone = c.String(maxLength: 100),
                        FinanceFundUserEmail = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        TreasureId = c.Int(),
                        TreasureName = c.String(maxLength: 300),
                        Note = c.String(),
                        UserId = c.Int(),
                        UserCode = c.String(maxLength: 20),
                        UserName = c.String(maxLength: 300),
                        UserApprovalId = c.Int(),
                        UserApprovalCode = c.String(maxLength: 20),
                        UserApprovalName = c.String(maxLength: 300),
                        OrderId = c.Int(),
                        OrderCode = c.String(maxLength: 20),
                        OrderType = c.Byte(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Gift",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        DiscountType = c.Byte(nullable: false),
                        DiscountValue = c.Decimal(nullable: false, precision: 18, scale: 4),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 150),
                        Mode = c.Byte(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UseNo = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        HashTag = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatContent",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.String(nullable: false, maxLength: 100),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 250),
                        FullName = c.String(nullable: false, maxLength: 250),
                        TitleName = c.String(maxLength: 300),
                        OfficeName = c.String(maxLength: 300),
                        Image = c.String(maxLength: 500),
                        Content = c.String(nullable: false),
                        SentTime = c.DateTime(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        Type = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        AttachmentCount = c.Int(),
                        Like = c.Int(),
                        Dislike = c.Int(),
                        NumberOfReplies = c.Int(),
                        ParentId = c.Long(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatLike",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ContentId = c.Long(nullable: false),
                        GroupId = c.String(nullable: false, maxLength: 100, unicode: false),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 100),
                        FullName = c.String(nullable: false, maxLength: 150),
                        Image = c.String(maxLength: 500, unicode: false),
                        UserType = c.Byte(nullable: false),
                        IsLike = c.Boolean(nullable: false),
                        CreatedOnDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatRead",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.String(nullable: false, maxLength: 100, unicode: false),
                        UserId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        Quantity = c.Int(),
                        FromChatId = c.Long(),
                        UserType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChat",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 100, unicode: false),
                        CreatorId = c.Int(nullable: false),
                        CreatedOnDate = c.DateTime(nullable: false),
                        GroupName = c.String(nullable: false, maxLength: 500),
                        Image = c.String(maxLength: 500, unicode: false),
                        CreatorFullName = c.String(nullable: false, maxLength: 150),
                        CreatorTitleName = c.String(nullable: false, maxLength: 300),
                        CreatorOfficeName = c.String(nullable: false, maxLength: 300),
                        UnsignName = c.String(nullable: false, maxLength: 500, unicode: false),
                        IsSystem = c.Boolean(nullable: false),
                        UserQuantity = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupChatUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.String(nullable: false, maxLength: 100, unicode: false),
                        UserId = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 150),
                        Image = c.String(maxLength: 500, unicode: false),
                        TitleName = c.String(maxLength: 350),
                        OfficeName = c.String(maxLength: 350),
                        InvitedByUserId = c.Int(nullable: false),
                        JoinTime = c.DateTime(nullable: false),
                        InviteStatus = c.Byte(nullable: false),
                        Type = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        NotifyUrl = c.String(maxLength: 1000, unicode: false),
                        IsShowNotify = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroupPermision",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(nullable: false, maxLength: 300),
                        Description = c.String(maxLength: 300),
                        IsSystem = c.Boolean(nullable: false),
                        UserNo = c.Int(nullable: false),
                        AppNo = c.Short(nullable: false),
                        ModuleNo = c.Short(nullable: false),
                        PageNo = c.Short(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HashTag",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HistoryPackage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Int(),
                        OrderPackage = c.Int(),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        Note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HistorySatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                        UserId = c.Int(nullable: false),
                        Note = c.String(nullable: false, maxLength: 500),
                        RecordId = c.Int(nullable: false),
                        Mode = c.Byte(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 150),
                        Json = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImportWarehouse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        PackageNumber = c.Int(),
                        WalletNumber = c.Int(),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        ShipperName = c.String(maxLength: 300),
                        ShipperPhone = c.String(maxLength: 300),
                        ShipperAddress = c.String(maxLength: 300),
                        ShipperEmail = c.String(maxLength: 300),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 30),
                        UserFullName = c.String(maxLength: 300),
                        WarehouseManagerId = c.Int(),
                        WarehouseManagerCode = c.String(maxLength: 20),
                        WarehouseManagerFullName = c.String(maxLength: 300),
                        WarehouseAccountantId = c.Int(),
                        WarehouseAccountantCode = c.String(maxLength: 20),
                        WarehouseAccountantFullName = c.String(maxLength: 300),
                        UnsignedText = c.String(nullable: false),
                        Note = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImportWarehouseDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImportWarehouseId = c.Int(nullable: false),
                        ImportWarehouseCode = c.String(nullable: false, maxLength: 30, unicode: false),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 300),
                        CustomerUserName = c.String(maxLength: 300),
                        Type = c.Byte(nullable: false),
                        PackageId = c.Int(nullable: false),
                        packageCode = c.String(nullable: false, maxLength: 50, unicode: false),
                        OrderPackageNo = c.Int(nullable: false),
                        OrderServices = c.String(maxLength: 500),
                        Note = c.String(maxLength: 500),
                        OrderCode = c.String(maxLength: 50),
                        OrderId = c.Int(),
                        OrderType = c.Byte(nullable: false),
                        TransportCode = c.String(maxLength: 50),
                        Status = c.Byte(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        WarehouseId = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Layout",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        Name = c.String(nullable: false, maxLength: 300),
                        Mode = c.Byte(nullable: false),
                        Code = c.String(nullable: false, maxLength: 50),
                        ParentLayoutId = c.Int(),
                        ParentLayoutName = c.String(maxLength: 300),
                        IdPath = c.String(maxLength: 500),
                        NamePath = c.String(),
                        Description = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Length = c.Int(),
                        Width = c.Int(),
                        Height = c.Int(),
                        MaxWeight = c.Int(),
                        ChildNo = c.Int(),
                        UnsignName = c.String(maxLength: 300, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LockHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KeyLock = c.String(nullable: false, maxLength: 50),
                        UpdateTime = c.DateTime(nullable: false),
                        UserId = c.Long(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 100),
                        FullName = c.String(nullable: false, maxLength: 150),
                        State = c.Byte(nullable: false),
                        ReasonUnlock = c.String(nullable: false, maxLength: 1000),
                        ObjectId = c.Int(nullable: false),
                        ObjectName = c.String(nullable: false, maxLength: 150),
                        IsLatest = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogAction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RecordId = c.Long(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(maxLength: 150),
                        UnsignedName = c.String(nullable: false, maxLength: 500, unicode: false),
                        ActionTime = c.DateTime(nullable: false),
                        Content = c.String(nullable: false, maxLength: 500),
                        SessionId = c.String(nullable: false, maxLength: 300),
                        Ip = c.String(maxLength: 20),
                        Os = c.String(maxLength: 200),
                        Version = c.Int(),
                        TableName = c.String(nullable: false, maxLength: 50),
                        ActionName = c.String(nullable: false, maxLength: 50),
                        OldRecord = c.String(),
                        NewRecord = c.String(),
                        CompareRecord = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(nullable: false, maxLength: 50),
                        UnsignName = c.String(nullable: false, maxLength: 100, unicode: false),
                        LoginTime = c.DateTime(nullable: false),
                        IP = c.String(maxLength: 30),
                        Token = c.String(maxLength: 1000),
                        OS = c.String(maxLength: 300),
                        Browser = c.String(maxLength: 400),
                        Version = c.String(maxLength: 300),
                        LogoutTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogSystem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LogType = c.Byte(nullable: false),
                        ShortMessage = c.String(maxLength: 300),
                        FullMessage = c.String(),
                        UserName = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(nullable: false, maxLength: 150),
                        UnsignedName = c.String(maxLength: 500),
                        SesstionId = c.String(maxLength: 50, unicode: false),
                        Ip = c.String(maxLength: 20, unicode: false),
                        Os = c.String(maxLength: 200),
                        Broswser = c.String(maxLength: 500),
                        Version = c.Int(),
                        RequestJson = c.String(nullable: false),
                        ObjectJson = c.String(nullable: false),
                        CreatedTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageRealTime",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromUser = c.String(nullable: false, maxLength: 200),
                        FromUserId = c.Long(nullable: false),
                        FromAvatar = c.String(maxLength: 500),
                        ToUser = c.String(nullable: false),
                        CcToUser = c.String(),
                        BccToUser = c.String(),
                        Title = c.String(nullable: false, maxLength: 500),
                        UnsignTitle = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        AttachmentCount = c.Short(nullable: false),
                        SendTime = c.DateTime(),
                        LastModifiedOnDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Message_User",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MessageId = c.Long(nullable: false),
                        UserId = c.Int(nullable: false),
                        Type = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        IsTrash = c.Boolean(nullable: false),
                        Star = c.Boolean(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        ReadTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Module",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Icon = c.String(nullable: false, maxLength: 50),
                        AppId = c.Byte(nullable: false),
                        Description = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        OrderNo = c.Int(nullable: false),
                        ParentId = c.Short(),
                        ParentName = c.String(maxLength: 300),
                        Level = c.Byte(nullable: false),
                        IdPath = c.String(maxLength: 300, unicode: false),
                        NamePath = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, maxLength: 1000),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        OrderId = c.Int(),
                        OrderType = c.Int(),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 90),
                        IsRead = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 255),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 90),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotifiCommon",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false, storeType: "ntext"),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        IsRead = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 255),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 90),
                        Url = c.String(maxLength: 255),
                        Status = c.Boolean(nullable: false),
                        PublishDate = c.DateTime(),
                        ImagePath = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotifiCustomer",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 200),
                        NotiCommonId = c.Long(),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 90),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotifyRealTime",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        ToUserId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 160),
                        Avatar = c.String(maxLength: 500),
                        Content = c.String(storeType: "ntext"),
                        SendTime = c.DateTime(nullable: false),
                        Type = c.Byte(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        UnsignName = c.String(maxLength: 500, unicode: false),
                        Url = c.String(maxLength: 500, unicode: false),
                        Group = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Office",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 30),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(nullable: false, maxLength: 600),
                        ShortName = c.String(maxLength: 50),
                        IdPath = c.String(maxLength: 400, unicode: false),
                        NamePath = c.String(maxLength: 2000),
                        Status = c.Byte(nullable: false),
                        Type = c.Byte(nullable: false),
                        ParentId = c.Int(),
                        ParentName = c.String(maxLength: 300),
                        Description = c.String(maxLength: 500),
                        Address = c.String(maxLength: 500),
                        UserNo = c.Int(nullable: false),
                        TitleNo = c.Int(nullable: false),
                        ChildNo = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        LastUpdateUserId = c.Int(nullable: false),
                        Culture = c.String(maxLength: 2, unicode: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderAddress",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvinceId = c.Int(nullable: false),
                        DistrictId = c.Int(nullable: false),
                        WardId = c.Int(),
                        Address = c.String(nullable: false, maxLength: 600),
                        ProvinceName = c.String(nullable: false, maxLength: 300),
                        DistrictName = c.String(nullable: false, maxLength: 300),
                        WardName = c.String(maxLength: 300),
                        Phone = c.String(nullable: false, maxLength: 300),
                        FullName = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderComment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(maxLength: 500),
                        OrderId = c.Int(nullable: false),
                        CustomerId = c.Int(),
                        UserId = c.Int(),
                        CreateDate = c.DateTime(),
                        IsRead = c.Boolean(),
                        CustomerName = c.String(maxLength: 50),
                        UserName = c.String(maxLength: 50),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        OrderType = c.Byte(),
                        CommentType = c.Byte(),
                        UserOffice = c.String(maxLength: 100),
                        UserPhone = c.String(maxLength: 20, unicode: false),
                        GroupId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderContractCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OrderType = c.Byte(nullable: false),
                        ContractCode = c.String(maxLength: 50),
                        TotalPrice = c.Decimal(precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                        AccountantDate = c.DateTime(),
                        AccountantId = c.Int(),
                        AccountantFullName = c.String(maxLength: 50),
                        AccountantOfficeId = c.Int(),
                        AccountantOfficeName = c.String(maxLength: 300),
                        AccountantOfficeIdPath = c.String(maxLength: 300),
                        IsShip = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetailCounting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(maxLength: 50, unicode: false),
                        OrderType = c.Byte(nullable: false),
                        WebsiteName = c.String(maxLength: 300),
                        ShopId = c.Int(),
                        ShopName = c.String(maxLength: 500),
                        ShopLink = c.String(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(maxLength: 500),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 300),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        OrderDetailId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 600),
                        Image = c.String(),
                        Note = c.String(maxLength: 600),
                        Link = c.String(),
                        Quantity = c.Int(),
                        Properties = c.String(maxLength: 600),
                        ProductNo = c.Int(nullable: false),
                        BeginAmount = c.Int(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        QuantityLose = c.Int(),
                        Mode = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        NotePrivate = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        TotalPriceLose = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchangeLose = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPriceShop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchangeShop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPriceCustomer = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        CommentNo = c.Int(nullable: false),
                        ImageJson = c.String(maxLength: 600),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 600),
                        Image = c.String(),
                        Quantity = c.Int(nullable: false),
                        BeginAmount = c.Int(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AuditPrice = c.Decimal(precision: 18, scale: 4),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Note = c.String(maxLength: 600),
                        Status = c.Byte(nullable: false),
                        Link = c.String(),
                        QuantityBooked = c.Int(),
                        QuantityRecived = c.Int(),
                        QuantityIncorrect = c.Int(),
                        QuantityActuallyReceived = c.Int(),
                        Properties = c.String(maxLength: 600),
                        HashTag = c.String(),
                        CategoryId = c.Int(),
                        CategoryName = c.String(maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        UniqueCode = c.String(),
                        Size = c.String(maxLength: 50),
                        Color = c.String(maxLength: 50),
                        UserNote = c.String(maxLength: 600),
                        CountingTime = c.DateTime(),
                        CountingUserId = c.Int(),
                        CountingUserName = c.String(maxLength: 50),
                        CountingFullName = c.String(maxLength: 300),
                        CountingOfficeId = c.Int(),
                        CountingOfficeName = c.String(maxLength: 300),
                        CountingOfficeIdPath = c.String(maxLength: 300),
                        Min = c.Int(nullable: false),
                        Max = c.Int(nullable: false),
                        Prices = c.String(maxLength: 600),
                        ProId = c.String(maxLength: 50),
                        SkullId = c.String(maxLength: 50),
                        WebsiteName = c.String(maxLength: 300),
                        ShopId = c.Int(),
                        ShopName = c.String(maxLength: 500),
                        ShopLink = c.String(),
                        IsView = c.Boolean(),
                        PrivateNote = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderExchange",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        Type = c.Byte(nullable: false),
                        ModeName = c.String(maxLength: 300),
                        Currency = c.String(nullable: false, maxLength: 50),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(precision: 18, scale: 4),
                        TotalExchange = c.Decimal(precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 300),
                        BankId = c.Int(),
                        Mode = c.Byte(nullable: false),
                        Note = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        HashTag = c.String(nullable: false),
                        OrderType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        Status = c.Byte(nullable: false),
                        Content = c.String(nullable: false, maxLength: 1000),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        UserId = c.Int(),
                        Type = c.Byte(nullable: false),
                        UserFullName = c.String(maxLength: 300),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Type = c.Byte(nullable: false),
                        DataBefore = c.String(maxLength: 3000),
                        DataAfter = c.String(maxLength: 3000),
                        Content = c.String(maxLength: 3000),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 150),
                        UserOfficeId = c.Int(),
                        UserOfficeName = c.String(maxLength: 300),
                        UserOfficePath = c.String(maxLength: 300),
                        CustomerId = c.Int(),
                        CustomerFullName = c.String(maxLength: 150),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderPackage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30, unicode: false),
                        Status = c.Byte(nullable: false),
                        Note = c.String(maxLength: 600),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50, unicode: false),
                        OrderServices = c.String(maxLength: 500),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        CustomerUserName = c.String(nullable: false, maxLength: 300),
                        CustomerLevelId = c.Byte(nullable: false),
                        CustomerLevelName = c.String(nullable: false, maxLength: 300),
                        CustomerWarehouseId = c.Int(nullable: false),
                        CustomerWarehouseAddress = c.String(maxLength: 500),
                        CustomerWarehouseName = c.String(nullable: false, maxLength: 300),
                        CustomerWarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        TransportCode = c.String(maxLength: 50),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        WeightActualPercent = c.Decimal(precision: 18, scale: 4),
                        WeightWapperPercent = c.Decimal(precision: 18, scale: 4),
                        OtherService = c.Decimal(precision: 18, scale: 4),
                        WeightWapper = c.Decimal(precision: 18, scale: 2),
                        TotalPriceWapper = c.Decimal(precision: 18, scale: 4),
                        Volume = c.Decimal(precision: 18, scale: 4),
                        VolumeActual = c.Decimal(precision: 18, scale: 4),
                        VolumeWapperPercent = c.Decimal(precision: 18, scale: 4),
                        VolumeWapper = c.Decimal(precision: 18, scale: 4),
                        Size = c.String(maxLength: 500),
                        Width = c.Decimal(precision: 18, scale: 4),
                        Height = c.Decimal(precision: 18, scale: 4),
                        Length = c.Decimal(precision: 18, scale: 4),
                        TotalPrice = c.Decimal(precision: 18, scale: 4),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(nullable: false, maxLength: 300),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 300),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        HashTag = c.String(nullable: false),
                        ForcastDate = c.DateTime(),
                        PackageNo = c.Int(nullable: false),
                        UnsignedText = c.String(nullable: false, maxLength: 500),
                        CurrentLayoutId = c.Int(),
                        CurrentLayoutName = c.String(maxLength: 300),
                        CurrentLayoutIdPath = c.String(maxLength: 300),
                        CurrentWarehouseId = c.Int(),
                        CurrentWarehouseName = c.String(maxLength: 300),
                        CurrentWarehouseIdPath = c.String(maxLength: 300),
                        CurrentWarehouseAddress = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        OrderCodes = c.String(maxLength: 1000),
                        PackageCodes = c.String(maxLength: 1000),
                        Customers = c.String(maxLength: 1000),
                        OrderCodesUnsigned = c.String(maxLength: 1000),
                        PackageCodesUnsigned = c.String(maxLength: 1000),
                        CustomersUnsigned = c.String(maxLength: 1000),
                        OrderType = c.Byte(nullable: false),
                        Mode = c.String(maxLength: 50),
                        SameCodeStatus = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderProcessItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderProcessId = c.Int(),
                        OrderProcessCode = c.String(maxLength: 20),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.Int(nullable: false),
                        OrderShopCode = c.String(maxLength: 100),
                        OrderShopContract = c.String(maxLength: 100),
                        LadingCode = c.String(nullable: false, maxLength: 100),
                        CustomerId = c.Int(nullable: false),
                        CustomerCode = c.String(nullable: false, maxLength: 20),
                        CustomerName = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 50),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 300),
                        WarehouseDesId = c.Int(),
                        WarehouseCode = c.String(maxLength: 20),
                        WarehouseName = c.String(maxLength: 300),
                        WarehouseAddress = c.String(maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(),
                        LastUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderReason",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ReasonId = c.Byte(nullable: false),
                        Reason = c.String(maxLength: 500),
                        CreateDate = c.DateTime(),
                        Type = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderRefundDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderRefundId = c.Int(nullable: false),
                        OrderDetailCountingId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 600),
                        Image = c.String(),
                        Link = c.String(),
                        Quantity = c.Int(),
                        Properties = c.String(maxLength: 600),
                        ProductNo = c.Int(nullable: false),
                        BeginAmount = c.Int(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        QuantityLose = c.Int(),
                        Note = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        TotalPriceLose = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchangeLose = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPriceShop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchangeShop = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPriceCustomer = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderRefund",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        OrderId = c.Int(nullable: false),
                        LinkNo = c.Int(nullable: false),
                        ProductNo = c.Int(nullable: false),
                        UnsignText = c.String(nullable: false),
                        Status = c.Byte(nullable: false),
                        Mode = c.Byte(nullable: false),
                        Note = c.String(),
                        CreateUserId = c.Int(),
                        CreateUserFullName = c.String(maxLength: 300),
                        CreateUserName = c.String(maxLength: 50),
                        CreateOfficeId = c.Int(),
                        CreateOfficeName = c.String(maxLength: 300),
                        CreateOfficeIdPath = c.String(maxLength: 300),
                        UpdateUserId = c.Int(),
                        UpdateUserFullName = c.String(maxLength: 300),
                        UpdateUserName = c.String(maxLength: 50),
                        UpdateOfficeId = c.Int(),
                        UpdateOfficeName = c.String(maxLength: 300),
                        UpdateOfficeIdPath = c.String(maxLength: 300),
                        CommentNo = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AmountActual = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalAcount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Percent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30),
                        Type = c.Byte(nullable: false),
                        WebsiteName = c.String(maxLength: 300),
                        ShopId = c.Int(),
                        ShopName = c.String(maxLength: 500),
                        ShopLink = c.String(nullable: false),
                        ProductNo = c.Int(nullable: false),
                        PackageNo = c.Int(nullable: false),
                        PackageNoDelivered = c.Int(nullable: false),
                        ContractCode = c.String(maxLength: 50),
                        ContractCodes = c.String(maxLength: 300),
                        LevelId = c.Byte(nullable: false),
                        LevelName = c.String(nullable: false, maxLength: 300),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountType = c.Byte(nullable: false),
                        DiscountValue = c.Decimal(precision: 18, scale: 4),
                        GiftCode = c.String(maxLength: 30),
                        CreatedTool = c.Byte(nullable: false),
                        Currency = c.String(nullable: false, maxLength: 50),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DepositPercent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPayed = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalRefunded = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Debt = c.Decimal(nullable: false, precision: 18, scale: 4),
                        HashTag = c.String(),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(maxLength: 500),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 300),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 150),
                        UserFullName = c.String(maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        CreatedOfficeIdPath = c.String(maxLength: 300),
                        CreatedUserId = c.Int(),
                        CreatedUserFullName = c.String(maxLength: 150),
                        CreatedOfficeId = c.Int(),
                        CreatedOfficeName = c.String(maxLength: 300),
                        OrderInfoId = c.Int(nullable: false),
                        FromAddressId = c.Int(nullable: false),
                        ToAddressId = c.Int(nullable: false),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        ServiceType = c.Byte(nullable: false),
                        Note = c.String(maxLength: 500),
                        PrivateNote = c.String(maxLength: 500),
                        LinkNo = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        ExpectedDate = c.DateTime(),
                        TotalPurchase = c.Decimal(precision: 18, scale: 4),
                        TotalAdvance = c.Decimal(storeType: "money"),
                        ReasonCancel = c.String(maxLength: 500),
                        PriceBargain = c.Decimal(storeType: "money"),
                        PaidShop = c.Decimal(storeType: "money"),
                        FeeShip = c.Decimal(storeType: "money"),
                        FeeShipBargain = c.Decimal(storeType: "money"),
                        IsPayWarehouseShip = c.Boolean(nullable: false),
                        UserNote = c.String(maxLength: 500),
                        PackageNoInStock = c.Int(),
                        UnsignName = c.String(maxLength: 2000),
                        PacketNumber = c.Int(),
                        Description = c.String(maxLength: 1000),
                        ProvisionalMoney = c.Decimal(precision: 18, scale: 4),
                        DepositType = c.Int(),
                        WarehouseDeliveryId = c.Int(),
                        WarehouseDeliveryName = c.String(maxLength: 500),
                        ApprovelUnit = c.String(maxLength: 50),
                        ApprovelPrice = c.Decimal(precision: 18, scale: 4),
                        ContactName = c.String(maxLength: 255),
                        ContactPhone = c.String(maxLength: 300, unicode: false),
                        ContactAddress = c.String(maxLength: 255),
                        ContactEmail = c.String(maxLength: 300),
                        CustomerCareUserId = c.Int(),
                        CustomerCareName = c.String(maxLength: 150),
                        CustomerCareFullName = c.String(maxLength: 150),
                        CustomerCareOfficeId = c.Int(),
                        CustomerCareOfficeName = c.String(maxLength: 300),
                        CustomerCareOfficeIdPath = c.String(maxLength: 300),
                        BargainType = c.Byte(),
                        LastDeliveryTime = c.DateTime(),
                        IsRetail = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderServiceOther",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Currency = c.String(nullable: false, maxLength: 50),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Mode = c.Byte(nullable: false),
                        ObjectId = c.Int(nullable: false),
                        Type = c.Byte(nullable: false),
                        Note = c.String(),
                        Created = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(nullable: false),
                        CreatedUserFullName = c.String(nullable: false, maxLength: 300),
                        CreatedUserUserName = c.String(nullable: false, maxLength: 50),
                        CreatedUserTitleId = c.Int(nullable: false),
                        CreatedUserTitleName = c.String(nullable: false, maxLength: 300),
                        CreatedOfficeId = c.Int(nullable: false),
                        CreatedOfficeName = c.String(nullable: false, maxLength: 300),
                        CreatedOfficeIdPath = c.String(nullable: false, maxLength: 500),
                        PackageNo = c.Int(nullable: false),
                        TotalWeightActual = c.Decimal(precision: 18, scale: 2),
                        PackageCodes = c.String(nullable: false, maxLength: 500),
                        DataJson = c.String(maxLength: 4000),
                        UnsignText = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderService",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        ServiceName = c.String(nullable: false, maxLength: 300),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Currency = c.String(nullable: false, maxLength: 50),
                        Type = c.Byte(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Note = c.String(maxLength: 600),
                        HashTag = c.String(),
                        Mode = c.Byte(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackageHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        PackageCode = c.String(maxLength: 50),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        Type = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        Content = c.String(nullable: false, maxLength: 1000),
                        JsonData = c.String(maxLength: 4000),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        UserId = c.Int(),
                        UserName = c.String(maxLength: 300),
                        UserFullName = c.String(maxLength: 300),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackageNoCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        PackageCode = c.String(maxLength: 30),
                        Note = c.String(maxLength: 4000),
                        UnsignText = c.String(nullable: false),
                        Status = c.Byte(nullable: false),
                        Mode = c.Byte(nullable: false),
                        ImageJson = c.String(),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        CreateUserId = c.Int(),
                        CreateUserFullName = c.String(maxLength: 300),
                        CreateUserName = c.String(maxLength: 50),
                        CreateOfficeId = c.Int(),
                        CreateOfficeName = c.String(maxLength: 300),
                        CreateOfficeIdPath = c.String(maxLength: 300),
                        UpdateUserId = c.Int(),
                        UpdateUserFullName = c.String(maxLength: 300),
                        UpdateUserName = c.String(maxLength: 50),
                        UpdateOfficeId = c.Int(),
                        CommentNo = c.Int(nullable: false),
                        UpdateOfficeName = c.String(maxLength: 300),
                        UpdateOfficeIdPath = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackageNote",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        PackageId = c.Int(),
                        PackageCode = c.String(maxLength: 50),
                        Mode = c.Byte(nullable: false),
                        Time = c.DateTime(nullable: false),
                        ObjectId = c.Int(),
                        ObjectCode = c.String(maxLength: 50),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 300),
                        DataJson = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackageTranport",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        StoreId = c.Int(nullable: false),
                        StoreName = c.String(nullable: false, maxLength: 300),
                        Note = c.String(maxLength: 600),
                        Address = c.String(maxLength: 600),
                        Type = c.Byte(),
                        TypeName = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackingList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        PackingListName = c.String(nullable: false, maxLength: 300),
                        TransportType = c.Byte(),
                        PackageNumber = c.Int(),
                        WalletNumber = c.Int(),
                        ExportWarehouseId = c.Int(),
                        ExportWarehouseCode = c.String(maxLength: 20),
                        ExportWarehouseName = c.String(maxLength: 300),
                        ExportWarehouseAddress = c.String(maxLength: 300),
                        TimeStart = c.DateTime(),
                        TimeEnd = c.DateTime(),
                        WarehouseSourceId = c.Int(),
                        WarehouseSourceCode = c.String(maxLength: 20),
                        WarehouseSourceName = c.String(maxLength: 300),
                        WarehouseSourceAddress = c.String(maxLength: 300),
                        WarehouseDesId = c.Int(),
                        WarehouseDesCode = c.String(maxLength: 20),
                        WarehouseDesName = c.String(maxLength: 300),
                        WarehouseDesAddress = c.String(maxLength: 300),
                        UserId = c.Int(nullable: false),
                        UserCode = c.String(nullable: false, maxLength: 20),
                        UserFullName = c.String(nullable: false, maxLength: 300),
                        WarehouseManagerId = c.Int(),
                        WarehouseManagerCode = c.String(maxLength: 20),
                        WarehouseManagerFullName = c.String(maxLength: 300),
                        WarehouseAccountantId = c.Int(),
                        WarehouseAccountantCode = c.String(maxLength: 20),
                        WarehouseAccountantFullName = c.String(maxLength: 300),
                        Created = c.DateTime(),
                        LastUpdate = c.DateTime(),
                        ShipperName = c.String(maxLength: 300),
                        ShipperPhone = c.String(maxLength: 50),
                        ShipperEmail = c.String(maxLength: 300),
                        ShipperAddress = c.String(maxLength: 300),
                        ShipperLicensePlate = c.String(maxLength: 100),
                        Note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Page",
                c => new
                    {
                        Id = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(maxLength: 500),
                        Description = c.String(maxLength: 500),
                        ModuleId = c.Short(nullable: false),
                        ModuleName = c.String(nullable: false, maxLength: 300),
                        AppId = c.Byte(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        AppName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        OrderNo = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Url = c.String(nullable: false, maxLength: 300, unicode: false),
                        Icon = c.String(maxLength: 300, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Partner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        Description = c.String(maxLength: 500),
                        Note = c.String(maxLength: 500),
                        UnsignName = c.String(maxLength: 4000),
                        IsDelete = c.Boolean(nullable: false),
                        PriorityNo = c.Int(),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PayReceivable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Idd = c.Int(),
                        IdPath = c.String(nullable: false, maxLength: 400, unicode: false),
                        NamePath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 300),
                        ParentId = c.Int(nullable: false),
                        ParentName = c.String(nullable: false, maxLength: 300),
                        Operator = c.Boolean(),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                        Description = c.String(maxLength: 600),
                        IsParent = c.Boolean(nullable: false),
                        IsIdSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermissionAction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppId = c.Byte(nullable: false),
                        ModuleId = c.Short(nullable: false),
                        PageId = c.Short(nullable: false),
                        GroupPermisionId = c.Short(),
                        GroupPermisionName = c.String(maxLength: 300),
                        RoleActionId = c.Byte(nullable: false),
                        AppName = c.String(nullable: false, maxLength: 300),
                        ModuleName = c.String(nullable: false, maxLength: 300),
                        PageName = c.String(nullable: false, maxLength: 300),
                        RoleName = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Checked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Position",
                c => new
                    {
                        OfficeId = c.Int(nullable: false),
                        TitleId = c.Int(nullable: false),
                        OfficeName = c.String(nullable: false, maxLength: 300),
                        TitleName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.OfficeId, t.TitleId });
            
            CreateTable(
                "dbo.PotentialCustomer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeId = c.Int(),
                        TypeIdd = c.Int(),
                        TypeName = c.String(maxLength: 100),
                        Code = c.String(maxLength: 30),
                        Email = c.String(nullable: false, maxLength: 200, unicode: false),
                        FirstName = c.String(maxLength: 30),
                        LastName = c.String(maxLength: 30),
                        MidleName = c.String(maxLength: 30),
                        FullName = c.String(nullable: false, maxLength: 90),
                        SystemId = c.Int(),
                        SystemName = c.String(maxLength: 200),
                        Phone = c.String(maxLength: 50),
                        Avatar = c.String(),
                        Nickname = c.String(maxLength: 150),
                        Birthday = c.DateTime(storeType: "date"),
                        LevelId = c.Byte(),
                        LevelName = c.String(maxLength: 150),
                        GenderId = c.Byte(),
                        GenderName = c.String(maxLength: 300),
                        DistrictId = c.Int(),
                        DistrictName = c.String(maxLength: 300),
                        ProvinceId = c.Int(),
                        ProvinceName = c.String(maxLength: 300),
                        WardId = c.Int(),
                        WardsName = c.String(maxLength: 300),
                        Address = c.String(maxLength: 600),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        HashTag = c.String(),
                        CountryId = c.String(maxLength: 10, unicode: false),
                        IsDelete = c.Boolean(nullable: false),
                        CustomerTypeId = c.Int(),
                        CustomerTypeName = c.String(maxLength: 300),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Province",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Culture = c.String(nullable: false, maxLength: 2, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PutAwayDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PutAwayId = c.Int(nullable: false),
                        PutAwayCode = c.String(nullable: false, maxLength: 30, unicode: false),
                        PackageId = c.Int(nullable: false),
                        PackageCode = c.String(nullable: false, maxLength: 50),
                        TransportCode = c.String(nullable: false, maxLength: 50),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 50),
                        OrderType = c.Byte(nullable: false),
                        OrderServices = c.String(maxLength: 500),
                        OrderPackageNo = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        CustomerUserName = c.String(nullable: false, maxLength: 300),
                        Note = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Size = c.String(maxLength: 500),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 4),
                        LayoutId = c.Int(),
                        LayoutName = c.String(maxLength: 300),
                        LayoutIdPath = c.String(maxLength: 300),
                        LayoutNamePath = c.String(),
                        ConvertedWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PutAway",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        TotalWeight = c.Decimal(precision: 18, scale: 2),
                        TotalActualWeight = c.Decimal(precision: 18, scale: 2),
                        TotalConversionWeight = c.Decimal(precision: 18, scale: 2),
                        PackageNo = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        WarehouseName = c.String(maxLength: 300),
                        WarehouseAddress = c.String(nullable: false, maxLength: 500),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        UserFullName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        UnsignedText = c.String(nullable: false, maxLength: 500),
                        Note = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Recent",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RecordId = c.Int(nullable: false),
                        Mode = c.Byte(nullable: false),
                        CountNo = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RechargeBill",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Type = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        Note = c.String(),
                        CurrencyFluctuations = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Increase = c.Decimal(precision: 18, scale: 4),
                        Diminishe = c.Decimal(precision: 18, scale: 4),
                        CurencyStart = c.Decimal(precision: 18, scale: 4),
                        CurencyEnd = c.Decimal(precision: 18, scale: 4),
                        UserId = c.Int(),
                        UserCode = c.String(maxLength: 20),
                        UserName = c.String(maxLength: 100),
                        UserApprovalId = c.Int(),
                        UserApprovalCode = c.String(maxLength: 20),
                        UserApprovalName = c.String(maxLength: 300),
                        CustomerId = c.Int(),
                        CustomerCode = c.String(maxLength: 20),
                        CustomerName = c.String(maxLength: 100),
                        CustomerPhone = c.String(maxLength: 100),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 300),
                        TreasureId = c.Int(),
                        TreasureName = c.String(maxLength: 100),
                        TreasureIdd = c.Int(),
                        IsAutomatic = c.Boolean(),
                        OrderId = c.Int(),
                        OrderCode = c.String(maxLength: 20),
                        OrderType = c.Byte(),
                        Created = c.DateTime(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RequestShip",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30),
                        OrderId = c.Int(nullable: false),
                        OrderCode = c.String(nullable: false, maxLength: 30),
                        PackageCode = c.String(maxLength: 500),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 300),
                        CustomerEmail = c.String(nullable: false, maxLength: 300),
                        CustomerPhone = c.String(nullable: false, maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        IsDelete = c.Boolean(nullable: false),
                        Note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleAction",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(maxLength: 500),
                        Description = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SendEmailResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Send_Email_Id = c.Long(nullable: false),
                        SendId = c.String(maxLength: 150),
                        Email = c.String(nullable: false, maxLength: 100),
                        Type = c.Byte(nullable: false),
                        Message = c.String(nullable: false),
                        ErrorCode = c.Int(),
                        Time = c.DateTime(nullable: false),
                        Status = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SendEmail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromUserId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 500),
                        FromUserName = c.String(nullable: false, maxLength: 100),
                        FromUserFullName = c.String(nullable: false, maxLength: 150),
                        FromUserEmail = c.String(maxLength: 100),
                        To = c.String(nullable: false),
                        Cc = c.String(),
                        Bcc = c.String(),
                        Content = c.String(nullable: false),
                        CreatedOnDate = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                        SendTime = c.DateTime(),
                        Type = c.Byte(nullable: false),
                        UnsignName = c.String(nullable: false, maxLength: 500, unicode: false),
                        AttachmentCount = c.Short(),
                        Attachments = c.String(),
                        IsLock = c.Boolean(nullable: false),
                        ErrorNo = c.Byte(nullable: false),
                        ErrorLastTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SettingKey = c.String(nullable: false),
                        SettingValue = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Website = c.String(nullable: false, maxLength: 300),
                        Url = c.String(nullable: false),
                        CategoryId = c.Int(),
                        CategoryName = c.String(maxLength: 300),
                        Vote = c.Int(),
                        Note = c.String(maxLength: 500),
                        HashTag = c.String(),
                        Status = c.Byte(nullable: false),
                        LinkNo = c.Int(nullable: false),
                        OrderNo = c.Int(nullable: false),
                        ProductNo = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BargainMax = c.Decimal(nullable: false, precision: 18, scale: 4),
                        BargainMin = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SourceId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 600),
                        Quantity = c.Int(nullable: false),
                        BeginAmount = c.Int(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Note = c.String(maxLength: 600),
                        Status = c.Byte(nullable: false),
                        Link = c.String(),
                        QuantityBooked = c.Int(nullable: false),
                        Properties = c.String(maxLength: 600),
                        HashTag = c.String(),
                        CategoryId = c.Int(),
                        CategoryName = c.String(maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        UniqueCode = c.String(),
                        Size = c.String(maxLength: 50),
                        Color = c.String(maxLength: 50),
                        ImagePath1 = c.String(maxLength: 255),
                        ImagePath2 = c.String(maxLength: 255),
                        ImagePath3 = c.String(maxLength: 255),
                        ImagePath4 = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Source",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 30),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        WarehouseId = c.Int(nullable: false),
                        WarehouseName = c.String(maxLength: 500),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 300),
                        CustomerEmail = c.String(maxLength: 300),
                        CustomerPhone = c.String(maxLength: 300),
                        CustomerAddress = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        UserId = c.Int(),
                        UserFullName = c.String(maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        CreatedOfficeIdPath = c.String(maxLength: 300),
                        CreatedUserId = c.Int(),
                        CreatedUserFullName = c.String(maxLength: 150),
                        CreatedOfficeId = c.Int(),
                        CreatedOfficeName = c.String(maxLength: 300),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                        TypeService = c.Int(nullable: false),
                        TypeServiceName = c.String(maxLength: 50),
                        ServiceMoney = c.Decimal(nullable: false, precision: 18, scale: 4),
                        IsDelete = c.Boolean(nullable: false),
                        AnalyticSupplier = c.String(),
                        ShipMoney = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SourceSupplierId = c.Long(),
                        Type = c.Byte(nullable: false),
                        UserNote = c.String(maxLength: 500),
                        UnsignName = c.String(maxLength: 500),
                        ReasonCancel = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceServiceCustomer",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(maxLength: 255),
                        SourceServiceId = c.Int(nullable: false),
                        SourceServiceName = c.String(maxLength: 255),
                        StartDate = c.DateTime(),
                        FinishDate = c.DateTime(),
                        CreateDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        IsActive = c.Boolean(),
                        CreateId = c.Int(),
                        CreateName = c.String(maxLength: 255),
                        UpdateId = c.Int(),
                        UpdateName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceService",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Description = c.String(),
                        SystemId = c.Int(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 100),
                        CreateDate = c.DateTime(),
                        LastUpdateDate = c.DateTime(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Status = c.Byte(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        CreateId = c.Int(),
                        CreateName = c.String(maxLength: 255),
                        UpdateId = c.Int(),
                        UpdateName = c.String(maxLength: 255),
                        LevelId = c.Byte(nullable: false),
                        LevelName = c.String(nullable: false, maxLength: 300),
                        IsFirst = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceSupplier",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SourceId = c.Long(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ExchangePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalExchange = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Quantity = c.Int(nullable: false),
                        Name = c.String(maxLength: 255),
                        Status = c.Byte(nullable: false),
                        Link = c.String(),
                        Description = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        ShipMoney = c.Decimal(precision: 18, scale: 4),
                        ActiveDate = c.DateTime(),
                        LimitDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.System",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        Domain = c.String(nullable: false, maxLength: 500),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Title",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 300),
                        UnsignedName = c.String(nullable: false, maxLength: 600),
                        ShortName = c.String(maxLength: 50),
                        Status = c.Byte(nullable: false),
                        Description = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        LastUpdateUserId = c.Long(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        OfficeNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tracker",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Browser = c.String(maxLength: 20, unicode: false),
                        Version = c.String(maxLength: 50, unicode: false),
                        OS = c.String(maxLength: 15, unicode: false),
                        PageUrl = c.String(unicode: false),
                        UrlReferrer = c.String(unicode: false),
                        SessionID = c.String(maxLength: 30, unicode: false),
                        IP = c.String(maxLength: 20, unicode: false),
                        InTime = c.DateTime(nullable: false),
                        Country = c.String(maxLength: 20, unicode: false),
                        City = c.String(maxLength: 20),
                        IsMobileDevice = c.Boolean(nullable: false),
                        MobileDeviceManufacturer = c.String(maxLength: 20, unicode: false),
                        WebsiteId = c.Byte(nullable: false),
                        Day = c.Byte(nullable: false),
                        Month = c.Byte(nullable: false),
                        Quater = c.Byte(nullable: false),
                        Year = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Type = c.Byte(nullable: false),
                        Mode = c.Byte(nullable: false),
                        UserId = c.Int(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 150),
                        Created = c.DateTime(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        SystemId = c.Byte(nullable: false),
                        SystemName = c.String(nullable: false, maxLength: 300),
                        Method = c.Byte(nullable: false),
                        Description = c.String(maxLength: 600),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransferDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TransferId = c.Int(nullable: false),
                        TransferCode = c.String(nullable: false, maxLength: 30, unicode: false),
                        PackageId = c.Int(nullable: false),
                        PackageCode = c.String(nullable: false, maxLength: 50, unicode: false),
                        WalletId = c.Int(nullable: false),
                        WalletCode = c.String(),
                        OrderId = c.Int(),
                        OrderCode = c.String(maxLength: 50),
                        OrderType = c.Byte(nullable: false),
                        OrderServices = c.String(maxLength: 500),
                        OrderPackageNo = c.Int(),
                        TransportCode = c.String(maxLength: 50),
                        Status = c.Byte(nullable: false),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transfer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        TotalWeight = c.Decimal(precision: 18, scale: 2),
                        TotalWeightConverted = c.Decimal(precision: 18, scale: 2),
                        TotalWeightActual = c.Decimal(precision: 18, scale: 2),
                        WalletNo = c.Int(nullable: false),
                        PackageNo = c.Int(nullable: false),
                        UnsignedText = c.String(nullable: false),
                        Note = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        FromUserId = c.Int(nullable: false),
                        FromUserFullName = c.String(nullable: false, maxLength: 300),
                        FromUserUserName = c.String(nullable: false, maxLength: 50),
                        FromUserTitleId = c.Int(nullable: false),
                        FromUserTitleName = c.String(nullable: false, maxLength: 300),
                        FromWarehouseId = c.Int(nullable: false),
                        FromWarehouseName = c.String(nullable: false, maxLength: 300),
                        FromWarehouseIdPath = c.String(nullable: false, maxLength: 500),
                        FromTime = c.DateTime(nullable: false),
                        ToUserId = c.Int(),
                        ToUserFullName = c.String(maxLength: 300),
                        ToUserUserName = c.String(maxLength: 50),
                        ToUserTitleId = c.Int(),
                        ToUserTitleName = c.String(maxLength: 300),
                        ToWarehouseId = c.Int(nullable: false),
                        ToWarehouseName = c.String(nullable: false, maxLength: 300),
                        ToWarehouseIdPath = c.String(nullable: false, maxLength: 500),
                        ToTime = c.DateTime(),
                        PriceShip = c.Decimal(precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransportMethod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Treasure",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Idd = c.Int(),
                        IdPath = c.String(nullable: false, maxLength: 400, unicode: false),
                        NamePath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 300),
                        ParentId = c.Int(nullable: false),
                        ParentName = c.String(nullable: false, maxLength: 300),
                        Operator = c.Boolean(),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                        Description = c.String(maxLength: 600),
                        IsParent = c.Boolean(nullable: false),
                        IsIdSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserConnection",
                c => new
                    {
                        ConnectionId = c.String(nullable: false, maxLength: 50, unicode: false),
                        UserId = c.Int(nullable: false),
                        Session_ID = c.String(nullable: false, maxLength: 150, unicode: false),
                        UserName = c.String(maxLength: 200, unicode: false),
                        OfficeID = c.Int(),
                        OfficeName = c.String(maxLength: 250),
                        TitleName = c.String(),
                        FullName = c.String(maxLength: 250),
                        Image = c.String(unicode: false),
                        Platform = c.Int(),
                        UnsignName = c.String(unicode: false),
                        UserType = c.Byte(),
                    })
                .PrimaryKey(t => new { t.ConnectionId, t.UserId });
            
            CreateTable(
                "dbo.UserPosition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TitleId = c.Int(nullable: false),
                        OfficeId = c.Int(nullable: false),
                        TitleName = c.String(nullable: false, maxLength: 300),
                        OfficeName = c.String(nullable: false, maxLength: 300),
                        IsDefault = c.Boolean(nullable: false),
                        Type = c.Byte(nullable: false),
                        OfficeIdPath = c.String(nullable: false, maxLength: 500),
                        OfficeNamePath = c.String(maxLength: 2000),
                        DirectUserId = c.Int(),
                        DirectFullName = c.String(maxLength: 100),
                        DirectTitleId = c.Int(),
                        DirectTitleName = c.String(maxLength: 300),
                        DirectOfficeId = c.Int(),
                        DirectOfficeName = c.String(maxLength: 300),
                        ApprovalUserId = c.Int(),
                        ApprovalFullName = c.String(maxLength: 100),
                        ApprovalTitleId = c.Int(),
                        ApprovalTitleName = c.String(maxLength: 300),
                        ApprovalOfficeId = c.Int(),
                        ApprovalOfficeName = c.String(maxLength: 300),
                        LevelId = c.Short(),
                        LevelName = c.String(maxLength: 300),
                        GroupPermisionId = c.Short(),
                        GroupPermissionName = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 50, unicode: false),
                        Password = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        MidleName = c.String(maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        FullName = c.String(nullable: false, maxLength: 100),
                        UnsignName = c.String(nullable: false, maxLength: 500),
                        Gender = c.Byte(nullable: false),
                        Email = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 500),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        LastUpdateUserId = c.Int(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        Status = c.Byte(nullable: false),
                        Birthday = c.DateTime(storeType: "date"),
                        StartDate = c.DateTime(storeType: "date"),
                        Avatar = c.String(maxLength: 2000),
                        IsLockout = c.Boolean(nullable: false),
                        LastLockoutDate = c.DateTime(),
                        LockoutToDate = c.DateTime(),
                        FirstLoginFailureDate = c.DateTime(),
                        LoginFailureCount = c.Byte(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        Phone = c.String(maxLength: 20),
                        Mobile = c.String(maxLength: 20),
                        NameBank = c.String(maxLength: 50),
                        Department = c.String(maxLength: 600),
                        BankAccountNumber = c.String(maxLength: 50, unicode: false),
                        IsCompany = c.Boolean(nullable: false),
                        TypeId = c.Int(),
                        TypeIdd = c.Int(),
                        TypeName = c.String(maxLength: 100),
                        Websites = c.String(maxLength: 150),
                        Culture = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Wallet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Status = c.Byte(nullable: false),
                        Width = c.Decimal(precision: 18, scale: 4),
                        Length = c.Decimal(precision: 18, scale: 4),
                        Height = c.Decimal(precision: 18, scale: 4),
                        Size = c.String(maxLength: 500),
                        TotalWeight = c.Decimal(precision: 18, scale: 2),
                        TotalWeightConverted = c.Decimal(precision: 18, scale: 2),
                        TotalWeightActual = c.Decimal(precision: 18, scale: 2),
                        TotalVolume = c.Decimal(precision: 18, scale: 4),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        WeightConverted = c.Decimal(precision: 18, scale: 2),
                        WeightActual = c.Decimal(precision: 18, scale: 2),
                        Volume = c.Decimal(precision: 18, scale: 4),
                        TotalValue = c.Decimal(precision: 18, scale: 4),
                        PackageNo = c.Int(nullable: false),
                        CreatedWarehouseId = c.Int(nullable: false),
                        CreatedWarehouseIdPath = c.String(nullable: false, maxLength: 300),
                        CreatedWarehouseName = c.String(maxLength: 300),
                        CreatedWarehouseAddress = c.String(nullable: false, maxLength: 500),
                        CurrentWarehouseId = c.Int(),
                        CurrentWarehouseIdPath = c.String(maxLength: 300),
                        CurrentWarehouseName = c.String(maxLength: 300),
                        CurrentWarehouseAddress = c.String(maxLength: 500),
                        TargetWarehouseId = c.Int(),
                        TargetWarehouseIdPath = c.String(maxLength: 300),
                        TargetWarehouseName = c.String(maxLength: 300),
                        TargetWarehouseAddress = c.String(maxLength: 500),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 50),
                        UserFullName = c.String(nullable: false, maxLength: 300),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        UnsignedText = c.String(nullable: false, maxLength: 500),
                        Note = c.String(maxLength: 500),
                        IsDelete = c.Boolean(nullable: false),
                        OrderCodes = c.String(maxLength: 1000),
                        PackageCodes = c.String(maxLength: 1000),
                        Customers = c.String(maxLength: 1000),
                        OrderCodesUnsigned = c.String(maxLength: 1000),
                        PackageCodesUnsigned = c.String(maxLength: 1000),
                        CustomersUnsigned = c.String(maxLength: 1000),
                        Mode = c.Byte(nullable: false),
                        PartnerId = c.Int(),
                        PartnerName = c.String(maxLength: 300),
                        PartnerUpdate = c.DateTime(),
                        EntrepotId = c.Int(),
                        EntrepotName = c.String(maxLength: 300),
                        OrderServices = c.String(),
                        OrderServicesJson = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WalletDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WalletId = c.Int(nullable: false),
                        WalletCode = c.String(nullable: false, maxLength: 30, unicode: false),
                        PackageId = c.Int(nullable: false),
                        packageCode = c.String(nullable: false, maxLength: 50, unicode: false),
                        OrderId = c.Int(),
                        OrderCode = c.String(maxLength: 50),
                        OrderType = c.Byte(nullable: false),
                        OrderServices = c.String(maxLength: 500),
                        OrderPackageNo = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 4),
                        TransportCode = c.String(maxLength: 50),
                        Note = c.String(maxLength: 500),
                        Status = c.Byte(nullable: false),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        Volume = c.Decimal(precision: 18, scale: 4),
                        ConvertedWeight = c.Decimal(precision: 18, scale: 2),
                        ActualWeight = c.Decimal(precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                        OrderCodes = c.String(maxLength: 1000),
                        PackageCodes = c.String(maxLength: 1000),
                        Customers = c.String(maxLength: 1000),
                        OrderCodesUnsigned = c.String(maxLength: 1000),
                        PackageCodesUnsigned = c.String(maxLength: 1000),
                        CustomersUnsigned = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ward",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        ProvinceId = c.Int(nullable: false),
                        ProvinceName = c.String(nullable: false, maxLength: 300),
                        DistrictId = c.Int(nullable: false),
                        DistrictName = c.String(nullable: false, maxLength: 300),
                        Culture = c.String(nullable: false, maxLength: 2, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Warehouse",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 300),
                        Address = c.String(nullable: false, maxLength: 600),
                        Status = c.Byte(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 600),
                        UserId = c.Int(nullable: false),
                        UserFullName = c.String(nullable: false, maxLength: 150),
                        OfficeId = c.Int(),
                        OfficeName = c.String(maxLength: 300),
                        OfficeIdPath = c.String(maxLength: 300),
                        Country = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractCodes", "DepositDetail_Id", "dbo.DepositDetail");
            DropIndex("dbo.ContractCodes", new[] { "DepositDetail_Id" });
            DropTable("dbo.Warehouse");
            DropTable("dbo.Ward");
            DropTable("dbo.WalletDetail");
            DropTable("dbo.Wallet");
            DropTable("dbo.User");
            DropTable("dbo.UserPosition");
            DropTable("dbo.UserConnection");
            DropTable("dbo.Treasure");
            DropTable("dbo.TransportMethod");
            DropTable("dbo.Transfer");
            DropTable("dbo.TransferDetail");
            DropTable("dbo.Transaction");
            DropTable("dbo.Tracker");
            DropTable("dbo.Title");
            DropTable("dbo.System");
            DropTable("dbo.SourceSupplier");
            DropTable("dbo.SourceService");
            DropTable("dbo.SourceServiceCustomer");
            DropTable("dbo.Source");
            DropTable("dbo.SourceDetail");
            DropTable("dbo.Shops");
            DropTable("dbo.Setting");
            DropTable("dbo.SendEmail");
            DropTable("dbo.SendEmailResult");
            DropTable("dbo.RoleAction");
            DropTable("dbo.RequestShip");
            DropTable("dbo.RechargeBill");
            DropTable("dbo.Recent");
            DropTable("dbo.PutAway");
            DropTable("dbo.PutAwayDetail");
            DropTable("dbo.Province");
            DropTable("dbo.PotentialCustomer");
            DropTable("dbo.Position");
            DropTable("dbo.PermissionAction");
            DropTable("dbo.PayReceivable");
            DropTable("dbo.Partner");
            DropTable("dbo.Page");
            DropTable("dbo.PackingList");
            DropTable("dbo.PackageTranport");
            DropTable("dbo.PackageNote");
            DropTable("dbo.PackageNoCode");
            DropTable("dbo.PackageHistory");
            DropTable("dbo.OrderService");
            DropTable("dbo.OrderServiceOther");
            DropTable("dbo.Order");
            DropTable("dbo.OrderRefund");
            DropTable("dbo.OrderRefundDetail");
            DropTable("dbo.OrderReason");
            DropTable("dbo.OrderProcessItem");
            DropTable("dbo.OrderPackage");
            DropTable("dbo.OrderLog");
            DropTable("dbo.OrderInfo");
            DropTable("dbo.OrderHistory");
            DropTable("dbo.OrderExchange");
            DropTable("dbo.OrderDetail");
            DropTable("dbo.OrderDetailCounting");
            DropTable("dbo.OrderContractCode");
            DropTable("dbo.OrderComment");
            DropTable("dbo.OrderAddress");
            DropTable("dbo.Office");
            DropTable("dbo.NotifyRealTime");
            DropTable("dbo.NotifiCustomer");
            DropTable("dbo.NotifiCommon");
            DropTable("dbo.Notification");
            DropTable("dbo.Module");
            DropTable("dbo.Message_User");
            DropTable("dbo.MessageRealTime");
            DropTable("dbo.LogSystem");
            DropTable("dbo.LogLogin");
            DropTable("dbo.LogAction");
            DropTable("dbo.LockHistory");
            DropTable("dbo.Layout");
            DropTable("dbo.ImportWarehouseDetail");
            DropTable("dbo.ImportWarehouse");
            DropTable("dbo.HistorySatus");
            DropTable("dbo.HistoryPackage");
            DropTable("dbo.HashTag");
            DropTable("dbo.GroupPermision");
            DropTable("dbo.GroupChatUsers");
            DropTable("dbo.GroupChat");
            DropTable("dbo.GroupChatRead");
            DropTable("dbo.GroupChatLike");
            DropTable("dbo.GroupChatContent");
            DropTable("dbo.Gift");
            DropTable("dbo.FundBill");
            DropTable("dbo.FinanceFund");
            DropTable("dbo.FinanceAccount");
            DropTable("dbo.ExportWarehouseDetail");
            DropTable("dbo.ExportWarehouse");
            DropTable("dbo.Entrepot");
            DropTable("dbo.Draw");
            DropTable("dbo.District");
            DropTable("dbo.Dispatcher");
            DropTable("dbo.DispatcherDetail");
            DropTable("dbo.Deposit");
            DropTable("dbo.ContractCodes");
            DropTable("dbo.DepositDetail");
            DropTable("dbo.DeliverySpend");
            DropTable("dbo.DeliveryDetail");
            DropTable("dbo.Delivery");
            DropTable("dbo.DebitReport");
            DropTable("dbo.DebitHistory");
            DropTable("dbo.Debit");
            DropTable("dbo.CustomerWallet");
            DropTable("dbo.CustomerType");
            DropTable("dbo.CustomerSale");
            DropTable("dbo.Customer");
            DropTable("dbo.CustomerLog");
            DropTable("dbo.CustomerLogLogin");
            DropTable("dbo.CustomerLevel");
            DropTable("dbo.CustomerConfigLevel");
            DropTable("dbo.CustomerCallHistory");
            DropTable("dbo.ConfigLoginFailure");
            DropTable("dbo.ComplainUser");
            DropTable("dbo.ComplainType");
            DropTable("dbo.Complain");
            DropTable("dbo.ComplainOrder");
            DropTable("dbo.ComplainHistory");
            DropTable("dbo.ClaimForRefundDetail");
            DropTable("dbo.ClaimForRefund");
            DropTable("dbo.ChatFilesAttach");
            DropTable("dbo.ChangePasswordLog");
            DropTable("dbo.Category");
            DropTable("dbo.Bag");
            DropTable("dbo.BagPackage");
            DropTable("dbo.Attachment");
            DropTable("dbo.Attachment_Message");
            DropTable("dbo.App");
            DropTable("dbo.AccountantSubject");
        }
    }
}
