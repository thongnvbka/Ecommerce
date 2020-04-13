/*
 *  proview editor override wysiwyg editor.
 *  - Custom popup.
 *  - Custom Buttons.
 */

(function ($) {
    var container = ".proviewEditorContainer",
        editorOptions = {
            buttons: {}
        },
        defaults = {
            classes: 'proview',
            isUseFileUpload: true,
            isShowMenuOnSelect: true,
            enableEmoticonFilter: true,
            isShowArrow: true,
            uploadImage: {
                url: "",
                callback: null,
                closePopupAfterUpload: true,
                placeholder: "Chọn hoặc kép thả yuanp tin vào đây"
            },
            fileUpload: {
                url: "",
                callback: null,
                closePopupAfterUpload: true,
                selectImageLabel: "",
                allowFilesType: ["png", "gif", "jpeg", "jpg", "rar"]
            },
            autocomplete: null,
            height: 0,
            submit: {
                title: 'Agree',
                image: '\uf00c',
                classes: 'btn btn-default'
            },
            toolbarButtons: [],
            popupHoverButtons: {},
            // ----------- Event            
            onKeydown: null,
            onKeyPress: null,
            onKeyUp: null
        };

    function ProviewEditor(element, options) {
        this._options = $.extend({}, defaults, options);

        this.init(element, this._options);
    };

    // Init editor
    ProviewEditor.prototype.init = function (element, options) {
        // Init auto complete
        if (options.autocomplete) {
            createAutoComplete(element, options);
        }
        createButtons(element, options);

        editorOptions = $.extend({}, editorOptions, options);
        initEditor(element, editorOptions);
    };

    function initEditor(element, options) {
        $(element).wysiwyg(options);
    };

    function getHtml(element) {
        return $(element).wysiwyg('shell').getHTML();
    };

    function setHtml(element, content) {
        if (typeof ($(element).wysiwyg("shell").setHTML) !== "undefined")
            return $(element).wysiwyg("shell").setHTML(content);
        else
            return "";
    };

    function insertHtml(element, html) {
        if (typeof ($(element).wysiwyg("shell").insertHTML) !== "undefined")
            return $(element).wysiwyg('shell').insertHTML(html);
        else
            return "";
    };

    function setFocus(element) {
        return $(element).wysiwyg('shell').getElement().focus();
    };

    function getText(element) {
        var html = getHtml(element);
        return html === null || html === undefined || html === "" ? html : $(html).text();
    };

    function createSmilePlugin(element, options) {
        options.buttons.smilies = {
            title: 'Smilies',
            image: '\uf118', // <img src="path/to/image.png" width="25" height="25" alt="" />
            popup: function ($popup, $button) {
                var list_smilies = [
                        '<img src="smiley/afraid.png" width="16" height="16" alt="" />',
                        '<img src="smiley/amorous.png" width="16" height="16" alt="" />',
                        '<img src="smiley/angel.png" width="16" height="16" alt="" />',
                        '<img src="smiley/angry.png" width="16" height="16" alt="" />',
                        '<img src="smiley/bored.png" width="16" height="16" alt="" />',
                        '<img src="smiley/cold.png" width="16" height="16" alt="" />',
                        '<img src="smiley/confused.png" width="16" height="16" alt="" />',
                        '<img src="smiley/cross.png" width="16" height="16" alt="" />',
                        '<img src="smiley/crying.png" width="16" height="16" alt="" />',
                        '<img src="smiley/devil.png" width="16" height="16" alt="" />',
                        '<img src="smiley/disappointed.png" width="16" height="16" alt="" />',
                        '<img src="smiley/dont-know.png" width="16" height="16" alt="" />',
                        '<img src="smiley/drool.png" width="16" height="16" alt="" />',
                        '<img src="smiley/embarrassed.png" width="16" height="16" alt="" />',
                        '<img src="smiley/excited.png" width="16" height="16" alt="" />',
                        '<img src="smiley/excruciating.png" width="16" height="16" alt="" />',
                        '<img src="smiley/eyeroll.png" width="16" height="16" alt="" />',
                        '<img src="smiley/happy.png" width="16" height="16" alt="" />',
                        '<img src="smiley/hot.png" width="16" height="16" alt="" />',
                        '<img src="smiley/hug-left.png" width="16" height="16" alt="" />',
                        '<img src="smiley/hug-right.png" width="16" height="16" alt="" />',
                        '<img src="smiley/hungry.png" width="16" height="16" alt="" />',
                        '<img src="smiley/invincible.png" width="16" height="16" alt="" />',
                        '<img src="smiley/kiss.png" width="16" height="16" alt="" />',
                        '<img src="smiley/lying.png" width="16" height="16" alt="" />',
                        '<img src="smiley/meeting.png" width="16" height="16" alt="" />',
                        '<img src="smiley/nerdy.png" width="16" height="16" alt="" />',
                        '<img src="smiley/neutral.png" width="16" height="16" alt="" />',
                        '<img src="smiley/party.png" width="16" height="16" alt="" />',
                        '<img src="smiley/pirate.png" width="16" height="16" alt="" />',
                        '<img src="smiley/pissed-off.png" width="16" height="16" alt="" />',
                        '<img src="smiley/question.png" width="16" height="16" alt="" />',
                        '<img src="smiley/sad.png" width="16" height="16" alt="" />',
                        '<img src="smiley/shame.png" width="16" height="16" alt="" />',
                        '<img src="smiley/shocked.png" width="16" height="16" alt="" />',
                        '<img src="smiley/shut-mouth.png" width="16" height="16" alt="" />',
                        '<img src="smiley/sick.png" width="16" height="16" alt="" />',
                        '<img src="smiley/silent.png" width="16" height="16" alt="" />',
                        '<img src="smiley/sleeping.png" width="16" height="16" alt="" />',
                        '<img src="smiley/sleepy.png" width="16" height="16" alt="" />',
                        '<img src="smiley/stressed.png" width="16" height="16" alt="" />',
                        '<img src="smiley/thinking.png" width="16" height="16" alt="" />',
                        '<img src="smiley/tongue.png" width="16" height="16" alt="" />',
                        '<img src="smiley/uhm-yeah.png" width="16" height="16" alt="" />',
                        '<img src="smiley/wink.png" width="16" height="16" alt="" />',
                        '<img src="smiley/working.png" width="16" height="16" alt="" />',
                        '<img src="smiley/bathing.png" width="16" height="16" alt="" />',
                        '<img src="smiley/beer.png" width="16" height="16" alt="" />',
                        '<img src="smiley/boy.png" width="16" height="16" alt="" />',
                        '<img src="smiley/camera.png" width="16" height="16" alt="" />',
                        '<img src="smiley/chilli.png" width="16" height="16" alt="" />',
                        '<img src="smiley/cigarette.png" width="16" height="16" alt="" />',
                        '<img src="smiley/cinema.png" width="16" height="16" alt="" />',
                        '<img src="smiley/coffee.png" width="16" height="16" alt="" />',
                        '<img src="smiley/girl.png" width="16" height="16" alt="" />',
                        '<img src="smiley/console.png" width="16" height="16" alt="" />',
                        '<img src="smiley/grumpy.png" width="16" height="16" alt="" />',
                        '<img src="smiley/in_love.png" width="16" height="16" alt="" />',
                        '<img src="smiley/internet.png" width="16" height="16" alt="" />',
                        '<img src="smiley/lamp.png" width="16" height="16" alt="" />',
                        '<img src="smiley/mobile.png" width="16" height="16" alt="" />',
                        '<img src="smiley/mrgreen.png" width="16" height="16" alt="" />',
                        '<img src="smiley/musical-note.png" width="16" height="16" alt="" />',
                        '<img src="smiley/music.png" width="16" height="16" alt="" />',
                        '<img src="smiley/phone.png" width="16" height="16" alt="" />',
                        '<img src="smiley/plate.png" width="16" height="16" alt="" />',
                        '<img src="smiley/restroom.png" width="16" height="16" alt="" />',
                        '<img src="smiley/rose.png" width="16" height="16" alt="" />',
                        '<img src="smiley/search.png" width="16" height="16" alt="" />',
                        '<img src="smiley/shopping.png" width="16" height="16" alt="" />',
                        '<img src="smiley/star.png" width="16" height="16" alt="" />',
                        '<img src="smiley/studying.png" width="16" height="16" alt="" />',
                        '<img src="smiley/suit.png" width="16" height="16" alt="" />',
                        '<img src="smiley/surfing.png" width="16" height="16" alt="" />',
                        '<img src="smiley/thunder.png" width="16" height="16" alt="" />',
                        '<img src="smiley/tv.png" width="16" height="16" alt="" />',
                        '<img src="smiley/typing.png" width="16" height="16" alt="" />',
                        '<img src="smiley/writing.png" width="16" height="16" alt="" />'
                ];

                var $smilies = $('<div/>').addClass('wysiwyg-plugin-smilies')
                                          .attr('unselectable', 'on');
                $.each(list_smilies, function (index, smiley) {
                    if (index != 0)
                        $smilies.append(' ');
                    var $image = $(smiley).attr('unselectable', 'on');
                    // Append smiley
                    var imagehtml = ' ' + $('<div/>').append($image.clone()).html() + ' ';
                    $image
                        .css({ cursor: 'pointer' })
                        .click(function (event) {
                            //$(element).wysiwyg('shell').insertHTML(imagehtml); // .closePopup(); - do not close the popup
                            $(element).wysiwyg("shell").insertHTML(imagehtml + "&nbsp;").closePopup(); // .closePopup(); - do not close the popup
                        })
                        .appendTo($smilies);
                });
                var $container = $(element).wysiwyg("container"); // Container
                $smilies.css({ maxWidth: parseInt($container.width() * 0.95) + 'px' });
                $popup.append($smilies);
                // Smilies do not close on click, so force the popup-position to cover the toolbar
                var $toolbar = $button.parents('.wysiwyg-toolbar');
                if (!$toolbar.length) // selection toolbar?
                    return;
                return { // this prevents applying default position
                    left: parseInt(($toolbar.outerWidth() - $popup.outerWidth()) / 2),
                    top: $toolbar.hasClass('wysiwyg-toolbar-bottom') ? ($container.outerHeight() - parseInt($button.outerHeight() / 4)) : (parseInt($button.outerHeight() / 4) - $popup.height())
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

    /* Create Buttons */
    function createLinkButton() {
        return {
            title: 'Insert link',
            image: '\uf0c1' // <img src="path/to/image.png" width="16" height="16" alt="" />
        };
    };

    function createFontNameButton(element) {
        return {
            title: 'Font',
            image: '\uf031', // <img src="path/to/image.png" width="16" height="16" alt="" />
            popup: function ($popup, $button) {
                var list_fontnames = {
                    // Name : Font
                    'Arial, Helvetica': 'Arial,Helvetica',
                    'Verdana': 'Verdana,Geneva',
                    'Georgia': 'Georgia',
                    'Courier New': 'Courier New,Courier',
                    'Times New Roman': 'Times New Roman,Times'
                };
                var $list = $('<div/>').addClass('wysiwyg-plugin-list')
                                       .attr('unselectable', 'on');
                $.each(list_fontnames, function (name, font) {
                    var $link = $('<a/>').attr('href', '#')
                                        .css('font-family', font)
                                        .html(name)
                                        .click(function (event) {
                                            $(element).wysiwyg('shell').fontName(font).closePopup();
                                            // prevent link-href-#
                                            event.stopPropagation();
                                            event.preventDefault();
                                            return false;
                                        });
                    $list.append($link);
                });
                $popup.append($list);
            },
            //showstatic: true,    // wanted on the toolbar
            showselection: true    // wanted on selection
        };
    };

    function createFontsizeButton(element) {
        return {
            title: 'Size',
            image: '\uf034', // <img src="path/to/image.png" width="16" height="16" alt="" />
            popup: function ($popup, $button) {
                // Hack: http://stackoverflow.com/questions/5868295/document-execcommand-fontsize-in-pixels/5870603#5870603
                var list_fontsizes = [];
                for (var i = 8; i <= 11; ++i)
                    list_fontsizes.push(i + 'px');
                for (var i = 12; i <= 28; i += 2)
                    list_fontsizes.push(i + 'px');
                list_fontsizes.push('36px');
                list_fontsizes.push('48px');
                list_fontsizes.push('72px');
                var $list = $('<div/>').addClass('wysiwyg-plugin-list')
                                       .attr('unselectable', 'on');
                $.each(list_fontsizes, function (index, size) {
                    var $link = $('<a/>').attr('href', '#')
                                        .html(size)
                                        .click(function (event) {
                                            $(element).wysiwyg('shell').fontSize(7).closePopup();
                                            $(element).wysiwyg('container')
                                                    .find('font[size=7]')
                                                    .removeAttr("size")
                                                    .css("font-size", size);
                                            // prevent link-href-#
                                            event.stopPropagation();
                                            event.preventDefault();
                                            return false;
                                        });
                    $list.append($link);
                });
                $popup.append($list);
            }
            //showstatic: true,    // wanted on the toolbar
            //showselection: true    // wanted on selection
        }
    }

    function createHeaderButton(buttons) {
        return {
            title: 'Header',
            image: '\uf1dc', // <img src="path/to/image.png" width="16" height="16" alt="" />
            popup: function ($popup, $button) {
                var list_headers = {
                    // Name : Font
                    'Header 1': '<h1>',
                    'Header 2': '<h2>',
                    'Header 3': '<h3>',
                    'Header 4': '<h4>',
                    'Header 5': '<h5>',
                    'Header 6': '<h6>',
                    'Code': '<pre>'
                };
                var $list = $('<div/>').addClass('wysiwyg-plugin-list')
                                       .attr('unselectable', 'on');
                $.each(list_headers, function (name, format) {
                    var $link = $('<a/>').attr('href', '#')
                                         .css('font-family', format)
                                         .html(name)
                                         .click(function (event) {
                                             $(element).wysiwyg('shell').format(format).closePopup();
                                             // prevent link-href-#
                                             event.stopPropagation();
                                             event.preventDefault();
                                             return false;
                                         });
                    $list.append($link);
                });
                $popup.append($list);
            }
            //showstatic: true,    // wanted on the toolbar
            //showselection: false    // wanted on selection
        }
    };

    function createBoldButton(buttons) {
        return {
            title: 'Bold (Ctrl+B)',
            image: '\uf032', // <img src="path/to/image.png" width="16" height="16" alt="" />
            hotkey: 'b'
        }
    };

    function createItalicButton() {
        return {
            title: 'Italic (Ctrl+I)',
            image: '\uf033', // <img src="path/to/image.png" width="16" height="16" alt="" />
            hotkey: 'i'
        }
    };

    function createSmilesButton(element) {
        return {
            title: 'Biểu tượng cảm xúc',
            image: '\uf118', // <img src="path/to/image.png" width="16" height="16" alt="" />
            popup: function ($popup, $button) {
                var list_smilies = [
                        '<img src="/Images/smiley/afraid.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/amorous.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/angel.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/angry.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/bored.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/cold.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/confused.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/cross.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/crying.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/devil.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/disappointed.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/dont-know.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/drool.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/embarrassed.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/excited.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/excruciating.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/eyeroll.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/happy.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/hot.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/hug-left.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/hug-right.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/hungry.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/invincible.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/kiss.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/lying.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/meeting.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/nerdy.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/neutral.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/party.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/pirate.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/pissed-off.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/question.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/sad.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/shame.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/shocked.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/shut-mouth.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/sick.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/silent.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/sleeping.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/sleepy.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/stressed.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/thinking.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/tongue.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/uhm-yeah.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/wink.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/working.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/bathing.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/beer.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/boy.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/camera.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/chilli.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/cigarette.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/cinema.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/coffee.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/girl.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/console.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/grumpy.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/in_love.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/internet.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/lamp.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/mobile.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/mrgreen.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/musical-note.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/music.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/phone.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/plate.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/restroom.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/rose.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/search.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/shopping.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/star.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/studying.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/suit.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/surfing.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/thunder.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/tv.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/typing.png" width="16" height="16" alt="" />',
                        '<img src="/Images/smiley/writing.png" width="16" height="16" alt="" />'
                ];
                var $smilies = $('<div/>').addClass('wysiwyg-plugin-smilies')
                                          .attr('unselectable', 'on');
                $.each(list_smilies, function (index, smiley) {
                    if (index != 0)
                        $smilies.append(' ');
                    var $image = $(smiley).attr('unselectable', 'on');
                    // Append smiley
                    var imagehtml = ' ' + $('<div/>').append($image.clone()).html() + ' ';
                    $image
                        .css({ cursor: 'pointer' })
                        .click(function (event) {
                            $(element).wysiwyg('shell').insertHTML(imagehtml); // .closePopup(); - do not close the popup
                        })
                        .appendTo($smilies);
                });
                var $container = $(element).wysiwyg('container');
                $smilies.css({ maxWidth: parseInt($container.width() * 0.95) + 'px' });
                $popup.append($smilies);
                // Smilies do not close on click, so force the popup-position to cover the toolbar
                var $toolbar = $button.parents('.wysiwyg-toolbar');
                if (!$toolbar.length) // selection toolbar?
                    return;
                return { // this prevents applying default position
                    left: parseInt(($toolbar.outerWidth() - $popup.outerWidth()) / 2),
                    top: $toolbar.hasClass('wysiwyg-toolbar-bottom') ? ($container.outerHeight() - parseInt($button.outerHeight() / 4)) : (parseInt($button.outerHeight() / 4) - $popup.height())
                };
            },
            showstatic: true,    // wanted on the toolbar
            showselection: true
        }
    };

    function createInsertImageButton(options) {        
        if (options.uploadImage.url != "" && options.uploadImage.url != undefined && options.uploadImage.url != null) {
            options.selectImagePlaceholder = options.uploadImage.placeholder;
            options.uploadImageUrl = options.uploadImage.url;
            options.afterUploadImage = options.uploadImage.callback;
            options.forceImageUpload = true;
            options.close_popup_after_upload = options.uploadImage.closePopupAfterUpload;

            return {
                title: 'Insert image',
                image: '\uf030', // <img src="path/to/image.png" width="16" height="16" alt="" />
                //showstatic: true,    // wanted on the toolbar,
                showselection: true    // wanted on selection
            }
        } else {
            return null;
        }
    };

    function createInsertVideoButton() {
        return {
            title: 'Insert video',
            image: '\uf03d', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: true    // wanted on selection
        }
    };

    function createUnderlineButton() {
        return {
            title: 'Underline (Ctrl+U)',
            image: '\uf0cd', // <img src="path/to/image.png" width="16" height="16" alt="" />
            hotkey: 'u'
        }
    };

    function createStrikeThroughButton() {
        return {
            title: 'Strikethrough (Ctrl+S)',
            image: '\uf0cc', // <img src="path/to/image.png" width="16" height="16" alt="" />
            hotkey: 's'
        }
    };

    function createForceColorButton() {
        return {
            title: 'Text color',
            image: '\uf1fc' // <img src="path/to/image.png" width="16" height="16" alt="" />
        }
    };

    function createHighlightButton() {
        return {
            title: 'Background color',
            image: '\uf043' // <img src="path/to/image.png" width="16" height="16" alt="" />
        }
    };

    function createAlignLeftButton() {
        return {
            title: 'Left',
            image: '\uf036', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createAlignCenterButton() {
        return {
            title: 'Center',
            image: '\uf037', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createAlignRightButton() {
        return {
            title: 'Right',
            image: '\uf038', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createAlignJustifyButton() {
        return {
            title: 'Justify',
            image: '\uf039', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createSubScriptButton() {
        return {
            title: 'Subscript',
            image: '\uf12c', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: true    // wanted on selection
        }
    };

    function createSupperScriptButton() {
        return {
            title: 'Superscript',
            image: '\uf12b', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: true    // wanted on selection
        }
    };

    function createIndentButton() {
        return {
            title: 'Indent',
            image: '\uf03c', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createOutdentButton() {
        return {
            title: 'Outdent',
            image: '\uf03b', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };


    function createOrderListButton() {
        return {
            title: 'Ordered list',
            image: '\uf0cb', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createUnorderlistButton() {
        return {
            title: 'Unordered list',
            image: '\uf0ca', // <img src="path/to/image.png" width="16" height="16" alt="" />
            //showstatic: true,    // wanted on the toolbar
            showselection: false    // wanted on selection
        }
    };

    function createRemoveFormatButton() {
        return {
            title: 'Remove format',
            image: '\uf12d' // <img src="path/to/image.png" width="16" height="16" alt="" />
        }
    };

    function createButtons(element, options) {        
        if (options.toolbarButtons.length === 0) {
            options.buttons = {
                smiles: createSmilesButton(element),
                uploadplugin: createUploadPlugin(options),
                insertimage: createInsertImageButton(options),
                insertvideo: createInsertVideoButton(),
                insertlink: createLinkButton(),
                fontname: createFontNameButton(element),
                fontsize: createFontsizeButton(element),
                header: createHeaderButton(),
                bold: createBoldButton(),
                italic: createItalicButton(),
                underline: createUnderlineButton(),
                strikethrough: createStrikeThroughButton(),
                forecolor: createForceColorButton(),
                highlight: createHighlightButton(),
                alignleft: createAlignLeftButton(),
                aligncenter: createAlignCenterButton(),
                alignright: createAlignRightButton(),
                alignjustify: createAlignJustifyButton(),
                subscript: createSubScriptButton(),
                superscript: createSupperScriptButton(),
                indent: createIndentButton(),
                outdent: createOutdentButton(),
                orderedList: createOrderListButton(),
                unorderedList: createUnorderlistButton(),
                removeformat: createRemoveFormatButton()
            }
        } else {
            options.buttons = {};
            $.each(options.toolbarButtons, function (index, item) {
                if (item === 'smiles') { options.buttons.smiles = createSmilesButton(element); }
                else if (item === 'upload') { options.buttons.uploadplugin = createUploadPlugin(options); }
                else if (item === 'image') { options.buttons.insertimage = createInsertImageButton(options); }
                else if (item === 'video') { options.buttons.insertvideo = createInsertVideoButton(); }
                else if (item === 'link') { options.buttons.insertlink = createLinkButton(); }
                else if (item === 'fontname') { options.buttons.fontname = createFontNameButton(element); }
                else if (item === 'fontsize') { options.buttons.fontsize = createFontsizeButton(element); }
                else if (item === 'header') { options.buttons.header = createHeaderButton(); }
                else if (item === 'bold') { options.buttons.bold = createBoldButton(); }
                else if (item === 'italic') { options.buttons.italic = createItalicButton(); }
                else if (item === 'underline') { options.buttons.underline = createUnderlineButton(); }
                else if (item === 'strikethrough') { options.buttons.strikethrough = createStrikeThroughButton(); }
                else if (item === 'forecolor') { options.buttons.forecolor = createForceColorButton(); }
                else if (item === 'highlight') { options.buttons.highlight = createHighlightButton(); }
                else if (item === 'alignleft') { options.buttons.alignleft = createAlignLeftButton(); }
                else if (item === 'aligncenter') { options.buttons.aligncenter = createAlignCenterButton(); }
                else if (item === 'alignright') { options.buttons.alignright = createAlignRightButton(); }
                else if (item === 'alignjustify') { options.buttons.alignjustify = createAlignJustifyButton(); }
                else if (item === 'subscript') { options.buttons.subscript = createSubScriptButton(); }
                else if (item === 'superscript') { options.buttons.superscript = createSupperScriptButton(); }
                else if (item === 'indent') { options.buttons.indent = createIndentButton(); }
                else if (item === 'orderedList') { options.buttons.orderedList = createOrderListButton(); }
                else if (item === 'unorderedList') { options.buttons.unorderedList = createUnorderlistButton(); }
                else if (item === 'removeformat') { options.buttons.removeformat = createRemoveFormatButton(); }
            });
        }
    };

    /* --- End create buttons  --- */

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

                var $list = $('<div/>').addClass('wysiwyg-plugin-list')
                                       .attr('unselectable', 'on');
                $.each(sources, function (index, source) {
                    if (source.toLowerCase().indexOf(typed.substring(1).toLowerCase()) !== 0) // don't count first character '@'
                        return;
                    var $link = $('<a/>').attr('href', '#')
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
                        $list.children(':first').click();
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
    $.fn.proviewEditor = function (options, content) {
        // @Henry        
        if ($(this).length <= 0)
            return false;

        if ((typeof options) === 'object' || !options) {
            return this.each(function () {
                var $that = $(this);
                if (!$that.data("proviewEditor")) {
                    $that.data("proviewEditor", new ProviewEditor(this, options));

                    // Init tool tips
                    $(".wysiwyg-toolbar-icon").tooltip();
                }
            });
        } else {            
            if (options === 'getHTML') {
                return getHtml(this);
            } else if (options === 'setHTML') {
                return setHtml(this, content);
            } else if (options === 'setFocus') {
                return setFocus(this);
            } else if (options === 'insertHTML') {
                return insertHtml(this, content);
            } else if (options === 'getText') {
                return getText(this);
            }
        }
        return true;
    };
})(jQuery);