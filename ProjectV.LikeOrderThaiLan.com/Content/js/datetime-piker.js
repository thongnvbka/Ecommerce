//start datetimepicker
jQuery(function () {
    jQuery("#date_timepicker_start").datetimepicker({
        format: "d/m/Y",
        onShow: function (ct) {
            this.setOptions({
                maxDate: jQuery("#date_timepicker_end").val() ? jQuery("#date_timepicker_end").val() : false,
                formatDate: 'd/m/Y'
            });
        },
        timepicker: false
    });
    jQuery("#date_timepicker_end").datetimepicker({
        format: "d/m/Y",
        onShow: function (ct) {
            this.setOptions({
                minDate: jQuery("#date_timepicker_start").val() ? jQuery("#date_timepicker_start").val() : false,
                formatDate: 'd/m/Y'
            });
        },
        timepicker: false
    });

    jQuery("#Birthday").datetimepicker({
        format: "d/m/Y", 
        timepicker: false
    });
});
//start datetimepicker