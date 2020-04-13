var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

var supporterViewModel = function (orderDetailViewModel, depositDetailViewModel, accountantDetail) {
    var self = this;

    //#region khai báo chung nhất isLoading,isDetailRending  

    //========== Các biến cho loading
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);
    self.active = ko.observable('SupportReport');
    self.templateId = ko.observable('SupportReport');
    self.totalTicketList = ko.observable();
    self.MoneyOther = ko.observable(0);

    //Hàm khởi tạo phân trang 
    self.pageStart = ko.observable(false);
    self.pageEnd = ko.observable(false);
    self.pageNext = ko.observable(false);
    self.pagePrev = ko.observable(false);
    self.pageTitle = ko.observable("");
    self.dataId = ko.observable("");
    self.listPage = ko.observableArray([]);

    //Khai báo biến Sum trong thống kê
    self.totalOrderWait = ko.observable(0);
    self.totalTicket = ko.observable(0);
    self.listComplainOrder = ko.observableArray([]);

    // Check don hang co CSKH hay chua
    self.checkCustomerCareId = ko.observable();
    self.objectTicket = ko.observable();
    self.customerCareOrder = ko.observable();
    self.customerCareOrderId = ko.observable();
    self.customerCareTicket = ko.observable();
    self.orderCodeTicket = ko.observable();


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

    //=== Khai bao cac list search data
    //SHOW RA TU KHOA TIM KIEM
    self.TitleSearch = ko.observable("Search keyword:");

    self.clickMenu = function (name) {
        self.SearchTicketModal().DateStart('');
        self.SearchTicketModal().DateEnd('');
        self.contentChart(1);
        page = 1;

        if (name === 'order-wait' || name === 'order-wait-new' || name === 'order-cus') {
            self.orderWaitViewModel.listStatus([]);
        };

        self.active(name);
        self.templateId(name);
        var checkArr = _.split(window.location.href, "#SHOW");
        if (checkArr.length == 1) {
            window.history.pushState('Ticket', '', 'Ticket#' + name);
        }

        if (name === 'reportOrderWaitSituation') {
            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
            self.viewReportOrderWaitSituation();

            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });

        };

        if (name === 'SupportReport') {
            self.viewReport();
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                if (self.reportDate() === 'Invalid date') {
                    $('#reportCalendarTicket span').html('Statistics day');
                }
                else {
                    $('#reportCalendarTicket span').html(self.reportDate().format('DD/MM/YYYY'));
                }
                self.viewReport();
            });

            $('#reportCalendarTicket span').html('Statistics day');
        };

        if (name === 'reportTicketSituation') {
            self.reportDateStart(self.reportDate().startOf('day').format());
            self.reportDateEnd(self.reportDate().endOf('day').format());
            self.viewReportTicketSituation();
            $('.report-date').datepicker({
                autoclose: true,
                language: 'en',
            }).on('changeDate', function (selected) {
                startDate = new Date(selected.date.valueOf());
                self.reportDate(moment(startDate));
                self.reportMode();
            });
        };

        if (name === 'ticket') {
            self.renderSystem();
            self.searchCustomer();
            self.resetSearchTicketModal();
            self.GetAllTicketList();
        };
        if (name === 'support') {
            self.renderSystem();
            self.searchCustomer();
            self.searchUser();
            self.resetSearchTicketModal();
            self.GetAllTicketSupportList();
        };
        if (name === 'assign') {
            self.renderSystem();
            self.resetSearchTicketModal();
            self.GetAllTicketAssignList();
        };
        if (name === 'complain') {
            self.renderSystem();
            self.searchCustomer();
            self.searchUser();
            self.resetSearchTicketModal();
            self.GetAllTicketComplainList();
        };
        if (name === 'last') {
            self.renderSystem();
            self.searchCustomer();
            self.searchUser();
            self.resetSearchTicketModal();
            self.GetAllTicketLastList();
        };
        if (name === 'claimforrefund') {
            //self.SearchClaimForRefundModal().CustomerId = -1;
            self.resetSearchClaimForRefundModal();
            self.searchCustomerClaimForRefund();
            //self.searchCustomer();
            self.renderSystem();
            self.GetClaimForRefundList();
        };

        if (name === 'customerfind') {
            self.searchCustomer();
        };

        if (name === 'order-wait') {
            self.getInit();
            self.orderWaitViewModel.renderSystem(name);
        };

        if (name === 'order-wait-new') {
            self.getInit();
            self.orderWaitViewModel.renderSystem(name);
        }

        if (name === 'order-cus') {
            self.getInit();
            self.orderWaitViewModel.renderSystem(name);
        };

        self.init();
        setTimeout(function () {
            window.DoubleScroll(document.getElementById('doublescroll'));
        },
            1000);
    }

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
                language: 'en'
            });
    };



    //Khai báo khởi tạo
    self.number = ko.observableArray();
    self.number = 0;
    self.listStatus = ko.observableArray([]);
    self.listStatusRefund = ko.observableArray([]);
    self.listUser = ko.observableArray([]);
    self.listUserDetail = ko.observableArray([]);
    //He thong
    self.listSystem = ko.observableArray([]);
    self.systemId = ko.observable();
    self.systemName = ko.observable();
    self.titleTicket = ko.observable();

    //Bien Search Don hang trong tao Khieu nai
    self.orderCodeCustomer = ko.observable();

    self.claimForRefundDetailModel = ko.observable(new ClaimForRefundDetailModel());
    self.listClaimForRefundData = ko.observableArray([]);
    self.listClaimForRefundDetail = ko.observableArray([]);

    // Khai báo biến thông tin Orders trong xử lý Refund
    self.listOrderService = ko.observableArray([]);
    //Biến nội dung khiếu nại
    self.contentTicket = ko.observable();


    // Tìm kiếm nhân viên chăm sóc khách hàng
    self.GetUser = function () {
        self.listUserDetail([]);
        $.post("/Ticket/GetAllUser", {}, function (data) {
            self.listUserDetail(data);
        });
    }


    //Lay du lieu do vao tim kiem ticket
    self.GetTicketSearchData = function () {
        self.listSystem([]);
        self.listStatus([]);
        self.listUser([]);
        self.listStatusRefund([]);
        $.post("/Ticket/GetAllSearchData", {}, function (data) {
            self.listSystem(data.listSystem);
            self.listStatus(data.listStatus);
            self.listStatusRefund(data.listStatusRefund);
            self.listUser(data.listUser);
        });
    }

    //================================POST LẤY DANH SÁCH DỮ LIỆU KHIẾU NẠI ĐỂ HIỂN THỊ========
    //khai bao 1 mang danh sach ticket
    //========== Khai báo ListData đổ dữ liệu danh sách
    self.listAllTicket = ko.observableArray([]);
    self.listAllTicketComplain = ko.observableArray([]);
    self.listAllTicketLast = ko.observableArray([]);
    self.listAllTicketAssign = ko.observableArray([]);
    self.listAllTicketSupport = ko.observableArray([]);
    self.listClaimForRefund = ko.observableArray([]);
    self.listClaimForRefundView = ko.observableArray([]);


    //KHOI TYAO BIEN SEARCH 
    self.dateStart = ko.observable();
    self.dateEnd = ko.observable();
    // Search Object 
    self.SearchTicketModal = ko.observable({
        Keyword: ko.observable(""),
        SystemId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchTicketModal = ko.observable(self.SearchTicketModal());

    // Ham reset doi tuong tim kiem khieu nai
    self.resetSearchTicketModal = function () {
        self.SearchTicketModal().Keyword("");
        self.SearchTicketModal().SystemId(-1);
        self.SearchTicketModal().Status(-1);
        self.SearchTicketModal().DateStart("");
        self.SearchTicketModal().DateEnd("");
        self.customerId(-1);
        self.userId(-1);
    }

    self.SearchClaimForRefundModal = ko.observable({
        Keyword: ko.observable(""),
        CustomerId: ko.observable(-1),
        UserId: ko.observable(-1),
        Status: ko.observable(-1),
        DateStart: ko.observable(""),
        DateEnd: ko.observable("")
    });
    self.SearchClaimForRefundData = ko.observable(self.SearchClaimForRefundModal());

    // Ham reset doi tuong tim kiem hoan tien
    self.resetSearchClaimForRefundModal = function () {
        self.SearchClaimForRefundModal().Keyword("");
        self.SearchClaimForRefundModal().CustomerId(-1);
        self.SearchClaimForRefundModal().UserId(-1);
        self.SearchClaimForRefundModal().Status(-1);
        self.SearchClaimForRefundModal().DateStart("");
        self.SearchClaimForRefundModal().DateEnd("");
    }

    //lay ra danh sach list ticket tôi cần xử lý 
    self.stringName = function (data) {
        if (data == null) {
            data = '';
        }
        return data;
    }
    self.viewContent = function (data) {
        self.contentTicket(data);
        if (self.active() == 'ticket') {
            $('#fixTicketDetail').show();
        }
        else if (self.active() == 'complain') {
            $('#fixComplainDetail').show();
        }
        else if (self.active() == 'last') {
            $('#fixTicketLastDetail').show();
        }
        else if (self.active() == 'assign') {
            $('#fixTicketAssignDetail').show();
        }
        else {
            $('#fixTicketSupportDetail').show();
        }

    }

    self.hideDiv = function () {
        self.contentTicket('');
        if (self.active() == 'ticket') {
            $('#fixTicketDetail').hide();
        }
        else if (self.active() == 'complain') {
            $('#fixComplainDetail').hide();
        }
        else if (self.active() == 'last') {
            $('#fixTicketLastDetail').hide();
        }
        else if (self.active() == 'assign') {
            $('#fixTicketAssignDetail').hide();
        }
        else {
            $('#fixTicketSupportDetail').hide();
        }

    }
    self.GetAllTicketList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Need to handle");

        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        if (SearchTicketModal.SystemId === undefined) {
            SearchTicketModal.SystemId = -1;
        }

        $.post("/Ticket/GetAllTicketList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId() }, function (data) {
            self.listAllTicket([]);
            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {

                    item.ContentInternal = ko.observable(item.ContentInternal);
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.ContentInternal.subscribe(function (newValue) {
                        self.NoteCloseCommon(ko.mapping.toJS(item));
                    });

                    item.CustomerCareFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['CustomerCareFullName'];

                    item.OrderFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['UserFullName'];

                    item.CustomerEmail = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Email'];

                    item.CustomerPhone = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Phone'];
                    item.CustomerWarehouseName = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['WarehouseName'];
                    item.ClaimId = 0;
                    item.ClaimCode = "";
                    if (data.listClaimId.length !== 0) {
                        item.ClaimId = _.find(data.listClaimId, function (it) {
                            return it['TicketId'] == item.Id;
                        });
                        if (item.ClaimId != undefined) {
                            item.ClaimId = _.find(data.listClaimId, function (it) {
                                return it['TicketId'] == item.Id;
                            })['Id'];
                            item.ClaimCode = _.find(data.listClaimId, function (it) {
                                return it['TicketId'] == item.Id;
                            })['Code'];
                        }

                    }


                    list.push(item);
                });
            self.listAllTicket(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }


    ///SUPPORT
    //lay ra danh sach list ticket support search
    self.GetAllTicketSupportList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Support");
        self.listAllTicketSupport([]);
        var searchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (searchTicketModal.Status === undefined) {
            searchTicketModal.Status = -1;
        }
        if (searchTicketModal.SystemId === undefined) {
            searchTicketModal.SystemId = -1;
        }
        $.post("/Ticket/GetAllTicketSupportList", { page: page, pageSize: pagesize, searchModal: searchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {

                    item.ContentInternal = ko.observable(item.ContentInternal);

                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }
                    item.ContentInternal.subscribe(function (newValue) {
                        self.NoteCloseCommon(ko.mapping.toJS(item));
                    });

                    item.CustomerCareFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['CustomerCareFullName'];

                    item.OrderFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['UserFullName'];

                    item.CustomerEmail = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Email'];

                    item.CustomerPhone = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Phone'];

                    item.CustomerWarehouseName = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['WarehouseName'];

                    list.push(item);
                });
            self.listAllTicketSupport(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    ///COMPLAIN
    self.GetAllTicketComplainList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Complain");
        self.listAllTicketComplain([]);
        self.customerId(null);
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        if (SearchTicketModal.SystemId === undefined) {
            SearchTicketModal.SystemId = -1;
        }
        $.post("/Ticket/GetAllTicketComplainList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {

                    item.ContentInternal = ko.observable(item.ContentInternal);

                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }
                    item.ContentInternal.subscribe(function (newValue) {
                        self.NoteCloseCommon(ko.mapping.toJS(item));
                    });

                    item.CustomerCareFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['CustomerCareFullName'];

                    item.OrderFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['UserFullName'];

                    item.CustomerEmail = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Email'];

                    item.CustomerPhone = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Phone'];

                    item.CustomerWarehouseName = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['WarehouseName'];

                    item.ClaimId = 0;
                    item.ClaimCode = "";
                    if (data.listClaimId.length !== 0) {
                        item.ClaimId = _.find(data.listClaimId, function (it) {
                            return it['TicketId'] == item.Id;
                        });
                        if (item.ClaimId != undefined) {
                            item.ClaimId = _.find(data.listClaimId, function (it) {
                                return it['TicketId'] == item.Id;
                            })['Id'];

                            item.ClaimCode = _.find(data.listClaimId, function (it) {
                                return it['TicketId'] == item.Id;
                            })['Code'];
                        }
                    }

                    list.push(item);
                });
            self.listAllTicketComplain(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    ///ASSIGN
    self.GetAllTicketAssignList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Not assigned");
        self.listAllTicketAssign([]);
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        if (SearchTicketModal.SystemId === undefined) {
            SearchTicketModal.SystemId = -1;
        }
        $.post("/Ticket/GetAllTicketAssignList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal }, function (data) {

            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {

                    item.ContentInternal = ko.observable(item.ContentInternal);
                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.ContentInternal.subscribe(function (newValue) {
                        self.NoteCloseCommon(ko.mapping.toJS(item));
                    });

                    item.CustomerEmail = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Email'];

                    item.CustomerPhone = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Phone'];

                    item.CustomerWarehouseName = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['WarehouseName'];

                    list.push(item);
                });
            self.listAllTicketAssign(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    ///LAST
    self.GetAllTicketLastList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Delayed processing");
        self.listAllTicketLast([]);
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        //thiết lập lại giá trị cho Status nếu status=undefined
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        if (SearchTicketModal.SystemId === undefined) {
            SearchTicketModal.SystemId = -1;
        }
        $.post("/Ticket/GetAllTicketLastList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {

            total = data.totalRecord;
            var list = [];
            _.each(data.ticketModal,
                function (item) {

                    item.ContentInternal = ko.observable(item.ContentInternal);

                    item.CssRead = ko.observable('no-read');
                    item.CheckRead = function () {
                        item.CssRead('yes-read');
                        return true;
                    }

                    item.ContentInternal.subscribe(function (newValue) {
                        self.NoteCloseCommon(ko.mapping.toJS(item));
                    });

                    item.CustomerCareFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['CustomerCareFullName'];

                    item.OrderFullName = _.find(data.listCustomerCase, function (it) {
                        return it['Id'] == item.OrderId;
                    })['UserFullName'];

                    item.CustomerEmail = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Email'];

                    item.CustomerPhone = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['Phone'];

                    item.CustomerWarehouseName = _.find(data.listCustomer, function (it) {
                        return it['Id'] == item.CustomerId;
                    })['WarehouseName'];

                    list.push(item);

                });
            self.listAllTicketLast(list);
            self.paging();
            self.isRending(true);
            self.isLoading(true);
        });
    }
    //ALL CUSTOMER
    self.CustomerList = ko.observableArray([]);
    self.GetAllCustomer = function () {
        self.CustomerList([]);
        self.titleTicket("Look up customer information");
        $.post("/Ticket/GetAllCustomer", {}, function (data) {
            self.CustomerList(data);
        });
    }
    //==================DANH SÁCH HOÀN TIỀN KHIẾU NẠI====================================

    self.reasonClaimId = ko.observable();
    self.reasonClaim = ko.observable();
    ///ClaimForRefund
    self.GetClaimForRefundList = function () {
        self.isRending(false);
        self.isLoading(false);
        self.titleTicket("Customer refund request");
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
        $.post("/Ticket/GetClaimForRefundList", { page: page, pageSize: pagesize, searchModal: searchClaimForRefundData }, function (data) {
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
    //================================================= POST LẤY DANH SÁCH DỮ LIỆU KHIẾU NẠI ĐỂ HIỂN THỊ


    //================================================= HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG
    self.customerId = ko.observable();
    self.userId = ko.observable();
    self.listOrderCustomer = ko.observableArray([]);

    self.customerId.subscribe(function (newValue) {
        if (self.customerId() > 0 || self.customerId() != null || self.customerId() != "" || self.customerId() != undefined) {
            $.post("/Ticket/GetCustomerInfo", { customerId: self.customerId() }, function (data) {
                self.mapCustomerModel(data.customer);
                self.searchCustomer();

                _.each(data.listOrder, function (item) {
                    item.Code = ReturnCode(item.Code);
                })

                self.listOrderCustomer(data.listOrder);

                $('.select2-order').select2();

                self.complainuserinternallist([]);
                self.complainuser([]);
            });
        }
    });

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
        self.complainuserinternallist([]);
        self.complainuser([]);
        $.post("/Ticket/SupportHistory", { customerId: self.customerId() }, function (data) {
            self.listComplainCustomer(data.customer);
            self.listComplainByCustomer(data.customerinfo);
        });
    }

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

    //================================================= /HỖ TRỢ TRA CỨU THÔNG TIN KHÁCH HÀNG

    /// Lấy thông tin hỗ trợ khiếu nại
    self.listComplainSupport = ko.observable([]);
    self.ComplainSupport = function (complainId) {
        $.post("/Ticket/listComplainSupport", { complainId: complainId }, function (data) {
            self.listComplainSupport(data);
        });
    }

    //SEARCH MODEL
    self.search = function (page) {
        window.page = page;

        self.isRending(false);
        self.isLoading(true);

        if (self.active() === 'ticket') {
            self.GetAllTicketList();
            //self.listAllTicket([]);
            //var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());

            //$.post("/Ticket/GetAllTicketList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listAllTicket(data.ticketModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }
        if (self.active() === 'support') {
            self.GetAllTicketSupportList();
            //self.listAllTicketSupport([]);
            //var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());

            //$.post("/Ticket/GetAllTicketSupportList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listAllTicketSupport(data.ticketModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }

        if (self.active() === 'assign') {
            self.GetAllTicketAssignList();
            //self.listAllTicketAssign([]);
            //var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());

            //$.post("/Ticket/GetAllTicketAssignList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listAllTicketAssign(data.ticketModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }

        if (self.active() === 'complain') {
            self.GetAllTicketComplainList();
            //self.listAllTicketComplain([]);
            //var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());

            //$.post("/Ticket/GetAllTicketComplainList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listAllTicketComplain(data.ticketModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }

        if (self.active() === 'last') {
            self.GetAllTicketLastList();
            //self.listAllTicketLast([]);
            //var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());

            //$.post("/Ticket/GetAllTicketLastList", { page: page, pageSize: pagesize, searchModal: SearchTicketModal, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listAllTicketLast(data.ticketModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(false);
            //});
        }

        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
            //self.listClaimForRefundView([]);
            //var searchClaimForRefundData = ko.mapping.toJS(self.SearchClaimForRefundModal());
            ////thiết lập lại giá trị cho Status nếu status=undefined
            //if (searchClaimForRefundData.Status === undefined) {
            //    searchClaimForRefundData.Status = -1;
            //}
            //if (searchClaimForRefundData.CustomerId === undefined) {
            //    searchClaimForRefundData.CustomerId = -1;
            //}
            //$.post("/Ticket/GetClaimForRefundList", { page: page, pageSize: pagesize, searchModal: searchClaimForRefundData, customerId: self.customerId() == undefined ? -1 : self.customerId(), userId: self.userId() == undefined ? -1 : self.userId() }, function (data) {
            //    total = data.totalRecord;
            //    self.listClaimForRefundView(data.claimForRefundModal);
            //    self.paging();
            //    self.isRending(true);
            //    self.isLoading(true);
            //});
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
        if (self.active() === 'ticket') {
            self.GetAllTicketList();
        }
        if (self.active() === 'support') {
            self.GetAllTicketSupportList();
        }
        if (self.active() === 'assign') {
            self.GetAllTicketAssignList();
        }
        if (self.active() === 'complain') {
            self.GetAllTicketComplainList();
        }
        if (self.active() === 'last') {
            self.GetAllTicketLastList();
        }
        if (self.active() === 'claimforrefund') {
            self.GetClaimForRefundList();
        }

        $('#nav-tab' + self.systemId()).click();
        self.isRending(true);
        self.isLoading(false);
    }
    //____________________________

    // File ảnh
    self.isUpload = ko.observable(true);
    self.DetailImagePath1 = ko.observable("");
    self.DetailImagePath2 = ko.observable("");
    self.DetailImagePath3 = ko.observable("");
    self.DetailImagePath4 = ko.observable("");
    self.selectedService = ko.observable();
    self.listComplainType = ko.observableArray();

    //Biến chọn Type biểu đồ thống kê
    self.contentChart = ko.observable(1);
    self.listTicket = ko.observableArray([]);
    self.listDate = ko.observableArray([]);
    self.listMoney = ko.observableArray([]);

    self.viewReport = function () {
        self.totalTicket(0);
        $.post("/Ticket/listViewReport", { dateStart: self.SearchTicketModal().DateStart(), dateEnd: self.SearchTicketModal().DateEnd() }, function (data) {
            self.totalTicket(data.total);
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
                        format: '{value} Ticket',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Ticket number',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    }
                }, { // Secondary yAxis
                    title: {
                        text: 'Ticket',
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    labels: {
                        format: '{value} Ticket',
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
                    name: 'Ticket number',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailTicket,
                    tooltip: {
                        valueSuffix: ' ticket'
                    }
                }]
            });

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
                    //text: 'Sum ticket hôm nay'
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
        });
    }

    //Tụ động Search khách hàng
    self.init = function () {
        self.totalTicketList(12);
        //self.viewReport();

        $('.nav-tabs').tabdrop();
        self.setDateRange();

        //$(".select-view").select2();
    }

    self.setDateRange = function () {
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
                      self.SearchTicketModal().DateStart('');
                      self.SearchTicketModal().DateEnd('');
                  }
                  else {
                      $('#daterange-btn span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                      self.SearchTicketModal().DateStart(start.format());
                      self.SearchTicketModal().DateEnd(end.format());
                  }
              }
          );
        $('#daterange-btn').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btn span').html('Create date');
            self.SearchTicketModal().DateStart("");
            self.SearchTicketModal().DateEnd("");
        });

        $('#daterange-btnReport').daterangepicker(
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
                     'This week': [moment().startOf('week'), moment().endOf('week')],
                     'This month': [moment().subtract(0, 'month').startOf('month'), moment().subtract(0, 'month').endOf('month')]
                 },
                 startDate: moment(),
                 endDate: moment()
             },
             function (start, end) {
                 if (start.format() === 'Invalid date') {
                     $('#daterange-btnReport span').html('Created date');
                     self.SearchTicketModal().DateStart('');
                     self.SearchTicketModal().DateEnd('');
                 }
                 else {
                     $('#daterange-btnReport span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                     self.SearchTicketModal().DateStart(start.format());
                     self.SearchTicketModal().DateEnd(end.format());
                 }
                 if (self.active() == 'SupportReport') {
                     self.viewReport();
                 }
                 else if (self.active() == 'reportOrderWaitSituation') {
                     self.viewReportOrderWaitSituation();
                 }
                 else {
                     self.viewReportTicketSituation();
                 }

             }
         );
        $('#daterange-btnReport').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btnReport span').html('Created date');
            self.SearchTicketModal().DateStart("");
            self.SearchTicketModal().DateEnd("");

            if (self.active() == 'SupportReport') {
                self.viewReport();
            }
            else if (self.active() == 'reportOrderWaitSituation')
            {
                self.viewReportOrderWaitSituation();
            }
            else {
                self.viewReportTicketSituation();
            }
        });

    }
    // =====================Hàm tạo Ticket khiếu nại============================
    self.createInfomationTicket = function () {
        self.listComplainType(window.listComplainTypeService);
        self.complainModel(new complainModel());
        self.initInputMark();
        $('#complaincreate').modal();
        self.searchCustomer();
    }
    self.createTicket = function () {
        if (self.selectedService() == undefined || self.orderCodeCustomer() == "" || self.orderCodeCustomer() == "") {
            toastr.error('Please provide full details before you create Ticket!');
        }
        else {
            self.complainModel().CustomerId(self.customerId());
            self.complainModel().OrderCode(self.orderCodeCustomer());
            self.complainModel().ImagePath1(self.DetailImagePath1());
            self.complainModel().ImagePath2(self.DetailImagePath2());
            self.complainModel().ImagePath3(self.DetailImagePath3());
            self.complainModel().ImagePath4(self.DetailImagePath4());
            self.complainModel().TypeService(self.selectedService());
            var list = [];
            _.each(self.listOrderDetail(), function (item) {
                if (item.NoteComplain != null && item.NoteComplain != undefined && item.NoteComplain != "") {
                    item.Note = item.NoteComplain,
                    list.push(item);
                }
            });

            $.post("/Ticket/CreateTicket", { complain: self.complainModel(), list: list }, function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                } else {

                    self.searchCustomer();
                    self.totalTicketWait(result.totalWait);
                    toastr.success(result.msg);
                    self.GetAllTicketAssignList();
                    //$('#complaincreate').hide();
                    self.complainModel(new complainModel());

                }
            });
            self.customerId("");
        }
    }

    self.listTypeService = function (str) {
        var list = ko.observableArray([]);
        //var array = str.split(',');
        //_.each(array, function (item) {
        //    list.push({ Name: window.listComplainTypeService[item].Text });
        //});

        return list;
    }


    var maxFileLength = 5120000;;
    self.addImage = function () {
        $(".flieuploadImg").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": The size is too large";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Not in correct format";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("The file is not allowed");
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


    //Hàm lấy thông tin nhân viên
    self.searchUser = function () {
        $(".user-search")
            .select2({
                ajax: {
                    url: "Ticket/GetUserSearch",
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
                language: 'en'
            });
    };


    self.titleCustomer = ko.observable();
    self.complainModel = ko.observable(new complainModel());
    self.customerModel = ko.observable(new customerOfStaffModel());
    self.complainDetailModel = ko.observable(new complainDetailModel());

    self.listUserOffice = ko.observableArray([]);
    self.customerEmail = ko.observable();
    self.avatar = ko.observable();
    self.customerPhone = ko.observable();
    self.customerAddress = ko.observable();
    self.customerLevel = ko.observable();
    self.levelName = ko.observable();
    self.complainuser = ko.observable([]);
    self.complainuserinternallist = ko.observable([]);
    self.complainuser1 = ko.observable([]);
    self.count = ko.observable();
    self.userselected = ko.observable();
    self.isSubmit = ko.observable(true);
    self.class = ko.observable();


    ///Detail Ticket
    self.NoteClose = function () {
        if (self.complainModel().Id() != null && self.complainModel().Id() != undefined && self.complainModel().Id() != "") {
            $.post("/Ticket/NoteClose", { complainId: self.complainModel().Id(), noteClose: self.complainModel().ContentInternal() }, function (result) {
                if (!result.status) {
                    toastr.error(result.msg);
                } else {
                    toastr.success(result.msg);
                    self.GetAllTicketList();
                }
            });
        }
    }
    //Thêm note nội bộ trên danh sách
    self.NoteCloseCommon = function (data) {
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
                    self.GetAllTicketComplainList();
                }
            },
            async: false
        });

        return isLook;
    }

    self.viewTicketDetail = function (data) {
        self.GetTicketDetailCommon(data.Id);
        self.interval();
    }
    self.GetTicketDetailCommon = function (ticketId) {

        self.titleCustomer('Detail customer');
        self.customerEmail("");
        self.customerPhone("");
        self.customerAddress("");
        self.customerLevel("");
        self.complainuser1([]);
        self.complainuser([]);
        self.complainuserinternallist([]);
        self.complainModel(new complainModel());
        self.complainDetailModel(new complainDetailModel());
        self.isDetailRending(false);
        self.GetUser();
        self.isUpload(false);
        $.post("/Ticket/GetTicketDetail", { ticketId: ticketId }, function (result) {
            self.isDetailRending(true);
            if (result.ticketModal != null) {
                self.mapComplainModel(result.ticketModal);
                self.customerEmail(result.customer.Email);
                self.customerPhone(result.customer.Phone);
                self.customerAddress(result.customer.Address);
                self.customerLevel(result.customer.LevelName);
                self.complainuser1(result.list);
                //self.complainuser(result.complainuserlist);
                //self.complainuserinternallist(result.complainuserinternallist);
                if (result.complainusermain != null) {
                    self.mapcomplainDetailModel(result.complainusermain);
                }
                self.listComplainOrder(result.listComplainOrder);

                var list = self.ChatView(result.complainuserinternallist);
                self.complainuserinternallist(list);
                list = self.ChatView(result.complainuserlist);
                self.complainuser(list);
                $('#ticketDetailModal').modal();
            }

            else {
                $('#ticketDetailModalMini').modal();
            }
        });
    }

    self.ChatView = function (complainuserlist) {
        var list = [];

        var group = _.groupBy(complainuserlist, function (obj) {
            return obj.GroupId;
        });

        var array = $.map(group, function (value) {
            return [value];
        });

        _.forEach(array, function (listObj) {
            var item = {
                Id: _.last(listObj).Id,
                ListObj: [],
                UserId: listObj[0].UserId,
                UserName: listObj[0].UserName,
                Content: listObj[0].Content,
                ComplainId: listObj[0].ComplainId,
                CreateDate: listObj[0].CreateDate,
                CustomerId: listObj[0].CustomerId,
                CustomerName: listObj[0].CustomerName,
                IsInHouse: listObj[0].IsInHouse,
                IsRead: listObj[0].IsRead,
                CommentType: listObj[0].CommentType,
                SystemId: listObj[0].SystemId,
                SystemName: listObj[0].SystemName,
                UserPosition: listObj[0].UserPosition,
                GroupId: listObj[0].GroupId
            }
            _.forEach(listObj, function (obj) {
                item.ListObj.push(obj);
            });

            list.push(item);
        });
        self.isUpload(true);
        return list;
    };
    self.GetTicketDetailView = function (ticketId) {
        self.GetTicketDetailCommon(ticketId);

    }
    self.GetTicketDetail = function (ticketId) {
        self.customerEmail("");
        self.customerPhone("");
        self.customerAddress("");
        self.customerLevel("");
        self.complainuser1([]);
        self.complainuser([]);
        self.complainuserinternallist([]);
        self.complainModel(new complainModel());
        self.complainDetailModel(new complainDetailModel());
        self.GetUser();
        self.isUpload(false);
        $.post("/Ticket/GetTicketDetail", { ticketId: ticketId }, function (result) {
            self.isDetailRending(true);
            if (result.ticketModal != null) {
                self.mapComplainModel(result.ticketModal);
                self.customerEmail(result.customer.Email);
                self.customerPhone(result.customer.Phone);
                self.customerAddress(result.customer.Address);
                self.customerLevel(result.customer.LevelName);
                self.complainuser1(result.list);
                //self.complainuser(result.complainuserlist);
                //self.complainuserinternallist(result.complainuserinternallist);
                if (result.complainusermain != null) {
                    self.mapcomplainDetailModel(result.complainusermain);
                }
                self.listComplainOrder(result.listComplainOrder);
                //Xuân thêm
                var list = self.ChatView(result.complainuserinternallist);
                self.complainuserinternallist(list);

                list = self.ChatView(result.complainuserlist);
                self.complainuser(list);

            }
            self.interval();
        });

    }

    // Hiển thị nội dung trao đổi về khiếu nại khách hàng để chỉnh Edit
    self.contentEdit = ko.observable();
    self.contentEditId = ko.observable();

    self.editContentUser = function (data) {
        self.contentEditId();
        self.contentEdit();
        self.contentEdit(data.Content);
        self.contentEditId(data.Id);
        $('#commentEdit').modal();
    }

    self.deleteContentUser = function (data) {
        self.contentEditId();
        self.contentEditId(data.Id);
        $('#commentDelete').modal();
    }
    // Cập nhật nội dung trao đổi về khiếu nại khách hàng
    self.updateContent = function () {
        $.post("/Ticket/UpdateContent", { complainDetailId: self.contentEditId(), complainContent: self.contentEdit() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                //toastr.success(response.msg);
                self.getInit();
                self.GetTicketDetail(self.complainModel().Id());
            }
        });
    }

    // Xóa nội dung trao đổi về khiếu nại khách hàng
    self.deleteContent = function () {
        $.post("/Ticket/DeleteContent", { complainDetailId: self.contentEditId() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                //toastr.success(response.msg);
                self.getInit();
                self.GetTicketDetail(self.complainModel().Id());
            }
        });
    }

    // hiển thị lựa chọn đối tượng chat cho nhân viên CSKH
    self.feedbackComplainModal = function () {
        $('#commentForCustomer').modal();
    }


    //Thêm người hỗ trợ
    self.AddUserSupport = function () {
        $.post("/Ticket/AddUserSupport", { userSupportId: self.complainDetailModel().UserRequestId(), complainId: self.complainModel().Id() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                self.GetTicketDetail(self.complainModel().Id());
            }
        });
    }

    //Xóa hỗ trợ
    self.deleteSupport = function (userId) {
        $.post("/Ticket/DeleteSupport", { userId }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
        } else {
                toastr.success(response.msg);
                self.GetTicketDetail(response.complain.Id);
                self.getInit();
                self.search(page);
        }
        });
        }

    // Nhận ticket nhận về xử lý
    self.receiveTicket = function (data) {
        //self.mapComplainModel(data);

        $.post("/Ticket/ReceiveTicket", { complainId: data.Id}, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                //self.search(page);
                self.GetAllTicketAssignList();
            }
        });
    }
    self.receiveTicketOrder = function (data) {
        //self.mapComplainModel(data);

        $.post("/Ticket/ReceiveTicket", { complainId: data}, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                //self.search(page);
                self.GetAllTicketAssignList();
            }
        });
    }

    //Hàm phân ticket trễ xử lý giao việc cho nhân viên 
    self.assignedComplain = function () {
        self.isSubmit(false);
        //Lấy về item
        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.userselected(); });

        $.post("/Ticket/AssignedComplain", { complainId: self.complainModel().Id(), user: userSelect }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                self.isSubmit(true);
                self.search(page);
                //self.GetAllTicketAssignList();
            }
        });
    };

    //Hàm phân ticket chưa nhận xử lý giao việc cho nhân viên
    self.assignedComplainNotGet = function (dataId) {
        self.isSubmit(false);
        //Lấy về item
        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.userselected(); });
        $.post("/Ticket/AssignedComplainNotGet", { complainId: dataId, user: userSelect }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                self.isSubmit(true);
                self.search(page);
                //self.GetAllTicketAssignList();
            }
        });
    };
    self.assignedCustomerCareComplain = function (complainId, userId) {
        self.isSubmit(false);

        $.post("/Ticket/AssignedComplainAuto", { complainId: complainId, userId: userId }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.getInit();
                self.isSubmit(true);
                self.search(page);
                //self.GetAllTicketAssignList();
            }
        });
    };

    //Hàm phân ticket khi chưa có nhân viên nào nhận xử lý
    self.assignedComplainNonGet = function () {
        self.isSubmit(false);
        //Lấy về item
        var userSelect = _.find(self.listUserOffice(), function (item) { return item.Id === self.userselected(); });

        $.post("/Ticket/AssignedComplainNonGet", { complainId: self.complainModel().Id(), user: userSelect }, function (response) {
            if (!response.status) {
                $('#ticketAssignedModal').modal('hide');
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                $('#ticketAssignedModal').modal('hide');
                self.getInit();
                self.isSubmit(true);
                self.search(page);
                //self.GetAllTicketAssignList();
            }
        });
    };

    //Tiếp nhận ticket
    self.receiveFixTicket = function (data) {
        var complainId = $('#complainID').text();
        $.post("/Ticket/ReceiveTicket", { complainId: complainId }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.viewTicketDetail(self.complainModel());
                self.GetAllTicketLastList();
            }
        });
    }

    //Hàm show modal phân ticket chưa nhận xử lý cho nhân viên 
    self.assignedTicketModal = function (data) {
        self.isSubmit(true);
        self.complainModel().Id(data);
        $.post("/Ticket/GetUserComplain", {}, function (result) {
            self.listUserOffice(result);
        });
        $('#ticketAssignedModal').modal();
    };

    //Hàm show modal phân ticket trễ xử lý cho nhân viên 
    self.assignedTicketLateModal = function (data) {
        self.isSubmit(true);
        self.mapComplainModel(data);
        $.post("/Ticket/GetUserComplain", {}, function (result) {
            self.listUserOffice(result);
        });
        $('#ticketAssignedLateModal').modal();
    };


    //self.btnExecuteClaimForRefund = function (data) {
    //    //self.isSubmit(true);
    //    //self.mapComplainModel(data);
    //    $.post("/Ticket/GetUserComplain", {}, function (data) {
    //        self.listUserOffice(data);
    //    });
    //    $('#ticketAssignedModal').modal();
    //};

    self.contentUser = ko.observable("");
    self.contentCustomer = ko.observable("");
    //hàm phản hồi cho khách hàng
    self.feedbackComplain = function () {
        if (self.contentCustomer() === "" || self.contentCustomer() == null) {
            $('#requireCustomer').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: self.contentCustomer(), objectChat: false }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    toastr.success(response.msg);
                    self.contentCustomer("");
                    self.GetTicketDetail(self.complainModel().Id());

                }
            });
        }
    };

    self.addImageDetail = function (data1) {

        $(".flieuploadImg").fileupload({
            url: "/Upload/UploadImages",
            sequentialUploads: true,
            dataType: "json",
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": The size is too large";
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += "<br/>";
                    }
                    msg += file.name + ": Not in correct format";
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (data.result === -5) {
                    toastr.error("The file is not allowed");
                    return;
                }

                self.isUpload(false);

                if (data1 == 1) {
                    $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: window.location.origin + data.result[0].path, type: 1, objectChat: false }, function (result) {
                        self.GetTicketDetail(self.complainModel().Id());
                        self.contentCustomer("");
                    });
                }
                else {
                    $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: window.location.origin + data.result[0].path, type: 1, objectChat: true }, function (response) {
                        if (!response.status) {
                            toastr.error(response.msg);
                        } else {
                            //toastr.success(response.msg);
                            self.GetTicketDetail(self.complainModel().Id());
                            self.contentUser("");
                        }
                    });

                }
            }
        });
        return true;
    }

    //hàm trao đổi giữa các nhân viên
    self.feedbackUser = function () {
        if (self.contentUser() === "" || self.contentUser() == null) {
            $('#requireUser').css('display', 'block');
        }
        else {
            $.post("/Ticket/feedbackComplain", { complainId: self.complainModel().Id(), content: self.contentUser(), objectChat: true }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    //toastr.success(response.msg);
                    self.GetTicketDetail(self.complainModel().Id());
                    self.contentUser("");
                }
            });
        }
    };

    // hàm huy ticket
    self.requestCancel = function () {
        $('#cancelComplain').modal();
    };

    self.cancelComplain = function () {
        $.post("/Ticket/CancelComplain", { complainId: self.complainModel().Id() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);

                $('#cancelComplain').modal('hide');
                $('#ticketDetailModal').modal('hide');
                self.renderSystem();
                self.totalTicketNeedFix(response.totalTicketNeedFix);
                self.GetAllTicketAssignList();
                self.GetAllTicketComplainList();
                self.GetAllTicketLastList();
                self.GetAllTicketList();
                self.GetAllTicketSupportList();
                self.GetClaimForRefundList();
            }

        });
    };

    // hàm hoàn thành ticket
    self.request = function () {
        $('#requestComplain').modal();
    };

    self.finishComplain = function () {
        $.post("/Ticket/finishComplain", { complainId: self.complainModel().Id() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                //self.viewTicketDetail(self.complainModel());
                self.renderSystem();
                self.GetAllTicketComplainList();
                self.GetAllTicketList();
                self.totalTicketNeedFix(response.totalTicketNeedFix);
                $('#ticketDetailModal').modal('hide');
                $('#requestComplain').modal('hide');
            }

        });
    };

    //===============================Hoàn tiền trong khiếu nại==================
    self.order = ko.observable(new orderModel());
    self.listOrder = ko.observable([]);
    self.ticketId = ko.observable();

    self.claimForRefund = ko.observable(new ClaimForRefund);
    self.listClaimForRefund = ko.observableArray([]);
    self.totalClaimForRefund = ko.observable();
    self.totalClaim = ko.observable(0);
    self.typeServiceClose = ko.observable("");

    //Khai báo biến thông tin Cấp Level Vip
    self.vipOrder = ko.observable(0);
    self.vipShip = ko.observable(0);
    self.vipName = ko.observable("");

    //Hiển thị Detail Refund
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
        self.complainModel().TypeServiceClose("");

    }
    self.btnViewClaimForRefundIdDetail = function (dataId) {
        self.listClaimForRefundDetail([]);
        self.isDetailRending(false);
        self.complainModel(new complainModel());
        self.vipOrder(0);
        self.vipShip(0);
        self.vipName("");
        $.post("/Ticket/GetClaimForRefundDetail", { claimForRefundId: dataId }, function (result) {
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
        self.complainModel().TypeServiceClose("");

    }

    // Show lên Modal btnViewRefundMoneyModal
    self.checkClaimForRefundId = ko.observable();
    self.checkClaimForRefund = function (data) {
        $.post("/Ticket/CheckRefundTicket", { ticketId: data.Id }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                self.checkClaimForRefundId(data.Id);
                if (result.value > 0) {
                    $('#checkClaim').modal();

                }
                else {
                    self.btnViewRefundMoneyModal();
                }

            }
        });
    };
   
    self.checkCustomerCare = function (data) {
        $.post("/Ticket/CheckCustomerCare", { orderId: data.OrderId }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                self.checkCustomerCareId(data.Id);
                self.orderCodeTicket(data.OrderCode);
                self.customerCareTicket(data.Code);
                self.customerCareOrder(result.name);
                self.customerCareOrderId(result.userId);
                if (result.value > 0) {
                    $('#customerCareReceive').modal();

                }
                else {
                    self.receiveTicket(data);
                }

            }
        });
    };

    //Assign
    self.checkAssignCustomerCare = function (data) {
        $.post("/Ticket/CheckCustomerCare", { orderId: data.OrderId }, function (result) {
            if (!result.status) {
                toastr.error(result.msg);
            } else {
                self.checkCustomerCareId(data.Id);
                self.orderCodeTicket(data.OrderCode);
                self.customerCareTicket(data.Code);
                self.customerCareOrder(result.name);
                self.customerCareOrderId(result.userId);
                if (result.value > 0) {
                    $('#customerCareAssign').modal();

                }
                else {
                    self.assignedTicketModal(data);
                }

            }
        });
    };
    self.viewRefundMoneyModalCommon = function () {
        self.btnViewRefundMoneyModal();
    };


    self.btnViewRefundMoneyModal = function (data) {
        self.totalClaim(0);
        self.complainModel().TypeServiceClose("");
        self.vipOrder(0);
        self.vipShip(0);
        self.vipName("");
        //self.listOrder([]);
        self.isDetailRending(false);
        $.post("/Ticket/RefundTicket", { ticketId: self.checkClaimForRefundId() }, function (result) {
            self.isDetailRending(true);
            self.customerEmail(result.customer.Email);
            self.customerPhone(result.customer.Phone);
            self.customerAddress(result.customer.Address);
            self.avatar(result.customer.Avatar);
            self.levelName(result.customer.LevelName);

            self.listOrderService(result.orderService);


            self.mapComplainModel(result.complain);
            //======Hàm tự động tính toán
            //self.listOrder(result.listOrder);
            if (result.orderDetail != null) {
                self.mapObject(result.orderDetail)
            }
            if (result.levelVip != null) {
                self.vipOrder(result.levelVip.Order);
                self.vipShip(result.levelVip.Ship);
                self.vipName(result.levelVip.Name);
            }
            _.each(result.claimForRefundVM.LstClaimForRefundDetails,
                        function (it) {
                            // bổ sung
                            it.Price = formatNumberic(it.Price, 'N2');
                            it.TotalPrice = formatNumberic(it.TotalPrice, 'N2');
                            it.TotalExchange = formatNumberic(it.TotalExchange, 'N2');
                            it.cacheQuantityFailed = it.QuantityFailed;
                            it.TotalQuantityFailed = ko.observable(it.TotalQuantityFailed);

                            it.QuantityFailed = ko.observable(it.QuantityFailed);
                            it.QuantityFailed.subscribe(function (newValue) {
                                if (it.cacheQuantityFailed !== newValue) {
                                    newValue = Globalize.parseFloat(newValue);
                                    //tính toán
                                    if (newValue <= it.Quantity) {
                                        it.TotalQuantityFailed(formatNumberic((newValue * Globalize.parseFloat(it.Price)), 'N2'));
                                        //it.TotalQuantityFailed = it.TotalQuantityFailedTest;
                                        self.totalAllClaimForRefundFix();
                                        it.cacheQuantityFailed = newValue;
                                    }
                                    else {
                                        it.QuantityFailed(0);
                                        //newValue = it.QuantityFailed;
                                        toastr.error('The number of input errors exceeded the number of orders!');
                                    }

                                }
                            });

                        });
            self.mapClaimForRefund(result.claimForRefundVM.ClaimForRefund);

            if (self.claimForRefund().MoneyOrderRefund() == null) self.claimForRefund().MoneyOrderRefund(0);
            if (self.claimForRefund().MoneyOrderRefundDicker() == null) self.claimForRefund().MoneyOrderRefundDicker(0);
            if (self.claimForRefund().SupporterMoneyRequest() == null) self.claimForRefund().SupporterMoneyRequest(0);
            if (self.claimForRefund().CurrencyDiscount() == null) self.claimForRefund().CurrencyDiscount(0);
            if (self.claimForRefund().RealTotalRefund() == null) self.claimForRefund().RealTotalRefund(0);
            if (self.claimForRefund().MoneyRefund() == null) self.claimForRefund().MoneyRefund(0);
            if (self.claimForRefund().MoneyOther() == null) self.claimForRefund().MoneyOther(0);
            self.listClaimForRefund(result.claimForRefundVM.LstClaimForRefundDetails);
            self.initInputMark();
        });
        $('#refundMoneyFixModal').modal();

        //$('#complain_tree .dropdownjstree').remove();
        //$("#complain_tree").dropdownjstree({
        //    source: window.listcomplainTypJsTree,
        //    selectedNode: '',
        //    selectNote: (node, selected) => {
        //        self.complainModel().TypeServiceClose(selected.node.original.id === 0 ? null : selected.node.original.id);
        //    },
        //    treeParent: {
        //        hover_node: false,
        //        select_node: false
        //    }
        //});


    }

    //Hoàn tiền
    self.btnClickRefundTicketModal = function () {
        self.CreateNewClaimForRefund();
    }

    self.btnErrorViewRefundMoneyModal = function () {

        self.isDetailRending(true);


        _.each(self.listClaimForRefund(),
                    function (it) {
                        // bổ sung
                        it.QuantityFailed.subscribe(function (newValue) {
                            if (it.cacheQuantityFailed !== newValue) {
                                newValue = Globalize.parseFloat(newValue);
                                //tính toán
                                if (newValue <= it.Quantity) {
                                    it.TotalQuantityFailed(formatNumberic((newValue * Globalize.parseFloat(it.Price)), 'N2'));
                                    //it.TotalQuantityFailed = it.TotalQuantityFailedTest;
                                    self.totalAllClaimForRefundFix();
                                    it.cacheQuantityFailed = newValue;
                                }
                                else {
                                    it.QuantityFailed(0);
                                    //newValue = it.QuantityFailed;
                                    toastr.error('The number of input errors exceeded the number of orders!');
                                }

                            }
                        });
                    });
        if (self.claimForRefund().MoneyOrderRefund() == null) self.claimForRefund().MoneyOrderRefund(0);
        if (self.claimForRefund().MoneyOrderRefundDicker() == null) self.claimForRefund().MoneyOrderRefundDicker(0);
        if (self.claimForRefund().SupporterMoneyRequest() == null) self.claimForRefund().SupporterMoneyRequest(0);
        if (self.claimForRefund().CurrencyDiscount() == null) self.claimForRefund().CurrencyDiscount(0);
        if (self.claimForRefund().RealTotalRefund() == null) self.claimForRefund().RealTotalRefund(0);
        if (self.claimForRefund().MoneyRefund() == null) self.claimForRefund().MoneyRefund(0);
        if (self.claimForRefund().MoneyOther() == null) self.claimForRefund().MoneyOther(0);
        self.initInputMark();

    }
    // tạo processing request Refund
    self.CreateNewClaimForRefund = function () {
        self.isDetailRending(false);
        if (self.complainModel().TypeServiceClose == 0) {
            toastr.error("You have not selected the type complain!");
            self.btnViewRefundMoneyModal(self.complainModel());
        }
        else {
            $.post("/Ticket/CreateNewClaimForRefund", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefund(), complainFund: self.complainModel() }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                    self.btnErrorViewRefundMoneyModal();
                } else {
                    toastr.success(response.msg);
                    self.totalClaimForRefund(response.totalClaimForRefund);
                    //self.btnViewRefundMoneyModal(self.complainModel());
                    self.btnErrorViewRefundMoneyModal();
                    self.GetAllTicketLastList();
                    self.GetAllTicketList();
                    self.GetAllTicketComplainList();
                }
            });
        }
    }

    //Cập nhật thông tin Refund
    self.btnClaimForRefundInfoUpdate = function () {
        self.claimForRefundInfoUpdate();
    }
    self.claimForRefundInfoUpdate = function () {
        self.isDetailRending(false);
        if (self.complainModel().TypeServiceClose == 0) {
            toastr.error("You have not selected the type of grievance!");
        }
        else {
            $.post("/Ticket/ClaimForRefundInfoUpdate", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData(), complainFund: self.complainModel() }, function (response) {
                if (!response.status) {
                    toastr.error(response.msg);
                } else {
                    toastr.success(response.msg);
                    self.GetClaimForRefundList();
                }
            });
        }
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

    //Chuyển sang cho CSKH xử lý tiếp 
    self.btnClaimForRefundForwardCareCustomer = function () {
        self.claimForRefundForwardCareCustomer();
    }
    self.claimForRefundForwardCareCustomer = function () {
        self.isDetailRending(false);
        $.post("/Ticket/ClaimForRefundForwardCareCustomer", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
                //self.btnViewRefundMoneyModalContinute(self.claimForRefund());
            } else {
                toastr.success(response.msg);
                //self.btnViewRefundMoneyModalContinute(self.claimForRefund());
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
        $.post("/Ticket/ClaimForRefundForwardBoss", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData() }, function (response) {
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
        $.post("/Ticket/ClaimForRefundForwardAccountant", { claimForRefund: self.claimForRefund(), listClaimForRefundDetail: self.listClaimForRefundData() }, function (response) {
            if (!response.status) {
                toastr.error(response.msg);
            } else {
                toastr.success(response.msg);
                self.GetClaimForRefundList();
            }
        });
    }


    self.btnViewRefundMoneyModalContinute = function (data) {
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

    self.totalAllClaimForRefundFix = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefund(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed()); });
        self.totalClaim(formatNumberic(total, 'N2'));
        self.claimForRefund().MoneyRefund(formatNumberic((total * Globalize.parseFloat(self.order().ExchangeRate()) + self.claimForRefund().MoneyOther()), 'N2'));
    }
    self.totalAllClaimForRefund = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefundData(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed()); });
        self.totalClaim(formatNumberic(total, 'N2'));
        self.claimForRefund().MoneyRefund(formatNumberic((total * Globalize.parseFloat(self.order().ExchangeRate()) + self.claimForRefund().MoneyOther()), 'N2'));
    }
    self.totalAllClaimForRefundDetail = function () {
        var total = 0;
        total = _.sumBy(self.listClaimForRefundDetail(), function (it) { return Globalize.parseFloat(it.TotalQuantityFailed); });
        self.totalClaim(formatNumberic(total, 'N2'));
    }

    //Số tiền khác khi tạo yêu cầu Refund
    self.minusMoneyOtherCreate = function () {
        var MoneyOther = 0;
        if (self.claimForRefund().MoneyOther() != null) {
            MoneyOther = Globalize.parseFloat(self.claimForRefund().MoneyOther());
        }
        var MoneyRefund = Globalize.parseFloat(self.claimForRefund().MoneyRefund());
        self.claimForRefund().MoneyRefund(formatNumberic(Globalize.parseFloat(self.totalClaim()) * Globalize.parseFloat(self.order().ExchangeRate()) + MoneyOther, 'N2'));

    };

    //Số tiền khác khi cập nhật yêu cầu Refund
    self.minusMoneyOther = function () {
        var MoneyOther = 0;
        if (self.claimForRefund().MoneyOther() != null) {
            MoneyOther = Globalize.parseFloat(self.claimForRefund().MoneyOther());
        }
        var MoneyRefund = Globalize.parseFloat(self.claimForRefund().MoneyRefund());
        self.claimForRefund().MoneyRefund(formatNumberic(Globalize.parseFloat(self.totalClaim()) * Globalize.parseFloat(self.order().ExchangeRate()) + MoneyOther, 'N2'));
        self.claimForRefundInfoUpdateAutomatic();

    };
    self.updateMoneyOrderRefund = function () { }
    // Trừ tiền trả khách

    self.minusClaimForRefundOld = function () {
        var supporterMoneyRequest = Globalize.parseFloat(self.claimForRefund().SupporterMoneyRequest());
        var currencyDiscount = Globalize.parseFloat(self.claimForRefund().CurrencyDiscount());
        if (supporterMoneyRequest >= currencyDiscount) {
            self.claimForRefund().RealTotalRefund(formatNumberic((supporterMoneyRequest - currencyDiscount), 'N2'));
            self.claimForRefundInfoUpdateAutomatic();
        }
        else {
            toastr.error('The deductible amount exceeds the amount of customer care required!');
            self.claimForRefund().CurrencyDiscount(0);
            self.claimForRefund().RealTotalRefund(formatNumberic(supporterMoneyRequest, 'N2'));
        }

    };

    //Tự động tính toán Refund

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

    //==================== Object Map dữ liệu trả về View =========================================


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
        self.complainModel().ContentInternal(data.ContentInternal);



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

    //Map dữ liệu order
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

    //// Object Detail ClaimForRefundDetail
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
        self.claimForRefundDetailModel().DescriptionSupport(data.DescriptionSupport);
        self.claimForRefundDetailModel().DescriptionOrder(data.DescriptionOrder);
        self.claimForRefundDetailModel().DescriptionAccountant(data.DescriptionAccountant);
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


    //==================== Khởi tạo ===================================================

    //HÀm lấy Sum dữ liệu
    self.totalTicket = ko.observable();
    self.totalComplain = ko.observable();
    self.totalTicketNeedFix = ko.observable();
    self.totalTicketWait = ko.observable();
    self.totalTicketLate = ko.observable();
    self.totalTicketSupport = ko.observable();
    self.totalClaimForRefund = ko.observable();
    self.totalOrderWaitNew = ko.observable();
    self.totalOrderWait = ko.observable();

    self.getInit = function () {
        $.post("/Ticket/GetInit", function (data) {
            self.totalComplain(data.totalTicket);
            self.totalTicketNeedFix(data.totalTicketNeedFix);
            self.totalTicketWait(data.totalTicketWait);
            self.totalTicketLate(data.totalTicketLate);
            self.totalClaimForRefund(data.totalClaimForRefund);
            self.totalTicketSupport(data.totalTicketSupport);
            self.totalOrderWaitNew(data.totalOrderWaitNew);
            self.totalOrderWait(data.totalOrderWait);
        });
    };

    //tab
    self.listSystemRender = ko.observableArray([]);
    self.listStatus = ko.observableArray([]);
    self.listTicketTypeService = ko.observableArray([]);
    self.listTicketType = ko.observableArray([]);

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

    self.clickTab = function (tab) {
        self.SearchTicketModal().SystemId(tab);
        //hiển thị ở trang đầu tiên
        self.search(1);
        $(".select-view").select2();
    }
    $(function () {
        self.listTicketTypeService(window.listComplainTypeService);
        self.listTicketType(window.listcomplainType);
        self.viewReport();
        self.init();
        self.GetTicketSearchData();
        self.getInit();

        $("#effect-7 .img").hover(function () {
            $("#effect-7 .img .overlay").css("width", "100%");
        }
        , function () {
            $("#effect-7 .img .overlay").css("width", "0");
        });

        var checkArr = _.split(window.location.href, "#SHOW");
        if (checkArr.length == 1) {
            var arr = _.split(window.location.href, "#ORD");
            if (arr.length > 1) {
                $.post("/Purchase/GetId",
                    { code: arr[1], type: orderType.order },
                    function (result) {
                        if (result.status === msgType.error) {
                            toastr.error(result.msg);
                        } else {
                            orderDetailViewModel.viewOrderDetail(result.id);
                        }
                    });
            } else {
                var arrCode = _.split(window.location.href, "#");
                if (arrCode.length > 1) {
                    var arrCheck = ['assign', 'complain', 'ticket', 'last', 'support', 'order-wait-new', 'order-wait', 'order-cus', 'claimforrefund', 'customerfind', 'SupportReport', 'reportTicketSituation', 'reportOrderWaitSituation'];
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
                            orderTypeViewModel.showViewOrderCode(arrCode[1]);
                        }
                    }
                }
            }
        } else {
            var arr = _.split(window.location.href, "#ORD");
            if (arr.length > 1) {
                $.post("/Purchase/GetId",
                    { code: arr[1], type: orderType.order },
                    function (result) {
                        if (result.status === msgType.error) {
                            toastr.error(result.msg);
                        } else {
                            self.clickMenu('order-cus');
                            self.orderWaitViewModel.viewEditDetail(result.id);
                        }
                    });
            } else {
                var arrCode = _.split(checkArr[1], "#");
                if (arrCode.length > 1) {
                    var arrCheck = ['assign', 'complain', 'ticket', 'last', 'support', 'order-wait-new', 'order-wait', 'order-cus', 'claimforrefund', 'customerfind', 'SupportReport', 'reportTicketSituation', 'reportOrderWaitSituation'];
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
                            self.clickMenu('order-cus');
                            orderTypeViewModel.showEditOrderCode(arrCode[1]);
                        }
                    }
                }
            }
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
            $.post("/Purchase/GetId",
                { code: arr[1], type: orderType.commerce },
                function (result) {
                    if (result.status === msgType.error) {
                        toastr.error(result.msg);
                    } else {
                        orderDetailViewModel.viewOrderDetail(result.id);
                    }
                });
        }

        var arr = _.split(window.location.href, "#TK");
        if (arr.length > 1) {
            $.post("/Ticket/GetTicketId", { code: arr[1] }, function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    console.log(result.ticket);
                } else {
                    self.GetTicketDetailCommon(result.id);
                }
            });
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

        //hàm check url
        self.CheckUrl();

        //$(".table-responsive").scroll(function () {
        //    $(".re-responsive")
        //        .scrollLeft($(".table-responsive").scrollLeft());
        //});
        //$(".re-responsive").scroll(function () {
        //    $(".table-responsive")
        //        .scrollLeft($(".re-responsive").scrollLeft());
        //});

    });

    $(window).bind('hashchange', function () {
        self.CheckUrl();
    });


    self.CheckUrl = function () {
        var arr = _.split(window.location.href, "#");
        var arrCheck = ['assign', 'complain', 'ticket', 'last', 'support', 'order-wait-new', 'order-wait', 'order-cus', 'claimforrefund', 'customerfind', 'SupportReport', 'reportTicketSituation', 'reportOrderWaitSituation'];
        if (arr.length > 1) {
            if (_.lastIndexOf(arrCheck, arr[1]) !== -1) {
                self.clickMenu(arr[1]);
                setTimeout(function () {
                    self.setDateRange();
                }, 300);
            }
        }
    }

    self.showOrderDetail = function (orderId) {
        if (orderDetailViewModel) {
            orderDetailViewModel.viewOrderDetail(orderId);
            return;
        }
    }

    //========================================= BÁO CÁO TÌNH HÌNH XỬ LÝ TICKET THEO KHOẢNG THỜI GIAN============================================
    self.reportTitleTicket = ko.observable("");
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
    //self.btnCalendarReport = function () {
    //    document.getElementById("reportDateTicket").focus();
    //    self.reportDateStart(self.reportDate().startOf('day').format());
    //}

    self.btnCalendarOrder = function () {
        document.getElementById("reportDateOrder").focus();
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
        if (self.active() == 'reportOrderWaitSituation') {
            self.viewReportOrderWaitSituation();
        }
        else {
            self.viewReportTicketSituation();
        }


    }

    self.viewChartTicket = function (listDate, listTicket, listMoney) {
        $("#userTicketSituation").highcharts({
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
                categories: listDate,
                crosshair: true
            }],
            yAxis: [{ // Primary yAxis
                labels: {
                    format: '{value} Ticket',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                },
                title: {
                    text: 'Ticket number',
                    style: {
                        color: Highcharts.getOptions().colors[1]
                    }
                }
            }, { // Secondary yAxis
                title: {
                    text: 'Ticket',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value} ticket',
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
            series: [
                {
                    name: 'Ticket number',
                    type: 'column',
                    yAxis: 1,
                    data: listTicket,
                    tooltip: {
                        valueSuffix: ' ticket'
                    }

                },
                {
                    name: 'Amount to be refunded',
                    type: 'spline',
                    data: listMoney,
                    tooltip: {
                        valueSuffix: ' VNĐ'
                    },

                }
            ]
        });
    };
    // Thống kê ticket theo thời gian
    self.viewReportTicketSituation = function () {
        var start = self.SearchTicketModal().DateStart();
        var end = self.SearchTicketModal().DateEnd();
        self.totalTicket(0);
        if (self.contentChart() == 1) {
            $.post("/Ticket/GetTicketSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Ticket statistics are initialized over time");
            });
        }
        else if (self.contentChart() == 2) {
            $.post("/Ticket/GetTicketTypeSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Thống kê ticket theo loại ticket theo thời gian");
            });
        }
        else if (self.contentChart() == 3) {
            $.post("/Ticket/GetTicketVIPSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Thống kê ticket theo loại VIP khách hàng theo thời gian");
            });
        }
        else if (self.contentChart() == 4) {
            $.post("/Ticket/GetTicketSuccessSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Thống kê ticket đã xử lý xong theo thời gian");
            });
        }
        else if (self.contentChart() == 5) {
            $.post("/Ticket/GetTicketReceiveSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Ticket statistics that employees receive over time");
            });
        }
        else {
            $.post("/Ticket/GetTicketClaimSituation", { startDay: start, endDay: end }, function (data) {
                self.totalTicket(data.total);
                self.isLoading(true);
                self.viewChartTicket(data.user, data.totalTicket, data.TotalMoney);
                self.reportTitleTicket("Ticket statistics are the same amount of money paid over time");
            });
        }

    }

    self.contentChart.subscribe(function (newValue) {
        if (self.active() == 'reportTicketSituation')
        {
            self.viewReportTicketSituation();
        }
        else
        {
            self.viewReportOrderWaitSituation();
        }
    });

    // Thống kê Number orders of quotations theo đầu nhân viên

    self.viewReportOrderWaitSituation = function () {
        var start = self.SearchTicketModal().DateStart();
        var end = self.SearchTicketModal().DateEnd();
        self.totalOrderWait(0);
        if (self.contentChart() == 1)
        {
            self.reportTitleTicket("Statistics the number of quotes by staff");
            $.post("/Ticket/GetOrderWaitSituation", { startDay: start, endDay: end }, function (data) {
                self.reportOrderWaitCommon(data.Name, data.totalOrder);
                self.totalOrderWait(data.total);
            });
        }
        else if (self.contentChart() == 2)
        {
            self.reportTitleTicket("Statistics the number of quotation requests over time");
            $.post("/Ticket/GetOrderWaitSituationContinute", { startDay: start, endDay: end }, function (data) {
                self.reportOrderWaitCommon(data.Name, data.totalOrder);
                self.totalOrderWait(data.total);
            });
        }
        else if (self.contentChart() == 3) {
            self.reportTitleTicket("Statistics the number of guest quotes created during the day");
            $.post("/Ticket/GetOrderCustomerSituation", { startDay: start, endDay: end }, function (data) {
                self.reportOrderWaitLineCommon(data.Name, data.totalOrder);
                self.totalOrderWait(data.total);
            });
        }
        else if (self.contentChart() == 4) {
            self.reportTitleTicket("Statistics the number of single-quote orders created during the day");
            $.post("/Ticket/GetOrderUserSituation", { startDay: start, endDay: end }, function (data) {
                self.reportOrderWaitLineCommon(data.Name, data.totalOrder);
                self.totalOrderWait(data.total);
            });
        }
    }

    self.reportOrderWaitCommon = function (name, totalOrder) {
        $('#userOrderWaitSituation').highcharts({
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
                categories: name,
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
                    text: 'Quotation form',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value} quotation form',
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
            series: [
                {
                    name: 'Quotation form',
                    type: 'column',
                    yAxis: 1,
                    data: totalOrder,
                    tooltip: {
                        valueSuffix: ' quotation form'
                    }
                }]
        });
    }
    self.reportOrderWaitLineCommon = function (name, totalOrder) {
        $('#userOrderWaitSituation').highcharts({
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
                categories: name,
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
                    text: 'Quotation form',
                    style: {
                        color: Highcharts.getOptions().colors[0]
                    }
                },
                labels: {
                    format: '{value} Quotation form',
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
            series: [
                {
                    name: 'Quotation form',
                    type: 'line',
                    yAxis: 1,
                    data: totalOrder,
                    tooltip: {
                        valueSuffix: ' quotation form'
                    }
                }]
        });
    }
    //======================== Ham xuat File Excel====================================

    self.ExcelReportTicketSituation = function () {
        var start = self.reportDateStart();
        var end = self.reportDateEnd();
        $.redirect("/Ticket/ExcelTicketSituationReport", { startDay: start, endDay: end, contentReport: self.contentChart() }, "POST");
    }

    self.ExcelReportOrderWaitSituation = function () {
        var start = self.SearchTicketModal().DateStart();
        var end = self.SearchTicketModal().DateEnd();
        if (self.contentChart() == 1) {
            $.redirect("/Ticket/ExcelReportOrderWaitSituation", { startDay: start, endDay: end }, "POST");
        }
        else if(self.contentChart() == 2)
        {
            $.redirect("/Ticket/ExcelReportOrderWaitSituationContinute", { startDay: start, endDay: end }, "POST");
        }
        else if (self.contentChart() == 3) {
            $.redirect("/Ticket/ExcelReportOrderCustomerSituation", { startDay: start, endDay: end }, "POST");
        }
        else {
            $.redirect("/Ticket/ExcelReportOrderUserSituation", { startDay: start, endDay: end }, "POST");
        }
    }

    self.exportExcelComplain = function () {
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        $.redirect("/Ticket/ExportExcelComplain",
        {
            searchModal: SearchTicketModal,
            userId: self.userId,
            customerId: self.customerId
        },
        "POST");
    }
    self.exportExcelTicket = function () {
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        $.redirect("/Ticket/ExportExcelTicket",
        {
            searchModal: SearchTicketModal,
            userId: self.userId,
            customerId: self.customerId

        },
        "POST");
    }
    self.exportExcelTicketLate = function () {
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        $.redirect("/Ticket/ExportExcelTicketLate",
        {
            searchModal: SearchTicketModal,
            userId: self.userId,
            customerId: self.customerId
        },
        "POST");
    }
    self.exportExcelTicketSupport = function () {
        var SearchTicketModal = ko.mapping.toJS(self.SearchTicketModal());
        if (SearchTicketModal.Status === undefined) {
            SearchTicketModal.Status = -1;
        }
        $.redirect("/Ticket/ExportExcelTicketSupport",
        {
            searchModal: SearchTicketModal,
            userId: self.userId,
            customerId: self.customerId
        },
        "POST");
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

    //Hàm xuât danh sách Orders
    self.ExcelGetOrderCustomerCare = function () {
        orderWaitViewModel.ExcelGetOrderCustomerCare();
    }

    self.interval = function () {
        self.renderedHandler();
        var interval = setInterval(function () {
            self.renderedHandler();
            var objDiv = $("div.customercomplain");
            _.forEach(objDiv, function (value) {
                var height = value.scrollHeight;
                if (height >= 400) {
                    clearInterval(interval);
                }
            });
        }, 100);
    }

    self.renderedHandler = function () {
        var objDiv = $("div.customercomplain");
        _.forEach(objDiv, function (value) {
            objDiv.scrollTop(value.scrollHeight);
        });
    }

    //self.UpdateClaim = function () {
    //    $.post("/Ticket/UpdateClaim", { productId: 10517, orderType: 1 }, function (data) {
    //    });
    //}
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
    //Search don hang trong Tao khieu nai

    self.listOrderDetail = ko.observableArray([]);
    self.orderCodeCustomer.subscribe(function (newValue) {
        if (self.orderCodeCustomer() > 0 || self.orderCodeCustomer() != null || self.orderCodeCustomer() != "" || self.orderCodeCustomer() != undefined) {
            $.post("/Ticket/GetOrderDetail", { orderCodeCustomerId: self.orderCodeCustomer() }, function (data) {
                var list = [];
                var i = 1;
                _.each(data.listOrderDetail, function (item) {
                    item.Index = i;
                    item.NoteComplain = ko.observable();
                    list.push(item);
                    i++;
                })
                self.listOrderDetail(list);
            });
        }
    });

    self.SaveNoteComplain = function (data) {
        var isLook = false;
        $.post({
            url: "/Ticket/SaveNoteComplain",
            data: { complainOrderId: data.Id, note: data.NoteComplain },
            success: function (result) {
                if (result.status === msgType.error) {
                    toastr.error(result.msg);
                    isLook = false;
                } else {
                    isLook = true;
                    self.GetAllTicketComplainList();
                }
            },
            async: false
        });

        return isLook;
    };


};

