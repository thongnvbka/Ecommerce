import { Component } from '@angular/core';
import { Router } from '@angular/router'
import { Title } from '@angular/platform-browser';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';

import { PagingComponent } from '../paging-component';
import { AppService } from '../../services/app.service';
import { ModuleService } from '../../services/module.service';
import { PageService } from '../../services/page.service';
import { PermissionActionService } from '../../services/permission-action.service';
import { RoleActionService } from '../../services/role-action.service';
import { ModalPageComponent } from './modal-page.component'

declare var $;
declare var _;
declare var toastr;
declare var swal;

@Component({
    selector: 'page',
    templateUrl: '/app/components/page/page.html',
    providers: [AppService, ModuleService, RoleActionService, PageService, PermissionActionService]
})

export class PageComponent {
    recordPerPage = 20;
    currentPage = 1;
    pages = [];
    totalRecord = 0;
    isSearch = false;
    isError = false;

    modules = [];
    apps = [];
    roleActions = [];

    pageFilter = {
        keyword: '',
        appId: null,
        moduleId: null,
        modulesByApId: []
    }

    constructor(private appService: AppService,
        private moduleService: ModuleService, private pageService: PageService, private roleActionService: RoleActionService) {

        // Lấy ra hết các App
        this.appService.getAll()
            .subscribe(data => {
                this.apps = data;
            });

        // Lấy ra hết các module
        this.moduleService.getAll()
            .subscribe(data => {
                this.modules = data;
            });

        // Lấy ra hết roleAction
        this.roleActionService.getAll()
            .subscribe(data => {
                this.roleActions = data;
            });

        this.search(1);
    }

    changeApp(appId: number) {
        // ReSharper disable once CoercedEqualsUsing
        this.pageFilter.modulesByApId = this.modules.filter(x => x.appId == appId);
        this.search(1);
    }

    changeModule(moduleId: number) {
        this.search(1);
    }

    search(currentPage) {
        this.currentPage = currentPage;
        this.isSearch = true;

        this.pageService.search(this.pageFilter.keyword,
            this.pageFilter.appId,
            this.pageFilter.moduleId,
            this.recordPerPage,
            this.currentPage)
            .subscribe((data) => {
                let groupActions = _.groupBy(data.permissionActions, "pageId");

                data.items.forEach(i => {
                    i.actions = groupActions[i.id + ""];
                    i.actions.forEach(ac => {
                        i["action_" + ac.roleActionId] = ac.checked;
                    });
                });

                this.pages = data.items;
                this.totalRecord = data.totalRecord;
            },
            ()=>{},
            () => this.isSearch = false);
    }

    afterHideModal(refresh) {
        if (refresh)
            this.search(1);
    }

    delete(page) {
        swal({
                title: "Bạn có chắc chắn muốn xóa page này?",
                showCancelButton: true,
                confirmButtonText: 'Delete',
                cancelButtonText: 'Không xóa'
            })
            .then(() => {
                this.pageService.delete(page.id)
                    .subscribe(data => {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _.remove(this.pages, item => item.id === page.id);
                                return;
                            }

                            toastr.error(data.content);
                        },
                        () => {});
            });
    }
}
