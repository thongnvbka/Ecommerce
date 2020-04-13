/*
* jQuery pager plugin
* Version 1.0 (12/22/2008)
* @requires jQuery v1.2.6 or later
*
* Example at: http://jonpauldavies.github.com/JQuery/Pager/PagerDemo.html
*
* Copyright (c) 2008-2009 Jon Paul Davies
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
* 
* Read the related blog post and contact the author at http://www.j-dee.com/2008/12/22/jquery-pager-plugin/
*
* This version is far from perfect and doesn't manage it's own state, therefore contributions are more than welcome!
*
* Usage: .pager({ pagenumber: 1, pagecount: 15, buttonClickCallback: PagerClickTest });
*
* Where pagenumber is the visible page number
*       pagecount is the total number of pages to display
*       buttonClickCallback is the method to fire when a pager button is clicked.
*
* buttonClickCallback signiture is PagerClickTest = function(pageclickednumber) 
* Where pageclickednumber is the number of the page clicked in the control.
*
* The included Pager.CSS file is a dependancy but can obviously tweaked to your wishes
* Tested in IE6 IE7 Firefox & Safari. Any browser strangeness, please report.
*/
(function ($) {

    $.fn.pager = function (options) {
        var opts = $.extend({}, $.fn.pager.defaults, options);

        return this.each(function () {

            // empty out the destination element and then render out the pager with the supplied options
            // @HenryDo: Truyền thêm type để xác định trong trường hợp có nhiều pager trên 1 trang
            if (options.type == undefined) {
                $(this).empty().append(renderpager(parseInt(options.pagenumber), parseInt(options.pagecount), options.buttonClickCallback));
            } else {
                $(this).empty().append(renderpagerWidthType(parseInt(options.pagenumber), parseInt(options.pagecount), options.buttonClickCallback, options.type));
            }

            // specify correct cursor activity
            $('.pages li').mouseover(function () { document.body.style.cursor = "pointer"; }).mouseout(function () { document.body.style.cursor = "auto"; });
        });
    };

    // @HenryDo
    $.fn.pagerNextBackOnly = function (options) {
        var opts = $.extend({}, $.fn.pager.defaults, options);

        return this.each(function () {

            // empty out the destination element and then render out the pager with the supplied options
            $(this).empty().append(renderpagerNextBackOnly(parseInt(options.pagenumber), parseInt(options.pagecount), options.buttonClickCallback));

            // specify correct cursor activity
            $('.pages li').mouseover(function () { document.body.style.cursor = "pointer"; }).mouseout(function () { document.body.style.cursor = "auto"; });
        });
    };

    // render and return the pager with the supplied options
    function renderpager(pagenumber, pagecount, buttonClickCallback) {
        if (pagecount < 2)
            return '';

        // setup $pager to hold render
        var $pager = $('<ul class="pagination"></ul>');

        // add in the previous and next buttons
        if (pagenumber > 1) {
            $pager.append(renderButton('first', '««', pagenumber, pagecount, buttonClickCallback)).append(renderButton('prev', '«', pagenumber, pagecount, buttonClickCallback));
        }

        // pager currently only handles 10 viewable pages ( could be easily parameterized, maybe in next version ) so handle edge cases
        var startPoint = (pagecount < 6) ? 1 : ((pagenumber > 5) ? pagenumber - 4 : 1);
        var endPoint = (pagecount < 6) ? pagecount : ((pagenumber > 5) ? pagenumber : 5);

        // loop thru visible pages and render buttons
        for (var page = startPoint; page <= endPoint; page++) {
            //var $li = $("<li></li>");
            //$li.appendTo($pager);
            if (page == pagenumber) {

                var currentButton = $('<li class="active"><a>' + page + '</a></li>');
                currentButton.appendTo($pager);
            }
            else {
                var $li = $("<li></li>");
                $li.appendTo($pager);
                var currentButton = $('<a href="javascript:void(0);">' + page + '</a>');

                currentButton.click(function () { buttonClickCallback(this.firstChild.data); });
                currentButton.appendTo($li);
            }
        }

        // render in the next and last buttons before returning the whole rendered control back.
        if (pagenumber < pagecount) {
            $pager.append(renderButton('next', '»', pagenumber, pagecount, buttonClickCallback)).append(renderButton('last', '»»', pagenumber, pagecount, buttonClickCallback));
        }

        return $pager;
    }

    // @HenryDo: render page with next and back button only.
    // render and return the pager with the supplied options
    function renderpagerWidthType(pagenumber, pagecount, buttonClickCallback, type) {
        if (pagecount < 2)
            return '';

        // setup $pager to hold render
        var $pager = $('<ul class="pagination"></ul>');

        // add in the previous and next buttons
        if (pagenumber > 1) {
            $pager.append(renderButton('first', '««', pagenumber, pagecount, buttonClickCallback)).append(renderButton('prev', '«', pagenumber, pagecount, buttonClickCallback));
        }

        // pager currently only handles 10 viewable pages ( could be easily parameterized, maybe in next version ) so handle edge cases
        var startPoint = (pagecount < 6) ? 1 : ((pagenumber > 5) ? pagenumber - 4 : 1);
        var endPoint = (pagecount < 6) ? pagecount : ((pagenumber > 5) ? pagenumber : 5);

        // loop thru visible pages and render buttons
        for (var page = startPoint; page <= endPoint; page++) {
            //var $li = $("<li></li>");
            //$li.appendTo($pager);
            if (page == pagenumber) {

                var currentButton = $('<li class="active"><a>' + page + '</a></li>');
                currentButton.appendTo($pager);
            }
            else {
                var $li = $("<li></li>");
                $li.appendTo($pager);
                var currentButton = $('<a href="javascript:void(0);">' + page + '</a>');

                currentButton.click(function () { buttonClickCallback(this.firstChild.data, type); });
                currentButton.appendTo($li);
            }
        }

        // render in the next and last buttons before returning the whole rendered control back.
        if (pagenumber < pagecount) {
            $pager.append(renderButton('next', '»', pagenumber, pagecount, buttonClickCallback)).append(renderButton('last', '»»', pagenumber, pagecount, buttonClickCallback));
        }

        return $pager;
    }

    function renderpagerNextBackOnly(pagenumber, pagecount, buttonClickCallback) {
        // setup $pager to hold render
        var $pager = $('<ul class="pagination"></ul>');

        // add in the previous and next buttons
        $pager.append(renderButtonNextBackOnly('first', '<i class="fa fa-liên-backward"></i>', pagenumber, pagecount, pagenumber === 1 ? function () { return; } : buttonClickCallback, pagenumber === 1 ? "disabled" : ""))
            .append(renderButtonNextBackOnly('prev', '<i class="fa fa-play fa-flip-horizontal"></i>', pagenumber, pagecount, pagenumber === 1 ? function () { return; } : buttonClickCallback, pagenumber === 1 ? "disabled" : ""));

        // render in the next and last buttons before returning the whole rendered control back.        
        $pager.append(renderButtonNextBackOnly('next', '<i class="fa fa-play"></i>', pagenumber, pagecount, pagenumber >= pagecount ? function () { return; } : buttonClickCallback, pagenumber >= pagecount ? "disabled" : ""))
            .append(renderButtonNextBackOnly('last', '<i class="fa fa-liên-forward"></i>', pagenumber, pagecount, pagenumber >= pagecount ? function () { return; } : buttonClickCallback, pagenumber >= pagecount ? "disabled" : ""));

        return $pager;
    }

    // renders and returns a 'specialized' button, ie 'next', 'previous' etc. rather than a page number button
    function renderButton(buttonName, buttonLabel, pagenumber, pagecount, buttonClickCallback) {

        if (buttonName == "next" || buttonName == "prev")
            var $Button = $('<li> <a href="javascript:void(0);">' + buttonLabel + '</a> </li>');
        else
            var $Button = $('<li> <a href="javascript:void(0);">' + buttonLabel + '</a> </li>');

        var destPage = 1;

        // work out destination page for required button type
        switch (buttonName) {
            case "first":
                destPage = 1;
                break;
            case "prev":
                destPage = pagenumber - 1;
                break;
            case "next":
                destPage = pagenumber + 1;
                break;
            case "last":
                destPage = pagecount;
                break;
        }

        $Button.click(function () { buttonClickCallback(destPage); });

        return $Button;
    }

    function renderButtonNextBackOnly(buttonName, buttonLabel, pagenumber, pagecount, buttonClickCallback, className) {

        if (buttonName == "next" || buttonName == "prev")
            var $Button = $('<li> <a href="javascript:void(0);" class="' + className + '">' + buttonLabel + '</a> </li>');
        else
            var $Button = $('<li> <a href="javascript:void(0);" class="' + className + '">' + buttonLabel + '</a> </li>');

        var destPage = 1;

        // work out destination page for required button type
        switch (buttonName) {
            case "first":
                destPage = 1;
                break;
            case "prev":
                destPage = pagenumber - 1;
                break;
            case "next":
                destPage = pagenumber + 1;
                break;
            case "last":
                destPage = pagecount;
                break;
        }

        $Button.click(function () { buttonClickCallback(destPage); });

        return $Button;
    }

    // pager defaults. hardly worth bothering with in this case but used as placeholder for expansion in the next version
    $.fn.pager.defaults = {
        pagenumber: 1,
        pagecount: 1
    };

})(jQuery);
