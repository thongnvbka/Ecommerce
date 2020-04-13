(function ($) {
    if (typeof ($) === 'undefined') {
        throw 'Thư viện này sử dụng jQuery, hãy tải thư viện jQuery trước khi sử dụng';
    }
    if (typeof ($.jstree) === 'undefined') {
        throw 'Thư viện này sử dụng jstree, hãy tải thư viện jstree trước khi sử dụng';
    }

    var mouseInside = false;

    jQuery.fn['dropdownjstree'] = function (settings, param) {
        if (typeof settings === 'string') {
            if (settings.toLowerCase() === 'reset') {
                this.each(function (i, el) {
                    var button = $(el).find('.dropdown-menu button:first');
                    var tree = $(el).find('.dropdown-menu div:first');

                    tree.jstree('deselect_all');
                    if (button.length > 0) {
                        $(el).find('.selectedLabel').text(button.text());
                    } else {
                        tree.jstree('select_node', tree.find('ul > li:first'));
                    }
                });
                return;
            }
            if (settings.toLowerCase() === 'selectnode') {
                this.each(function (i, el) {
                    var tree = $(el).find('.dropdown-menu div:first');

                    tree.jstree('deselect_all');
                    tree.jstree('select_node', param);
                });
                return;
            }
            if (settings.toLowerCase() === 'refresh') {
                this.each(function (i, el) {
                    var tree = $(el).find('.dropdown-menu div:first');
                    if (param.onRefresh && typeof param.onRefresh === 'function') {
                        tree.unbind('refresh.jstree');
                        tree.on('refresh.jstree', function (e, data) {
                            param.onRefresh(e, data);
                        });
                    }

                    tree.jstree('deselect_all');
                    tree.jstree(true).settings.core.data = param.source;
                    tree.jstree(true).refresh();
                });
                return;
            }
            if (settings.toLowerCase() === 'referenceTree') {
                var result = [];
                this.each(function (i, el) {
                    var tree = $(el).find('.dropdown-menu div:first');

                    result.push(tree.jstree(true));
                });
                if (result.length === 1) {
                    return result[0];
                }
                return result;
            }
            if (settings.toLowerCase() === 'disable') {
                this.each(function (i, el) {
                    var button = $(el).find('.dropdown button:first');

                    button.attr('disabled', 'disabled');
                });
                return;
            }
            if (settings.toLowerCase() === 'enable') {
                this.each(function (i, el) {
                    var button = $(el).find('.dropdown button:first');

                    button.removeAttr('disabled');
                });
                return;
            }
        }

        settings = $.extend({
            source: []
        }, settings);

        var backdrop = '.dropdownjstree .dropdown-backdrop';
        var toggle = '.dropdownjstree .dropdown-toggle';

        var getParent = function ($this) {
            var selector = $this.attr('data-target');
            if (!selector) {
                selector = $this.attr('href');
                selector = selector && /#[A-Za-z]/.test(selector) && selector.replace(/.*(?=#[^\s]*$)/, ''); // strip for ie7
            }

            var $parent = selector && $(selector);

            return $parent && $parent.length ? $parent : $this.parent();
        };

        var clearMenus = function () {
            $(backdrop).remove();
            $(toggle).each(function () {
                var $this = $(this);
                var $parent = getParent($this);

                if (!$parent.hasClass('open')) return;

                $this.attr('aria-expanded', 'false');
                $parent.removeClass('open');
            });
        };

        $('html').unbind('mouseup');

        $('html').mouseup(function () {
            if (!mouseInside) {
                clearMenus();
            }
        });

        this.each(function (i, el) {
            var $elQ = $(el);
            if ($elQ.length === 0) {
                return false;
            }

            //Build
            var $dropdown = $('<div class="dropdown dropdownjstree"></div>');
            var $button = $('<button id="dropdownButton' + i + '" type="button" class="btn btn-block btn-default dropdown-toggle" aria-expanded="false"></button>');
            var $label = $('<span class="selectedLabel" style="padding-right: 6px;"></span>');
            var $caret = $(' <span class="fa fa-caret-down" style="display: block;position: absolute;top:  0;right: 9px;line-height: 240%;height: 100%;"></span>');
            var $dropdownMenu = $('<div class="dropdown-menu" role="menu" aria-labelledby="dropdownButton' + i + '" style="max-height:400px !important;overflow-y:auto !important;min-width:250px;font-size: 11px;"></div>');
            var $buttonLabel = $('<button class="btn btn-block btn-sm drown-title" type="button" style="margin-top: -5px; margin-bottom: 5px; font-weight: bold; background: none repeat scroll 0% 0% transparent; border-bottom: 1px solid rgb(221, 221, 221);"></button>');
            var $tree = $('<div></div>');

            $dropdown.append($button).append($dropdownMenu);
            $button.append($label).append('&nbsp;').append($caret);

            if (settings.dropdownLabel && typeof settings.dropdownLabel === 'string') {
                $buttonLabel.text(settings.dropdownLabel);
                $($buttonLabel).click(function () {
                    clearMenus();
                    $tree.jstree("deselect_all");
                    $label.text(settings.dropdownLabel);
                    if (settings.dropdownLabelClick && typeof settings.dropdownLabelClick === 'function') {
                        settings.dropdownLabelClick();
                    }
                });
                $dropdownMenu.append($buttonLabel);
            }
            $dropdownMenu.append($tree);
            $elQ.append($dropdown);

            $dropdownMenu.hover(function () {
                mouseInside = true;
            }, function () {
                mouseInside = false;
            });

            $button.click(function () {
                var $this = $(this);

                if ($this.is('.disabled, :disabled')) return;

                var $parent = getParent($this);
                var isActive = $parent.hasClass('open');

                clearMenus();

                if (!isActive) {
                    if ('ontouchstart' in document.documentElement && !$parent.closest('.navbar-nav').length) {
                        // if mobile we use a backdrop because click events don't delegate
                        $('<div class="dropdown-backdrop"/>').insertAfter($(this)).on('click', clearMenus);
                    }

                    $this
			            .trigger('focus')
			            .attr('aria-expanded', 'true');

                    $parent
			            .toggleClass('open');
                }
            });

            $tree.jstree({
                'core': {
                    "themes" : {
                        "responsive": false
                    },
                    multiple: false,
                    'data': settings.source
                },
                "plugins": ["types"]
            }).on("select_node.jstree", function (node, selected) {
                $label.text(selected.node.text);
                clearMenus();

                if (settings.selectNote && typeof settings.selectNote === 'function') {
                    settings.selectNote(node, selected);
                }
            }).on("ready.jstree", function () {
                if (settings.selectedNode) {
                    $tree.jstree("select_node", settings.selectedNode);
                } else {
                    if (settings.dropdownLabel && typeof settings.dropdownLabel === 'string') {
                        $label.text(settings.dropdownLabel);
                    } else {
                        $tree.jstree("select_node", $tree.find('ul > li:first'));
                    }
                }

                if (settings.ready && typeof settings.ready === 'function') {
                    settings.ready();
                }
            }).on('refresh.jstree', function (e, data) {
                if (settings.refresh && typeof settings.refresh === 'function') {
                    settings.refresh(e, data);
                }
            });
        });
    };
})(window.jQuery);