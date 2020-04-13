var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var PartnerViewModel = function () {
    var self = this;
    //========== Các biến cho loading
    self.isLoading = ko.observable(true);
    self.active = ko.observable('Partner');
    self.templateId = ko.observable('Partner');
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);
    //===== khai báo tìm kiếm =======
    self.listStatus = ko.observableArray([
    { Text: "New partner", Value: "0" },
    { Text: "Current partner", Value: "1" },
    { Text: "Former partner", Value: "2" }
    ]);
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
        //==================== Load dữ liệu mặc định =====================================
        self.isRending(false);
        self.isLoading(false);
        self.GetAllData();
    }

    //==================== Hàm load dữ liệu =====================================
    ///========== Khai báo ListData đổ dữ liệu danh sách
    self.listAllData = ko.observableArray([]);
    ///=======Lấy danh sách dữ liệu============

    self.GetAllData = function () {
        self.listAllData([]);
        $.post("/Partner/GetListData", { page: page, pageSize: pagesize}, function (data) {
            total = data.totalRecord;
            self.listAllData(data.partnerModal);
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
        if (self.active() === 'Partner') {
            self.GetAllData();
        }
    };
    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);

        if (self.active() === 'Partner') {
            self.GetAllData();
        }
    }
    self.removePartner = function (data) {
        swal({
            title: 'You sure you want to delete the selected records?',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/Partner/DeletePartner", { id: data.Id }, function (result) {
                self.GetAllData();
                toastr.success("Deleted successfully");
                self.GetInit();
            });
        }, function () {
            toastr.warning("Notification system does not exist or has been removed");
        });
    };

    //==== xem Detail ====


    ///========== Khai báo Model show dữ liệu trên View
    self.partnerModel = ko.observable(new partnerModel());
    
    /// Object Detail thông báo
    self.mapPartner = function (data) {
        self.partnerModel(new partnerModel());
        self.partnerModel().Id(data.Id);
        self.partnerModel().UnsignName(data.UnsignName);
        self.partnerModel().Code(data.Code);
        self.partnerModel().Name(data.Name);
        self.partnerModel().Description(data.Description);
        self.partnerModel().Note(data.Note);
        self.partnerModel().IsDelete(data.IsDelete);
        self.partnerModel().Status(data.Status);
        self.partnerModel().PriorityNo(data.PriorityNo);
    };
    self.viewPartnerModal = function (data) {
        self.isDetailRending(false);
        $.ajax({
            type: 'GET',
            url: "/Partner/GetDetailPartner",
            data: { 'id': data.Id },
            success: function (response) {
                self.isDetailRending(true);
                self.mapPartner(response);
                self.data(data);
            },
            async: false
        });
        $('#PartnerAddOrEdit').modal();
    }

    self.addPartnerModal = function (data) {
        self.isDetailRending(true);
        self.partnerModel().Id(0);
        self.partnerModel().UnsignName();
        self.partnerModel().Code();
        self.partnerModel().Name();
        self.partnerModel().Description();
        self.partnerModel().Note();
        self.partnerModel().IsDelete();
        self.partnerModel().Status();
        self.partnerModel().PriorityNo();
        $('#PartnerAddOrEdit').modal();
    }

    //Lưu thông tin thông báo thêm mới (SAVE) 
    self.savePartner = function (data) {
        var err = 0;
        if (self.partnerModel().Name().trim().length == 0) {
            toastr.error("Transporters name can not be blank");
            err = 1;
        }
        if (self.partnerModel().Code().trim().length == 0) {
            toastr.error("Transporters code can not be blank");
            err = 1;
        }
        if (err == 0) {
            $.post("/Partner/CreateNew", { model: self.partnerModel() }, function (result) {
                if (!result.msgType) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.GetAllData();
                }
            });
        }
    }

    //==== Khởi tạo dữ liệu ===
    $(document).ready(function () {
        self.init();

    });
};

var partnerViewModel = new PartnerViewModel();
ko.applyBindings(partnerViewModel, $("#partnerView")[0]);