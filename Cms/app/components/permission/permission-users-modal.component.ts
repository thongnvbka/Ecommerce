import { Component, AfterContentInit } from '@angular/core';

declare var $;
declare var swal;

@Component({
    selector: 'permission-users-modal',
    templateUrl: '/app/components/permission/permission-users-modal.html'
})
export class PermissionUsersModalComponent implements AfterContentInit {
    users = [];
    recordPerPage = 20;
    currentPage = 1;
    totalRecord = 6;

    constructor() {
        //this.users = users;
    }

    show() {
        console.log("show");
        $("#permission-users-modal").modal('show');
    }

    hide() {
        $("#permission-users-modal").modal('hide');
    }

    delete() {
        swal({
                title: "Bạn có chắc chắn muốn xóa quyền của nhân viên này?",
                showCancelButton: true,
                confirmButtonText: 'Delete',
                cancelButtonText: 'Không'
            })
            .then(()=> {});
    }

    ngAfterContentInit() {
        $(() => {
        });
    }
}
