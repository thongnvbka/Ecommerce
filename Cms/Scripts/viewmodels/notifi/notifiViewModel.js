var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var NotifiViewModel = function () {
    var self = this;
    //========== Các biến cho loading
    self.isLoading = ko.observable(true);
    self.active = ko.observable('Noti');
    self.templateId = ko.observable('Noti');
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);
    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listSystem = ko.observableArray([]);
    self.listSystemNo = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);
    //===Lấy danh sách dữ liệu đổ vào Search data của PotentialCustomer====

    
    //============================================== Init khởi tạo =====================================
    self.GetRenderSystem = function () {
        self.listSystem([]);
        self.listStatus([]);
        self.listSystemNo([]);
        $.ajax({
            type: 'POST',
            url: "/NotifiCommon/GetRenderSystem",
            success: function (response) {
                self.listSystem(response.listSystem);
                self.listStatus(response.listStatus);
                self.listSystemNo(response.listSystemNo);
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

        if (name === 'Noti') {
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

        $.post("/NotifiCommon/GetListData", { page: page, pageSize: pagesize, searchModal: searchSearchData }, function (data) {
            total = data.totalRecord;
            self.listAllData(data.notifiModal);
            console.log(data);
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
        if (self.active() === 'Noti') {
            self.GetAllData();
        }
    };
    self.clickTab = function (tab) {
        self.SearchNotifiModal().SystemId(tab);
        //hiển thị ở trang đầu tiên
        self.search(1);
    }
    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);

        if (self.active() === 'Noti') {
            self.GetAllData();
        }
    }
    self.removeNotifi = function (data) {
        swal({
            title: 'Are you sure to delete the chosen record?',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/NotifiCommon/DeleteNotitfi", { notiId: data.Id }, function (result) {
                self.GetAllData();
                toastr.success("Deleted successfully");
                self.GetInit();
            });
        }, function () {
            toastr.warning("System does not exist or has been deleted");
        });
    };

    //==== xem Detail ====


    ///========== Khai báo Model show dữ liệu trên View
    self.notifiModel = ko.observable(new notifiModel());
    //He thong
    self.systemId = ko.observable();
    self.systemName = ko.observable();
    self.data = ko.observable();
    /// Object Detail thông báo
    self.mapNotiModel = function (data) {
        self.notifiModel(new notifiModel());
        self.notifiModel().Id(data.Id);
        self.notifiModel().SystemId(data.SystemId);
        self.notifiModel().SystemName(data.SystemName);
        self.notifiModel().Description(data.Description);
        self.notifiModel().CreateDate(data.CreateDate);
        self.notifiModel().UpdateDate(data.UpdateDate);
        self.notifiModel().Title(data.Title);
        self.notifiModel().Status(data.Status);
        self.notifiModel().PublishDate(data.PublishDate);
        self.notifiModel().ImagePath(data.ImagePath);
    };
    self.viewNotifiModal = function (data) {
        self.isDetailRending(false);
        $.ajax({
            type: 'GET',
            url: "/NotifiCommon/GetDetailNotitfi",
            data: { 'notiId': data.Id},
            success: function (response) {
                self.isDetailRending(true);
                self.mapNotiModel(response);
                self.data(data);
            },
            async: false
        });
        $('#NotifiAddOrEdit').modal();
    }

    self.addNotifiModal = function (data) {
        self.isDetailRending(true);
        self.notifiModel().Id(0);
        self.notifiModel().SystemName();
        self.notifiModel().Description();
        self.notifiModel().CreateDate();
        self.notifiModel().UpdateDate();
        self.notifiModel().Title();
        self.notifiModel().Status();
        self.notifiModel().PublishDate();
        self.notifiModel().ImagePath();
        self.notifiModel().SystemId($('.ddlAdd-system option:eq(0)').attr('value'));
        $('#NotifiAddOrEdit').modal();
    }

    //Lưu thông tin thông báo thêm mới (SAVE) 
    self.saveNotifi = function (data) {
        self.notifiModel().SystemName($('.ddlAdd-system option:selected').text());
        $.post("/NotifiCommon/CreateNew", { model: self.notifiModel() }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.GetAllData();
            }
        });
    }
    
    //==== Khởi tạo dữ liệu ===
    $(document).ready(function () {
        self.init();

    });
};

var notifiViewModel = new NotifiViewModel();
ko.applyBindings(notifiViewModel, $("#notifiView")[0]);