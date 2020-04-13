import { Component, AfterContentInit } from '@angular/core';
import { PermissionActionService } from '../../services/permission-action.service';
import { GroupPermissionService } from '../../services/group-permission.service';

import { PermissionDetailPipe } from '../../pipes/permission-detail.pipe';

declare var _;
declare var $;
declare var toastr;
declare var swal;

@Component({
    selector: 'permission-detail-modal',
    templateUrl: '/app/components/permission/permission-detail-modal.html',
    providers: [PermissionActionService]
})
export class PermissionDetailModalComponent implements AfterContentInit {
    recordPerPage = 20;
    currentPage = 1;
    totalRecord = 0;

    permission;
    pageIds = [];

    isSearch = false;
    pages = [];
    actions = [];
    keyword = "";

    constructor(private permissionActionService: PermissionActionService, private groupPermissionService: GroupPermissionService) {
    }

    moduleIdFirst;
    checkFirst(page, idx) {
        console.log(page, idx);
        if (idx === 0)
            this.moduleIdFirst = null;

        let check = page.moduleId === this.moduleIdFirst;

        this.moduleIdFirst = page.moduleId;

        return check;
    }

    search() {
        this.permissionActionService.getByPermissionId(this.permission.id, this.keyword)
            .subscribe((data) => {
                    let pagesCache = [];
                    let groupPages = _.groupBy(data.items, "pageId");
                    this.pageIds = _.keys(groupPages);

                    _.forEach(this.pageIds, (pageId) => {
                        let actions = groupPages[pageId + ""];

                        let p = <any>actions[0];
                        let page = <any>{
                            appId: p.appId,
                            appName: p.appName,
                            moduleId: p.moduleId,
                            moduleName: p.moduleName,
                            pageId: p.pageId,
                            pageName: p.pageName,
                            actions: actions
                        };

                        _.forEach(actions, (action) => {
                            let a = <any>action;
                            page["action_" + a.roleActionId] = a.checked;
                        });

                        pagesCache.push(page);
                    });

                    pagesCache = _.orderBy(pagesCache, ['moduleId', 'pageId'], ['asc', 'asc']);

                    let moduleIdFist;
                    _.forEach(pagesCache, (item) => {
                        item.firstRecord = moduleIdFist !== item.moduleId;
                        item.pathName = `${item.appName} / ${item.moduleName}`;
                        moduleIdFist = item.moduleId;
                    });

                    this.pages = pagesCache;
                    this.actions = data.actions;
                },
            () => { },
                () => this.isSearch = false);
    }

    show(permission) {
        this.permission = permission;
        this.search();
        $("#permission-detail-modal").modal('show');
    }

    hide() {
        $("#permission-detail-modal").modal('hide');
    }

    update(page, actionId) {
        this.groupPermissionService.updatePermission(this.permission.id, page.pageId, actionId, !page['action_' + actionId]).subscribe(data => {
            if (data.status > 0) {
                page['action_' + actionId] = !page['action_' + actionId];
                return;
            }

        }, () => { });
    }

    delete(page) {
        swal({
                title: "Bạn có chắc chắn muốn Trang này này?",
                showCancelButton: true,
                confirmButtonText: 'Delete',
                cancelButtonText: 'Không'
            })
            .then(() => {
                this.groupPermissionService.deletePermission(this.permission.id, page.pageId)
                    .subscribe(data => {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(this.pages, item => item.pageId === page.pageId);
                                // ReSharper disable once CoercedEqualsUsing
                                _.remove(this.pageIds, item => item == page.pageId);
                                return;
                            }

                            toastr.error(data.content);
                        },
                        () => {});
            });

    }

    ngAfterContentInit() {
        $(() => {
        });
    }
}
