function PackageNoCodeModel(packageDetailModel, orderDetailModel) {
    var self = this;
    self.isLoading = ko.observable(false);

    // Search
    self.status = ko.observable(null);
    self.orderType = ko.observable(null);
    self.fromDate = ko.observable(null);
    self.toDate = ko.observable(null);
    self.keyword = ko.observable("");
    self.statusPackage = ko.observable(null);

    // list data
    self.packages = ko.observableArray();
    self.packageStatuss = ko.observableArray([]);

    self.tabMode = ko.observable(0);
    self.createdNo = ko.observable(0);
    self.approvelNo = ko.observable(0);
    self.allNo = ko.observable(0);

    // pagging
    self.currentPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.totalPage = ko.observable(0);
    self.totalRecord = ko.observable(0);

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "package", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

    self.changeTabMode = function (tabMode) {
        if (tabMode !== self.tabMode()) {
            self.tabMode(tabMode);
            self.search(1);
        }
    }

    var statusFirst = true;
    self.status.subscribe(function () {
        if (statusFirst) {
            statusFirst = false;
            return;
        }
        self.search(1);
    });

    var orderTypeFirst = true;
    self.orderType.subscribe(function () {
        if (orderTypeFirst) {
            orderTypeFirst = false;
            return;
        }
        self.search(1);
    });
    //Xuan them loc trang thai kien hang
    var statusPackageFirst = true;
    self.statusPackage.subscribe(function () {
        if (statusPackageFirst) {
            statusPackageFirst = false;
            return;
        }
        self.search(1);
    });
    self.changeCheck = function (data) {
        data.checked(!data.checked());
        self.search(1);
    }
    

   self.renderPager = function () {
        self.totalPage(Math.ceil(self.totalRecord() / self.recordPerPage()));

        $("#sumaryPagerPackage").html(self.totalRecord() === 0 ? "No package" : "Show " + ((self.currentPage() - 1) * self.recordPerPage() + 1) + " to " + (self.currentPage() * self.recordPerPage() < self.totalRecord() ?
            self.currentPage() * self.recordPerPage() : self.totalRecord()) + " of " + self.totalRecord() + " package");

        $("#pagerPackage").pager({ pagenumber: self.currentPage(), pagecount: self.totalPage(), totalrecords: self.totalRecord(), buttonClickCallback: self.pageClick });
    };

    self.pageClick = function (currentPage) {
        $("#pagerPackage").pager({ pagenumber: currentPage, pagecount: self.totalPage(), buttonClickCallback: self.search });

        self.search(currentPage);
    };

    self.search = function(currentPage, isFirstRequest) {
        self.currentPage(currentPage);
        if (isFirstRequest === undefined)
            isFirstRequest = false;

        self.isLoading(true);
        var packageStatusTexts = _.map(_.filter(self.packageStatuss(), function (it) { return it.checked() }), "id");
        var packageStatusText = packageStatusTexts.length === 0 ? "" : ';' + _.join(packageStatusTexts, ';') + ';';

        $.get("/packageNoCode/search",
            {
            orderType: self.orderType(),
            mode: window.modePage,
            tabMode: self.tabMode(),
            status: self.status(),
            fromDate: self.fromDate(),
            toDate: self.toDate(),
            keyword: self.keyword(),
            currentPage: self.currentPage(),
            recordPerPage: self.recordPerPage(),
            isFirstRequest: isFirstRequest,
            statusPackage: packageStatusText,
        }, function(data) {
            self.isLoading(false);
            _.each(data.items,
                function (item) {
                    item.statusText = item.packageNoCodeStatus === 0 ? 'Newly created' : 'Closed';
                    item.statusClass = item.packageNoCodeStatus === 0 ? 'label label-warning' : 'label label-success';
                    item.packageNoCodeCreatedTextNow = item.packageNoCodeCreated ? moment(item.packageNoCodeCreated).fromNow() : '';
                    item.packageNoCodeCreatedText = item.packageNoCodeCreated ? moment(item.packageNoCodeCreated).format('DD/MM/YYYY HH:mm') : '';
                    item.packageNoCodeUpdatedTextNow = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).fromNow() : '';
                    item.packageNoCodeUpdatedText = item.packageNoCodeUpdated ? moment(item.packageNoCodeUpdated).format('DD/MM/YYYY HH:mm') : '';
                    item.packageNoCodeCommentNo = ko.observable(item.packageNoCodeCommentNo);
                    item.images = item.packageNoCodeImageJson === null ? [] : JSON.parse(item.packageNoCodeImageJson);

                    item.packageNoCodeNote = ko.observable(item.packageNoCodeNote);
                    item.packageNoCodeNote.subscribe(function (newValue) {
                        self.notePackageNoCode(ko.mapping.toJS(item));
                    });

                    item.note = ko.observable(item.note);
                    item.note.subscribe(function (newValue) {
                        self.notePackage(ko.mapping.toJS(item));
                    });

                });
            //Xuan them
            if (isFirstRequest) {
                _.each(data.packageStatus,
                   function (item) {
                       item.checked = ko.observable(item.checked);
                   });
                self.packageStatuss(data.packageStatus);
            }
           
            self.createdNo(data.createdNo);
            self.approvelNo(data.approvelNo);
            self.allNo(data.allNo);
            self.packages(data.items);
            self.totalRecord(data.totalRecord);
            self.renderPager();
        });
    }



    //Thêm note mất mã trên danh sách
    self.notePackageNoCode = function (data) {
        var isLook = false;
        var input = {
            id: data.packageNoCodeId,
            imageJson: JSON.stringify(data.packageNoCodeImageJson),
            note: data.packageNoCodeNote,
            packageId: data.id,
            tranportCode: data.transportCode
        };
        $.post("/order/updatepackagenocode",
                input,
                function (rs) {
                    self.isLoading(false);
                    if (rs.status < 0) {
                        toastr.error(rs.msg);
                        return;
                    }

                    toastr.success(rs.msg);
                });
        return isLook;
    }

    self.notePackage = function (data) {
        var isLook = false;
        var input = {
            id: data.id,
            note: data.note
        };
        $.post("/order/updatepackage",
                input,
                function (rs) {
                    self.isLoading(false);
                    if (rs.status < 0) {
                        toastr.error(rs.msg);
                        return;
                    }

                    toastr.success(rs.msg);
                });
        return isLook;
    }

    // Bình luận
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "packageNoCode_" + data.packageNoCodeId;
        self.groupCommentBoxModelModal().callback = function () {
            // Cập nhật số lượng comment cho package
            $.post("/PackageNoCode/UpdateCommentNo",
                { id: data.packageNoCodeId },
                function(rs) {
                    if (rs && rs.status < 0) {
                        toastr.warning(rs.text);
                        return;
                    }
                    data.packageNoCodeCommentNo(data.packageNoCodeCommentNo() + 1);
                });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle("P" + data.code);
        self.groupCommentBoxModelModal().pageTitle = "package P" + data.code;

        self.groupCommentBoxModelModal().pageUrl = "/PackageNoCode";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    $('#groupCommentModal').on('hide', function () {
        self.search(self.currentPage());
    });

    self.addPackageLoseForm = ko.observable(null);
    self.showUpdatePackageLose = function (data) {
        if (self.addPackageLoseForm() == null) {
            self.addPackageLoseForm(new AddPackageLoseModel());
            ko.applyBindings(self.addPackageLoseForm(), $("#addPackageLoseModal")[0]);
        }

        self.addPackageLoseForm().setUpdate(data.packageNoCodeId, data.code, data.transportCode, data.packageNoCodeNote(), data.images);
    }

    $('#addPackageLoseModal').on('hide', function () {
        self.search(self.currentPage());
    });


    self.callbackAssignPackage = function () {
        self.search(self.currentPage());
    }
    self.assignPackageForm = ko.observable();
    self.showAddForOrder = function(data) {
        if (self.assignPackageForm() == null) {
            self.assignPackageForm(new AssignPackageModel(self.callbackAssignPackage));
            ko.applyBindings(self.assignPackageForm(), $("#assignPackageModal")[0]);
        }

        self.assignPackageForm().setUpdate(data.packageNoCodeId, data.transportCode, data.code);
    }

    self.approvelAssignPackage = function(data) {
        swal({
            title: 'You certainly confirm this fact for orders?',
                text: '',
                type: 'warning',
                showCancelButton: true,
               cancelButtonText: 'Cancel',
                confirmButtonText: 'accept'
            })
            .then(function() {
                    $.post("/Order/ApprovelAssignPackage",
                        { packageNoCodeId: data.packageNoCodeId },
                        function(rs) {
                            if (rs && rs.status < 0) {
                                toastr.warning(rs.msg);
                                return;
                            }
                            toastr.success(rs.msg);
                            self.search(self.currentPage());
                        });
                },
                function() {});
    }

    self.remove = function (data) {
        swal({
            title: 'Are you sure to delete this lost code package?',
            text: '',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        })
            .then(function () {
                $.post("/PackageNoCode/Remove",
                    { id: data.packageNoCodeId },
                    function (rs) {
                        if (rs && rs.status < 0) {
                            toastr.warning(rs.text);
                            return;
                        }
                        toastr.success(rs.text);
                        self.search(self.currentPage());
                    });
            },
            function () { });
    }

    //self.detailModal = ko.observable(null);
    self.showDetail = function(data) {
        if (packageDetailModel) {
            packageDetailModel.showModel(data);
            return;
        }
    }

    self.showOrderDetail = function(orderId) {
        if (orderDetailModel) {
            orderDetailModel.viewOrderDetail(orderId);
            return;
        }
    }

    $(function() {
        $('#forcastDate-btn').daterangepicker({
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
                self.fromDate(start.format());
                self.toDate(end.format());
                self.search(1);
                $('#forcastDate-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
            });

        ////Xuan them

        $('.dropdown-toggle-custome').click(function () {
            var $this = $(this);
            var drop = $this.parent().find(".dropdown-menu");
            drop.slideToggle(10);

            $(document).click(function (e) {
                if (drop && $(e.target).parents().filter($this).length === 0 && $(e.toElement).filter($this).length === 0
                    && $(e.target).parents().filter('.size18.isCheckbox').length === 0 && $(e.toElement).filter(('.size18.isCheckbox')).length === 0) {
                    drop.slideUp(10);
                }
            });
        });
        
        self.search(1, true);
    });

   
}

// Bind PackageDetail
var packageDetailModelView = new PackageDetail(window.states);
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

// Bind OrderDetail
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

// Bind PackageModel
var modelView = new PackageNoCodeModel(packageDetailModelView, orderDetailViewModel);
ko.applyBindings(modelView, $("#package")[0]);