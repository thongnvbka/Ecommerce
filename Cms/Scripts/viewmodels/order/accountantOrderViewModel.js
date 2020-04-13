var total = 0;
var page = 1;
var pagesize = 10;
var pageTotal = 0;

function AccountantOrderViewModel(orderDetailViewModel) {
    var self = this;

    //============================================================= Tìm kiếm ===================================================
    //các biến tìm kiếm
    self.keyword = ko.observable('');
    self.status = ko.observable();
    self.systemId = ko.observable();
    self.dateStart = ko.observable();
    self.dateEnd = ko.observable();
    self.userId = ko.observable();
    self.customerId = ko.observable();

    self.listStatus = ko.observableArray([]);
    self.listSystem = ko.observableArray([]);
    self.listSystemRender = ko.observableArray([]);
    self.listOrder = ko.observableArray([]);
    self.listUser = ko.observableArray([]);

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

    self.subjectId = ko.observable();
    self.subjectName = ko.observable();
    self.subjectTypeName = ko.observable();
    self.subjectEmail = ko.observable();
    self.subjectPhone = ko.observable();
    self.subjectAddress = ko.observable();

    self.treasureId = ko.observable();
    self.treasureName = ko.observable();

    self.currencyFluctuations = ko.observable();
    self.note = ko.observable();
    self.totalPrice = ko.observable(0);

    self.isRending = ko.observable(false);
    //các biến cho submit form
    self.isSubmit = ko.observable(true);

    //Hàm tìm kiếm 
    self.search = function (page) {
        window.page = page;

        self.isSubmit(true);
        self.isRending(false);
        $.post("/Purchase/Accountant", { page: page, pageSize: pagesize, keyword: self.keyword(), status: self.status() == undefined ? -1 : self.status(), systemId: self.systemId(), userId: self.userId() == undefined ? -1 : self.userId(), customerId: self.customerId() == undefined ? -1 : self.customerId(), dateStart: self.dateStart(), dateEnd: self.dateEnd() }, function (data) {
            total = data.totalRecord;
            self.totalPrice(data.totalPrice);
            self.listOrder(data.listOrder);
            self.paging();
            self.isRending(true);
        });
    };

    //Hàm chọn tab để tìm kiếm
    self.clickSearch = function (data, event) {
        $('#nav-tab' + self.systemId()).click();
    }

    self.clickTab = function (tab) {
        self.systemId(tab);
        self.search(1);
        $(".select-view").select2();
    }

    //Hàm load lại dữ liệu trên các tab
    self.renderSystem = function () {
        self.listSystemRender([]);
        self.listStatus([]);
        $.post("/Purchase/GetRenderSystem", { active: 'accountantOrder' }, function (data) {
            self.listStatus(data.listStatus);
            self.listSystem(data.listSystem);
            self.listSystemRender(data.listSystem);
            $('.nav-tabs').tabdrop();
            $(".select-view").select2();

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
                $('#daterange-btn span').html('Create Date');
                self.dateStart(null);
                self.dateEnd(null);
            });

            self.searchCustomer();
            self.searchUser();

            self.search(1);
        });
    }

    //Thanh toán Contract
    var onCheck = false;
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
                    $('#moneyFundAddOrEditOrder').modal('hide');
                }
            });
    };

    //Hủy thanh toán Contract
    self.cannalContractCode = function (data) {
        swal({
            title: 'Are you sure to go for re-examination order?',
            text: 'Contract code #"' + data.ContractCode + '"',
            type: 'warning',
            showCancelButton: true,
           cancelButtonText: 'Cancel',
            confirmButtonText: 'Tranfer'
        }).then(function () {
            //toastr.success("successful");
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
    self.showPaymentContractCode = function (data) {
        self.contractCodeIdSubmit(data.Id);
        self.contractCodeStatusSubmit(data.Status);
        //self.accountantSubjectId('');
        self.financeFundId('');
        self.financeFundName('');
        self.financeFundBankAccountNumber('');
        self.financeFundDepartment('');
        self.financeFundNameBank('');
        self.financeFundUserFullName('');
        self.financeFundUserPhone('');
        self.financeFundUserEmail('');
        //self.treasureId('');
        //self.treasureName('');
        self.currencyFluctuations('');
                self.subjectId('');
                self.subjectName('');
                self.subjectTypeName('');
                self.subjectEmail('');
                self.subjectPhone('');
                self.subjectAddress('');

        self.currencyFluctuations(formatNumberic(data.TotalPrice, 'N2'));
        $.post("/Purchase/GetAccountant", {orderId: data.OrderId}, function (result) {
            $('#moneyFundAddOrEditOrder').modal();
               
                self.subjectId(result.subject.Id);
                self.subjectName(result.subject.FullName);
                self.subjectTypeName(result.subject.TypeName);
                self.subjectEmail(result.subject.Email);
                self.subjectPhone(result.subject.Phone);
                self.subjectAddress(result.subject.Address);
                $('#financeFund_order_tree .dropdownjstree').remove();
                $("#financeFund_order_tree")
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
            //$('#treasure_order_tree').dropdownjstree('selectNode', 0);
            //$('#financeFund_order_tree').dropdownjstree('selectNode', 0);
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
                language: 'en'
            });
    };

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
}