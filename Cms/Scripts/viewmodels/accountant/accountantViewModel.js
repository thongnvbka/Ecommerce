var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var AccountantViewModel = function (orderDetail, depositDetail, ticketDetail, accountantDetail, orderCommerceDetailViewModel) {
    var self = this;
    //========== Các biến cho template
    self.active = ko.observable('originate');
    self.templateId = ko.observable('originate');

    self.totalImportWarehouseList = ko.observable();

    //========== Các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);

    //========== Các biến đếm
    self.totalFundBill = ko.observable();
    self.totalRechargeBill = ko.observable();
    self.totalRequestMoney = ko.observable();
    self.totalDebit = ko.observable();
    self.totalDebitOrder = ko.observable();
    self.totalMustReturn = ko.observable();
    self.totalComplain = ko.observable();
    self.totalClaimForRefund = ko.observable();
    self.totalAccountantOrder = ko.observable();

    //===========Biến tính Total money thu/ chi theo danh sách
    self.totalCollectMoney = ko.observable(0);
    self.totalReturnMoney = ko.observable(0);
    self.totalMinusMoney = ko.observable(0);

    //========== Các biến cho Search user
    self.customerName = ko.observable('- Select customer -');
    self.customer = ko.observable(null);

    // Các biến lấy thông tin trên Form tạo
    self.FinanceFund = ko.observable({
        BankAccountNumber: ko.observable(""),
        Department: ko.observable(""),
        NameBank: ko.observable(""),
        UserFullName: ko.observable(""),
        UserPhone: ko.observable(""),
        UserEmail: ko.observable("")
    });
    // Khai báo biến thông tin Orders trong xử lý Refund
    self.listOrderService = ko.observableArray([]);

    // Hàm Select customer - RechargeBill
    self.selectCustomerRecharge = function () {
        self.rechargeBillModel().CustomerId(self.customer().Id);
        self.rechargeBillModel().CustomerCode(self.customer().Code);
        self.rechargeBillModel().CustomerName(self.customer().FullName);
        self.rechargeBillModel().CustomerAddress(self.customer().Address);
        self.rechargeBillModel().CustomerEmail(self.customer().Email);
        self.rechargeBillModel().CustomerPhone(self.customer().Phone);
    }

    // Hàm xóa khách hàng - RechargeBill
    self.removeSelectCustomerRecharge = function () {
        self.rechargeBillModel().CustomerId(null);
        self.rechargeBillModel().CustomerName(null);
        self.rechargeBillModel().CustomerAddress(null);
        self.rechargeBillModel().CustomerEmail(null);
        self.rechargeBillModel().CustomerPhone(null);
    }

    // Hàm Select customer - FundBill
    self.selectCustomerFundBill = function () {
        self.fundBillModel().SubjectId(self.customer().Id);
        self.fundBillModel().SubjectCode(self.customer().Code);
        self.fundBillModel().SubjectName(self.customer().FullName);
        self.fundBillModel().SubjectPhone(self.customer().Phone);
        self.fundBillModel().SubjectEmail(self.customer().Email);
    }
    // Hàm xóa khách hàng - FundBill
    self.removeSelectCustomerFundBill = function () {
        self.fundBillModel().SubjectId(null);
        self.fundBillModel().SubjectCode(null);
        self.fundBillModel().SubjectName(null);
        self.fundBillModel().SubjectPhone(null);
        self.fundBillModel().SubjectEmail(null);
    }

    // Hàm Select customer - Debit
    self.selectCustomerDebit = function () {
        self.debitModel().SubjectId(self.customer().Id);
        self.debitModel().SubjectCode(self.customer().Code);
        self.debitModel().SubjectName(self.customer().FullName);
        self.debitModel().SubjectPhone(self.customer().Phone);
        self.debitModel().SubjectEmail(self.customer().Email);
    }
    // Hàm xóa khách hàng - FundBill
    self.removeSelectCustomerDebit = function () {
        self.debitModel().SubjectId(null);
        self.debitModel().SubjectCode(null);
        self.debitModel().SubjectName(null);
        self.debitModel().SubjectPhone(null);
        self.debitModel().SubjectEmail(null);
    }

    // Hàm Select customer - Debit
    self.selectCustomerMustReturn = function () {
        self.mustReturnModel().SubjectId(self.customer().Id);
        self.mustReturnModel().SubjectCode(self.customer().Code);
        self.mustReturnModel().SubjectName(self.customer().FullName);
        self.mustReturnModel().SubjectPhone(self.customer().Phone);
        self.mustReturnModel().SubjectEmail(self.customer().Email);
    }

    // Hàm xóa khách hàng - FundBill
    self.removeSelectCustomerMustReturn = function () {
        self.mustReturnModel().SubjectId(null);
        self.mustReturnModel().SubjectCode(null);
        self.mustReturnModel().SubjectName(null);
        self.mustReturnModel().SubjectPhone(null);
        self.mustReturnModel().SubjectEmail(null);
    }

    //============================Phương thức gọi lấy danh sách khiếu nại hỗ trợ===============
    self.listStatus = ko.observable([]);
    self.listComplainSystem = ko.observable([]);
    self.listComplainStatus = ko.observable([]);
    self.listSystemRenderPoCustomer = ko.observable([]);
    self.listSystemWallet = ko.observable([]);
    self.listSystemMustReturn = ko.observable([]);
    self.listSystemDebit = ko.observable([]);
    self.listAllCustomerComplain = ko.observable([]);
    self.listSystemRechargeWallet = ko.observable([]);
    self.listSystemWithdrawal = ko.observable([]);
    self.listDebitType = ko.observable([]);

    self.complainModel = ko.observable(new complainModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    //self.order().StatusName(statusApp.order[data.Status].Name);
    //self.order().StatusClass(statusApp.order[data.Status].Class);

    self.TitleSearch = ko.observable("Search keyword:");

    self.customerEmail = ko.observable();
    self.customerPhone = ko.observable();
    self.customerAddress = ko.observable();
    self.customerLevel = ko.observable();
    self.titleTicket = ko.observable();
    self.complainuser = ko.observable([]);
    self.complainuserinternallist = ko.observable([]);
    self.complainuser1 = ko.observable([]);
    self.titleCustomer = ko.observable();
    self.totalSupport = ko.observable();

    // Search Object - Ticket
    //KHOI TYAO BIEN SEARCH 
    self.dateStart = ko.observable();
    self.dateEnd = ko.observable();
    // Search Object 
    self.SearchCustomerModal = ko.observable({
        Keyword: ko.observable(""),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchCustomerModal = ko.observable(self.SearchCustomerModal());

    self.SearchClaimForRefundModal = ko.observable({
        Keyword: ko.observable(""),
        CustomerId: ko.observable(-1),
        UserId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchClaimForRefundData = ko.observable(self.SearchClaimForRefundModal());

    //lay ra danh sach list ticket support search
    self.GetAllTicketListByStaff = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Hỗ trợ");
        self.listAllCustomerComplain([]);

        var searchCustomerModal = ko.mapping.toJS(self.SearchCustomerModal());
        // Thiết lập lại giá trị cho Status nếu status=undefined
        if (searchCustomerModal.Status === undefined) {
            searchCustomerModal.Status = -1;
        }
        if (searchCustomerModal.SystemId === undefined) {
            searchCustomerModal.SystemId = -1;
        }
        $.post("/Accountant/GetAllTicketListByStaff", { page: page, pageSize: pagesize, searchModal: searchCustomerModal }, function (data) {
            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {
                    Id = item.Id;
                    item.UserId = item.UserId;
                    item.UserName = item.UserName;
                    item.Code = item.Code;
                    item.CustomerId = item.CustomerId;
                    item.CustomerName = item.CustomerName;
                    item.CreateDate = item.CreateDate;
                    item.Status = item.Status;
                    item.OrderId = item.OrderId;
                    item.LastUpdateDate = item.LastUpdateDate;
                    item.OrderCode = item.OrderCode;
                    item.OrderType = item.OrderType;
                    item.Content = item.Content;
                    item.TypeService = item.TypeService;
                    item.TypeServiceName = item.TypeServiceName
                    item.TypeServiceClose = item.TypeServiceClose;
                    item.TypeServiceCloseName = item.TypeServiceCloseName;
                    item.SystemName = item.SystemName;
                    item.BigMoney = item.BigMoney;
                    item.RequestMoney = item.RequestMoney;
                    item.ContentInternal = item.ContentInternal;
                    item.ContentInternalOrder = ko.observable(item.ContentInternalOrder);
                    item.UserSupportNo = item.UserSupportNo;
                    item.StatusClaimForRefund = item.StatusClaimForRefund;
                    item.CountClaimForRefund = item.CountClaimForRefund;
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.ContentInternalOrder.subscribe(function (newValue) {
                        self.NoteCloseCommonOrder(ko.mapping.toJS(item));
                    });

                    list.push(item);
                });
            self.listAllCustomerComplain(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }

    //Hàm load lại dữ liệu trên các tab
    self.renderSystemtab = function () {
        self.listSystemRenderPoCustomer([]);
        self.listComplainStatus([]);
        self.listComplainSystem([]);
        $.post("/Accountant/GetRenderSystemTab", { active: self.active() }, function (data) {
            self.listComplainStatus(data.listStatus);
            self.listSystemRenderPoCustomer(data.listSystem);
            self.listComplainSystem(data.listComplainSystem);
            $('.nav-tabs').tabdrop();
            $(".select-view").select2();
        });
    }

    //Lay du lieu do vao tim kiem ticket
    self.GetTicketSearchData = function () {
        self.listComplainSystem([]);
        self.listComplainStatus([]);
        self.totalSupport([]);
        $.post("/Accountant/GetAllSearchData", {}, function (data) {
            self.listComplainSystem(data.listComplainSystem);
            self.listComplainStatus(data.listComplainStatus);
            self.totalSupport(data.count);
        });
    }

    self.ticketSupportViewModel = function (listStatus, listSystem, listSystemRender, listAllTicketSupport) {
        //self.listComplainSystem([]);
        //self.listComplainStatus([]);
        self.listSystemRenderPoCustomer([]);
        self.listAllCustomerComplain([]);

        //self.listComplainSystem(listSystem);
        //self.listComplainStatus(listStatus);
        self.listSystemRenderPoCustomer(listSystemRender);
        self.listAllCustomerComplain(listAllTicketSupport);

    }
    //Khai báo biến xử lý Refund
    self.claimForRefund = ko.observable(new ClaimForRefund);
    //================================================Detail KHIẾU NẠI===============
    //hàm phản hồi cho khách hàng
    self.feedbackComplain = function () {
        ticketDetail.feedbackComplain();
    };
    // Tìm kiếm nhân viên hỗ trợ khiếu nại
    self.GetUser = function () {
        ticketDetail.GetUser();
    }
    self.GetTicketDetailView = function (ticketId) {
        self.GetUser();
        ticketDetail.GetTicketDetailView(ticketId);
    }
    self.viewTicketDetail = function (data) {
        self.GetUser();
        ticketDetail.viewTicketDetail(data);
    }

    self.contentCustomer = ko.observable("");
    //hàm phản hồi cho khách hàng
    self.feedbackComplain = function () {
        self.contentCustomer(ticketDetail.contentCustomer);
        if (self.contentCustomer() === "" || self.contentCustomer() === null) {
            $('#requireCustomer').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: ticketDetail.complainModel().Id(), content: self.contentCustomer(), objectChat: false }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    ticketDetail.GetTicketDetail(ticketDetail.complainModel().Id());
                    ticketDetail.contentCustomer("");
                }
            });
        }
    };
    self.contentEdit = ko.observable("");


    // Cập nhật nội dung trao đổi về khiếu nại khách hàng
    self.updateContent = function () {
        $.post("/Ticket/UpdateContent", { complainDetailId: ticketDetail.contentEditId(), complainContent: self.contentEdit() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                ticketDetail.GetTicketDetail(ticketDetail.complainModel().Id());

            }
        });
    }

    // Xóa nội dung trao đổi về khiếu nại khách hàng
    self.deleteContent = function () {
        $.post("/Ticket/DeleteContent", { complainDetailId: ticketDetail.contentEditId() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                ticketDetail.GetTicketDetail(ticketDetail.complainModel().Id());
            }
        });
    }

    //================================================/Detail KHIẾU NẠI===============


    //================================================= HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG

    self.titleCustomer = ko.observable();
    self.complainModel = ko.observable(new complainModel());
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    self.customerId = ko.observable();

    self.customerId.subscribe(function (newValue) {
        if (self.customerId() > 0 || self.customerId() != null || self.customerId() != undefined) {
            $.post("/Ticket/GetCustomerInfo", { customerId: self.customerId() }, function (data) {
                self.mapCustomerModel(data.customer);
                self.searchCustomer();
            });
        }
    });

    /// Lấy thông tin hỗ trợ khiếu nại
    self.listComplainSupport = ko.observable([]);
    self.ComplainSupport = function (complainId) {
        $.post("/Ticket/listComplainSupport", { complainId: complainId }, function (data) {
            self.listComplainSupport(data);
        });
    }



    self.active2 = ko.observable('customerDetail');
    self.templateId2 = ko.observable('customerDetail');
    //click menu tab detail  
    self.clickMenuDetail = function (name) {
        page = 1;
        if (name !== self.active2()) {
            //self.init();
            self.active2(name);
            self.templateId2(name);
            if (name === 'customerDetail') {
                self.GetCustomerInfo();
            }

            if (name === 'OrderHistory') {
                self.OrderHistory();
            }

            if (name === 'SupportHistory') {
                self.SupportHistory();
            }

            if (name === 'OrderMoney') {
                self.OrderMoney();
            }
        }
    }

    // Account khách hàng
    self.GetCustomerInfo = function () {
        $.post("/Ticket/GetCustomerInfo", { customerId: self.customerId() }, function (data) {
            if (data.customer !== null) {
                self.mapCustomerModel(data.customer);
            }

        });
    }
    ///SupportHistory
    self.listComplainByCustomer = ko.observable([]);
    self.listComplainCustomer = ko.observable([]);
    self.SupportHistory = function () {
        $.post("/Ticket/SupportHistory", { customerId: self.customerId() }, function (data) {
            self.listComplainCustomer(data.customer);
            self.listComplainByCustomer(data.customerinfo);
        });
    }

    //Xem thêm thông tin hỗ trợ khiêu nại
    self.listCustomerLookUp = ko.observable([]);
    self.CustomerLookUpId = ko.observable();
    self.CustomerListLookUp = function (Id) {
        self.Show(Id);
        self.CustomerLookUpId(Id);
    }
    self.Show = function (Id) {
        self.complainuserinternallist([]);
        self.complainuser([]);
        $.post("/Ticket/CustomerListLookUp", { complainId: Id }, function (data) {
            self.complainuserinternallist(data.complainuserinternallist);
            self.complainuser(data.complainuserlist);
        });
    }
    self.CustomerDown = function (Id) {
        self.complainuserinternallist([]);
        self.complainuser([]);
    }
    ///OrderHistory
    self.listOrderByCustomer = ko.observable([]);

    self.OrderHistory = function () {
        self.listOrderByCustomer([]);
        $.post("/Ticket/OrderHistory", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {
            total = data.totalRecord;
            self.listOrderByCustomer(data.customer);
            self.paging();
        });
    }
    ///OrderMoney
    self.listOrderMoneyByCustomer = ko.observable([]);
    self.OrderMoney = function () {
        self.listOrderMoneyByCustomer([]);
        $.post("/Ticket/OrderMoney", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {
            total = data.totalRecord;
            self.listOrderMoneyByCustomer(data.customer);
            self.paging();
        });
    }
    self.fundBillModel = ko.observable(new fundBillDetailModel());
    self.fundBillModel(accountantDetail.fundBillModel());

    // Object Detail khách hàng
    self.mapCustomerModel = function (data) {
        self.customerModel(new customerOfStaffModel());

        self.customerModel().Id(data.Id);
        self.customerModel().Email(data.Email);
        self.customerModel().FirstName(data.FirstName);
        self.customerModel().LastName(data.LastName);
        self.customerModel().MidleName(data.MidleName);
        self.customerModel().FullName(data.FullName);
        self.customerModel().Password(data.Password);
        self.customerModel().SystemId(data.SystemId);
        self.customerModel().SystemName(data.SystemName);
        self.customerModel().Phone(data.Phone);
        self.customerModel().Avatar(data.Avatar);
        self.customerModel().Nickname(data.Nickname);
        self.customerModel().LevelId(data.LevelId);
        self.customerModel().LevelName(data.LevelName);
        self.customerModel().Point(data.Point);
        self.customerModel().GenderId(data.GenderId);
        self.customerModel().GenderName(data.GenderName);
        self.customerModel().DistrictId(data.DistrictId);
        self.customerModel().DistrictName(data.DistrictName);
        self.customerModel().ProvinceId(data.ProvinceId);
        self.customerModel().ProvinceName(data.ProvinceName);
        self.customerModel().WardId(data.WardId);
        self.customerModel().WardsName(data.WardsName);
        self.customerModel().Address(data.Address);
        self.customerModel().UserId(data.UserId);
        self.customerModel().UserFullName(data.UserFullName);
        self.customerModel().Created(data.Created);
        self.customerModel().Updated(data.Updated);
        self.customerModel().LastLockoutDate(data.LastLockoutDate);
        self.customerModel().LockoutToDate(data.LockoutToDate);
        self.customerModel().FirstLoginFailureDate(data.FirstLoginFailureDate);
        self.customerModel().LoginFailureCount(data.LoginFailureCount);
        self.customerModel().HashTag(data.HashTag);
        self.customerModel().Balance(data.Balance);
        self.customerModel().BalanceAvalible(data.BalanceAvalible);
        self.customerModel().IsActive(data.IsActive);
        self.customerModel().IsLockout(data.IsLockout);
        self.customerModel().CodeActive(data.CodeActive);
        self.customerModel().CreateDateActive(data.CreateDateActive);
        self.customerModel().DateActive(data.DateActive);
        self.customerModel().CountryId(data.CountryId);
        self.customerModel().Code(data.Code);
        self.customerModel().Status(data.Status);
        self.customerModel().IsDelete(data.IsDelete);
        self.customerModel().DepositPrice(data.DepositPrice);
    };


    self.listUserToOffice = ko.observableArray([]);
    //==================== Khai báo các Object ViewModal =====================================
    // Search Object - FundBill
    self.SearchFundBillModal = ko.observable({
        Keyword: ko.observable(""),
        TypeId: ko.observable(-1),
        //FinanceFundId: ko.observable(),
        AccountantSubjectId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable(""),
        //Bổ sung
        TreasureId: ko.observable(),
        UserId: ko.observable(),
        FinanceFundId: ko.observable(),
        FinanceFundIdPath: ko.observable(),
        CurrencyFluctuations: ko.observable()

    });
    self.SearchFundBillData = ko.observable(self.SearchFundBillModal());

    // Search Object - RechargeBill
    self.SearchRechargeBillModal = ko.observable({
        Keyword: ko.observable(""),
        TypeId: ko.observable(-1),
        CustomerId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable(""),
        CustomerWalletId: ko.observable(),
        CurrencyFluctuations: ko.observable()
    });
    self.SearchRechargeBillData = ko.observable(self.SearchRechargeBillModal());



    // Search Object - MustReturn
    self.SearchMustReturnModal = ko.observable({
        Keyword: ko.observable(""),
        SubjectTypeId: ko.observable(-1),   // Mã Type đối tượng
        SubjectId: ko.observable(-1),       // Mã tên đối tượng
        OfficeId: ko.observable(-1),        // Mã văn phòng
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable(""),

    });
    self.SearchMustReturnData = ko.observable(self.SearchMustReturnModal());
    // Search Object - Debit
    self.SearchDebitModal = ko.observable({
        Keyword: ko.observable(""),
        SubjectTypeId: ko.observable(-1),   // Mã Type đối tượng
        SubjectId: ko.observable(-1),       // Mã tên đối tượng
        OfficeId: ko.observable(-1),        // Mã văn phòng
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable(""),
        Type: ko.observable(-1),    //Bổ sung
        TreasureId: ko.observable(),
        UserId: ko.observable(),
        FinanceFundId: ko.observable()

    });
    self.SearchDebitData = ko.observable(self.SearchDebitModal());

    // Search Object - WithDrawal
    self.SearchDrawalModal = ko.observable({
        Keyword: ko.observable(""),
        CustomerId: ko.observable(-1),   // Customer code
        UserId: ko.observable(-1),       // Mã nhân viên tạo
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchDrawalData = ko.observable(self.SearchDrawalModal());


    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listFundBillType = ko.observableArray([]);
    self.listFinanceFund = ko.observableArray([]);
    self.listAccountantSubject = ko.observableArray([]);
    self.listSubjectReturn = ko.observableArray([]);
    self.listSubjectCollect = ko.observableArray([]);
    self.listFundBillStatus = ko.observableArray([]);

    self.listRechargeBillType = ko.observableArray([]);
    self.listCustomer = ko.observableArray([]);
    self.listRechargeBillStatus = ko.observableArray([]);

    self.listDebitStatus = ko.observableArray([]);
    self.listSubjectType = ko.observableArray([]);
    self.listSubject = ko.observableArray([]);
    self.listOffice = ko.observableArray([]);

    self.listMustReturnStatus = ko.observableArray([]);
    self.listCustomer = ko.observableArray([]);
    self.listUser = ko.observableArray([]);
    self.listUserDetail = ko.observableArray([]);

    self.listWithDrawalStatus = ko.observableArray([]);

    self.listSystem = ko.observableArray([]);
    self.listTicketStatus = ko.observableArray([]);


    //========== Khai báo ListData đổ dữ liệu danh sách
    self.listFundBill = ko.observableArray([]);
    self.listRechargeBill = ko.observableArray([]);
    self.listDebit = ko.observableArray([]);
    self.listDebitOrder = ko.observableArray([]);
    self.listMustReturn = ko.observableArray([]);


    self.listTreasure = ko.observableArray([]);

    self.listDrawal = ko.observableArray([]);

    self.listTicketSupport = ko.observableArray([]);


    self.listClaimForRefundView = ko.observableArray([]);
    self.listClaimForRefundViewData = ko.observableArray([]);
    self.listClaimForRefundData = ko.observable([]);
    self.listClaimForRefundDetail = ko.observableArray([]);

    ///========== Khai báo Model show dữ liệu trên View
    self.fundBillModel = ko.observable(new fundBillDetailModel());
    self.rechargeBillModel = ko.observable(new rechargeBillDetailModel());
    self.debitModel = ko.observable(new debitDetailModel());
    self.debitCollectModel = ko.observable(new debitHistoryModel());
    self.mustReturnModel = ko.observable(new mustReturnDetailModel());
    self.mustDebitReturnModel = ko.observable(new debitHistoryModel());

    self.claimForRefundDetailModel = ko.observable(new ClaimForRefundDetailModel());

    ///========== Khai báo Model show dữ liệu trên View
    self.PlusOrMinus = ko.observable();
    self.AddOrEdit = ko.observable();

    self.DebitOption = ko.observable();
    self.mustReturnOption = ko.observable();

    //==================== Khởi tạo ===================================================

    self.CheckUrl = function () {
        var arr = _.split(window.location.href, "#");
        var arrCheck = ['moneyfund', 'accountantOrder', 'recharge', 'withdrawal', 'debit', 'ticket-support', 'claimforrefund', 'customerfind', 'fund-report', 'recharge-report', 'withdraw-report', 'reportAccount', 'reportOrderOffice', 'reportBusinessOffice', 'reportGomContOffice'];
        if (arr.length > 1) {
            if (_.lastIndexOf(arrCheck, arr[1]) !== -1) {
                self.clickMenu(arr[1]);
                setTimeout(function () {
                    self.dateTime();
                }, 1000);
            }
        }
    }

    $(function () {
        self.GetRenderSystem();
        self.GetInit();
        self.init();

        //tao link tu dong
        self.CheckUrl();

        $(window).bind('hashchange', function () {
            self.CheckUrl();
        });


        // Chọn quỹ
        $("#debitmustcollect_finance_tree").dropdownjstree({
            source: window.treasureAddTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.debitCollectModel().TreasureId(selected.node.original.id);
                self.debitCollectModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        // Định khoản 
        $("#debitfinanceFund_addtree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.debitCollectModel().FinanceFundId(selected.node.original.id);
                self.debitCollectModel().FinanceFundName(selected.node.original.text);
                self.debitCollectModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.debitCollectModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.debitCollectModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.debitCollectModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.debitCollectModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.debitCollectModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });


        //Định khoản 
        $("#debitreturnfinanceFund_addtree").dropdownjstree({
            source: window.treasureMinusTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.mustDebitReturnModel().TreasureId(selected.node.original.id);
                self.mustDebitReturnModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });


        //Chọn quỹ
        $("#debitfinanceFund_Minustree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.mustDebitReturnModel().FinanceFundId(selected.node.original.id);
                self.mustDebitReturnModel().FinanceFundName(selected.node.original.text);
                self.mustDebitReturnModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.mustDebitReturnModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.mustDebitReturnModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.mustDebitReturnModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.mustDebitReturnModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.mustDebitReturnModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });


        $("#treasure_addtree").dropdownjstree({
            source: window.treasureAddTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().TreasureId(selected.node.original.id);
                self.fundBillModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        $("#treasure_minustree").dropdownjstree({
            source: window.treasureMinusTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().TreasureId(selected.node.original.id);
                self.fundBillModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        // Định khoản - Nạp tiền ví
        $("#treasureWallet_addtree").dropdownjstree({
            source: window.treasureWalletAddTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.rechargeBillModel().TreasureId(selected.node.original.id);
                self.rechargeBillModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        // Định khoản - trừ tiền ví
        $("#treasureWallet_minustree").dropdownjstree({
            source: window.treasureWalletMinusTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.rechargeBillModel().TreasureId(selected.node.original.id);
                self.rechargeBillModel().TreasureName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });
        $("#financeFund_addtree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        $("#financeFund_minustree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        //Nhập định khoản trên công nợ phải thu
        $("#debitCollect_treasure_tree").dropdownjstree({
            source: window.treasureCollectJsTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.debitCollectModel().PayReceivableId(selected.node.original.id);
                self.debitCollectModel().PayReceivableIName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        //Nhập định khoản trên công nợ phải trả
        $("#debitReturn_treasure_tree").dropdownjstree({
            source: window.treasureReturnJsTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.mustDebitReturnModel().PayReceivableId(selected.node.original.id);
                self.mustDebitReturnModel().PayReceivableIName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        $("#debitCollect_financeFund_tree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.debitModel().FinanceFundId(selected.node.original.id);
                self.debitModel().FinanceFundName(selected.node.original.text);
                self.debitModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.debitModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.debitModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.debitModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.debitModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.debitModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });



        $("#mustreturn_financeFund_tree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.mustReturnModel().FinanceFundId(selected.node.original.id);
                self.mustReturnModel().FinanceFundName(selected.node.original.text);
                self.mustReturnModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.mustReturnModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.mustReturnModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.mustReturnModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.mustReturnModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.mustReturnModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });


        $("#claimforrefund_treasure_tree").dropdownjstree({
            source: window.treasureTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().TreasureId(selected.node.original.id);
                self.fundBillModel().TreasureName(selected.node.original.text);

            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        $("#claimforrefund_financeFund_tree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        //hàm check url
        var arrClaim = _.split(window.location.href, "#CFRF");
        if (arrClaim.length > 1) {
            $.post("/Ticket/GetClaimForRefundCode", { code: arrClaim[1] }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.btnViewExecuteClaimForRefund(result.claimForRefundModal);
                }
            });
        }

        var arr = _.split(window.location.href, "#TK");
        if (arr.length > 1) {
            $.post("/Ticket/GetTicketId", { code: arr[1] }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.GetTicketDetailCommon(result.id);
                }
            });
        }

        $('#commentEdit').bind('show', function () {
            self.contentEdit(ticketDetail.contentEdit());
        });

    });

    self.init = function () {
        $('.nav-tabs').tabdrop();

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserToOffice(result);
        });

        self.dateTime();

        //$(".select-view").select2();
    }

    self.dateTime = function () {
        $('#daterange-btnpayreceiveday').daterangepicker(
              {
                  locale: {
                      applyLabel: "Agree",
                      cancelLabel: "All",
                      fromLabel: "From",
                      toLabel: "To",
                      customRangeLabel: "Option",
                      firstDay: 1
                  },
                  ranges: {
                      'Today': [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                  },
                  startDate: moment().subtract(6, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  if (start.format() === 'Invalid date') {
                      $('#daterange-btnpayreceiveday span').html('Created date');
                      self.SearchFundBillModal().DateStart('');
                      //self.viewReportPotentialCustomer();
                  }
                  else {
                      $('#daterange-btnpayreceiveday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchFundBillModal().DateStart(start.format());
                      //self.viewReportPotentialCustomer();
                  }
              }
          );
        $('#daterange-btnpayreceiveday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnpayreceiveday span').html('Created date');
            self.SearchFundBillModal().DateStart('');
        });

        $('#daterange-btn').daterangepicker(
              {
                  locale: {
                      applyLabel: "Agree",
                      cancelLabel: "All",
                      fromLabel: "From",
                      toLabel: "To",
                      customRangeLabel: "Option",
                      firstDay: 1
                  },
                  ranges: {
                      'Today': [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                      '30 days ago': [moment().subtract(29, 'days'), moment()],
                      'This week': [moment().startOf('week'), moment().endOf('week')],
                      'This month': [moment().subtract(0, 'month').startOf('month'), moment().subtract(0, 'month').endOf('month')]
                  },
                  startDate: moment().subtract(29, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  if (start.format() === 'Invalid date') {
                      self.dateStart('');
                      self.dateEnd('');
                      $('#daterange-btn span').html('Created date');
                      //FundBill
                      self.SearchFundBillModal().DateStart('');
                      self.SearchFundBillModal().DateEnd('');
                      //RechargeBill
                      self.SearchRechargeBillModal().DateStart('');
                      self.SearchRechargeBillModal().DateEnd('');
                      //Debit
                      self.SearchDebitModal().DateStart('');
                      self.SearchDebitModal().DateEnd('');
                      //MustReturn
                      self.SearchMustReturnModal().DateStart('');
                      self.SearchMustReturnModal().DateEnd('');
                      //WithDrawal
                      self.SearchDrawalModal().DateStart('');
                      self.SearchDrawalModal().DateEnd('');
                      //SearchCustomerModal
                      self.SearchCustomerModal().DateStart('');
                      self.SearchCustomerModal().DateEnd('');
                      //SearchClaimForRefundModal
                      self.SearchClaimForRefundModal().DateStart('');
                      self.SearchClaimForRefundModal().DateEnd('');

                      self.reportDateStart('');
                      self.reportDateEnd('');
                  }
                  else {
                      self.dateStart(start.format());
                      self.dateEnd(end.format());
                      $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
                      //FundBill
                      self.SearchFundBillModal().DateStart(start.format());
                      self.SearchFundBillModal().DateEnd(end.format());
                      //RechargeBill
                      self.SearchRechargeBillModal().DateStart(start.format());
                      self.SearchRechargeBillModal().DateEnd(end.format());
                      //Debit
                      self.SearchDebitModal().DateStart(start.format());
                      self.SearchDebitModal().DateEnd(end.format());
                      //MustReturn
                      self.SearchMustReturnModal().DateStart(start.format());
                      self.SearchMustReturnModal().DateEnd(end.format());
                      //WithDrawal
                      self.SearchDrawalModal().DateStart(start.format());
                      self.SearchDrawalModal().DateEnd(end.format());
                      //SearchCustomerModal
                      self.SearchCustomerModal().DateStart(start.format());
                      self.SearchCustomerModal().DateEnd(end.format());
                      //SearchClaimForRefundModal
                      self.SearchClaimForRefundModal().DateStart(start.format());
                      self.SearchClaimForRefundModal().DateEnd(end.format());

                      self.reportDateStart(start.format());
                      self.reportDateEnd(end.format());
                  }

              }
          );

        $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btn span').html('Created date');
            self.SearchFundBillModal().DateStart('');
            self.SearchFundBillModal().DateEnd('');
            //RechargeBill
            self.SearchRechargeBillModal().DateStart('');
            self.SearchRechargeBillModal().DateEnd('');
            //Debit
            self.SearchDebitModal().DateStart('');
            self.SearchDebitModal().DateEnd('');
            //MustReturn
            self.SearchMustReturnModal().DateStart('');
            self.SearchMustReturnModal().DateEnd('');
            //WithDrawal
            self.SearchDrawalModal().DateStart('');
            self.SearchDrawalModal().DateEnd('');
            //SearchCustomerModal
            self.SearchCustomerModal().DateStart('');
            self.SearchCustomerModal().DateEnd('');
            //SearchClaimForRefundModal
            self.SearchClaimForRefundModal().DateStart('');
            self.SearchClaimForRefundModal().DateEnd('');

            self.reportDateStart('');
            self.reportDateEnd('');
        });

    }


    self.listStatusRefund = ko.observableArray([]);

    self.GetRenderSystem = function () {
        self.listFundBillType([]);
        self.listFinanceFund([]);
        self.listAccountantSubject([]);
        self.listFundBillStatus([]);

        self.listRechargeBillType([]);
        self.listCustomer([]);
        self.listRechargeBillStatus([]);

        self.listDebitStatus([]);
        self.listSubjectType([]);
        self.listSubject([]);
        self.listOffice([]);

        self.listMustReturnStatus([]);
        self.listCustomer([]);
        self.listUser([]);
        self.listUserDetail([]);

        self.listWithDrawalStatus([]);

        self.listSystem([]);
        self.listTicketStatus([]);
        self.listStatusRefund([]);

        self.listSystemDebit([]);
        self.listSystemMustReturn([]);

        self.listSystemWallet([]);
        self.listSystemRechargeWallet([]);

        self.listDebitType([]);

        $.post("/Accountant/GetRenderSystem", {}, function (data) {
            self.listFundBillType(data.listFundBillType);
            self.listFinanceFund(data.listFinanceFund);
            self.listAccountantSubject(data.listAccountantSubject);
            self.listFundBillStatus(data.listFundBillStatus);

            self.listRechargeBillType(data.listRechargeBillType);
            self.listCustomer(data.listCustomer);
            self.listRechargeBillStatus(data.listRechargeBillStatus);

            self.listDebitStatus(data.listDebitStatus);
            self.listSubjectType(data.listSubjectType);
            self.listSubject(data.listSubject);
            self.listOffice(data.listOffice);

            self.listMustReturnStatus(data.listMustReturnStatus);
            self.listCustomer(data.listCustomer);
            self.listUser(data.listUser);
            self.listUserDetail(data.listUserDetail);

            self.listWithDrawalStatus(data.listWithDrawalStatus);

            self.listSystem(data.listSystem);
            self.listTicketStatus(data.listTicketStatus);
            self.listStatusRefund(data.listStatusRefund);

            self.listSystemWallet(data.listSystemWallet);
            self.listSystemRechargeWallet(data.listSystemRechargeWallet);

            self.listSystemDebit(data.listSystemDebit);
            self.listSystemMustReturn(data.listSystemMustReturn);
            self.listDebitType(data.listDebitType);

        });
    }

    self.GetInit = function () {
        $.post("/Accountant/GetInit", function (data) {
            self.totalFundBill(data.totalFundBill);
            self.totalRechargeBill(data.totalRechargeBill);
            self.totalRequestMoney(data.totalRequestMoney);
            self.totalDebit(data.totalDebit);
            self.totalDebitOrder(data.totalDebitOrder);
            self.totalMustReturn(data.totalMustReturn);
            self.totalComplain(data.totalComplain);
            self.totalClaimForRefund(data.totalClaimForRefund);
            self.totalAccountantOrder(data.totalAccountantOrder);
        });
    };                    // Hàm lấy Sum dữ liệu khách hàng

    //==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    //Hàm khởi tạo phân trang
    self.paging = function () {

        var listPage = [];

        page = page <= 0 ? 1 : page;
        pageTotal = Math.ceil(total / pagesize);
        page > 3 ? self.pageStart(true) : self.pageStart(false);
        page > 4 ? self.pageNext(true) : self.pageNext(false);
        pageTotal - 2 > page ? self.pageEnd(true) : self.pageEnd(false);
        pageTotal - 3 > page ? self.pagePrev(true) : self.pagePrev(false);

        var start = (page - 2) <= 0 ? 1 : (page - 2);
        var end = (page + 2) >= pageTotal ? pageTotal : (page + 2);

        for (var i = start; i <= end; i++) {
            listPage.push({ Page: i });
        }

        self.listPage(listPage);
        self.pageTitle("Show <b>" + (((page - 1) * pagesize) + 1) + "</b> to <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) + "</b> of <b>" + total + "</b> Record" );
    }

    //Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    //==================== Các sự kiện click menu =====================================
    self.clickMenu = function (name) {
        self.active(name);
        self.templateId(name);
        total = 0;
        page = 1;
        pageTotal = 0;
        window.history.pushState('Accountant', '', 'accountant#' + name);
        //Gọi page danh sách nạp/Subtract funds - FundBill
        if (name === 'moneyfund') {
            self.isRending(false);
            self.isLoading(false);
            self.GetAllFundBillList();
            self.searchCustomerFundBill();

            self.isRending(true);
            self.isLoading(true);
            $('#treasureSearch_tree .dropdownjstree').remove();
            $("#treasureSearch_tree").dropdownjstree({
                source: window.treasureSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.SearchFundBillModal().TreasureId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
            $('#financeFundSearch_tree .dropdownjstree').remove();
            $("#financeFundSearch_tree").dropdownjstree({
                source: window.financeFundSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.SearchFundBillModal().FinanceFundId(selected.node.original.id === 0 ? null : selected.node.original.id);
                    self.SearchFundBillModal().FinanceFundIdPath(selected.node.original.idPath === 0 ? null : selected.node.original.idPath);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
        }

        //Gọi page danh sách nạp/trừ tiền ví - RechargeBill
        if (name === 'recharge') {

            self.isRending(false);
            self.isLoading(false);

            self.SearchRechargeBillModal().CustomerId = -1;

            self.GetAllRechargeBillList();
            self.searchCustomerRechargeBill();

            self.isRending(true);
            self.isLoading(true);
            $('#customerWalletSearch_tree .dropdownjstree').remove();
            $("#customerWalletSearch_tree").dropdownjstree({
                source: window.treasureWalletSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.SearchRechargeBillModal().CustomerWalletId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
        }

        //Gọi page danh sách nạp/Subtract funds - FundBill

        if (name === 'debitOrder') {
            self.isRending(false);
            self.isLoading(false);
            self.searchCustomerDebit();
            self.GetAllDebitOrderList();
            self.isRending(true);
            self.isLoading(true);
            $('#treasureDebitSearch_tree .dropdownjstree').remove();
            $("#treasureDebitSearch_tree").dropdownjstree({
                source: window.payReceiveSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn

                    self.SearchDebitModal().TreasureId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
            $('#financeDebitSearch_tree .dropdownjstree').remove();
            $("#financeDebitSearch_tree").dropdownjstree({
                source: window.financeFundSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.SearchDebitModal().FinanceFundId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });

        }

        //Gọi page danh sách nạp/Subtract funds - FundBill
        if (name === 'mustreturn') {
            self.isRending(false);
            self.isLoading(false);

            self.GetAllMustReturnList();
            //self.searchCustomerMustReturn();

            self.isRending(true);
            self.isLoading(true);
        }

        //Gọi page danh sách nạp công nợ
        if (name === 'debit') {
            self.isRending(false);
            self.isLoading(false);

            self.GetAllDebitList();
            self.searchCustomerDebit();

            self.isRending(true);
            self.isLoading(true);
            $('#treasureDebitSearch_tree .dropdownjstree').remove();
            $("#treasureDebitSearch_tree").dropdownjstree({
                source: window.payReceiveSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn

                    self.SearchDebitModal().TreasureId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
            $('#financeDebitSearch_tree .dropdownjstree').remove();
            $("#financeDebitSearch_tree").dropdownjstree({
                source: window.financeFundSearchTree,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.SearchDebitModal().FinanceFundId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
        }

        //Gọi page danh sách nạp/trừ tiền ví - RechargeBill
        if (name === 'withdrawal') {

            self.isRending(false);
            self.isLoading(false);
            self.SearchDrawalModal().CustomerId = -1;

            self.GetAllWithDrawalList();
            self.searchCustomer();

            self.isRending(true);
            self.isLoading(true);
        }

        if (name === 'claimforrefund') {
            self.searchCustomerClaimForRefund();
            self.renderSystem();

            self.GetClaimForRefundList();
        };

        //hỗ trợ khiếu nại
        if (name === 'ticket-support') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystemtab();
            self.GetAllTicketListByStaff();
            //self.ticketSupportViewModel(self.listComplainStatus, self.listComplainSystem, self.listSystemRenderPoCustomer, self.listAllCustomerComplain);
            self.isRending(true);
            self.isLoading(true);
        };

        if (name === 'customerfind') {
            self.searchCustomer();
        }
        //Thống kê quỹ
        if (name === 'fund-report') {

            self.financeFundReport();

            self.fundReport();


            self.fundReportAdd();
            self.fundReportMinus();
        }

        //Thống kê thu chi quỹ theo thời gian
        if (name === 'reportAccountSituation') {
            self.reportDateStart("");
            self.reportDateEnd("");
            self.viewReportAccountSituation();
            $('#financeFundSearch_treeAccount .dropdownjstree').remove();
            $("#financeFundSearch_treeAccount").dropdownjstree({
                source: window.financeFundJsAccountantSearch,
                selectedNode: '',
                selectNote: (node, selected) => { // sự kiện chọn
                    self.FinanceFundId(selected.node.original.id === 0 ? null : selected.node.original.id);
                },
                treeParent: {
                    hover_node: false,
                    select_node: false
                }
            });
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });
        }

        if (name === 'reportAccount') {
            self.reportDateStart("");
            self.reportDateEnd("");
            self.viewReportAccount(1);
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });
        }

        if (name === 'fund-reportSituation') {
            self.reportDateStart("");
            self.reportDateEnd("");
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });
        }

        //Thah toán Orders
        if (name === "accountantOrder") {
            self.accountantOrderViewModel.renderSystem();
        }
        //self.searchCustomer();
        setTimeout(function () {
            self.init();
        },
            1000);
    }

    //Cap nhat thong tin cac bien search tu dong



    self.searchCustomerFundBill = function () {
        $(".subject-search")
            .select2({
                ajax: {
                    url: "Customer/GetSubjectSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    self.fundBillModel().SubjectId(repo.id);
                    self.fundBillModel().SubjectCode(repo.code);
                    self.fundBillModel().SubjectName(repo.text);
                    self.fundBillModel().SubjectPhone(repo.phone);
                    self.fundBillModel().SubjectEmail(repo.email);

                    if (repo.code === undefined || repo.email === undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    self.searchCustomerRechargeBill = function () {
        $(".customer-search")
            .select2({
                ajax: {
                    url: "Customer/GetCustomerSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    self.rechargeBillModel().CustomerId(repo.id);
                    self.rechargeBillModel().CustomerCode(repo.code);
                    self.rechargeBillModel().CustomerName(repo.text);
                    self.rechargeBillModel().CustomerPhone(repo.phone);
                    self.rechargeBillModel().CustomerEmail(repo.email);

                    if (repo.code === undefined || repo.email === undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    self.searchCustomerDebit = function () {
        $(".subject-search")
            .select2({
                ajax: {
                    url: "Customer/GetSubjectSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    self.debitCollectModel().SubjectId(repo.id);
                    self.debitCollectModel().SubjectCode(repo.code);
                    self.debitCollectModel().SubjectName(repo.text);
                    self.debitCollectModel().SubjectPhone(repo.phone);
                    self.debitCollectModel().SubjectEmail(repo.email);


                    if (repo.code === undefined || repo.email === undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    self.searchCustomerMustReturn = function () {
        $(".subject-search")
            .select2({
                ajax: {
                    url: "Customer/GetSubjectSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    self.mustDebitReturnModel().SubjectId(repo.id);
                    self.mustDebitReturnModel().SubjectCode(repo.code);
                    self.mustDebitReturnModel().SubjectName(repo.text);
                    self.mustDebitReturnModel().SubjectPhone(repo.phone);
                    self.mustDebitReturnModel().SubjectEmail(repo.email);

                    if (repo.code === undefined || repo.email === undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };

    self.searchCustomerClaimForRefund = function () {
        $(".customer-search")
            .select2({
                ajax: {
                    url: "Customer/GetCustomerSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    if (repo.code === undefined || repo.email === undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'en'
            });
    };


    //Hàm lấy thông tin khách hàng
    self.searchCustomer = function () {
        $(".customer-search")
            .select2({
                ajax: {
                    url: "Customer/GetCustomerSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    return repo.text;
                },
                placeholder: "All",
                allowClear: true,
                language: 'en'
            });
    };

    self.searchSubject = function () {
        $(".subject-search")
            .select2({
                ajax: {
                    url: "Customer/GetSubjectSearch",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        return {
                            keyword: params.term,
                            page: params.page
                        };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data.items,
                            pagination: {
                                more: (params.page * 10) < data.total_count
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; },
                minimumInputLength: 1,
                templateResult: function (repo) {
                    if (repo.loading) return repo.text;
                    var markup = "<div class='select2-result-repository clearfix'>\
                                    <div class='pull-left'>\
                                        <img class='w-40 mr10 mt5' src='" + repo.avatar + "'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
                                            <i class='fa fa-globe'></i> " + repo.systemName + "<br />\
                                        </div>\
                                    </div>\
                                    <div class='clear-fix'></div>\
                                </div>";
                    return markup;
                },
                templateSelection: function (repo) {
                    return repo.text;
                },
                placeholder: "All",
                allowClear: true,
                language: 'en'
            });
    };


    //hỗ trợ khiếu nại
    self.clickTab = function (tab) {
        self.SearchCustomerModal().SystemId(tab);
        //self.SearchFundBillModal().Status(tab);
        self.SearchFundBillModal().TypeId(tab);
        self.SearchRechargeBillModal().TypeId(tab);
        self.SearchMustReturnModal().Status(tab);
        self.SearchDebitModal().Status(tab);
        self.SearchDrawalModal().Status(tab);

        //hiển thị ở trang đầu tiên
        self.search(1);
        $(".select-view").select2();
    }

    //tab
    self.listSystemRender = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);


    //Hàm load lại dữ liệu trên các tab
    self.renderSystem = function () {
        self.listSystemRender([]);
        self.listStatus([]);

        $.post("/Ticket/GetRenderSystem", { active: self.active() }, function (data) {
            self.listStatus(data.listStatus);
            self.listSystemRender(data.listSystem);

            $('.nav-tabs').tabdrop();
            $(".select-view").select2();
        });
    }

    //==================== Tìm kiếm ===================================================
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);

        if (self.active() === 'moneyfund') {
            self.GetAllFundBillList();
        }

        if (self.active() === 'recharge') {
            self.GetAllRechargeBillList();
        }

        if (self.active() === 'debit') {
            self.GetAllDebitList();
        }
        if (self.active() === 'debitOrder') {
            self.GetAllDebitOrderList();
        }

        if (self.active() === 'mustreturn') {
            self.GetAllMustReturnList();
        }

        if (self.active() === 'withdrawal') {
            self.GetAllWithDrawalList();
        }
        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        };

        if (self.active() === 'ticket-support') {
            self.GetAllTicketListByStaff();
        }

        if (self.active() == 'reportAccount') {
            self.viewReportAccount(page);
        }

        if (name === 'customerfind') {
            self.searchCustomer();
        }

        if (self.active() === 'claimforrefund') {
            self.listClaimForRefundView([]);
            var searchClaimForRefundData = ko.mapping.toJS(self.SearchClaimForRefundModal());
            //thiết lập lại giá trị cho Status nếu status=undefined
            if (searchClaimForRefundData.Status === undefined) {
                searchClaimForRefundData.Status = -1;
            }
            if (searchClaimForRefundData.CustomerId === undefined) {
                searchClaimForRefundData.CustomerId = -1;
            }
            $.post("/Ticket/GetClaimForRefundList", { page: page, pageSize: pagesize, searchModal: searchClaimForRefundData }, function (data) {
                total = data.totalRecord;
                self.listClaimForRefundView(data.claimForRefundModal);
                self.paging();
                self.isRending(true);
                self.isLoading(true);
            });
        }

        if (self.active2() === 'OrderMoney') {
            self.OrderMoney();
        }

        if (self.active2() === 'OrderHistory') {
            self.OrderHistory();
        }
    };

    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);
        page = 1;
        if (self.active() === 'moneyfund') {
            self.GetAllFundBillList();
        }

        if (self.active() === 'recharge') {
            self.GetAllRechargeBillList();
        }


        //if (self.active() === 'debitOrder') {
        //    self.GetAllDebitOrderList();
        //}

        //if (self.active() === 'mustreturn') {
        //    self.GetAllMustReturnList();
        //}

        if (self.active() === 'debit') {
            self.GetAllDebitList();
        }

        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        };
        if (self.active() === 'ticket-support') {
            self.GetAllTicketListByStaff();
        }
        if (self.active() === 'withdrawal') {
            self.GetAllWithDrawalList();
        }

        if (self.active() === 'reportAccount') {
            self.reportDateStart("");
            self.reportDateEnd("");
            self.viewReportAccount(1);
        }

        //self.isRending(true);
        //self.isLoading(false);
    }
    //==================== Các sự kiện show Modal ==========================================
    //============================================== MoneyFund - FundBill
    self.viewMoneyFundDetail = function (data) {
        accountantDetail.viewMoneyFundDetail(data.Id);
        $('#moneyFundDetailModal').modal();
    } // Thông tin Detail FundBill

    self.viewMoneyFundAddOrEditPlus = function () {
        // Nạp tiền
        self.AddOrEdit(0);

        self.fundBillModel(new fundBillDetailModel());
        //lấy thông tin
        self.GetFundBillInitCreateOrEdit();

        self.searchCustomerFundBill();

        $('#financeFund_addtree .dropdownjstree').remove();
        $("#financeFund_addtree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        self.initInputMark();

        $('#moneyFundAddOrEdit').modal();
        $(".customer-fund-bill-model").empty().trigger("change");
    } // Modal show thông tin Tạo/Edit thông tin nạp/rút tiền quỹ công ty

    self.viewMoneyFundMinus = function () {
        // Trừ tiền
        self.AddOrEdit(0);
        self.fundBillModel(new fundBillDetailModel());
        //Lấy thông tin
        self.GetFundBillInitCreateOrEdit();
        self.searchCustomerFundBill();
        $('#financeFund_minustree .dropdownjstree').remove();
        $("#financeFund_minustree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });
        self.initInputMark();
        $('#moneyFundMinus').modal();
        $(".customer-fund-bill-model").empty().trigger("change");
    } // Modal show thông tin Tạo/Edit thông tin nạp/rút tiền quỹ công ty




    self.btnCreateOrEditFundBill = function (data) {
        if (self.AddOrEdit() === 0) {
            self.CreateNewFundBill(data);
        }
        else {
            self.EditFundBill(data);
        }



    } // Button tạo mới giao dịch quỹ
    self.viewFundBillEdit = function (data) {
        self.AddOrEdit(1);
        self.fundBillModel(new fundBillDetailModel());

        self.GetFundBillDetail(data);

        $('#financeFund_addtree .dropdownjstree').remove();

        $("#financeFund_addtree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: self.fundBillModel().FinanceFundId() + '',
            selectNote: (node, selected) => { // sự kiện chọn
                if (self.fundBillModel().FinanceFundId() === selected.node.original.id)
                    return;

                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });

        self.initInputMark();
        $('#moneyFundAddOrEdit').modal("show");

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            maxDate: '0',
            language: 'en'
        });
    }   // Edit thông tin phiếu giao dịch quỹ - FundBill

    self.viewFundBillEditMinus = function (data) {
        self.AddOrEdit(1);
        self.fundBillModel(new fundBillDetailModel());
        //self.customerName = ko.observable('- Select customer -');
        //self.customer = ko.observable(null);
        self.GetFundBillDetail(data);
        $('#financeFund_minustree .dropdownjstree').remove();
        $("#financeFund_minustree").dropdownjstree({
            source: window.financeFundTree,
            selectedNode: self.fundBillModel().FinanceFundId() + '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.fundBillModel().FinanceFundId(selected.node.original.id);
                self.fundBillModel().FinanceFundName(selected.node.original.text);
                self.fundBillModel().FinanceFundBankAccountNumber(selected.node.original.CardId);
                self.fundBillModel().FinanceFundDepartment(selected.node.original.CardBranch);
                self.fundBillModel().FinanceFundNameBank(selected.node.original.CardBank);
                self.fundBillModel().FinanceFundUserFullName(selected.node.original.UserFullName);
                self.fundBillModel().FinanceFundUserPhone(selected.node.original.UserPhone);
                self.fundBillModel().FinanceFundUserEmail(selected.node.original.UserEmail);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });
        
        self.initInputMark();
        $('#moneyFundMinus').modal();
        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            maxDate: '0',
             language: 'en'
        });

    }   // Edit thông tin phiếu giao dịch quỹ - FundBill

    self.GetFundBillSearchData = function () {
        self.listType([]);
        self.listStatus([]);
        self.listFinanceFund([]);
        self.listAccountantSubject([]);

        $.post("/FundBill/GetFundBillSearchData", {}, function (data) {
            self.listType(data.listType);
            self.listFinanceFund(data.listFinanceFund);
            self.listAccountantSubject(data.listAccountantSubject);
            self.listStatus(data.listStatus);
        });

    } // Hàm khởi tạo lấy thông tin SearchData

    self.GetAllFundBillList = function () {
        self.listFundBill([]);
        var searchFundBillData = ko.mapping.toJS(self.SearchFundBillModal());
        self.totalCollectMoney(0);
        self.totalReturnMoney(0);
        self.totalMinusMoney(0);
        if (searchFundBillData.TypeId == undefined) {
            searchFundBillData.TypeId = -1;
        }
        if (searchFundBillData.AccountantSubjectId == undefined) {
            searchFundBillData.AccountantSubjectId = -1;
        }
        if (searchFundBillData.Status == undefined) {
            searchFundBillData.Status = -1;
        }
        if (searchFundBillData.TreasureId == undefined) {
            searchFundBillData.TreasureId = 0;
        }
        if (searchFundBillData.FinanceFundId == undefined) {
            searchFundBillData.FinanceFundId = 0;
        }
        if (searchFundBillData.FinanceFundIdPath == undefined) {
            searchFundBillData.FinanceFundIdPath = '0';
        }
        searchFundBillData.CurrencyFluctuations >= 0 ? Globalize.parseFloat(searchFundBillData.CurrencyFluctuations) : "";
        $.post("/FundBill/GetAllFundBillList", { page: page, pageSize: pagesize, searchModal: searchFundBillData }, function (data) {
            total = data.totalRecord;
            self.listFundBill(data.fundBillModal);
            self.paging();

            self.totalCollectMoney(data.collectMoney);
            self.totalReturnMoney(data.returnMoney);
            self.totalMinusMoney(data.minusMoney);
            self.initInputMark();
            self.isRending(true);
            self.isLoading(true);
        });
    }   // Lấy danh sách FundBill

    self.GetFundBillDetail = function (data) {
        $.ajax({
            url: "/FundBill/GetFundBillDetail",
            global: false,
            type: 'POST',
            data: { funBillId: data.Id },
            async: false, //blocks window close
            success: function (result) {
                self.mapFundBillModel(result.fundBillModal);
            }
        });

        // $.post("/FundBill/GetFundBillDetail", { funBillId: data.Id }, function (result) {
        //     self.mapFundBillModel(result.fundBillModal);
        // });
    } // Lấy Detail nạp/chi tiền quỹ - FundBill

    self.CreateNewFundBill = function (data) {
        var fundBillData = ko.mapping.toJS(self.fundBillModel());

        if (self.PlusOrMinus() === 0) {
            fundBillData.Type = 0;
        }
        else {
            fundBillData.Type = 1;
        }

        // Khởi tạo trạng thái nạp tiền ví
        fundBillData.Id = 0;
        fundBillData.Code = "";
        fundBillData.Status = 0;
        fundBillData.LastUpdated = moment(new Date()).format();
        fundBillData.CurrencyFluctuations = formatVN(fundBillData.CurrencyFluctuations);

        $.post("/FundBill/CreateNewFundBill", { model: fundBillData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.fundBillModel(new fundBillDetailModel());
                $(".customer-fund-bill-model").empty().trigger("change");
                self.GetAllFundBillList();
            }
        });
    } // Tạo mới FundBill

    self.EditFundBill = function (data) {
        var fundBillData = ko.mapping.toJS(self.fundBillModel());
        fundBillData.CurrencyFluctuations = formatVN(fundBillData.CurrencyFluctuations);
        $.post("/FundBill/EditFundBill", { model: fundBillData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllFundBillList();
            }
        });
    } //Edit thông tin FundBill

    self.ApprovalFundBill = function (data) {
        $.post("/FundBill/ApprovalFundBill", { fundBillId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllFundBillList();
            }
        });
    } // Duyệt thông tin FundBill

    self.DeleteFundBill = function (data) {
        $.post("/FundBill/DeleteFundBill", { fundBillId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllFundBillList();
            }
        });
    } // Xóa FundBill

    self.GetFundBillInitCreateOrEdit = function () {

        self.listFinanceFund([]);
        self.listTreasure([]);

        $.post("/FundBill/GetFundBillInitCreateOrEdit", {}, function (response) {

            self.listFinanceFund(response.listFinanceFund);
            self.listTreasure(response.listTreasure);
        });
    } // Lấy danh sách khởi tạo trên Form Tạo/Edit FundBill
    //============================================== /MoneyFund

    //============================================= RechargeBill
    self.viewRechargeAddOrEditPlus = function () {
        self.AddOrEdit(0);
        self.PlusOrMinus(0);
        self.rechargeBillModel(new rechargeBillDetailModel());
        self.searchCustomerRechargeBill();
        self.initInputMark();
        $('#rechargeAddOrEditPlus').modal();
        $(".customer-recharge-bill-model").empty().trigger("change");
    }

    self.viewRechargeEditPlus = function (data) {
        self.AddOrEdit(1);
        self.PlusOrMinus(0);
        self.customerName = ko.observable('- Select customer -');
        self.customer = ko.observable(null);

        self.GetRechargeBillDetail(data);

        self.initInputMark();
        $('#rechargeAddOrEditPlus').modal();


    }   //Edit thông tin phiếu giao dịch ví - RechargeBill

    self.viewRechargeAddOrEditMinus = function () {
        self.AddOrEdit(0);
        self.PlusOrMinus(1);
        self.rechargeBillModel(new rechargeBillDetailModel());
        self.searchCustomerRechargeBill();
        self.initInputMark();
        $('#rechargeAddOrEditMinus').modal();
        $(".customer-recharge-bill-model").empty().trigger("change");

    }   // Modal show thông tin tạo/Edit RechargeBill

    self.viewRechargeEditMinus = function (data) {
        self.AddOrEdit(1);
        self.PlusOrMinus(1);
        self.customerName = ko.observable('- Select customer -');
        self.customer = ko.observable(null);

        self.GetRechargeBillDetail(data);

        self.initInputMark();
        $('#rechargeAddOrEditMinus').modal();


    }   //Edit thông tin phiếu giao dịch ví - RechargeBill

    self.viewRechargeBillDetail = function (data) {
        self.GetRechargeBillDetail(data);
        $('#rechargeDetailModal').modal();
    }   // Modal show thông tin Detail RechargeBill

    self.btnCreateOrEditRechargeBill = function (data) {
        if (self.AddOrEdit() === 0) {
            self.CreateNewRechargeBill(data);
        }
        else {
            self.EditRechargeBill(data);
        }

        self.rechargeBillModel(new rechargeBillDetailModel());
        $(".customer-recharge-bill-model").empty().trigger("change");
    }   // Button tạo mới giao dịch ví RechargeBill

    self.GetRechargeBillSearchData = function () {
        self.listType([]);
        self.listStatus([]);
        self.listCustomer([]);

        $.post("/RechargeBill/GetRechargeBillSearchData", {}, function (data) {
            self.listType(data.listType);
            self.listCustomer(data.listCustomer);
            self.listStatus(data.listStatus);
        });
    }   // Hàm khởi tạo lấy thông tin SearchData

    self.GetAllRechargeBillList = function () {
        self.listRechargeBill([]);
        var SearchRechargeBillData = ko.mapping.toJS(self.SearchRechargeBillModal());
        self.totalCollectMoney(0);
        self.totalReturnMoney(0);
        self.totalMinusMoney(0);
        if (SearchRechargeBillData.TypeId == undefined) {
            SearchRechargeBillData.TypeId = -1;
        }
        if (SearchRechargeBillData.Status == undefined) {
            SearchRechargeBillData.Status = -1;
        }
        SearchRechargeBillData.CurrencyFluctuations >= 0 ? Globalize.parseFloat(SearchRechargeBillData.CurrencyFluctuations) : "";
        $.post("/RechargeBill/GetAllRechargeBillList", { page: page, pageSize: pagesize, searchModal: SearchRechargeBillData }, function (data) {
            total = data.totalRecord;
            self.listRechargeBill(data.rechargeBillModal);
            self.paging();

            self.totalCollectMoney(data.collectMoney);
            self.totalReturnMoney(data.returnMoney);
            self.totalMinusMoney(data.minusMoney);
            self.initInputMark();
            self.isRending(true);
            self.isLoading(false);
        });
    }      // Lấy danh sách RechargeBill

    self.GetRechargeBillDetail = function (data) {
        self.rechargeBillModel(new rechargeBillDetailModel());

        $.post("/RechargeBill/GetRechargeBillDetail", { rechargeBillId: data.Id }, function (result) {
            self.mapRechargeBillModel(result.rechargeBillModal);
        });
    }   // Lấy Detail nạp/chi tiền quỹ RechargeBill

    self.CreateNewRechargeBill = function (data) {
        var rechargeData = ko.mapping.toJS(self.rechargeBillModel());
        rechargeData.CurrencyFluctuations = formatVN(rechargeData.CurrencyFluctuations);

        if (self.PlusOrMinus() === 0) {
            rechargeData.Type = 0;
        }
        else {
            rechargeData.Type = 1;
        }

        // Khởi tạo trạng thái nạp tiền ví
        rechargeData.Id = 0;
        rechargeData.Status = 0;

        $.post("/RechargeBill/CreateNewRechargeBill", { model: rechargeData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllRechargeBillList();
            }
        });
    }   // Tạo mới RechargeBill

    self.EditRechargeBill = function (data) {
        var rechargeData = ko.mapping.toJS(self.rechargeBillModel());
        rechargeData.CurrencyFluctuations = formatVN(rechargeData.CurrencyFluctuations);

        $.post("/RechargeBill/EditRechargeBill", { model: rechargeData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllRechargeBillList();
            }
        });
    }        // Edit thông tin RechargeBill

    self.ApprovalRechargeBill = function (data) {

        $.post("/RechargeBill/ApprovalRechargeBill", { rechargeBillId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllRechargeBillList();
            }
        });
    }    // Duyệt giao dịch

    self.DeleteRechargeBill = function (data) {
        $.post("/RechargeBill/DeleteRechargeBill", { rechargeBillId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllRechargeBillList();
            }
        });
    }       // Xóa phiếu RechargeBill
    //============================================= /RechargeBill

    //============================================= Debit
    self.viewDebitDetail = function (data) {
        self.GetDebitDetail(data);
        $('#debitDetailModal').modal();
    }   // Modal show thông tin Detail Debit

    self.viewMoneyFundAddCollect = function () {
        self.DebitOption(0);
        self.AddOrEdit(0);

        self.debitCollectModel(new debitHistoryModel());
        $("#debitCollect_treasure_tree .dropdownjstree").remove();
        $("#debitCollect_treasure_tree").dropdownjstree({
            source: window.treasureCollectJsTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.debitCollectModel().PayReceivableId(selected.node.original.id);
                self.debitCollectModel().PayReceivableIName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });
        self.GetDebitInitCreateCollectOrEdit();
        self.searchCustomerDebit();
        self.initInputMark();
        $('#mustCollectDebitAddOrEdit').modal();
        $(".customer-collect-model").empty().trigger("change");
    } // Hien thị Modal thêm phiêu công nợ phai thu

    self.viewDebitCollectEdit = function (data) {
        self.DebitOption(1);
        self.AddOrEdit(1);
        self.debitCollectModel(new debitHistoryModel());
        self.getDebitHistoryDetail(data);
        self.initInputMark();
        $('#mustCollectDebitAddOrEdit').modal();

    } // Edit thông tin phiếu công nợ phải thu - Debit


    self.viewDebitHistoryDetail = function (data) {
        self.debitCollectModel(new debitHistoryModel());
        self.getDebitHistoryDetail(data);
        self.initInputMark();
        $('#debitHistoryDetailModal').modal();
    } //Hien thi chi tiet debithistory


    self.btnUpdateDebitHistory = function (data) {
        var DebitData = ko.mapping.toJS(self.debitCollectModel());

        $.post("/Debit/EditDebitHistory", { model: DebitData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
            }
        });
    } // Thuc hien Cap nhat so tien trong debithistory


    self.btnDeleteDebitHistory = function (data) {
        $.post("/Debit/DeleteDebitHistory", { debitHistoryId: data }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
            }
        });
    } // Thuc hien xoá  debithistory

    self.viewDebitCollectExecute = function () {
        self.DebitOption(1);
        self.AddOrEdit(0);

        self.debitCollectModel(new debitHistoryModel());
        self.GetDebitInitCreateCollectOrEdit();
        self.GetDebitDetail(data);

        $('#mustCollectDebitAddOrEdit').modal();
    }

    self.btnCreateCollectOrEditDebit = function (data) {
        if (self.AddOrEdit() === 0) {
            self.CreateNewCollectDebit(data);
        }
        else {
            self.EditDebitCollect(data);
        }


    }   // Button tạo mới giao dịch ví - Debit

    self.btnCreateReturnOrEditDebit = function (data) {
        if (self.AddOrEdit() === 0) {
            self.CreateNewCollectDebit(data);
        }
        else {
            self.EditDebitReturn(data);
        }


    }   // Button tạo mới giao dịch ví - Debit

    self.GetAllDebitList = function () {
        self.listDebit([]);
        var SearchDebitData = ko.mapping.toJS(self.SearchDebitModal());
        if (SearchDebitData.SubjectTypeId == undefined) {
            SearchDebitData.SubjectTypeId = -1;
        }
        if (SearchDebitData.Status == undefined) {
            SearchDebitData.Status = -1;
        }
        if (SearchDebitData.Type == undefined) {
            SearchDebitData.Type = -1;
        }
        if (SearchDebitData.TreasureId == undefined) {
            SearchDebitData.TreasureId = 0;
        }
        if (SearchDebitData.FinanceFundId == undefined) {
            SearchDebitData.FinanceFundId = 0;
        }

        $.post("/Debit/GetAllDebitList", { page: page, pageSize: pagesize, searchModal: SearchDebitData }, function (data) {
            total = data.totalRecord;
            self.listDebit(data.debitModal);

            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }       // Lấy danh sách Debit


    self.GetAllDebitOrderList = function () {
        self.listDebitOrder([]);
        var SearchDebitData = ko.mapping.toJS(self.SearchDebitModal());

        $.post("/Debit/GetAllDebitOrderList", { page: page, pageSize: pagesize, searchModal: SearchDebitData }, function (data) {
            total = data.totalRecord;
            self.listDebitOrder(data.debitModal);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }       // Lấy danh sách Debit cua Order

    self.GetDebitDetail = function (data) {
        $.post("/Debit/GetDebitDetail", { debitId: data.Id }, function (result) {
            self.mapDebitDetailModel(result.debitModal);
        });
    }    // Lấy Detail công nợ phải thu - Debit

    self.GetDebitSearchData = function () {
        self.listSubjectType([]);
        self.listSubject([]);
        self.listOffice([]);
        self.listStatus([]);

        $.post("/Debit/GetDebitSearchData", {}, function (data) {
            self.listSubjectType(data.listSubjectType);
            self.listSubject(data.listSubject);
            self.listOffice(data.listOffice);
            self.listStatus(data.listStatus);
        });
    }    // Lấy toàn bộ danh đổ lên Form Search - Debit

    self.CreateNewCollectDebit = function (data) {
        var DebitData = ko.mapping.toJS(self.debitCollectModel());

        // Khởi tạo trạng thái nạp tiền ví
        DebitData.Id = 0;
        DebitData.Status = 0;

        $.post("/Debit/CreateNewHandDebit", { model: DebitData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
                self.debitCollectModel(new debitHistoryModel());
                $(".customer-collect-model").empty().trigger("change");
            }
        });
    }   // Tạo mới Debit

    self.EditDebitCollect = function (data) {
        var DebitData = ko.mapping.toJS(self.debitCollectModel());
        self.EditDebit(DebitData);

    }

    self.EditDebitReturn = function (data) {
        var DebitData = ko.mapping.toJS(self.mustDebitReturnModel());
        self.EditDebit(DebitData);

    }
    self.EditDebit = function (data) {
        $.post("/Debit/EditDebitHistory", { model: data }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
            }
        });
    }        // Edit thông tin Debit

    self.DeleteDebit = function (data) {
        $.post("/Debit/DeleteDebit", { DebitId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
            }
        });
    }       // Xóa phiếu Debit

    self.ApprovalDebit = function (data) {
        $.post("/Debit/ApprovalDebit", { DebitId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
            }
        });
    }    // Duyệt giao dịch

    self.GetDebitInitCreateCollectOrEdit = function () {
        self.listSubjectCollect([]);
        self.listFinanceFund([]);
        self.listTreasure([]);

        $.post("/Debit/GetDebitInitCreateOrEdit", {}, function (response) {
            self.listSubjectCollect(response.listAccountantSubject);
            self.listFinanceFund(response.listFinanceFund);
            self.listTreasure(response.listTreasure);
        });
    } // Lấy danh sách khởi tạo trên Form Tạo/Edit Debit
    //============================================= /Debit

    //======== Xử lý công nợ phai thu quỹ ======
    self.viewExecuteMustCollect = function (Id) {

        self.getDebitHistoryDetail(Id);
        $('#executeMustCollectDebit').modal();
    }


    self.ExecuteDebitMustCollect = function () {
        var debitMustCollect = ko.mapping.toJS(self.debitCollectModel());
        debitMustCollect.Type = 0;

        $.post("/Debit/ExecuteDebitMustCollect", { model: debitMustCollect }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
                self.debitCollectModel(new debitHistoryModel());
                $(".customer-collect-model").empty().trigger("change");
            }
        });
    }

    //======== Xử lý công nợ phai thu ví ======
    self.viewExecuteMustCollectWallet = function (Id) {

        self.getDebitHistoryDetail(Id);
        self.initInputMark();
        $('#executeMustCollectWallet').modal();
    }


    self.ExecuteWalletMustCollect = function () {
        var debitMustCollect = ko.mapping.toJS(self.debitCollectModel());
        debitMustCollect.Type = 0;
        $.post("/Debit/ExecuteDebitWalletMustCollect", { model: debitMustCollect }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
                self.debitCollectModel(new debitHistoryModel());
                $(".customer-collect-model").empty().trigger("change");
            }
        });
    }

    //======== Xử lý công nợ phai trả quỹ ==========
    self.viewExecuteMustReturn = function (Id) {
        self.getDebitHistoryDetailReturn(Id);
        $('#executeMustReturnDebit').modal();
    }

    self.ExecuteDebitMustReturn = function () {
        var debitMustReturn = ko.mapping.toJS(self.mustDebitReturnModel());
        debitMustReturn.Type = 1;

        $.post("/Debit/ExecuteDebitMustReturn", { model: debitMustReturn }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
                self.mustDebitReturnModel(new debitHistoryModel());
                $(".customer-collect-model").empty().trigger("change");
            }
        });
    }

    //======== Xử lý công nợ phai trả ví ==========
    self.viewExecuteMustReturnWallet = function (Id) {
        self.getDebitHistoryDetailReturn(Id);
        self.initInputMark();
        $('#executeMustReturnWallet').modal();
    }

    self.ExecuteWalletMustReturn = function () {
        var debitMustReturn = ko.mapping.toJS(self.mustDebitReturnModel());
        debitMustReturn.Type = 1;

        $.post("/Debit/ExecuteDebitWalletMustReturn", { model: debitMustReturn }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllDebitList();
                self.mustDebitReturnModel(new debitHistoryModel());
            }
        });
    }


    self.getDebitHistoryDetail = function (Id) {
        $.post("/Debit/GetDebitHistoryDetail", { debitId: Id }, function (result) {
            self.mapDebitModel(result.debitModal);
        });
    }


    self.getDebitHistoryDetailReturn = function (Id) {
        $.post("/Debit/GetDebitHistoryDetail", { debitId: Id }, function (result) {

            self.mapDebitMustReturnModel(result.debitModal);
        });
    }
    //============================================= MustReturn ==================================
    self.viewMustReturnDetail = function (data) {
        self.GetMustReturnDetail(data);
        $('#mustReturnDetailModal').modal();
    }   // Modal show thông tin Detail MustReturn

    self.viewMoneyFundAddOrReturn = function () {
        self.mustReturnOption(0);
        self.AddOrEdit(0);

        self.mustDebitReturnModel(new debitHistoryModel());
        self.searchCustomerMustReturn();
        //self.GetMustReturnInitCreateOrEdit();
        //Nhập định khoản trên công nợ phải trả
        $("#mustreturn_treasure_tree .dropdownjstree").remove();
        $("#mustreturn_treasure_tree").dropdownjstree({
            source: window.treasureReturnJsTree,
            selectedNode: '',
            selectNote: (node, selected) => { // sự kiện chọn
                self.mustDebitReturnModel().PayReceivableId(selected.node.original.id);
                self.mustDebitReturnModel().PayReceivableIName(selected.node.original.text);
            },
            treeParent: {
                hover_node: false,
                select_node: false
            }
        });
        self.initInputMark();
        $('#mustReturnDebitAddOrEdit').modal();

        $(".customer-mustreturn-model").empty().trigger("change");
    }

    self.viewDebitReturnEdit = function (data) {
        self.DebitOption(1);
        self.AddOrEdit(1);
        self.debitCollectModel(new debitHistoryModel());

        //self.GetMustReturnInitCreateOrEdit();


        self.getDebitHistoryDetailReturn(data);
        self.initInputMark();
        $('#mustReturnDebitAddOrEdit').modal();
    }       // Edit thông tin phiếu công nợ phải thu - MustReturn

    self.viewMustReturnExecute = function () {
        self.mustReturnOption(1);
        self.AddOrEdit(0);

        self.mustReturnModel(new mustReturnDetailModel());
        //self.GetMustReturnInitCreateOrEdit();
        self.GetMustReturnDetail(data);

        $('#mustReturnAddOrEdit').modal();
    }

    self.btnCreateOrEditMustReturn = function (data) {
        if (self.AddOrEdit() === 0) {
            self.CreateNewMustReturn(data);
        }
        else {
            self.EditMustReturn(data);
        }

        self.mustReturnModel(new debitDetailModel());
        $(".customer-mustreturn-model").empty().trigger("change");
    }   // Button tạo mới giao dịch ví - MustReturn

    self.GetAllMustReturnList = function () {
        self.listMustReturn([]);
        var SearchMustReturnData = ko.mapping.toJS(self.SearchMustReturnModal());

        $.post("/MustReturn/GetAllMustReturnList", { page: page, pageSize: pagesize, searchModal: SearchMustReturnData }, function (data) {
            total = data.totalRecord;
            self.listMustReturn(data.MustReturnModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }   // Lấy danh sách MustReturn

    self.GetMustReturnDetail = function (data) {
        $.post("/MustReturn/GetMustReturnDetail", { mustReturnId: data.Id }, function (result) {
            self.mapMustReturnModel(result.mustReturnModal);

        });
    }   // Lấy Detail công nợ phải trả MustReturn

    self.GetMustReturnSearchData = function () {
        self.listSubjectType([]);
        self.listSubject([]);
        self.listOffice([]);
        self.listStatus([]);

        $.post("/MustReturn/GetMustReturnSearchData", {}, function (data) {
            self.listSubjectType(data.listSubjectType);
            self.listSubject(data.listSubject);
            self.listOffice(data.listOffice);
            self.listStatus(data.listStatus);
        });
    }    // Lấy toàn bộ danh đổ lên Form Search MustReturn

    self.CreateNewMustReturn = function (data) {
        var mustReturnData = ko.mapping.toJS(self.mustDebitReturnModel());

        // Khởi tạo trạng thái nạp tiền ví
        mustReturnData.Id = 0;
        mustReturnData.Status = 0;

        $.post("/Debit/CreateNewHandDebit", { model: mustReturnData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.viewMoneyFundAddOrReturn();
            }
        });
    }   // Tạo mới MustReturn

    self.EditMustReturn = function (data) {
        var mustReturnData = ko.mapping.toJS(self.mustReturnModel());

        $.post("/MustReturn/EditMustReturn", { model: mustReturnData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                sself.viewMoneyFundAddOrReturn();
            }
        });
    }        // Edit thông tin MustReturn

    self.DeleteMustReturn = function (data) {
        $.post("/MustReturn/DeleteMustReturn", { mustReturnId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllMustReturnList();
            }
        });
    }       // Xóa phiếu MustReturn

    self.ApprovalMustReturn = function (data) {
        $.post("/MustReturn/ApprovalMustReturn", { mustReturnId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllMustReturnList();
            }
        });
    }    // Duyệt giao dịch MustReturn

    self.GetMustReturnInitCreateOrEdit = function () {
        self.listAccountantSubject([]);
        self.listFinanceFund([]);
        self.listTreasure([]);

        $.post("/MustReturn/GetMustReturnInitCreateOrEdit", {}, function (response) {
            self.listAccountantSubject(response.listAccountantSubject);
            self.listFinanceFund(response.listFinanceFund);
            self.listTreasure(response.listTreasure);
        });
    } // Lấy danh sách khởi tạo trên Form Tạo/Edit Debit
    //============================================= /MustReturn

    //============================================= Drawal

    self.GetWithDrawalSearchData = function () {
        self.listStatus([]);
        self.listCustomer([]);
        self.listUser([]);

        $.post("/WithDrawal/GetWithDrawalSearchData", {}, function (data) {
            self.listStatus(data.listStatus);
            self.listCustomer(data.listCustomer);
            self.listUser(data.listUser);
        });
    }

    self.GetAllWithDrawalList = function () {
        self.listDrawal([]);
        var SearchDrawalData = ko.mapping.toJS(self.SearchDrawalModal());
        if (SearchDrawalData.CustomerId === undefined) {
            SearchDrawalData.CustomerId = -1;
        }
        if (SearchDrawalData.UserId === undefined) {
            SearchDrawalData.UserId = -1;
        }
        if (SearchDrawalData.Status === undefined) {
            SearchDrawalData.Status = -1;
        }

        $.post("/WithDrawal/GetAllWithDrawalList", { page: page, pageSize: pagesize, searchModal: SearchDrawalData }, function (data) {
            total = data.totalRecord;
            self.listDrawal(data.drawalModal);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }   // Lấy danh sách WithDrawal

    // Hien thi chi tiet WithDrawal
    self.GetWithDrawalDetail = function (data) {
        $.post("/WithDrawal/GetWithDrawalDetail", { drawalId: data.Id }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            }
            else {
                self.mapWithDrawalDetailModel(result.withDrawalModal);
                $('#moneyWithdrawalDetailModal').modal();

            }

        });
    }

    self.DeleteWithDrawal = function (data) {
        $.post("/WithDrawal/DeleteWithDrawal", { drawalId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllWithDrawalList();
            }
        });
    }

    //============================================= /Drawal

    //============================================= ClaimForRefund

    self.reasonClaimId = ko.observable();
    self.reasonClaim = ko.observable();
    self.listClaimForRefund = ko.observableArray([]);
    self.order = ko.observable(new orderModel());
    self.avatar = ko.observable();
    self.levelName = ko.observable();
    self.listStatusRefund = ko.observableArray([]);
    self.GetClaimForRefundList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Request a refund for the customer");
        self.listClaimForRefundView([]);

        var searchClaimForRefundData = ko.mapping.toJS(self.SearchClaimForRefundModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (searchClaimForRefundData.Status === undefined) {
            searchClaimForRefundData.Status = -1;
        }
        if (searchClaimForRefundData.CustomerId === undefined) {
            searchClaimForRefundData.CustomerId = -1;
        }
        if (searchClaimForRefundData.UserId === undefined) {
            searchClaimForRefundData.UserId = -1;
        }
        $.post("/Accountant/GetClaimForRefundList", { page: page, pageSize: pagesize, searchModal: searchClaimForRefundData }, function (data) {
            total = data.totalRecord;
            self.listClaimForRefundView(data.claimForRefundModal);

            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    //Hiển thị Detail khiếu nại
    self.totalClaim = ko.observable(0);
    //Khai báo biến thông tin Cấp Level Vip
    self.vipOrder = ko.observable(0);
    self.vipShip = ko.observable(0);
    self.vipName = ko.observable("");

    self.btnViewClaimForRefundDetail = function (data) {
        self.listClaimForRefundDetail([]);
        self.isDetailRending(false);
        self.complainModel(new complainModel());
        self.vipOrder(0);
        self.vipShip(0);
        self.vipName("");
        $.post("/Ticket/GetClaimForRefundDetail", { claimForRefundId: data.Id }, function (result) {
            self.isDetailRending(true);
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                self.listOrderService(result.orderService);
                if (result.orderDetail != null) {
                    self.mapObject(result.orderDetail)
                }
                if (result.levelVip != null) {
                    self.vipOrder(result.levelVip.Order);
                    self.vipShip(result.levelVip.Ship);
                    self.vipName(result.levelVip.Name);
                }
                self.mapComplainModel(result.ticket);
                self.mapClaimForRefund(result.claimForRefundViewModel.ClaimForRefund);

                _.each(result.claimForRefundViewModel.LstClaimForRefundDetails,
                      function (it) {
                          it.TotalQuantityFailed = formatNumberic(it.TotalQuantityFailed, 'N2');
                      });
                self.listClaimForRefundDetail(result.claimForRefundViewModel.LstClaimForRefundDetails);
                self.totalAllClaimForRefundDetail();
                self.initInputMark();
                $('#claimForRefundDetail').modal();
            }
        });
    }

    self.totalAllClaimForRefundDetail = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefundDetail(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed); });
        self.totalClaim(formatNumberic(total, 'N2'));
    }

    self.btnViewExecuteClaimForRefund = function (data) {
        self.fundBillModel(new fundBillDetailModel());
        //lấy thông tin
        self.GetFundBillInitCreateOrEdit();

        $.post("/Ticket/GetClaimForRefundDetail", { claimForRefundId: data.Id }, function (result) {
            self.isDetailRending(true);
            self.mapClaimForRefund(result.claimForRefundViewModel.ClaimForRefund);
        });

        self.initInputMark();
        $('#ExecuteClaimForRefund').modal();
        $(".customer-fund-bill-model").empty().trigger("change");
    }

    self.btnExecuteClaimForRefund = function () {
        $.post("/Accountant/ExecuteClaimForRefund", { claimForRefund: self.claimForRefund() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
                $('#ExecuteClaimForRefund').hide();
            }
        });
    }
    //Quay lại trạng thái trước đó
    self.btnViewRefundMoneyModalBack = function (id) {
        $.post("/Accountant/ViewRefundMoneyModalBack", { claimId: id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }
    //Hàm hủy phiếu yêu cầu Refund
    self.viewDeleteClaimForRefund = function (data) {
        self.reasonClaimId(data.Id);
        $('#commentCancelClaim').modal();
    }

    self.btnDeleteClaimForRefund = function (data) {
        $.post("/Ticket/DeleteClaimForRefund", { claimForRefundId: self.reasonClaimId(), content: self.reasonClaim() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }

    //trao đổi với khách hàng
    self.feedbackComplainModal = function () {
        $('#commentForCustomer').modal();
    }
    //hàm trao đổi giữa các nhân viên
    self.feedbackUser = function () {
        var content = $('#feedback').val();
        if (content === "" || content === null) {
            $('#requireUser').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: content, objectChat: true }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    self.GetTicketDetail(self.complainModel().Id());
                    $('#feedback').val("");
                }
            });
        }
    };

    //Hàm map dữ liệu
    self.mapObject = function (data) {
        self.order(new orderModel());

        self.order().Id(data.Id);
        self.order().Code(data.Code);
        self.order().Type(data.Type);
        self.order().WebsiteName(data.WebsiteName);
        self.order().ShopId(data.ShopId);
        self.order().ShopName(data.ShopName);
        self.order().ShopLink(data.ShopLink);
        self.order().ProductNo(formatNumberic(data.ProductNo, 'N0'));
        self.order().PackageNo(formatNumberic(data.PackageNo, 'N0'));
        self.order().ContractCode(data.ContractCode);
        self.order().ContractCodes(data.ContractCodes);
        self.order().LevelId(data.LevelId);
        self.order().LevelName(data.LevelName);
        self.order().TotalWeight(formatNumberic(data.TotalWeight, 'N2'));
        self.order().DiscountType(data.DiscountType);
        self.order().DiscountValue(data.DiscountValue);
        self.order().GiftCode(data.GiftCode);
        self.order().CreatedTool(data.CreatedTool);
        self.order().Currency(formatNumberic(data.Currency, 'N2'));
        self.order().ExchangeRate(formatNumberic(data.ExchangeRate, 'N2'));
        self.order().TotalExchange(formatNumberic(data.TotalExchange, 'N2'));
        //self.order().TotalPrice(formatNumberic(data.TotalPrice, 'N2'));
        self.order().Total(formatNumberic(data.Total, 'N2'));
        self.order().HashTag(data.HashTag);
        self.order().WarehouseId(data.WarehouseId);
        self.order().WarehouseName(data.WarehouseName);
        self.order().CustomerId(data.CustomerId);
        self.order().CustomerName(data.CustomerName);
        self.order().CustomerEmail(data.CustomerEmail);
        self.order().CustomerPhone(data.CustomerPhone);
        self.order().CustomerAddress(data.CustomerAddress);
        self.order().Status(data.Status);
        self.order().UserId(data.UserId);
        self.order().UserFullName(data.UserFullName);
        self.order().OfficeId(data.OfficeId);
        self.order().OfficeName(data.OfficeName);
        self.order().OfficeIdPath(data.OfficeIdPath);
        self.order().CreatedOfficeIdPath(data.CreatedOfficeIdPath);
        self.order().CreatedUserId(data.CreatedUserId);
        self.order().CreatedUserFullName(data.CreatedUserFullName);
        self.order().CreatedOfficeId(data.CreatedOfficeId);
        self.order().CreatedOfficeName(data.CreatedOfficeName);
        self.order().OrderInfoId(data.OrderInfoId);
        self.order().FromAddressId(data.FromAddressId);
        self.order().ToAddressId(data.ToAddressId);
        self.order().SystemId(data.SystemId);
        self.order().SystemName(data.SystemName);
        self.order().ServiceType(data.ServiceType);
        self.order().Note(data.Note);
        self.order().PrivateNote(data.PrivateNote);
        self.order().LinkNo(data.LinkNo);
        self.order().IsDelete(data.IsDelete);
        self.order().Created(data.Created);
        self.order().LastUpdate(data.LastUpdate);
        self.order().StatusName(statusApp.order[data.Status].Name);
        self.order().StatusClass(statusApp.order[data.Status].Class);
        self.order().ReasonCancel(data.ReasonCancel);
        self.order().PriceBargain(formatNumberic(data.PriceBargain === null ? 0 : data.PriceBargain, 'N2'));
        self.order().ExpectedDate(data.ExpectedDate);
        self.order().PaidShop(formatNumberic(data.PaidShop === null ? 0 : data.PaidShop, 'N2'));
        self.order().TotalShop(self.order().PaidShop());
        self.order().FeeShip(formatNumberic(data.FeeShip === null ? 0 : data.FeeShip, 'N2'));
        self.order().FeeShipBargain(formatNumberic(data.FeeShipBargain === null ? 0 : data.FeeShipBargain, 'N2'));
        self.order().UserNote(data.UserNote);
        self.order().TotalPayed(formatNumberic(data.TotalPayed === null ? 0 : data.TotalPayed, 'N2'));
        self.order().TotalPrice(formatNumberic(data.TotalPrice === null ? 0 : data.TotalPrice, 'N2'));
        self.order().DepositPercent(data.DepositPercent);

        self.order().IsPayWarehouseShip(data.IsPayWarehouseShip);

        self.order().TotalPriceCustomer(formatNumberic(Globalize.parseFloat(self.order().TotalPrice()) + Globalize.parseFloat(self.order().FeeShip()), 'N2'));
    }


    //==================== Object Map dữ liệu trả về View =========================================
    // Object FundBill - Detail phiếu nạp/Subtract funds
    self.mapFundBillModel = function (data) {
        self.fundBillModel(new fundBillDetailModel());

        self.fundBillModel().Id(data.Id);
        self.fundBillModel().Code(data.Code);
        self.fundBillModel().Type(data.Type);
        self.fundBillModel().TypeName(statusApp.typeFundBill[data.Type].Name);
        self.fundBillModel().TypeClass(statusApp.typeFundBill[data.Type].Class);
        self.fundBillModel().Status(data.Status);
        self.fundBillModel().CurrencyFluctuations(formatNumbericCN(data.CurrencyFluctuations, 'N2'));
        self.fundBillModel().Increase(data.Increase);
        self.fundBillModel().Diminishe(data.Diminishe);
        self.fundBillModel().CurencyStart(data.CurencyStart);
        self.fundBillModel().CurencyEnd(data.CurencyEnd);
        self.fundBillModel().AccountantSubjectId(data.AccountantSubjectId);
        self.fundBillModel().AccountantSubjectName(data.AccountantSubjectName);
        self.fundBillModel().SubjectId(data.SubjectId);
        self.fundBillModel().SubjectCode(data.SubjectCode);
        self.fundBillModel().SubjectName(data.SubjectName);
        self.fundBillModel().SubjectPhone(data.SubjectPhone);
        self.fundBillModel().SubjectEmail(data.SubjectEmail);
        self.fundBillModel().SubjectAddress(data.SubjectAddress);

        self.fundBillModel().FinanceFundId(data.FinanceFundId);
        self.fundBillModel().FinanceFundName(data.FinanceFundName);
        self.fundBillModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.fundBillModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.fundBillModel().FinanceFundNameBank(data.FinanceFundNameBank);

        self.fundBillModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.fundBillModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.fundBillModel().FinanceFundUserEmail(data.FinanceFundUserEmail);

        self.fundBillModel().IsDelete(data.IsDelete);
        self.fundBillModel().TreasureId(data.TreasureId);
        self.fundBillModel().TreasureName(data.TreasureName);
        self.fundBillModel().Note(data.Note);
        self.fundBillModel().UserId(data.UserId);
        self.fundBillModel().UserCode(data.UserCode);
        self.fundBillModel().UserName(data.UserName);
        self.fundBillModel().UserApprovalId(data.UserApprovalId);
        self.fundBillModel().UserApprovalCode(data.UserApprovalCode);
        self.fundBillModel().UserApprovalName(data.UserApprovalName);

        self.fundBillModel().OrderId(data.OrderId);
        self.fundBillModel().OrderCode(data.OrderCode);
        self.fundBillModel().OrderType(data.OrderType);

        self.fundBillModel().Created(data.Created);
        if (data.LastUpdated == null || data.LastUpdated == undefined || data.LastUpdated == '') {
            //self.potentialCustomerModel().Birthday(moment(new Date()).format('DD/MM/YYYY'));
        } else {
            self.fundBillModel().LastUpdated(moment(data.LastUpdated).format('DD/MM/YYYY'));
        }
        //self.fundBillModel().LastUpdated(data.LastUpdated);

    }

    // Object RechargeBill - Detail phiếu nạp/trừ tiền ví điện tử khách hàng
    self.mapRechargeBillModel = function (data) {
        self.rechargeBillModel(new rechargeBillDetailModel());

        self.rechargeBillModel().Id(data.Id);
        self.rechargeBillModel().Code(data.Code);
        self.rechargeBillModel().Type(data.Type);
        self.rechargeBillModel().TypeName(statusApp.typeRechargeBill[data.Type].Name);
        self.rechargeBillModel().TypeClass(statusApp.typeRechargeBill[data.Type].Class);
        self.rechargeBillModel().Status(data.Status);
        self.rechargeBillModel().Note(data.Note);
        self.rechargeBillModel().CurrencyFluctuations(formatNumbericCN(data.CurrencyFluctuations, 'N2'));
        self.rechargeBillModel().Increase(data.Increase);
        self.rechargeBillModel().Diminishe(data.Diminishe);

        self.rechargeBillModel().CurencyStart(data.CurencyStart);
        self.rechargeBillModel().CurencyEnd(data.CurencyEnd);
        self.rechargeBillModel().UserId(data.UserId);
        self.rechargeBillModel().UserCode(data.UserCode);
        self.rechargeBillModel().UserName(data.UserName);
        self.rechargeBillModel().UserApprovalId(data.UserApprovalId);
        self.rechargeBillModel().UserApprovalCode(data.UserApprovalCode);
        self.rechargeBillModel().UserApprovalName(data.UserApprovalName);
        self.rechargeBillModel().CustomerId(data.CustomerId);
        self.rechargeBillModel().CustomerCode(data.CustomerCode);
        self.rechargeBillModel().CustomerName(data.CustomerName);
        self.rechargeBillModel().CustomerPhone(data.CustomerPhone);
        self.rechargeBillModel().CustomerEmail(data.CustomerEmail);
        self.rechargeBillModel().CustomerAddress(data.CustomerAddress);
        self.rechargeBillModel().IsDelete(data.IsDelete);
        self.rechargeBillModel().TreasureId(data.TreasureId);
        self.rechargeBillModel().TreasureName(data.TreasureName);
        self.rechargeBillModel().TreasureIdd(data.TreasureIdd);
        self.rechargeBillModel().IsAutomatic(data.IsAutomatic);
        self.rechargeBillModel().OrderId(data.OrderId);
        self.rechargeBillModel().OrderCode(data.OrderCode);
        self.rechargeBillModel().OrderType(data.OrderType);
        self.rechargeBillModel().Created(data.Created);
        self.rechargeBillModel().LastUpdated(data.LastUpdated);

    }

    // Object Debit - Detail công nợ phải thu
    self.mapDebitModel = function (data) {
        self.debitCollectModel(new debitHistoryModel());

        self.debitCollectModel().Id(data.Id);
        self.debitCollectModel().Code(data.Code);

        self.debitCollectModel().Status(data.Status);
        self.debitCollectModel().Note(data.Note);
        self.debitCollectModel().DebitId(data.DebitId);
        self.debitCollectModel().DebitType(data.DebitType);
        self.debitCollectModel().DebitCode(data.DebitCode);
        self.debitCollectModel().Money(data.Money);
        self.debitCollectModel().OrderId(data.OrderId);
        self.debitCollectModel().OrderType(data.OrderType);
        self.debitCollectModel().OrderCode(data.OrderCode);
        self.debitCollectModel().PayReceivableId(data.PayReceivableId);
        self.debitCollectModel().PayReceivableIdd(data.PayReceivableIdd);
        self.debitCollectModel().PayReceivableIName(data.PayReceivableIName);
        self.debitCollectModel().IsSystem(data.IsSystem);
        self.debitCollectModel().SubjectId(data.SubjectId);
        self.debitCollectModel().SubjectCode(data.SubjectCode);
        self.debitCollectModel().SubjectName(data.SubjectName);
        self.debitCollectModel().SubjectPhone(data.SubjectPhone);
        self.debitCollectModel().SubjectEmail(data.SubjectEmail);
        self.debitCollectModel().SubjectAddress(data.SubjectAddress);

        self.debitCollectModel().FinanceFundId(data.FinanceFundId);
        self.debitCollectModel().FinanceFundName(data.FinanceFundName);
        self.debitCollectModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.debitCollectModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.debitCollectModel().FinanceFundNameBank(data.FinanceFundNameBank);
        self.debitCollectModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.debitCollectModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.debitCollectModel().FinanceFundUserEmail(data.FinanceFundUserEmail);
        self.debitCollectModel().TreasureId(data.TreasureId);
        self.debitCollectModel().TreasureName(data.TreasureName);


        self.debitCollectModel().Created(data.Created);
        self.debitCollectModel().LastUpdated(data.LastUpdated);

        self.debitCollectModel().TypeHistoryName(statusApp.typeDebit[data.DebitType].Name);
        self.debitCollectModel().TypeHistoryClass(statusApp.typeDebit[data.DebitType].Class);

        self.debitCollectModel().StatusHistoryName(statusApp.StatusDebitHistory[data.Status].Name);
        self.debitCollectModel().StatusHistoryClass(statusApp.StatusDebitHistory[data.Status].Class);

    }

    //Object Debit - Detail công nợ 
    self.mapDebitDetailModel = function (data) {

        self.debitModel().Id(data.Id);
        self.debitModel().Code(data.Code);
        self.debitModel().Status(data.Status);
        self.debitModel().Note(data.Note);
        self.debitModel().MustCollectMoney(data.MustCollectMoney);
        self.debitModel().MustReturnMoney(data.MustReturnMoney);
        self.debitModel().TreasureId(data.TreasureId);
        self.debitModel().TreasureIdd(data.TreasureIdd);
        self.debitModel().TreasureName(data.TreasureName);
        self.debitModel().FinanceFundId(data.FinanceFundId);
        self.debitModel().FinanceFundName(data.FinanceFundName);
        self.debitModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.debitModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.debitModel().FinanceFundNameBank(data.FinanceFundNameBank);
        self.debitModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.debitModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.debitModel().FinanceFundUserEmail(data.FinanceFundUserEmail);
        self.debitModel().SubjectTypeId(data.SubjectTypeId);
        self.debitModel().SubjectTypeName(data.SubjectTypeName);
        self.debitModel().AccountantSubjectId(data.AccountantSubjectId);
        self.debitModel().AccountantSubjectName(data.AccountantSubjectName);
        self.debitModel().SubjectId(data.SubjectId);
        self.debitModel().SubjectCode(data.SubjectCode);
        self.debitModel().SubjectName(data.SubjectName);
        self.debitModel().SubjectPhone(data.SubjectPhone);
        self.debitModel().SubjectEmail(data.SubjectEmail);
        self.debitModel().SubjectAddress(data.SubjectAddress);
        self.debitModel().OrderId(data.OrderId);
        self.debitModel().OrderType(data.OrderType);
        self.debitModel().OrderCode(data.OrderCode);
        self.debitModel().UserId(data.UserId);
        self.debitModel().UserCode(data.UserCode);
        self.debitModel().UserName(data.UserName);
        self.debitModel().UserApprovalId(data.UserApprovalId);
        self.debitModel().UserApprovalCode(data.UserApprovalCode);
        self.debitModel().UserApprovalName(data.UserApprovalName);
        self.debitModel().IsSystem(data.IsSystem);
        self.debitModel().Created(data.Created);
        self.debitModel().LastUpdated(data.LastUpdated);
        self.debitModel().IsDelete(data.IsDelete);
    }

    // Object MustReturn - Detail công nợ phải trả
    self.mapMustReturnModel = function (data) {
        self.mustReturnModel(new mustReturnDetailModel());

        self.mustReturnModel().Id(data.Id);
        self.mustReturnModel().Code(data.Code);
        self.mustReturnModel().Note(data.Note);

        self.mustReturnModel().Status(data.Status);
        self.mustReturnModel().StatusName(statusApp.statusMustReturn[data.Status].Name);
        self.mustReturnModel().StatusClass(statusApp.statusMustReturn[data.Status].Class);

        self.mustReturnModel().CurrencyFluctuations(data.CurrencyFluctuations);
        self.mustReturnModel().CurrencyDiscount(data.CurrencyDiscount);
        self.mustReturnModel().CurrencyReal(data.CurrencyReal);
        self.mustReturnModel().TreasureId(data.TreasureId);
        self.mustReturnModel().TreasureName(data.TreasureName);
        self.mustReturnModel().AccountantSubjectId(data.AccountantSubjectId);
        self.mustReturnModel().AccountantSubjectName(data.AccountantSubjectName);

        self.mustReturnModel().FinanceFundId(data.FinanceFundId);
        self.mustReturnModel().FinanceFundName(data.FinanceFundName);
        self.mustReturnModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.mustReturnModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.mustReturnModel().FinanceFundNameBank(data.FinanceFundNameBank);
        self.mustReturnModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.mustReturnModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.mustReturnModel().FinanceFundUserEmail(data.FinanceFundUserEmail);

        self.mustReturnModel().SubjectTypeId(data.SubjectTypeId);
        self.mustReturnModel().SubjectTypeName(data.SubjectTypeName);
        self.mustReturnModel().SubjectId(data.SubjectId);
        self.mustReturnModel().SubjectCode(data.SubjectCode);
        self.mustReturnModel().SubjectName(data.SubjectName);
        self.mustReturnModel().SubjectPhone(data.SubjectPhone);
        self.mustReturnModel().SubjectEmail(data.SubjectEmail);
        self.mustReturnModel().SubjectAddress(data.SubjectAddress);
        self.mustReturnModel().OfficeId(data.OfficeId);
        self.mustReturnModel().OfficeCode(data.OfficeCode);
        self.mustReturnModel().OfficeName(data.OfficeName);
        self.mustReturnModel().UserId(data.UserId);
        self.mustReturnModel().UserCode(data.UserCode);
        self.mustReturnModel().UserName(data.UserName);
        self.mustReturnModel().UserApprovalId(data.UserApprovalId);
        self.mustReturnModel().UserApprovalCode(data.UserApprovalCode);
        self.mustReturnModel().UserApprovalName(data.UserApprovalName);
        self.mustReturnModel().Created(data.Created);
        self.mustReturnModel().LastUpdated(data.LastUpdated);
        self.mustReturnModel().IsDelete(data.IsDelete);
    }
    //DebitHistory
    self.mapDebitMustReturnModel = function (data) {
        self.mustDebitReturnModel(new debitHistoryModel());
        self.mustDebitReturnModel().Id(data.Id);
        self.mustDebitReturnModel().Code(data.Code);
        self.mustDebitReturnModel().Note(data.Note);
        //self.mustDebitReturnModel().Type(data.Type);
        self.mustDebitReturnModel().Status(data.Status);
        self.mustDebitReturnModel().DebitId(data.DebitId);
        self.mustDebitReturnModel().DebitType(data.DebitType);
        self.mustDebitReturnModel().DebitCode(data.DebitCode);
        self.mustDebitReturnModel().Money(data.Money);
        self.mustDebitReturnModel().OrderId(data.OrderId);
        self.mustDebitReturnModel().OrderType(data.OrderType);
        self.mustDebitReturnModel().OrderCode(data.OrderCode);
        self.mustDebitReturnModel().PayReceivableId(data.PayReceivableId);
        self.mustDebitReturnModel().PayReceivableIdd(data.PayReceivableIdd);
        self.mustDebitReturnModel().PayReceivableIName(data.PayReceivableIName);
        self.mustDebitReturnModel().IsSystem(data.IsSystem);
        self.mustDebitReturnModel().SubjectId(data.SubjectId);
        self.mustDebitReturnModel().SubjectCode(data.SubjectCode);
        self.mustDebitReturnModel().SubjectName(data.SubjectName);
        self.mustDebitReturnModel().SubjectPhone(data.SubjectPhone);
        self.mustDebitReturnModel().SubjectEmail(data.SubjectEmail);
        self.mustDebitReturnModel().SubjectAddress(data.SubjectAddress);

        self.mustDebitReturnModel().FinanceFundId(data.FinanceFundId);
        self.mustDebitReturnModel().FinanceFundName(data.FinanceFundName);
        self.mustDebitReturnModel().FinanceFundBankAccountNumber(data.FinanceFundBankAccountNumber);
        self.mustDebitReturnModel().FinanceFundDepartment(data.FinanceFundDepartment);
        self.mustDebitReturnModel().FinanceFundNameBank(data.FinanceFundNameBank);
        self.mustDebitReturnModel().FinanceFundUserFullName(data.FinanceFundUserFullName);
        self.mustDebitReturnModel().FinanceFundUserPhone(data.FinanceFundUserPhone);
        self.mustDebitReturnModel().FinanceFundUserEmail(data.FinanceFundUserEmail);
        self.mustDebitReturnModel().TreasureId(data.TreasureId);
        self.mustDebitReturnModel().TreasureName(data.TreasureName);

        self.mustDebitReturnModel().Created(data.Created);
        self.mustDebitReturnModel().LastUpdated(data.LastUpdated);

        self.mustDebitReturnModel().TypeHistoryName(statusApp.typeDebit[data.DebitType].Name);
        self.mustDebitReturnModel().TypeHistoryClass(statusApp.typeDebit[data.DebitType].Class);

        self.mustDebitReturnModel().StatusHistoryName(statusApp.StatusDebitHistory[data.Status].Name);
        self.mustDebitReturnModel().StatusHistoryClass(statusApp.StatusDebitHistory[data.Status].Class);
    }

    self.mapClaimForRefund = function (data) {
        self.claimForRefund(new ClaimForRefund());

        self.claimForRefund().Id(data.Id);
        self.claimForRefund().Code(data.Code);
        self.claimForRefund().OrderId(data.OrderId);
        self.claimForRefund().OrderCode(data.OrderCode);
        self.claimForRefund().OrderType(data.OrderType);
        self.claimForRefund().Status(data.Status);
        self.claimForRefund().CustomerId(data.CustomerId);
        self.claimForRefund().CustomerCode(data.CustomerCode);
        self.claimForRefund().CustomerFullName(data.CustomerFullName);
        self.claimForRefund().CustomerPhone(data.CustomerPhone);
        self.claimForRefund().CustomerEmail(data.CustomerEmail);
        self.claimForRefund().CustomerAddress(data.CustomerAddress);
        self.claimForRefund().CustomerOfficeId(data.CustomerOfficeId);
        self.claimForRefund().CustomerOfficeName(data.CustomerOfficeName);
        self.claimForRefund().CustomerOfficePath(data.CustomerOfficePath);
        self.claimForRefund().OrderUserId(data.OrderUserId);
        self.claimForRefund().OrderUserCode(data.OrderUserCode);
        self.claimForRefund().OrderUserFullName(data.OrderUserFullName);
        self.claimForRefund().OrderUserEmail(data.OrderUserEmail);
        self.claimForRefund().OrderUserPhone(data.OrderUserPhone);
        self.claimForRefund().OrderUserOfficeId(data.OrderUserOfficeId);
        self.claimForRefund().OrderUserOfficeName(data.OrderUserOfficeName);
        self.claimForRefund().OrderUserOfficePath(data.OrderUserOfficePath);
        self.claimForRefund().SupportId(data.SupportId);
        self.claimForRefund().SupportCode(data.SupportCode);
        self.claimForRefund().SupportFullName(data.SupportFullName);
        self.claimForRefund().SupportEmail(data.SupportEmail);
        self.claimForRefund().AccountantId(data.AccountantId);
        self.claimForRefund().AccountantCode(data.AccountantCode);
        self.claimForRefund().AccountantFullName(data.AccountantFullName);
        self.claimForRefund().AccountantEmail(data.AccountantEmail);
        self.claimForRefund().MoneyRefund(data.MoneyRefund);
        self.claimForRefund().RealTotalRefund(data.RealTotalRefund);
        if (data.MoneyOrderRefundDicker != null) {
            self.claimForRefund().MoneyOrderRefundDicker(formatNumberic(data.MoneyOrderRefundDicker, 'N2'));
        }
        else {
            self.claimForRefund().MoneyOrderRefundDicker(data.MoneyOrderRefund);
        }
        //self.claimForRefund().MoneyOrderRefundDicker(data.MoneyOrderRefundDicker);
        if (data.MoneyOrderRefund != null) {
            self.claimForRefund().MoneyOrderRefund(formatNumberic(data.MoneyOrderRefund, 'N2'));
        }
        else {
            self.claimForRefund().MoneyOrderRefund(data.MoneyOrderRefund);
        }
        self.claimForRefund().ExchangeRate(data.ExchangeRate);
        self.claimForRefund().UserId(data.UserId);
        self.claimForRefund().UserCode(data.UserCode);
        self.claimForRefund().UserName(data.UserName);
        self.claimForRefund().UserEmail(data.UserEmail);
        self.claimForRefund().UserPhone(data.UserPhone);
        self.claimForRefund().OfficeId(data.OfficeId);
        self.claimForRefund().OfficeName(data.OfficeName);
        self.claimForRefund().OfficeIdPath(data.OfficeIdPath);
        self.claimForRefund().IsDelete(data.IsDelete);
        self.claimForRefund().Created(data.Created);
        self.claimForRefund().LastUpdated(data.LastUpdated);
        self.claimForRefund().NoteOrderer(data.NoteOrderer);
        self.claimForRefund().NoteSupporter(data.NoteSupporter);
        self.claimForRefund().NoteAccountanter(data.NoteAccountanter);
        self.claimForRefund().TicketId(data.TicketId);
        self.claimForRefund().TicketCode(data.TicketCode);
        self.claimForRefund().SupporterMoneyRequest(data.SupporterMoneyRequest);
        self.claimForRefund().CurrencyDiscount(data.CurrencyDiscount);
        self.claimForRefund().MoneyOther(data.MoneyOther);
    }
    // Object Detail ClaimForRefundDetail
    self.mapClaimForRefundDetailModel = function (data) {
        self.claimForRefundDetailModel(new ClaimForRefundDetailModel());

        self.claimForRefundDetailModel().Id(data.Id);
        self.claimForRefundDetailModel().Code(data.Code);
        self.claimForRefundDetailModel().OrderId(data.OrderId);
        self.claimForRefundDetailModel().OrderCode(data.OrderCode);
        self.claimForRefundDetailModel().OrderType(data.OrderType);
        self.claimForRefundDetailModel().Status(data.Status);
        self.claimForRefundDetailModel().CustomerId(data.CustomerId);
        self.claimForRefundDetailModel().CustomerCode(data.CustomerCode);
        self.claimForRefundDetailModel().CustomerFullName(data.CustomerFullName);
        self.claimForRefundDetailModel().CustomerPhone(data.CustomerPhone);
        self.claimForRefundDetailModel().CustomerEmail(data.CustomerEmail);
        self.claimForRefundDetailModel().CustomerAddress(data.CustomerAddress);
        self.claimForRefundDetailModel().CustomerOfficeId(data.CustomerOfficeId);
        self.claimForRefundDetailModel().CustomerOfficeName(data.CustomerOfficeName);
        self.claimForRefundDetailModel().CustomerOfficePath(data.CustomerOfficePath);
        self.claimForRefundDetailModel().OrderUserId(data.OrderUserId);
        self.claimForRefundDetailModel().OrderUserCode(data.OrderUserCode);
        self.claimForRefundDetailModel().OrderUserFullName(data.OrderUserFullName);
        self.claimForRefundDetailModel().OrderUserEmail(data.OrderUserEmail);
        self.claimForRefundDetailModel().OrderUserPhone(data.OrderUserPhone);
        self.claimForRefundDetailModel().OrderUserOfficeId(data.OrderUserOfficeId);
        self.claimForRefundDetailModel().OrderUserOfficeName(data.OrderUserOfficeName);
        self.claimForRefundDetailModel().OrderUserOfficePath(data.OrderUserOfficePath);
        self.claimForRefundDetailModel().SupportId(data.SupportId);
        self.claimForRefundDetailModel().SupportCode(data.SupportCode);
        self.claimForRefundDetailModel().SupportFullName(data.SupportFullName);
        self.claimForRefundDetailModel().SupportEmail(data.SupportEmail);
        self.claimForRefundDetailModel().AccountantId(data.AccountantId);
        self.claimForRefundDetailModel().AccountantCode(data.AccountantCode);
        self.claimForRefundDetailModel().AccountantFullName(data.AccountantFullName);
        self.claimForRefundDetailModel().AccountantEmail(data.AccountantEmail);
        self.claimForRefundDetailModel().MoneyRefund(data.MoneyRefund);
        self.claimForRefundDetailModel().ExchangeRate(data.ExchangeRate);
        self.claimForRefundDetailModel().UserId(data.UserId);
        self.claimForRefundDetailModel().UserCode(data.UserCode);
        self.claimForRefundDetailModel().UserName(data.UserName);
        self.claimForRefundDetailModel().UserEmail(data.UserEmail);
        self.claimForRefundDetailModel().UserPhone(data.UserPhone);
        self.claimForRefundDetailModel().OfficeId(data.OfficeId);
        self.claimForRefundDetailModel().OfficeName(data.OfficeName);
        self.claimForRefundDetailModel().OfficeIdPath(data.OfficeIdPath);
        self.claimForRefundDetailModel().Created(data.Created);
        self.claimForRefundDetailModel().LastUpdated(data.LastUpdated);
    };
    //=====================hỗ trợ khiếu nại==============

    // Object Detail khiếu nại
    self.mapComplainModel = function (data) {
        self.complainModel(new complainModel());

        self.complainModel().Id(data.Id);
        self.complainModel().Code(data.Code);
        self.complainModel().TypeOrder(data.TypeOrder);
        self.complainModel().TypeServiceName(data.TypeServiceName);
        self.complainModel().TypeServiceClose(data.TypeServiceClose);
        self.complainModel().TypeServiceCloseName(data.TypeServiceCloseName);
        self.complainModel().TypeService(data.TypeService);
        self.complainModel().ImagePath1(data.ImagePath1);
        self.complainModel().ImagePath2(data.ImagePath2);
        self.complainModel().ImagePath3(data.ImagePath3);
        self.complainModel().ImagePath4(data.ImagePath4);
        self.complainModel().ImagePath5(data.ImagePath5);
        self.complainModel().ImagePath6(data.ImagePath6);
        self.complainModel().Content(data.Content);
        self.complainModel().OrderId(data.OrderId);
        self.complainModel().OrderCode(data.OrderCode);
        self.complainModel().OrderType(data.OrderType);
        self.complainModel().CustomerId(data.CustomerId);
        self.complainModel().CustomerName(data.CustomerName);
        self.complainModel().CreateDate(data.CreateDate);
        self.complainModel().LastUpdateDate(data.LastUpdateDate);
        self.complainModel().SystemId(data.SystemId);
        self.complainModel().SystemName(data.SystemName);
        self.complainModel().Status(data.Status);
        self.complainModel().LastReply(data.LastReply);
        self.complainModel().BigMoney(data.BigMoney);
        self.complainModel().IsDelete(data.IsDelete);
        self.complainModel().RequestMoney(data.RequestMoney);


    };
    //// Object Detail ComplainUser
    self.mapcomplainDetailModel = function (data) {
        self.complainDetailModel(new complainDetailModel());
        self.complainDetailModel().Id(data.Id);
        self.complainDetailModel().ComplainId(data.ComplainId);
        self.complainDetailModel().UserId(data.UserId);
        self.complainDetailModel().Content(data.Content);
        self.complainDetailModel().AttachFile(data.AttachFile);
        self.complainDetailModel().CreateDate(data.CreateDate);
        self.complainDetailModel().UpdateDate(data.UpdateDate);
        self.complainDetailModel().UserRequestId(data.UserRequestId);
        self.complainDetailModel().UserRequestName(data.UserRequestName);
        self.complainDetailModel().CustomerId(data.CustomerId);
        self.complainDetailModel().CustomerName(data.CustomerName);
        self.complainDetailModel().UserName(data.UserName);
        self.complainDetailModel().IsRead(data.IsRead);
        self.complainDetailModel().UserPosition(data.UserPosition);
    };

    self.withDrawalDetailModel = ko.observable(new withDrawalDetailModel());

    self.mapWithDrawalDetailModel = function (data) {
        self.withDrawalDetailModel(new withDrawalDetailModel());
        self.withDrawalDetailModel().Id(data.Id);
        self.withDrawalDetailModel().Code(data.Code);
        self.withDrawalDetailModel().CustomerId(data.CustomerId);
        self.withDrawalDetailModel().CustomerName(data.CustomerName);
        self.withDrawalDetailModel().CustomerCode(data.CustomerCode);
        self.withDrawalDetailModel().CustomerEmail(data.CustomerEmail);
        self.withDrawalDetailModel().CustomerPhone(data.CustomerPhone);
        self.withDrawalDetailModel().CardName(data.CardName);
        self.withDrawalDetailModel().CardId(data.CardId);
        self.withDrawalDetailModel().CardBank(data.CardBank);
        self.withDrawalDetailModel().CardBranch(data.CardBranch);
        self.withDrawalDetailModel().CreateDate(data.CreateDate);
        self.withDrawalDetailModel().LastUpdate(data.LastUpdate);
        self.withDrawalDetailModel().UserId(data.UserId);
        self.withDrawalDetailModel().UserName(data.UserName);
        self.withDrawalDetailModel().Status(data.Status);
        self.withDrawalDetailModel().Note(data.Note);
        self.withDrawalDetailModel().AdvanceMoney(data.AdvanceMoney);
        self.withDrawalDetailModel().SystemId(data.SystemId);
        self.withDrawalDetailModel().SystemName(data.SystemName);

    }


    //============================================================= THỐNG KÊ QUỸ ===============================================

    //== Thống kê: Tình hình tiền còn lại trong các quỹ
    self.financeFundReport = function () {
        $.post("/Accountant/FinanceFundReport", function (data) {
            self.isLoading(true);
            $('#financefund').highcharts({
                chart: {
                    type: 'bar'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: data.fundName,
                    title: {
                        text: null
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Tiền (Baht)',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify'
                    }
                },
                tooltip: {
                    valueSuffix: ' (Baht)'
                },
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'top',
                    x: -40,
                    y: 80,
                    floating: true,
                    borderWidth: 1,
                    backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                    shadow: true
                },
                credits: {
                    enabled: false
                },
                series: [{
                    name: 'Available balance',
                    data: data.fundData
                }]
            });
        });
    }

    self.fundReport = function () {
        $.post("/Accountant/FundReport", function (data) {
            self.isLoading(true);
            $('#fundbill').highcharts({
                chart: {
                    type: 'bar'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: data.fundName,
                    title: {
                        text: null
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Money (Baht)',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify'
                    }
                },
                tooltip: {
                    valueSuffix: ' (Baht)'
                },
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'top',
                    x: -40,
                    y: 80,
                    floating: true,
                    borderWidth: 1,
                    backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                    shadow: true
                },
                credits: {
                    enabled: false
                },
                series: [{
                    name: 'Deposit fund',
                    data: data.fundAdd
                }, {
                    name: 'Disburse fund',
                    data: data.fundMinus
                }]
            });
        });
    }

    self.fundReportAdd = function () {
        $(function () {

            // Create the chart
            $('#fundbill_add').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Nạp tiền quỹ trong ngày'
                },
                xAxis: {
                    type: 'category'
                },
                yAxis: {
                    title: {
                        text: 'Total percent market share'
                    }

                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    series: {
                        borderWidth: 0,
                        dataLabels: {
                            enabled: true,
                            format: '{point.y:.1f}%'
                        }
                    }
                },

                tooltip: {
                    headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                    pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
                },

                series: [{
                    name: 'Brands',
                    colorByPoint: true,
                    data: [{
                        name: 'Microsoft Internet Explorer',
                        y: 56.33,
                        drilldown: 'Microsoft Internet Explorer'
                    }, {
                        name: 'Chrome',
                        y: 24.03,
                        drilldown: 'Chrome'
                    }, {
                        name: 'Firefox',
                        y: 10.38,
                        drilldown: 'Firefox'
                    }, {
                        name: 'Safari',
                        y: 4.77,
                        drilldown: 'Safari'
                    }, {
                        name: 'Opera',
                        y: 0.91,
                        drilldown: 'Opera'
                    }, {
                        name: 'Proprietary or Undetectable',
                        y: 0.2,
                        drilldown: null
                    }]
                }],
                drilldown: {
                    series: [{
                        name: 'Microsoft Internet Explorer',
                        id: 'Microsoft Internet Explorer',
                        data: [
                            [
                                'v11.0',
                                24.13
                            ],
                            [
                                'v8.0',
                                17.2
                            ],
                            [
                                'v9.0',
                                8.11
                            ],
                            [
                                'v10.0',
                                5.33
                            ],
                            [
                                'v6.0',
                                1.06
                            ],
                            [
                                'v7.0',
                                0.5
                            ]
                        ]
                    }, {
                        name: 'Chrome',
                        id: 'Chrome',
                        data: [
                            [
                                'v40.0',
                                5
                            ],
                            [
                                'v41.0',
                                4.32
                            ],
                            [
                                'v42.0',
                                3.68
                            ],
                            [
                                'v39.0',
                                2.96
                            ],
                            [
                                'v36.0',
                                2.53
                            ],
                            [
                                'v43.0',
                                1.45
                            ],
                            [
                                'v31.0',
                                1.24
                            ],
                            [
                                'v35.0',
                                0.85
                            ],
                            [
                                'v38.0',
                                0.6
                            ],
                            [
                                'v32.0',
                                0.55
                            ],
                            [
                                'v37.0',
                                0.38
                            ],
                            [
                                'v33.0',
                                0.19
                            ],
                            [
                                'v34.0',
                                0.14
                            ],
                            [
                                'v30.0',
                                0.14
                            ]
                        ]
                    }, {
                        name: 'Firefox',
                        id: 'Firefox',
                        data: [
                            [
                                'v35',
                                2.76
                            ],
                            [
                                'v36',
                                2.32
                            ],
                            [
                                'v37',
                                2.31
                            ],
                            [
                                'v34',
                                1.27
                            ],
                            [
                                'v38',
                                1.02
                            ],
                            [
                                'v31',
                                0.33
                            ],
                            [
                                'v33',
                                0.22
                            ],
                            [
                                'v32',
                                0.15
                            ]
                        ]
                    }, {
                        name: 'Safari',
                        id: 'Safari',
                        data: [
                            [
                                'v8.0',
                                2.56
                            ],
                            [
                                'v7.1',
                                0.77
                            ],
                            [
                                'v5.1',
                                0.42
                            ],
                            [
                                'v5.0',
                                0.3
                            ],
                            [
                                'v6.1',
                                0.29
                            ],
                            [
                                'v7.0',
                                0.26
                            ],
                            [
                                'v6.2',
                                0.17
                            ]
                        ]
                    }, {
                        name: 'Opera',
                        id: 'Opera',
                        data: [
                            [
                                'v12.x',
                                0.34
                            ],
                            [
                                'v28',
                                0.24
                            ],
                            [
                                'v27',
                                0.17
                            ],
                            [
                                'v29',
                                0.16
                            ]
                        ]
                    }]
                }
            });
        });


    }



    self.fundReportMinus = function () {
        $(function () {
            // Create the chart
            $('#fundbill_minus').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Within-day fund disbursement'
                },
                xAxis: {
                    type: 'category'
                },
                yAxis: {
                    title: {
                        text: 'Total percent market share'
                    }

                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    series: {
                        borderWidth: 0,
                        dataLabels: {
                            enabled: true,
                            format: '{point.y:.1f}%'
                        }
                    }
                },

                tooltip: {
                    headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                    pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
                },

                series: [{
                    name: 'Brands',
                    colorByPoint: true,
                    data: [{
                        name: 'Microsoft Internet Explorer',
                        y: 56.33,
                        drilldown: 'Microsoft Internet Explorer'
                    }, {
                        name: 'Chrome',
                        y: 24.03,
                        drilldown: 'Chrome'
                    }, {
                        name: 'Firefox',
                        y: 10.38,
                        drilldown: 'Firefox'
                    }, {
                        name: 'Safari',
                        y: 4.77,
                        drilldown: 'Safari'
                    }, {
                        name: 'Opera',
                        y: 0.91,
                        drilldown: 'Opera'
                    }, {
                        name: 'Proprietary or Undetectable',
                        y: 0.2,
                        drilldown: null
                    }]
                }],
                drilldown: {
                    series: [{
                        name: 'Microsoft Internet Explorer',
                        id: 'Microsoft Internet Explorer',
                        data: [
                            [
                                'v11.0',
                                24.13
                            ],
                            [
                                'v8.0',
                                17.2
                            ],
                            [
                                'v9.0',
                                8.11
                            ],
                            [
                                'v10.0',
                                5.33
                            ],
                            [
                                'v6.0',
                                1.06
                            ],
                            [
                                'v7.0',
                                0.5
                            ]
                        ]
                    }, {
                        name: 'Chrome',
                        id: 'Chrome',
                        data: [
                            [
                                'v40.0',
                                5
                            ],
                            [
                                'v41.0',
                                4.32
                            ],
                            [
                                'v42.0',
                                3.68
                            ],
                            [
                                'v39.0',
                                2.96
                            ],
                            [
                                'v36.0',
                                2.53
                            ],
                            [
                                'v43.0',
                                1.45
                            ],
                            [
                                'v31.0',
                                1.24
                            ],
                            [
                                'v35.0',
                                0.85
                            ],
                            [
                                'v38.0',
                                0.6
                            ],
                            [
                                'v32.0',
                                0.55
                            ],
                            [
                                'v37.0',
                                0.38
                            ],
                            [
                                'v33.0',
                                0.19
                            ],
                            [
                                'v34.0',
                                0.14
                            ],
                            [
                                'v30.0',
                                0.14
                            ]
                        ]
                    }, {
                        name: 'Firefox',
                        id: 'Firefox',
                        data: [
                            [
                                'v35',
                                2.76
                            ],
                            [
                                'v36',
                                2.32
                            ],
                            [
                                'v37',
                                2.31
                            ],
                            [
                                'v34',
                                1.27
                            ],
                            [
                                'v38',
                                1.02
                            ],
                            [
                                'v31',
                                0.33
                            ],
                            [
                                'v33',
                                0.22
                            ],
                            [
                                'v32',
                                0.15
                            ]
                        ]
                    }, {
                        name: 'Safari',
                        id: 'Safari',
                        data: [
                            [
                                'v8.0',
                                2.56
                            ],
                            [
                                'v7.1',
                                0.77
                            ],
                            [
                                'v5.1',
                                0.42
                            ],
                            [
                                'v5.0',
                                0.3
                            ],
                            [
                                'v6.1',
                                0.29
                            ],
                            [
                                'v7.0',
                                0.26
                            ],
                            [
                                'v6.2',
                                0.17
                            ]
                        ]
                    }, {
                        name: 'Opera',
                        id: 'Opera',
                        data: [
                            [
                                'v12.x',
                                0.34
                            ],
                            [
                                'v28',
                                0.24
                            ],
                            [
                                'v27',
                                0.17
                            ],
                            [
                                'v29',
                                0.16
                            ]
                        ]
                    }]
                }
            });
        });
    }




    //============================================================= THỐNG KÊ VÍ ĐIỆN TỬ =========================================

    //========================================= BÁO CÁO TÌNH HÌNH THU CHI QUỸ THEO KHOẢNG THỜI GIAN============================================
    self.reportTitle = ko.observable("Daily report " + moment().format('DD/MM/YYYY'));
    self.titleToday = ko.observable("Today");
    self.selectDateReport = ko.observable("day");
    self.reportDate = ko.observable(moment());
    self.reportDateStart = ko.observable(moment().startOf('day'));
    self.reportDateEnd = ko.observable(moment().endOf('day'));
    self.FinanceFundId = ko.observable("");

    self.btnNext = function () {
        if (self.selectDateReport() === 'day') {
            self.reportDate(self.reportDate().add(1, 'days'));
        } else if (self.selectDateReport() === 'week') {
            self.reportDate(self.reportDate().add(7, 'days'));
        } else {
            self.reportDate(self.reportDate().add(1, 'months'));
        }
        self.reportMode();
    }

    self.btnPre = function () {
        if (self.selectDateReport() === 'day') {
            self.reportDate(self.reportDate().add(-1, 'days'));
        } else if (self.selectDateReport() === 'week') {
            self.reportDate(self.reportDate().add(-7, 'days'));
        } else {
            self.reportDate(self.reportDate().add(-1, 'months'));
        }
        self.reportMode();
    }

    self.btnToday = function () {
        self.reportDate(moment(new Date()));
        self.reportMode();
    }

    self.btnCalendar = function () {
        document.getElementById("reportDate").focus();
    }

    self.btnSelect = function (name) {
        self.selectDateReport(name);
        self.titleToday(name == 'day' ? "Today" : name == 'week' ? 'This week' : 'This month');
        self.reportMode();

        $('.report-date').datepicker("remove");
        if (self.selectDateReport() == 'day') {
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en'
            });
        } else if (self.selectDateReport() == 'week') {
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en'
            });
        } else {
            $('.report-date').datepicker({
                autoclose: true,
                minViewMode: 1,
                language: 'en'
            });
        }
    }

    self.reportMode = function () {
        if (self.selectDateReport() == 'day') {
            self.reportTitle("Daily report " + self.reportDate().format('DD/MM/YYYY'));

            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
        } else if (self.selectDateReport() == 'week') {
            self.reportTitle("Weekly report " + self.reportDate().week() + '(' + self.reportDate().startOf('week').isoWeekday(1).format('DD/MM/YYYY') + ' - ' + self.reportDate().endOf('week').format('DD/MM/YYYY') + ')');

            self.reportDateStart(self.reportDate().startOf('week').isoWeekday(1).startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('week').endOf('day').format());
        } else {
            self.reportTitle("Monthly report " + self.reportDate().format('MM/YYYY'));

            self.reportDateStart(self.reportDate().startOf('month').format());
            self.reportDateEnd(self.reportDate().endOf('month').format());
        }

        if (self.active() == 'reportAccountSituation') {
            self.viewReportAccountSituation();
        }
        if (self.active() == 'reportAccount') {
            self.viewReportAccount(1);
        }
    }

    self.viewReportAccountSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.post("/Accountant/GetAccountSituation", { startDay: start, endDay: end, financeFundId: self.FinanceFundId() }, function (data) {
            self.isLoading(true);
            $("#userAccountSituation").highcharts({
                chart: {
                    zoomType: 'xy'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: [{
                    categories: data.day,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    allowDecimals: false,
                    labels: {
                        format: '{value} Baht',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Sum',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: '',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} Baht',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    series: {
                        borderWidth: 0,
                        dataLabels: {
                            enabled: true,
                            format: '{point.y}'
                        }
                    }
                },
                series: [{
                    name: 'Thu',
                    type: 'column',
                    data: data.increase,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }

                },
                {
                    name: 'Chi',
                    type: 'column',
                    data: data.diminishe,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }

                }
                ,
                {
                    name: 'Fund balance',
                    type: 'spline',
                    data: data.balance,
                    tooltip: {
                        valueSuffix: ' Baht'
                    },

                }]
            });
        });
    }

    self.keyword = ko.observable();

    self.viewReportAccount = function (page) {
        self.isRending(false);
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.post("/Accountant/GetAccount", { page: page, pageSize: pagesize, keyword: self.keyword(), startDay: start, endDay: end }, function (data) {
            total = data.totalRecord;
            self.paging();
            self.isRending(true);
            self.isLoading(true);
            $("#userAccount").highcharts({
                chart: {
                    type: 'bar'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: data.name,
                    title: {
                        text: null
                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Money (Baht)',
                        align: 'high'
                    },
                    labels: {
                        overflow: 'justify'
                    }
                },
                tooltip: {
                    shared: true
                },
                plotOptions: {
                    bar: {
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'top',
                    x: -40,
                    y: 80,
                    floating: true,
                    borderWidth: 1,
                    backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
                    shadow: true
                },
                credits: {
                    enabled: false
                },
                series: [{
                    name: 'Add', tooltip: {
                        valueSuffix: ' đ'
                    },
                    data: data.increase
                }, {
                    name: 'Deduct', tooltip: {
                        valueSuffix: ' đ'
                    },
                    data: data.diminishe
                }, {
                    name: 'Opening balance', tooltip: {
                        valueSuffix: ' đ'
                    },
                    data: data.before
                }, {
                    name: 'Ending balance', tooltip: {
                        valueSuffix: ' đ'
                    },
                    data: data.after
                }, {
                    name: 'Current balance', tooltip: {
                        valueSuffix: ' đ'
                    },
                    data: data.balance
                }]
            });
        });
    }



    //============================================================= XUẤT BÁO CÁO EXCEL ==========================================

    //============================ Xuất danh sách báo cáo quỹ
    self.ExcelReport = function () {

        if (self.active() === 'moneyfund') {
            self.FundBillExcelReport();
        }

        if (self.active() === 'recharge') {
            self.RechargeExcelReport();
        }

        if (self.active() === 'Debit') {
            self.GetAllDebitList();
        }

        if (self.active() === 'mustreturn') {
            self.GetAllMustReturnList();
        }

        if (self.active() === 'mustreturn') {
            self.GetAllMustReturnList();
        }
        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        };

        if (self.active() === 'ticket-support') {
            self.GetAllTicketListByStaff();
        }

        //if (self.active() === 'accountantOrder') {
        //    self.AccountantOrderExcelReport();
        //}
    }


    self.ExcelReportAccountSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/Accountant/ExportAccountExcelFund", { startDay: start, endDay: end, financeFundId: self.FinanceFundId() }, "POST");
    }

    self.ExcelReportFundSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/Accountant/FinanceFundBalanceExcelSituationReport", { startDay: start, endDay: end }, "POST");

    }

    self.ExcelReportAccount = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/Accountant/ExportAccountExcel", { keyword: self.keyword(), startDay: start, endDay: end }, "POST");
    }

    self.FundBillExcelReport = function () {
        var searchFundBillData = ko.mapping.toJS(self.SearchFundBillModal());
        $.redirect("/FundBill/FundBillExcelReport", { searchModal: searchFundBillData }, "POST");

    }

    self.RechargeExcelReport = function () {
        var searchRechargeBillData = ko.mapping.toJS(self.SearchRechargeBillModal());
        {
            $.redirect("/RechargeBill/RechargeExcelReport", { searchModal: searchRechargeBillData }, "POST");
        }
    }

    self.AccountantOrderExcelReport = function () {
        var searchRechargeBillData = ko.mapping.toJS(self.SearchRechargeBillModal());
        {
            $.redirect("/RechargeBill/RechargeExcelReport", { searchModal: searchRechargeBillData }, "POST");
        }
    }

    // Báo cáo trong phần báo cáo
    self.FinanceFundExcelReport = function () {
        $.redirect("/FundBill/FinanceFundBalanceExcelReport", {}, "POST");
    }

    // Báo cáo trong phần báo cáo
    self.ExportExcelFund = function () {
        var start = self.SearchFundBillModal().DateStart();
        $.redirect("/Accountant/ExportExcelFund", { dateInput: start }, "POST");
    }
    self.exportExcelClaimForRefund = function () {
        var SearchClaimForRefundModal = ko.mapping.toJS(self.SearchClaimForRefundModal());
        if (SearchClaimForRefundModal.Status === undefined) {
            SearchClaimForRefundModal.Status = -1;
        }
        $.redirect("/Ticket/ExportExcelClaimForRefund",
        {
            searchModal: SearchClaimForRefundModal,
            userId: self.userId,
            customerId: self.customerId
        },
        "POST");
    }

    self.ExcelReportOrderOffice = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/AccountantReport/ExportExcelRevenueOrderOffice", { startDay: start, endDay: end }, "POST");
    }

    self.ExcelReportBusinessOffice = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/AccountantReport/ExportExcelRevenueBusinessOffice", { startDay: start, endDay: end }, "POST");
    }

    self.ExcelReportGomContOffice = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/AccountantReport/ExportExcelRevenueGomContOffice", { startDay: start, endDay: end }, "POST");
    }

    //============================================================= Các hàm Print ===============================================
    $(function () {
        $("#moneyFundDetailModal .btn-primary").click(function () {
            $("body").addClass("printModal");
            $("#moneyFundDetailModal").addClass('modalPrint');

            $("body").removeClass("printModal");
            $("#moneyFundDetailModal").removeClass('modalPrint');
            window.print();
        });
    });


    //============================================================= Các hàm hỗ trợ ==============================================

    self.initInputMark = function () {
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });

        $('input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat[','], autoGroup: true, groupSeparator: Globalize.culture().numberFormat['.'], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat[','], autoGroup: true, groupSeparator: Globalize.culture().numberFormat['.'], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.showChil = function (id) {
        $('.chil' + id).toggle();
    }

    self.GetRechargeBillDetailFind = function (data) {
        $.post("/RechargeBill/GetRechargeBillDetail", { rechargeBillId: data.Id }, function (result) {
            self.mapRechargeBillModel(result.rechargeBillModal);
            console.log(result.rechargeBillModal);
        });
    }
    self.viewMoneyFundDetailFind = function (data) {
        self.GetRechargeBillDetailFind(data);
        $('#rechargeDetailModal').modal();
        //self.GetFundBillDetail(id);
        //$('#rechargeDetailModal').modal();
    }

};

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var ticketDetailViewModel = new TicketDetailViewModel();
ko.applyBindings(ticketDetailViewModel, $("#ticketDetailModal")[0]);

var accountantDetail = new CustomerLookUp();
ko.applyBindings(accountantDetail, $("#moneyFundDetailModal")[0]);

var modelView = new AccountantViewModel(orderDetailViewModel, depositDetailViewModel, ticketDetailViewModel, accountantDetail, orderCommerceDetailViewModel);
//var modelView = new AccountantViewModel(orderDetailViewModel, depositDetailViewModel, ticketDetailViewModel, orderCommerceDetailViewModel);
modelView.accountantOrderViewModel = new AccountantOrderViewModel(orderDetailViewModel);

ko.applyBindings(modelView, $("#accountantPage")[0]);