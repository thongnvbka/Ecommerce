/* global jQuery */
; (function ($) {
    var time = new Date().getTime();
    var containerId = "#hhContainer-" + time,
        contentViewer = "#hhContentViewer-" + time,
        hhViewerName = "#hhViewerName-" + time,

		defaults = {
		    isShowThumbView: true,
		    isShowDelete: false,
		    onImageShow: null,
		    onDelete: null
		};

    function HHSlider(element, options) {
        this.element = element;
        this.options = $.extend({}, defaults, options);

        this._defaults = defaults;
        this.init(element, this.options);
        this.initEvent(element, this.options);
    };

    HHSlider.prototype.init = function (element, options) {
        // Set index for item
        setItemIndex(element);

        // Create options
        createOptions(element, options);

        // Create overlay
        createMainSlider();

        // Create list thumbnail
        if (options.isShowThumbView) {
            createThumbnailView(element, options);
        }
    };

    HHSlider.prototype.initEvent = function (element, options) {
        eventHhItemClick();
        eventHhDeleteClick(options);
        eventThumbnailClick();
        eventShowHideThumbnail();
        eventClose();
        eventPrevious(element);
        eventNext(element);
        eventKeydown();
    };

    function refresh(element) {
        var options = $(element).data().hhSlider.options;

        createOptions(element, options);
        destroyThumbnailView();
        createThumbnailView(element, options);

    };

    function createMainSlider() {
        if ($(containerId).length === 0) {
            var _html = "<div id='" + containerId.replace("#", "") + "' class='hhContainer'>\
			<ul class='wrapper-hh-option-button'><li class='hh-close-button'><i class='fa fa-times'></i></li></ul>\
			<div class='hh-previous'><i class='fa fa-arrow-left'></i></div>\
            <div id='"+ hhViewerName.replace("#", "") + "' class=''></div>\
			<div id='"+ contentViewer.replace("#", "") + "' class='contentViewer'></div>\
			<div class='hh-next'><i class='fa fa-arrow-right'></i></div>\
			</div>";

            $("body").append(_html);
        }
    };

    function createThumbnailView(element, options) {
        if ($(containerId + " .hh-thumbnail-view").length === 0) {
            var _listThumbItem = createListThumbnail(element);
            var _html = "<div class='hh-wrapper-thumbnail-view'><div class='hh-thumbnail-view'><a href='javascript://' class='button-show-hide' data-show='true'><i class='fa fa-bars '></i></a>{content}</div></div>";
            _html = _html.replace(/{content}/ig, _listThumbItem);

            $(containerId).append(_html);
        } else {
            destroyThumbnailView();

            var _listThumbItem = createListThumbnail(element);
            var _html = "<div class='hh-wrapper-thumbnail-view'><div class='hh-thumbnail-view'><a href='javascript://' class='button-show-hide' data-show='true'><i class='fa fa-bars '></i></a>{content}</div></div>";
            _html = _html.replace(/{content}/ig, _listThumbItem);

            $(containerId).append(_html);
        }
    };

    function destroyThumbnailView() {
        $(".hh-thumbnail-view").remove();
    };

    function createListThumbnail(element) {
        var _listItem = $(element).find('.hh-item');
        var _thumbnailHtml = "<ul class='wrapper-list-thumbnail'>";

        $.each(_listItem, function (index, item) {
            var _thumbSrc = $(item).attr("data-thumb");
            var _src = $(item).attr("data-src");
            var _name = $(item).attr("data-name");
            var _isShowDelete = $(item).attr("data-show-delete");

            _thumbnailHtml += "<li data-index='" + index + "' class='thumb-item'><a href='javascript://' data-name='" + _name + "' data-index='" + index + "' data-src='" + _src + "'><img src='" + _thumbSrc + "'/></a></li>";
        });

        _thumbnailHtml += "</ul>";

        return _thumbnailHtml;
    };

    function setThumbnailActive(index) {
        $(".wrapper-list-thumbnail li").removeClass("active");
        $(".wrapper-list-thumbnail li[data-index='" + index + "']").addClass("active");
    };

    function setContentViewSource(src, index, name) {
        if ($(contentViewer).children().length === 0) {
            $(contentViewer).html("<img data-index='" + index + "' style='position: absolute; top: ' src='" + src + "'/>");
            setTimeout(function () {
                calculateImageViewPosition();
            }, 200);
        } else {
            $(contentViewer).children().fadeOut(500, function () {
                $(contentViewer).children().attr({ "src": src, "data-index": index });
                $(this).fadeIn(500);
                setTimeout(function () {
                    calculateImageViewPosition();
                }, 200);
            });
        }

        $(hhViewerName).html(name);
    };

    function createOptions(element, options) {
        var listItem = $(element).find(".hh-item");

        $.each(listItem, function (index, item) {
            var _isShowDelete = $(item).attr("data-show-delete");

            var _deleteItem = options.isShowDelete || _isShowDelete ? '<li><a href="javascript://" class="delete" data-id={id}><i class="fa fa-times"></i></a></li>' : '';

            var _options = '<ul class="options">' +
                                '<li><a href="javascript://" class="show" data-index="{index}" data-src="{src}" data-name="{name}"><i class="fa fa-search"></i></a></li>' +
                                _deleteItem +
            '</ul>';

            var $item = $(item);
            var _id = $item.attr("data-id");
            var _src = $item.attr("data-src");
            var _name = $item.attr("data-name");

            if ($item.find("ul.options").length == 0) {
                _options = _options.replace(/{id}/ig, _id);
                _options = _options.replace(/{index}/ig, index);
                _options = _options.replace(/{src}/ig, _src);
                _options = _options.replace(/{name}/ig, _name);

                $item.append(_options);
            }
        });
    };

    function setItemIndex(element) {
        var listItem = $(element).find(".hh-item");
        $.each(listItem, function (index, item) {
            $(item).attr("data-index", index);
        });
    };

    function eventThumbnailClick() {
        $(document).on("click", ".thumb-item a", function () {
            var t = $(this);

            if (t.parent().hasClass("active"))
                return;

            $(".thumb-item").removeClass("active");
            $(this).parent().addClass("active");

            var _src = $(this).attr("data-src");
            var _index = $(this).attr("data-index");

            setContentViewSource(_src, _index);
        });
    };

    function eventShowHideThumbnail() {
        $("a.button-show-hide").on("click", function () {
            var _wrapperThumbElement = $(".hh-wrapper-thumbnail-view");
            var _isShow = $(this).attr("data-show");

            var _thumbnailHeight = _wrapperThumbElement.outerHeight();
            if (_isShow == "true") {
                _wrapperThumbElement.animate({ "bottom": -_thumbnailHeight }, 500);
                $(this).attr("data-show", false);
            } else {
                _wrapperThumbElement.animate({ "bottom": 0 }, 500);
                $(this).attr("data-show", true);
            }
        });
    };

    function eventClose() {
        $(".hh-close-button").on("click", function () {
            $(containerId).css("display", "none");
        });
    };

    function eventPrevious(element) {
        // Previous
        $(".hh-previous").on("click", function () {
            var _index = $(contentViewer + " img").attr("data-index");
            var _hhSliderLength = $(element).find(".hh-item").length;

            var _nextIndex = parseInt(_index) - 1;
            if (_nextIndex < 0) {
                _nextIndex = _hhSliderLength - 1;
            }
            var _selectedElement = $(element).find("[data-index='" + _nextIndex + "']");
            var _src = _selectedElement.attr("data-src");
            var _name = _selectedElement.attr("data-name");

            setThumbnailActive(_nextIndex);
            setContentViewSource(_src, _nextIndex, _name);
        });
    };

    function eventNext(element) {
        // Next
        $(".hh-next").on("click", function () {
            var _index = $(contentViewer + " img").attr("data-index");
            var _hhSliderLength = $(element).find(".hh-item").length;

            var _nextIndex = parseInt(_index) + 1;
            if (_nextIndex >= _hhSliderLength) {
                _nextIndex = 0;
            }

            var _selectedElement = $(element).find("[data-index='" + _nextIndex + "']");
            var _src = _selectedElement.attr("data-src");
            var _name = _selectedElement.attr("data-name");

            setThumbnailActive(_nextIndex);
            setContentViewSource(_src, _nextIndex, _name);
        });
    };

    function eventHhItemClick() {
        $(document).on("click", ".hh-item .show", function () {
            $(containerId).css("display", "block");

            var _index = $(this).attr("data-index");
            var _src = $(this).attr("data-src");
            var _name = $(this).attr("data-name");

            setThumbnailActive(_index);
            setContentViewSource(_src, _index, _name);
        });
    };

    function eventHhDeleteClick(options) {
        $(document).on("click", ".hh-item .delete", function () {
            var _id = $(this).attr("data-id");
            if ($.isFunction(options.onDelete)) {
                options.onDelete(_id, this);
            }
        });
    };

    function eventHhImageShow(options) {
        $($(contentViewer) + " .hh-item .show").on("click", function () {
            var _id = $(this).attr("data-id");
            if ($.isFunction(options.onImageShow())) {
                options.onImageShow(_id);
            }
        });
    };

    function calculateImageViewPosition() {
        var $contentViewer = $(contentViewer);
        var $img = $contentViewer.children("img");
        var contentViewerHeight = $contentViewer.outerHeight();
        var imageWidth = $img.outerWidth();        
        var imageHeight = $img.outerHeight();
        var windowWidth = $(window).outerWidth();

        if (imageWidth >= windowWidth) {
            $img.css("width", "100%");
        }

        var space = contentViewerHeight - imageHeight;
        var top = space / 2;

        $img.css("top", top);
    };

    function eventKeydown() {
        $(window).on("keydown", function (e) {
            // Right
            if (e.keyCode === 39) {
                $(".hh-next").trigger("click");
            }

            // Left
            if (e.keyCode === 37) {
                $(".hh-previous").trigger("click");
            }

            // Up
            if (e.keyCode === 38) {

            }

            // Down
            if (e.keyCode === 40) {

            }

            // Escape
            if (e.keyCode === 27) {
                $(".hh-close-button").trigger("click");
                return false;
            }
        });
    };

    $.fn.hhSlider = function (options) {
        if ((typeof options) === "object" || !options) {
            return this.each(function () {
                if (!$.data(this, "hhSlider")) {
                    $.data(this, "hhSlider", new HHSlider(this, options));
                } else {
                    new refresh(this);
                }
            });
        } else {            
            if (options === "refresh") {                
                new refresh(this);
            }
        }
    }
})(jQuery);