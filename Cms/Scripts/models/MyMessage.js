var messageModel = function () { };

amplify.request.define("SearchMessageInbox", "ajax", {
    url: '/Message/SearchInbox',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("SearchMessageSent", "ajax", {
    url: '/Message/SearchSent',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("SearchMessageDraft", "ajax", {
    url: '/Message/SearchDraft',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("SearchMessageTrash", "ajax", {
    url: '/Message/SearchTrash',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("SearchMessageStar", "ajax", {
    url: '/Message/SearchStar',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("GetOfficeAndUser", "ajax", {
    url: '/Message/GetOfficeAndUser',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("GetUserByOfficeId", "ajax", {
    url: '/Message/GetUserByOfficeId',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("DetailMessage", "ajax", {
    url: '/Message/Detail',
    dataType: 'json',
    type: 'GET'
});

amplify.request.define("ComposeMessage", "ajax", {
    url: '/Message/Compose',
    dataType: 'json',
    type: 'POST'
});

amplify.request.define("DiscardMessage", "ajax", {
    url: '/Message/Discard',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

amplify.request.define("SaveDraftMessage", "ajax", {
    url: '/Message/SaveDraft',
    dataType: 'json',
    type: 'POST'
});

amplify.request.define("SendMessage", "ajax", {
    url: '/Message/SendMessage',
    dataType: 'json',
    type: 'POST'
});

amplify.request.define("RemoveAttachment", "ajax", {
    url: '/Message/RemoveAttachment',
    dataType: 'json',
    type: 'POST'
});

amplify.request.define("MoveTrash", "ajax", {
    url: '/Message/MoveTrash',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

amplify.request.define("UndoMoveTrash", "ajax", {
    url: '/Message/UndoMoveTrash',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

amplify.request.define("DeleteMessage", "ajax", {
    url: '/Message/Delete',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

amplify.request.define("MarkReadedMessage", "ajax", {
    url: '/Message/MarkReaded',
    dataType: 'json',
    type: 'POST',
    traditional: true
});

amplify.request.define("SetStarMessage", "ajax", {
    url: '/Message/SetStar',
    dataType: 'json',
    type: 'POST'
});

amplify.request.define("GetDetailByMessageId", "ajax", {
    url: '/Message/GetDetailById',
    dataType: 'json',
    type: 'GET'
});

messageModel.searchInbox = function (keyword, isRead, star, currentPage, recordPerPage, callback) {
    amplify.request("SearchMessageInbox", {
        keyword: keyword,
        isRead: isRead,
        star: star,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.searchSent = function (keyword, isRead, star, currentPage, recordPerPage, callback) {
    amplify.request("SearchMessageSent", {
        keyword: keyword,
        isRead: isRead,
        star: star,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.searchDraft = function (keyword, isRead, star, currentPage, recordPerPage, callback) {
    amplify.request("SearchMessageDraft", {
        keyword: keyword,
        isRead: isRead,
        star: star,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.searchTrash = function (keyword, isRead, star, currentPage, recordPerPage, callback) {
    amplify.request("SearchMessageTrash", {
        keyword: keyword,
        isRead: isRead,
        star: star,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.searchStar = function (keyword, isRead, star, currentPage, recordPerPage, callback) {
    amplify.request("SearchMessageStar", {
        keyword: keyword,
        isRead: isRead,
        currentPage: currentPage,
        recordPerPage: recordPerPage
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.compose = function (messageIdForward, token, callback) {
    amplify.request("ComposeMessage", {
        messageIdForward: messageIdForward,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.discard = function (id, token, callback) {
    amplify.request("DiscardMessage", {
        ids: id,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.saveDraft = function (id, to, cc, bcc, title, content, token, callback) {
    amplify.request("SaveDraftMessage", {
        id: id,
        to: to, 
        cc: cc, 
        bcc: bcc,
        title: title,
        content: content,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.detail = function (id, callback) {
    amplify.request("DetailMessage", {
        id: id,
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.send = function (id, to, cc, bcc, title, content, isSendOut, token, callback) {
    amplify.request("SendMessage", {
        id: id,
        to: to,
        cc: cc,
        bcc: bcc,
        title: title,
        content: content,
        isSendOut: isSendOut,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.removeAttachment = function(attachmentId, messageId, token, callback) {
    amplify.request("RemoveAttachment", {
        id: attachmentId,
        messageId: messageId,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.moveTrash = function(ids, token, callback) {
    amplify.request("MoveTrash", {
        ids: ids,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.undoMoveTrash = function (ids, token, callback) {
    amplify.request("UndoMoveTrash", {
        ids: ids,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.delete = function (ids, token, callback) {
    amplify.request("DeleteMessage", {
        ids: ids,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.markReaded = function (ids, isRead, token, callback) {
    amplify.request("MarkReadedMessage", {
        ids: ids,
        isRead: isRead,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.setStar = function (id, star, token, callback) {
    amplify.request("SetStarMessage", {
        id: id,
        star: star,
        __RequestVerificationToken: token
    }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.getOfficeAndUser = function (callback) {
    amplify.request("GetOfficeAndUser", {}, function (data) {
        if (callback) callback(data);
    });
};

messageModel.getUserByOfficeId = function (id, idPath, callback) {
    amplify.request("GetUserByOfficeId", { id: id, idPath: idPath }, function (data) {
        if (callback) callback(data);
    });
};

messageModel.GetDetailByMessageId = function (id, callback) {
    amplify.request("GetDetailByMessageId", { id: id }, function (data) {
        if (callback) callback(data);
    });
};

