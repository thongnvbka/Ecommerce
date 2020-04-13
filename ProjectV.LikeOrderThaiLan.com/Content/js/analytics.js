//tab chart oderstatics
$(function () {
    Highcharts.chart('container', {
        chart: {
            type: 'area'
        },
        title: {
            text: 'Area chart with negative values'
        },
        xAxis: {
            categories: ['Đơn hàng mới tạo', 'Đơn đang chờ xử lý', 'Đơn hàng chưa giao', 'Đơn hàng hoàn thành']
        },
        credits: {
            enabled: false
        },
        series: [{
            name: '7 ngày',
            data: [1349, 125, 549, 89]
        }, {
            name: 'Tuần trước',
            data: [2643, 545, 234, 312]
        }, {
            name: 'Tháng trước',
            data: [4387, 1025, 679, 709]
        },
        {
            name: '3 tháng',
            data: [12987, 5005, 6087, 2012]
        }
        ]
    });
});
