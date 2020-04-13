var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var SourciingViewModel = function () {
    var self = this;
    self.active = ko.observable('');
    //Todo Khai báo ListData đổ dữ liệu danh sách order
    self.listAllSourcing = ko.observableArray([]);
    self.isRendingPage = ko.observable(false);
    self.listAll = ko.observableArray([]);
    self.listOrderService = ko.observableArray([]);
    self.listProductDetail = ko.observableArray([]);
    self.listOrderPackage = ko.observableArray([]);
    self.listOrderComment = ko.observableArray([]);
    self.listSourceSupplier = ko.observableArray([]);
    self.isRending = ko.observable(false);

    self.sourcingDetail = ko.observable(new sourcingDetailModel());
    self.sourcingProduct = ko.observable(new sourcingProductModel());

    self.templateId = ko.observable("sourcing");

    //==================== Search Object -
    self.Keyword = ko.observable("");
    self.Status = ko.observable(-1);
    self.StartDate = ko.observable("");
    self.FinishDate = ko.observable("");
    self.StartSearch = ko.observable("");
    self.FinishSearch = ko.observable("");
    self.AllTime = ko.observable(-1);
    //Todo khai bao bien model show du lieu tren view cua bang status
    self.sourceStatusItemModel = ko.observable(new sourceStatusItemModel());
    //Todo Object chi tiết status
    self.mapSourceStatusItemModel = function (data) {
        self.sourceStatusItemModel(new sourceStatusItemModel());
        self.sourceStatusItemModel().WaitProccess(data.WaitProccess);
        self.sourceStatusItemModel().Proccess(data.Proccess);
        self.sourceStatusItemModel().WaitingChoice(data.WaitingChoice);
        self.sourceStatusItemModel().Finish(data.Finish);
        self.sourceStatusItemModel().Cancel(data.Cancel);
    };
    //Todo Lấy danh sách notification
    self.GetAllSourcing = function () {
        self.listAllSourcing([]);
        self.isRendingPage(false);
        $.post("/" + window.culture + "/CMS/Order/GetAllResourceList",
        {
            PageIndex: page,
            PageSize: pagesize,
            Keyword: self.Keyword(),
            Status: self.Status(),
            StartDateS: self.StartDate(),
            FinishDateS: self.FinishDate(),
            AllTime: self.AllTime()
        }, function (data) {
            total = data.Page.Total;
            self.listAllSourcing(data.ListItems);
            self.mapSourceStatusItemModel(data.StatusItem);
            self.paging();
            self.isRending(true);
            self.isRendingPage(true);
        });
    };
    self.ExcelReport = function () {
        $.redirect("/" + window.culture + "/CMS/Order/ExportSource",
            {
                PageIndex: 1,
                PageSize: 1000000000,
                Keyword: self.Keyword(),
                Keyword: self.Keyword(),
                Status: self.Status(),
                StartDateS: self.StartDate(),
                FinishDateS: self.FinishDate(),
                AllTime: self.AllTime()
            },
            "POST");
    }
    self.clickSearch = function () {
        page = 1;
        self.GetAllSourcing();
    };
    //Todo /tìm kiếm
    self.search = function (page) {
        self.GetAllSourcing();
    };

    //Todo ==================== Phân trang ==================================================
    self.listPage = ko.observableArray([]);
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");

    //Todo Hàm khởi tạo phân trang
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
        self.pageTitle("Hiển thị <b>" + (((page - 1) * pagesize) + 1) +
            "</b> đến <b>" + (((page) * pagesize) > total ? total : ((page) * pagesize)) +
            "</b> của <b>" + total + "</b> Bản ghi");
    }

    //Todo Sự kiện click vào nút phân trang
    self.setPage = function (data) {
        if (page !== data.Page) {
            page = data.Page;
            self.search(page);
        }
    }
    // Search status
    self.searchStatus = function (tmpStatus) {
        page = 1;
        self.Status(tmpStatus);
        self.search(1);
    }
    self.returnAtt = function (start, start1) {
        if (start == start1) {
            return "outOfStock";
        }
        else {
            return "";
        }
    }
    //Search time
    self.searchAllTime = function () {
        page = 1;
        self.AllTime(-1);
        self.Status(-1);
        self.search(1);
    }
    self.SetDate = function () {
        self.StartSearch($('#date_timepicker_start').val());
        self.FinishSearch($('#date_timepicker_end').val());
        var startDate = $('#date_timepicker_start').val();
        if (startDate.length > 0) {
            var arrStart = startDate.split('/');
            if (arrStart.length == 3) {
                startDate = arrStart[1] + '/' + arrStart[0] + '/' + arrStart[2] + ' 00:00';
                self.StartDate(startDate);
            }
        }
        var finishDate = $('#date_timepicker_end').val();
        if (finishDate.length > 0) {
            var arrFinish = finishDate.split('/');
            if (arrFinish.length == 3) {
                finishDate = arrFinish[1] + '/' + arrFinish[0] + '/' + arrFinish[2] + ' 23:00';
                self.FinishDate(finishDate);
            }
        }
        page = 1;
        self.AllTime(1);
        self.Status(-1);
        self.search(1);
    }
    self.FinishSearch.subscribe(function () {
        self.SetDate();
    });
    self.StartSearch.subscribe(function () {
        self.SetDate();
    });

    // Search status
    self.searchKeyword = function () {
        page = 1;
        self.search(1);
    }
    //Enter
    self.onEnter = function (d, e) {
        if (e.which == 13 || e.keyCode == 13) {
            self.Keyword($('#txtKeyword').val());
            self.search(1);
        }
        return true;
    };
    //==================== TẠO ĐƠN HÀNG TÌM NGUỒN ===================
    //chi tiết đơn hàng
    self.DetailId = ko.observable();
    self.DetailName = ko.observable();
    self.DetailLink = ko.observable();
    self.DetailNote = ko.observable();
    self.DetailProperties = ko.observable();
    self.DetailCategoryId = ko.observable();
    self.DetailCategoryName = ko.observable();
    self.DetailQuantity = ko.observable();
    self.DetailImagePath1 = ko.observable("");
    self.DetailImagePath2 = ko.observable("");
    self.DetailImagePath3 = ko.observable("");
    self.DetailImagePath4 = ko.observable("");

    self.setDetail = function (data) {
        self.DetailId(data.Id);
        self.DetailName(data.Name);
        self.DetailLink(data.Link);
        self.DetailNote(data.Note);
        self.DetailProperties(data.Properties);
        self.DetailCategoryId(data.CategoryId);
        self.DetailCategoryName(data.CategoryName);
        self.DetailQuantity(data.Quantity);
        self.DetailImagePath1(data.ImagePath1);
        self.DetailImagePath2(data.ImagePath2);
        self.DetailImagePath3(data.ImagePath3);
        self.DetailImagePath4(data.ImagePath4);
    }

    self.resetDetail = function () {
        self.DetailId("");
        self.DetailName("");
        self.DetailLink("");
        self.DetailNote("");
        self.DetailProperties("");
        self.DetailCategoryId("");
        self.DetailCategoryName("");
        self.DetailQuantity("");
        self.DetailImagePath1("");
        self.DetailImagePath2("");
        self.DetailImagePath3("");
        self.DetailImagePath4("");
    }

    self.isSubmit = ko.observable(true);
    self.isAdd = ko.observable(true);
    self.isModelAdd = ko.observable(true);
    self.isUpload = ko.observable(true);

    self.listDetailSourcing = ko.observableArray([]);
    self.listTest = ko.observableArray([]);

    // File ảnh
    self.isUpload = ko.observable(true);
    self.DetailImagePath1 = ko.observable("");
    self.DetailImagePath2 = ko.observable("");
    self.DetailImagePath3 = ko.observable("");
    self.DetailImagePath4 = ko.observable("");
    var maxFileLength = 5120000;;
    self.addImage = function () {
        $(".flieuploadImg").fileupload({
            url: "/" + window.culture + "/CMS/Ticket/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Có kích thước quá lớn";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Không đúng định dạng";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("Tệp tin là không được phép");
                    return;
                }

                if (self.DetailImagePath1() !== "" &&
                    self.DetailImagePath2() !== "" &&
                    self.DetailImagePath3() !== "" &&
                    self.DetailImagePath4() !== "") {
                    self.DetailImagePath1(window.location.origin + data.result[0].path);
                } else {
                    if (self.DetailImagePath1() === "") {
                        self.DetailImagePath1(window.location.origin + data.result[0].path);
                    } else {
                        if (self.DetailImagePath2() === "") {
                            self.DetailImagePath2(window.location.origin + data.result[0].path);
                        } else {
                            if (self.DetailImagePath3() === "") {
                                self.DetailImagePath3(window.location.origin + data.result[0].path);
                            } else {
                                if (self.DetailImagePath4() === "") {
                                    self.DetailImagePath4(window.location.origin + data.result[0].path);
                                }
                            }
                        }
                    }
                }

                $("div").removeClass("hover");
            }
        });
        return true;
    }

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf(".")).toLowerCase();
        return !_.some([".jpg", ".jpeg", ".gif", ".png"], function (item) { return item === ext; });
    };
    //Kiểm tra dữ liệu đầu vào
    self.checkDetail = function () {
        if (self.DetailName() === "") {
            toastr.error("Tên sản phẩm không được để trống!");
            return false;
        }
        if (self.DetailLink() === "") {
            toastr.error("Link không được để trống!");
            return false;
        }
        if (self.DetailCategoryName() === "") {
            toastr.error("Ngành hàng không được để trống!");
            return false;
        }
        if (self.DetailQuantity() === "") {
            toastr.error("Số lượng không được để trống!");
            return false;
        }
        //if (self.DetailImagePath1() === "" && self.DetailImagePath2() === "" && self.DetailImagePath3() === "" && self.DetailImagePath4() === "") {
        //    toastr.error("Hình ảnh không được để trống!");
        //    return false;
        //}

        return true;
    }
    self.ViewCreateSource = function () {
        $('.dropdownjstree').remove();
        $("#category_tree_sourcing").dropdownjstree({
            source: window.categoryJsTree,
            selectedNode: self.DetailCategoryId(),
            selectNote: (node, selected) => {
                self.DetailCategoryId(selected.node.id);
                self.DetailCategoryName(selected.node.text);
            }
        });
    }

    // Lưu thông tin chi tiết đơn hàng tìm nguồn
    self.SaveDetail = function () {
        var detail = {
            Id: (new Date()).getTime(),
            Name: self.DetailName(),
            Link: self.DetailLink(),
            Note: self.DetailNote(),
            Properties: self.DetailProperties(),
            CategoryId: self.DetailCategoryId(),
            CategoryName: self.DetailCategoryName(),
            Quantity: self.DetailQuantity(),
            ImagePath1: self.DetailImagePath1(),
            ImagePath2: self.DetailImagePath2(),
            ImagePath3: self.DetailImagePath3(),
            ImagePath4: self.DetailImagePath4()
        };
        if (self.checkDetail()) {
            self.listDetailSourcing.push(detail);
            self.resetDetail();
            toastr.success("Thêm sản phẩm thành công!");
        }
    }

    //Lưu thông tin đơn tìm nguồn
    self.SaveSourcing = function () {
        $.post("/" + window.culture + "/CMS/Order/SaveSourcing", { sourceDetail: self.listDetailSourcing() }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
                self.listDetailSourcing([]);
            } else {
                toastr.success(result.msg);
                self.listDetailSourcing([]);
                window.location.href = "/" + window.culture + "/CMS/Order/Sourcing";
            }
        });
    }

    // Sua thong tin san pham
    self.ViewEditSourceDetail = function (id) {
        for (var i = 0; i < self.listDetailSourcing().length; i++) {
            if (self.listDetailSourcing()[i].Id == id) {
                self.DetailId(self.listDetailSourcing()[i].Id);
                self.DetailName(self.listDetailSourcing()[i].Name);
                self.DetailLink(self.listDetailSourcing()[i].Link);
                self.DetailNote(self.listDetailSourcing()[i].Note);
                self.DetailProperties(self.listDetailSourcing()[i].Properties);
                self.DetailCategoryId(self.listDetailSourcing()[i].CategoryId);
                self.DetailCategoryName(self.listDetailSourcing()[i].CategoryName);
                self.DetailQuantity(self.listDetailSourcing()[i].Quantity);
                self.DetailImagePath1(self.listDetailSourcing()[i].ImagePath1);
                self.DetailImagePath2(self.listDetailSourcing()[i].ImagePath2);
                self.DetailImagePath3(self.listDetailSourcing()[i].ImagePath3);
                self.DetailImagePath4(self.listDetailSourcing()[i].ImagePath4);
                break;
            }
        }
    }

    self.EditSourceDetail = function (id) {
        for (var i = 0; i < self.listDetailSourcing().length; i++) {
            if (self.listDetailSourcing()[i].Id == id) {
                self.listDetailSourcing()[i].Id = self.DetailId();
                self.listDetailSourcing()[i].Name = self.DetailName();
                self.listDetailSourcing()[i].Link = self.DetailLink();
                self.listDetailSourcing()[i].Note = self.DetailNote();
                self.listDetailSourcing()[i].Properties = self.DetailProperties();
                self.listDetailSourcing()[i].CategoryId = self.DetailCategoryId();
                self.listDetailSourcing()[i].CategoryName = self.DetailCategoryName();
                self.listDetailSourcing()[i].Quantity = self.DetailQuantity();
                self.listDetailSourcing()[i].ImagePath1 = self.DetailImagePath1();
                self.listDetailSourcing()[i].ImagePath2 = self.DetailImagePath2();
                self.listDetailSourcing()[i].ImagePath3 = self.DetailImagePath3();
                self.listDetailSourcing()[i].ImagePath4 = self.DetailImagePath4();
                break;
            }
        }
        self.listTest(self.listDetailSourcing());
        self.listDetailSourcing([]);
        self.listDetailSourcing(self.listTest());
        self.resetDetail();
    }

    // Xoa thong tin san pham
    self.DeleteSourceDetail = function (id) {
        self.listTest([]);
        for (var i = 0; i < self.listDetailSourcing().length; i++) {
            if (self.listDetailSourcing()[i].Id != id) {
                self.listTest.push(self.listDetailSourcing()[i]);
            }
        }
        self.listDetailSourcing(self.listTest());
    }

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

    //Todo chi tiết đơn hàng tìm nguồn
    self.detailSourcing = function (data) {
        window.location.href = "/" + window.culture + "/CMS/Order/DetailSourcing?sourcingId=" + data.id;
    }

    self.backToList = function () {
        window.location.href = "/" + window.culture + "/CMS/Order/Sourcing";
    };

    //Map thông tin chi tiết đơn hàng tìm nguồn
    self.mapSourcingDetail = function (data) {
        self.sourcingDetail(new sourcingDetailModel());
        self.sourcingDetail().Id(data.Id);
        self.sourcingDetail().Code(data.Code);
        self.sourcingDetail().AnalyticSupplier(data.AnalyticSupplier);
        self.sourcingDetail().CreateDate(data.CreateDate);
        self.sourcingDetail().UpdateDate(data.UpdateDate);
        self.sourcingDetail().Status(data.Status);
        self.sourcingDetail().ServiceMoney(data.ServiceMoney);
        self.sourcingDetail().TypeService(data.TypeService);
        self.sourcingDetail().ServiceType(data.ServiceType);
        self.sourcingDetail().ShipMoney(data.ShipMoney);
        self.sourcingDetail().SourceSupplierId(data.SourceSupplierId);

        self.sourcingDetail().CustomerName(data.CustomerName);

        self.sourcingDetail().CustomerEmail(data.CustomerEmail);
        self.sourcingDetail().CustomerPhone(data.CustomerPhone);
        self.sourcingDetail().CustomerAddress(data.CustomerAddress);
        self.sourcingDetail().TypeServiceName(data.TypeServiceName);
    }

    //Map thông tin sản phẩm tìm nguồn
    self.mapSourcingProduct = function (data) {
        self.sourcingProduct(new sourcingProductModel());
        self.sourcingProduct().Name(data.Name);
        self.sourcingProduct().Quantity(data.Quantity);
        self.sourcingProduct().Link(data.Link);
        self.sourcingProduct().CategoryName(data.CategoryName);

        self.sourcingProduct().Size(data.Size);
        self.sourcingProduct().Color(data.Color);
        self.sourcingProduct().Note(data.Note);

        self.sourcingProduct().ImagePath1(data.ImagePath1);
        self.sourcingProduct().ImagePath2(data.ImagePath2);
        self.sourcingProduct().ImagePath3(data.ImagePath3);
        self.sourcingProduct().ImagePath4(data.ImagePath4);
    }
    $(function () {
        self.initInputMark();
        self.GetAllSourcing();
        self.resetDetail();
        self.ViewCreateSource();

        $("#effect-7 .img").hover(function () {
            $("#effect-7 .img .overlay").css("width", "100%");
        }
       , function () {
           $("#effect-7 .img .overlay").css("width", "0");
       });
    });
}

ko.applyBindings(new SourciingViewModel());