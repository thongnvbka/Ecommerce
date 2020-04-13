var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var ShipViewModel = function (orderDetailViewModel) {
    var self = this;
    //========== Các biến cho loading
    self.isLoading = ko.observable(true);
    self.active = ko.observable('ship');
    self.templateId = ko.observable('ship');
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);
    var statusArray = ko.observableArray([
    { Text: "Not yet processed", Value: 0 },
    { Text: "Processed", Value: 1 }
    ]);
    self.statusArray = statusArray;
    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listStatus = ko.observableArray([]);
    //===Lấy danh sách dữ liệu đổ vào Search data của PotentialCustomer====


    //============================================== Init khởi tạo =====================================
    self.GetRenderSystem = function () {
        self.listStatus([]);
        $.ajax({
            type: 'POST',
            url: "/RequestShip/GetRenderSystem",
            success: function (response) {
                self.listStatus(response.listStatus);
            },
            async: false
        });

    }

    //==================== Search Object - Customer
    self.SearchNotifiModal = ko.observable({
        Keyword: ko.observable(""),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchNotifiModal = ko.observable(self.SearchNotifiModal());
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

    self.init = function () {
        $('#daterange-btn').daterangepicker(
              {
                  ranges: {
                      'Today' : [moment(), moment()],
                      'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                      '7 days ago': [moment().subtract(6, 'days'), moment()],
                      '30 days ago': [moment().subtract(29, 'days'), moment()],
                      'This month': [moment().startOf('month'), moment().endOf('month')],
                      'Last month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                      'All': ['', '']
                  },
                  startDate: moment().subtract(29, 'days'),
                  endDate: moment()
              },
              function (start, end) {
                  if (start.format() === 'Invalid date') {
                      $('#daterange-btn span').html('Created date');
                      self.SearchNotifiModal().DateStart('');
                      self.SearchNotifiModal().DateEnd('');
                  }
                  else {
                      $('#daterange-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchNotifiModal().DateStart(start.format());
                      self.SearchNotifiModal().DateEnd(end.format());
                  }

              }
          );

        //Date picker
        $('#datepicker').datepicker({
            autoclose: true
        });
        //==================== Load dữ liệu mặc định =====================================
        self.GetRenderSystem();
        self.isRending(false);
        self.isLoading(false);
        self.GetAllData();
    }

    //==================== Các sự kiện click menu =====================================
    self.clickMenu = function (name) {

        //self.active(name);
        self.templateId(name);

        total = 0;
        page = 1;
        pageTotal = 0;

        if (name === 'ship') {
            self.isRending(false);
            self.isLoading(false);
            self.GetAllData();
        }
    }
    //==================== Hàm load dữ liệu =====================================
    ///========== Khai báo ListData đổ dữ liệu danh sách
    self.listAllData = ko.observableArray([]);
    ///=======Lấy danh sách dữ liệu============

    self.GetAllData = function () {
        self.listAllData([]);
        ///==== Lấy thông tin tìm kiếm ====
        var searchSearchData = ko.mapping.toJS(self.SearchNotifiModal());

        $.post("/RequestShip/GetListData", { page: page, pageSize: pagesize, searchModal: searchSearchData }, function (data) {
            total = data.totalRecord;
            self.listAllData(data.requestModal);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    ///tìm kiếm
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);
        if (self.active() === 'ship') {
            self.GetAllData();
        }
    };
    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);

        if (self.active() === 'ship') {
            self.GetAllData();
        }
    }
    //Click detail order
    self.showOrderDetail = function (orderId) {
        orderDetailViewModel.viewOrderDetail(orderId);
    }

    // khai bao chi tiet yeu cau ship
    self.requestShip = ko.observable(new requestShipModel());
    self.listPackageView = ko.observableArray([]);
    self.mapRequestModel = function (data) {
        self.requestShip(new requestShipModel());
        self.requestShip().Id(data.Id);
        self.requestShip().Code(data.Code);
        self.requestShip().OrderId(data.OrderId);
        self.requestShip().OrderCode(data.OrderCode);
        self.requestShip().PackageCode(data.PackageCode);
        self.requestShip().UpdateDate(data.UpdateDate);
        self.requestShip().CreateDate(data.CreateDate);
        self.requestShip().CustomerId(data.CustomerId);
        self.requestShip().CustomerName(data.CustomerName);
        self.requestShip().CustomerEmail(data.CustomerEmail);
        self.requestShip().CustomerPhone(data.CustomerPhone);
        self.requestShip().CustomerAddress(data.CustomerAddress);
        self.requestShip().Status(data.Status);
        self.requestShip().SystemId(data.SystemId);
        self.requestShip().SystemName(data.SystemName);
        self.requestShip().IsDelete(data.IsDelete);
        self.requestShip().Note(data.Note);
    };
    self.viewRequestModal = function (data) {
        self.isDetailRending(false);
        self.requestShip(new requestShipModel());
        self.listPackageView([]);

        $.ajax({
            type: 'GET',
            url: "/RequestShip/GetDetail",
            data: { 'id': data.Id },
            success: function (response) {
                self.isDetailRending(true);
                self.mapRequestModel(response.RequestShipDetail);
                self.listPackageView(response.ListPackage);
            },
            async: false
        });
        $('#requestShipDetailModal').modal();
        $('.ddlAdd-status').select2();
    }
    //Lưu thông tin thông báo thêm mới (SAVE) 
    self.saveRequestShip = function (data) {
        $.post("/RequestShip/CreateNew", { model: self.requestShip() }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                $('#requestShipDetailModal').modal('toggle');
                self.GetAllData();
            }
        });
    }
    //==== Khởi tạo dữ liệu ===
    $(document).ready(function () {
        self.init();

    });
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

var shipViewModel = new ShipViewModel(orderDetailViewModel);
ko.applyBindings(shipViewModel, $("#ShipView")[0]);
