(function (resources) {
    resources.internalEmail = {
        title: {
            mail: 'mail',
            detail: 'detail'
        },
        menu: {
            drafts: 'drafts',
            sentMail: 'sent mail',
            inbox: 'inbox',
            trash: 'trash',
            starred: 'starred',
            compose: 'compose'
        },
        descriptionSubject: {
            draft: 'draft',
            sentMail: 'sent mail',
            inbox: 'inbox',
            noSubject: 'no subject'
        },
        placeHolder: {
            to: 'recipient ',
            cc: 'recipient Cc',
            bcc: 'recipient Bcc'
        },
        pager: {
            description: '{0}-{1} of {2}'
        },
        button: {
            expandToUser: 'Details',
            collapseToUser: 'Collapse'
        },
        message: {
            confirmLeavePage: 'Do you want to leave the page and save the draft?',
            confirmDelete: 'Are you sure you want to delete this message?',
            confirmDeleteMultiple: 'Are you sure to delete {0} mail?',
            confirmDeleteForeverMultiple: 'Are you sure to permanently delete the {0} mail??',
            confirmDiscard: 'Are you sure  to discard this message?',
            confirmDiscardMultiple: 'Are you sure  to discard message {0}?',
            confirmRemoveAttachment: 'Are you sure you want to delete this attachment?',
            confirmRestoreMultiple: 'Are you sure to restore {0} mail',
            noAttachmentUpload: 'No uploaded file',
            notHavePermissionAttachFile: 'You do not have permission to attach files',
            notHavePermissionViewMail: 'You do not have permission to view messages',
            notHavePermissionSendMail: 'You do not have permission to send a message',
            notHavePermissionRemoveAttachment: 'You do not have permission to delete this attachment',
            error: 'Something is not working properly when processing your request',
            saveDraffSuccess: 'Draft saved successfully',
            toRequired: 'You must enter the recipient account',
            regexListUserCc: 'The cc recipients list must be separated by a semicolon or comma',
            regexListUserBcc: 'The bcc recipient list must be separated by a semicolon or comma',
            regexListUser: 'The recipients list must be separated by a semicolon or comma',
            contentRequired: 'You must enter the content',
            subjectRequired: 'You must enter a title',
            recipientNotExist: 'The recipient does not exist',
            sendMailSuccess: 'Mail sent successfully',
            fileNotAllowUpload: 'This file type is not allowed to be uploaded',
            maxFileLengthUpload: 'File size is larger than allowed'
        },
        filter: {
            title: 'See: {0}',
            read: 'read',
            unread: 'unread',
            starred: 'starred',
            all: 'all'
        },
        detailUserSend: {
            me: 'me',
            to: 'to',
            on: 'on'
        },
        prefixReply: {
            on: 'on',
            wrote: 'wrote at'
        }
    };
})(window.resources = window.resources || {});