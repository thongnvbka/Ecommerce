var total = 0;
var page = 1;
var pagesize = 4;
var pageTotal = 0;


var customerViewModel = function () {
    var self = this;
    //#region khai báo chung nhất isLoading,isDetailRending

    //========== Các biến cho template
    self.active = ko.observable('');
    self.templateId = ko.observable('');

    self.totalCustomerList = ko.observable();

    //========== Các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);


    //#endregion
     
    //#region hàm phân trang chung
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

    //==================== Khởi tạo ===================================================
    $(function () {
        self.init();
        //self.clickSearch();
    });

    self.init = function () {
        $('.nav-tabs').tabdrop();

        $('#daterange-btn').daterangepicker(
              {
                  ranges: {
                      'Today' : [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                      '30 days ago': [moment().subtract(29, 'days'), moment()],
                      'This month': [moment().startOf('month'), moment().endOf('month')],
                      'Last month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                  },
                  startDate: moment().subtract(29, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  $('#daterange-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
              }
          );

        $(".select-view").select2();
    }

    //#endregion


    //#region quản lý khách hàng của phòng
    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listUser = ko.observableArray([]);
    self.listCustomerType = ko.observableArray([]); 
    self.listStatus = ko.observableArray([]);
    self.listSexCustomer = ko.observableArray([]);

    //He thong
    self.listSystem = ko.observableArray([]);
    self.systemId = ko.observable();
    self.systemName = ko.observable();

    //========== Khai báo ListData đổ dữ liệu danh sách
    self.listAllCustomer = ko.observableArray([]);

    // Lấy danh sách khach hang
    self.GetAllCustomerList = function () {
        self.listAllCustomer([]);
        var SearchCustomData = ko.mapping.toJS(self.SearchCustomerModal());

        $.post("/CustomerOfStaff/GetAllCustomerList", { page: page, pageSize: pagesize, searchModal: SearchCustomData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomer(data.customerModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy toàn bộ danh đổ lên Form Search khách hàng
    self.GetCustomerSearchData = function () {
        self.listSystem([]);
        self.listStatus([]);
        self.listUser([]);
        self.listCustomerType([]);

        $.post("/CustomerOfStaff/GetAllCustomerSearchData", {}, function (data) {
            self.listSystem(data.listSystem);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);
            self.listSexCustomer(data.listSexCustomer);
            self.listCustomerType(data.listCustomerType);

        });
    }
    //Lấy ra danh sách Detail khách hàng

    self.GetCustomerOfStaffDetail = function (data) {

        self.mapCustomerModel(data);
        $.post("/CustomerOfStaff/GetCustomerOfStaffDetail", { customerOfStaffId: data.Id }, function (result) {
            self.customerOfStafDetail(result.customerModal);

            console.log(self.customerOfStafDetail().Code);
        });
    }
    //#endregion

    //#region Quản lý khách hàng Official đang phụ trách 
    //===========khai báo dữ liệ đổ về dữ liệu search trên view CustomerByStaff================

    self.listSystemByStaff = ko.observableArray([]);
    self.listStatusByStaff = ko.observableArray([]);
    self.listUserByStaff = ko.observableArray([]);
    self.listCustomerTypeByStaff = ko.observableArray([]);

    //==========Khai báo Listdata đổ dữ liệu vào danh sách CustomerByStaff==========
    self.listAllCustomerByStaff = ko.observableArray([]);

     
    //============Lấy danh sách dữ liệu đổ vào Search data của CustomerByStaff============
    self.GetCustomerByStaffSearchData = function () {
        self.listSystemByStaff([]);
        self.listStatusByStaff([]);
        self.listUserByStaff([]);
        self.listCustomerTypeByStaff([]);

        $.post("/CustomerOfStaff/GetAllCustomerByStaffSearchData", {}, function (data) {
            self.listSystemByStaff(data.listSystemByStaff);
            self.listStatusByStaff(data.listStatusByStaff);
            self.listUserByStaff(data.listUserByStaff);
            self.listCustomerTypeByStaff(data.listCustomerTypeByStaff);

        });
    }
    //Lấy danh sách khách hàng Official phụ trách
    self.GetAllCustomerByStaffList = function () {
        self.listAllCustomerByStaff([]);
        var SearchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerByStaffModal());

        $.post("/CustomerOfStaff/GetAllCustomerByStaffList", { page: page, pageSize: pagesize, searchModal: SearchCustomByStaffData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomerByStaff(data.customerByStaffModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }
     
    //#endregion
     

    //#region Danh sách khách hàng tiềm năng của phòng
    //===========khai báo dữ liệu đổ về dữ liệu search trên view CustomerFeasibilityByOffice================

    self.listSystemFeasibilityByOffice = ko.observableArray([]);
    self.listStatusFeasibilityByOffice = ko.observableArray([]);
    self.listUserFeasibilityByOffice = ko.observableArray([]);
    self.listCustomerTypeFeasibilityByOffice = ko.observableArray([]);
     

    //==========Khai báo Listdata đổ dữ liệu vào danh sách CustomerFeasibilityByOffice==========
    self.listAllCustomerFeasibilityByOffice = ko.observableArray([]);

     
    //============Lấy danh sách dữ liệu đổ vào Search data của CustomerFeasibilityByOffice============
    self.GetCustomerFeasibilityByOfficeSearchData = function () {
        self.listSystemFeasibilityByOffice([]);
        self.listStatusFeasibilityByOffice([]);
        self.listUserFeasibilityByOffice([]);
        self.listCustomerTypeFeasibilityByOffice([]);

        $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByOfficeSearchData", {}, function (data) {
            self.listSystemFeasibilityByOffice(data.listSystemFeasibilityByOffice);
            self.listStatusFeasibilityByOffice(data.listStatusFeasibilityByOffice);
            self.listUserFeasibilityByOffice(data.listUserFeasibilityByOffice);
            self.listCustomerTypeFeasibilityByOffice(data.listCustomerTypeFeasibilityByOffice);

        });
    }

    //Lấy danh sách khách hàng tiềm năng của phòng
    self.GetAllFeasibilityByOfficeList = function () {
        self.listAllCustomerFeasibilityByOffice([]);
        var SearchCustomFeasibilityByOfficeData = ko.mapping.toJS(self.SearchCustomerFeasibilityByOfficeModal());

        $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByOfficeList", { page: page, pageSize: pagesize, searchModal: SearchCustomFeasibilityByOfficeData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomerFeasibilityByOffice(data.customerFeasibilityByOfficeModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    //#endregion


    //#region Khách hàng tiềm năng đang phụ trách

    //===========khai báo dữ liệ đổ về dữ liệu search trên view CustomerFeasibilityByStaff================

    self.listSystemFeasibilityByStaff = ko.observableArray([]);
    self.listStatusFeasibilityByStaff = ko.observableArray([]);
    self.listUserFeasibilityByStaff = ko.observableArray([]);
    self.listCustomerTypeFeasibilityByStaff = ko.observableArray([]);


    //==========Khai báo Listdata đổ dữ liệu vào danh sách CustomerFeasibilityByStaff==========
    self.listAllCustomerFeasibilityByStaff = ko.observableArray([]);


    //============Lấy danh sách dữ liệu đổ vào Search data của CustomerFeasibilityByStaff============
    self.GetCustomerFeasibilityByStaffSearchData = function () {
        self.listSystemFeasibilityByStaff([]);
        self.listStatusFeasibilityByStaff([]);
        self.listUserFeasibilityByStaff([]);
        self.listCustomerTypeFeasibilityByStaff([]);

        $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByStaffSearchData", {}, function (data) {
            self.listSystemFeasibilityByStaff(data.listSystemFeasibilityByStaff);
            self.listStatusFeasibilityByStaff(data.listStatusFeasibilityByStaff);
            self.listUserFeasibilityByStaff(data.listUserFeasibilityByStaff);
            self.listCustomerTypeFeasibilityByStaff(data.listCustomerTypeFeasibilityByStaff);

        });
    }

    //Lấy danh sách khách hàng tiềm năng đang phụ trách
    self.GetAllFeasibilityByStaffList = function () {
        self.listAllCustomerFeasibilityByStaff([]);
        var SearchCustomFeasibilityByStaffData = ko.mapping.toJS(self.SearchCustomerFeasibilityByStaffModal());

        $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByStaffList", { page: page, pageSize: pagesize, searchModal: SearchCustomFeasibilityByStaffData }, function (data) {
            total = data.totalRecord;
            self.listAllCustomerFeasibilityByStaff(data.customerFeasibilityByStaffModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }
     
    //#endregion

    //#region ==================== Khai báo các Object ViewModal =====================================
    // Search Object - Customer
    self.SearchCustomerModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        CustomerTypeId: ko.observable(-1),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchCustomerModal = ko.observable(self.SearchCustomerModal());

    // Search Object - CustomerByStaff 
    self.SearchCustomerByStaffModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        CustomerTypeId: ko.observable(-1),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchCustomerByStaffModal = ko.observable(self.SearchCustomerByStaffModal());

    // Search Object - CustomerFeasibilityByOffice
    self.SearchCustomerFeasibilityByOfficeModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        CustomerTypeId: ko.observable(-1),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1) 
    });
    self.SearchCustomerFeasibilityByOfficeModal = ko.observable(self.SearchCustomerFeasibilityByOfficeModal());

    // Search Object - CustomerFeasibilityByStaff
    self.SearchCustomerFeasibilityByStaffModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        CustomerTypeId: ko.observable(-1),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchCustomerFeasibilityByStaffModal = ko.observable(self.SearchCustomerFeasibilityByStaffModal());

    // Search Object - CustomerComplain
    self.SearchCustomerComplainModal = ko.observable({
        Keyword: ko.observable(""),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1),
        CountryId:ko.observable(-1)
    });
    self.SearchCustomerComplainModal = ko.observable(self.SearchCustomerComplainModal());


    //#endregion

    //#region Khieu nai
    //===========khai báo dữ liệ đổ về dữ liệu search trên view khiếu nại================
    self.listComplainStatus = ko.observableArray([]);
    self.listComplainSystem = ko.observableArray([]);
    self.listCustomerCountry = ko.observableArray([]);
   
    //==========Khai báo Listdata đổ dữ liệu vào danh sách CustomerFeasibilityByStaff==========
    self.listAllCustomerComplain = ko.observableArray([]);

    //============Lấy danh sách dữ liệu đổ vào Search data của CustomerFeasibilityByStaff============
    self.GetCustomerComplainSearchData = function () {
        self.listComplainStatus([]);
        self.listComplainSystem([]);
        self.listCustomerCountry([]); 

        $.post("/CustomerOfStaff/GetAllCustomerComplainSearchData", {}, function (data) {
            self.listComplainStatus(data.listComplainStatus);
            self.listComplainSystem(data.listComplainSystem);
            self.listCustomerCountry(data.listCustomerCountry); 
        });
    }

    // 
    self.GetAllCustomerComplainList = function () {
        self.listAllCustomerComplain([]);
        var SearchCustomerComplainData = ko.mapping.toJS(self.SearchCustomerComplainModal());

        $.post("/CustomerOfStaff/GetCustomerComplainList", { page: page, pageSize: pagesize, searchModal: SearchCustomerComplainData }, function (data) {
            //total = data.totalRecord;
            self.listAllCustomerComplain(data.customerComplainModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    //#endregion



    ///========== Khai báo Model show dữ liệu trên View
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainModel = ko.observable(new complainModel());
    self.complainUserModel = ko.observable(new complainUserModel());
 
    //==================== Các sự kiện click menu =====================================
    self.clickMenu = function (name) {
        if (name !== self.active()) {
            self.init();

            total = 0;
            page = 1;
            pageTotal = 0;

            self.active(name);
            self.templateId(name);

            //Gọi page danh sách khachs hangf
            if (name === 'customer') {
                self.isRending(false);
                self.isLoading(false);

                self.GetCustomerSearchData();
                self.GetAllCustomerList();
                //self.isRending(true);
            }

            if (name === 'customer-by-staff') {
                self.isRending(false);
                self.isLoading(false);

                self.GetCustomerByStaffSearchData();
                self.GetAllCustomerByStaffList();    
                //self.isRending(true);
            }


            if (name === 'customerfeasibility-by-office') {
                self.isRending(false);
                self.isLoading(false);

                self.GetCustomerFeasibilityByOfficeSearchData();
                self.GetAllFeasibilityByOfficeList();
                //self.isRending(true);
            }

            if (name === 'customerfeasibility-by-staff') {
                self.isRending(false);
                self.isLoading(false);

                self.GetCustomerFeasibilityByStaffSearchData();
                self.GetAllFeasibilityByStaffList();
                //self.isRending(true);
            }

            if (name === 'ticket-support') {
                self.isRending(false);
                self.isLoading(false);

                self.GetCustomerComplainSearchData();
                self.GetAllCustomerComplainList();
                //self.isRending(true);
            }

            //self.clickSearch();
            $(".select-view").select2();
        }
    }

    self.clickTab = function (tab) {
        //self.systemId(tab);
        //self.search(1);
        $(".select-view").select2();
    }


    //#region  Tìm kiếm  
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);

        if (self.active() === 'customer') {
            self.listAllCustomer([]);
            var SearchCustomData = ko.mapping.toJS(self.SearchCustomerModal());
            $.post("/CustomerOfStaff/GetAllCustomerList", { page: page, pageSize: pagesize, searchModal: SearchCustomData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomer(data.customerModal);
                self.paging();
                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'customer-by-staff') {

            self.listAllCustomerByStaff([]);
            var SearchCustomByStaffData = ko.mapping.toJS(self.SearchCustomerByStaffModal());

            $.post("/CustomerOfStaff/GetAllCustomerByStaffList", { page: page, pageSize: pagesize, searchModal: SearchCustomByStaffData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomerByStaff(data.customerByStaffModal);
                self.paging();

                self.isRending(true);
                self.isLoading(false);
            }); 
        }

        if (self.active() === 'customerfeasibility-by-office') {

            self.listAllCustomerFeasibilityByOffice([]);
            var SearchCustomFeasibilityByOfficeData = ko.mapping.toJS(self.SearchCustomerFeasibilityByOfficeModal());

            $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByOfficeList", { page: page, pageSize: pagesize, searchModal: SearchCustomFeasibilityByOfficeData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomerFeasibilityByOffice(data.customerFeasibilityByOfficeModal);
                self.paging();

                self.isRending(true);
                self.isLoading(false);
            });
        }

        if (self.active() === 'customerfeasibility-by-staff') {

            self.listAllCustomerFeasibilityByStaff([]);
            var SearchCustomFeasibilityByStaffData = ko.mapping.toJS(self.SearchCustomerFeasibilityByStaffModal());

            $.post("/CustomerOfStaff/GetAllCustomerFeasibilityByStaffList", { page: page, pageSize: pagesize, searchModal: SearchCustomFeasibilityByStaffData }, function (data) {
                total = data.totalRecord;
                self.listAllCustomerFeasibilityByStaff(data.customerFeasibilityByStaffModal);
                self.paging();

                self.isRending(true);
                self.isLoading(false);
            });
        }


        if (self.active() === 'ticket-support') {
            self.listAllCustomerComplain([]);
            var SearchCustomerComplainData = ko.mapping.toJS(self.SearchCustomerComplainModal());

            $.post("/CustomerOfStaff/GetCustomerComplainList", { page: page, pageSize: pagesize, searchModal: SearchCustomerComplainData }, function (data) {
                //total = data.totalRecord;
                self.listAllCustomerComplain(data.customerComplainModal);
                self.paging();

                self.isRending(true);
                self.isLoading(false);
            });
        }
    };

    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);

        if (self.active() === 'customer') {
            self.GetAllCustomerList();
        }
        if (self.active() === 'customer-by-staff') {
            self.GetAllCustomerByStaffList();
        }
        if (self.active() === 'customerfeasibility-by-office') {
            self.GetAllFeasibilityByOfficeList();
        }

        if (self.active() === 'customerfeasibility-by-staff') {
            self.GetAllFeasibilityByStaffList();
        }
        if (self.active() === 'ticket-support') {
            self.GetAllCustomerComplainList();
        }
        self.isRending(true);
        self.isLoading(false);
    }
    //#endregion
    //#region xử lý Logics
    //==================== Các hàm xử lý Logics ===================================================
    self.viewReport = function () {
        $.post("/Purchase/GetOrderReport", function (data) {
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
                    //text: 'Sum Orders hôm nay'
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
                        format: '{value} CNY',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Total revenue',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Order',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} order',
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
                    name: 'Order',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailOrder,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revene',
                    type: 'spline',
                    data: data.detailPrice,
                    tooltip: {
                        valueSuffix: ' CNY'
                    }
                }]
            });
        });
    }
    //#endregion

    //#region xử lý sự kiện
    //==================== Các sự kiện xử lý ==========================================
    self.viewTicketDetail = function (data) {
        $('#ticketDetailModal').modal();
    }

    self.viewCustomerAddOrEdit = function (data) {
        $('#CustomerAddOrEdit').modal();
    }

    self.viewCustomerDetailModal = function (data) {
        self.GetCustomerOfStaffDetail(data);
        $('#CustomerDetailModal').modal();
    }

    //#endregion
  
    //#region them moi khách hàng


    //lưu thông tin Orders ký gửi
    self.saveCustomer = function () {
        $('#addCustomer').valid();
        $('#inforCustomer').valid();
        $.post("/CustomerOfStaff/CustomerCreate", { customer: self.customerModel() }, function (result) {
            if (result > 0) {
                toastr.success('Add customer:' + self.customerModel().Code() + ' is successful!');
                $('#CustomerAddOrEdit').modal('hide'); 
            }
            else if (result < 0) {
                toastr.error('Add customer' + 'failed!'); 
            }
          

        });
         
    }
    //#endregion

    
    //#region Map object
  

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
        self.customerModel().Phone(data.Phone);
        self.customerModel().Avatar(data.Avatar);
        self.customerModel().Nickname(data.Nickname);
        self.customerModel().Birthday(data.Birthday);
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
        self.customerModel().CustomerTypeId(data.CustomerTypeId);

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
    self.mapComplainModel = function (data) {
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
    //#endregion 

};

ko.applyBindings(new customerViewModel());