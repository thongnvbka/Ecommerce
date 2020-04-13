function DashboardViewModel() {
    var self = this;
    self.isLoading = ko.observable(false);

    //==========================
    self.totalCustomer = ko.observable(0);
    self.totalPotentialCustomer = ko.observable(0);
    self.totalStaff = ko.observable(0);

    self.totalFundBillVNAddOfDay = ko.observable(0);
    self.totalFundBillVNMinusOfDay = ko.observable(0);
    self.totalRechargeBillAddOfDay = ko.observable(0);
    self.totalRechargeBillMinusOfDay = ko.observable(0);
    self.totalFundBillCNAddOfDay = ko.observable(0);
    self.totalFundBillCNMinusOfDay = ko.observable(0);
    self.totalFundBillALPAddOfDay = ko.observable(0);
    self.totalFundBillALPMinusOfDay = ko.observable(0);
    
    self.startDay = ko.observable();
    self.endDay = ko.observable();

    //Doanh thu Orders Order
    self.expectedRevenueOrder = ko.observable(0);
    self.expectedProfitOrder = ko.observable(0);

    self.expectedRevenueDeposit = ko.observable(0);
    self.expectedProfitDeposit = ko.observable(0);

    self.expectedRevenueSourcing = ko.observable(0);
    self.expectedProfitSourcing = ko.observable(0);

    self.expectedRevenueCommerce = ko.observable(0);
    self.expectedProfitCommerce = ko.observable(0);
    //============================================================= Báo cáo ====================================================
    self.viewReportOrder = function () {
        $.post("/Dashboard/ReportOrderOfDay", { day: self.startDay(), end: self.endDay() }, function (data) {
            self.isLoading(true);
            self.expectedRevenueOrder(data.expectedRevenueOrder);
            self.expectedProfitOrder(data.expectedProfitOrder);

            $('#order').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '<b>{point.name}</b>: {point.y} order.'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.y} order.',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: 'Number: ',
                    colorByPoint: true,
                    data: data.overview
                }]
            });

            $('#staff_order').highcharts({
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
                        format: '{value} Baht',
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
                    data: data.detailOrder,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revenue',
                    type: 'spline',
                    data: data.detailPrice,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }
                }]
            });
        });
    }

    self.viewReportCommerce = function () {
        $.post("/Dashboard/ReportCommerceOfDay", { day: self.startDay(), end: self.endDay() }, function (data) {
            self.isLoading(true);
            self.expectedRevenueCommerce(data.expectedRevenueOrder);
            self.expectedProfitCommerce(data.expectedProfitOrder);

            $('#commerce').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '<b>{point.name}</b>: {point.y} order.'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.y} order.',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: 'Number: ',
                    colorByPoint: true,
                    data: data.overview
                }]
            });

            $('#staff_commerce').highcharts({
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
                        format: '{value} Baht',
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
                    data: data.detailOrder,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revene',
                    type: 'spline',
                    data: data.detailPrice,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }
                }]
            });
        });
    }

    self.viewReportDeposit = function () {
        $.post("/Dashboard/ReportDepositOfDay", { day: self.startDay(), end: self.endDay() }, function (data) {
            self.isLoading(true);

            self.expectedRevenueDeposit(data.expectedRevenueDeposit);
            self.expectedProfitDeposit(data.expectedProfitDeposit);

            $('#deposit').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '<b>{point.name}</b>: {point.y} order.'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.y} order.',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: 'Number: ',
                    colorByPoint: true,
                    data: data.overview
                }]
            });

            $('#staff_deposit').highcharts({
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
                        format: '{value} Baht',
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
                    data: data.detailDeposit,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revenue',
                    type: 'spline',
                    data: data.detailPrice,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }
                }]
            });
        });
    }

    self.viewReportSourcing = function () {
        $.post("/Dashboard/ReportSourcingOfDay", { day: self.startDay(), end: self.endDay() }, function (data) {
            self.isLoading(true);
            self.expectedRevenueSourcing(data.expectedRevenueSourcing);
            self.expectedProfitSourcing(data.expectedProfitSourcing);

            $('#sourcing').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '<b>{point.name}</b>: {point.y} order.'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.y} order.',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: 'Number: ',
                    colorByPoint: true,
                    data: data.overview
                }]
            });

            $('#staff_sourcing').highcharts({
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
                        format: '{value} Baht',
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
                    data: data.detailSourcing,
                    tooltip: {
                        valueSuffix: ' order'
                    }

                }, {
                    name: 'Total revenue',
                    type: 'spline',
                    data: data.detailPrice,
                    tooltip: {
                        valueSuffix: ' Baht'
                    }
                }]
            });
        });
    }

    self.viewReportUser = function () {
        $.post("/Dashboard/ReportUser", { },
            function (data) {
                self.totalCustomer(data.totalCustomer);
                self.totalPotentialCustomer(data.totalPotentialCustomer);
                self.totalStaff(data.totalStaff);
            });
    }

    self.viewReportAccountant = function () {
        $.post("/Dashboard/ReportAccountantOfDay", { day: self.startDay(), end: self.endDay() },
            function (data) {
                self.totalFundBillVNAddOfDay(data.totalFundBillVNAddOfDay);
                self.totalFundBillVNMinusOfDay(data.totalFundBillVNMinusOfDay);
                self.totalRechargeBillAddOfDay(data.totalRechargeBillAddOfDay);
                self.totalRechargeBillMinusOfDay(data.totalRechargeBillMinusOfDay);
                self.totalFundBillCNAddOfDay(data.totalFundBillCNAddOfDay);
                self.totalFundBillCNMinusOfDay(data.totalFundBillCNMinusOfDay);
                self.totalFundBillALPAddOfDay(data.totalFundBillALPAddOfDay);
                self.totalFundBillALPMinusOfDay(data.totalFundBillALPMinusOfDay);
            });
    }


    self.viewReportTicket = function () {
        $.post("/Dashboard/ReportTicketOfDay", { day: self.startDay(), end: self.endDay() }, function (data) {
            self.isLoading(true);
            //self.expectedRevenueOrder(data.expectedRevenueOrder);
            //self.expectedProfitOrder(data.expectedProfitOrder);

            $('#ticket').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                tooltip: {
                    pointFormat: '<b>{point.name}</b>: {point.y} ticket.'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.y} ticket.',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                },
                series: [{
                    name: 'Number: ',
                    colorByPoint: true,
                    data: data.overview
                }]
            });

            $('#staff_ticket').highcharts({
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
                        format: '{value} ticket',
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: 'Total tickets',
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
                series: [{
                    name: 'Ticket',
                    type: 'column',
                    yAxis: 1,
                    data: data.detailTicket,
                    tooltip: {
                        valueSuffix: ' ticket'
                    }
                }]
            });
        });
    }

    //============================================================= khởi tạo ====================================================
    //Hàm khởi tạo khi load trang
    $(function () {
        self.viewReportUser();
        self.viewReportAccountant();

        self.viewReportOrder();
        self.viewReportCommerce();
        self.viewReportDeposit();
        self.viewReportSourcing();
        self.viewReportTicket();

        self.initInputMark();
        $('#daterange-btndashboard').daterangepicker(
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
                     'This week': [moment().startOf('month'), moment().endOf('month')],
                     'This month': [moment().subtract(0, 'month').startOf('month'), moment().subtract(0, 'month').endOf('month')]
                 },
                 startDate: moment().subtract(29, 'days'),
                 endDate: moment()
             },
             function (start, end) {
                 if (start.format() === 'Invalid date') {
                     $('#daterange-btndashboard span').html('Created date');
                     self.startDay('');
                     self.endDay('');
                 }
                 else {
                     $('#daterange-btndashboard span').html(start.format('DD/MM/YYYY') + ' - ' + end.format('DD/MM/YYYY'));
                     self.startDay(start.format());
                     self.endDay(end.format());
                     self.viewReportAccountant();

                     self.viewReportOrder();
                     self.viewReportCommerce();
                     self.viewReportDeposit();
                     self.viewReportSourcing();
                     self.viewReportTicket();
                 }
             }
         );
        $('#daterange-btndashboard').on('cancel.daterangepicker', function (ev, picker) {
            //do something, like clearing an input
            $('#daterange-btndashboard span').html('Created date');
            self.startDay('');
            self.endDay('');
        });

    });


    //============================================================= Các hàm hỗ trợ ==============================================
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
    }
};

var dashboardViewModel = new DashboardViewModel();
ko.applyBindings(dashboardViewModel, $("#dashboard")[0]);