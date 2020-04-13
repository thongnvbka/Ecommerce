var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

ko.bindingHandlers.executeOnEnter = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var callback = valueAccessor();
        $(element).keypress(function (event) {
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode === 13) {
                callback.call(viewModel);
                return false;
            }
            return true;
        });
    }
};

function OrderViewModel(chatViewModel, orderDetailViewModel, stockQuotesAddOrEditViewModel, depositAddOrEditViewModel, depositDetailViewModel, stockQuotesViewModel, ticketDetail, orderAddViewModel, accountantDetail, orderCommerce, orderCommerceDetailViewModel, packageDetailModelView) {
    var self = this;

    //============================================================= khai báo biến system ==============================================
    //biến system
    self.active = ko.observable('isEmp');
    self.templateId = ko.observable('isEmp');
    self.token = ko.observable();

    self.totalOrderNew = ko.observable();
    self.totalOrderWait = ko.observable();
    self.totalOrder = ko.observable();
    self.totalOrderLate = ko.observable();

    self.totalOrderDepositNew = ko.observable();
    self.totalOrderDeposit = ko.observable();
    self.totalOrderDepositLate = ko.observable();

    self.totalStockQuoesNew = ko.observable();
    self.totalStockQuoes = ko.observable();
    self.totalOrderSourcing = ko.observable();

    self.totalOrderRisk = ko.observable();
    self.totalOrderAccountant = ko.observable();
    self.totalOrderNoWarehouse = ko.observable();

    self.listStatus = ko.observableArray([]);
    self.listSystem = ko.observableArray([]);
    self.listSystemRender = ko.observableArray([]);
    self.listOrder = ko.observableArray([]);
    self.listUser = ko.observableArray([]);
    self.listUserDetail = ko.observableArray([]);
    self.listUserOffice = ko.observableArray([]);
    self.exchangeRate = ko.observable();
    self.listHistory = ko.observableArray([]);
    self.listOrderExchage = ko.observableArray([]);
    self.mess = ko.observable();

    self.totalTicketSupport = ko.observable();
    self.totalClaimForRefund = ko.observable();
    self.totalClaim = ko.observable(0);

    //kho hàng
    self.listWarehouse = ko.observableArray([]);
    self.listWarehouseVN = ko.observableArray([]);
    self.warehouseVNId = ko.observable();

    // Khai báo biến thông tin Orders trong xử lý Refund
    self.listOrderService = ko.observableArray([]);

    //Khai báo biến thông tin Cấp Level Vip
    self.vipOrder = ko.observable(0);
    self.vipShip = ko.observable(0);
    self.vipName = ko.observable("");

    // Chat
    self.groupCommentBoxModelModal = ko.observable(new GroupChatHubModalViewModel(null, "Orders", { isShowNotify: true, listUserTag: "/user/searchusertag" }));

    // Bình luận package
    self.comment = function (data) {
        self.groupCommentBoxModelModal().groupId = "order_" + data.OrderCode;
        self.groupCommentBoxModelModal().callback = function () {
            //PlanRepo.UpdateCommentPending(data.Id, function () { });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle(ReturnCode(data.OrderCode));
        self.groupCommentBoxModelModal().pageTitle = Order + ReturnCode(data.OrderCode);

        // todo: Henry bổ xung xin khi click vào thông báo
        //self.groupCommentBoxModelModal().pageUrl = "/";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }

    //bình luận Orders
    self.commentOrder = function (data) {
        self.groupCommentBoxModelModal().groupId = "order_" + data.Code;
        self.groupCommentBoxModelModal().callback = function () {
            //PlanRepo.UpdateCommentPending(data.Id, function () { });
        }
        self.groupCommentBoxModelModal().joinGroup();
        self.groupCommentBoxModelModal().objectTitle(ReturnCode(data.Code));
        self.groupCommentBoxModelModal().pageTitle = "Orders " + ReturnCode(data.Code);

        // todo: Henry bổ xung xin khi click vào thông báo
        //self.groupCommentBoxModelModal().pageUrl = "/";

        self.groupCommentBoxModelModal().isLoadingComment(false);
        $("#groupCommentModal").modal("show");
    }


    // Hàm lấy thông tin kho khi đã chọn
    self.warehouseVNId.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouseVN(), function (item) { return item.Id === newId; });
        if (warehouse !== undefined) {

            if (self.warehouseVNId() === self.order().WarehouseDeliveryId()) {
                return;
            }
            $('#update').modal();
            self.order().WarehouseDeliveryId(warehouse.Id);
            self.order().WarehouseDeliveryName(warehouse.Name);

            $.post("/Order/UpdateOrderWarehouseVn",
                {
                    id: self.order().Id(),
                    warehouseDeliveryId: self.order().WarehouseDeliveryId(),
                    warehouseDeliveryName: self.order().WarehouseDeliveryName()
                },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        self.orderCodeWarehouse(self.warehouseVNId(), self.order().Code());
                    }
                    $('#update').modal('hide');
                });
        }
    });

    self.warehouseId = ko.observable();
    self.warehouse = ko.observable();

    //lấy thông tin khách hàng
    self.customer = ko.observable(null);
    self.customerName = ko.observable('Select customer');

    //các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);

    //các biến cho submit form
    self.isSubmit = ko.observable(true);

    self.isShowHistory = ko.observable(false);
    self.checkShowHistory = function () {
        self.isShowHistory(!self.isShowHistory());
    }

    self.isShowLog = ko.observable(false);
    self.checkShowLog = function () {
        self.isShowLog(!self.isShowLog());
    }

    //============================================================= Các sự kiện click menu =====================================
    self.clickMenu = function (name) {
        self.keyword('');
        self.status(-1);
        self.systemId(-1);
        self.dateStart(undefined);
        self.dateEnd(undefined);
        self.userId(null);
        self.customerId(null);
        self.checkExactCode(false);

        window.history.pushState('Purchase', '', 'Purchase/Order#' + name);

        page = 1;
        //if (name !== self.active()) {
        self.listOrder([]);

        total = 0;
        page = 1;
        pageTotal = 0;
        self.status(-1);
        self.systemId(-1);

        self.isLoading(false);
        self.active(name);
        self.templateId(name);
        self.dateStart(undefined);
        self.dateEnd(undefined);
        // tra cứu thông tin khách hàng
        if (name === 'customerfind') {
            self.searchCustomer();
        }

        if (name === 'reportOrder') {
            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
            self.viewReportProfitOrder();
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });
        }

        //gọi page báo cáo
        if (name === 'report') {
            self.dateStart('');
            self.dateEnd('');
            self.viewReportBargain();
        }

        if (name === 'reportDeposit') {
            self.dateStart('');
            self.dateEnd('');
            self.viewReportDeposit();
        }
        if (name === 'reportBargain') {
            self.dateStart('');
            self.dateEnd('');
            self.viewReport();

        }
        else {
            self.renderSystem();
            if (self.dateStart() !== undefined) {
                $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
            }
            self.search(1);
            self.isLoading(true);
        }
        self.isLoading(true);

        $('#daterange-btn').daterangepicker({
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
                self.dateStart(start.format());
                self.dateEnd(end.format());
                $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
            });
        $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btn span').html('Create date');
            self.dateStart('');
            self.dateEnd('');
        });

        //hỗ trợ khiếu nại
        if (name === 'ticket-support') {
            //self.isRending(false);
            //self.isLoading(false);
            self.getRenderSystemTab();
            self.GetAllTicketListByStaff();
            //self.ticketSupportViewModel(self.listComplainStatus, self.listComplainSystem, self.listSystemRenderPoCustomer, self.listAllCustomerComplain);
            //self.isRending(true);
            //self.isLoading(true);
        };

        if (name === 'claimforrefund') {
            //self.SearchClaimForRefundModal().CustomerId = -1;
            self.searchCustomerClaimForRefund();
            //self.searchCustomer();
            self.renderSystem();
            self.GetClaimForRefundList();
        };
        //}
    }

    self.AddWebsite = function (item) {
        var website = "";
        var isOk = true;

        if (item.Taobao() === true) {
            website += "taobao.com ";
        }
        if (item.W1688() === true) {
            website += "1688.com ";
        }
        if (item.Tmall() === true) {
            website += "tmall.com ";
        }

        $.post("/Order/UpdateUserWebsite", { userId: item.Id, website: website }, function (data) {
            if (data.status === msgType.success) {
                item.Websites = website;
            } else {
                isOk = false;
                toastr.error(data.msg);
            }
        });
        return isOk;
    }

    self.clickTab = function (tab) {
        self.SearchCustomerModal().SystemId(tab);
        self.systemId(tab);
        self.search(1);
        $(".select-view").select2();
    }


    //============================================================= Tìm kiếm ===================================================
    //các biến tìm kiếm
    self.keyword = ko.observable("");
    self.status = ko.observable();
    self.systemId = ko.observable();
    self.dateStart = ko.observable();
    self.dateEnd = ko.observable();
    self.userId = ko.observable();
    self.customerId = ko.observable();
    self.checkExactCode = ko.observable(false);

    self.isAllDelay = ko.observable(false);
    self.isAllNoCodeOfLading = ko.observable(false);
    self.isAllNotEnoughInventory = ko.observable(false);

    self.totalAwait = ko.observable(0); //Sum số Orders chưa nhận
    self.cogAwait = ko.observable(5); //Số Orders được nhận
    //Hàm tìm kiếm 
    self.search = function (page) {
        window.page = page;

        self.totalAwait(0);
        self.cogAwait(5);

        self.isSubmit(true);
        self.isRending(false);
        if (self.active() === 'order-new') {
            $.post("/Order/GetOrderNew", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.Status = ko.observable(item.Status);
                    item.LinkNo = ko.observable(item.LinkNo);
                    item.ProductNo = ko.observable(item.ProductNo);
                    item.TotalPrice = ko.observable(item.TotalPrice);
                    item.TotalExchange = ko.observable(item.TotalExchange);

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);

                self.totalAwait(data.totalAwait);
                if (data.totalAwaitUser < 5) {
                    self.cogAwait(data.totalAwaitUser);
                }

                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order') {
            $.post("/Order/GetOrder", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.Status = ko.observable(item.Status);
                    item.LinkNo = ko.observable(item.LinkNo);
                    item.ProductNo = ko.observable(item.ProductNo);
                    item.TotalPrice = ko.observable(item.TotalPrice);
                    item.TotalExchange = ko.observable(item.TotalExchange);

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);

                self.listStatusView(data.listStatus);

                self.totalAwait(data.totalAwait);
                if (data.totalAwaitUser < 5) {
                    self.cogAwait(data.totalAwaitUser);
                }

                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-delay') {
            $.post("/Order/GetOrderDelay", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd(), isAllDelay: self.isAllDelay() }, function (data) {
                total = data.totalRecord;
                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.Status = ko.observable(item.Status);
                    item.LinkNo = ko.observable(item.LinkNo);
                    item.ProductNo = ko.observable(item.ProductNo);
                    item.TotalPrice = ko.observable(item.TotalPrice);
                    item.TotalExchange = ko.observable(item.TotalExchange);

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-retail') {
            $.post("/Order/GetOrderRetail", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.Status = ko.observable(item.Status);
                    item.LinkNo = ko.observable(item.LinkNo);
                    item.ProductNo = ko.observable(item.ProductNo);
                    item.TotalPrice = ko.observable(item.TotalPrice);
                    item.TotalExchange = ko.observable(item.TotalExchange);

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);

                self.listStatusView(data.listStatus);

                self.totalAwait(data.totalAwait);
                if (data.totalAwaitUser < 5) {
                    self.cogAwait(data.totalAwaitUser);
                }

                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-deposit-new') {
            $.post("/Deposit/GetOrderDepositNew", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.exchangeRate(data.exchangeRate);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-deposit') {
            $.post("/Deposit/GetOrderDeposit", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.exchangeRate(data.exchangeRate);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-deposit-delay') {
            $.post("/Deposit/GetOrderDepositDelay", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.exchangeRate(data.exchangeRate);
                self.paging();
                self.isRending(true);
            });
        }
        // Orders chưa có mã vận đơn
        if (self.active() === 'order-risk') {
            $.post("/OrderRisk/GetOrderRisk", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd(), isAllNoCodeOfLading: self.isAllNoCodeOfLading() }, function (data) {
                total = data.totalRecord;

                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);
                self.paging();
                self.isRending(true);
            });
        }

        // Orders chờ kế toán thanh toán
        if (self.active() === 'order-accountant') {
            $.post("/OrderRisk/GetOrderAccountant", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);
                self.paging();
                self.isRending(true);
            });
        }

        //Orders chưa đủ kiện về kho
        if (self.active() === 'order-warehouse') {
            $.post("/OrderRisk/GetOrderNoWarehouse", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd(), isAllNotEnoughInventory: self.isAllNotEnoughInventory() }, function (data) {
                total = data.totalRecord;

                var list = [];
                _.each(data.listOrder, function (item) {
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.CacheUserNote = ko.observable(item.UserNote);
                    item.UserNote = ko.observable(item.UserNote);
                    item.UserNote.subscribe(function (newValue) {
                        if (item.CacheUserNote !== newValue) {

                            $('#update').modal();
                            $.post("/Order/UpdateOrderUserNote",
                                {
                                    id: item.Id,
                                    note: newValue,
                                    __RequestVerifiCationToken: self.token()
                                },
                                function (result) {
                                    if (result.status === msgType.error) {
                                        toastr.error(result.msg);
                                    }
                                    $('#update').modal('hide');
                                });

                            item.CacheUserNote = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);

                self.paging();
                self.isRending(true);
            });
        }

        // hoàn thành Orders
        if (self.active() === 'order-success') {
            $.post("/Purchase/GetOrderSuccess", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
        }

        //phiếu yêu cầu tìm nguồn chưa nhân xử lý
        if (self.active() === 'stock-quotes-new') {
            $.post("/Source/StockQuotesNew", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
        }

        //phiếu yêu cầu tìm nguồn đang xử lý
        if (self.active() === 'stock-quotes') {
            $.post("/Source/StockQuotes", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
        }

        //đơn tìm nguồn
        if (self.active() === 'order-sourcing') {
            $.post("/Source/OrderSourcing", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'order-commerce') {
            $.post("/OrderCommerce/GetOrderCommerce", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
            //self.isRending(true);
        }

        if (self.active() === 'lading-code') {
            $.post("/Order/GetOrderLadingCode", { page: page, pageSize: pagesize, keyword: self.keyword(), checkExactCode: self.checkExactCode(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId(), customerId: self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
                total = data.totalRecord;
                self.listOrder(data.listOrder);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'user-website') {
            $.post("/Order/GetUser", { page: page, pageSize: pagesize, userId: self.userId() }, function (data) {
                total = data.totalRecord;

                var list = [];

                _.each(data.listOrder, function (item) {

                    item.CacheTaobao = false;
                    item.CacheW1688 = false;
                    item.CacheTmall = false;

                    item.Taobao = ko.observable(false);
                    item.W1688 = ko.observable(false);
                    item.Tmall = ko.observable(false);

                    var str = item.Websites + "";

                    if (str.search("taobao.com") !== -1) {
                        item.Taobao(true);
                        item.CacheTaobao = true;
                    }

                    if (str.search("1688.com") !== -1) {
                        item.W1688(true);
                        item.CacheW1688 = true;
                    }

                    if (str.search("tmall.com") !== -1) {
                        item.Tmall(true);
                        item.CacheTmall = true;
                    }

                    item.Taobao.subscribe(function (newValue) {
                        if (!self.AddWebsite(item)) {
                            item.Taobao(item.CacheTaobao);
                        }
                        else {
                            item.CacheTaobao = newValue;
                        }
                    });

                    item.W1688.subscribe(function (newValue) {
                        if (!self.AddWebsite(item)) {
                            item.W1688(item.CacheW1688);
                        }
                        else {
                            item.CacheW1688 = newValue;
                        }
                    });

                    item.Tmall.subscribe(function (newValue) {
                        if (!self.AddWebsite(item)) {
                            item.Tmall(item.CacheTmall);
                        }
                        else {
                            item.CacheTmall = newValue;
                        }
                    });

                    list.push(item);
                });

                self.listOrder(list);
                self.paging();
                self.isRending(true);
            });
        }

        if (self.active() === 'ticket-support') {
            self.GetAllTicketListByStaff();
        }
        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        }

        if (self.active2() === 'OrderMoney') {
            self.OrderMoney();
        }

        if (self.active2() === 'OrderHistory') {
            self.OrderHistory();
        }
    };

    self.orderFinish = function (data) {
        swal({
            title: 'You definitely want to complete the order?',
            text: 'orders "#' + ReturnCode(data.Code) + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Comfirm'
        }).then(function () {
            $.post("/Purchase/OrderFinish", { id: data.Id }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    self.search(page);
                }
                else {
                    toastr.error(result.msg);
                }
            });
        }, function () { });
    }

    //Hàm chọn tab để tìm kiếm
    self.clickSearch = function (data, event) {
        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        };

        if (self.active() === 'user-website') {
            self.search(1);
        }
        if (self.active() === 'ticket-support') {
            self.systemId(self.SearchCustomerModal().SystemId());
            self.GetAllTicketListByStaff();
        }

        if (event.which) {
            //Actually clicked
            $('#nav-tab' + self.systemId()).click();
        } else {
            //Triggered by code
            self.search(page);
        }
    }

    //Hàm load lại dữ liệu trên các tab
    self.renderSystem = function () {
        self.listSystemRender([]);
        self.listStatus([]);
        $.post("/Purchase/GetRenderSystem", { active: self.active() }, function (data) {
            self.listStatus(data.listStatus);
            self.listSystemRender(data.listSystem);

            $('.nav-tabs').tabdrop();
            $(".select-view").select2();

            self.searchCustomer();
            self.searchUser();
        });
    }

    //Hàm lấy thông tin nhân viên
    self.searchUser = function () {
        $(".user-search")
            .select2({
                ajax: {
                    url: "User/GetUserSearch",
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
                                        <img class='w-40 mr10 mt5' src='/images/" + repo.avatar + "_50x50_1'/>\
                                    </div>\
                                    <div class='pull-left'>\
                                        <div>\
                                            <b>" + repo.text + "</b><br/>\
                                            <i class='fa fa-envelope-o'></i> " + repo.email + "<br/>\
                                            <i class='fa fa-phone'></i> " + repo.phone + "<br />\
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
                language: 'vi'
            });
    };

    //============================================================= Xử lý Orders ==========================================
    //Khai báo biến
    self.order = ko.observable(new orderModel());
    self.customerToAddress = ko.observableArray([]);
    self.customerFormAddress = ko.observableArray([]);
    self.orderExchange = ko.observable();
    self.totalPriceService = ko.observable();           //Total money dịch vụ
    self.userOrder = ko.observableArray([]);
    self.listPackageView = ko.observableArray([]);
    self.codePackage = ko.observable();
    self.listWarehouseOrder = ko.observableArray([]);
    self.listReason = ko.observableArray([]);
    self.listReasonNoCodeOfLading = ko.observableArray([]);
    self.listReasonNotEnoughInventory = ko.observableArray([]);
    self.warehouseIdOrder = ko.observable();
    self.warehouseOrder = ko.observable();
    self.expectedDate = ko.observable();
    self.listContractCode = ko.observableArray([]);
    self.contractCode = ko.observable();
    self.piceContractCode = ko.observable();
    self.successDetail = ko.observable(0);              //Số link Detail Orders đặt được

    self.resetFormOrder = function () {
        self.order(new orderModel());
        self.customerToAddress([]);
        self.customerFormAddress([]);
        self.orderExchange(undefined);
        self.totalPriceService(undefined);
        self.userOrder([]);
        self.listPackageView([]);
        self.codePackage();
        self.listWarehouseOrder([]);
        self.warehouseIdOrder(undefined);
        self.warehouseOrder(undefined);
        self.expectedDate(undefined);
        self.successDetail(0);
        self.listContractCode([]);
        self.orderReasonId(null);
        self.orderReasonNoCodeOfLadingId(null);
        self.orderReasonNotEnoughInventoryId(null);
        self.orderReasonText('');
        self.listOrderShop([]);
        self.bargainType(0);
    };

    //Hàm show modal Cancel Orders
    self.reasonId = ko.observable();
    self.reasonStatus = ko.observable();
    self.reasonCode = ko.observable();
    self.reasonType = ko.observable();
    self.reasonNote = ko.observable();

    self.orderClose = function (data) {
        self.isSubmit(true);

        self.reasonId("");
        self.reasonStatus("");
        self.reasonCode("");
        self.reasonType("");
        self.reasonNote("");

        self.reasonId(data.Id);
        self.reasonStatus(data.Status);
        self.reasonCode(ReturnCode(data.Code));
        self.reasonType(data.Type);
        $("#orderCloseModal").modal();
        $('#showReasonNote').show();
    };

    //Hàm Cancel Orders
    self.submitCancelOrder = function () {
        self.isSubmit(false);
        if (self.reasonNote() == undefined || self.reasonNote() === "") {
            toastr.error("Reason for order cancellation");
            self.isSubmit(true);
        }
        else if (self.reasonNote().length < 10) {
            toastr.error("Reason for canceling an order can not be less than 10 characters");
            self.isSubmit(true);
        }
        else {
            $.post("/Purchase/OrderCancel", { id: self.reasonId(), type: self.reasonType(), note: self.reasonNote(), status: self.reasonStatus() }, function (result) {
                if (result.status === msgType.success) {
                    toastr.success(result.msg);
                    $("#orderCloseModal").modal("hide");
                    $("#orderAddOrEditModal").modal("hide");
                    self.getInit();
                    self.search(page);
                    self.isSubmit(true);
                }
                else {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                }
            });
        }
    };

    self.orderCloseDetail = function () {
        self.isSubmit(true);
        $("#orderCloseModal").modal();
    };

    //Hàm gửi báo giá
    self.orderWait = function () {
        $.post("/OrderAwait/OrderWait", { id: self.order().Id(), type: self.order().Type() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                $('#orderAddOrEditModal').modal('hide');
                toastr.success(result.msg);
                self.getInit();
            }
        });
    };

    //nhận Orders về xử lý
    self.receivePurchaseOrder = function (data) {
        self.mapObject(data);
        $.post("/Order/ReceivePurchaseOrder", { id: self.order().Id() }, function (result) {
            if (result.status === msgType.success) {
                toastr.success(result.msg);
                self.getInit();
                self.search(page);
            }
            else {
                toastr.error(result.msg);
            }
        });
    };

    //Hàm show modal Divide Orders for staff
    self.assignedId = ko.observable();
    self.assignedStatus = ko.observable();
    self.assignedType = ko.observable();
    self.assignedCode = ko.observable();
    self.assignedUserId = ko.observable();
    self.assignedOrder = function (data) {
        self.isSubmit(true);
        //self.mapObject(data);
        self.assignedId(data.Id);
        self.assignedStatus(data.Status);
        self.assignedType(data.Type);
        self.assignedCode(ReturnCode(data.Code));

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });
        $("#orderAssignedModal").modal();
    };

    //Hàm phân Orders cho Staff handling
    self.submitAssignedOrder = function () {
        self.isSubmit(false);

        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.assignedUserId(); });

        $.post("/Purchase/AssignedOrder", { orderId: self.assignedId(), orderType: self.assignedType(), user: userSelect, status: self.assignedStatus() }, function (result) {
            if (result.status === msgType.success) {
                $("#orderAssignedModal").modal("hide");
                toastr.success(result.msg);
                self.getInit();
                self.search(page);
                self.isSubmit(true);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });
    };

    //Hàm show modal chuyển Orders cho nhân viên khác
    self.orderReplyModal = function (data) {
        data = ko.mapping.toJS(data);

        self.isSubmit(true);
        self.mapObject(data);
        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });
        $('#orderReplyModal').modal();
    };

    //Hàm chuyển Orders cho nhân viên khác
    self.submitOrderReplyModal = function () {
        self.isSubmit(false);

        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.order().UserId(); });

        $.post("/Order/OrderReply", { orderId: self.order().Id(), user: userSelect }, function (result) {
            if (result.status === msgType.success) {
                $('#orderReplyModal').modal('hide');
                toastr.success(result.msg);
                self.getInit();
                self.search(page);
                self.isSubmit(true);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });
    };

    //Hàm lấy nhiều Orders về xử lý
    self.receivePurchaseMultiOrder = function () {
        self.isSubmit(false);
        $.post("/Order/ReceivePurchaseMultiOrder", {}, function (result) {
            if (result.status === msgType.success) {
                self.isSubmit(true);

                var str = '';
                var p = '';
                _.each(result.listOrderCode, function (item) {
                    str += p + ReturnCode(item);
                    p = ', ';
                });

                toastr.success(result.msg);
                self.getInit();
                self.search(page);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });

    }

    //Hàm lấy nhiều Orders về xử lý
    self.receivePurchaseMultiOrderRetail = function () {
        self.isSubmit(false);
        $.post("/Order/ReceivePurchaseMultiOrderRetail", {}, function (result) {
            if (result.status === msgType.success) {
                self.isSubmit(true);

                var str = '';
                var p = '';
                _.each(result.listOrderCode, function (item) {
                    str += p + ReturnCode(item);
                    p = ', ';
                });

                toastr.success(result.msg);
                self.getInit();
                self.search(page);
            }
            else {
                toastr.error(result.msg);
                self.isSubmit(true);
            }
        });

    }

    //Hàm hiển thị form Edit thông tin Orders
    var oneCheck = false;
    self.listLog = ko.observableArray([]);
    self.orderReason = ko.observable();
    self.orderReasonNoCodeOfLading = ko.observable();
    self.orderReasonNotEnoughInventory = ko.observable();
    self.listOrderShop = ko.observableArray([]);
    self.bargainType = ko.observable();
    self.listBargainType = ko.observableArray([]);

    self.viewEditDetail = function (id) {
        self.isShowHistory(false);
        self.isShowLog(false);
        self.isDetailRending(false);
        self.resetFormOrder();

        $.post("/Order/GetOrderDetail", { orderId: id }, function (result) {
            self.successDetail(0);

            if (result.status !== msgType.error) {
                self.bargainType(result.order.BargainType + '');
                self.mapObject(result.order);
                self.customerToAddress(result.toAddress);
                self.customerFormAddress(result.formAddress);
                self.listWarehouseOrder(result.listWarehouse);
                self.listOrderExchage(result.listOrderExchage);
                self.mess(result.mess);
                self.userOrder(result.userOrder);
                self.orderReason(result.orderReason);
                self.orderReasonNoCodeOfLading(result.orderReasonNoCodeOfLading);
                self.orderReasonNotEnoughInventory(result.orderReasonNotEnoughInventory);
                self.listOrderShop(result.listOrderShop);

                if (result.orderReason != null) {
                    self.orderReasonId(result.orderReason.ReasonId);

                    if (result.orderReason.ReasonId === window.reasonOther) {
                        self.orderReasonText(result.orderReason.Reason);
                    }
                }

                if (result.orderReasonNoCodeOfLading != null) {
                    self.orderReasonId(result.orderReasonNoCodeOfLading.ReasonId);
                }

                if (result.orderReasonNotEnoughInventory != null) {
                    self.orderReasonId(result.orderReasonNotEnoughInventory.ReasonId);
                }

                //Lấy danh sách kiên hàng
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderListPackage",
                    data: { orderId: id },
                    success: function (resultListPackage) {
                        _.each(resultListPackage.listPackageView,
                            function (it) {
                                it.cacheTransportCode = it.TransportCode;
                                it.cacheForcastDate = it.ForcastDate;
                                it.cacheNote = it.Note;

                                it.TransportCode = ko.observable(it.TransportCode);
                                it.TransportCode.subscribe(function (newValue) {
                                    if (it.cacheTransportCode !== newValue) {
                                        self.updatePackage(ko.mapping.toJS(it));
                                        it.cacheTransportCode = newValue;
                                    }
                                });

                                it.ForcastDate = ko
                                    .observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                                it.ForcastDate.subscribe(function (newValue) {
                                    if (it.cacheForcastDate !== newValue) {
                                        self.updatePackage(ko.mapping.toJS(it));
                                        it.cacheForcastDate = newValue;
                                    }
                                });

                                it.Note = ko.observable(it.Note);
                                it.Note.subscribe(function (newValue) {
                                    if (it.cacheNote !== newValue) {
                                        self.updatePackage(ko.mapping.toJS(it));
                                        it.cacheNote = newValue;
                                    }
                                });
                            });

                        self.listPackageView(resultListPackage.listPackageView);
                    },
                    async: false
                });

                //Lấy danh sách Detail Orders
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderListDetail",
                    data: { orderId: id },
                    success: function (resultListDetail) {
                        self.setOrderDetai(resultListDetail);
                    },
                    async: false
                });

                //Lấy danh sách lịch sử và log
                $.ajax({
                    type: 'POST',
                    url: "/Order/GetOrderHistory",
                    data: { orderId: id },
                    success: function (resultHistory) {
                        self.listLog(resultHistory.listLog);
                        self.listHistory(resultHistory.listHistory);
                    },
                    async: false
                });

                self.warehouseIdOrder(result.order.WarehouseId);
                self.warehouseVNId(result.order.WarehouseDeliveryId);
                self.order().ListOrderService(result.listOrderService);
                self.customer(result.customer);
                self.orderExchange(result.orderExchange);
                self.totalPriceService(_.reduce(result.listOrderService,
                    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    0));

                if (self.orderExchange() !== null) {
                    //self.order()
                    //    .CashShortage(formatNumbericCN(Globalize('en-US').parseFloat(self.order().Total()) -
                    //        self.orderExchange().TotalPrice,
                    //        'N2'));
                    self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
                }

                self.setContractCode(result);
                self.calculatePrice();
                //Chat
                self.viewBoxChat.showChat(self.order().Id(), self.order().Code(), self.order().Type(), 1);
                $(".select-view").select2();
            }
            else {
                toastr.error(result.msg);
                return;
            }
            self.isDetailRending(true);

            self.orderCodeWarehouse(self.warehouseVNId(), self.order().Code());

            $('.datepicker')
                .datepicker({
                    autoclose: true,
                    language: 'en',
                    format: 'dd/mm/yyyy',
                    startDate: new Date()
                });

            $('#orderAddOrEditModal').modal();

            $('#orderAddOrEditModal')
                .on('hide.bs.modal',
                function (e) {
                    if (self.active() === 'order-delay' || self.active() === 'order') {
                        _.each(self.listOrder(),
                            function (item) {
                                if (item.Id == self.order().Id()) {
                                    $.post("/Order/GetOrderView",
                                        { id: id },
                                        function (data) {
                                            item.Status(data.Status);
                                            item.LinkNo(data.LinkNo);
                                            item.ProductNo(data.ProductNo);
                                            item.TotalPrice(data.TotalPrice);
                                            item.TotalExchange(data.TotalExchange);
                                            item.UserNote(data.UserNote);
                                        });
                                }
                            });
                    }
                });
        });
    };

    self.CheckbargainType = function () {
        var isOk = true;

        $.post("/Order/UpdateOrderBargainType", { orderId: self.order().Id, bargainType: self.bargainType }, function (data) {
            if (data.status !== msgType.success) {
                isOk = false;
                toastr.error(data.msg);
            }
        });
        return isOk;
    };

    //Đặt được hàng trong Detail Orders
    self.orderDetailSuccess = function (data) {
        //self.isDetailRending(false);
        $('#update').modal();

        $.post("/Order/OrderDetailSuccess", { id: data.Id }, function (result) {
            if (result.status !== msgType.success) {
                toastr.error(result.msg);
            }
            else {
                $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                    self.successDetail(0);

                    self.bargainType(result.order.BargainType + '');
                    self.mapObject(result.order);
                    self.listOrderExchage(result.listOrderExchage);

                    //Lấy danh sách Detail Orders
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderListDetail",
                        data: { orderId: data.OrderId },
                        success: function (resultListDetail) {
                            self.setOrderDetai(resultListDetail);
                        },
                        async: false
                    });

                    //Lấy danh sách lịch sử và log
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderHistory",
                        data: { orderId: data.OrderId },
                        success: function (resultHistory) {
                            self.listLog(resultHistory.listLog);
                            self.listHistory(resultHistory.listHistory);
                        },
                        async: false
                    });

                    self.order().ListOrderService(result.listOrderService);
                    self.orderExchange(result.orderExchange);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    if (self.orderExchange() !== null) {
                        //self.order().CashShortage(formatNumbericCN(Globalize('en-US').parseFloat(self.order().Total()) - self.orderExchange().TotalPrice, 'N0'));
                        self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
                    }

                    self.calculatePrice();

                    //self.isDetailRending(true);
                    $(".select-view").select2();
                    $('#update').modal('hide');
                });

                toastr.success(result.msg);
            }
            //self.isDetailRending(true);
        });
    };

    //Hủy đặt hàng trong Detail Orders
    self.orderDetailCancel = function (data) {
        //self.isDetailRending(false);
        $('#update').modal();

        $.post("/Order/OrderDetailCancel", { id: data.Id }, function (result) {

            if (result.status !== msgType.success) {
                toastr.error(result.msg);
            }
            else {
                $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                    self.successDetail(0);

                    //self.setOrderDetai(result);

                    //self.calculatePrice();

                    self.bargainType(result.order.BargainType + '');
                    self.mapObject(result.order);
                    self.listOrderExchage(result.listOrderExchage);

                    //Lấy danh sách Detail Orders
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderListDetail",
                        data: { orderId: data.OrderId },
                        success: function (resultListDetail) {
                            self.setOrderDetai(resultListDetail);
                        },
                        async: false
                    });

                    //Lấy danh sách lịch sử và log
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderHistory",
                        data: { orderId: data.OrderId },
                        success: function (resultHistory) {
                            self.listLog(resultHistory.listLog);
                            self.listHistory(resultHistory.listHistory);
                        },
                        async: false
                    });

                    self.order().ListOrderService(result.listOrderService);
                    self.orderExchange(result.orderExchange);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    if (self.orderExchange() !== null) {
                        //self.order()
                        //    .CashShortage(formatNumbericCN(Globalize('en-US').parseFloat(self.order().Total()) -
                        //        self.orderExchange().TotalPrice,
                        //        'N2'));
                        self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
                    }

                    self.calculatePrice();

                    //self.isDetailRending(true);
                    $(".select-view").select2();
                    $('#update').modal('hide');
                });
                toastr.success(result.msg);
            }
            //self.isDetailRending(true);
        });
    };

    //Cập nhật Detail Orders
    self.editDetailOrder = function (data) {
        var isLook = false;
        data.QuantityBooked = formatVN(data.QuantityBooked);
        data.Price = formatVN(data.Price);

        console.log(data);

        $('#update').modal();
        $.post({
            url: "/Order/OrderDetailUpdate",
            data: { orderDetailMode: data },
            success: function (result) {
                if (result.status === msgType.success) {

                    //self.order().ListOrderService(result.listOrderService);
                    //self.totalPriceService(_.reduce(result.listOrderService,
                    //    function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                    //    0));

                    //self.setOrderDetai(result);

                    $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                        self.successDetail(0);

                        self.bargainType(result.order.BargainType + '');
                        self.mapObject(result.order);
                        self.listOrderExchage(result.listOrderExchage);

                        //Lấy danh sách Detail Orders
                        $.ajax({
                            type: 'POST',
                            url: "/Order/GetOrderListDetail",
                            data: { orderId: data.OrderId },
                            success: function (resultListDetail) {
                                self.setOrderDetai(resultListDetail);
                            },
                            async: false
                        });

                        //Lấy danh sách lịch sử và log
                        $.ajax({
                            type: 'POST',
                            url: "/Order/GetOrderHistory",
                            data: { orderId: data.OrderId },
                            success: function (resultHistory) {
                                self.listLog(resultHistory.listLog);
                                self.listHistory(resultHistory.listHistory);
                            },
                            async: false
                        });

                        self.order().ListOrderService(result.listOrderService);
                        self.orderExchange(result.orderExchange);
                        self.totalPriceService(_.reduce(result.listOrderService,
                            function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                            0));

                        if (self.orderExchange() !== null) {
                            //self.order().CashShortage(formatNumbericCN(Globalize('en-US').parseFloat(self.order().Total()) - self.orderExchange().TotalPrice, 'N2'));
                            self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
                        }

                        self.calculatePrice();

                        //self.isDetailRending(true);
                        $(".select-view").select2();
                    });


                    isLook = true;
                } else {
                    toastr.error(result.msg);
                    isLook = false;
                }

                $('#update').modal('hide');
            },
            async: false
        });

        return isLook;
    };

    //Cập nhật dịch vụ khách hàng
    self.updateService = function (data) {
        $('#update').modal();
        $.post("/Order/UpdateService", { id: data.Id }, function (result) {
            $.post("/Order/GetOrderDetail", { orderId: data.OrderId }, function (result) {
                if (result == -1) {
                    toastr.error('Orders: ' + ReturnCode(data.Code) + ' does not exist or has been deleted!');
                }
                else {
                    //self.order().ListOrderService(result.listOrderService);
                    //self.totalPriceService(_.reduce(result.listOrderService, function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); }, 0));

                    self.bargainType(result.order.BargainType + '');
                    self.mapObject(result.order);
                    self.listOrderExchage(result.listOrderExchage);

                    //Lấy danh sách Detail Orders
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderListDetail",
                        data: { orderId: data.OrderId },
                        success: function (resultListDetail) {
                            self.setOrderDetai(resultListDetail);
                        },
                        async: false
                    });

                    //Lấy danh sách lịch sử và log
                    $.ajax({
                        type: 'POST',
                        url: "/Order/GetOrderHistory",
                        data: { orderId: data.OrderId },
                        success: function (resultHistory) {
                            self.listLog(resultHistory.listLog);
                            self.listHistory(resultHistory.listHistory);
                        },
                        async: false
                    });

                    self.order().ListOrderService(result.listOrderService);
                    self.orderExchange(result.orderExchange);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));

                    if (self.orderExchange() !== null) {
                        //self.order()
                        //    .CashShortage(formatNumbericCN(Globalize('en-US').parseFloat(self.order().Total()) -
                        //        self.orderExchange().TotalPrice,
                        //        'N2'));

                        self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
                    }

                    self.calculatePrice();
                    $(".select-view").select2();
                }
            });
            $('#update').modal('hide');
        });
    };

    //Hàm cập nhật thông tin Orders
    self.cssFeeShipBargain = ko.observable('');
    self.isSelectedFeeShipBargain = ko.observable(false);

    self.cssFeeShip = ko.observable('');
    self.isSelectedFeeShip = ko.observable(false);

    self.updateOrder = function () {

        self.calculatePrice();

        if (Globalize('en-US').parseFloat(self.order().TotalShop()) > Globalize('en-US').parseFloat(self.order().TotalPriceCustomer())) {
            toastr.error('Actual cash payment amount less than or equal total money');
            self.cssFeeShipBargain('error');
            self.isSelectedFeeShipBargain(true);
            return true;
        }
        if (Globalize('en-US').parseFloat(self.order().FeeShip()) < Globalize('en-US').parseFloat(self.order().FeeShipBargain())) {
            toastr.error('Chinese domestic delivery money customers pay cash delivery smaller Chinese domestic companies pay');
            self.cssFeeShipBargain('error');
            self.isSelectedFeeShipBargain(true);
            return true;
        }

        $('#update').modal();

        $.post("/Order/UpdateOrder",
            {
                id: self.order().Id(),                                      //Id của Orders
                priceBargain: formatVN(self.order().PriceBargain()),        //Tiền bargain được
                totalShop: formatVN(self.order().TotalShop()),              //Total money paid company with shop
                feeShipBargain: formatVN(self.order().FeeShipBargain()),    //Tiền phí ship công ty thanh toán
                feeShip: formatVN(self.order().FeeShip()),                  //Tiền phí ship khách thanh toán
                __RequestVerifiCationToken: self.token()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);

                    self.cssFeeShipBargain('error');
                    self.isSelectedFeeShipBargain(true);
                } else {
                    self.cssFeeShipBargain('');
                    self.isSelectedFeeShipBargain(false);

                    self.cssFeeShip = ko.observable('');
                    self.isSelectedFeeShip = ko.observable(false);

                    self.order().ListOrderService(result.listOrderService);
                    self.totalPriceService(_.reduce(result.listOrderService,
                        function (count, item) { return count + (item.Checked === true ? item.TotalPrice : 0); },
                        0));
                    self.calculatePrice();
                }
                $('#update').modal('hide');
            });
        return true;
    };

    //Hàm cập nhật thông tin Orders
    self.updateOrderUserNote = function () {
        $('#update').modal();
        self.calculatePrice();

        $.post("/Order/UpdateOrderUserNote",
            {
                id: self.order().Id(),
                note: self.order().UserNote(),
                __RequestVerifiCationToken: self.token()
            },
            function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    //toastr.success(result.msg);
                }
                $('#update').modal('hide');
            });
        return true;
    };

    self.orderExchangeOther = ko.observable(0); //Tiền khác khách hàng đã thanh toán
    self.IsExchangeOther = ko.observable(false);
    self.titleExchangeOther = ko.observable("Detail");
    self.showExchangeOther = function () {
        self.IsExchangeOther(!self.IsExchangeOther());
        self.titleExchangeOther(self.IsExchangeOther() ? "collapse" : "Detail");
    };


    self.titleTotalCus = ko.observable('Customer must pay');
    self.calculatePrice = function () {

        self.order().TotalPrice(formatNumbericCN(_.reduce(self.order().ListDetail(), function (count, item) { return count + (item.Status === 1 ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.QuantityBooked()))); }, 0), 'N2'));
        self.order().TotalShop(formatNumbericCN(_.reduce(self.listContractCode(), function (count, item) { return count + (item.Status === 1 ? 0 : Globalize('en-US').parseFloat(item.TotalPrice())); }, 0), 'N2'));
        self.orderExchangeOther(_.reduce(self.listOrderExchage(), function (count, item) { return count + item.TotalPrice; }, 0));

        //Tiền khách
        var priceCustomer = Globalize('en-US').parseFloat(self.order().TotalPrice());
        var shipCustomer = Globalize('en-US').parseFloat(self.order().FeeShip());
        var totalCustomer = priceCustomer + shipCustomer;
        self.order().TotalPriceCustomer(formatNumbericCN(totalCustomer, 'N2'));

        //Tiền công ty
        var totalCompany = Globalize('en-US').parseFloat(self.order().TotalShop());
        var shipCompany = Globalize('en-US').parseFloat(self.order().FeeShipBargain());
        if (totalCompany !== 0) {
            if (totalCompany < shipCompany) {
                self.order().FeeShipBargain(0);
                shipCompany = 0;
            }
            var priceCompany = totalCompany - shipCompany;

            if (priceCompany > priceCustomer) {
                shipCompany += priceCompany - priceCustomer;
                self.order().FeeShipBargain(formatNumbericCN(shipCompany, 'N2'));
                priceCompany = priceCustomer;
            }

            self.order().PaidShop(formatNumbericCN(priceCompany, 'N2'));
        } else {
            self.order().TotalShop(0);
            self.order().PaidShop(0);
            self.order().FeeShipBargain(0);
        }

        //Tiền bargain được
        var priceBargain = totalCustomer - totalCompany;
        if (priceBargain < 0) {
            priceBargain = 0;
        }

        self.order().PriceBargain(formatNumbericCN(priceBargain, 'N2'));

        //Tính lại tiền việt nam đồng
        var totalPrice = Globalize('en-US').parseFloat(self.order().TotalPrice());
        var exchangeRate = Globalize('en-US').parseFloat(self.order().ExchangeRate());

        self.order().TotalExchange(formatNumbericCN((totalPrice * exchangeRate), 'N2'));
        self.order().Total(formatNumbericCN((totalPrice * exchangeRate + self.totalPriceService()), 'N2'));

        //var cashShortage = (totalPrice * exchangeRate + self.totalPriceService() - (self.orderExchange() === null ? 0 : self.orderExchange().TotalPrice) - self.orderExchangeOther());
        //if (cashShortage < 0) {
        //    self.titleTotalCus('Money to pay customer');
        //    self.order().CashShortage(formatNumbericCN(cashShortage * (-1), 'N2'));
        //} else {
        //    self.titleTotalCus('Customer must pay');
        //    self.order().CashShortage(formatNumbericCN(cashShortage, 'N2'));
        //}

        if (self.order().Debt() < 0) {
            self.titleTotalCus('Money to pay customer');
            self.order().CashShortage(formatNumbericCN(self.order().Debt() * (-1), 'N2'));
        } else {
            self.titleTotalCus('Customer must pay');
            self.order().CashShortage(formatNumbericCN(self.order().Debt(), 'N2'));
        }

        self.initInputMark();
        self.autoSize();
    }

    self.totalPriceIsCheck = ko.observable(0);

    self.setOrderDetai = function (result) {
        var list = [];
        _.each(result.listOrderDetail,
            function (item) {
                item.isCheck = ko.observable(false);

                item.cachePrice = item.Price;
                item.cacheQuantityBooked = item.QuantityBooked;

                item.Quantity = ko.observable(formatNumbericCN(item.Quantity, 'N0'));
                item.QuantityBooked = ko.observable(formatNumbericCN(item.QuantityBooked, 'N0'));
                item.QuantityActuallyReceived = ko.observable(formatNumbericCN(item.QuantityActuallyReceived, 'N0'));
                item.Price = ko.observable(formatNumbericCN(item.Price, 'N2'));
                item.ExchangeRate = ko.observable(formatNumbericCN(item.ExchangeRate, 'N2'));
                item.ExchangePrice = ko.observable(formatNumbericCN(item.ExchangePrice, 'N2'));
                item.TotalPrice = ko.observable(formatNumbericCN(item.TotalPrice, 'N2'));
                item.TotalExchange = ko.observable(formatNumbericCN(item.TotalExchange, 'N2'));
                item.UserNote = ko.observable(item.UserNote);

                item.cssQuantityBooked = ko.observable();
                item.isSelectedQuantityBooked = ko.observable(false);
                item.QuantityBooked.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        item.isSelectedQuantityBooked(true);
                        item.cssQuantityBooked('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (item.cacheQuantityBooked !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.editDetailOrder(ko.mapping.toJS(item))) {
                                item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                            } else {
                                item.isSelectedQuantityBooked(true);
                                item.cssQuantityBooked('error');
                            }
                        } else {
                            item.isSelectedQuantityBooked(false);
                            item.cssQuantityBooked('');
                            item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                        }
                    }
                });

                item.cssPrice = ko.observable();
                item.isSelectedPrice = ko.observable(false);

                item.Price.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        item.isSelectedPrice(true);
                        item.cssPrice('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (item.cachePrice !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.editDetailOrder(ko.mapping.toJS(item))) {
                                item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                            } else {
                                item.isSelectedPrice(true);
                                item.cssPrice('error');
                            }
                        } else {
                            item.isSelectedPrice(false);
                            item.cssPrice('');
                            item.TotalPrice(formatNumbericCN(Globalize('en-US').parseFloat(item.QuantityBooked()) * Globalize('en-US').parseFloat(item.Price()), 'N2'));
                        }
                    }
                });

                item.UserNote.subscribe(function (newValue) {
                    self.editDetailOrder(ko.mapping.toJS(item));
                });

                if (item.Status === 0) {
                    self.successDetail(self.successDetail() + 1);
                }

                item.isCheck.subscribe(function (newValue) {
                    self.totalPriceIsCheck(formatNumbericCN(_.reduce(self.order().ListDetail(), function (count, item) { return count + (item.isCheck() === false ? 0 : (Globalize('en-US').parseFloat(item.Price()) * Globalize('en-US').parseFloat(item.QuantityBooked()))); }, 0), 'N2'));
                });

                list.push(item);
            });

        self.order().ListDetail(list);
    }

    self.isDisableFeeShipBargain = ko.observable(true);
    self.setContractCode = function (result) {
        if (result.listContractCode.length === 0) {
            self.isDisableFeeShipBargain(true);
        }
        // danh sách Contract
        _.each(result.listContractCode,
            function (it) {
                it.cacheContractCode = it.ContractCode;
                it.cacheTotalPrice = it.TotalPrice;
                if (it.TotalPrice > 0) {
                    self.isDisableFeeShipBargain(false);
                }

                //định nghĩa cho biến knockout
                it.ContractCode = ko.observable(it.ContractCode);
                it.TotalPrice = ko.observable(formatNumbericCN((it.TotalPrice == '0' ? '' : it.TotalPrice), 'N2'));

                it.cssTotalPrice = ko.observable();
                it.isSelectedTotalPrice = ko.observable(false);
                it.isDisableTotalPrice = ko.observable(true);

                if (it.ContractCode()) {
                    it.isDisableTotalPrice(false);
                }

                it.TotalPrice.subscribe(function (newValue) {
                    if (!newValue || newValue === '0') {
                        it.isSelectedTotalPrice(true);
                        it.cssTotalPrice('error');
                        toastr.error('The input value must be greater 0');
                    } else {
                        if (it.cacheTotalPrice !== Globalize('en-US').parseFloat(newValue)) {
                            if (self.updateContractCodeOrder(ko.mapping.toJS(it))) {
                                it.cacheTotalPrice = Globalize('en-US').parseFloat(newValue);
                            } else {
                                it.isSelectedTotalPrice(true);
                                it.cssTotalPrice('error');
                            }
                        } else {
                            it.isSelectedTotalPrice(false);
                            it.cssTotalPrice('');
                        }
                    }
                });

                it.cssContractCode = ko.observable();
                it.isSelectedContractCode = ko.observable(false);
                it.ContractCode.subscribe(function (newValue) {
                    if (newValue) {
                        it.isDisableTotalPrice(false);
                        if (it.cacheContractCode !== newValue) {
                            if (self.updateContractCodeOrder(ko.mapping.toJS(it))) {
                                it.cacheContractCode = newValue;
                            } else {
                                it.isDisableTotalPrice(true);
                                it.isSelectedContractCode(true);
                                it.cssContractCode('error');
                            }
                        } else {
                            it.isDisableTotalPrice(false);
                            it.isSelectedContractCode(false);
                            it.cssContractCode('');
                        }
                    } else {
                        it.isSelectedContractCode(true);
                        it.isDisableTotalPrice(true);
                        it.cssContractCode('error');
                        toastr.error('Can not be empty');
                    }
                });
            });

        self.listContractCode(result.listContractCode);

        self.calculatePrice();
    }

    // Hàm lấy thông tin kho khi đã chọn
    self.warehouseIdOrder.subscribe(function (newId) {
        var warehouse = _.find(self.listWarehouseOrder(), function (item) { return item.Id === newId; });

        if (warehouse !== undefined) {

            if (self.warehouseIdOrder() === self.order().WarehouseId()) {
                return;
            }
            $('#update').modal();
            self.warehouseOrder(warehouse);
            self.order().WarehouseId(warehouse.Id);
            self.order().WarehouseName(warehouse.Name);

            $.post("/Order/UpdateOrderWarehouse",
                {
                    id: self.order().Id(),
                    warehouseId: self.order().WarehouseId(),
                    warehouseName: self.order().WarehouseName()
                },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        //toastr.success(result.msg);
                    }
                    $('#update').modal('hide');
                });
        }
    });

    //Hàm Add the tracking code
    self.addPackage = function () {
        self.isSubmit(false);

        if (self.codePackage() === '' || self.codePackage() === null || self.codePackage() === undefined) {
            toastr.error('Transport code Can not be empty!');
            self.isSubmit(true);
        } else {
            $.post("/Order/AddContractCode", { id: self.order().Id(), codePackage: self.codePackage() }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    self.isSubmit(true);
                } else {
                    _.each(result.list,
                        function (it) {
                            it.cacheTransportCode = it.TransportCode;
                            it.cacheForcastDate = it.ForcastDate;
                            it.cacheNote = it.Note;

                            it.TransportCode = ko.observable(it.TransportCode);
                            it.TransportCode.subscribe(function (newValue) {
                                if (it.cacheTransportCode !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheTransportCode = newValue;
                                }
                            });

                            it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                            it.ForcastDate.subscribe(function (newValue) {
                                if (it.cacheForcastDate !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheForcastDate = newValue;
                                }
                            });

                            it.Note = ko.observable(it.Note);
                            it.Note.subscribe(function (newValue) {
                                if (it.cacheNote !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheNote = newValue;
                                }
                            });
                        });
                    self.listPackageView(result.list);

                    $('.datepicker')
                        .datepicker({
                            autoclose: true,
                            language: 'en',
                            format: 'dd/mm/yyyy',
                            startDate: new Date()
                        });

                    self.isSubmit(true);
                }
            });
            self.codePackage('');
        }
    };

    //Thêm Contract code
    self.showContractCode = function () {
        self.addContractCode();
    }

    self.addContractCode = function () {
        self.isSubmit(false);
        $.post("/Order/AddContractCodeOrder", { id: self.order().Id(), contractCode: '', pice: 0 }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
                self.isSubmit(true);
            } else {
                self.setContractCode(result);
                self.isSubmit(true);
                self.calculatePrice();
            }
        });
        self.contractCode('');
        self.piceContractCode(0);
    }

    //Hàm Edit mã vận đơn
    self.updatePackage = function (data) {
        $('#update').modal();
        $.post("/Order/EditContractCode", { packageId: data.Id, packageName: data.TransportCode, date: data.ForcastDate, note: data.Note }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            }
            $('#update').modal('hide');
        });
    };

    //Hàm Edit Contract code
    self.updateContractCodeOrder = function (data) {
        var isLook = false;
        data.TotalPrice = formatVN(data.TotalPrice);

        $('#update').modal();
        $.post({
            url: "/Order/EditContractCodeOrder",
            data: { id: data.Id, code: data.ContractCode, totalPrice: data.TotalPrice },
            success: function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    isLook = false;
                } else {
                    self.setContractCode(result);
                    isLook = true;
                }
                $('#update').modal('hide');
            },
            async: false
        });
    };

    //Hàm xóa mã vận đơn
    self.deletePackage = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'Transport code "' + data.TransportCode() + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/Order/DeleteContractCode", { id: data.Id }, function (result) {
                if (result === -1) {
                    toastr.error('Orders: ' + ReturnCode(data.OrderCode()) + ' does not exist or has been deleted!');
                } else {
                    toastr.success('Successfully deleted transport code "' + data.TransportCode() + '"');

                    _.each(result.list,
                        function (it) {
                            it.cacheTransportCode = it.TransportCode;
                            it.cacheForcastDate = it.ForcastDate;
                            it.cacheNote = it.Note;

                            it.TransportCode = ko.observable(it.TransportCode);
                            it.TransportCode.subscribe(function (newValue) {
                                if (it.cacheTransportCode !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheTransportCode = newValue;
                                }
                            });

                            it.ForcastDate = ko.observable(it.ForcastDate ? moment(it.ForcastDate).format("L") : '');
                            it.ForcastDate.subscribe(function (newValue) {
                                if (it.cacheForcastDate !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheForcastDate = newValue;
                                }
                            });

                            it.Note = ko.observable(it.Note);
                            it.Note.subscribe(function (newValue) {
                                if (it.cacheNote !== newValue) {
                                    self.updatePackage(ko.mapping.toJS(it));
                                    it.cacheNote = newValue;
                                }
                            });
                        });
                    self.listPackageView(result.list);
                }
            });
        }, function () { });
    };

    //Hàm xóa Contract code
    self.deleteContractCode = function (data) {
        swal({
            title: 'Are you sure you want to delete this item?',
            text: 'contract code "' + data.ContractCode() + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            $.post("/Order/DeleteContractCodeOrder", { id: data.Id }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.setContractCode(result);
                }
            });
        }, function () { });
    };

    //Gủi lại kế toán thanh toán
    self.reviewContractCode = function (data) {
        $.post("/Order/ReviewContractCodeOrder", { id: data.Id }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.setContractCode(result);
            }
        });
    };

    //Gủi lại kế toán thanh toán Orders mới
    self.sendContractCode = function (data) {
        $.post("/Order/SendContractCodeOrder", { id: data.Id }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                toastr.success(result.msg);
                self.setContractCode(result);
            }
        });
    };


    //Đặt xong Orders
    self.orderSuccess = function () {
        if (self.warehouseIdOrder() === undefined) {
            toastr.error('Warehouse not selected!');
            return;
        }

        if (self.orderReason() != null) {
            if (self.orderReason().ReasonId === 0) {
                toastr.error('No select to hanlding!');
                return;
            }
        }

        $.post("/Order/OrderSuccess", { id: self.order().Id() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                $('#orderAddOrEditModal').modal('hide');
                toastr.success(result.msg);
                self.getInit();
            }
        });
    };

    //Hàm hiển thị form thêm mới Orders
    self.viewAddDetail = function () {
        $('#orderAddOrEditModal').modal();
    };

    //self.viewTicketDetail = function (data) {
    //    $('#ticketDetailModal').modal();
    //};

    //Hiển thị chat
    self.showChatOrder = function (data) {
        if (chatViewModel) {
            chatViewModel.showChat(data.Id, data.Code, data.Type);

            $('#chatModal')
                .on('hide.bs.modal',
                function (e) {
                    //self.search(page);
                });
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailViewModel) {
            orderDetailViewModel.viewOrderDetail(orderId);
            return;
        }
    }

    //Lý do trễ xử lý
    self.orderReasonId = ko.observable();
    self.orderReasonText = ko.observable();

    self.orderReasonId.subscribe(function (newValue) {
        if (newValue) {
            if (newValue !== self.orderReason().ReasonId) {
                self.submitReason();
            }
        }
    });

    self.orderReasonText.subscribe(function (newValue) {
        if (newValue) {
            if (newValue !== self.orderReason().Reason) {
                self.submitReason();
            }
        }
    });

    self.submitReason = function () {
        $.post("/Order/Reason", { id: self.order().Id, orderReasonId: self.orderReasonId(), orderReasonText: self.orderReasonText() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                self.orderReason(result.orderReason);
            }
        });
    }

    //Lý do chưa có mã vận đơn
    self.orderReasonNoCodeOfLadingId = ko.observable();

    self.orderReasonNoCodeOfLadingId.subscribe(function (newValue) {
        if (newValue) {
            if (newValue !== self.orderReasonNoCodeOfLading().ReasonId) {
                self.submitReasonNoCodeOfLading();
            }
        }
    });

    self.submitReasonNoCodeOfLading = function () {
        $.post("/Order/ReasonNoCodeOfLading", { id: self.order().Id, orderReasonId: self.orderReasonNoCodeOfLadingId() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                self.orderReasonNoCodeOfLading(result.orderReason);
            }
        });
    }

    //Lý do chưa đủ kiện về kho
    self.orderReasonNotEnoughInventoryId = ko.observable();

    self.orderReasonNotEnoughInventoryId.subscribe(function (newValue) {
        if (newValue) {
            if (newValue !== self.orderReasonNotEnoughInventory().ReasonId) {
                self.submitReasonNotEnoughInventory();
            }
        }
    });

    self.submitReasonNotEnoughInventory = function () {
        $.post("/Order/ReasonNotEnoughInventory", { id: self.order().Id, orderReasonId: self.orderReasonNotEnoughInventoryId() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                self.orderReasonNotEnoughInventory(result.orderReason);
            }
        });
    }


    self.showOrderShop = function () {
        $('#orderShop').modal();
    }

    self.listBargain = ko.observableArray([]);
    self.listPrice = ko.observableArray([]);
    self.listRatioPrice = ko.observableArray([]);

    self.showOrderHistoryShop = function () {
        $.post("/Order/OrderHistoryShop", { name: self.order().ShopName() }, function (result) {
            if (result.status === msgType.error) {
                toastr.error(result.msg);
            } else {
                self.listBargain(result.listBargain);
                self.listPrice(result.listPrice);
                self.listRatioPrice(result.listRatioPrice);
            }
        });



        $('#orderHistoryShop').modal();
    }
    self.forwardTransportCode = ko.observable([]);
    self.fTCOrderCode = ko.observable();
    self.showForwardTransportCode = function (data) {
        self.forwardTransportCode(data);

        $('#forwardTransportCode').modal();
    }

    self.fTCForward = function () {

    }
    //Hàm map dữ liệu
    self.mapObject = function (data) {
        self.order(new orderModel());

        self.order().Id(data.Id);
        self.order().Code(data.Code);
        self.order().Type(data.Type);
        self.order().IsRetail(data.IsRetail);
        self.order().WebsiteName(data.WebsiteName);
        self.order().ShopId(data.ShopId);
        self.order().ShopName(data.ShopName);
        self.order().ShopLink(data.ShopLink);
        self.order().ProductNo(formatNumbericCN(data.ProductNo, 'N0'));
        self.order().PackageNo(formatNumbericCN(data.PackageNo, 'N0'));
        self.order().ContractCode(data.ContractCode);
        self.order().ContractCodes(data.ContractCodes);
        self.order().LevelId(data.LevelId);
        self.order().LevelName(data.LevelName);
        self.order().TotalWeight(formatNumbericCN(data.TotalWeight, 'N2'));
        self.order().DiscountType(data.DiscountType);
        self.order().DiscountValue(data.DiscountValue);
        self.order().GiftCode(data.GiftCode);
        self.order().CreatedTool(data.CreatedTool);
        self.order().Currency(formatNumbericCN(data.Currency, 'N2'));
        self.order().ExchangeRate(formatNumbericCN(data.ExchangeRate, 'N2'));
        self.order().TotalExchange(formatNumbericCN(data.TotalExchange, 'N2'));
        self.order().Total(formatNumbericCN(data.Total, 'N2'));
        self.order().HashTag(data.HashTag);
        self.order().WarehouseId(data.WarehouseId);
        self.order().WarehouseName(data.WarehouseName);
        self.order().WarehouseDeliveryId(data.WarehouseDeliveryId);
        self.order().WarehouseDeliveryName(data.WarehouseDeliveryName);
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
        self.order().ExpectedDate(data.ExpectedDate);
        self.order().UserNote(data.UserNote);
        self.order().IsPayWarehouseShip(data.IsPayWarehouseShip);
        self.order().Debt(data.Debt);
        self.order().TotalRefunded(data.TotalRefunded);
        self.order().TotalPayed(data.TotalPayed === null ? 0 : data.TotalPayed);
        //self.order().TotalPrice(formatNumberic(data.TotalPrice === null ? 0 : data.TotalPrice, 'N2'));
        self.order().DepositPercent(data.DepositPercent);

        //Tiền hàng khách trả
        self.order().TotalPrice(formatNumbericCN(data.TotalPrice, 'N2'));

        //Tiền hàng công ty trả
        self.order().PaidShop(formatNumbericCN(data.PaidShop === null ? 0 : data.PaidShop, 'N2'));

        //Phí ship khách trả
        self.order().FeeShip(formatNumbericCN(data.FeeShip === null ? 0 : data.FeeShip, 'N2'));

        //Phí ship công ty trả
        self.order().FeeShipBargain(formatNumbericCN(data.FeeShipBargain === null ? 0 : data.FeeShipBargain, 'N2'));

        //Total money hàng khách trả (Tiền hàng + Phí ship trung quốc)
        self.order().TotalPriceCustomer(formatNumbericCN(data.TotalPrice + data.FeeShip, 'N2'));

        //Tông tiền thanh toán với shop (Tiền hàng công ty trả + Phí ship công ty trả)
        self.order().TotalShop(formatNumbericCN(data.PaidShop + data.FeeShipBargain, 'N2'));

        //Tiền bargain được (Total money khách trả - Total money công ty trả)
        self.order().PriceBargain(formatNumbericCN(data.PriceBargain === null ? 0 : data.PriceBargain, 'N2'));
    }

    //Thanh toán Contract
    self.paymentContractCode = function () {
        $.post("/Purchase/AccountantAwaitingPayment",
            {
                id: self.contractCodeIdSubmit(),
                accountantSubjectId: self.accountantSubjectId(),
                financeFundId: self.financeFundId(),
                financeFundName: self.financeFundName(),
                financeFundBankAccountNumber: self.financeFundBankAccountNumber(),
                financeFundDepartment: self.financeFundDepartment(),
                financeFundNameBank: self.financeFundNameBank(),
                financeFundUserFullName: self.financeFundUserFullName(),
                financeFundUserPhone: self.financeFundUserPhone(),
                financeFundUserEmail: self.financeFundUserEmail(),
                treasureId: self.treasureId(),
                treasureName: self.treasureName(),
                currencyFluctuations: self.currencyFluctuations(),
                note: self.note(),
                status: self.contractCodeStatusSubmit()
            }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.search(1);
                }

                $('#moneyFundAddOrEdit').modal('hide');
            });
    };

    //Hủy thanh toán Contract
    self.cannalContractCode = function (data) {
        swal({
            title: 'You definitely want to cancel?',
            text: 'Contract code "' + data.ContractCode + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Delete'
        }).then(function () {
            toastr.success("Deleted successfully");
            $.post("/Purchase/AccountantCancel", { id: data.Id, status: data.Status }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.search(1);
                }
            });
        }, function () { });
    };

    //Show hiển thị
    var onCheck = false;
    self.contractCodeIdSubmit = ko.observable();
    self.contractCodeStatusSubmit = ko.observable();
    self.listAccountantSubject = ko.observableArray([]);

    self.accountantSubjectId = ko.observable();

    self.financeFundId = ko.observable();
    self.financeFundName = ko.observable();
    self.financeFundBankAccountNumber = ko.observable();
    self.financeFundDepartment = ko.observable();
    self.financeFundNameBank = ko.observable();
    self.financeFundUserFullName = ko.observable();
    self.financeFundUserPhone = ko.observable();
    self.financeFundUserEmail = ko.observable();

    self.treasureId = ko.observable();
    self.treasureName = ko.observable();

    self.currencyFluctuations = ko.observable();
    self.note = ko.observable();

    self.showPaymentContractCode = function (data) {
        self.contractCodeIdSubmit(data.Id);
        self.contractCodeStatusSubmit(data.Status);
        self.accountantSubjectId('');
        self.financeFundId('');
        self.financeFundName('');
        self.financeFundBankAccountNumber('');
        self.financeFundDepartment('');
        self.financeFundNameBank('');
        self.financeFundUserFullName('');
        self.financeFundUserPhone('');
        self.financeFundUserEmail('');
        self.treasureId('');
        self.treasureName('');
        self.currencyFluctuations('');

        self.currencyFluctuations(formatNumberic(data.TotalPrice, 'N2'));
        $.post("/Purchase/GetAccountant", {}, function (result) {
            $('#moneyFundAddOrEdit').modal();

            if (onCheck === false) {
                self.listAccountantSubject(result.listAccountantSubject);

                $("#treasure_tree")
                    .dropdownjstree({
                        source: JSON.parse(result.treasureTree),
                        selectedNode: '',
                        selectNote: (node, selected) => { // sự kiện chọn
                            self.treasureId(selected.node.original.id);
                            self.treasureName(selected.node.original.text);

                        },
                        treeParent: {
                            hover_node: false,
                            select_node: false
                        }
                    });

                $("#financeFund_tree")
                    .dropdownjstree({
                        source: JSON.parse(result.financeFundTree),
                        selectedNode: '',
                        selectNote: (node, selected) => { // sự kiện chọn
                            self.financeFundId(selected.node.original.id);
                            self.financeFundName(selected.node.original.text);
                            self.financeFundBankAccountNumber(selected.node.original.BankAccountNumber);
                            self.financeFundDepartment(selected.node.original.Department);
                            self.financeFundNameBank(selected.node.original.NameBank);
                            self.financeFundUserFullName(selected.node.original.UserFullName);
                            self.financeFundUserPhone(selected.node.original.UserPhone);
                            self.financeFundUserEmail(selected.node.original.UserEmail);
                        },
                        treeParent: {
                            hover_node: false,
                            select_node: false
                        }
                    });

                onCheck = true;
            }
            $('#treasure_tree').dropdownjstree('selectNode', 0);
            $('#financeFund_tree').dropdownjstree('selectNode', 0);
        });
    }

    //============================================================= Xử lý hàng tìm nguồn ======================================================

    self.viewOrderSourcingAddOrEdit = function (data) {
        $('#orderSourcingAddOrEdit').modal();
    }



    self.viewOrderSourcingAddOrEdit = function (data) {
        $('#orderSourcingAddOrEdit').modal();
    }

    //Modal phiếu báo giá, tìm nguồn
    self.viewStockQuotesDetail = function () {
        $('#stockQuotesDetailModal').modal();
    }

    //Modal Orders tìm nguồn
    self.viewOrderSourcingDetail = function () {
        $('#orderSourcingDetailModal').modal();
    }

    //Modal Orders ký gửi
    self.viewOrderDepositDetail = function (data) {

        self.isDetailRending(false);
        self.mapOrderDeposit(data);

        $.post("/Purchase/GetOrderDepositDetail", { orderId: data.Id }, function (result) {
            self.orderDeposit().ListDetail(result.listOrderDetail);
            self.orderDeposit().Warehouse([result.warehouse]);

            self.isDetailRending(true);
        });

        $('#orderDepositDetailModal').modal();
    }


    //================================================= HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG
    self.titleCustomer = ko.observable();
    self.complainModel = ko.observable(new complainModel());
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());

    self.active2 = ko.observable('customerDetail');
    self.templateId2 = ko.observable('customerDetail');

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
                language: 'en',
            });
    };
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

    //============================Phương thức gọi lấy danh sách khiếu nạị hỗ trợ===============
    self.listStatus = ko.observable([]);
    self.listStatusView = ko.observable([]);
    self.listComplainSystem = ko.observable([]);
    self.listComplainStatus = ko.observable([]);
    self.listSystemRenderPoCustomer = ko.observable([]);
    self.listAllCustomerComplain = ko.observable([]);
    self.complainModel = ko.observable(new complainModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());
    //self.order().StatusName(statusApp.order[data.Status].Name);
    //self.order().StatusClass(statusApp.order[data.Status].Class);

    self.customerEmail = ko.observable();
    self.customerPhone = ko.observable();
    self.customerAddress = ko.observable();
    self.customerLevel = ko.observable();
    self.titleTicket = ko.observable();
    self.complainuser = ko.observable([]);
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
        if (searchCustomerModal.SystemId == undefined) {
            searchCustomerModal.SystemId = -1;
        }
        $.post("/Purchase/GetAllTicketListByStaff", { page: page, pageSize: pagesize, searchModal: searchCustomerModal }, function (data) {
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

    //Thêm note nội bộ bên đặt hàng trên danh sách
    self.NoteCloseCommonOrder = function (data) {
        var isLook = false;
        $.post({
            url: "/Ticket/NoteCloseCommon",
            data: { complain: data },
            success: function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    isLook = false;
                } else {
                    isLook = true;
                    self.GetAllTicketListByStaff();
                }
            },
            async: false
        });

        return isLook;
    }

    //Hàm load lại dữ liệu trên các tab
    self.getRenderSystemTab = function () {
        self.listSystemRenderPoCustomer([]);
        self.listComplainStatus([]);
        self.listComplainSystem([]);
        $.post("/Purchase/GetRenderSystemTab", { active: self.active() }, function (data) {
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
        self.listUser([]);
        self.listUserDetail([]);
        self.totalSupport([]);
        self.listStatusRefund([]);
        $.post("/Purchase/GetAllSearchData", {}, function (data) {
            self.listComplainSystem(data.listComplainSystem);
            self.listComplainStatus(data.listComplainStatus);
            self.listUser(data.listUser);
            self.listUserDetail(data.listUserDetail);
            self.listStatusRefund(data.listStatusRefund);
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
    self.approvalUser = function (data) {
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

    self.claimForRefundNoteUpdate = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundNoteUpdate", { claimForRefund: self.claimForRefund() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                self.btnNotViewRefundMoneyModalContinute(self.claimForRefund());
                //toastr.success(response.msg);
            }
        });
    }

    //================================================Detail KHIẾU NẠI===============

    //=====================hỗ trợ khiếu nại==============

    // Object Detail khiếu nại
    self.mapComplainModel = function (data) {
        self.complainModel(new complainModel());

        self.complainModel().Id(data.Id);
        self.complainModel().Code(data.Code);
        self.complainModel().TypeOrder(data.TypeOrder);
        self.complainModel().TypeService(data.TypeService);
        self.complainModel().TypeServiceName(data.TypeServiceName);
        self.complainModel().TypeServiceClose(data.TypeServiceClose);
        self.complainModel().TypeServiceCloseName(data.TypeServiceCloseName);
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



    //================================================= /HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG

    //============================================================= Phân trang ==================================================
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

    //============================================================= khởi tạo ====================================================
    //Hàm khởi tạo khi load trang
    $(function () {
        self.init();
        //self.viewReport();
        self.initInputMark();
        self.getInit();
        //hỗ trợ khiếu nại
        self.GetTicketSearchData();

        //hàm check url
        var arr = _.split(window.location.href, "#ORD");
        if (arr.length > 1) {
            $.post("/Purchase/GetId", { code: arr[1], type: orderType.order }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.viewEditDetail(result.id);
                }
            });
        }

        var arr = _.split(window.location.href, "#DEP");
        if (arr.length > 1) {
            $.post("/Purchase/GetId", { code: arr[1], type: orderType.deposit }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    depositDetailViewModel.showModalDialog(result.id);
                }
            });
        }

        var arr = _.split(window.location.href, "#COM");
        if (arr.length > 1) {
            $.post("/Purchase/GetId", { code: arr[1], type: orderType.commerce }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.viewEditDetail(result.id);
                }
            });
        }

        var arrCode = _.split(window.location.href, "#");
        if (arrCode.length > 1) {
            var arrCheck = ['order-new', 'order', 'order-retail', 'order-delay', 'order-deposit-new', 'order-deposit', 'order-deposit-delay', 'stock-quotes-new', 'stock-quotes', 'order-sourcing', 'order-commerce', 'lading-code', 'user-website', 'order-risk', 'order-accountant', 'order-warehouse', 'order-success', 'customerfind', 'ticket-support', 'claimforrefund', 'report', 'reportBargain', 'reportOrder'];
            if (_.lastIndexOf(arrCheck, arrCode[1]) === -1) {
                var check = false;
                var arrO = _.split(window.location.href, "#ORD");
                if (arrO.length > 1) {
                    check = true;
                }
                var arrD = _.split(window.location.href, "#DEP");
                if (arrD.length > 1) {
                    check = true;
                }
                var arrC = _.split(window.location.href, "#COM");
                if (arrC.length > 1) {
                    check = true;
                }
                if (check === false) {
                    orderTypeViewModel.showEditOrderCode(arrCode[1]);
                }
            }
        }

        //hàm check url
        var arrClaim = _.split(window.location.href, "#CFRF");
        if (arrClaim.length > 1) {

            $.post("/Ticket/GetClaimForRefundCode", { code: arrClaim[1] }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                } else {
                    self.btnViewRefundMoneyModalContinute(result.claimForRefundModal);
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
        self.token($("#orderViewAddOrEditId input[name='__RequestVerificationToken']").val());

        self.CheckUrl();

        $(window).bind('hashchange', function () {
            self.CheckUrl();
        });

        self.autoSize();
        //Lay thoi gian: ngay, tuan, thang
        $('#commentEdit').bind('show', function () {
            self.contentEdit(ticketDetail.contentEdit());
        });

    });

    self.autoSize = function () {
        setTimeout(function () {
            $('.auto-size').css("height", "");
            $('.auto-size').autogrow({ onInitialize: true });
        }, 300);
    }

    self.CheckUrl = function () {
        var arr = _.split(window.location.href, "#");
        var arrCheck = ['order-new', 'order', 'order-retail', 'order-delay', 'order-deposit-new', 'order-deposit', 'order-deposit-delay', 'stock-quotes-new', 'stock-quotes', 'order-sourcing', 'order-commerce', 'lading-code', 'user-website', 'order-risk', 'order-accountant', 'order-warehouse', 'order-success', 'customerfind', 'ticket-support', 'claimforrefund', 'report', 'reportBargain', 'reportOrder'];
        if (arr.length > 1) {
            if (_.lastIndexOf(arrCheck, arr[1]) !== -1) {
                self.clickMenu(arr[1]);

                setTimeout(function () {
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
                                self.dateStart('');
                                self.dateEnd('');
                                $('#daterange-btn span').html('Created date');
                                self.SearchCustomerModal().DateStart('');
                                self.SearchCustomerModal().DateEnd('');
                            }
                            else {
                                self.dateStart(start.format());
                                self.dateEnd(end.format());
                                $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
                                self.SearchCustomerModal().DateStart(start.format());
                                self.SearchCustomerModal().DateEnd(end.format());
                            }

                        }
                    );
                    $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
                        //do something, like clearing an input
                        self.dateStart('');
                        self.dateEnd('');
                        $('#daterange-btn span').html('Created date');
                        self.SearchCustomerModal().DateStart('');
                        self.SearchCustomerModal().DateEnd('');
                    });

                }, 300);
            }
        }
    }

    //Hàm khởi tạo load dữ liệu
    self.getInit = function () {
        $.post("/Purchase/GetInit", function (data) {
            self.totalOrderNew(data.totalOrderNew);
            self.totalOrderWait(data.totalOrderWait);
            self.totalOrder(data.totalOrder);
            self.totalOrderLate(data.totalOrderLate);

            self.totalOrderDepositNew(data.totalOrderDepositNew);
            self.totalOrderDeposit(data.totalOrderDeposit);

            self.totalStockQuoesNew(data.totalStockQuoesNew);
            self.totalStockQuoes(data.totalStockQuoes);
            self.totalOrderSourcing(data.totalOrderSourcing);

            self.totalOrderRisk(data.totalOrderRisk);
            self.totalOrderAccountant(data.totalOrderAccountant);
            self.totalOrderNoWarehouse(data.totalOrderNoWarehouse);

            self.totalTicketSupport(data.totalTicketSupport);
            self.totalClaimForRefund(data.totalClaimForRefund);
        });
    };

    self.initInputMark = function () {
        $('#orderAddOrEditModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('#contractCodeModal input.decimal').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize.culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize.culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });

        $('#orderAddOrEditModal input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
        $('#contractCodeModal input.decimalCN').each(function () {
            if (!$(this).data()._inputmask) {
                $(this).inputmask("decimal", { radixPoint: Globalize("en-US").culture().numberFormat['.'], autoGroup: true, groupSeparator: Globalize("en-US").culture().numberFormat[','], digits: 2, digitsOptional: true, allowPlus: false, allowMinus: false });
            }
        });
    }

    self.initInputMarkCommon = function () {
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

    //Hàm khởi tạo dữ liệu, các jquery
    self.init = function () {
        self.isLoading(false);
        self.totalOrderNew(window.totalOrderNew);
        self.totalOrderWait(window.totalOrderWait);
        self.totalOrder(window.totalOrder);
        self.totalOrderLate(window.totalOrderLate);

        self.totalOrderDepositNew(window.totalOrderDepositNew);
        self.totalOrderDeposit(window.totalOrderDeposit);
        self.totalOrderDepositLate(window.totalOrderDepositLate);

        self.totalStockQuoesNew(window.totalStockQuoesNew);
        self.totalStockQuoes(window.totalStockQuoes);
        self.totalOrderSourcing(window.totalOrderSourcing);

        self.totalOrderRisk(window.totalOrderRisk);
        self.totalOrderAccountant(window.totalOrderAccountant);
        self.totalOrderNoWarehouse(window.totalOrderNoWarehouse);

        self.listStatus(window.listStastus);
        self.listSystem(window.listSystem);
        self.listWarehouse(window.listWarehouse);
        self.listWarehouseVN(window.listWarehouseVN);
        self.listUser(window.listUser);
        self.listReason(window.listReason);
        self.listReasonNoCodeOfLading(window.listReasonNoCodeOfLading);
        self.listReasonNotEnoughInventory(window.listReasonNotEnoughInventory);
        self.listBargainType(window.listBargainType);

        $.post("/Purchase/GetUserOffice", {}, function (result) {
            self.listUserOffice(result);
        });

        self.totalTicketSupport(window.totalTicketSupport);
        $('.nav-tabs').tabdrop();
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
                    self.dateStart('');
                    self.dateEnd('');
                    $('#daterange-btn span').html('Created date');
                    self.SearchCustomerModal().DateStart('');
                    self.SearchCustomerModal().DateEnd('');
                }
                else {
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    $('#daterange-btn span').html(moment(self.dateStart()).format('DD/MM/YYYY') + ' - ' + moment(self.dateEnd()).format('DD/MM/YYYY'));
                    self.SearchCustomerModal().DateStart(start.format());
                    self.SearchCustomerModal().DateEnd(end.format());
                }

            }
        );
        self.isLoading(true);

        //$(".select-view").select2();
    }

    self.codeOw = ko.observable();
    self.orderCodeWarehouse = function (id, code) {
        $.post("/Purchase/OrderCodeWarehouse", { idWarehouse: id, code: code }, function (result) {
            self.codeOw(result.code);
        });
    }

    //==================DANH SÁCH HOÀN TIỀN KHIẾU NẠI====================================
    self.listStatusRefund = ko.observableArray([]);
    self.listClaimForRefundData = ko.observableArray([]);
    self.listClaimForRefundDetail = ko.observableArray([]);
    self.claimForRefund = ko.observable(new ClaimForRefund);
    self.listClaimForRefund = ko.observableArray([]);
    self.totalClaimForRefund = ko.observable();
    self.listClaimForRefundView = ko.observableArray([]);
    self.TitleSearch = ko.observable("Search keyword:");
    self.titleTicket = ko.observable();
    self.avatar = ko.observable();
    self.levelName = ko.observable();
    self.listTicketType = ko.observableArray([]);

    self.SearchClaimForRefundModal = ko.observable({
        Keyword: ko.observable(""),
        CustomerId: ko.observable(-1),
        UserId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchClaimForRefundData = ko.observable(self.SearchClaimForRefundModal());

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
                    if (repo.code == undefined || repo.email == undefined) {
                        return repo.text;
                    } else {
                        return "(" + repo.code + ")" + " - " + repo.text + " - " + repo.email;
                    }
                },
                placeholder: "",
                allowClear: true,
                language: 'vi'
            });
    };
    self.stringName = function (data) {
        if (data == null) {
            data = '';
        }
        return data;
    }
    ///ClaimForRefund
    self.GetClaimForRefundList = function () {
        self.listTicketType(window.listcomplainType);
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
        $.post("/Purchase/GetClaimForRefundList", { page: page, pageSize: pagesize, searchModal: searchClaimForRefundData }, function (data) {
            total = data.totalRecord;
            var list = [];
            _.each(data.claimForRefundModal,
                function (item) {

                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }
                    item.CustomerCareFullName = _.find(data.listOrder, function (it) {
                        return it['Id'] == item.OrderId;
                    })['CustomerCareFullName'];

                    item.UserName = _.find(data.listComplain, function (it) {
                        return it['ComplainId'] == item.TicketId;
                    })['UserName'];


                    list.push(item);
                });

            self.listClaimForRefundView(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    //Hiển thị Detail khiếu nại
    self.totalClaim = ko.observable(0);
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
                self.initInputMarkCommon();
                $('#claimForRefundDetail').modal();
            }

        });
    }
    self.totalAllClaimForRefundDetail = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefundDetail(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed); });
        self.totalClaim(formatNumberic(total, 'N2'));
    }
    //Số tiền khác
    self.minusMoneyOther = function () {
        var MoneyOther = 0;
        if (self.claimForRefund().MoneyOther() != null) {
            MoneyOther = Globalize.parseFloat(self.claimForRefund().MoneyOther());
        }
        var MoneyRefund = Globalize.parseFloat(self.claimForRefund().MoneyRefund());
        self.claimForRefund().MoneyRefund(formatNumberic(Globalize.parseFloat(self.totalClaim()) * Globalize.parseFloat(self.order().ExchangeRate()) + MoneyOther, 'N2'));
        self.claimForRefundInfoUpdateAutomatic();

    };
    self.btnDeleteClaimForRefund = function (data) {
        $.post("/Ticket/DeleteClaimForRefund", { claimForRefundId: data.Id }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }
    self.TicketStatusUpdate = function (claimForRefund) {
        self.isDetailRending(false);
        $.post("/Ticket/TicketStatusUpdate", { claimForRefund: claimForRefund }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
            }
        });
    }



    self.btnViewRefundMoneyModalContinute = function (data) {
        self.isDetailRending(false);
        self.complainModel(new complainModel());
        self.vipOrder(0);
        self.vipShip(0);
        self.vipName("");
        $.post("/Ticket/ClaimForRefundUpdate", { claimForRefundId: data.Id }, function (result) {
            self.isDetailRending(true);
            if (!result.status) {
                toastr.error(result.msg);
            }
            else {
                self.listOrderService(result.orderService);
                if (result.orderDetail != null) {
                    self.mapObject(result.orderDetail);
                }
                if (result.levelVip != null) {
                    self.vipOrder(result.levelVip.Order);
                    self.vipShip(result.levelVip.Ship);
                    self.vipName(result.levelVip.Name);
                }
                _.each(result.claimForRefundViewModel.LstClaimForRefundDetails,
                    function (it) {
                        it.Price = formatNumberic(it.Price, 'N2');
                        it.TotalPrice = formatNumberic(it.TotalPrice, 'N2');
                        it.TotalExchange = formatNumberic(it.TotalExchange, 'N2');
                        it.cacheQuantityFailed = it.QuantityFailed;
                        it.TotalQuantityFailed = ko.observable(formatNumberic(it.TotalQuantityFailed, 'N2'));
                        it.QuantityFailed = ko.observable(it.QuantityFailed);
                        it.QuantityFailed.subscribe(function (newValue) {
                            if (it.cacheQuantityFailed !== newValue) {
                                newValue = Globalize.parseFloat(newValue);
                                //tính toán
                                if (newValue <= it.Quantity) {
                                    it.TotalQuantityFailed(formatNumberic((newValue * Globalize.parseFloat(it.Price)), 'N2'));
                                    self.totalAllClaimForRefund();
                                    it.cacheQuantityFailed = newValue;
                                }
                                else {
                                    it.QuantityFailed(0);
                                    //newValue = it.QuantityFailed;
                                    toastr.error('Number of input errors has exceeded the number of orders!');
                                }

                            }
                        });
                    });

                self.mapClaimForRefund(result.claimForRefundViewModel.ClaimForRefund);

                if (self.claimForRefund().MoneyOrderRefund() == null) self.claimForRefund().MoneyOrderRefund(0);
                if (self.claimForRefund().MoneyOrderRefundDicker() == null) self.claimForRefund().MoneyOrderRefundDicker(0);
                if (self.claimForRefund().SupporterMoneyRequest() == null) self.claimForRefund().SupporterMoneyRequest(0);
                if (self.claimForRefund().CurrencyDiscount() == null) self.claimForRefund().CurrencyDiscount(0);
                if (self.claimForRefund().RealTotalRefund() == null) self.claimForRefund().RealTotalRefund(0);
                if (self.claimForRefund().MoneyRefund() == null) self.claimForRefund().MoneyRefund(0);

                self.listClaimForRefundData(result.claimForRefundViewModel.LstClaimForRefundDetails);
                self.totalAllClaimForRefund();
                self.mapComplainModel(result.ticket);
                self.initInputMarkCommon();
                $('#refundMoneyModal').modal();
            }

        });


    }

    //Cap nhat nhung khong load popup
    self.btnNotViewRefundMoneyModalContinute = function (data) {
        self.totalClaim(0);
        self.complainModel(new complainModel());
        self.isDetailRending(false);
        self.vipOrder(0);
        self.vipShip(0);
        self.vipName("");
        $.post("/Ticket/ClaimForRefundUpdate", { claimForRefundId: data.Id }, function (result) {
            self.isDetailRending(true);
            self.listOrderService(result.orderService);
            if (!result.status) {
                toastr.error(result.msg);
            }
            else {
                if (result.orderDetail != null) {
                    self.mapObject(result.orderDetail);
                }
                if (result.levelVip != null) {
                    self.vipOrder(result.levelVip.Order);
                    self.vipShip(result.levelVip.Ship);
                    self.vipName(result.levelVip.Name);
                }

                _.each(result.claimForRefundViewModel.LstClaimForRefundDetails,
                    function (it) {
                        it.Price = formatNumberic(it.Price, 'N2');
                        it.TotalPrice = formatNumberic(it.TotalPrice, 'N2');
                        it.TotalExchange = formatNumberic(it.TotalExchange, 'N2');
                        it.cacheQuantityFailed = it.QuantityFailed;
                        it.TotalQuantityFailed = ko.observable(formatNumberic(it.TotalQuantityFailed, 'N2'));
                        it.QuantityFailed = ko.observable(it.QuantityFailed);
                        it.QuantityFailed.subscribe(function (newValue) {
                            if (it.cacheQuantityFailed !== newValue) {
                                newValue = Globalize.parseFloat(newValue);
                                //tính toán
                                if (newValue <= it.Quantity) {
                                    it.TotalQuantityFailed(formatNumberic((newValue * Globalize.parseFloat(it.Price)), 'N2'));
                                    self.totalAllClaimForRefund();
                                    it.cacheQuantityFailed = newValue;
                                }
                                else {
                                    it.QuantityFailed(0);
                                    //newValue = it.QuantityFailed;
                                    toastr.error('Number of input errors has exceeded the number of orders!');
                                }

                            }
                        });
                    });

                self.mapClaimForRefund(result.claimForRefundViewModel.ClaimForRefund);

                if (self.claimForRefund().MoneyOrderRefund() == null) self.claimForRefund().MoneyOrderRefund(0);
                if (self.claimForRefund().MoneyOrderRefundDicker() == null) self.claimForRefund().MoneyOrderRefundDicker(0);
                if (self.claimForRefund().SupporterMoneyRequest() == null) self.claimForRefund().SupporterMoneyRequest(0);
                if (self.claimForRefund().CurrencyDiscount() == null) self.claimForRefund().CurrencyDiscount(0);
                if (self.claimForRefund().RealTotalRefund() == null) self.claimForRefund().RealTotalRefund(0);
                if (self.claimForRefund().MoneyRefund() == null) self.claimForRefund().MoneyRefund(0);
                if (self.claimForRefund().MoneyOther() == null) self.claimForRefund().MoneyOther(0);

                self.listClaimForRefundData(result.claimForRefundViewModel.LstClaimForRefundDetails);
                self.totalAllClaimForRefund();
                self.mapComplainModel(result.ticket);
                self.initInputMark();
            }

        });


    }
    //tính Sum
    self.totalAllClaimForRefund = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefundData(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed()); });
        self.totalClaim(formatNumberic(total, 'N2'));
        self.claimForRefund().MoneyRefund(formatNumberic((total * Globalize.parseFloat(self.order().ExchangeRate()) + self.claimForRefund().MoneyOther()), 'N2'));
    }

    // Cập nhật số tiền đòi shop
    self.claimForRefundInfoUpdateAutomatic = function () {
        if (self.complainModel().TypeServiceClose == 0) {
            toastr.error("You have not selected the type of grievance!");
        }
        else {
            $.post("/Ticket/ClaimForRefundInfoUpdate", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData(), complainFund: self.complainModel() }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    self.btnNotViewRefundMoneyModalContinute(self.claimForRefund());
                    self.GetClaimForRefundList();
                }
            });
        }
    }
    self.updateMoneyOrderRefund = function () {
        self.claimForRefundInfoUpdateAutomatic();
    };
    self.minusClaimForRefundOld = function () {
    };

    //Cập nhật thông tin Refund
    self.btnClaimForRefundInfoUpdate = function () {
        self.claimForRefundInfoUpdate();
    }
    self.claimForRefundInfoUpdate = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundInfoUpdate", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }

    //Chuyển sang cho CSKH xử lý tiếp 
    self.btnClaimForRefundForwardCareCustomer = function () {
        self.claimForRefundForwardCareCustomer();
    }
    self.claimForRefundForwardCareCustomer = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundForwardCareCustomer", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }

    //Chuyển sang cho giám đốc, trưởng phòng CSKH phê duyệt
    self.btnClaimForRefundForwardBoss = function () {
        self.claimForRefundForwardBoss();
    }
    self.claimForRefundForwardBoss = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundForwardBoss", { claimForRefund: self.claimForRefund() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }

    //Chuyển sang kế toán xử lý tiếp
    self.btnClaimForRefundForwardAccountant = function () {
        self.claimForRefundForwardAccountant();
    }
    self.claimForRefundForwardAccountant = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundForwardAccountant", { claimForRefund: self.claimForRefund() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
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
        self.claimForRefund().ApproverId(data.ApproverId);
        self.claimForRefund().ApproverName(data.ApproverName);
        self.claimForRefund().MoneyOther(data.MoneyOther);
    }
    //============================================================= Các hàm hỗ trợ ==============================================

    //============================================================= Báo cáo ====================================================

    self.viewReportCommon = function (detailName, detailOrder, detailPrice, idName) {
        self.isLoading(true);
        $(idName).highcharts({
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
                categories: detailName,
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
                    text: 'Total revene',
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
                name: 'Order',
                type: 'column',
                yAxis: 1,
                data: detailOrder,
                tooltip: {
                    valueSuffix: ' order'
                }

            }, {
                name: 'Total money bargain',
                type: 'spline',
                data: detailPrice,
                tooltip: {
                    valueSuffix: ' CNY'
                }
            }]
        });
    }

    self.viewReportBargainDayCommon = function (detailName, detailOrder, detailPrice, idName) {
        self.isLoading(true);
        $(idName).highcharts({
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
                categories: detailName,
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
                    text: 'Total revene',
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
                name: 'Order',
                type: 'column',
                yAxis: 1,
                data: detailOrder,
                tooltip: {
                    valueSuffix: ' order'
                }

            }, {
                name: 'Total order amount',
                type: 'spline',
                data: detailPrice,
                tooltip: {
                    valueSuffix: ' CNY'
                }
            }]
        });
    }

    //==========================Thong ke doanh thu ky gui=================================
    self.viewReportDepositDay = function () {
        var start = self.dateStart();
        $.post("/Purchase/GetTotalDepositBargainReportDay", { startDay: start }, function (data) {
            var detailNameordered = [];
            var detailUserordered = [];
            var detailPriceordered = [];

            var orderGroup = _.groupBy(data.ordered, "UserId");

            _.each(data.listUser,
                function (item) {

                    item.countorder = orderGroup[item.Id + ""] ? orderGroup[item.Id + ""].length : 0;

                    item.sumorder = orderGroup[item.Id + ""] ? _.sumBy(orderGroup[item.Id + ""], "Total") : 0;
                    if (item.sumorder == undefined) {
                        item.sumorder = 0;
                    }
                    detailNameordered.push(item.FullName);
                    detailUserordered.push(item.countorder);
                    detailPriceordered.push(item.sumorder);
                });
            self.viewReportBargainDayCommon(detailNameordered, detailUserordered, detailPriceordered, '#userDepositDay');
        });
    }

    self.viewReportDepositAllDay = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        $.post("/Purchase/GetTotalDepositBargainReportAllDay", { startDay: start, endDay: end }, function (data) {
            var detailNameordered = [];
            var detailUserordered = [];
            var detailPriceordered = [];

            var orderGroup = _.groupBy(data.ordered, "UserId");

            _.each(data.listUser,
                function (item) {

                    item.countorder = orderGroup[item.Id + ""] ? orderGroup[item.Id + ""].length : 0;

                    item.sumorder = orderGroup[item.Id + ""] ? _.sumBy(orderGroup[item.Id + ""], "Total") : 0;
                    if (item.sumorder == undefined) {
                        item.sumorder = 0;
                    }
                    detailNameordered.push(item.FullName);
                    detailUserordered.push(item.countorder);
                    detailPriceordered.push(item.sumorder);
                });
            self.viewReportBargainDayCommon(detailNameordered, detailUserordered, detailPriceordered, '#userDepositAllDay');
        });
    }

    // ==============================Thong ke tien mac ca don hang=================================
    self.viewReportBargainDay = function () {
        var start = self.dateStart();
        $.post("/Purchase/GetTotalPriceBargainReportDay", { startDay: start }, function (data) {
            self.viewReportBargainDayCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userBargainDay');
        });
    }
    self.viewReportBargainAllDay = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        $.post("/Purchase/GetTotalPriceBargainReportAllDay", { startDay: start, endDay: end }, function (data) {
            self.viewReportBargainDayCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userBargainAllDay');
        });
    }
    self.viewReportBargain = function () {
        self.viewReportBargainDay();
        self.viewReportBargainAllDay();

        $('#daterange-btnBargainday').daterangepicker(
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
                    $('#daterange-btnBargainday span').html('Created date');
                    self.dateStart('');
                    self.viewReportBargainDay();
                }
                else {
                    $('#daterange-btnBargainday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.viewReportBargainDay();
                }
            }
        );
        $('#daterange-btnBargainday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnBargainday span').html('Create date');
            self.dateStart('');
        });

        $('#daterange-btnBargainAllday').daterangepicker(
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
                    $('#daterange-btnBargainAllday span').html('Created date');
                    self.dateStart('');
                    self.dateEnd('');
                    self.viewReportBargainAllDay();
                }
                else {
                    $('#daterange-btnBargainAllday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    self.viewReportBargainAllDay();
                }
            }
        );
        $('#daterange-btnBargainAllday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnBargainAllday span').html('Create date');
            self.dateStart("");
            self.dateEnd("");
        });
    }

    self.viewReportDeposit = function () {
        self.viewReportDepositDay();
        self.viewReportDepositAllDay();

        $('#daterange-btnDepositday').daterangepicker(
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
                    $('#daterange-btnDepositday span').html('Created date');
                    self.dateStart('');
                    self.viewReportDepositDay();
                }
                else {
                    $('#daterange-btnDepositday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.viewReportDepositDay();
                }
            }
        );
        $('#daterange-btnDepositday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnDepositday span').html('Create date');
            self.dateStart('');
        });

        $('#daterange-btnDepositAllday').daterangepicker(
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
                    $('#daterange-btnDepositAllday span').html('Created date');
                    self.dateStart('');
                    self.dateEnd('');
                    self.viewReportDepositAllDay();
                }
                else {
                    $('#daterange-btnDepositAllday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    self.viewReportDepositAllDay();
                }
            }
        );
        $('#daterange-btnDepositAllday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnDepositAllday span').html('Create date');
            self.dateStart("");
            self.dateEnd("");
        });
    }

    //=====================================Thong ke tong tien don hang=====================================
    self.viewReportOrderedDay = function () {
        var start = self.dateStart();
        $.post("/Purchase/GetTotalMoneyReportDay", { startDay: start, type: false }, function (data) {
            self.viewReportCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userOrderedDay');
        });
    }

    self.viewReportFinishedDay = function () {
        var start = self.dateStart();
        $.post("/Purchase/GetTotalMoneyReportDay", { startDay: start, type: true }, function (data) {
            self.viewReportCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userFinishedDay');
        });
    }

    self.viewReportOrderedAllDay = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        $.post("/Purchase/GetTotalMoneyReportAllDay", { startDay: start, endDay: end, type: false }, function (data) {
            self.viewReportCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userOrderedAllDay');
        });
    }

    self.viewReportFinishedAllDay = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        $.post("/Purchase/GetTotalMoneyReportAllDay", { startDay: start, endDay: end, type: true }, function (data) {
            self.viewReportCommon(data.detailNameordered, data.detailUserordered, data.detailPriceordered, '#userFinishedAllDay');
        });
    }
    self.viewReport = function () {
        self.viewReportOrderedDay();
        self.viewReportFinishedDay();
        self.viewReportOrderedAllDay();
        self.viewReportFinishedAllDay();

        $('#daterange-btnOrderedday').daterangepicker(
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
                    $('#daterange-btnOrderedday span').html('Created date');
                    self.dateStart('');
                    self.viewReportOrderedDay();
                }
                else {
                    $('#daterange-btnOrderedday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.viewReportOrderedDay();
                }
            }
        );
        $('#daterange-btnOrderedday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnOrderedday span').html('Create date');
            self.dateStart('');
        });

        $('#daterange-btnOrderedAllday').daterangepicker(
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
                    $('#daterange-btnOrderedAllday span').html('Created date');
                    self.dateStart('');
                    self.dateEnd('');
                    self.viewReportOrderedAllDay();
                }
                else {
                    $('#daterange-btnOrderedAllday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    self.viewReportOrderedAllDay();
                }
            }
        );
        $('#daterange-btnOrderedAllday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnOrderedAllday span').html('Create date');
            self.dateStart("");
            self.dateEnd("");
        });

        $('#daterange-btnFinishedAllday').daterangepicker(
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
                    $('#daterange-btnFinishedAllday span').html('Created date');
                    self.dateStart('');
                    self.dateEnd('');
                    self.viewReportFinishedAllDay();
                }
                else {
                    $('#daterange-btnFinishedAllday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.SearchCustomerModal().DateStart(start.format());
                    self.dateStart(start.format());
                    self.dateEnd(end.format());
                    self.viewReportFinishedAllDay();
                }
            }
        );
        $('#daterange-btnFinishedAllday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnFinishedAllday span').html('Create date');
            self.dateStart("");
            self.dateEnd("");
        });
        $('#daterange-btnFinishedday').daterangepicker(
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
                    $('#daterange-btnFinishedday span').html('Created date');
                    self.dateStart('');
                    self.viewReportFinishedDay();
                }
                else {
                    $('#daterange-btnFinishedday span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                    self.dateStart(start.format());
                    self.viewReportFinishedDay();
                }
            }
        );
        $('#daterange-btnFinishedday').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnFinishedday span').html('Create date');
            self.dateStart('');
        });
    }
    //=============Thông ke loi nhuan==================

    self.viewReportProfit = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.post("/Purchase/GetTotalProfitReportAllDay", { startDay: start, endDay: end }, function (data) {

            var list = _.filter(data.day,
                function (item) {
                    return moment(item).format('DD/MM/YYYY');
                });

            self.isLoading(true);
            $("#userProfitOrder").highcharts({
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
                    categories: list,
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
                        text: 'Total revene',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Orders',
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
                    name: 'Orders',
                    type: 'column',
                    yAxis: 1,
                    data: data.totalOrder,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revene',
                    type: 'spline',
                    data: data.totalMoney,
                    tooltip: {
                        valueSuffix: ' CNY'
                    }
                }
                    , {
                    name: 'Sum profit',
                    type: 'spline',
                    data: data.totalBargain,
                    tooltip: {
                        valueSuffix: ' CNY'
                    }
                }]
            });
        });
    }

    self.viewReportBargainSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.post("/Purchase/GetTotalProfitBargainReportAllDay", { startDay: start, endDay: end }, function (data) {
            self.isLoading(true);

            $("#userProfitOrderFinished").highcharts({
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
                        format: '{value} CNY',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Total revene',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Orders',
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
                    name: 'bargain amount',
                    type: 'column',
                    yAxis: 1,
                    data: data.totalBargain,
                    tooltip: {
                        valueSuffix: ' CNY'
                    }

                }]
            });
        });
    }
    self.viewReportProfitOrder = function () {
        self.viewReportProfit();
        self.viewReportBargainSituation();
    }

    //============================================================= XUẤT BÁO CÁO EXCEL ==========================================

    //Thong ke Total money (NDT)  mac cả với Shop trong ngày(Đơn đặt hàng thành công)
    self.ExcelMoneyReportOrderedday = function () {
        var start = self.dateStart();
        var title = "total of money (CNY) bargain was with Shop of the day (Successful Orders)";
        $.redirect("/Purchase/ExcelGetTotalMoneyReportDay", { titleExcel: title, startDay: start, all: false, status: false }, "POST");
    }

    //Thong ke Total money (NDT)  mac cả với Shop trong ngày(Hoàn thành)
    self.ExcelMoneyReportFinishedday = function () {
        var start = self.dateStart();
        var title = "Total of money (CNY) bargain was with Shop of the day (Complete)";

        $.redirect("/Purchase/ExcelGetTotalMoneyReportDay", { titleExcel: title, startDay: start, all: false, status: true }, "POST");
    }

    //Thong ke Total money (NDT)  mac cả với Shop trong tất cả ngày(Đơn đặt hàng thành công)
    self.ExcelMoneyReportOrderedAllday = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        var title = "Total of money (CNY) bargain was with Shop in all days (Successful Orders)";

        $.redirect("/Purchase/ExcelGetTotalMoneyReportAllDay", { titleExcel: title, startDay: start, endDay: end, all: true, status: false }, "POST");
    }

    //Thong ke Total money (NDT)  mac cả với Shop trong tất cả các ngày(Hoàn thành)
    self.ExcelMoneyReportFinishedAllday = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        var title = "Total money (CNY) bargain was with Shop in all days (Complete)";

        $.redirect("/Purchase/ExcelGetTotalMoneyReportAllDay", { titleExcel: title, startDay: start, endDay: end, all: true, status: true }, "POST");
    }

    //Thong ke Total money (NDT) thanh toán với Shop trong ngày
    self.ExcelMoneyReportBargainday = function () {
        var start = self.dateStart();
        var title = "Total of money (CNY) paid with Shop of the day";

        $.redirect("/Purchase/ExcelGetTotalPriceBargainReportDay", { titleExcel: title, startDay: start, all: false }, "POST");
    }

    //Thong ke tinh hinh Total money (NDT) thanh toán với Shop
    self.ExcelMoneyReportBargainAllday = function () {
        var start = self.dateStart();
        var end = self.dateEnd();
        var title = "Total of money (CNY) paid with Shop for about days";

        $.redirect("/Purchase/ExcelGetTotalPriceBargainReportAllDay", { titleExcel: title, startDay: start, endDay: end, all: true }, "POST");
    }

    //Thong ke loi nhuan don hang
    self.ExcelReportProfitOrder = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        var title = "Total of money (CNY) bargain with Shop of the day"

        $.redirect("/Purchase/ExcelReportProfitOrder", { titleExcel: title, startDay: start, all: false }, "POST");
    }

    //Thong ke Total money (NDT) mac ca với Shop trong tất cả cac ngày
    self.ExcelReportProfitOrderFinished = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        var title = "Lợi nhuận bargain theo thời gian"

        $.redirect("/Purchase/ExcelReportProfitOrderFinished", { titleExcel: title, startDay: start, endDay: end, all: true }, "POST");
    }

    // Báo cáo trong phần báo cáo
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

    self.reportTitle = ko.observable("daily report " + moment().format('DD/MM/YYYY'));
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
                language: 'vi'
            });
        } else if (self.selectDateReport() == 'week') {
            $('.report-date').datepicker({
                autoclose: true,
                language: 'vi'
            });
        } else {
            $('.report-date').datepicker({
                autoclose: true,
                minViewMode: 1,
                language: 'vi'
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

        self.viewReportProfit();
        self.viewReportBargainSituation();

    }

    //============================================================== Export Excel =================================================
    self.exportExcelOrder = function () {
        $.redirect("/Order/ExportExcelOrder",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd()
            },
            "POST");
    }

    self.exportExcelOrderDelay = function () {
        $.redirect("/Order/ExportExcelOrderDelay",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd()
            },
            "POST");
    }

    self.exportExcelOrderNew = function () {
        $.redirect("/Order/ExportExcelOrderNew",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                //userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd()
            },
            "POST");
    }

    self.exportExcelOrderAwaitingPayment = function () {
        $.redirect("/Order/ExportExcelOrderAwaitingPayment",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd()
            },
            "POST");
    }

    self.exportExcelOrderNoCodeOfLading = function () {
        $.redirect("/Order/ExportExcelOrderNoCodeOfLading",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd(),
                isAllNoCodeOfLading: self.isAllNoCodeOfLading()
            },
            "POST");
    }

    self.exportExcelOrderNotEnoughInventory = function () {
        $.redirect("/Order/ExportExcelOrderNotEnoughInventory",
            {
                keyword: self.keyword(),
                status: self.status() == undefined ? -1 : self.status(),
                systemId: self.systemId(),
                userId: self.userId(),
                customerId: self.customerId(),
                dateStart: self.dateStart(),
                dateEnd: self.dateEnd(),
                isAllNotEnoughInventory: self.isAllNotEnoughInventory()
            },
            "POST");
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
};

