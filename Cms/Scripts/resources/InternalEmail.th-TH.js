(function (resources) {
    resources.internalEmail = {
        title: {
            mail: 'Thư',
            detail: 'Detail'
        },
        menu: {
            drafts: 'Thư nháp',
            sentMail: 'Thư đã gửi',
            inbox: 'Hộp thư đến',
            trash: 'Thùng rác',
            starred: 'Thư gắn sao',
            compose: 'Soạn thư'
        },
        descriptionSubject: {
            draft: 'Thư nháp',
            sentMail: 'Thư đã gửi',
            inbox: 'Hộp thư đến',
            noSubject: 'Không Title'
        },
        placeHolder: {
            to: 'Người nhận',
            cc: 'Người nhận Cc',
            bcc: 'Người nhận Bcc'
        },
        pager: {
            description: '{0}-{1} của {2}'
        },
        button: {
            expandToUser: 'Detail',
            collapseToUser: 'collapse'
        },
        message: {
            confirmLeavePage: 'Bạn muốn rời khỏi trang và lưu thư nháp chứ?',
            confirmDelete: 'Bạn có chắc muốn xóa thư này không?',
            confirmDeleteMultiple: 'Bạn có chắc muốn xóa {0} thư không?',
            confirmDeleteForeverMultiple: 'Bạn có chắc muốn xóa vĩnh viến {0} thư không?',
            confirmDiscard: 'Bạn có chắc muốn hủy thư này không?',
            confirmDiscardMultiple: 'Bạn có chắc muôn hủy {0} thư không?',
            confirmRemoveAttachment: 'Bạn có chắc muốn xóa yuanp đính kèm này không?',
            confirmRestoreMultiple: 'Bạn có chắc muốn khôi phục {0} thư không',
            noAttachmentUpload: 'Không có yuanp tải lên',
            notHavePermissionAttachFile: 'Bạn không có quyền đính kèm yuanp',
            notHavePermissionViewMail: 'Bạn không có quyền xem thư',
            notHavePermissionSendMail: 'Bạn không có quyền gửi thư',
            notHavePermissionRemoveAttachment: 'Bạn không có quyền xóa yuanp đính kèm này',
            error: 'Có gì đó hoạt động không đúng khi xử lý yêu cầu của bạn',
            saveDraffSuccess: 'Lưu nháp thành công',
            toRequired: 'Bạn phải nhập tài khoản người nhận',
            regexListUserCc: 'Danh sách người nhận cc phải phân cách nhau bằng dấu chấm phẩy hoặc dấu phẩy',
            regexListUserBcc: 'Danh sách người nhận bcc phải phân cách nhau bằng dấu chấm phẩy hoặc dấu phẩy',
            regexListUser: 'Danh sách người nhận phải phân cách nhau bằng dấu chấm phẩy hoặc dấu phẩy',
            contentRequired: 'Bạn phải nhập nội dung',
            subjectRequired: 'Bạn phải nhập Title',
            recipientNotExist: 'Người nhận does not exist',
            sendMailSuccess: 'Gửi thư thành công',
            fileNotAllowUpload: 'Type yuanp này không cho phép tải lên',
            maxFileLengthUpload: 'Kích thước yuanp lớn hơn kích thước cho phép'
        },
        filter: {
            title: 'Xem: {0}',
            read: 'Đã đọc',
            unread: 'Chưa đọc',
            starred: 'Thư gắn sao',
            all: 'Tất cả'
        },
        detailUserSend: {
            me: 'Tôi',
            to: 'tới',
            on: 'lúc'
        },
        prefixReply: {
            on: 'Vào lúc',
            wrote: 'đã viết' 
        }
    };
})(window.resources = window.resources || {});