import { Component, AfterContentInit } from '@angular/core';
import { PermissionModalComponent } from './permission-modal.component';
import { PermissionUsersModalComponent } from './permission-users-modal.component';
import { PermissionDetailModalComponent } from './permission-detail-modal.component';

import { GroupPermissionService } from '../../services/group-permission.service';

declare var _;
declare var $;
declare var toastr;
declare var swal;

@Component({
    selector: 'permissions',
    templateUrl: '/app/components/permission/permission.html',
    providers: [GroupPermissionService]
})
export class PermissionComponent implements AfterContentInit {
    permissions = [];
    recordPerPage = 20;
    currentPage = 1;
    totalRecord = 0;
    isSearch = false;
    keyword = "";

    constructor(private permissionService: GroupPermissionService) {
        this.search(1);
    }

    search(currentPage) {
        this.currentPage = currentPage;
        this.isSearch = true;

        this.permissionService.search(this.keyword, this.recordPerPage, this.currentPage)
            .subscribe((data) => {
                    this.permissions = data.items;
                    this.totalRecord = data.totalRecord;
                },
                ()=>{},
                () => this.isSearch = false);
    }

    afterHideModal(refresh) {
        if (refresh)
            this.search(this.currentPage);
    }

    delete(permission) {
        swal({
                title: "Bạn có chắc chắn muốn xóa nhóm quyền này?",
                showCancelButton: true,
                confirmButtonText: 'Delete',
                cancelButtonText: 'Không'
            })
            .then(() => {
                this.permissionService.delete(permission.id)
                    .subscribe(data => {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(this.permissions, item => item.id === permission.id);
                                return;
                            }

                            toastr.error(data.content);
                        },
                        () => {});
            });
    }

    ngAfterContentInit() {
        //jQuery(document).ready(() => {
        //    App.init(); // init metronic core componets
        //    Layout.init();
        //});
    }
}
