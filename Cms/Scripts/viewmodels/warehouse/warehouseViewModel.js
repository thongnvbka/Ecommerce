var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;



var warehouseViewModel = function () {
    var self = this;
    //========== Các biến cho template
    self.active = ko.observable('originate');
    self.templateId = ko.observable('originate');

    self.totalImportWarehouseList = ko.observable();

    //========== Các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);

    //==================== Khai báo các Object ViewModal =====================================
    // Search Object - ImportWarehouse
    self.SearchPackageModal = ko.observable({
        Keyword: ko.observable(""),
        OrderId: ko.observable(-1),
        WarehouseId: ko.observable(-1),
        CustomerId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchPackageData = ko.observable(self.SearchPackageModal());

    // Search Object - ImportWarehouse
    self.SearchImportModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        WarehouseId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchImportData = ko.observable(self.SearchImportModal());

    // Search Object - Wallet
    self.SearchWalletModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        WarehouseId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchWalletData = ko.observable(self.SearchWalletModal());

    // Search Object - ExportWarehouse
    self.SearchExportModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        WarehouseId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchExportData = ko.observable(self.SearchExportModal());

    // Search Object - PackingList
    self.SearchPackingModal = ko.observable({
        Keyword: ko.observable(""),
        UserId: ko.observable(-1),
        WarehouseSourceId: ko.observable(-1),
        WarehouseDesId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchExportData = ko.observable(self.SearchExportModal());

    // Search Object - Delivery
    self.SearchDeliveryModal = ko.observable({
        Keyword: ko.observable(""),
        CustomerId: ko.observable(-1),
        UserId: ko.observable(-1),
        WarehouseSourceId: ko.observable(-1),
        WarehouseDesId: ko.observable(-1),
        Status: ko.observable(-1)
    });
    self.SearchDeliveryData = ko.observable(self.SearchDeliveryModal());

    //========== Khai báo ListData đổ về dữ liệu Search trên View
    self.listWarehouse = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);
    self.listUser = ko.observableArray([]);
    self.listCustomer = ko.observableArray([]);
    self.listOrder = ko.observableArray([]);


    //========== Khai báo ListData đổ dữ liệu danh sách
    self.listPackage = ko.observableArray([]);
    self.listImportWarehouse = ko.observableArray([]);
    self.listWallet = ko.observableArray([]);
    self.listExportWarehouse = ko.observableArray([]);
    self.listPacking = ko.observableArray([]);
    self.listDelivery = ko.observableArray([]);
    self.deliveryDetail = ko.observableArray([]);

    ///========== Khai báo Model show dữ liệu trên View
    //self.packageModel = ko.observable(new packageDetailModel());
    self.importModel = ko.observable(new importWarehouseModel());
    self.walletModel = ko.observable(new walletModel());
    self.exportModel = ko.observable(new exportWarehouseModel());
    self.deliveryModel = ko.observable(new deliveryDetailModel());
    self.packingModel = ko.observable(new packingListModel());


    //==================== Các sự kiện show Modal ==========================================
    // Thông tin Detail package Package
    self.viewpackageDetailModal = function (data) {
        //self.GetPackageDetail(data);
        $('#packageDetailModal').modal();
    }

    // Thông tin Detail ImportWarehouse
    self.viewImportWarehouseDetailModal = function (data) {
        self.GetImportWarehouseDetail(data);
        $('#importWarehouseDetailModal').modal();
    }

    // importWarehouseAddOrEdit
    self.viewImportWarehouseAddOrEdit = function () {
        $('#importWarehouseAddOrEdit').modal();
    }

    // ExportWarehouse
    self.viewExportWarehouseDetailModal = function (data) {
        self.GetExportWarehouseDetail(data);
        $('#exportWarehouseDetailModal').modal();
    }

    // exportWarehouseAddOrEdit
    self.viewExportWarehouseAddOrEdit = function () {
        $('#exportWarehouseAddOrEdit').modal();
    }

    // Wallet
    self.viewWalletDetailModal = function (data) {
        self.GetWalletDetail(data);
        $('#walletDetailModal').modal();
    }

    // walletAddOrEdit
    self.viewWalletAddOrEdit = function () {
        $('#walletAddOrEdit').modal();
    }

    //Packing
    self.viewPackingDetailModal = function (data) {
        self.GetPackingListDetail(data);
        $('#packingDetailModal').modal();
    }

    // Transport
    self.viewTransportDetailModal = function (data) {
        self.GetTransportDetail(data);
        $('#transportDetailModal').modal();
    }
    // transportAddOrEdit
    self.viewTransportAddOrEdit = function () {
        $('#transportAddOrEdit').modal();
    }

    // Delivery
    self.viewDeliveryDetailModal = function (data) {
        self.GetDeliveryDetail(data);
        $('#deliveryDetailModal').modal();
    }

    // Ticket
    self.viewTicketDetail = function () {
        $('#ticketDetailModal').modal();
    }

    self.viewCustomerDetailModal = function () {
        $('#CustomerDetailModal').modal();
    }

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

    //==================== Các sự kiện click menu =====================================
    self.clickMenu = function (name) {
        if (name !== self.active()) {
            self.init();

            total = 0;
            page = 1;
            pageTotal = 0;

            self.active(name);
            self.templateId(name);

            //Gọi page danh sách package - Package
            if (name === 'package') {
                self.isRending(false);
                self.isLoading(false);

                self.GetPackageSearchData();
                self.GetAllPackageList();
                //self.isRending(true);
            }

            //Gọi page danh sách phiếu nhập kho
            if (name === 'importwarehouse') {
                self.isRending(false);
                self.isLoading(false);

                self.GetImportWarehouseSearchData();
                self.GetAllImportWarehouseList();
                //self.isRending(true);
            }

            //Gọi page danh sách bao hàng
            if (name === 'wallet') {
                self.isRending(false);
                self.isLoading(false);

                self.GetWalletSearchData();
                self.GetAllWalletList();
                //self.isRending(true);
            }

            //Gọi page danh sách bao hàng
            if (name === 'exportwarehouse') {
                self.isRending(false);
                self.isLoading(false);

                self.GetExportWarehouseSearchData();
                self.GetAllExportWarehouseList();
                //self.isRending(true);
            }

            //Gọi page danh sách bao hàng
            if (name === 'packing') {
                self.isRending(false);
                self.isLoading(false);

                self.GetPackingListSearchData();
                self.GetAllPackingList();
                //self.isRending(true);
            }

            //Gọi page danh sách bao hàng
            if (name === 'delivery') {
                self.isRending(false);
                self.isLoading(false);

                self.GetDeliverySearchData();
                self.GetAllDeliveryList();
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

    //==================== Tìm kiếm ===================================================
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);

        if (self.active() === 'importwarehouse') {
            self.listImportWarehouse([]);
            var SearchImportData = ko.mapping.toJS(self.SearchImportModal());

            $.post("/Warehouse/GetAllImportWarehouseList", { page: page, pageSize: pagesize, SearchImportModal: SearchImportData }, function (data) {
                total = data.totalRecord;
                self.listImportWarehouse(data.importWarehouseModal);
                self.paging();
                self.isRending(true);
            });
        }
    };

    // Click Search dữ liệu
    self.clickSearch = function (data, event) {
        self.isLoading(true);
        self.isRending(false);

        if (self.active() === 'package') {
            self.GetAllPackageList();
        }

        if (self.active() === 'importwarehouse') {
            self.GetAllImportWarehouseList();
        }

        if (self.active() === 'wallet') {
            self.GetAllWalletList();
        }

        if (self.active() === 'exportwarehouse') {
            self.GetAllExportWarehouseList();
        }

        if (self.active() === 'packing') {
            self.GetAllPackingList();
        }

        if (self.active() === 'delivery') {
            self.GetAllDeliveryList();
        }

        self.isRending(true);
        self.isLoading(false);
    }
    //==================== Các hàm xử lý Logics ===================================================
    //======== Package
    // Lấy toàn bộ danh đổ lên Form Search package
    self.GetPackageSearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listCustomer([]);
        self.listOrder([]);

        $.post("/Package/GetPackageSearchData", {}, function (data) {
            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listCustomer(data.listCustomer);
        });
    }

    // Lấy danh sách phiếu nhập kho
    self.GetAllPackageList = function () {
        self.listPackage([]);
        var SearchPackageData = ko.mapping.toJS(self.SearchPackageModal());

        $.post("/Package/GetAllPackageList", { page: page, pageSize: pagesize, searchModal: SearchPackageData }, function (data) {
            total = data.totalRecord;
            self.listPackage(data.packageModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail phiếu nhập kho
    self.GetPackageDetail = function (data) {
        $.post("/Package/GetImportWarehouseDetail", { importWarehouseId: data.Id }, function (result) {
            self.mapImportModel(result.importWarehouseModal);
        });
    }

    //======== ImportWarehouse
    // Lấy toàn bộ danh đổ lên Form Search phiếu nhập kho
    self.GetImportWarehouseSearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listUser([]);

        $.post("/Warehouse/GetImportWarehouseSearchData", {}, function (data) {
            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);

        });
    }
    // Lấy danh sách phiếu nhập kho
    self.GetAllImportWarehouseList = function () {
        self.listImportWarehouse([]);
        var SearchImportData = ko.mapping.toJS(self.SearchImportModal());

        $.post("/Warehouse/GetAllImportWarehouseList", { page: page, pageSize: pagesize, searchModal: SearchImportData }, function (data) {
            total = data.totalRecord;
            self.listImportWarehouse(data.importWarehouseModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail phiếu nhập kho
    self.GetImportWarehouseDetail = function (data) {
        $.post("/Warehouse/GetImportWarehouseDetail", { importWarehouseId: data.Id }, function (result) {
            self.mapImportModel(result.importWarehouseModal);
        });
    }

    //======== Wallet
    // Lấy toàn bộ danh đổ lên Form Search lên danh sách bao hàng
    self.GetWalletSearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listUser([]);

        $.post("/Wallet/GetWalletSearchData", {}, function (data) {
            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);

        });
    }

    // Lấy danh sách bao hàng
    self.GetAllWalletList = function () {
        self.listWallet([]);
        var SearchWalletData = ko.mapping.toJS(self.SearchWalletModal());

        $.post("/Wallet/GetAllWalletList", { page: page, pageSize: pagesize, searchModal: SearchWalletData }, function (data) {
            total = data.totalRecord;
            self.listWallet(data.walletModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail bao hàng
    self.GetWalletDetail = function (data) {
        $.post("/Wallet/GetWalletDetail", { walletId: data.Id }, function (result) {
            self.mapWalletModel(result.walletModal);
        });
    }

    //======== ExportWarehouse
    // Lấy toàn bộ danh đổ lên Form Search phiếu xuất kho
    self.GetExportWarehouseSearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listUser([]);

        $.post("/ExportWarehouse/GetExportWarehouseSearchData", {}, function (data) {
            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);

        });
    }
    // Lấy danh sách phiếu xuất kho
    self.GetAllExportWarehouseList = function () {
        self.listExportWarehouse([]);
        var SearchExportData = ko.mapping.toJS(self.SearchImportModal());

        $.post("/ExportWarehouse/GetAllExportWarehouseList", { page: page, pageSize: pagesize, searchModal: SearchExportData }, function (data) {
            total = data.totalRecord;
            self.listExportWarehouse(data.exportWarehouseModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail phiếu xuất kho
    self.GetExportWarehouseDetail = function (data) {
        $.post("/ExportWarehouse/GetExportWarehouseDetail", { exportWarehouseId: data.Id }, function (result) {
            self.mapExportModel(result.exportWarehouseModal);
        });
    }

    //======== PackingList
    // Lấy toàn bộ danh đổ lên Form Search phiếu packingList
    self.GetPackingListSearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listUser([]);

        $.post("/PackingList/GetPackingListSearchData", {}, function (data) {
            console.log(data);

            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);

        });
    }
    // Lấy danh sách packinglist
    self.GetAllPackingList = function () {
        self.listPacking([]);
        var SearchPackingData = ko.mapping.toJS(self.SearchPackingModal());

        $.post("/PackingList/GetAllPackingList", { page: page, pageSize: pagesize, searchModal: SearchPackingData }, function (data) {
            total = data.totalRecord;
            self.listPacking(data.packingListModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail packinglist
    self.GetPackingListDetail = function (data) {
        $.post("/PackingList/GetPackingListDetail", { packingListId: data.Id }, function (result) {
            self.mapPackingListModel(result.packingModal);
        });
    }

    //======== Delivery
    // Lấy toàn bộ danh đổ lên Form Search danh sách phiếu giao hàng
    self.GetDeliverySearchData = function () {
        self.listWarehouse([]);
        self.listStatus([]);
        self.listUser([]);
        self.listCustomer([]);

        $.post("/Delivery/GetDeliverySearchData", {}, function (data) {
            self.listWarehouse(data.listWarehouse);
            self.listStatus(data.listStatus);
            self.listUser(data.listUser);
            self.listCustomer(data.listCustomer);

        });
    }
    // Lấy danh sách phiếu nhập kho
    self.GetAllDeliveryList = function () {
        self.listDelivery([]);
        var SearchDeliveryData = ko.mapping.toJS(self.SearchDeliveryModal());

        $.post("/Delivery/GetAllDeliveryList", { page: page, pageSize: pagesize, searchModal: SearchDeliveryData }, function (data) {
            total = data.totalRecord;
            self.listDelivery(data.deliveryModal);
            self.paging();

            self.isRending(true);
            self.isLoading(false);
        });
    }

    // Lấy Detail phiếu nhập kho
    self.GetDeliveryDetail = function (data) {
        $.post("/Delivery/GetDeliveryDetail", { deliveryId: data.Id }, function (result) {
            self.mapDeliveryModel(result.deliveryModal);
        });
    }


    //==================== Object Map dữ liệu trả về View =========================================
    // Object Detail phiếu nhập kho
    self.mapImportModel = function (data) {
        self.importModel(new importWarehouseModel());

        self.importModel().Id(data.Id);
        self.importModel().Code(data.Code);
        self.importModel().Type(data.Type);
        self.importModel().Status(data.Status);
        self.importModel().PackageNumber(data.PackageNumber);
        self.importModel().WalletNumber(data.WalletNumber);
        self.importModel().WarehouseId(data.WarehouseId);
        self.importModel().WarehouseName(data.WarehouseName);
        self.importModel().WarehouseAddress(data.WarehouseAddress);
        self.importModel().ShipperName(data.ShipperName);
        self.importModel().ShipperPhone(data.ShipperPhone);
        self.importModel().ShipperAddress(data.ShipperAddress);
        self.importModel().ShipperEmail(data.ShipperEmail);
        self.importModel().UserId(data.UserId);
        self.importModel().UserCode(data.UserCode);
        self.importModel().UserFullName(data.UserFullName);
        self.importModel().WarehouseManagerId(data.WarehouseManagerId);
        self.importModel().WarehouseManagerCode(data.WarehouseManagerCode);
        self.importModel().WarehouseManagerFullName(data.WarehouseManagerFullName);
        self.importModel().WarehouseAccountantId(data.WarehouseAccountantId);
        self.importModel().WarehouseAccountantCode(data.WarehouseAccountantCode);
        self.importModel().WarehouseAccountantFullName(data.WarehouseAccountantFullName);
        self.importModel().Created(data.Created);
        self.importModel().LastUpdated(data.LastUpdated);
    }

    // Object Detail bao hàng
    self.mapWalletModel = function (data) {
        self.walletModel(new walletModel());

        self.walletModel().Id(data.Id);
        self.walletModel().Code(data.Code);
        self.walletModel().TransportMethod(data.TransportMethod);
        self.walletModel().Status(data.Status);
        self.walletModel().TotalValue(data.TotalValue);
        self.walletModel().TotalWeight(data.TotalWeight);
        self.walletModel().TotalActualWeight(data.TotalActualWeight);
        self.walletModel().TotalConversionWeight(data.TotalConversionWeight);
        self.walletModel().TotalVolume(data.TotalVolume);
        self.walletModel().TotalPackage(data.TotalPackage);
        self.walletModel().CreatedWarehouseId(data.CreatedWarehouseId);
        self.walletModel().CreatedWarehouseCode(data.CreatedWarehouseCode);
        self.walletModel().CreatedWarehouseName(data.CreatedWarehouseName);
        self.walletModel().CurrentWarehouseId(data.CurrentWarehouseId);
        self.walletModel().CurrentWarehouseCode(data.CurrentWarehouseCode);
        self.walletModel().CurrentWarehouseName(data.CurrentWarehouseName);
        self.walletModel().UserId(data.UserId);
        self.walletModel().UserCode(data.UserCode);
        self.walletModel().UserName(data.UserName);
        self.walletModel().CreatedWarehouseManagerId(data.CreatedWarehouseManagerId);
        self.walletModel().CreatedWarehouseManagerCode(data.CreatedWarehouseManagerCode);
        self.walletModel().CreatedWarehouseManagerName(data.CreatedWarehouseManagerName);
        self.walletModel().CreatedWarehouseAccountantId(data.CreatedWarehouseAccountantId);
        self.walletModel().CreatedWarehouseAccountantCode(data.CreatedWarehouseAccountantCode);
        self.walletModel().CreatedWarehouseAccountantName(data.CreatedWarehouseAccountantName);
        self.walletModel().CurrentZone(data.CurrentZone);
        self.walletModel().Created(data.Created);
        self.walletModel().LastUpdate(data.LastUpdate);
    }

    // Object Detail phiếu xuất kho
    self.mapExportModel = function (data) {
        self.exportModel(new exportWarehouseModel());

        self.exportModel().Id(data.Id);
        self.exportModel().Code(data.Code);
        self.exportModel().Status(data.Status);
        self.exportModel().ReceiverName(data.ReceiverName);
        self.exportModel().ReveiverPhone(data.ReveiverPhone);
        self.exportModel().ReveiverEmail(data.ReveiverEmail);
        self.exportModel().ReveiverAddress(data.ReveiverAddress);
        self.exportModel().PackageNumber(data.PackageNumber);
        self.exportModel().WalletNumber(data.WalletNumber);
        self.exportModel().UserId(data.UserId);
        self.exportModel().UserCode(data.UserCode);
        self.exportModel().UserFullName(data.UserFullName);
        self.exportModel().WarehouseId(data.WarehouseId);
        self.exportModel().WarehouseName(data.WarehouseName);
        self.exportModel().WarehouseAddress(data.WarehouseAddress);
        self.exportModel().WarehouseManagerId(data.WarehouseManagerId);
        self.exportModel().WarehouseManagerCode(data.WarehouseManagerCode);
        self.exportModel().WarehouseManagerFullName(data.WarehouseManagerFullName);
        self.exportModel().WarehouseAccountantId(data.WarehouseAccountantId);
        self.exportModel().WarehouseAccountantCode(data.WarehouseAccountantCode);
        self.exportModel().WarehouseAccountantFullName(data.WarehouseAccountantFullName);
        self.exportModel().Created(data.Created);
        self.exportModel().LastUpdated(data.LastUpdated);
    }

    // Object PakingList - Detail phiếu PackingList
    self.mapPackingListModel = function (data) {
        self.packingModel(new packingListModel());

        self.packingModel().Id(data.Id);
        self.packingModel().Code(data.Code);
        self.packingModel().Status(data.Status);
        self.packingModel().PackingListName(data.PackingListName);
        self.packingModel().TransportType(data.TransportType);
        self.packingModel().PackageNumber(data.PackageNumber);
        self.packingModel().WalletNumber(data.WalletNumber);
        self.packingModel().ExportWarehouseId(data.ExportWarehouseId);
        self.packingModel().ExportWarehouseCode(data.ExportWarehouseCode);
        self.packingModel().ExportWarehouseName(data.ExportWarehouseName);
        self.packingModel().ExportWarehouseAddress(data.ExportWarehouseAddress);
        self.packingModel().TimeStart(data.TimeStart);
        self.packingModel().TimeEnd(data.TimeEnd);
        self.packingModel().WarehouseSourceId(data.WarehouseSourceId);
        self.packingModel().WarehouseSourceCode(data.WarehouseSourceCode);
        self.packingModel().WarehouseSourceName(data.WarehouseSourceName);
        self.packingModel().WarehouseSourceAddress(data.WarehouseSourceAddress);
        self.packingModel().WarehouseDesId(data.WarehouseDesId);
        self.packingModel().WarehouseDesCode(data.WarehouseDesCode);
        self.packingModel().WarehouseDesName(data.WarehouseDesName);
        self.packingModel().WarehouseDesAddress(data.WarehouseDesAddress);
        self.packingModel().UserId(data.UserId);
        self.packingModel().UserCode(data.UserCode);
        self.packingModel().UserFullName(data.UserFullName);
        self.packingModel().WarehouseManagerId(data.WarehouseManagerId);
        self.packingModel().WarehouseManagerCode(data.WarehouseManagerCode);
        self.packingModel().WarehouseManagerFullName(data.WarehouseManagerFullName);
        self.packingModel().WarehouseAccountantId(data.WarehouseAccountantId);
        self.packingModel().WarehouseAccountantCode(data.WarehouseAccountantCode);
        self.packingModel().WarehouseAccountantFullName(data.WarehouseAccountantFullName);
        self.packingModel().Created(data.Created);
        self.packingModel().LastUpdate(data.LastUpdate);
        self.packingModel().ShipperName(data.ShipperName);
        self.packingModel().ShipperPhone(data.ShipperPhone);
        self.packingModel().ShipperEmail(data.ShipperEmail);
        self.packingModel().ShipperAddress(data.ShipperAddress);
        self.packingModel().ShipperLicensePlate(data.ShipperLicensePlate);
        self.packingModel().Note(data.Note);
    }

    // Object Delivery - Detail phiếu xuất giao hàng cho khách
    self.mapDeliveryModel = function (data) {
        self.deliveryModel(new deliveryDetailModel());
        self.deliveryModel().Id(data.Id);
        self.deliveryModel().Code(data.Code);
        self.deliveryModel().Status(data.Status);
        self.deliveryModel().DeliveryName(data.DeliveryName);
        self.deliveryModel().PackageNumber(data.PackageNumber);
        self.deliveryModel().UserId(data.UserId);
        self.deliveryModel().UserCode(data.UserCode);
        self.deliveryModel().UserFullName(data.UserFullName);
        self.deliveryModel().CustomerId(data.CustomerId);
        self.deliveryModel().CustomerCode(data.CustomerCode);
        self.deliveryModel().CustomerName(data.CustomerName);
        self.deliveryModel().CustomerPhone(data.CustomerPhone);
        self.deliveryModel().CustomerEmail(data.CustomerEmail);
        self.deliveryModel().CustomerReceivable(data.CustomerReceivable);
        self.deliveryModel().CustomerAddress(data.CustomerAddress);
        self.deliveryModel().WarehouseId(data.WarehouseId);
        self.deliveryModel().WarehouseName(data.WarehouseName);
        self.deliveryModel().WarehouseAddress(data.WarehouseAddress);
        self.deliveryModel().WarehouseAccountantId(data.WarehouseAccountantId);
        self.deliveryModel().WarehouseAccountantCode(data.WarehouseAccountantCode);
        self.deliveryModel().WarehouseAccountantFullName(data.WarehouseAccountantFullName);
        self.deliveryModel().ShipperName(data.ShipperName);
        self.deliveryModel().ShipperPhone(data.ShipperPhone);
        self.deliveryModel().ShipperEmail(data.ShipperEmail);
        self.deliveryModel().ShipperAddress(data.ShipperAddress);
        self.deliveryModel().ShipperLicensePlate(data.ShipperLicensePlate);
        self.deliveryModel().ShipperVehicle(data.ShipperVehicle);
        self.deliveryModel().TransportCost(data.TransportCost);
        self.deliveryModel().Note(data.Note);
        self.deliveryModel().IsDelete(data.IsDelete);
        self.deliveryModel().Created(data.Created);
        self.deliveryModel().LastUpdate(data.LastUpdate);
    }

};

ko.applyBindings(new warehouseViewModel());