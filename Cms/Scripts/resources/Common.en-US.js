(function (resources) {
    resources.common = {
        dateRangePicker: {
            applyLabel: 'Apply',
            cancelLabel: 'Cancel',
            fromLabel: 'From',
            toLabel: 'To',
            weekLabel: 'W',
            customRangeLabel: 'Custom Range'
        },
        defaultFormat: {
            date: 'L',
            dateTime: 'L HH:mm',
            dateTimeWithSecond: 'L HH:mm:ss',
            dateAndMonth: 'MM/DD',
            time: 'HH:mm',
            number: 'N',
            currency: 'C',
            dateAllFormat: ['DD/MM/YYYY', 'DD/MM/YYYY HH:mm', 'DD/MM/YYYY HH:mm:ss', 'DD/MM/YYYY hh:mm a', 'DD/MM/YYYY hh:mm:ss a', 'D/M/YYYY', 'D/M/YYYY HH:mm', 'D/M/YYYY HH:mm:ss', 'D/M/YYYY hh:mm a', 'D/M/YYYY hh:mm:ss a', 'D/M/YYYY H:m', 'D/M/YYYY H:m:s', 'D/M/YYYY h:m a', 'D/M/YYYY h:m:s a', 'DD/MM/YYYY H:m', 'DD/MM/YYYY H:m:s', 'DD/MM/YYYY h:m a', 'DD/MM/YYYY h:m:s a'],
            dateTimeAllFormat: ['D/M/YYYY h:m', 'DD/MM/YYYY hh:mm', 'D/M/YYYY hh:mm', 'DD/MM/YYYY h:m', 'DD/MM/YYYY H:m']
        },
        mask: {
            numeric: {
                prefix: '$ ',
                suffix: '',
                groupSeparator: ',',
                radixPoint: '.'
            },
            dateTime: {
                date: 'mm/dd/yyyy',
                dateTime: 'datetime'
            }
        },
        pager: {
            noRecord: 'No record of {0}',
            description: 'Display {0} - {1} of {2} {3}'
        },
        message: {
            confirmDelete: 'Are you sure delete this {0}?',
            confirmDelete2: 'Are you sure you want to delete?',
            confirmDeleteSelected: 'Are you sure want to delete selected {0}?',
            notExistsOrDeleted: '{0} not exist or deleted!',
            notExist: '{0} not exist',
            deleteSuccess: 'Delete success!',
            addNewSuccess: 'Add new success!',
            addNewFail: 'Add new fail',
            updateSuccess: 'Update success!',
            approveSuccess: 'Approve success!',
            alreadyExist: '{0} is already exist! <br/> Please check again.',
            toYearGreaterThanFromYear: 'To year must be greater than from year',
            notNull: '{0} can not be null.',
            notHavePermission: "You do not have permission to do this action.",
            max: "{0} must be less than or equal {1}.",
            fileNotAllowUpload: 'This file types are not allowed to upload',
            maxFileLengthUpload: 'File size larger than the size allowed',
            maxFileLengthUploadFormat: 'File size must be less than or equal {0}',
            selectFiles: "Select files",
            selectFile: "Select a file",
            enterContent: "Enter content",
            pleaseEnterCommentContent: "Please enter comment content.",
            accept: "Accept",
            cancel: "Cancel",
            showAll: "Show all",
            showLess: "Show less",
            uploadProgressNotDone: "The upload progress have not finished. Please wait.",
            invalidDateFormat: "{0} invalid!",
            saved: "Đã lưu thông tin thành công",
            locked: "Tài khoản của bạn đã bị khóa hoặc does not exist",
            changePass: "Bạn có chắc chắn muốn đổi Password cho tài khoản {0} không?",
            confirmPass: "Password và xác nhận Password phải giống nhau?",
            lengthPass: "Password phải tối thiểu 6 ký tự",
            pleaseSelect: "Please select {0}",
            notFound: "Resources not found",
            stringLength: "{0} length should not exceed {1} characters",
            lessThan: "{0} must less than {1}",
            invalidFileTypes: "Invalid file type. just file: {0} allowed",
            somethingWentWrong: "Something went wrong. Please contact with administrator.",
            mustBeNumber: "{0} must be number",
            cannotTheSame: "{0} - {1} can not be the same",
            greaterThan: "{0} phải lớn hơn {1}"
        },
        contextMenuChart: {
            contextButton: {
                menuItems: [
                    {
                        text: 'Print',
                        onclick: function() {
                            this.print();
                        }
                    },
                    {
                        separator: true
                    },
                    {
                        text: 'Download PNG image',
                        onclick: function() {
                            this.exportChart();
                        }
                    },
                    {
                        text: 'Download JPEG image',
                        onclick: function() {
                            this.exportChart({
                                type: 'image/jpeg'
                            });
                        }
                    },
                    {
                        text: 'Download PDF document',
                        onclick: function() {
                            this.exportChart({
                                type: 'application/pdf'
                            });
                        }
                    },
                    {
                        text: 'Download SVG vector image',
                        onclick: function() {
                            this.exportChart({
                                type: 'image/svg+xml'
                            });
                        }
                    }
                ]
            }
        },
        allMonth: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        allQuarter: ['Q1', 'Q2', 'Q3', 'Q4'],
        selectpicker: {
            noneSelectedText: 'Nothing selected',
            noneResultsText: 'No results match',
            selectAllText: 'Select All',
            deselectAllText: 'Deselect All'
        },
        button: {
            ok: 'Ok',
            cancel: 'Cancel',
            upload: 'Upload file',
            loadMore: 'Load more'
        },
        title: {
            add: 'Add new {0}',
            edit: 'Update {0}',            
        },
        label: {
            download: 'Download',
            receiveNotification: 'Receive notification',
            stopReceiveNotification: 'Stop notification',
            like: 'Like',
            unLike: 'Unlike',
            comment: 'Comment',
            deletes: 'Delete',
            all: 'All',
            files: ' Files',
            news: " news",
            fromDate: "From date",
            toDate: "To date",
            addCustomerPotential: 'Thêm mới Account tiềm năng',
            customerDetail: 'Detail khách hàng',
        },
        page: {
            show: 'Show <b>',
            to: '</b> to <b>',
            of: '</b> of <b>',
            record: '</b> Records'
        }
    };
})(window.resources = window.resources || {});