(function (resources) {
    resources.common = {
        dateRangePicker: {
            applyLabel: 'Áp dụng',
            cancelLabel: 'Hủy',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            weekLabel: 'T',
            customRangeLabel: 'Tùy chọn'
        },
        defaultFormat: {
            date: 'L',
            dateTime: 'L HH:mm',
            dateTimeWithSecond: 'L HH:mm:ss',
            dateAndMonth: 'DD/MM',
            time: 'HH:mm',
            number: 'N',
            currency: 'C',
            dateAllFormat: ['DD/MM/YYYY', 'DD/MM/YYYY HH:mm', 'DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY hh:mm a', 'DD/MM/YYYY hh:mm:ss a', 'D/M/YYYY', 'D/M/YYYY HH:mm', 'D/M/YYYY HH:mm:ss', 'D/M/YYYY hh:mm a', 'D/M/YYYY hh:mm:ss a', 'D/M/YYYY H:m', 'D/M/YYYY H:m:s', 'D/M/YYYY h:m a', 'D/M/YYYY h:m:s a', 'DD/MM/YYYY H:m', 'DD/MM/YYYY H:m:s', 'DD/MM/YYYY h:m a', 'DD/MM/YYYY h:m:s a'],
            dateTimeAllFormat: ['D/M/YYYY h:m', 'DD/MM/YYYY hh:mm', 'D/M/YYYY hh:mm', 'DD/MM/YYYY h:m', 'DD/MM/YYYY H:m', 'D/M/YYYY H:m']
        },
        mask: {
            numeric: {
                prefix: '',
                suffix: ' đ',
                groupSeparator: ',',
                radixPoint: '.'
            },
            dateTime: {
                date: 'dd/mm/yyyy',
                dateTime: 'datetimevi'
            }
        },
        pager: {
            noRecord: 'Chưa có {0}',
            description: 'Hiển thị từ {0} đến {1} của {2} {3}'
        },
        message: {
            confirmDelete: 'Bạn có chắc muốn xóa {0} này không?',
            confirmDelete2: 'Bạn có chắc muốn xóa?',
            confirmDeleteSelected: 'Bạn có chắc chắn muốn xóa {0} đã chọn?',
            notExistsOrDeleted: '{0} Does not exist or has been deleted!',
            notExist: '{0} Does not exist or has been deleted',
            deleteSuccess: 'Đã xóa thành công!',
            addNewSuccess: 'Add new success!',
            addNewFail: 'Thêm mới không thành công!',
            updateSuccess: 'Update successful!',
            approveSuccess: 'Duyệt thành công!',
            alreadyExist: '{0} đã tồn tại! <br/> Xin vui lòng kiểm tra lại.',
            toYearGreaterThanFromYear: 'Đến năm phải lớn hơn từ năm',
            notNull: '{0} không được để trống.',
            notHavePermission: "Bạn không có quyền để thực hiện chức năng này.",
            max: "{0} phải nhỏ hơn hoặc bằng {1}.",
            fileNotAllowUpload: 'Type yuanp này không cho phép tải lên',
            maxFileLengthUpload: 'Kích thước yuanp lớn hơn kích thước cho phép',
            maxFileLengthUploadFormat: 'Kích thước yuanp phải nhỏ hơn hoặc bằng {0}',
            selectFiles: "Chọn yuanp tin tải lên",
            selectFile: "Chọn một yuanp tin",
            enterContent: "Nhập nội dung",
            pleaseEnterCommentContent: "Vui lòng nhập nội dung bình luận",
            accept: "Đồng ý",
            cancel: "Hủy bỏ",
            showAll: "Tất cả",
            showLess: "Rút gọn",
            uploadProgressNotDone: "Tiến trình tải chưa hoàn thành. Vui lòng đợi.",
            invalidDateFormat: "{0} không đúng định dạng!",
            saved: "Đã lưu thông tin thành công",
            locked: "Tài khoản của bạn đã bị khóa hoặc does not exist",
            changePass: "Bạn có chắc chắn muốn đổi Password cho tài khoản {0} không?",
            confirmPass: "Password và xác nhận Password phải giống nhau?",
            lengthPass: "Password phải tối thiểu 6 ký tự",
            pleaseSelect: "Vui lòng chọn {0}",
            notFound: "Không tìm thấy dữ liệu",
            stringLength: "{0} không được vượt quá {1} kí tự",
            lessThan: "{0} phải nhỏ hơn {1}",
            invalidFileTypes: "yuanp tin không đúng định dạng. Chỉ những file: {0} được phép tải lên",
            somethingWentWrong: "Có gì đó hoạt động chưa đúng. Vui lòng liên hệ với quản trị viên",
            mustBeNumber: "{0} phải là số",
            cannotTheSame: "{0} - {1} không được phép trùng nhau",
            greaterThan: "{0} phải lớn hơn {1}"
        },
        contextMenuChart: {
            contextButton: {
                menuItems: [
                    {
                        text: 'In',
                        onclick: function () {
                            this.print();
                        }
                    },
                    {
                        separator: true
                    },
                    {
                        text: 'Tải ảnh PNG',
                        onclick: function () {
                            this.exportChart();
                        }
                    },
                    {
                        text: 'Tải ảnh JPEG',
                        onclick: function () {
                            this.exportChart({
                                type: 'image/jpeg'
                            });
                        }
                    },
                    {
                        text: 'Tải yuanp PDF',
                        onclick: function () {
                            this.exportChart({
                                type: 'application/pdf'
                            });
                        }
                    },
                    {
                        text: 'Tải ảnh SVG vector',
                        onclick: function () {
                            this.exportChart({
                                type: 'image/svg+xml'
                            });
                        }
                    }
                ]
            }
        },
        allMonth: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
        allQuarter: ['Quý 1', 'Quý 2', 'Quý 3', 'Quý 4'],
        selectpicker: {
            noneSelectedText: 'Không có dữ liệu',
            noneResultsText: 'Không tìm thấy',
            selectAllText: 'Chọn tất cả',
            deselectAllText: 'Bỏ chọn tất cả'
        },
        button: {
            ok: 'Đồng ý',
            cancel: 'Hủy bỏ',
            upload: 'Tải yuanp lên',
            loadMore: 'Tải thêm'
        },
        title: {
            add: 'Thêm mới {0}',
            edit: 'Cập nhật {0}',            
        },
        label: {
            download: 'Tải về',
            receiveNotification: 'Nhận thông báo',
            stopReceiveNotification: 'Ngừng nhận thông báo',
            like: 'Thích',
            unLike: 'Bỏ thích',
            comment: 'Trả lời',
            deletes: 'Xóa',
            all: 'Tất cả',
            files: ' yuanp tin',
            news: "tin tức",
            fromDate: "Từ ngày",
            toDate: "Đến ngày",
            addCustomerPotential: 'Thêm mới Account tiềm năng',
            customerDetail: 'Detail khách hàng',
        },
        page: {
            show: 'Hiển thị <b>',
            to: '</b> đến <b>',
            of: '</b> của <b>',
            record: '</b> records'
        }
    };
})(window.resources = window.resources || {});