// Bind PackageDetail
var packageDetailModelView = new PackageDetail();
ko.applyBindings(packageDetailModelView, $("#packageDetail")[0]);

var chatViewModel = new ChatViewModel();
ko.applyBindings(chatViewModel, $("#chatModal")[0]);
//var modelView = new supporterViewModel();
var orderDetailViewModel = new OrderDetailViewModel();
ko.applyBindings(orderDetailViewModel, $("#orderDetailModal")[0]);

var depositDetailViewModel = new DepositDetailViewModel();
ko.applyBindings(depositDetailViewModel, $("#orderDepositDetail")[0]);

var orderCommerceDetailViewModel = new OrderCommerceDetailViewModel();
ko.applyBindings(orderCommerceDetailViewModel, $("#orderCommerceDetailModal")[0]);

var accountantDetail = new CustomerLookUp();
ko.applyBindings(accountantDetail, $("#rechargeDetailModal")[0]);

var viewModel = new supporterViewModel(orderDetailViewModel, depositDetailViewModel, accountantDetail);

var orderWaitViewModel = new OrderWaitViewModel(orderDetailViewModel, chatViewModel, depositDetailViewModel, orderCommerceDetailViewModel);
orderWaitViewModel.viewBoxChat = new ChatViewModel();

viewModel.orderWaitViewModel = orderWaitViewModel;
ko.applyBindings(viewModel, $("#supporterView")[0]);
//ko.applyBindings(modelView);