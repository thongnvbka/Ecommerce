var exportwarehouseViewModel = function () {
    var self = this;

    self.active = ko.observable('importwarehouse');
    self.templateId = ko.observable('importwarehouse');

    self.totalCustomerList = ko.observable();

    ///==================== Các biến cho loading ======================================
    self.isLoading = ko.observable(false);
    self.isRending = ko.observable(false);
    self.isDetailRending = ko.observable(false);



    //==================== Các sự kiện xử lý ==========================================
    self.viewExportWarehouseDetail = function (data) {
        $('#ExportWarehouseDetailModal').modal();
    }


    //==================== Báo cáo ====================================================
    self.viewReport = function () {
        $.post("/Purchase/GetOrderReport", function (data) {
            self.isLoading(false);
            $('#importwarehouse').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Weekly goods dispatch report'
                },
                subtitle: {
                    //text: 'Source: WorldClimate.com'
                },
                xAxis: {
                    categories: [
                        '01/09/2016',
                        '02/09/2016',
                        '03/09/2016',
                        '04/09/2016',
                        '05/09/2016',
                        '06/09/2016',
                        '07/09/2016',
                        '08/09/2016',
                        '09/09/2016',
                        '10/09/2016',
                        '11/09/2016',
                        '12/09/2016'
                    ],
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Rainfall (mm)'
                    }
                },
                tooltip: {
                    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                        '<td style="padding:0"><b>{point.y:.1f} package</b></td></tr>',
                    footerFormat: '</table>',
                    shared: true,
                    useHTML: true
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: [{
                    name: ' TQ warehouse',
                    data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4]

                }, {
                        name: ' Nội Bài warehouse',
                    data: [83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3]

                }, {
                        name: ' Lò Đúc warehouse',
                    data: [48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2]

                }, {
                    name: 'Đội Cấn warehouse',
                    data: [42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1]
                }]
            });
        });
    }

    //==================== Khởi tạo ===================================================
    self.init = function () {
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

    $(function () {
        self.init();
        self.viewReport();
    });
};

ko.applyBindings(new exportwarehouseViewModel());