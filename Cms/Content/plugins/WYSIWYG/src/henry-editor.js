/*
 *  Henry Editor override wysiwyg editor.
 *  - Custome popup.
 *  - Custome Smile plugins.
 *  - Custome Buttons.
 */
; (function ($) {
    var previousCharater = -1;
    var container = ".henryEditorContainer",
        editorOptions = {
            buttons: {}
        },
        defaults = {
            isUserSmile: true,
            isUseFileUpload: true,
            isShowMenuOnSelect: true,
            enableEmoticonFilter: true,
            isShowArrow: false,
            suggestSource: [],
            fileUpload: {
                url: "",
                callback: null,
                closePopupAfterUpload: true,
                selectImageLabel: "",
                allowFilesType: ["png", "gif", "jpeg", "jpg", "rar"]
            },
            buttons: {},
            popupHoverButtons: {},
            // ----------- Event            
            onKeydown: null,
            onKeyPress: null,
            onKeyUp: null
        };

    function HenryEditor(element, options) {
        this._options = $.extend({}, defaults, options);

        this.init(element, this._options);
    };

    // Init editor
    HenryEditor.prototype.init = function (element, options) {
        // Init auto complete
        if (options.autocomplete) {
            debugger;
            createAutoComplete(element, options);
        }

        // Init upload file
        if (options.isUseFileUpload) {
            createUploadPlugin(options);
        }
        // Init smile plugins
        if (options.isUserSmile) {
            createSmilePlugin(element, options);
        }

        // init keypress event.        
        if (options.suggestSource.length > 0)
            onKeyDown(element, options);

        initEditor(element, options);
    };

    function initEditor(element, options) {
        $(element).wysiwyg(options);
    };

    function getHtml(element) {
        return $(element).wysiwyg("shell").getHTML();
    };

    function setHtml(element, content) {
        return $(element).wysiwyg("shell").setHTML(content);
    };

    function insertHtml(element, html) {
        return $(element).wysiwyg("shell").insertHTML(html);
    };

    function setFocus(element) {
        return $(element).wysiwyg("shell").getElement().focus();
    };

    function getText(element) {
        var html = getHtml(element);
        html = html.replace(/<br\/>/ig, "");
        html = html.replace(/&nbsp;/ig, "");
        html = html.replace(/<br>/ig, "");
        return html;
    };

    function createSmilePlugin(element, options) {
        options.buttons.smilies = {
            title: "Smilies",
            image: "\uf118", // <img src="path/to/image.png" width="25" height="25" alt="" />
            popup: function ($popup, $button) {
                var list_smilies = [
                        '<img src="/content/images/advance_smiles/angel_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/angry_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bandit_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bartlett_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/beer_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bigsmile_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bike_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/blushing_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bomb_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bow_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/brb_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/brokenheart_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/bug_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/cake_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/call_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/camera_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/canyoutalk_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/car_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/cash_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/cat_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/clapping_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/coffee_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/computer_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/confidential_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/cool_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/crying_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/dancing_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/devil_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/dog_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/doh_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/drink_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/drunk_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/dull_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/dull_tauri_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/emo_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/envy_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/facepalm_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/fingerscrossed_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/fistbump_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/flower_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/games_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/gift_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/giggle_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/goodluck_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/hands_in_air_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/handshake_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/happy_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/headbang_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/heart_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/heidy_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/hi_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/highfive_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/holdon_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/hug_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/inlove_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/island_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/itwasntme_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/kiss_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/lalala_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/letsmeet_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/lipssealed_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/mail_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/makeup_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/malthe_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/man_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/mmm_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/monkey_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/movember_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/movie_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/muscle_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/music_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/nerd_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/ninja_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/no_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/nod_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/oliver_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/party_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/phone_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/pizza_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/plane_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/poke_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/poolparty_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/praying_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/priidu_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/puking_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/punch_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/rain_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/rainbow_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/rock_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/rofl_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/sadsmile_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/shake_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/sheep_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/skype_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/sleepy_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/smile_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/smirk_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/smoking_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/snail_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/speechless_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/star_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/stop_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/sunshine_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/surprised_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/swear_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/sweating_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/talking_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/talktothehand_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/thinking_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/time_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/tmi_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/toivo_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/tongueout_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/tumbleweed_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/umbrella_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/victory_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/wait_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/waiting_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/wfh_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/whatsgoingon_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/whew_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/wink_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/woman_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/wondering_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/worried_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/yawning_80_anim_gif.gif" width="25" height="25" alt="" />',
                        '<img src="/content/images/advance_smiles/yes_80_anim_gif.gif" width="25" height="25" alt="" />'
                ];

                var $smilies = $("<div/>").addClass("wysiwyg-plugin-smilies")
                                          .attr("unselectable", "on");
                $.each(list_smilies, function (index, smiley) {
                    if (index !== 0)
                        $smilies.append(" ");
                    var $image = $(smiley).attr("unselectable", "on");
                    // Append smiley
                    var imagehtml = " " + $("<div/>").append($image.clone()).html() + " ";
                    $image
                        .css({ cursor: "pointer" })
                        .click(function (event) {
                            //$(element).wysiwyg('shell').insertHTML(imagehtml); // .closePopup(); - do not close the popup
                            $(element).wysiwyg("shell").insertHTML(imagehtml + "&nbsp;").closePopup(); // .closePopup(); - do not close the popup
                            $(element).wysiwyg("shell").getElement().focus();
                        })
                        .appendTo($smilies);
                });
                var $container = $(element).wysiwyg("container"); // Container
                //$smilies.css({ maxWidth: parseInt($container.width() * 0.95) + 'px' });
                $popup.append($smilies);
                // Smilies do not close on click, so force the popup-position to cover the toolbar
                var $toolbar = $button.parents(".wysiwyg-toolbar");
                if (!$toolbar.length) // selection toolbar?
                    return;
                return { // this prevents applying default position
                    left: parseInt(($toolbar.outerWidth() - $popup.outerWidth()) / 2),
                    top: $toolbar.hasClass("wysiwyg-toolbar-bottom") ? ($container.outerHeight() - parseInt($button.outerHeight() / 4)) : (parseInt($button.outerHeight() / 4) - $popup.height())
                };
            },
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createUploadPlugin(options) {
        options.buttons.uploadFile = {
            title: "Tải file lên",
            image: "\uf0ee", // <img src="path/to/image.png" width="25" height="25" alt="" />,            
            showselection: false    // wanted on selection
        };

        options.upload_options = options.fileUpload;
        options.select_file_text = options.fileUpload.selectImageLabel;
        options.fileupload_url = options.fileUpload.url;
        options.afterUploadFile = options.fileUpload.callback;
        options.forceImageUpload = true;
        options.close_popup_after_upload = options.fileUpload.closePopupAfterUpload;
    };

    function createAutoComplete(options) {
        var editor = $(element).wysiwyg("shell");
        editor.onAutocomplete = function (typed, key, character, shiftKey, altKey, ctrlKey, metaKey) {
            if (typed.indexOf(options.autocomplete.mapCharacter) == 0) // startswith '@'
            {
                var sources = [];

                if ($.isFunction(options.autocomplete.source)) {
                    sources = options.autocomplete.source();
                } else {
                    sources = !options.autocomplete.source ? [] : options.autocomplete.source;
                }

                var $list = $("<div/>").addClass("wysiwyg-plugin-list")
                                       .attr("unselectable", "on");
                $.each(sources, function (index, source) {
                    if (source.toLowerCase().indexOf(typed.substring(1).toLowerCase()) !== 0) // don't count first character '@'
                        return;
                    var $link = $("<a/>").attr("href", "#")
                                        .text(source)
                                        .click(function (event) {
                                            var url = 'http://example.com/user/' + source,
                                                html = '<a href="' + url + '">@' + source + '</a> ';

                                            // Expand selection and set inject HTML
                                            editor.expandSelection(typed.length, 0).insertHTML(html);
                                            editor.closePopup().getElement().focus();
                                            // prevent link-href-#
                                            event.stopPropagation();
                                            event.preventDefault();
                                            return false;
                                        });
                    $list.append($link);
                });
                if ($list.children().length) {
                    if (key == 13) {
                        $list.children(":first").click();
                        return false; // swallow enter
                    }
                        // Show popup
                    else if (character || key == 8)
                        return $list;
                }
            }
        }
    }

    // ----------- Events
    function onKeyDown(element, options) {
        options.onKeyDown = function (key, character, shiftKey, altKey, ctrlKey, metaKey) {
            var shell = $(element).wysiwyg("shell");
            var range = window.getSelection().getRangeAt(0);
            var text = window.getSelection().baseNode.wholeText;

            var startPosition = range.startOffset;
            var previousCharater = text != undefined ? text.substr(startPosition - 1, 1) : "";

            if (shiftKey === true && key === 50 && ($.trim(previousCharater) === "&nbsp;" || $.trim(previousCharater) === "" || $.trim(previousCharater) === "@")) {
                var autoComplete = "<input class='comment-auto-complete'/>";
                shell.insertHTML(autoComplete);

                //_.each(options.suggestSource, function () {

                //});

                $(".comment-auto-complete").focus();
                $(".comment-auto-complete").autocomplete({
                    autoFocus: true,
                    source: options.suggestSource,
                    select: function (event, ui) {
                        var userTagItem = "<a href='javascript://' class='user-tag-item' data-id='{id}'>@{fullName}</a>&nbsp;";
                        userTagItem = userTagItem.replace(/{id}/ig, ui.item.Id);
                        userTagItem = userTagItem.replace(/{fullName}/ig, ui.item.FullName);
                        var commentContent = shell.getHTML();
                        commentContent = commentContent.replace('<input class="comment-auto-complete ui-autocomplete-input" autocomplete="off">', "");
                        shell.setHTML(commentContent);
                        shell.insertHTML(userTagItem);
                    }
                }).autocomplete("instance")._renderItem = function (ul, item) {
                    return renderItemUserAutocomplete(ul, item);
                };
            }

            if (!shiftKey)
                previousCharater = key;
        };
    };

    $.fn.henryEditor = function (options, content) {
        if ((typeof options) === "object" || !options) {
            return this.each(function () {
                $that = $(this);
                if (!$that.data("henryEditor")) {
                    $that.data("henryEditor", new HenryEditor(this, options));
                }
            });
        } else {
            if (options === "getHTML") {
                return getHtml(this);
            } else if (options === "setHTML") {
                return setHtml(this, content);
            } else if (options === "setFocus") {
                return setFocus(this);
            } else if (options === "getText") {
                return getText(this);
            }
        }
    };
})(jQuery);