var chatViewModel = new ChatViewModel();
ko.applyBindings(chatViewModel, $("#chatModal")[0]);

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var orderAddViewModel = new OrderAddViewModel();
ko.applyBindings(orderAddViewModel, $("#orderAddModal")[0]);

var depositAddOrEditViewModel = new DepositAddOrEditViewModel();
depositAddOrEditViewModel.viewBoxChat = new ChatViewModel();
ko.applyBindings(depositAddOrEditViewModel, $("#orderDepositAddOrEdit")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var stockQuotesAddOrEditViewModel = new StockQuotesAddOrEditViewModel();
stockQuotesAddOrEditViewModel.viewBoxChat = new ChatViewModel();
ko.applyBindings(stockQuotesAddOrEditViewModel, $("#stockQuotesAddOrEdit")[0]);

var stockQuotesViewModel = new StockQuotesViewModel();
ko.applyBindings(stockQuotesViewModel, $("#stockQuotesView")[0]);

var ticketDetailViewModel = new TicketDetailViewModel();
ko.applyBindings(ticketDetailViewModel, $("#ticketDetailModal")[0]);

var accountantDetail = new CustomerLookUp();
ko.applyBindings(accountantDetail, $("#rechargeDetailModal")[0]);

var orderCommerce = new OrderCommerceViewModel();
orderCommerce.viewBoxChat = new ChatViewModel();
ko.applyBindings(orderCommerce, $("#orderCommerceModal")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var viewModel = new OrderViewModel(chatViewModel, orderDetailViewModel, stockQuotesAddOrEditViewModel, depositAddOrEditViewModel, depositDetailViewModel, stockQuotesViewModel, ticketDetailViewModel, orderAddViewModel, accountantDetail, orderCommerce, orderCommerceDetailViewModel);

viewModel.viewBoxChat = new ChatViewModel();
ko.applyBindings(viewModel, $("#orderView")[0]);