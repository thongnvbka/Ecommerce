var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var totalDetail = 0;
var pageDetail = 1;
var pagesizeDetail = 10;
var pageTotalDetail = 0;


function CustomerViewModel(orderDetailViewModel, ticketDetail, depositDetailViewModel, accountantDetail, orderCommerceDetailViewModel) {
    var self = this;
    //========== Các biến cho template
    self.active = ko.observable('customer-report');
    self.templateId = ko.observable('customer-report');
    self.active2 = ko.observable('customerDetail');
    self.templateId2 = ko.observable('customerDetail');
    self.totalCustomerList = ko.observable();

    //========== Các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);
    self.titleCustomer = ko.observable();

    ///========== Khai báo Model show dữ liệu trên View
    self.AddOrEdit = ko.observable();

    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listUser = ko.observableArray([]);
    self.listUserDetail = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);
    self.listSexCustomer = ko.observableArray([]);

    self.listSystemByStaff = ko.observableArray([]);
    self.listStatusByStaff = ko.observableArray([]);
    self.listUserByStaff = ko.observableArray([]);
    self.listSexCustomerByStaff = ko.observableArray([]);
    //tiem nang by user
    self.listSystemPotentialCustomerByUser = ko.observableArray([]);
    self.listUserPotentialCustomerByUser = ko.observableArray([]);
    self.listSexPotentialCustomerByUser = ko.observableArray([]);
    self.listCustomerStypePotentialCustomerByUser = ko.observableArray([]);
    //tiem nang
    self.listSystemRenderPoCustomer = ko.observableArray([]);
    self.listUserPotentialCustomer = ko.observableArray([]);
    self.lisSexPotentialCustomer = ko.observableArray([]);
    self.listCustomerStypePotentialCustomer = ko.observableArray([]);

    //khieu nai
    self.listComplainStatus = ko.observableArray([]);
    self.listComplainSystem = ko.observableArray([]);
    self.listCustomerCountry = ko.observableArray([]);

    //He thong
    self.listSystem = ko.observableArray([]);
    self.systemId = ko.observable();
    self.systemName = ko.observable();
    self.data = ko.observable();
    self.editCustomer = ko.observable;

    //========== Khai báo ListData đổ dữ liệu danh sách
    self.listAllCustomer = ko.observableArray([]);
    self.listAllCustomerByStaff = ko.observableArray([]);
    self.listAllPotentialCustomerByUser = ko.observableArray([]);
    self.listAllCustomerComplain = ko.observableArray([]);
    self.listAllOrderCustomer = ko.observableArray([]);

    self.listAllPotentialCustomer = ko.observableArray([]);

    self.listWarehouseCustomer = ko.observableArray([]);
    self.listWarehouse = ko.observableArray([]);
    self.listCountry = ko.observableArray([]);
    self.listVip = ko.observableArray([]);

    self.listRevenueReport = ko.observableArray([]);
    self.listOrderPending = ko.observableArray([]);

    ///========== Khai báo Model show dữ liệu trên View
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainModel = ko.observable(new complainModel());
    self.complainUserModel = ko.observable(new complainUserModel());
    self.rechargeBill = ko.observable(new rechargeBillDetailModel());
    self.order = ko.observable(new orderModel());
    self.user = ko.observable(new userModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    self.potentialCustomerModel = ko.observable(new PotentialCustomerModel());
    self.customerOrderPendingModel = ko.observable(new customerOrderPendingModel());

    //=========== Sum số
    self.totalPotentialCustomer = ko.observable();
    self.totalPotentialCustomerByUser = ko.observable();
    self.totalCustomer = ko.observable();
    self.totalCustomerByUser = ko.observable();
    self.totalTicketSupport = ko.observable();

    //=========tab
    self.listSystemRender = ko.observableArray([]);
    self.listStatusS = ko.observableArray([]);
    self.listOrderCustomerStatus = ko.observableArray([]);
    self.listOrderCustomerType = ko.observableArray([]);


    self.listUserOfPosition = ko.observableArray([]);

    //==listbycustomer
    self.listComplainByCustomer = ko.observableArray([]);
    self.listOrderMoneyByCustomer = ko.observableArray([]);
    self.listOrderByCustomer = ko.observableArray([]);
    //-danh sach khach hang tiem nang
    self.customerEmail = ko.observable();
    self.customerPhone = ko.observable();
    self.customerAddress = ko.observable();
    self.customerLevel = ko.observable();
    self.complainuser = ko.observable([]);
    self.count = ko.observable();
    self.titleTicket = ko.observable();
    self.complainuser1 = ko.observable([]);
    self.listSystemPotentialCustomer = ko.observableArray([]);

    //DANH SACH QUAN, HUYEN, TINH THANH
    self.listProvince = ko.observableArray([]);
    self.listDistrict = ko.observableArray([]);
    self.listWard = ko.observableArray([]);

    //Dữ liệu thống kê doanh số nhận viên
    self.totalOrderExchange = ko.observable(0);
    self.totalServicePurchase = ko.observable(0);
    self.totalOrderBargain = ko.observable(0);
    self.TotalOrderWeight = ko.observable(0);

    //Track Staff orders
    self.orderTypeCustomer = ko.observable(-1);

    //==================== Search Object - Customer
    self.SearchCustomerModal = ko.observable({
        Keyword: ko.observable(""),
        GenderId: ko.observable(-1),
        UserId: ko.observable(-1),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable(""),
        Money: ko.observable(""),
        WarehouseId: ko.observable(-1),
        CustomerType: ko.observable(-1),
        CustomerStatus: ko.observable(-1),
        WarehouseCustomer: ko.observable(-1)
    });
    self.SearchCustomerData = ko.observable(self.SearchCustomerModal());

    // Search PotentialCustomer
    self.SearchPotentialCustomer = ko.observable({
        Keyword: ko.observable(""),
        GenderId: ko.observable(-1),
        UserId: ko.observable(-1),
        SystemId: ko.observable(-1),
        CustomerType: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchPotentialCustomer = ko.observable(self.SearchPotentialCustomer());

    // Search PotentialCustomerByUser
    self.SearchPotentialCustomerByUser = ko.observable({
        Keyword: ko.observable(""),
        GenderId: ko.observable(-1),
        UserId: ko.observable(-1),
        CustomerType: ko.observable(-1),
        SystemId: ko.observable(-1)
    });
    self.SearchPotentialCustomerByUser = ko.observable(self.SearchPotentialCustomerByUser());

    // SearchRevenueReportModal
    self.SearchRevenueReportModal = ko.observable({
        Keyword: ko.observable(""),
        GenderId: ko.observable(-1),
        UserId: ko.observable(-1),
        SystemId: ko.observable(-1)
    });
    self.SearchRevenueReportData = ko.observable(self.SearchRevenueReportModal());

    //self.customerId = ko.observable();

    //==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    self.listPageDetail = ko.observableArray([]);
    self.pageDetailStart = ko.observable(false);
    self.pageDetailEnd = ko.observable(false);
    self.pageDetailNext = ko.observable(false);
    self.pageDetailPrev = ko.observable(false);
    self.pageTitleDetail = ko.observable("");

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
        self.pageTitle(window.resources.common.page.show + (((page - 1) * pagesize) + 1) + window.resources.common.page.to + (((page) * pagesize) > total ? total : ((page) * pagesize)) + window.resources.common.page.of + total + window.resources.common.page.record);
    }

    //Hàm khởi tạo phân trang
    self.pagingDetail = function () {
        var listPageDetail = [];

        pageDetail = pageDetail <= 0 ? 1 : pageDetail;
        pageTotalDetial = Math.ceil(totalDetail / pagesizeDetail);
        pageDetail > 3 ? self.pageDetailStart(true) : self.pageDetailStart(false);
        pageDetail > 4 ? self.pageDetailNext(true) : self.pageDetailNext(false);
        pageTotalDetail - 2 > pageDetail ? self.pageDetailEnd(true) : self.pageDetailEnd(false);
        pageTotalDetail - 3 > pageDetail ? self.pageDetailPrev(true) : self.pageDetailPrev(false);

        var start = (pageDetail - 2) <= 0 ? 1 : (pageDetail - 2);
        var end = (pageDetail + 2) >= pageTotalDetial ? pageTotalDetial : (pageDetail + 2);

        for (var i = start; i <= end; i++) {
            listPageDetail.push({ PageDetail: i });
        }

        self.listPageDetail(listPageDetail);
        self.pageTitleDetail(window.resources.common.page.show + (((pageDetail - 1) * pagesizeDetail) + 1) + window.resources.common.page.to + (((pageDetail) * pagesizeDetail) > totalDetail ? totalDetail : ((pageDetail) * pagesizeDetail)) + window.resources.common.page.of + totalDetail + window.resources.common.page.record);
    }

    //Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }

    //Sự kiện  click vào nút phân trang Detail khách hàng
    self.setPageDetail = function (data) {
        if (pageDetail !== data.PageDetail) {
            pageDetail = data.PageDetail;
            self.searchDetail(pageDetail);
        }
    }

    // Hàm reset lại đối tượng tìm kiếm
    self.resetSearchCustomerModal = function () {
        self.SearchCustomerModal().Keyword("");
        self.SearchCustomerModal().GenderId(-1)
        self.SearchCustomerModal().UserId(-1);
        self.SearchCustomerModal().SystemId(-1);
        self.SearchCustomerModal().Status(-1);
        self.SearchCustomerModal().DateStart("");
        self.SearchCustomerModal().DateEnd("");
        self.SearchCustomerModal().Money("");
        self.SearchCustomerModal().WarehouseId(-1);
        self.SearchCustomerModal().CustomerType(-1);
        self.SearchCustomerModal().CustomerStatus(-1);
        self.SearchCustomerModal().WarehouseCustomer(-1);
    }

    //==================== Các sự kiện click menu =====================================
    self.clickMenu = function (name) {
        self.active(name);
        self.templateId(name);
        total = 0;
        page = 1;
        pageTotal = 0;
        window.history.pushState('Customerofstaff', '', 'customerofstaff#' + name);
        if (name === 'customer') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystem2();
            self.resetSearchCustomerModal();
            self.GetAllCustomerList();

            self.isRending(true);
            self.isLoading(true);
        }

        if (name === 'customer-by-staff') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystem2();
            self.resetSearchCustomerModal();
            self.GetAllCustomerByStaffList();
            self.isRending(true);
            self.isLoading(true);
        }

        if (name === 'customerfeasibility-by-staff') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystem2();
            self.resetSearchCustomerModal();
            self.GetPotentialCustomerByUserSearchData();

            self.isRending(true);
            self.isLoading(true);
        }
        if (name === 'ticket-support') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystem2();
            self.resetSearchCustomerModal();
            self.GetCustomerComplainSearchData();
            self.isRending(true);
            self.isLoading(true);
        }
        if (name === 'PotentialCustomer') {
            self.isRending(false);
            self.isLoading(false);
            self.renderSystem2();
            self.resetSearchCustomerModal();
            self.GetPotentialCustomerSearchData();

            self.isRending(true);
            self.isLoading(true);
        }
        if (name === 'revenue') {
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.GetRevenueReportList();

            self.isRending(true);
            self.isLoading(true);
        }
        if (name === 'revenueDeposit') {
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.GetRevenueReportDepositList();

            self.isRending(true);
            self.isLoading(true);
        }
        //Thống kê khách hàng chậm đặt hàng
        if (name === 'customer-orderPending') {
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.GetOrderPendingList();

            self.isRending(true);
            self.isLoading(true);
        }
        if (name === 'customerfind') {
            self.searchCustomer();
        }

        if (name === 'customer-report') {
            self.resetSearchCustomerModal();
            self.viewReportPotentialCustomerOffStaff();
            self.viewReportPotentialCustomer();
            self.viewReportCustomer();
            self.viewReportCustomerOffStaff();
        }

        if (name === 'reportCustomerSituation') {
            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
            self.viewReportCustomerSituation();
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });

        }

        if (name === 'ordercustomer') {
            self.orderTypeCustomer(-1);
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.renderSystem2();
            self.GetAllOrderCustomerList();
            self.isRending(true);
            self.isLoading(true);
        }

        self.init();
    }

    //============================================================= XUẤT BÁO CÁO EXCEL ==========================================

    //============================ Xuất danh sách báo cáo quỹ
    self.ExcelReport = function () {
        if (self.active() === 'customer') {
            self.CustomerExcelReport();
        }

        if (self.active() === 'customer-by-staff') {
            self.CustomerByUserExcelReport();
        }

        if (self.active() === 'customerfeasibility-by-staff') {
            self.PotentialCustomerByUserExcelReport();
        }
        if (self.active() === 'ticket-support') {
        }
        if (self.active() === 'PotentialCustomer') {
            self.PotentialCustomerExcelReport();
        }
        //Thống kê doanh số nhân viên
        if (self.active() === 'revenue') {
            self.revenueExcelReport();
        }
        if (self.active() === 'revenueDeposit') {
            self.revenueExcelDepositReport();
        }


        //Track Staff orders
        if (self.active() === 'ordercustomer') {
            self.ordercustomerExcelReport();
        }

        //Thống kê khách hàng chậm đặt hàng
        if (self.active() === 'customer-orderPending') {
        }
        if (self.active() === 'customerfind') {
        }

        if (self.active() === 'customer-report') {
        }
    }

    self.PotentialCustomerExcelReport = function () {
        var searchPotentialCustomerData = ko.mapping.toJS(self.SearchPotentialCustomer());
        {
            $.redirect("/CustomerOfStaff/PotentialCustomerExcelReport", { searchModal: searchPotentialCustomerData }, "POST");
        }
    }

    self.PotentialCustomerByUserExcelReport = function () {
        var SearchPotentialCustomerByUserData = ko.mapping.toJS(self.SearchPotentialCustomerByUser());
        {
            $.redirect("/CustomerOfStaff/PotentialCustomerByUserExcelReport", { searchModal: SearchPotentialCustomerByUserData }, "POST");
        }
    }

    self.CustomerExcelReport = function () {
        var searchCustomerData = ko.mapping.toJS(self.SearchCustomerModal());
        {
            $.redirect("/CustomerOfStaff/CustomerExcelReport", { searchModal: searchCustomerData }, "POST");
        }
    }

    self.CustomerByUserExcelReport = function () {
        var SearchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerModal());
        {
            $.redirect("/CustomerOfStaff/CustomerByUserExcelReport", { searchModal: SearchCustomByStaffData }, "POST");
        }
    }

    self.revenueExcelReport = function () {
        var searchCustomerModalData = ko.mapping.toJS(self.SearchCustomerModal());
        {
            $.redirect("/CustomerOfStaff/RevenueExcelReport", { searchModal: searchCustomerModalData }, "POST");
        }
    }

    self.revenueExcelDepositReport = function () {
        var searchCustomerModalData = ko.mapping.toJS(self.SearchCustomerModal());
        {
            $.redirect("/CustomerOfStaff/RevenueExcelDepositReport", { searchModal: searchCustomerModalData }, "POST");
        }
    }

    self.ordercustomerExcelReport = function () {
        var searchCustomerModalData = ko.mapping.toJS(self.SearchCustomerModal());
        {
            $.redirect("/CustomerOfStaff/OrdercustomerExcelReport", { searchModal: searchCustomerModalData, orderType: self.orderTypeCustomer() == undefined ? -1 : self.orderTypeCustomer() }, "POST");
        }
    }


    // Tìm kiếm Detail khách hàng
    self.searchDetail = function (page) {
        self.isRending(false);
        self.isLoading(true);
        if (self.active2() === 'OrderMoney') {


            self.mapRechargeBillModel(self.data());
            $.post("/CustomerOfStaff/GetListOrderMoneyByCustomer", { customerId: self.data().Id, page: page, pageSize: pagesize }, function (result) {
                self.isDetailRending(true);
                totalDetail = result.totalRecord;
                $(".select-view").select2();
                self.listOrderMoneyByCustomer(result.orderMoneyByCustomer);
                self.pagingDetail();
                self.isRending(true);
                self.isLoading(false);
            });
            $(".select-view").select2();

            //self.OrderMoney();
            //self.viewOrderMoneyByCustomerDetail(self.data());
        }
        if (self.active2() === 'OrderMoney1') {
            self.listOrderMoneyByCustomer([]);
            $.post("/Ticket/OrderMoney", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {

                totalDetail = data.totalRecord;
                self.listOrderMoneyByCustomer(data.customer);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });

        }

        if (self.active2() === 'OrderHistory1') {

            self.listOrderByCustomer([]);
            $.post("/Ticket/OrderHistory", {
                customerId: self.customerId(), page: page, pageSize: pagesize
            }, function (data) {
                totalDetail = data.totalRecord;
                self.listOrderByCustomer(data.customer);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active2() === 'OrderHistory') {

            self.mapOrderModel(self.data());
            $.post("/CustomerOfStaff/GetListOrderByCustomer", { customerId: self.data().Id, page: page, pageSize: pagesize }, function (result) {
                self.isDetailRending(true);
                totalDetail = result.totalRecord;
                $(".select-view").select2();
                self.listOrderByCustomer(result.orderByCustomer);
                self.pagingDetail();
                self.isRending(true);
                self.isLoading(false);
            });
            $(".select-view").select2();

            //self.OrderHistory();
            //self.viewOrderByCustomerDetail(self.data());

        }
    }
    ///tìm kiếm
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);
        //if (self.active2() === 'OrderMoney') {
        //    self.OrderMoney();
        //}

        //if (self.active2() === 'OrderHistory') {
        //    self.OrderHistory();
        //}
        if (self.active() === 'customer') {

            self.listAllCustomer([]);
            var searchCustomerData = ko.mapping.toJS(self.SearchCustomerModal());

            $.post("/CustomerOfStaff/GetAllCustomerList", { page: page, pageSize: pagesize, searchModal: searchCustomerData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomer(data.customerModal);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'customer-by-staff') {
            self.listAllCustomerByStaff([]);
            var SearchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerModal());

            $.post("/CustomerOfStaff/GetAllCustomerByStaffList", { page: page, pageSize: pagesize, searchModal: SearchCustomByStaffData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomerByStaff(data.customerByStaffModal);
                self.paging();

                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'customerfeasibility-by-staff') {
            self.listAllPotentialCustomerByUser([]);
            var SearchPotentialCustomerByUserData = ko.mapping.toJS(self.SearchCustomerModal());

            $.post("/CustomerOfStaff/GetAllPotentialCustomerListByUser", { page: page, pageSize: pagesize, searchModal: SearchPotentialCustomerByUserData }, function (data) {
                total = data.totalRecord;
                self.listAllPotentialCustomerByUser(data.potentialCustomerByUserModal);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'ticket-support') {
            self.GetAllCustomerComplainList();
        }

        if (self.active() === 'PotentialCustomer') {
            self.listAllPotentialCustomer([]);
            var searchPotentialCustomerData = ko.mapping.toJS(self.SearchCustomerModal());

            $.post("/CustomerOfStaff/GetAllPotentialCustomerList", { page: page, pageSize: pagesize, searchModal: searchPotentialCustomerData }, function (data) {
                total = data.totalRecord;
                self.listAllPotentialCustomer(data.potentialCustomerModal);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'revenue') {
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.GetRevenueReportList();
            self.isRending(true);
            self.isLoading(false);
            //self.listRevenueReport([]);
            //var searchRevenueData = ko.mapping.toJS(self.SearchPotentialCustomer());
            //self.totalOrderExchange(0);
            //self.totalServicePurchase(0);
            //self.totalOrderBargain(0);
            //$.post("/CustomerOfStaff/GetRevenueReportList", { page: page, pageSize: pagesize, searchModal: searchRevenueData }, function (data) {
            //    total = data.Total;
            //    self.listRevenueReport(data.ListItems);
            //    self.totalOrderExchange(data.TotalOrderExchange);
            //    self.totalServicePurchase(data.TotalServicePurchase);
            //    self.totalOrderBargain(data.TotalOrderBargain);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }

        if (self.active() === 'revenueDeposit') {
            self.isRending(false);
            self.isLoading(false);
            self.resetSearchCustomerModal();
            self.GetRevenueReportDepositList();
            self.isRending(true);
            self.isLoading(false);
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

        if (self.active() === 'customer-orderPending') {
            self.isRending(false);
            self.isLoading(false);

            self.GetOrderPendingList();
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        }

        if (self.active() === 'ordercustomer') {
            self.isRending(false);
            self.isLoading(false);
            self.GetAllOrderCustomerList();
            self.isRending(true);
            self.isLoading(true);
        }

    };

    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);
        page = 1;

        if (self.active() === 'customer') {
            self.GetAllCustomerList();
        }
        if (self.active() === 'customer-by-staff') {
            self.GetAllCustomerByStaffList();
        }
        if (self.active() === 'customerfeasibility-by-staff') {
            self.GetAllPotentialCustomerByUserList();
        }
        if (self.active() === 'ticket-support') {
            self.GetAllCustomerComplainList();
        }
        if (self.active() === 'PotentialCustomer') {
            self.GetAllPotentialCustomerList();
        }
        if (self.active() === 'revenue') {
            self.GetRevenueReportList();
        }
        if (self.active() === 'revenueDeposit') {
            self.GetRevenueReportDepositList();
        }
        if (self.active() === 'customer-orderPending') {
            self.listOrderPending([]);
            var searchCustomerModal = ko.mapping.toJS(self.SearchCustomerModal());
            $.post("/CustomerOfStaff/GetOrderPendingList", { page: page, pageSize: pagesize, searchModal: searchCustomerModal }, function (data) {
                total = data.totalRecord;
                self.listOrderPending(data.customerModal);
                self.paging();
                self.initInputMark();
                self.isRending(true);
                self.isLoading(false);
            });
        }
        if (self.active() === 'ordercustomer') {
            self.isRending(false);
            self.isLoading(false);
            self.GetAllOrderCustomerList();
        }

        //self.isRending(true);
        //self.isLoading(false);
    }

    //===== Khởi tạo =========
    $(function () {
        self.GetRenderSystem();
        self.GetInit();
        //self.GetInitPotentialCustomer();
        self.init();
        self.SearchCustomerModal().DateStart("");
        self.SearchCustomerModal().DateEnd("");
        self.viewReportPotentialCustomerOffStaff();
        self.viewReportPotentialCustomer();
        self.viewReportCustomer();
        self.viewReportCustomerOffStaff();

        self.CheckUrl();

        $(window).bind('hashchange', function () {
            self.CheckUrl();
        });

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

    self.CheckUrl = function () {
        var arr = _.split(window.location.href, "#");
        var arrCheck = ['PotentialCustomer', 'customerfeasibility-by-staff', 'customer', 'customer-by-staff', 'ticket-support', 'customerfind', 'customer-report', 'customer-orderPending', 'revenue', 'ordercustomer', 'revenueDeposit'];
        if (arr.length > 1) {
            if (_.lastIndexOf(arrCheck, arr[1]) !== -1) {
                self.clickMenu(arr[1]);
                setTimeout(function () {
                    self.setDaterange();
                }, 300);
            }
        }
    }

    self.init = function () {
        self.setDaterange();
        //Date picker
        $('#datepicker').datepicker({
            autoclose: true
        });

        $(".select-view").select2();
    }

    self.setDaterange = function () {
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
                      'Today' : [moment(), moment()],
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
                      $('#daterange-btn span').html('Created date');
                      self.SearchCustomerModal().DateStart('');
                      self.SearchCustomerModal().DateEnd('');
                  }
                  else {
                      $('#daterange-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchCustomerModal().DateStart(start.format());
                      self.SearchCustomerModal().DateEnd(end.format());
                  }
              }
          );
        $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btn span').html('Created date');
            self.SearchCustomerModal().DateStart("");
            self.SearchCustomerModal().DateEnd("");
        });

        $('#daterange-btnuser').daterangepicker(
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
                     'Today' : [moment(), moment()],
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
                     $('#daterange-btnuser span').html('Created date');
                     self.SearchCustomerModal().DateStart('');
                     self.SearchCustomerModal().DateEnd('');
                 }
                 else {
                     $('#daterange-btnuser span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                     self.SearchCustomerModal().DateStart(start.format());
                     self.SearchCustomerModal().DateEnd(end.format());
                 }
             }
         );
        $('#daterange-btnuser').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnuser span').html('Create date');
            self.SearchCustomerModal().DateStart("");
            self.SearchCustomerModal().DateEnd("");
        });

        $('#daterange-btnallday1').daterangepicker(
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
                      'Today' : [moment(), moment()],
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
                      $('#daterange-btnallday1 span').html('Created date');
                      self.SearchCustomerModal().DateStart('');
                      self.SearchCustomerModal().DateEnd('');
                      self.viewReportPotentialCustomerOffStaff();
                  }
                  else {
                      $('#daterange-btnallday1 span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchCustomerModal().DateStart(start.format());
                      self.SearchCustomerModal().DateEnd(end.format());
                      self.viewReportPotentialCustomerOffStaff();
                  }
              }
          );
        $('#daterange-btnallday1').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnallday1 span').html('Created date');
            self.SearchCustomerModal().DateStart("");
            self.SearchCustomerModal().DateEnd("");
        });
        $('#daterange-btnallday2').daterangepicker(
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
                      'Today' : [moment(), moment()],
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
                      $('#daterange-btnallday2 span').html('Created date');
                      self.SearchCustomerModal().DateStart('');
                      self.SearchCustomerModal().DateEnd('');
                      self.viewReportCustomerOffStaff();
                  }
                  else {
                      $('#daterange-btnallday2 span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchCustomerModal().DateStart(start.format());
                      self.SearchCustomerModal().DateEnd(end.format());
                      self.viewReportCustomerOffStaff();
                  }
              }
          );
        $('#daterange-btnallday2').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnallday2 span').html('Created date');
            self.SearchCustomerModal().DateStart("");
            self.SearchCustomerModal().DateEnd("");
        });
        $('#daterange-btnday1').daterangepicker(
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
                      'Today' : [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                  },
                  startDate: moment().subtract(6, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  if (start.format() === 'Invalid date') {
                      $('#daterange-btnday1 span').html('Created date');
                      self.SearchCustomerModal().DateStart('');
                      self.viewReportPotentialCustomer();
                  }
                  else {
                      $('#daterange-btnday1 span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchCustomerModal().DateStart(start.format());
                      self.viewReportPotentialCustomer();
                  }
              }
          );
        $('#daterange-btnday1').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnday1 span').html('Created date');
            self.SearchCustomerModal().DateStart("");
        });

        $('#daterange-btnday2').daterangepicker(
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
                      'Today' : [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                  },
                  startDate: moment().subtract(6, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  if (start.format() === 'Invalid date') {
                      $('#daterange-btnday2 span').html('Created date');
                      self.SearchCustomerModal().DateStart('');
                      self.viewReportCustomer();
                  }
                  else {
                      $('#daterange-btnday2 span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchCustomerModal().DateStart(start.format());
                      self.viewReportCustomer();
                  }
              }
          );
        $('#daterange-btnday2').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnday2 span').html('Created date');
            self.SearchCustomerModal().DateStart("");
        });
    }

    //============================================== Init khởi tạo =====================================
    self.GetRenderSystem = function () {
        self.listSystem([]);
        self.listStatus([]);
        self.listUser([]);
        self.listSexCustomer([]);
        self.listWarehouse([]);
        self.listWarehouseCustomer([]);
        self.listCountry([]);
        self.listVip([]);

        $.post("/CustomerOfStaff/GetRenderSystem", {}, function (response) {
            self.listSystem(response.listSystem);
            self.listStatus(response.listStatus);
            self.listUser(response.listUser);
            self.listSexCustomer(response.listSexCustomer);
            self.listWarehouse(response.listWarehouse);
            self.listCountry(response.listCountry);
            self.listProvince(response.listProvince);
            self.listWarehouseCustomer(response.listWarehouseStatus);
            self.listVip(response.listVip);
            $('.nav-tabs').tabdrop();
        });
    }

    self.GetInit = function () {
        $.post("/CustomerOfStaff/GetInit", function (data) {
            self.totalPotentialCustomer(data.totalPotentialCustomer);
            self.totalPotentialCustomerByUser(data.totalPotentialCustomerByUser);
            self.totalCustomer(data.totalCustomer);
            self.totalCustomerByUser(data.totalCustomerByUser);
            self.totalTicketSupport(data.totalTicketSupport);
        });
    };


    // Hàm lấy Sum dữ liệu khách hàng

    self.GetInitPotentialCustomer = function () {
        $.post("/CustomerOfStaff/GetInitPotentialCustomer", function (data) {
            self.totalPotentialCustomer(data.totalPotentialCustomer);
        });
    };   // Hàm lấy Sum dữ liệu khách hàng tiềm năng

    //============================================== Customer
    self.GetAllCustomerList = function () {
        self.listAllCustomer([]);
        var searchCustomerData = ko.mapping.toJS(self.SearchCustomerModal());

        if (searchCustomerData.Status === undefined) {
            searchCustomerData.Status = -1;
        }
        if (searchCustomerData.GenderId === undefined) {
            searchCustomerData.GenderId = -1;
        }
        if (searchCustomerData.UserId === undefined) {
            searchCustomerData.UserId = -1;
        }
        if (searchCustomerData.SystemId === undefined) {
            searchCustomerData.SystemId = -1;
        }
        if (searchCustomerData.WarehouseId === undefined) {
            searchCustomerData.WarehouseId = -1;
        }
        if (searchCustomerData.WarehouseCustomer === undefined) {
            searchCustomerData.WarehouseCustomer = -1;
        }

        $.post("/CustomerOfStaff/GetAllCustomerList", { page: page, pageSize: pagesize, searchModal: searchCustomerData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomer(data.customerModal);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }          // Lấy danh sách khach hang

    self.btnCreateOrEditCustomer = function (data) {
        if (self.AddOrEdit() === 0) {
            self.saveCustomer(data);
        }
        else {
            self.EditCustomerDetail(data);
        }
    } // Button tạo mới khách hàng


    self.btnCreateOrEditCustomerByStaff = function (data) {
        if (self.AddOrEdit() === 0) {
            self.saveCustomerByStaff(data);
        }
        else {
            self.EditCustomerByStaff(data);
        }
    }

    //Lưu thông tin khách hàng thêm mới (SAVE)
    self.saveCustomerByStaff = function (data) {
        console.log(data);

        $('#addCustomer').valid();
        $('#inforCustomer').valid();

        var customerData = ko.mapping.toJS(self.customerModel());

        console.log(self.customerModel());
        // Khởi tạo
        customerData.Id = 0;
        customerData.DistrictId = -1;
        customerData.ProvinceId = -1;
        customerData.WardId = -1;
        if (customerData.Balance == null || customerData.Balance == "" || customerData.Balance == undefined) {
            customerData.Balance = 0;
        }
        if (customerData.BalanceAvalible == null || customerData.BalanceAvalible == "" || customerData.BalanceAvalible == undefined) {
            customerData.BalanceAvalible = 0;
        }
        $.post("/CustomerOfStaff/CreateNewCustomerByStaff", { model: customerData }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.GetAllCustomerList();
                self.customerModel(new customerOfStaffModel());
            }
        });
        //self.customerModel(new customerOfStaffModel());
        //$('#CustomerAddOrEdit').modal('hide');
    }

    //Edit thông tin khách hàng
    self.EditCustomerByStaff = function (data) {
        $('#addCustomer').valid();
        $('#inforCustomer').valid();

        var customerData = ko.mapping.toJS(self.customerModel());
        customerData.DistrictId = self.DistrictId;
        customerData.ProvinceId = self.ProvinceId;
        customerData.WardId = self.WardId;

        $.post("/CustomerOfStaff/EditCustomerByStaff", { model: customerData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllCustomerList();
            }
        });
    }

    //========================================KHÁCH HÀNG Official=============================================
    self.reportDate = ko.observable(moment());
    // Show form thêm mới khách hàng
    self.viewCustomerAdd = function () {
        self.AddOrEdit(0);
        self.customerModel(new customerOfStaffModel());
        self.titleCustomer('Add new customer account');
        //lấy thông tin
        self.initInputMark();

        $('#CustomerAddOrEdit').modal();

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });
    }

    self.viewCustomerEdit = function (data) {
        self.AddOrEdit(1);

        self.customerModel(new customerOfStaffModel());
        self.titleCustomer('Edit customer account');

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });
        //lấy thông tin
        $('#CustomerAddOrEdit').modal();
    }

    self.btnCalendar = function () {
        document.getElementById("reportDate").focus();
    }

    self.viewCustomerDetailModal = function (data) {
        self.titleCustomer(window.resources.common.label.customerDetail);
        self.isDetailRending(false);
        self.mapCustomerModel(data);
        self.data(data);
        $.post("/CustomerOfStaff/GetCustomerOfStaffDetail", { customerId: data.Id }, function (result) {
            self.isDetailRending(true);
            self.mapCustomerModel(result.customerModal);
            $(".select-view").select2();
        });

        $('#CustomerDetailModal').modal();
    }

    //Lưu thông tin khách hàng thêm mới (SAVE)
    self.saveCustomer = function (data) {
        $('#addCustomer').valid();
        $('#inforCustomer').valid();

        var customerData = ko.mapping.toJS(self.customerModel());

        // Khởi tạo
        customerData.Id = 0;
        customerData.DistrictId = -1;
        customerData.ProvinceId = -1;
        customerData.WardId = -1;
        if (customerData.Balance == null || customerData.Balance == "" || customerData.Balance == undefined) {
            customerData.Balance = 0;
        }
        if (customerData.BalanceAvalible == null || customerData.BalanceAvalible == "" || customerData.BalanceAvalible == undefined) {
            customerData.BalanceAvalible = 0;
        }
        $.post("/CustomerOfStaff/CreateNewCustomer", { model: customerData }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.GetAllCustomerList();
                self.customerModel(new customerOfStaffModel());
            }
        });
        //self.customerModel(new customerOfStaffModel());
        //$('#CustomerAddOrEdit').modal('hide');
    }

    //Show form edit (CUSTOMER OF STAFF)
    self.viewEditCustomerDetail = function (data) {
        self.AddOrEdit(1);
        self.isDetailRending(false);
        self.titleCustomer('Edit customer information officially');

        self.customerModel(new customerOfStaffModel());
        //self.mapCustomerModel(data);
        $.post("/CustomerOfStaff/GetCustomerOfStaffDetail", { customerId: data.Id }, function (result) {
            self.mapCustomerModel(result.customerModal);
            //self.ProvinceId(result.customerModal.ProvinceId);
            //self.DistrictId(result.customerModal.DistrictId);
            //self.WardId(result.customerModal.WardId);

            //self.cacheDistrictId(result.customerModal.DistrictId);
            //self.cacheWardId(result.customerModal.WardId);
            self.isDetailRending(true);
            $(".select-view").select2();
        });

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });


        $('#CustomerAddOrEdit').modal();
    };

    //Xóa khach hang(DELETE) (CUSTOMER OF STAFF)
    self.removeCustomer = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Customer "' + data.FullName + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/CustomerOfStaff/DeleteCustomer", { customerId: data.Id }, function (result) { });
            self.GetAllCustomerList();
            toastr.success("Deleted successfully");
            self.GetInit();
        }, function () {
            toastr.warning("Category does not exist or has been deleted");
        });

        self.GetAllCustomerList();
    };

    //Edit thông tin khách hàng
    self.EditCustomerDetail = function (data) {
        $('#addCustomer').valid();
        $('#inforCustomer').valid();

        var customerData = ko.mapping.toJS(self.customerModel());
        customerData.DistrictId = -1;
        customerData.ProvinceId = -1;
        customerData.WardId = -1;
        console.log(self.customerModel());
        $.post("/CustomerOfStaff/EditCustomer", { model: customerData }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllCustomerList();
            }
        });
    }

    //Lấy danh sách khách hàng Official phụ trách
    self.GetAllCustomerByStaffList = function () {
        self.listAllCustomerByStaff([]);
        var searchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerModal());

        if (searchCustomByStaffData.Status === undefined) {
            searchCustomByStaffData.Status = -1;
        }
        if (searchCustomByStaffData.GenderId === undefined) {
            searchCustomByStaffData.GenderId = -1;
        }
        if (searchCustomByStaffData.SystemId === undefined) {
            searchCustomByStaffData.SystemId = -1;
        }
        if (searchCustomByStaffData.WarehouseId === undefined) {
            searchCustomByStaffData.WarehouseId = -1;
        }
        if (searchCustomByStaffData.WarehouseCustomer === undefined) {
            searchCustomByStaffData.WarehouseCustomer = -1;
        }

        $.post("/CustomerOfStaff/GetAllCustomerByStaffList", { page: page, pageSize: pagesize, searchModal: searchCustomByStaffData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomerByStaff(data.customerByStaffModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    //Lấy danh sách Orders nhân viên phụ trách
    self.total = ko.observable(0);
    self.totalPriceBargain = ko.observable(0);
    self.totalServiceOrder = ko.observable(0);
    self.totalWeight = ko.observable(0);


    self.GetAllOrderCustomerList = function () {
        self.listAllOrderCustomer([]);
        var searchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerModal());

        if (searchCustomByStaffData.Status === undefined) {
            searchCustomByStaffData.Status = -1;
        }
        if (searchCustomByStaffData.UserId === undefined) {
            searchCustomByStaffData.UserId = -1;
        }
        $.post("/CustomerOfStaff/GetAllOrderCustomerList", { page: page, pageSize: pagesize, searchModal: searchCustomByStaffData, orderType: self.orderTypeCustomer() == undefined ? -1 : self.orderTypeCustomer() }, function (data) {
            total = data.totalRecord;
            self.total(data.total);
            self.totalPriceBargain(data.totalPriceBargain);
            self.totalServiceOrder(data.totalServiceOrder);
            self.totalWeight(data.totalWeight);
            self.listAllOrderCustomer(data.customerByStaffModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
        //self.SearchCustomerModal().UserId(-1);
        //self.SearchCustomerModal().Status(-1);
        //self.SearchCustomerModal().Keyword("");
    }

    //Show form edit( CUSTOMER BY STAFF)
    self.viewEditCustomerByStaff = function (data) {
        self.AddOrEdit(1);
        self.titleCustomer(window.resources.common.label.customerDetail);
        $('#btnThem2').hide();
        $('#btnEdit2').show();
        self.isDetailRending(false);
        self.mapCustomerModel(data);
        $.post("/CustomerOfStaff/GetCustomerOfStaffDetail", { customerId: data.Id }, function (result) {
            self.isDetailRending(true);
            console.log(result.customerModal);
            self.mapCustomerModel(result.customerModal);
            self.ProvinceId(result.customerModal.ProvinceId);
            self.DistrictId(result.customerModal.DistrictId);
            self.WardId(result.customerModal.WardId);

            self.cacheDistrictId(result.customerModal.DistrictId);
            self.cacheWardId(result.customerModal.WardId);
            $(".select-view").select2();
        });
        $(".select-view").select2();

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });
        $('#CustomerByStaffAddOrEdit').modal();
    }

    //Show form thêm mới customerbystaff
    self.viewCustomerByStaffAddOrEdit = function (data) {
        self.AddOrEdit(0);
        self.customerModel(new customerOfStaffModel());
        $('#btnThem2').show();
        $('#btnEdit2').hide();
        self.titleCustomer('Add new customer account');

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });

        $('#CustomerByStaffAddOrEdit').modal();
    }

    //__________________________________//

    //===Lấy danh sách dữ liệu đổ vào Search data của PotentialCustomer====

    self.GetPotentialCustomerSearchData = function () {
        self.listSystemPotentialCustomer([]);
        self.lisSexPotentialCustomer([]);
        self.listUserOfPosition([]);
        self.listCustomerStypePotentialCustomer([]);

        $.post("/CustomerOfStaff/GetAllPotentialCustomerSearchData", {}, function (data) {
            self.listSystemPotentialCustomer(data.listSystemPotentialCustomer);
            self.lisSexPotentialCustomer(data.listPotentialCustomerSex);
            self.listUserOfPosition(data.listUserOfPosition);
            self.listCustomerStypePotentialCustomer(data.listCustomerType);
            self.GetAllPotentialCustomerList();
        });
    }

    //Lấy danh sách khách hàng tiềm năng
    self.GetAllPotentialCustomerList = function () {
        self.listAllPotentialCustomer([]);
        var searchCustomerData = ko.mapping.toJS(self.SearchCustomerModal());

        $.post("/CustomerOfStaff/GetAllPotentialCustomerList", { page: page, pageSize: pagesize, searchModal: searchCustomerData }, function (data) {
            total = data.totalRecord;
            self.listAllPotentialCustomer(data.potentialCustomerModal);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }

    //show chi tiet
    self.viewPotentialCustomerDetail = function (data) {
        self.isDetailRending(false);
        self.titleCustomer(window.resources.common.label.customerDetail);
        //self.mapPotentialCustomerModel(data);
        self.potentialCustomerModel(new PotentialCustomerModel());
        $.post("/CustomerOfStaff/GetPotentialCustomerDetail", { potentialCustomerId: data.Id }, function (result) {
            self.isDetailRending(true);
            self.mapPotentialCustomerModel(result.potentialCustomerModal);
            $(".select-view").select2();
        });

        $('#PotentialCustomerDetail').modal();
    };

    //Show form edit khách hàng (CustomerFeasibilityByStaff)
    self.cacheDistrictId = ko.observable();
    self.cacheWardId = ko.observable();

    self.viewEditPotentialCustomer = function (data) {
        $('#btnThem').hide();
        $('#btnEdit').show();

        self.AddOrEdit(1);

        self.isDetailRending(false);
        self.titleCustomer(window.resources.common.label.customerDetail);
        //self.mapPotentialCustomerModel(data);
        self.potentialCustomerModel(new PotentialCustomerModel());

        $.post("/CustomerOfStaff/GetPotentialCustomerDetail", { potentialCustomerId: data.Id }, function (result) {

            self.mapPotentialCustomerModel(result.potentialCustomerModal);
            //self.ProvinceId(result.potentialCustomerModal.ProvinceId);

            //self.DistrictId(result.potentialCustomerModal.DistrictId);
            //self.WardId(result.potentialCustomerModal.WardId);

            //self.cacheDistrictId(result.potentialCustomerModal.DistrictId);
            //self.cacheWardId(result.potentialCustomerModal.WardId);

            //$(".select-district").empty().append($("<option/>").val(result.potentialCustomerModal.DistrictId).text(result.potentialCustomerModal.DistrictName).val(result.potentialCustomerModal.DistrictId)).trigger("change");
            //$(".select-ward").empty().append($("<option/>").val(result.potentialCustomerModal.WardId).text(result.potentialCustomerModal.WardsName).val(result.potentialCustomerModal.WardId)).trigger("change");

            self.isDetailRending(true);
            $(".select-view").select2();
        });

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });

        $('#PotentialCustomerAddOrEdit').modal();
    }

    //===================Thêm quan, huyen, xa, phuong===============
    self.ProvinceId = ko.observable();
    self.OldProvinceId = ko.observable();
    self.DistrictId = ko.observable();
    self.OldDistrictId = ko.observable();
    self.WardId = ko.observable();
    self.ProvinceId.subscribe(function (newValue) {
        if (newValue !== '' && newValue !== undefined && newValue !== null && newValue !== 0) {
            if (newValue != self.OldProvinceId()) {
                self.OldProvinceId(newValue);
                self.getDistrict(self.ProvinceId);
            }
        }
    });

    self.DistrictId.subscribe(function (newValue) {
        if (newValue !== '' && newValue !== undefined && newValue !== null && newValue !== 0) {
            if (newValue != self.OldDistrictId()) {
                self.OldDistrictId(newValue);
                self.GetWard(self.DistrictId);
            }
        }
    });

    //Lay huyen theo tinh, thanh pho
    self.getDistrict = function (data) {
        self.listDistrict([]);
        if (data != null) {
            $.post("/CustomerOfStaff/GetDistrict", { provinceId: data }, function (result) {
                self.listDistrict(result);
                if (self.AddOrEdit() == 1) {
                    self.DistrictId(self.cacheDistrictId());
                    $(".select-view").select2();
                }
            });
        }
    }
    //Lay xa theo huyen
    self.GetWard = function (data) {
        self.listWard([]);
        if (data != null) {
            $.post("/CustomerOfStaff/GetWard", { districtId: data }, function (result) {
                self.listWard(result);
                if (self.AddOrEdit() == 1) {
                    self.WardId(self.cacheWardId());
                    $(".select-view").select2();
                }
            });
        }
    }

    self.btnCreateOrEditPotentialCustomer = function (data) {
        if (self.AddOrEdit() === 0) {
            self.SavePotentialCustomer(data);
        }
        else {
            self.EditPotentialCustomer(data);
        }
    } // Button tạo mới khách hàng

    //Edit thông tin khách hàng tiềm năng
    self.EditPotentialCustomer = function (data) {
        $('#addCustomer').valid();

        var potentialCustomer = ko.mapping.toJS(self.potentialCustomerModel());

        potentialCustomer.Created = moment(potentialCustomer.Created).format();
        potentialCustomer.Updated = moment(potentialCustomer.Updated).format();
        potentialCustomer.DistrictId = -1;
        potentialCustomer.ProvinceId = -1;
        potentialCustomer.WardId = -1;

        $.post("/CustomerOfStaff/EditPotentialCustomer", { model: potentialCustomer }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllPotentialCustomerList();
            }
        });
    }

    //show form them moi  khach hang tiem nang
    self.viewAddPotentialCustomer = function () {
        self.AddOrEdit(0);
        $(".select-view").select2();

        self.potentialCustomerModel(new PotentialCustomerModel());
        self.titleCustomer(window.resources.common.label.addCustomerPotential);

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });

        $('#PotentialCustomerAddOrEdit').modal();
    }

    self.SavePotentialCustomer = function (data) {
        $('#addCustomer').valid();
        var potentialCustomer = ko.mapping.toJS(self.potentialCustomerModel());
        // Khởi tạo
        potentialCustomer.Created = null;
        potentialCustomer.Updated = null;
        potentialCustomer.Id = 0;
        potentialCustomer.DistrictId = -1;
        potentialCustomer.ProvinceId = -1;
        potentialCustomer.WardId = -1;

        $.post("/CustomerOfStaff/CreateNewPotentialCustomer", { model: potentialCustomer }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.totalPotentialCustomer(result.totalPotialCustomer);
                self.totalPotentialCustomerByUser(result.totalPotialCustomerOfStaff);
                self.GetAllPotentialCustomerList();
                self.potentialCustomerModel(new PotentialCustomerModel());
            }
        });
    }

    //Xóa khach hang TIEM NANG
    self.removePotentialCustomer = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Customer "' + data.FullName + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/CustomerOfStaff/DeletePotentialCustomer", { potentialCustomerId: data.Id }, function (result) { });
            self.GetAllPotentialCustomerList();
            toastr.success("Deleted successfully");
            self.GetInit();
        }, function () {
            toastr.warning("This section does not exist or has been deleted");
        });

        self.GetAllPotentialCustomerList();
    };

    //____________________________________//

    //____________________=======Lấy danh sách dữ liệu đổ vào Search data của khách hàng tiềm năng đang phụ trách
    self.GetPotentialCustomerByUserSearchData = function () {
        self.listSystemPotentialCustomerByUser([]);
        self.listUserPotentialCustomerByUser([]);
        self.listSexPotentialCustomerByUser([]);
        self.listCustomerStypePotentialCustomerByUser([]);
        $.post("/CustomerOfStaff/GetAllPotentialCustomerByUserSearchData", {}, function (data) {
            self.listSystemPotentialCustomerByUser(data.listSystemPotentialCustomerByUser);
            self.listUserPotentialCustomerByUser(data.listUserPotentialCustomerByUser);
            self.listSexPotentialCustomerByUser(data.listSexPotentialCustomerByUser);
            self.listCustomerStypePotentialCustomerByUser(data.listCustomerTypePotentialCustomerByUser);
            self.GetAllPotentialCustomerByUserList();
        });
    }

    //Lấy danh sách khách hàng tiềm năng đang phụ trách
    self.GetAllPotentialCustomerByUserList = function () {
        self.listAllPotentialCustomerByUser([]);
        var SearchPotentialCustomerByUserData = ko.mapping.toJS(self.SearchCustomerModal());

        $.post("/CustomerOfStaff/GetAllPotentialCustomerListByUser", { page: page, pageSize: pagesize, searchModal: SearchPotentialCustomerByUserData }, function (data) {
            total = data.totalRecord;
            self.listAllPotentialCustomerByUser(data.potentialCustomerByUserModal);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }

    //Show form thêm mới khách hàng (CustomerFeasibilityByStaff)
    self.viewAddPotentialCustomerByUser = function (data) {
        self.potentialCustomerModel(new PotentialCustomerModel());
        self.AddOrEdit(0);
        self.titleCustomer('Add new customer account');

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });

        $('#PotentialCustomerByUserAddOrEdit').modal();
    }

    self.SavePotentialCustomerByUser = function (data) {
        $('#addCustomer').valid();
        var potentialCustomerByUser = ko.mapping.toJS(self.potentialCustomerModel());
        // Khởi tạo
        potentialCustomerByUser.Created = null;
        potentialCustomerByUser.Updated = null;
        potentialCustomerByUser.Id = 0;
        potentialCustomerByUser.DistrictId = -1;
        potentialCustomerByUser.ProvinceId = -1;
        potentialCustomerByUser.WardId = -1;

        $.post("/CustomerOfStaff/CreateNewPotentialCustomerByUser", { model: potentialCustomerByUser }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.totalPotentialCustomer(result.totalPotialCustomer);
                self.totalPotentialCustomerByUser(result.totalPotialCustomerOfStaff);
                self.GetAllPotentialCustomerList();
                self.potentialCustomerModel(new PotentialCustomerModel());
            }
        });
    }

    //Show form edit khách hàng
    self.viewEditPotentialCustomerByUser = function (data) {
        self.titleCustomer(window.resources.common.label.customerDetail);
        self.isDetailRending(false);
        self.AddOrEdit(1);
        self.mapPotentialCustomerModel(data);
        $.post("/CustomerOfStaff/GetPotentialCustomerDetail", { potentialCustomerId: data.Id }, function (result) {
            self.isDetailRending(true);
            self.mapPotentialCustomerModel(result.potentialCustomerModal);
            //self.ProvinceId(result.potentialCustomerModal.ProvinceId);

            //self.DistrictId(result.potentialCustomerModal.DistrictId);
            //self.WardId(result.potentialCustomerModal.WardId);

            //self.cacheDistrictId(result.potentialCustomerModal.DistrictId);
            //self.cacheWardId(result.potentialCustomerModal.WardId);

            $(".select-view").select2();
        });

        $('.birthday').datepicker("remove");
        $('.birthday').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy',
            language: 'en'
        });
        $('#PotentialCustomerByUserAddOrEdit').modal();

    }

    //Edit thông tin khách hàng tiềm năng
    self.EditPotentialCustomerByUser = function (data) {
        $('#addCustomer').valid();

        var potentialCustomer = ko.mapping.toJS(self.potentialCustomerModel());

        potentialCustomer.Created = moment(potentialCustomer.Created).format();
        potentialCustomer.Updated = moment(potentialCustomer.Updated).format();
        potentialCustomer.DistrictId = -1;
        potentialCustomer.ProvinceId = -1;
        potentialCustomer.WardId = -1;

        $.post("/CustomerOfStaff/EditPotentialCustomerByUser", { model: potentialCustomer }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetAllPotentialCustomerByUserList();
            }
        });
    }

    self.btnCreateOrEditPotentialCustomerByUser = function (data) {
        if (self.AddOrEdit() === 0) {
            self.SavePotentialCustomerByUser(data);
        }
        else {
            self.EditPotentialCustomerByUser(data);
        }
    } // Button tạo mới khách hàng

    //show chi tiet
    self.viewPotentialCustomerByUserDetail = function (data) {
        self.isDetailRending(false);
        self.titleCustomer(window.resources.common.label.customerDetail);
        self.mapPotentialCustomerModel(data);
        //self.potentialCustomerModel(new PotentialCustomerModel());
        $.post("/CustomerOfStaff/GetPotentialCustomerDetail", { potentialCustomerId: data.Id }, function (result) {
            self.isDetailRending(true);
            $(".select-view").select2();
        });
        $('#PotentialCustomerDetailByUser').modal();

    };

    //____________________=======Lấy danh sách dữ liệu đổ vào Search data của CustomerFeasibilityByStaff============

    self.GetCustomerComplainSearchData = function () {
        self.listComplainStatus([]);
        self.listComplainSystem([]);

        $.post("/CustomerOfStaff/GetAllSearchComplainData", {}, function (data) {
            self.listComplainStatus(data.listStatus);
            self.listComplainSystem(data.listSystem);
            self.GetAllCustomerComplainList();
        });
    }

    self.GetAllCustomerComplainList = function () {
        self.listAllCustomerComplain([]);
        var SearchCustomerComplainData = ko.mapping.toJS(self.SearchCustomerModal());
        $.post("/CustomerOfStaff/GetAllTicketList", { page: page, pageSize: pagesize, searchModal: SearchCustomerComplainData }, function (data) {
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
            self.isLoading(false);
        });
    }

    //================================================Detail KHIẾU NẠI===============
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
    //trao đổi với khách hàng
    self.feedbackComplainModal = function () {
        $('#commentForCustomer').modal();
    }
    self.contentCustomer = ko.observable("");
    //hàm phản hồi cho khách hàng
    self.feedbackComplain = function () {
        self.contentCustomer(ticketDetail.contentCustomer);
        if (self.contentCustomer() === "" || self.contentCustomer() == null) {
            $('#requireCustomer').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: ticketDetail.complainModel().Id(), content: self.contentCustomer(), objectChat: false }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    ticketDetail.contentCustomer("");
                    ticketDetail.GetTicketDetail(ticketDetail.complainModel().Id());
                    
                }
            });
        }
    };

    //================================================Detail KHIẾU NẠI===============

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

    self.active2 = ko.observable('customerDetail');
    self.templateId2 = ko.observable('customerDetail');
    //click menu tab detail
    self.clickMenuDetail = function (name) {
        pageDetail = 1;
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
            if (data.customer != null) {
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
    self.complainuserinternallist = ko.observable([]);
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
            totalDetail = data.totalRecord;
            self.listOrderByCustomer(data.customer);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }
    ///OrderMoney
    self.listOrderMoneyByCustomer = ko.observable([]);
    self.OrderMoney = function () {
        self.listOrderMoneyByCustomer([]);
        $.post("/Ticket/OrderMoney", { customerId: self.customerId(), page: page, pageSize: pagesize }, function (data) {

            totalDetail = data.totalRecord;
            self.listOrderMoneyByCustomer(data.customer);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
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
        if (data.Birthday == null || data.Birthday == undefined || data.Birthday == '') {
            //self.potentialCustomerModel().Birthday(moment(new Date()).format('DD/MM/YYYY'));
        } else {
            self.customerModel().Birthday(moment(data.Birthday).format('DD/MM/YYYY'));
        }
    };

    //#region xử lý Logics
    //==================== Các hàm xử lý Logics ===================================================

    //Thong ke khach hang
    self.viewReportCustomer = function () {
        var start = self.SearchCustomerModal().DateStart();
        $.post("/CustomerOfStaff/GetCustomerReport", { startDay: start }, function (data) {
            $('#staff2').highcharts({
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
                    categories: data.detailName,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    labels: {
                        format: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                series: [{
                    name: 'Staff',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailUser,
                    tooltip: {
                        valueSuffix: ' customer'
                    }
                }]
            });

        });
    }
    self.viewReportCustomerOffStaff = function () {
        var start = self.SearchCustomerModal().DateStart();
        var end = self.SearchCustomerModal().DateEnd();
        $.post("/CustomerOfStaff/GetCustomerOffStaffReport", { startDay: start, endDay: end }, function (data) {
            $('#staff4').highcharts({
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
                    categories: data.detailName,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    labels: {
                        format: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                series: [{
                    name: 'Staff',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailUser,
                    tooltip: {
                        valueSuffix: ' customer'
                    }
                }]
            });
        });
    }
    self.viewReportPotentialCustomer = function () {
        var start = self.SearchCustomerModal().DateStart();
        $.post("/CustomerOfStaff/GetPotentialCustomerReport", { startDay: start }, function (data) {
            $('#staff1').highcharts({
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
                    categories: data.detailName,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    labels: {
                        format: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                series: [{
                    name: 'Staff',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailUser,
                    tooltip: {
                        valueSuffix: ' customer'
                    }
                }]
            });
        });
    }
    self.viewReportPotentialCustomerOffStaff = function () {
        var start = self.SearchCustomerModal().DateStart();
        var end = self.SearchCustomerModal().DateEnd();
        $.post("/CustomerOfStaff/GetPotentialCustomerOffStaffReport", { startDay: start, endDay: end }, function (data) {
            $('#Staff03').highcharts({
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
                    categories: data.detailName,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    labels: {
                        format: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: '',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                series: [{
                    name: 'Staff',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailUser,
                    tooltip: {
                        valueSuffix: ' customer'
                    }
                }]
            });
        });
    }
    self.viewReport = function (data) {

        $('#tongdon').highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45
                }
            },
            title: {
                text: ''
            },
            subtitle: {

            },
            plotOptions: {
                pie: {
                    innerSize: 100,
                    depth: 45
                }
            },
            series: [{
                name: 'Number: ',
                data: data.overview
            }]
        });

        $('#staff').highcharts({
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
                categories: data.detailName,
                crosshair: true
            }],
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                title: {
                    text: '',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                }
            }, { // Secondary yAxis
                title: {
                    text: 'Customer',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value} customer',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                opposite: true
            }],
            tooltip: {
                shared: true
            },
            series: [{
                name: 'Staff',
                type: 'column',
                yAxis: 1,
                data: data.detailUser,
                tooltip: {
                    valueSuffix: ' customer'
                }
            }]
        });
    }

    //==================== Object Map dữ liệu trả về View =========================================
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
        self.customerModel().CardName(data.CardName);
        self.customerModel().CardId(data.CardId);
        self.customerModel().CardBank(data.CardBank);
        self.customerModel().CardBranch(data.CardBranch);
        self.customerModel().WarehouseId(data.WarehouseId);
        self.customerModel().WarehouseName(data.WarehouseName);
        self.customerModel().DepositPrice(data.DepositPrice);
        if (data.Birthday == null || data.Birthday == undefined || data.Birthday == '') {
            //self.potentialCustomerModel().Birthday(moment(new Date()).format('DD/MM/YYYY'));
        } else {
            self.customerModel().Birthday(moment(data.Birthday).format('DD/MM/YYYY'));
        }
    };

    //mapper ds khach hang tiem nang
    self.mapPotentialCustomerModel = function (data) {
        self.potentialCustomerModel(new PotentialCustomerModel());

        self.potentialCustomerModel().Id(data.Id);
        self.potentialCustomerModel().Code(data.Code);
        self.potentialCustomerModel().Email(data.Email);
        self.potentialCustomerModel().FirstName(data.FirstName);
        self.potentialCustomerModel().LastName(data.LastName);
        self.potentialCustomerModel().MidleName(data.MidleName);
        self.potentialCustomerModel().FullName(data.FullName);
        self.potentialCustomerModel().SystemId(data.SystemId);
        self.potentialCustomerModel().SystemName(data.SystemName);
        self.potentialCustomerModel().Phone(data.Phone);
        self.potentialCustomerModel().Avatar(data.Avatar);
        self.potentialCustomerModel().Nickname(data.Nickname);
        if (data.Birthday == null || data.Birthday == undefined || data.Birthday == '') {
            //self.potentialCustomerModel().Birthday(moment(new Date()).format('DD/MM/YYYY'));
        } else {
            self.potentialCustomerModel().Birthday(moment(data.Birthday).format('DD/MM/YYYY'));
        }
        self.potentialCustomerModel().LevelId(data.LevelId);
        self.potentialCustomerModel().LevelName(data.LevelName);
        self.potentialCustomerModel().GenderId(data.GenderId);
        self.potentialCustomerModel().GenderName(data.GenderName);
        self.potentialCustomerModel().DistrictId(data.DistrictId);
        self.potentialCustomerModel().DistrictName(data.DistrictName);
        self.potentialCustomerModel().ProvinceId(data.ProvinceId);
        self.potentialCustomerModel().ProvinceName(data.ProvinceName);
        self.potentialCustomerModel().WardId(data.WardId);
        self.potentialCustomerModel().WardsName(data.WardsName);
        self.potentialCustomerModel().Address(data.Address);
        self.potentialCustomerModel().UserId(data.UserId);
        self.potentialCustomerModel().UserFullName(data.UserFullName);
        self.potentialCustomerModel().Created(data.Created);
        self.potentialCustomerModel().Updated(data.Updated);
        self.potentialCustomerModel().HashTag(data.HashTag);
        self.potentialCustomerModel().CountryId(data.CountryId);
        self.potentialCustomerModel().IsDelete(data.IsDelete);
        self.potentialCustomerModel().CustomerTypeId(data.CustomerTypeId);
        self.potentialCustomerModel().CustomerTypeName(data.CustomerTypeName);
    };

    // Object Detail Khiếu nại
    self.mapComplainModel = function (data) {
        self.complainModel(new complainModel());

        self.complainModel().Id(data.Id);
        self.complainModel().Code(data.Code);
        self.complainModel().TypeOrder(data.TypeOrder);
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

    // Object Detail ComplainUser
    self.mapComplainUserModel = function (data) {
        self.complainUserModel(new complainUserModel());

        self.complainUserModel().Id(data.Id);
        self.complainUserModel().ComplainId(data.ComplainId);
        self.complainUserModel().UserId(data.UserId);
        self.complainUserModel().Content(data.Content);
        self.complainUserModel().AttachFile(data.AttachFile);
        self.complainUserModel().CreateDate(data.CreateDate);
        self.complainUserModel().UpdateDate(data.UpdateDate);
        self.complainUserModel().UserRequestId(data.UserRequestId);
        self.complainUserModel().UserRequestName(data.UserRequestName);
        self.complainUserModel().CustomerId(data.CustomerId);
        self.complainUserModel().CustomerName(data.CustomerName);
        self.complainUserModel().UserName(data.UserName);
        self.complainUserModel().IsRead(data.IsRead);
    };
    // Object Order
    //Hàm map dữ liệu
    self.mapOrderModel = function (data) {
        self.order(new orderModel());

        self.order().Id(data.Id);
        self.order().Code(data.Code);
        self.order().Type(data.Type);
        self.order().WebsiteName(data.WebsiteName);
        self.order().ShopId(data.ShopId);
        self.order().ShopName(data.ShopName);
        self.order().ShopLink(data.ShopLink);
        self.order().ProductNo(data.ProductNo);
        self.order().PackageNo(data.PackageNo);
        self.order().ContractCode(data.ContractCode);
        self.order().ContractCodes(data.ContractCodes);
        self.order().LevelId(data.LevelId);
        self.order().LevelName(data.LevelName);
        self.order().TotalWeight(data.TotalWeight);
        self.order().DiscountType(data.DiscountType);
        self.order().DiscountValue(data.DiscountValue);
        self.order().GiftCode(data.GiftCode);
        self.order().CreatedTool(data.CreatedTool);
        self.order().Currency(data.Currency);
        self.order().ExchangeRate(data.ExchangeRate);
        self.order().TotalExchange(Math.ceil(data.TotalExchange));
        self.order().TotalPrice(data.TotalPrice);
        self.order().Total(data.Total);
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
        self.order().StatusName(data.Status);
        self.order().ReasonCancel(data.ReasonCancel);
        self.order().PriceBargain(data.PriceBargain);

        self.order().ExpectedDate(data.ExpectedDate);
        self.order().PaidShop(data.PaidShop);
        self.order().FeeShip(data.FeeShip);
        self.order().FeeShipBargain(data.FeeShipBargain);
        self.order().IsPayWarehouseShip(data.IsPayWarehouseShip);
        self.order().UserNote(data.UserNote);
        self.order().TotalPriceCustomer(data.TotalPriceCustomer);
        self.order().TotalShop(data.TotalShop);
    }
    self.mapRechargeBillModel = function (data) {
        self.rechargeBill(new rechargeBillDetailModel());

        self.rechargeBill().Id(data.Id);
        self.rechargeBill().Code(data.Code);
        self.rechargeBill().Type(data.Type);
        self.rechargeBill().Status(data.Status);
        self.rechargeBill().Note(data.Note);
        self.rechargeBill().CurrencyFluctuations(data.CurrencyFluctuations);
        self.rechargeBill().Increase(data.Increase);
        self.rechargeBill().Diminishe(data.Diminishe);

        self.rechargeBill().CurencyStart(data.CurencyStart);
        self.rechargeBill().CurencyEnd(data.CurencyEnd);
        self.rechargeBill().UserId(data.UserId);
        self.rechargeBill().UserCode(data.UserCode);
        self.rechargeBill().UserName(data.UserName);
        self.rechargeBill().UserApprovalId(data.UserApprovalId);
        self.rechargeBill().UserApprovalCode(data.UserApprovalCode);
        self.rechargeBill().UserApprovalName(data.UserApprovalName);
        self.rechargeBill().CustomerId(data.CustomerId);
        self.rechargeBill().CustomerCode(data.CustomerCode);
        self.rechargeBill().CustomerName(data.CustomerName);
        self.rechargeBill().CustomerPhone(data.CustomerPhone);
        self.rechargeBill().CustomerEmail(data.CustomerEmail);
        self.rechargeBill().CustomerAddress(data.CustomerAddress);
        self.rechargeBill().TreasureId(data.TreasureId);
        self.rechargeBill().TreasureName(data.TreasureName);
        self.rechargeBill().TreasureIdd(data.TreasureIdd);
        self.rechargeBill().IsAutomatic(data.IsAutomatic);
        self.rechargeBill().OrderId(data.OrderId);
        self.rechargeBill().OrderCode(data.OrderCode);
        self.rechargeBill().OrderType(data.OrderType);
        self.rechargeBill().Created(data.Created);
        self.rechargeBill().LastUpdated(data.LastUpdated);
        self.rechargeBill().IsDelete(data.IsDelete);
    }

    self.mapUserModel = function (data) {
        self.user(new userModel());

        self.user().Id(data.Id);
        self.user().UserName(data.UserName);
        self.user().Password(data.Password);
        self.user().FirstName(data.FirstName);
        self.user().MidleName(data.MidleName);
        self.user().LastName(data.LastName);
        self.user().FullName(data.FullName);
        self.user().UnsignName(data.UnsignName);
        self.user().Gender(data.Gender);
        self.user().Email(data.Email);
        self.user().Description(data.Description);
        self.user().Created(data.Created);
        self.user().Updated(data.Updated);
        self.user().LastUpdateUserId(data.LastUpdateUserId);
        self.user().IsDelete(data.IsDelete);
        self.user().Status(data.Status);
        self.user().Birthday(data.Birthday);
        self.user().StartDate(data.StartDate);
        self.user().Avatar(data.Avatar);
        self.user().IsLockout(data.IsLockout);
        self.user().LastLockoutDate(data.LastLockoutDate);
        self.user().LockoutToDate(data.LockoutToDate);
        self.user().FirstLoginFailureDate(data.FirstLoginFailureDate);
        self.user().LoginFailureCount(data.LoginFailureCount);
        self.user().IsSystem(data.IsSystem);
        self.user().Phone(data.Phone);
        self.user().Mobile(data.Mobile);
        self.user().NameBank(data.NameBank);
        self.user().Department(data.Department);
        self.user().BankAccountNumber(data.BankAccountNumber);
    }

    self.mapcomplainDetailModel = function (data) {
        self.complainDetailModel(new complainDetailModel());
        self.complainDetailModel().Id(data.Id);
        self.complainDetailModel().Avatar(data.Avatar);
        self.complainDetailModel().UserId(data.UserId);
        self.complainDetailModel().Email(data.Email);
        self.complainDetailModel().Status(data.Status);
        self.complainDetailModel().LevelId(data.LevelId);
        self.complainDetailModel().LevelName(data.LevelName);
        self.complainDetailModel().UserFullName(data.UserFullName);
        self.complainDetailModel().Created(data.Created);
        self.complainDetailModel().Updated(data.Updated);
        self.complainDetailModel().IsDelete(data.IsDelete);
        self.complainDetailModel().TotalOrder(data.TotalOrder);
        self.complainDetailModel().TotalOrderAverage(data.TotalOrderAverage);
    };

    // Map doi tuong khach hang cham dat hang
    self.mapcustomerOrderPendingModel = function (data) {
        self.customerOrderPendingModel(new customerOrderPendingModel());
        self.customerOrderPendingModel().Id(data.Id);
        self.customerOrderPendingModel().ComplainId(data.ComplainId);
        self.customerOrderPendingModel().UserId(data.UserId);
        self.customerOrderPendingModel().Content(data.Content);
        self.customerOrderPendingModel().AttachFile(data.AttachFile);
        self.customerOrderPendingModel().CreateDate(data.CreateDate);
        self.customerOrderPendingModel().UpdateDate(data.UpdateDate);
        self.customerOrderPendingModel().UserRequestId(data.UserRequestId);
        self.customerOrderPendingModel().UserRequestName(data.UserRequestName);
        self.customerOrderPendingModel().CustomerId(data.CustomerId);
        self.customerOrderPendingModel().CustomerName(data.CustomerName);
        self.customerOrderPendingModel().UserName(data.UserName);
        self.customerOrderPendingModel().IsRead(data.IsRead);
        self.customerOrderPendingModel().UserPosition(data.UserPosition);
    };

    //Hàm load lại dữ liệu trên các tab
    self.renderSystem2 = function () {
        self.listSystemRenderPoCustomer([]);
        self.listSystemRender([]);
        self.listOrderCustomerStatus([]);
        self.listOrderCustomerType([]);
        $.post("/CustomerOfStaff/GetRenderSystemPotentialCustomer", {
            active: self.active()
        }, function (data) {
            self.listSystemRenderPoCustomer(data.listSystemPotentialCustomer);
            self.listSystemRender(data.listSystemPotentialCustomer);
            self.listOrderCustomerStatus(data.listOrderCustomer);
            self.listOrderCustomerType(data.listOrderCustomerType);
            $('.nav-tabs').tabdrop();
            $(".select-view").select2();
        });
    }

    self.clickTab2 = function (tab) {
        self.SearchPotentialCustomer().SystemId(tab);
        //hiển thị ở trang đầu tiên
        self.search(1);
        $(".select-view").select2();
    }

    //Hàm load lại dữ liệu trên các tab
    self.renderSystem = function () {
        self.listSystemRender([]);
        self.listStatusS([]);
        $.post("/CustomerOfStaff/GetRenderSystem", { active: self.active() }, function (data) {
            self.listStatusS(data.listStatus);
            self.listSystemRender(data.listSystem);

            $('.nav-tabs').tabdrop();
            $(".select-view").select2();
        });
    }

    self.clickTab = function (tab) {
        self.SearchCustomerModal().SystemId(tab);
        //hiển thị ở trang đầu tiên
        self.search(1);
        $(".select-view").select2();
    }

    self.viewOrderByCustomerDetail = function (data) {
        self.isDetailRending(false);
        self.mapOrderModel(data);
        $.post("/CustomerOfStaff/GetListOrderByCustomer", { customerId: data.Id, page: page, pageSize: pagesize }, function (result) {
            self.isDetailRending(true);
            totalDetail = result.totalRecord;
            $(".select-view").select2();
            self.listOrderByCustomer(result.orderByCustomer);
            self.pagingDetail();
        });
        $(".select-view").select2();
    };

    self.viewComplainByCustomerDetail = function (data) {
        self.isDetailRending(false);
        self.mapComplainModel(data);
        $.post("/CustomerOfStaff/GetListComplainByCustomer", { customerId: data.Id }, function (result) {
            self.isDetailRending(true);
            $(".select-view").select2();
            self.listComplainByCustomer(result.complainByCustomer);
            //self.listOrderByCustomer(result.orderByCustomer);
        });
        $(".select-view").select2();
    };

    self.viewOrderMoneyByCustomerDetail = function (data) {
        self.isDetailRending(false);
        self.mapRechargeBillModel(data);
        $.post("/CustomerOfStaff/GetListOrderMoneyByCustomer", { customerId: data.Id, page: page, pageSize: pagesize }, function (result) {
            self.isDetailRending(true);
            totalDetail = result.totalRecord;
            $(".select-view").select2();
            self.listOrderMoneyByCustomer(result.orderMoneyByCustomer);
            self.pagingDetail();
        });
        $(".select-view").select2();
    };

    //click menu tab detail
    self.clickMenuDetail2 = function (name) {
        pageDetail = 1;
        if (name !== self.active2()) {
            //self.init();

            totalDetail = 0;

            pageTotalDetail = 0;
            self.active2(name);
            self.templateId2(name);
            if (name === 'customerDetail') {
                self.viewCustomerDetailModal(self.data());
            }

            if (name === 'OrderHistory1') {
                self.viewOrderByCustomerDetail(self.data());
            }

            if (name === 'SupportHistory') {
                self.viewComplainByCustomerDetail(self.data());
            }
            if (name === 'OrderMoney1') {
                self.viewOrderMoneyByCustomerDetail(self.data());
            }
            //$(".select-view").select2();
        }
    }

    //TODO[Giỏi]: Bổ xung thêm chức năng chuyển từ khách hàng tiềm năng sang Official và ngược lại
    self.customerOfficialPotential = function (data) {
        $.post("/CustomerOfStaff/CustomerOfficialPotential", { id: data.Id }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);

                self.GetAllPotentialCustomerList();
            }
        });
    }

    //____________________=======Thống kê============

    self.GetRevenueReportDepositList = function () {
        self.listRevenueReport([]);
        var searchRevenueData = ko.mapping.toJS(self.SearchCustomerModal());
        self.totalOrderExchange(0);
        self.totalServicePurchase(0);
        self.totalOrderBargain(0);
        self.TotalOrderWeight(0);

        $.post("/CustomerOfStaff/GetRevenueReportDepositList", { page: page, pageSize: pagesize, searchModal: searchRevenueData }, function (data) {
            total = data.totalRecord;
            self.listRevenueReport(data.list);
            self.totalOrderExchange(data.TotalOrderExchange);
            self.totalServicePurchase(data.TotalServicePurchase);
            self.totalOrderBargain(data.TotalOrderBargain);
            self.TotalOrderWeight(data.TotalOrderWeight);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }
    self.GetRevenueReportList = function () {
        self.listRevenueReport([]);
        var searchRevenueData = ko.mapping.toJS(self.SearchCustomerModal());
        self.totalOrderExchange(0);
        self.totalServicePurchase(0);
        self.totalOrderBargain(0);
        self.TotalOrderWeight(0);

        $.post("/CustomerOfStaff/GetRevenueReportList", { page: page, pageSize: pagesize, searchModal: searchRevenueData }, function (data) {
            total = data.totalRecord;
            self.totalOrderExchange(data.TotalOrderExchange);
            self.totalServicePurchase(data.TotalServicePurchase);
            self.TotalOrderWeight(data.TotalOrderWeight);
            self.totalOrderBargain(data.TotalOrderBargain);
            var list = [];
            _.each(data.list, function (item) {
                item.OrderBargain = item.OrderBargain * item.ExchangeRate;
                list.push(item);
            });
            
            self.listRevenueReport(list);
            self.paging();
            self.isRending(true);
            self.isLoading(false);
        });
    }
    // Thống kê khách hàng chậm đặt hàng
    self.GetOrderPendingList = function () {
        self.listOrderPending([]);
        var searchCustomerModal = ko.mapping.toJS(self.SearchCustomerModal());
        $.post("/CustomerOfStaff/GetOrderPendingList", { page: page, pageSize: pagesize, searchModal: searchCustomerModal }, function (data) {
            total = data.totalRecord;
            self.listOrderPending(data.customerModal);
            self.paging();
            self.initInputMark();
            self.isRending(true);
            self.isLoading(false);
        });
    }

    //============================================================= Các hàm hỗ trợ ==============================================

    //============================================================== Export Excel =================================================

    //các biến tìm kiếm
    //self.keyword = ko.observable("");
    //self.status = ko.observable();
    //self.dateStart = ko.observable();
    //self.dateEnd = ko.observable();
    //self.userId = ko.observable();
    // self.customerId = ko.observable();

    self.exportExcelCustomerOfStaff = function () {
        $.redirect("/CustomerOfStaff/ExportExcelCustomerOfStaff",
        {
            keyword: self.keyword(),
            status: self.status() == undefined ? -1 : self.status(),
            userId: self.userId(),
            customerId: self.customerId(),
            dateStart: self.dateStart(),
            dateEnd: self.dateEnd()
        },
        "POST");
    }
    //=========================//
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
    }

    //========================================= BÁO CÁO TÌNH HÌNH KHÁCH HÀNG THEO KHOẢNG THỜI GIAN============================================
    self.reportTitle = ko.observable("Daily report " + moment().format('DD/MM/YYYY'));
    self.titleToday = ko.observable("Today");
    self.selectDateReport = ko.observable("day");
    self.reportDate = ko.observable(moment());
    self.reportDateStart = ko.observable(moment().startOf('day'));
    self.reportDateEnd = ko.observable(moment().endOf('day'));

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
        self.viewReportCustomerSituation();
    }

    self.viewReportCustomerSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.post("/CustomerOfStaff/GetCustomerSituation", { startDay: start, endDay: end }, function (data) {

            self.isLoading(true);
            $("#userCustomerSituation").highcharts({
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
                    labels: {
                        format: '{value} Customer',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Number of customers',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Customer',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} Customer',
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
                    name: 'Number of customers',
                    type: 'column',
                    yAxis: 1,
                    data: data.totalOrder,
                    tooltip: {
                        valueSuffix: ' customer'
                    }

                }]
            });
        });
    }

    //============================================================= XUẤT BÁO CÁO EXCEL ==========================================

    self.ExcelReportCustomerSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        //$.redirect("/CustomerOfStaff/ExcelReportCustomerSituation", { startDay: start, endDay: end }, "POST");
    }

    //Thong ke luong khach hang tiem nang theo ngay
    self.ExcelPotentialCustomerReport = function () {
        var start = self.SearchCustomerModal().DateStart();
        var title = "DAILY NUMBER OF CUSTOMERS/EMPLOYEE "

        $.redirect("/CustomerOfStaff/ExcelPotentialCustomerReport", { titleExcel: title, startDay: start, all: false }, "POST");
    }

    //Thong ke luong khach hang chinh thuc theo ngay
    self.ExcelGetCustomerReport = function () {
        var start = self.SearchCustomerModal().DateStart();
        var title = "DAILY NUMBER OF CUSTOMERS/EMPLOYEE "

        $.redirect("/CustomerOfStaff/ExcelGetCustomerReport", { titleExcel: title, startDay: start, all: false }, "POST");
    }

    //Thong ke luong khach hang tiem nang theo tat ca cac ngay
    self.ExcelGetPotentialCustomerOffStaffReport = function () {
        var start = self.SearchCustomerModal().DateStart();
        var end = self.SearchCustomerModal().DateEnd();
        var title = "NUMBER OF CUSTOMERS/EMPLOYEE "

        $.redirect("/CustomerOfStaff/ExcelGetPotentialCustomerOffStaffReport", { titleExcel: title, startDay: start, endDay: end, all: true }, "POST");
    }

    //Thong ke luong khach hang chinh thuc theo tat ca ngay
    self.ExcelGetCustomerOffStaffReport = function () {
        var start = self.SearchCustomerModal().DateStart();
        var end = self.SearchCustomerModal().DateEnd();
        var title = "NUMBER OF CUSTOMERS/EMPLOYEE "

        $.redirect("/CustomerOfStaff/ExcelGetCustomerOffStaffReport", { titleExcel: title, startDay: start, endDay: end, all: true }, "POST");
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

var accountantDetail = new CustomerLookUp();
ko.applyBindings(accountantDetail, $("#rechargeDetailModal")[0]);

var ticketDetailViewModel = new TicketDetailViewModel();
ko.applyBindings(ticketDetailViewModel, $("#ticketDetailModal")[0]);

var customerViewModel = new CustomerViewModel(orderDetailViewModel, ticketDetailViewModel, depositDetailViewModel, accountantDetail, orderCommerceDetailViewModel);
ko.applyBindings(customerViewModel, $("#customerOffStaffPage")[0]);

//var customerViewModel = new CustomerViewModel();

//ko.applyBindings(customerViewModel);