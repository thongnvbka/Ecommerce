var detailUserSend = function (target) {
    if ($("#detailUserSend").is(":visible")) {
        $(target).text(resources.internalEmail.button.expand);
        $('#detailUserSend').hide();
    } else {
        $(target).text(resources.internalEmail.button.collapse);
        $('#detailUserSend').show();
    }
};

var MyMessageModel = function () {
    var self = this;

    //Globalize.culture("en-US");

    var listType = {
        inbox: 'inbox',
        sent: 'sent',
        draft: 'draft',
        trash: 'trash',
        star: 'star'
    };

    var filterType = {
        all: 'all',
        readed: 'read',
        unRead: 'unRead',
        star: 'star'
    };

    var focusTopCkeditor = function () {
        var editor = CKEDITOR.instances.messageContent;
        editor.focus();

        var selection = editor.getSelection();
        var range = selection.getRanges()[0];
        var pCon = range.startContainer.getAscendant({ p: 1 }, true);
        var newRange = new CKEDITOR.dom.range(range.document);
        newRange.moveToPosition(pCon, CKEDITOR.POSITION_BEFORE_START);
        newRange.select();
    };

    self.pageClickSearch = function (pageclickednumber) {
        self.search(pageclickednumber);
        $("#pager").pager({ pagenumber: pageclickednumber, pagecount: totalPage, buttonClickCallback: self.pageClickSearch });
    };

    var renderPage = function (currentPage, totalRecord) {
        self.totalPage(Math.ceil(totalRecord / self.recordPerPage()));
        $("#sumaryMessagePager").text(totalRecord === 0 ? "There are no messages" : "Show {0}->{1} of {2} mail".format(((currentPage - 1) * self.recordPerPage() + 1), (currentPage * self.recordPerPage() < totalRecord ? currentPage * self.recordPerPage() : totalRecord), totalRecord));
        $("#messagePager").pagerNextBackOnly({ pagenumber: currentPage, pagecount: self.totalPage(), totalrecords: totalRecord, buttonClickCallback: self.pageClickSearch });
    };

    var confirmNavigation = function (e) {
        e = e || window.event;
        var message = resources.internalEmail.message.confirmLeavePage;
        if (e) {
            e.returnValue = message;
        }
        return message;
    };

    var formatList = function (list) {
        var currentUserId = (window.currentUser).UserId;
        $.each(list, function (index, item) {
            var sendTime = moment(item.SendTime).format(resources.common.defaultFormat.dateTime);
            item.SendTime = sendTime == 'Invalid date' ? '' : sendTime;

            var lastModifiedOnDate = moment(item.LastModifiedOnDate).format(resources.common.defaultFormat.dateTime);
            item.LastModifiedOnDate = lastModifiedOnDate == 'Invalid date' ? '' : lastModifiedOnDate;

            var fromUser;
            try {
                fromUser = JSON.parse(item.FromUser);
            } catch (e) {
                fromUser = {};
            }

            if (fromUser.userId == currentUserId) {
                if (self.listType() === listType.draft) {
                    item.FullName = resources.internalEmail.draft;
                } else if (item.SendTime == '' && self.listType() === listType.star) {
                    item.FullName = resources.internalEmail.draft;
                } else {
                    var toUser;
                    try {
                        toUser = JSON.parse(item.ToUser);
                    } catch (e) {
                        toUser = [];
                    }
                    if (toUser.length > 0) {
                        item.FullName = toUser[0].fullName;
                    }

                    if (toUser.length > 1 || item.CcToUser) {
                        item.FullName += ' ...';
                    }
                }
            } else {
                item.FullName = fromUser.fullName;
                item.UserName = fromUser.userName;
            }

            item.Description = '';
            item.Title = item.Title == '' ? resources.internalEmail.descriptionSubject.noSubject : item.Title;
            if (self.listType() === listType.star) {
                if (item.Type) {
                    if (item.SendTime == '') {
                        item.Description = '[{0}] - '.format(resources.internalEmail.descriptionSubject.draft);
                    } else {
                        item.Description = '[{0}] - '.format(resources.internalEmail.descriptionSubject.sentMail);
                    }
                } else {
                    item.Description = '[{0}] - '.format(resources.internalEmail.descriptionSubject.inbox);
                }
            }
        });
    };

    var validateBlackListExtensions = function (file) {
        var ext = file.substring(file.lastIndexOf('.')).toLowerCase();
        return _.some(blackListExtensions, function (item) { return item === ext; });
    };

    var bindUploadAttachment = function () {
        $('#messageFileUpload, #messageFileUpload1').fileupload({
            formData: { id: self.id() },
            url: '/Upload/UploadAttachmentMessage',
            sequentialUploads: true,
            dataType: 'json',
            add: function (e, data) {
                var file = data.files[0];
                var msg = "";
                if (maxFileLength && file.size > maxFileLength) {
                    if (msg) {
                        msg += '<br/>';
                    }
                    msg += file.name + ": " + resources.commonl.message.maxFileLengthUpload;
                } else if (validateBlackListExtensions(file.name)) {
                    if (msg) {
                        msg += '<br/>';
                    }
                    msg += file.name + ": " + resources.common.message.fileNotAllowUpload;
                }
                if (msg !== "") {
                    toastr.error(msg);
                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                if (!data.result) {
                    toastr.error(resources.internalEmail.message.noAttachmentUpload);
                } else {
                    if (data.result == -2) {
                        toastr.error(resources.internalEmail.message.notHavePermissionAttachFile);
                    } else if (data.result == -5) {
                        toastr.error(resources.common.message.fileNotAllowUpload);
                    } else if ($.isArray(data.result)) {
                        for (var i = 0; i < data.result.length; i++) {
                            self.attachments.push(data.result[i]);
                        }
                        self.attachmentCount(self.attachmentCount() + data.result.length);
                    }
                }
                $('#loadingUpload1').hide().siblings('i').show();
            },
            send: function () {
                $('#loadingUpload1').show().siblings('i').hide();
            }, fail: function () {
                $('#loadingUpload1').show().siblings('i').show();
            }
        });
    };

    var bindAutocomplete = function (name, initialTags) {
        var placeHolder = resources.internalEmail.placeHolder.to;
        if (name == 'cc') {
            placeHolder = resources.internalEmail.placeHolder.cc;
        } else if (name == 'bcc') {
            placeHolder = resources.internalEmail.placeHolder.bcc;
        }
        $('#' + name).tagEditor({
            initialTags: initialTags ? initialTags : [],
            delimiter: ';',
            placeholder: placeHolder,
            forceLowercase: true,
            onChange: function () {
                self[name]($('#' + name).val());

            },
            autocomplete: {
                autoFocus: true,
                delay: 300,
                position: { collision: 'flip' },
                source: '/user/searchfullname',
                //focus: function (event, ui) {
                //    $('.active', $('.tag-editor')).find('input').val(ui.item.UserName);
                //    return false;
                //},
                select: function (event, ui) {
                    $('.tag-editor-tag.active .ui-autocomplete-input').val(ui.item.UserName).parents('ul:first').click();
                    return false;
                },
                _renderItem: function (ul, item1) {
                    return renderItemUserAutocomplete(ul, item1);
                }
            }
        });
    };

    var reBindAutocomplete = function (name, initialTags) {
        self[name]('');
        var input = $('#' + name);
        try {
            input.tagEditor('destroy');
        } catch (e) {

        } 
        bindAutocomplete(name, initialTags);
    }

    self.RebindTagEditor = function (name, list) {
        reBindAutocomplete(name, list);
    }

    var listOffice = {};

    self.title = ko.observable(resources.internalEmail.inbox);
    self.keyword = ko.observable('');
    self.listType = ko.observable(listType.inbox);

    self.currentPage = ko.observable(1);
    self.totalPage = ko.observable(1);
    self.recordPerPage = ko.observable(20);
    self.IsSendOut = ko.observable(false);

    self.totalInboxUnread = ko.observable(0);
    self.totalStarUnread = ko.observable(0);
    self.totalDraft = ko.observable(0);
    self.listMessage = ko.observableArray([]);
    self.attachments = ko.observableArray([]);

    self.currentFilter = ko.observable(filterType.all);
    self.currentCheckboxFilter = ko.observable('');
    self.filterTitle = ko.observable(resources.internalEmail.filter.title.format(resources.internalEmail.filter.all));

    self.loading = ko.observable(false);
    self.isShowCc = ko.observable(false);
    self.isShowBcc = ko.observable(false);
    self.isShowCompose = ko.observable(false);
    self.isCreateNewMessage = ko.observable(false);
    self.isShowList = ko.observable(true);
    self.isShowDetail = ko.observable(false);
    self.toFocus = ko.observable(false);
    self.ccFocus = ko.observable(false);
    self.bccFocus = ko.observable(false);
    self.subjectFocus = ko.observable(false);
    self.to = ko.observable('');
    self.cc = ko.observable('');
    self.bcc = ko.observable('');
    self.subject = ko.observable('');
    self.content = ko.observable('');
    self.id = ko.observable(0);
    self.sendTime = ko.observable('');
    self.attachmentCount = ko.observable(0);
    self.fromUser = ko.observable('');
    self.toUser = ko.observable('');
    self.ccToUser = ko.observable('');
    self.bccToUser = ko.observable('');
    self.selected = ko.observable(null);
    self.checkedMessageId = ko.observableArray();
    self.ShowMessageContent = ko.observable(false); // @HenryDo
    self.IsLoadingMore = ko.observable(false); // @HenryDo
    self.CurrentPage = ko.observable(1); // @HenryDo    
    self.IsSearching = ko.observable(false); // @HenryDo
    self.firstContentMessage = {};    

    self.saveDraftInterval = null;

    self.isShowCompose.subscribe(function (newValue) {
        if (newValue) {
            self.saveDraftInterval = setInterval(function () { self.saveDraft(); }, 15000);
        } else {
            clearInterval(self.saveDraftInterval);
        }
    });

    self.search = function (pageclickednumber) {
        self.IsSearching(true);
        var search = function () {
            self.loading(true);
            self.currentPage(pageclickednumber);
            var method, isRead, star;
            if (self.currentFilter() === filterType.readed) {
                isRead = true;
                star = null;
            } else if (self.currentFilter() === filterType.unRead) {
                isRead = false;
                star = null;
            } else if (self.currentFilter() === filterType.star) {
                isRead = null;
                star = true;
            } else {
                isRead = null;
                star = null;
            }
            switch (self.listType()) {
                case listType.inbox:
                    method = 'searchInbox';
                    self.title(resources.internalEmail.menu.inbox);
                    break;
                case listType.sent:
                    method = 'searchSent';
                    self.title(resources.internalEmail.menu.sentMail);
                    break;
                case listType.draft:
                    method = 'searchDraft';
                    self.title(resources.internalEmail.menu.drafts);
                    break;
                case listType.trash:
                    method = 'searchTrash';
                    self.title(resources.internalEmail.menu.trash);
                    break;
                case listType.star:
                    method = 'searchStar';
                    self.title(resources.internalEmail.menu.starred);
                    break;
                default:
                    method = 'searchInbox';
                    self.title(resources.internalEmail.menu.inbox);
                    break;
            }
            self.isShowCompose(false);
            self.isCreateNewMessage(false);
            self.isShowList(true);
            self.isShowDetail(false);
            messageModel[method](self.keyword(), isRead, star, self.currentPage(), self.recordPerPage(), function (result) {
                self.IsSearching(false);
                renderPage(self.currentPage(), result.totalRecord);
                self.checkedMessageId([]);
                self.loading(false);
                self.IsLoadingMore(false);

                formatList(result.items);

                _.each(result.items,
                    function (it) {
                        it.IsRead = ko.observable(it.IsRead);
                        it.Star = ko.observable(it.Star);
                    });

                self.listMessage(result.items);
            });
        };

        if (self.isShowCompose()) {
            var jsonFirst = JSON.stringify(self.firstContentMessage);
            var jsonLast = JSON.stringify({
                to: self.to(),
                cc: self.cc(),
                bcc: self.bcc(),
                subject: self.subject(),
                content: CKEDITOR.instances.messageContent.getData(),
                attachments: self.attachments()
            });
            if (jsonFirst !== jsonLast) {
                // @HenryDo
                swal({
                    title: "",
                    text: resources.internalEmail.message.confirmLeavePage,
                    showCancelButton: true,
                    confirmButtonText: "Agree",
                    cancelButtonText: "Cancel",
                    closeOnConfirm: true
                }).then(function () {
                    self.loading(true);
                    self.saveDraft(function () {
                        self.loading(false);
                        //window.onbeforeunload = null;
                        search();
                    });
                    }, function() {
                        //window.onbeforeunload = null;
                        if (self.isCreateNewMessage()) {
                            messageModel.discard([self.id()], token, function (result) {
                                if (result.length === 0) {
                                    toastr.error(resources.internalEmail.message.error);
                                } else {
                                    self.totalDraft(self.totalDraft() - 1);
                                    self.isShowCompose(false);
                                    self.isCreateNewMessage(false);
                                    //window.onbeforeunload = null;
                                }
                                search();
                                self.loading(false);
                            });
                        } else {
                            self.isShowCompose(false);
                            search();
                        }
                });
            } else {
                if (self.isCreateNewMessage()) {
                    messageModel.discard([self.id()], token, function (result) {
                        if (result.length === 0) {
                            toastr.error(resources.internalEmail.message.error);
                        } else {
                            self.totalDraft(self.totalDraft() - 1);
                            self.isShowCompose(false);
                            self.isCreateNewMessage(false);
                            //window.onbeforeunload = null;
                        }
                        search();
                        self.loading(false);
                    });
                } else {
                    search();
                }
            }

            // End @HenryDo

            //if (confirm(resources.internalEmail.message.confirmLeavePage)) {
            //    self.loading(true);
            //    self.saveDraft(function () {
            //        self.loading(false);
            //        window.onbeforeunload = null;
            //        search();
            //    });
            //} else {
            //    window.onbeforeunload = null;
            //    search();
            //}
        } else {
            search();
        }
    };

    self.searchSubmit = function () {
        self.search(1);
    };

    self.prevPage = function () {
        if (self.currentPage() > 1) {
            self.search(self.currentPage() - 1);
        }
    };

    self.nextPage = function () {
        if (self.currentPage() < self.totalPage()) {
            self.search(self.currentPage() + 1);
        }
    };

    self.detail = function (item) {
        self.loading(true);
        self.selected(item);
        self.id(item.Id);
        self.title(resources.internalEmail.title.detail);
        self.subject(item.Title);
        self.sendTime(item.SendTime);
        self.content('');
        self.toUser('');
        self.ccToUser('');
        self.bccToUser('');
        self.attachmentCount(item.AttachmentCount);
        self.attachments([]);
        var currentUserId = (window.currentUser).UserId;
        
        var fromUser = JSON.parse(item.FromUser);
        var toUser = JSON.parse(item.ToUser);

        // @HenryDo
        self.ShowMessageContent(true);
        if (fromUser.userId == currentUserId) {
            self.fromUser('<img style="width: 30px;height:30px" src="/Upload/Resize/' + (item.FromAvatar ? item.FromAvatar : 'L0NvbnRlbnQvaW1hZ2VzL25vbmUuanBn') + '_30x30_0">&nbsp;<span class="bold">' + resources.internalEmail.detailUserSend.me + '</span> ' + resources.internalEmail.detailUserSend.to + ' <span class="bold">' + toUser[0].fullName + '</span><span> &lt;' + toUser[0].userName + '&gt;</span>' + (toUser.length > 1 || item.CcToUser ? '...' : '') + ' ' + resources.internalEmail.detailUserSend.on + ' <span>' + item.SendTime + '</span>&nbsp;<a href="javascript://" onclick="detailUserSend(this)">' + resources.internalEmail.button.expandToUser + '</a>');
            //self.fromUser(resources.internalEmail.detailUserSend.me);
        } else {
            self.fromUser('<img style="width: 30px;height:30px" src="/Upload/Resize/' + (item.FromAvatar ? item.FromAvatar : 'L0NvbnRlbnQvaW1hZ2VzL25vbmUuanBn') + '_30x30_0">&nbsp;<span class="bold">' + fromUser.fullName + '</span><span> &lt;' + fromUser.userName + '&gt;</span> ' + resources.internalEmail.detailUserSend.to + ' <span class="bold"> ' + resources.internalEmail.detailUserSend.me + ' </span><span>' + (toUser.length > 1 || item.CcToUser ? '...' : '') + ' ' + resources.internalEmail.detailUserSend.on + ' </span>' + item.SendTime + '</span>&nbsp;<a href="javascript://" onclick="detailUserSend(this)">' + resources.internalEmail.button.expandToUser + '</a>');
            //self.fromUser(fromUser.fullName + '&lt;' + fromUser.userName + '&gt;');
        }
        // End @HenryDo

        var toUserStr = '', ccToUserStr = '', bccToUserStr = '';
        for (var i = 0; i < toUser.length; i++) {
            toUserStr += '<span>' + toUser[i].fullName + '</span><span> &lt;' + toUser[i].userName + '&gt;</span>';
            if (i < toUser.length - 1) {
                toUserStr += ', ';
            }
        }
        self.toUser(toUserStr);
        if (item.CcToUser) {
            var ccToUser = JSON.parse(item.CcToUser);
            for (var j = 0; j < ccToUser.length; j++) {

                ccToUserStr += '<span>' + ccToUser[j].fullName + '</span><span> &lt;' + ccToUser[j].userName + '&gt;</span>';
                if (j < ccToUser.length - 1) {
                    ccToUserStr += ', ';
                }
            }
            self.ccToUser(ccToUserStr);
        }

        self.isShowCompose(false);
        self.isShowList(false);
        self.isShowDetail(true);
        messageModel.detail(item.Id, function (result) {
            if (!result) {
                toastr.error(resources.internalEmail.message.notHavePermissionViewMail);
            } else {
                if (!item.Type) {
                    self.totalInboxUnread(self.totalInboxUnread() - 1);
                }
                if (item.Star()) {
                    self.totalStarUnread(self.totalStarUnread() - 1);
                }
                item.IsRead(true);
                self.content(result.content);
                self.attachments(result.attachments);
                if (result.bcc) {
                    var bccToUser = JSON.parse(result.bcc);
                    var bccStr = '';
                    for (var k = 0; k < bccToUser.length; k++) {
                        bccStr += bccToUser[k].userName;
                        bccToUserStr += '<span>' + bccToUser[k].fullName + '</span><span> &lt;' + bccToUser[k].userName + '&gt;</span>';
                        if (k < bccToUser.length - 1) {
                            bccToUserStr += ', ';
                            bccStr += ';';
                        }
                    }
                    self.bccToUser(bccToUserStr);
                    item.BccToUser = bccStr;
                }
            }
            self.loading(false);
        });
    };

    // @HenryDo
    self.getDetail = function (id) {
        messageModel.GetDetailByMessageId(id, function (result) {
            if (!result) {
                toastr.error(resources.internalEmail.message.notHavePermissionViewMail);
            } else {
                // @HenryDo
                formatList([result]);

                var model = result;
                model.IsRead = ko.observable(model.IsRead);
                model.Star = ko.observable(model.Star);

                var message = _.find(self.listMessage(), function (msg) {
                    return msg.Id === model.Id;
                });

                if (message == undefined) {
                    self.listMessage.unshift(model);
                } else {
                    message.IsRead(true);
                }

                self.detail(model);
            }
        });
    }
    // End @HenryDo

    self.changeListType = function (type) {
        self.listType(type);
        self.search(1);
    };

    self.changeShowCc = function () {
        self.isShowCc(!self.isShowCc());
        if (!self.isShowCc()) {
            self.cc('');
            reBindAutocomplete('cc');
        } else {
            setTimeout(function () { $('#cc').tagEditor('addTag', '', false); }, 200);
        }
    };

    self.changeShowBcc = function () {
        self.isShowBcc(!self.isShowBcc());

        if (!self.isShowBcc()) {
            self.bcc('');
            reBindAutocomplete('cc');
        } else {
            setTimeout(function () { $('#bcc').tagEditor('addTag', '', false); }, 200);
        }
    };

    self.changeFilter = function (filter) {
        if (filter === filterType.readed) {
            self.filterTitle(resources.internalEmail.filter.title.format(resources.internalEmail.filter.read));
            self.currentFilter(filterType.readed);
        } else if (filter === filterType.unRead) {
            self.filterTitle(resources.internalEmail.filter.title.format(resources.internalEmail.filter.unread));
            self.currentFilter(filterType.unRead);
        } else if (filter === filterType.star) {
            self.filterTitle(resources.internalEmail.filter.title.format(resources.internalEmail.filter.starred));
            self.currentFilter(filterType.star);
        } else {
            self.filterTitle(resources.internalEmail.filter.title.format(resources.internalEmail.filter.all));
            self.currentFilter(filterType.all);
        }

        self.search(1);
    };

    self.changeCheckboxFilter = function (filter) {
        var ids = [];
        if (filter === filterType.readed) {
            ids = _.map(_.map(_.filter(self.listMessage(), function (item) {
                return item.IsRead();
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.unRead) {
            ids = _.map(_.map(_.filter(self.listMessage(), function (item) {
                return !item.IsRead();
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.star) {
            ids = _.map(_.map(_.filter(self.listMessage(), function (item) {
                return item.Star();
            }), 'Id'), function (id) {
                return id.toString();
            });
        } else if (filter === filterType.all) {
            ids = _.map(_.map(self.listMessage(), 'ID'), function (id) {
                return id.toString();
            });
        }

        self.checkedMessageId(ids);
    };

    self.changeRecordPerPage = function (recordPerPage) {
        self.recordPerPage(recordPerPage);
        self.search(1);
    };

    self.compose = function () {
        // @HenryDo: Change editor content -> message-content, Change get token in input -> token
        self.loading(true);
        //if (!CKEDITOR.instances.messageContent) {
        //    CKEDITOR.replace('messageContent', { height: '340px', toolbarLocation: 'bottom', uiColor: '#FAFAFA', autoGrow_onStartup: false });
        //}

        //CKEDITOR.instances.messageContent.updateElement();
        //CKEDITOR.instances.messageContent.setData('');

        self.title(resources.internalEmail.menu.compose);
        reBindAutocomplete('cc');
        reBindAutocomplete('bcc');
        reBindAutocomplete('to');
        setTimeout(function () { $('#to').tagEditor('addTag', '', false); }, 200);

        self.subject('');
        self.firstContentMessage = {
            to: '',
            cc: '',
            bcc: '',
            subject: '',
            content: '',
            attachments: []
        };

        self.isShowCompose(true);
        self.isCreateNewMessage(true);
        self.isShowList(false);
        self.isShowDetail(false);
        self.attachmentCount(0);
        self.attachments([]);
        self.toFocus(true);
        messageModel.compose(null, token, function (result) {
            if (result <= 0) {
                toastr.error(resources.internalEmail.message.error);
                self.isShowCompose(false);
                self.isCreateNewMessage(false);
                self.isShowList(true);
                self.isShowDetail(false);
            }
            self.id(result);
            self.totalDraft(self.totalDraft() + 1);
            //window.onbeforeunload = confirmNavigation;
            bindUploadAttachment();
            self.loading(false);
        });
    };

    self.quickCompose = function () {
        messageModel.compose(null, token, function (result) {
            if (result <= 0) {
                toastr.error("Something is not working properly. Please contact the administrator.");
                self.isShowCompose(false);
                self.isCreateNewMessage(false);
                return;
            }
            self.id(result);
            self.totalDraft(self.totalDraft() + 1);
            bindUploadAttachment();
        });
    }

    self.composeContinue = function (item) {
        self.loading(true);
        if (!CKEDITOR.instances.messageContent) {
            CKEDITOR.replace('messageContent', { height: '340px', toolbarLocation: 'bottom', uiColor: '#FAFAFA', autoGrow_onStartup: true });
        }
        CKEDITOR.instances.messageContent.updateElement();
        CKEDITOR.instances.messageContent.setData('');
        self.selected(item);
        self.id(item.Id);
        self.title(resources.internalEmail.menu.compose);

        if (item.ToUser) {
            var splitTo = item.ToUser.split(/[,;]+/);
            reBindAutocomplete('to', splitTo);
            self.to($('#to').val());
        } else {
            reBindAutocomplete('to');
            self.to('');
            setTimeout(function () { $('#to').tagEditor('addTag', '', false); }, 200);
        }
        if (item.CcToUser) {
            self.isShowCc(true);
            var splitCc = item.CcToUser.split(/[,;]+/);
            reBindAutocomplete('cc', splitCc);
            self.cc($('#cc').val());
        } else {
            self.isShowCc(false);
            self.cc('');
        }

        self.subject(item.Title === resources.internalEmail.descriptionSubject.noSubject ? '' : item.Title);
        self.sendTime(item.SendTime);
        self.attachmentCount(item.AttachmentCount);
        self.attachments([]);
        self.isShowCompose(true);
        self.isCreateNewMessage(false);
        self.isShowList(false);
        self.isShowDetail(false);
        bindUploadAttachment();

        self.firstContentMessage = {
            to: self.to(),
            cc: self.cc(),
            bcc: self.bcc(),
            subject: self.subject(),
            content: '',
            attachments: []
        };
        //window.onbeforeunload = confirmNavigation;

        messageModel.detail(item.Id, function (result) {
            if (!result) {
                toastr.error(resources.internalEmail.message.notHavePermissionViewMail);
            } else {
                CKEDITOR.instances.messageContent.setData(result.content);
                if (result.bcc) {
                    self.isShowBcc(true);
                    self.bcc(result.bcc);
                } else {
                    self.isShowBcc(false);
                    self.bcc('');
                }

                if (result.bcc) {
                    self.isShowBcc(true);
                    var splitBcc = result.bcc.split(/[,;]+/);
                    reBindAutocomplete('bcc', splitBcc);
                    self.bcc($('#bcc').val());
                } else {
                    self.isShowBcc(false);
                    self.bcc('');
                }
                self.attachments(result.attachments);

                self.firstContentMessage.bcc = self.bcc();
                setTimeout(function () { self.firstContentMessage.content = CKEDITOR.instances.messageContent.getData(); }, 300);
                self.firstContentMessage.attachments = result.attachments;
            }
            self.loading(false);
        });
    };

    self.discard = function () {
        swal({
            title: "",
            text: resources.internalEmail.message.confirmDiscard,
            showCancelButton: true,
            confirmButtonText: "Agree",
            cancelButtonText: "Cancel",
            closeOnConfirm: true
        }).then(function () {
            self.loading(true);

            messageModel.discard([self.id()], token, function (result) {
                if (result.length === 0) {
                    toastr.error(resources.internalEmail.message.error);
                } else {
                    self.totalDraft(self.totalDraft() - 1);
                    self.isShowCompose(false);
                    self.isCreateNewMessage(false);
                    self.isShowList(true);
                    self.isShowDetail(false);
                    self.listMessage.remove(self.selected());
                    self.search(self.currentPage());
                    //window.onbeforeunload = null;
                }
                self.loading(false);
            });
        }, function () {});
    };

    self.discardMultiMessage = function () {
        if (self.checkedMessageId().length > 0) {
            swal({
                title: "",
                text: resources.internalEmail.message.confirmDiscardMultiple.format(self.checkedMessageId().length),
                showCancelButton: true,
                confirmButtonText: "Agree",
                cancelButtonText: "Cancel",
                closeOnConfirm: true
            }).then(function () {
                self.loading(true);

                messageModel.discard(self.checkedMessageId(), token, function (result) {
                    if (result.length === 0) {
                        toastr.error(resources.internalEmail.message.error);
                    } else {
                        self.totalDraft(self.totalDraft() - result.length);
                        self.search(self.currentPage());
                        self.checkedMessageId([]);
                    }
                    self.loading(false);
                });
            }, function () { });
            
            //if (confirm(resources.internalEmail.message.confirmDiscardMultiple.format(self.checkedMessageId().length))) {
            //    self.loading(true);
            //    messageModel.discard(self.checkedMessageId(), token, function (result) {
            //        if (result.length === 0) {
            //            toastr.error(resources.internalEmail.message.error);
            //        } else {
            //            self.totalDraft(self.totalDraft() - result.length);
            //            self.search(self.currentPage());
            //            self.checkedMessageId([]);
            //        }
            //        self.loading(false);
            //    });
            //}
        }
    };

    self.saveDraft = function (callback, isAlert) {
        self.loading(true);
        messageModel.saveDraft(self.id(), self.to(), self.cc(), self.bcc(), self.subject(), CKEDITOR.instances.messageContent.getData(), token, function (result) {
            if (result <= 0) {
                toastr.error(resources.internalEmail.message.error);
            } else {
                if (callback) {
                    callback();
                }
                if (isAlert) {
                    toastr.success(resources.internalEmail.message.saveDraffSuccess);
                }
            }
            self.loading(false);
        });
    };

    self.send = function () {
        var regex = /^\s*((\s*[a-zA-Z0-9\._%-]{1,100}\s*[,;]{1}\s*){1,100000}?)?([a-zA-Z0-9\._%-]{1,100})\s*$/;

        if (!self.to()) {
            toastr.error(resources.internalEmail.message.toRequired);
            setTimeout(function () { $('#to').tagEditor('addTag', '', false); }, 200);
            return;
        }
        if (!regex.test(self.to())) {
            toastr.error(resources.internalEmail.message.regexListUser);
            self.toFocus(true);
            return;
        }
        if (CKEDITOR.instances.messageContent.getData() == '') {
            toastr.error(resources.internalEmail.message.contentRequired);
            CKEDITOR.instances.messageContent.focus();
            return;
        }
        if (!self.subject()) {
            toastr.error(resources.internalEmail.message.subjectRequired);
            self.subjectFocus(true);
            return;
        }
        if (self.cc()) {
            if (!regex.test(self.cc())) {
                toastr.error(resources.internalEmail.message.regexListUserCc);
                setTimeout(function () { $('#cc').tagEditor('addTag', '', false); }, 200);
                return;
            }
        }
        if (self.bcc()) {
            if (!regex.test(self.bcc())) {
                toastr.error(resources.internalEmail.message.regexListUserBcc);
                setTimeout(function () { $('#bcc').tagEditor('addTag', '', false); }, 200);
                return;
            }
        }
        self.loading(true);
        messageModel.send(self.id(), self.to(), self.cc(), self.bcc(), self.subject(), CKEDITOR.instances.messageContent.getData(), self.IsSendOut(), token, function (result) {
            if ($.isArray(result)) {
                toastr.error(resources.internalEmail.message.recipientNotExist);
                $.each(result, function (i, item) {
                    var tagError = _.filter($('.tag-editor-tag'), function (elem) {
                        return $(elem).text().toLowerCase() == item;
                    });
                    $.each(tagError, function (j, tag) {
                        $(tag).parent().addClass('red-tag');
                    });
                });
            } else if (result == -2) {
                toastr.warning(resources.internalEmail.message.notHavePermissionSendMail);
            } else if (result == -3) {
                toastr.error(resources.internalEmail.message.toRequired);
                self.toFocus(true);
            } else if (result == -4) {
                toastr.error(resources.internalEmail.message.subjectRequired);
                self.subjectFocus(true);
            } else if (result == -5) {
                toastr.error(resources.internalEmail.message.contentRequired);
                CKEDITOR.instances.messageContent.focus();
            } else {
                toastr.success(resources.internalEmail.message.sendMailSuccess);
                self.totalDraft(self.totalDraft() - 1);
                self.isShowCompose(false);
                self.isCreateNewMessage(false);
                self.isShowList(true);
                self.isShowDetail(false);
                if (self.listType() === listType.draft) {
                    var message = _.find(self.listMessage(), function (item) {
                        return item.Id == self.id();
                    });
                    if (message) {
                        self.listMessage.remove(message);
                    }
                }
                //window.onbeforeunload = null;
            }
            self.firstContentMessage = {};
            self.loading(false);
        });
    };

    self.removeAttachment = function (item) {
        // @HenryDo
        swal({
            title: "",
            text: resources.internalEmail.message.confirmRemoveAttachment,
            showCancelButton: true,
            confirmButtonText: "Agree",
            cancelButtonText: "Cancel",
            closeOnConfirm: true
        }).then(function () {
            self.loading(true);
            messageModel.removeAttachment(item.Id, self.id(), token, function (result) {
                if (result == -2) {
                    toastr.warning(resources.internalEmail.message.notHavePermissionRemoveAttachment);
                } else {
                    self.attachmentCount(self.attachmentCount() - 1);
                    self.attachments.remove(item);
                }
                self.loading(false);
            });
            }, function () { });
        // End @HenryDo

        //if (confirm(resources.internalEmail.message.confirmRemoveAttachment)) {
        //    self.loading(true);
        //    messageModel.removeAttachment(item.Id, self.id(), token, function (result) {
        //        if (result == -2) {
        //            toastr.warning(resources.internalEmail.message.notHavePermissionRemoveAttachment);
        //        } else {
        //            self.attachmentCount(self.attachmentCount() - 1);
        //            self.attachments.remove(item);
        //        }
        //        self.loading(false);
        //    });
        //}
    };

    self.downloadAttachment = function (item) {
        document.location = '/message/downloadattachment?id=' + item.Id + '&messageId=' + self.id();
    }

    self.reply = function (isReplyAll) {
        self.loading(true);
        self.isShowCompose(true); // @HenryDo
        self.isCreateNewMessage(true);
        self.ShowMessageContent(false); // @HenryDo
        var fromUser = JSON.parse(self.selected().FromUser);
        var setContent = function (editor) {
            editor.updateElement();
            editor.setData('<p id="start"></p><p>-------------------- ' + resources.internalEmail.prefixReply.on + ' ' + self.selected().SendTime + ' ' + fromUser.fullName + '  &lt;' + fromUser.userName + '&gt; ' + resources.internalEmail.prefixReply.wrote + '--------------------</p>' + self.content());
            setTimeout(focusTopCkeditor, 300);
        };
        if (!CKEDITOR.instances.messageContent) {
            CKEDITOR.replace('messageContent', {
                height: '340px', toolbarLocation: 'bottom', uiColor: '#FAFAFA', autoGrow_onStartup: true,
                on: {
                    instanceReady: function (evt) {
                        setContent(evt.editor);
                    }
                }
            });
        } else {
            setContent(CKEDITOR.instances.messageContent);
        }

        self.title(resources.internalEmail.menu.compose);
        self.subject('Re: ' + self.selected().Title);
        self.attachmentCount(0);
        self.attachments([]);
        var currentUserId = (window.currentUser).UserId;
        if (currentUserId == fromUser.userId) {
            var toUser = _.map(JSON.parse(self.selected().ToUser), 'userName');
            reBindAutocomplete('to', toUser);
            self.to($('#to').val());
        } else {
            reBindAutocomplete('to', [fromUser.userName]);
            self.to($('#to').val());
        }
        reBindAutocomplete('cc', []);
        reBindAutocomplete('bcc', []);
        self.isShowCc(false);
        self.isShowBcc(false);

        if (isReplyAll) {
            var ccToUser = self.selected().CcToUser ? JSON.parse(self.selected().CcToUser) : null;
            if (ccToUser) {
                reBindAutocomplete('cc', _.map(ccToUser, 'userName'));
                self.cc($('#cc').val());
                self.isShowCc(true);
            }
            if (self.selected().BccToUser) {
                var splitBcc = self.selected().BccToUser.split(/[,;]+/);
                reBindAutocomplete('bcc', _.map(splitBcc, 'userName'));
                self.bcc($('#bcc').val());
                self.isShowBcc(true);
            }
        }

        self.isShowCompose(true);
        self.isShowList(false);
        self.isShowDetail(false);
        self.firstContentMessage = {
            to: self.to(),
            cc: self.cc(),
            bcc: self.bcc(),
            subject: self.subject(),
            content: CKEDITOR.instances.messageContent.getData(),
            attachments: self.attachments()
        };
        setTimeout(function () { self.firstContentMessage.content = CKEDITOR.instances.messageContent.getData(); }, 500);

        messageModel.compose(null, token, function (result) {
            if (result <= 0) {
                toastr.error(resources.internalEmail.message.error);
                self.isShowCompose(false);
                self.isCreateNewMessage(false);
                self.isShowList(true);
                self.isShowDetail(false);
            }
            self.id(result);
            self.totalDraft(self.totalDraft() + 1);
            //window.onbeforeunload = confirmNavigation;
            bindUploadAttachment();
            self.saveDraft(null);
            self.loading(false);
        });
    };

    self.forward = function () {
        self.loading(true);
        self.isShowCompose(true); // @HenryDo
        self.isCreateNewMessage(true);
        self.ShowMessageContent(false); // @HenryDo

        var fromUser = JSON.parse(self.selected().FromUser);
        var setContent = function (editor) {
            editor.updateElement();
            editor.setData('<p id="start"></p><p>-------------------- ' + resources.internalEmail.prefixReply.on + ' ' + self.selected().SendTime + ' ' + fromUser.fullName + '  &lt;' + fromUser.userName + '&gt; ' + resources.internalEmail.prefixReply.wrote + '--------------------</p>' + self.content());
            editor.focus();
        };
        if (!CKEDITOR.instances.messageContent) {
            CKEDITOR.replace('messageContent', {
                height: '340px', toolbarLocation: 'bottom', uiColor: '#FAFAFA',
                on: {
                    instanceReady: function (evt) {
                        setContent(evt.editor);
                    }
                }
            });
        } else {
            setContent(CKEDITOR.instances.messageContent);
        }
        reBindAutocomplete('to');
        reBindAutocomplete('cc');
        reBindAutocomplete('bcc');
        setTimeout(function () { $('#to').tagEditor('addTag', '', false); }, 200);
        self.title(resources.internalEmail.menu.compose);
        self.subject('Fw: ' + self.selected().Title);
        self.isShowCompose(true);
        self.isShowList(false);
        self.isShowDetail(false);
        self.firstContentMessage = {
            to: self.to(),
            cc: self.cc(),
            bcc: self.bcc(),
            subject: self.subject(),
            content: CKEDITOR.instances.messageContent.getData(),
            attachments: self.attachments()
        };
        setTimeout(function () { self.firstContentMessage.content = CKEDITOR.instances.messageContent.getData(); }, 500);

        messageModel.compose(self.selected().Id, token, function (result) {
            if (result <= 0) {
                toastr.error(resources.internalEmail.message.error);
                self.isShowCompose(false);
                self.isCreateNewMessage(false);
                self.isShowList(true);
                self.isShowDetail(false);
            }
            self.id(result);
            self.totalDraft(self.totalDraft() + 1);
            //window.onbeforeunload = confirmNavigation;
            bindUploadAttachment();
            self.saveDraft(null);
            self.loading(false);
        });
    };

    self.checkAllMessage = ko.computed({
        read: function () {
            return self.checkedMessageId().length == self.listMessage().length && self.listMessage().length > 0;
        },
        write: function (value) {
            self.checkedMessageId([]);
            if (value) {
                ko.utils.arrayForEach(self.listMessage(), function (item) {
                    self.checkedMessageId.push('' + item.Id);
                });
            }
        }
    });

    self.deleteMessage = function (isInDetail) {
        if (isInDetail) {
            if (self.listType() === listType.trash) {
                // @HenryDo
                swal({
                    title: "",
                    text: resources.internalEmail.message.confirmDelete,
                    showCancelButton: true,
                    confirmButtonText: "Agree",
                    cancelButtonText: "Cancel",
                    closeOnConfirm: true
                }).then(function () {
                    self.loading(true);

                    messageModel.delete([self.id()], token, function (result) {
                        if (result.length === 0) {
                            toastr.error(resources.internalEmail.message.error);
                        } else {
                            var message = _.find(self.listMessage(), function (item) {
                                return item.Id == self.id() && !item.IsRead();
                            });
                            if (message) {
                                if (!message.Type) {
                                    self.totalInboxUnread(self.totalInboxUnread() - 1);
                                }
                                if (message.Star()) {
                                    self.totalStarUnread(self.totalStarUnread() - 1);
                                }
                            }

                            

                            if (_.some(result, function (num) { return num === self.id();})) {
                                self.id("");
                                self.fromUser("");
                                self.toUser("");
                                self.cc("");
                                self.bcc("");
                                self.ShowMessageContent(false);
                            }

                            self.search(self.currentPage());
                            self.isShowCompose(false);
                            self.isCreateNewMessage(false);
                            self.isShowList(true);
                            self.isShowDetail(false);
                        }
                        self.loading(false);
                    });
                    }, function () { });
                // End @HenryDo

                //if (confirm(resources.internalEmail.message.confirmDelete)) {
                //    self.loading(true);
                //    messageModel.delete([self.id()], token, function (result) {
                //        if (result.length === 0) {
                //            toastr.error(resources.internalEmail.message.error);
                //        } else {
                //            var message = _.find(self.listMessage(), function (item) {
                //                return item.Id == self.id() && !item.IsRead();
                //            });
                //            if (message) {
                //                if (!message.Type) {
                //                    self.totalInboxUnread(self.totalInboxUnread() - 1);
                //                }
                //                if (message.Star()) {
                //                    self.totalStarUnread(self.totalStarUnread() - 1);
                //                }
                //            }
                //            self.search(self.currentPage());
                //            self.isShowCompose(false);
                //            self.isShowList(true);
                //            self.isShowDetail(false);
                //        }
                //        self.loading(false);
                //    });
                //}
            } else {
                self.moveTrash(true);
            }

        } else {
            if (self.listType() === listType.draft) {
                self.discardMultiMessage();
            } else if (self.listType() === listType.trash) {
                if (self.checkedMessageId().length > 0) {
                    // @HenryDo
                    swal({
                        title: "",
                        text: resources.internalEmail.message.confirmDeleteForeverMultiple.format(self.checkedMessageId().length),
                        showCancelButton: true,
                        confirmButtonText: "Agree",
                        cancelButtonText: "Cancel",
                        closeOnConfirm: true
                    }).then(function () {
                        self.loading(true);

                        messageModel.delete(self.checkedMessageId(), token, function (result) {
                            if (result.length === 0) {
                                toastr.error(resources.internalEmail.message.error);
                            } else {
                                if (_.some(result, function (num) { return num === self.id(); })) {
                                    self.id("");
                                    self.fromUser("");
                                    self.toUser("");
                                    self.cc("");
                                    self.bcc("");
                                    self.ShowMessageContent(false);
                                }

                                self.search(self.currentPage());
                                self.checkedMessageId([]);
                            }
                            self.loading(false);
                        });
                        }, function () { });
                    // End @HenryDo

                    //if (confirm(resources.internalEmail.message.confirmDeleteForeverMultiple.format(self.checkedMessageId().length))) {
                    //    self.loading(true);
                    //    messageModel.delete(self.checkedMessageId(), token, function (result) {
                    //        if (result.length === 0) {
                    //            toastr.error(resources.internalEmail.message.error);
                    //        } else {
                    //            self.search(self.currentPage());
                    //            self.checkedMessageId([]);
                    //        }
                    //        self.loading(false);
                    //    });
                    //}
                }
            } else {
                self.moveTrash(false);
            }
        }
    };

    self.moveTrash = function (isInDetail) {
        if (isInDetail) {
            // @HenryDo
            swal({
                title: "",
                text: resources.internalEmail.message.confirmDelete,
                showCancelButton: true,
                confirmButtonText: "Agree",
                cancelButtonText: "Cancel",
                closeOnConfirm: true
            }).then(function () {
                self.loading(true);

                messageModel.moveTrash([self.id()], token, function (result) {
                    if (result.length === 0) {
                        toastr.error(resources.internalEmail.message.error);
                    } else {
                        var message = _.find(self.listMessage(), function (item) {
                            return item.Id == self.id() && !item.IsRead();
                        });
                        if (message) {
                            if (!message.Type) {
                                self.totalInboxUnread(self.totalInboxUnread() - 1);
                            }
                            if (message.Star()) {
                                self.totalStarUnread(self.totalStarUnread() - 1);
                            }
                        }

                        if (_.some(result, function (num) { return num === self.id(); })) {
                            self.id("");
                            self.fromUser("");
                            self.toUser("");
                            self.cc("");
                            self.bcc("");
                            self.ShowMessageContent(false);
                        }

                        self.search(self.currentPage());
                        self.isShowCompose(false);
                        self.isCreateNewMessage(false);
                        self.isShowList(true);
                        self.isShowDetail(false);
                    }
                    self.loading(false);
                });
            }, function () { });
            // End @HenryDo

            //if (confirm(resources.internalEmail.message.confirmDelete)) {
            //    self.loading(true);
            //    messageModel.moveTrash([self.id()], token, function (result) {
            //        if (result.length === 0) {
            //            toastr.error(resources.internalEmail.message.error);
            //        } else {
            //            var message = _.find(self.listMessage(), function (item) {
            //                return item.Id == self.id() && !item.IsRead();
            //            });
            //            if (message) {
            //                if (!message.Type) {
            //                    self.totalInboxUnread(self.totalInboxUnread() - 1);
            //                }
            //                if (message.Star()) {
            //                    self.totalStarUnread(self.totalStarUnread() - 1);
            //                }
            //            }
            //            self.search(self.currentPage());
            //            self.isShowCompose(false);
            //            self.isShowList(true);
            //            self.isShowDetail(false);
            //        }
            //        self.loading(false);
            //    });
            //}
        } else {
            if (self.checkedMessageId().length > 0) {
                swal({
                    title: "",
                    text: resources.internalEmail.message.confirmDeleteMultiple.format(self.checkedMessageId().length),
                    showCancelButton: true,
                    confirmButtonText: "Agree",
                    cancelButtonText: "Cancel",
                    closeOnConfirm: true
                }).then(function () {
                    self.loading(true);

                    messageModel.moveTrash(self.checkedMessageId(), token, function (result) {
                        if (result.length === 0) {
                            toastr.error(resources.internalEmail.message.error);
                        } else {
                            var currentUserId = (window.currentUser).UserId;
                            var messages = _.filter(self.listMessage(), function (item) {
                                return _.some(result, function (num) { return num == item.Id; }) && !item.Type && !item.IsRead();
                            });
                            if (messages.length > 0) {
                                self.totalInboxUnread(self.totalInboxUnread() - messages.length);
                            }
                            var stars = _.filter(messages, function (item) {
                                return item.Star();
                            });

                            if (_.some(result, function (num) { return num === self.id(); })) {
                                self.id("");
                                self.fromUser("");
                                self.toUser("");
                                self.cc("");
                                self.bcc("");
                                self.ShowMessageContent(false);
                            }

                            self.totalStarUnread(self.totalStarUnread() - stars.length);
                            self.search(self.currentPage());
                            self.checkedMessageId([]);
                        }
                        self.loading(false);
                    });
                    }, function () { });
                //if (confirm(resources.internalEmail.message.confirmDeleteMultiple.format(self.checkedMessageId().length))) {
                //    self.loading(true);
                //    messageModel.moveTrash(self.checkedMessageId(), token, function (result) {
                //        if (result.length === 0) {
                //            toastr.error(resources.internalEmail.message.error);
                //        } else {
                //            var currentUserId = (window.currentUser).UserId;
                //            var messages = _.filter(self.listMessage(), function (item) {
                //                return _.contains(result, item.Id) && !item.Type && !item.IsRead();
                //            });
                //            if (messages.length > 0) {
                //                self.totalInboxUnread(self.totalInboxUnread() - messages.length);
                //            }
                //            var stars = _.filter(messages, function (item) {
                //                return item.Star();
                //            });
                //            self.totalStarUnread(self.totalStarUnread() - stars.length);
                //            self.search(self.currentPage());
                //            self.checkedMessageId([]);
                //        }
                //        self.loading(false);
                //    });
                //}
            }
        }
    };

    self.undoMoveTrash = function () {
        if (self.checkedMessageId().length > 0) {
            // @HenryDo
            swal({
                title: "",
                text: resources.internalEmail.message.confirmRestoreMultiple.format(self.checkedMessageId().length),
                showCancelButton: true,
                confirmButtonText: "Agree",
                cancelButtonText: "Cancel",
                closeOnConfirm: true
            }).then(function () {
                self.loading(true);
                messageModel.undoMoveTrash(self.checkedMessageId(), token, function (result) {
                    if (result.length === 0) {
                        toastr.error(resources.internalEmail.message.error);
                    } else {
                        var currentUserId = (window.currentUser).UserId;
                        var messages = _.filter(self.listMessage(), function (item) {
                            return _.some(result, function (num) { return num === item.Id; }) && !item.Type && !item.IsRead();
                        });
                        if (messages.length > 0) {
                            self.totalInboxUnread(self.totalInboxUnread() + messages.length);
                        }
                        var stars = _.filter(messages, function (item) {
                            return item.Star();
                        });

                        self.totalStarUnread(self.totalStarUnread() + stars.length);
                        self.search(self.currentPage());
                        self.checkedMessageId([]);
                    }
                    self.loading(false);
                });
                }, function () { });

            // End @HenryDo

            //if (confirm(resources.internalEmail.message.confirmRestoreMultiple.format(self.checkedMessageId().length))) {
            //    self.loading(true);
            //    messageModel.undoMoveTrash(self.checkedMessageId(), token, function (result) {
            //        if (result.length === 0) {
            //            toastr.error(resources.internalEmail.message.error);
            //        } else {
            //            var currentUserId = (window.currentUser).UserId;
            //            var messages = _.filter(self.listMessage(), function (item) {
            //                return _.contains(result, item.Id) && !item.Type && !item.IsRead();
            //            });
            //            if (messages.length > 0) {
            //                self.totalInboxUnread(self.totalInboxUnread() + messages.length);
            //            }
            //            var stars = _.filter(messages, function (item) {
            //                return item.Star();
            //            });
            //            self.totalStarUnread(self.totalStarUnread() + stars.length);
            //            self.search(self.currentPage());
            //            self.checkedMessageId([]);
            //        }
            //        self.loading(false);
            //    });
            //}
        }
    };

    self.markReaded = function (isRead) {
        if (self.checkedMessageId().length > 0) {
            var messages = _.filter(self.listMessage(), function (item) {
                return _.some(self.checkedMessageId(), function (num) { return num === item.Id.toString(); }) && item.IsRead() == !isRead && item.SendTime != '';
            });
            if (messages.length > 0) {
                self.loading(true);
                messageModel.markReaded(_.map(messages, 'Id'), isRead, token, function (result) {
                    if (result.length === 0) {
                        toastr.error(resources.internalEmail.message.error);
                    } else {
                        var messagesSucess = _.filter(self.listMessage(), function (item) {
                            return _.some(result, function (num) { return num === item.Id; });
                        });
                        if (messagesSucess.length > 0) {
                            var currentUserId = (window.currentUser).UserId;
                            $.each(messagesSucess, function (i, item) {
                                item.IsRead(isRead);
                                var add = isRead ? -1 : 1;
                                if (self.listType() !== listType.trash) {
                                    if (!item.Type) {
                                        self.totalInboxUnread(self.totalInboxUnread() + add);
                                    }
                                    if (item.Star()) {
                                        self.totalStarUnread(self.totalStarUnread() + add);
                                    }
                                }
                            });
                        }
                        self.checkedMessageId([]);
                    }
                    self.loading(false);
                });
            }
        }
    };

    self.setCheck = function (id) {
        $("input[value=" + id + "]").click();
    };

    self.setStar = function (item, star) {
        //  self.loading(true);
        messageModel.setStar(item.Id, star, token, function (result) {
            if (result < 0) {
                toastr.error(resources.internalEmail.message.error);
            } else {
                item.Star(star);
                if (!item.IsRead() && item.SendTime != '') {
                    if (self.listType() !== listType.trash) {
                        var add = star ? 1 : -1;
                        self.totalStarUnread(self.totalStarUnread() + add);
                    }
                }
            }
            //  self.loading(false);
        });
    };

    var isChooseUserLoaded = false;

    self.listChooseUser = ko.observableArray([]);
    self.checkedUserName = ko.observableArray();

    self.checkAllUserName = ko.computed({
        read: function () {
            return self.checkedUserName().length == self.listChooseUser().length && self.listChooseUser().length > 0;
        },
        write: function (value) {
            self.checkedUserName([]);
            if (value) {
                ko.utils.arrayForEach(self.listChooseUser(), function (item) {
                    self.checkedUserName.push('' + item.UserName);
                });
            }
        }
    });

    self.clickCheckAllUserName = function () {
        //console.log(self.checkAllUserName());
    };

    self.officeId = ko.observable('');
    self.modalName = ko.observable('');

    var firstOfficeSubscribe = false;
    self.officeId.subscribe(function (newValue) {
        if (!firstOfficeSubscribe) {
            firstOfficeSubscribe = true;
        } else {
            messageModel.getUserByOfficeId(newValue, listOffice[newValue][0].idPath ? listOffice[newValue][0].idPath : null, function (result) {
                result = _.sortBy(result, function (it) { return it.FullName; });
                self.listChooseUser(result);
                self.checkedUserName([]);
                $('#tableChooseUser tbody input[type=checkbox]').click(function () {
                    if ($(this).prop('checked')) {
                        $('#chooseUser').tagEditor('addTag', $(this).val(), true);
                    } else {
                        $('#chooseUser').tagEditor('removeTag', $(this).val(), true);
                    }
                });
            });
        }
    });

    self.chooseUser = function () {
        var tags = $('#chooseUser').tagEditor('getTags')[0].tags;
        for (var i = 0; i < tags.length; i++) {
            $('#' + self.modalName()).tagEditor('addTag', tags[i]);
        }
    };

    self.showChooseUserModal = function (name) {
        self.modalName(name);
        if (isChooseUserLoaded) {
            $('#chooseUser').tagEditor('destroy');
            $('#chooseUser').val('');
            $('#chooseUser').tagEditor({
                initialTags: $('#' + self.modalName()).tagEditor('getTags')[0].tags,
                delimiter: ';',
                placeholder: '',
                forceLowercase: true,
                onChange: function () {
                    self[name]($('#' + name).val());
                },
                autocomplete: {
                    autoFocus: true,
                    delay: 300,
                    position: { collision: 'flip' },
                    source: '/user/searchfullname',
                    //focus: function (event, ui) {
                    //    $('.active', $('.tag-editor')).find('input').val(ui.item.UserName);
                    //    return false;
                    //},
                    select: function (event, ui) {
                        $('.tag-editor-tag.active .ui-autocomplete-input').val(ui.item.UserName).parents('ul:first').click();
                        return false;
                    },
                    _renderItem: function (ul, item1) {
                        return renderItemUserAutocomplete(ul, item1);
                    }
                }
            });
            self.checkedUserName([]);
        } else {
            $('#chooseUser').tagEditor({
                initialTags: $('#' + self.modalName()).tagEditor('getTags')[0].tags,
                delimiter: ';',
                placeholder: '',
                forceLowercase: true,
                onChange: function () {
                    self[name]($('#' + name).val());
                },
                autocomplete: {
                    autoFocus: true,
                    delay: 300,
                    position: { collision: 'flip' },
                    source: '/user/searchfullname',
                    //focus: function (event, ui) {
                    //    $('.active', $('.tag-editor')).find('input').val(ui.item.UserName);
                    //    return false;
                    //},
                    select: function (event, ui) {
                        $('.tag-editor-tag.active .ui-autocomplete-input').val(ui.item.UserName).parents('ul:first').click();
                        return false;
                    },
                    _renderItem: function (ul, item1) {
                        return renderItemUserAutocomplete(ul, item1);
                    }
                }
            });
            $('#tableChooseUser thead input[type=checkbox]').click(function () {
                if (self.checkAllUserName()) {
                    for (var i = 0; i < self.checkedUserName().length; i++) {
                        $('#chooseUser').tagEditor('addTag', self.checkedUserName()[i], true);
                    }
                } else {
                    $('#chooseUser').tagEditor('destroy');
                    $('#chooseUser').val('');
                    $('#chooseUser').tagEditor({
                        initialTags: [],
                        delimiter: ';',
                        placeholder: ''
                    });
                }
            });
            messageModel.getOfficeAndUser(function (result) {
                isChooseUserLoaded = true;
                self.listChooseUser(_.sortBy(result.listUser, function (it) { return it.FullName; }));
                var $body = $('#chooseUserModal .modal-body');
                var height = $(window).height() - ($('#chooseUserModal .modal-header').outerHeight() + $('#chooseUserModal .modal-footer').outerHeight()) - ($body.outerHeight() - $body.height());
                $body.attr('style', 'overflow: auto !important;height: ' + height + 'px !important');

                $('#tableChooseUser tbody input[type=checkbox]').click(function () {
                    if ($(this).prop('checked')) {
                        $('#chooseUser').tagEditor('addTag', $(this).val(), true);
                    } else {
                        $('#chooseUser').tagEditor('removeTag', $(this).val(), true);
                    }
                });
                listOffice = _.groupBy(result.listOffice, function (it) { return it.id; });
                $('#officesDropdown').dropdownjstree({
                    source: result.listOffice,
                    selectNote: function (node, selected) {
                        if (selected.selected[0] == self.officeId()) {
                            return;
                        }

                        self.officeId(selected.selected[0]);
                    },
                });

                //var $office = $('#Office');
                //var renderOffice = function (listParentOffice, level) {
                //    var space = '';
                //    if (level > 0) {
                //        for (var i = 1; i <= level; i++) {
                //            space += '&nbsp;&nbsp;&nbsp;';
                //        }
                //    }
                //    $.each(listParentOffice, function (idx, office) {
                //        $office.append('<option value="' + office.Id + '">' + space + office.Name + '</option>');
                //        var childrenOffice = _.filter(result.listOffice, function (item) {
                //            return item.ParrentOfficeId === office.Id;
                //        });
                //        if (childrenOffice.length > 0) {
                //            renderOffice(childrenOffice, level + 1);
                //        }
                //    });
                //}
                //var rootOffice = _.filter(result.listOffice, function (item) {
                //    return item.ParrentOfficeId === -1;
                //});
                //if (rootOffice.length > 0) {
                //    renderOffice(rootOffice, 0);
                //}
            });
        }
        $('#chooseUserModal').modal('show');
    };

    // @HenryDo
    self.LoadMore = function () {
        self.IsLoadingMore(true);
        self.search(self.CurrentPage() + 1);
    }
    // End @HenryDo

    $(document).ready(function () {
        if (listMessage == null && messageDetail != null) {
            var item = messageDetail.message;
            item.IsRead = ko.observable(true);
            item.Star = ko.observable(false);
            var sendTime = moment(item.SendTime).format(resources.common.defaultFormat.dateTime);
            item.SendTime = sendTime == 'Invalid date' ? '' : sendTime;
            self.selected(item);
            self.id(item.Id);
            self.title(resources.internalEmail.title.detail);
            self.subject(item.Title);
            self.sendTime(item.SendTime);
            self.content(item.Content);
            self.toUser('');
            self.ccToUser('');
            self.bccToUser('');
            self.attachmentCount(item.AttachmentCount);
            self.attachments(messageDetail.attachments);
            var currentUserId = (window.currentUser).UserId;
            var fromUser = JSON.parse(item.FromUser);
            var toUser = JSON.parse(item.ToUser);
            if (fromUser.userId == currentUserId) {
                self.fromUser('<img style="width: 30px;height:30px" src="/Upload/Resize/' + (item.FromAvatar ? item.FromAvatar : 'L0NvbnRlbnQvaW1hZ2VzL25vbmUuanBn') + '_30x30_0">&nbsp;<span class="bold">' + resources.internalEmail.detailUserSend.me + '</span> ' + resources.internalEmail.detailUserSend.to + ' <span class="bold">' + toUser[0].fullName + '</span><span> &lt;' + toUser[0].userName + '&gt;</span>' + (toUser.length > 1 || item.CcToUser ? '...' : '') + ' ' + resources.internalEmail.detailUserSend.on + ' <span>' + item.SendTime + '</span>&nbsp;<a href="#" onclick="detailUserSend(this)">' + resources.internalEmail.button.expandToUser + '</a>');
            } else {
                self.fromUser('<img style="width: 30px;height:30px" src="/Upload/Resize/' + (item.FromAvatar ? item.FromAvatar : 'L0NvbnRlbnQvaW1hZ2VzL25vbmUuanBn') + '_30x30_0">&nbsp;<span class="bold">' + fromUser.fullName + '</span><span> &lt;' + fromUser.userName + '&gt;</span> ' + resources.internalEmail.detailUserSend.to + ' <span class="bold"> ' + resources.internalEmail.detailUserSend.me + ' </span><span>' + (toUser.length > 1 || item.CcToUser ? '...' : '') + ' ' + resources.internalEmail.detailUserSend.on + ' </span>' + item.SendTime + '</span>&nbsp;<a href="#" onclick="detailUserSend(this)">' + resources.internalEmail.button.expandToUser + '</a>');
            }
            var toUserStr = '', ccToUserStr = '';
            for (var i = 0; i < toUser.length; i++) {
                toUserStr += '<span>' + toUser[i].fullName + '</span><span> &lt;' + toUser[i].userName + '&gt;</span>';
                if (i < toUser.length - 1) {
                    toUserStr += ', ';
                }
            }
            self.toUser(toUserStr);
            if (item.CcToUser) {
                var ccToUser = JSON.parse(item.CcToUser);
                for (var j = 0; j < ccToUser.length; j++) {

                    ccToUserStr += '<span>' + ccToUser[j].fullName + '</span><span> &lt;' + ccToUser[j].userName + '&gt;</span>';
                    if (j < ccToUser.length - 1) {
                        ccToUserStr += ', ';
                    }
                }
                self.ccToUser(ccToUserStr);
            }

            self.isShowCompose(false);
            self.isShowList(false);
            self.isShowDetail(true);
        } else if (messageDetail == null) {
            self.listType(listType.inbox);
            self.title(resources.internalEmail.menu.inbox);
            formatList(listMessage.items);

            _.each(listMessage.items,
                function (it) {
                    it.IsRead = ko.observable(it.IsRead);
                    it.Star = ko.observable(it.Star);
                });

            self.listMessage(listMessage.items);
            renderPage(self.currentPage(), listMessage.totalRecord);
        }
        self.totalInboxUnread(totalInboxUnread);
        self.totalStarUnread(totalStarUnread);
        self.totalDraft(totalDraft);
        bindAutocomplete('to');
        bindAutocomplete('cc');
        bindAutocomplete('bcc');

        $('#chooseUserModal').on('shown', function () {
            var $body = $('#chooseUserModal .modal-body');
            var height = $(window).height() - ($('#chooseUserModal .modal-header').outerHeight() + $('#chooseUserModal .modal-footer').outerHeight()) - ($body.outerHeight() - $body.height());
            $body.attr('style', 'overflow: auto !important;height: ' + height + 'px !important');

            $body.find('#tableChooseUser').parent().height(height - 150);
        });

        var isFirefox = typeof InstallTrigger !== 'undefined';
        if (isFirefox) {
            var $subject = $('input[name=subject]');
            var $hiddenSubjectButton = $('<button style="width: 0px;height: 0px;opacity: 0;top: 1px;-moz-opacity: 0;right: 1px;position: absolute;cursor:text;"></button>');
            $hiddenSubjectButton.focusin(function () {
                $subject.focus();
            });
            $subject.before($hiddenSubjectButton);
            $subject.click(function () {
                $hiddenSubjectButton.focus();
            });

            var $searchKeyword = $('#searchKeyword');
            var $hiddenSearchKeywordButton = $('<button style="width: 0px;height: 0px;opacity: 0;top: 1px;-moz-opacity: 0;left: 1px;position: absolute;cursor:text;"></button>');
            $hiddenSearchKeywordButton.focusin(function () {
                $searchKeyword.focus();
            });
            $searchKeyword.before($hiddenSearchKeywordButton);
            $searchKeyword.click(function () {
                $hiddenSearchKeywordButton.focus();
            });
        }
    });
};

//var modelView = new MyMessageModel();
//ko.applyBindings(modelView, $('.main-content')[0]);