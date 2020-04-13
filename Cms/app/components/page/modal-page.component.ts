import { Component, Input, Output, EventEmitter } from '@angular/core';
import { JsonPipe } from '@angular/common';
import { FormBuilder, Validators, FormGroup, FormControl, AbstractControl } from '@angular/forms';

import { ValidationService } from '../../services/validation.service';
import { PermissionActionService } from '../../services/permission-action.service';
import { PageService } from '../../services/page.service';

export interface IPageForm {
    id: FormControl,
    name: FormControl,
    icon: FormControl,
    appId: FormControl,
    moduleId: FormControl,
    description: FormControl,
    orderNo: FormControl,
    url: FormControl,
    showInMenu: boolean,
    roleActions: any[]
}

declare var _;
declare var $;
declare var toastr;

@Component({
    selector: 'modal-page',
    templateUrl: '/app/components/page/modal-page.html'
})

export class ModalPageComponent {
    @Input()
    apps = [];
    @Input()
    modules = [];
    @Input()
    roleActions = [];
    @Output() addSuccess = new EventEmitter();
    @Output() afterHideModal = new EventEmitter();

    private modulesbyeAppId = [];

    private isAdd = true;
    private pageForm: FormGroup;
    private pageData: IPageForm;
    private hasUpdate = false;
    isSubmit = false;

    constructor(private formBuilder: FormBuilder, private permissionActionService: PermissionActionService,
        private pageService: PageService) {
        this.buildForm();
    }

    private buildForm() {
        // Init Form Control
        this.pageData = {
            id: new FormControl(0, [Validators.required, Validators.maxLength(300), ValidationService.minValue(1)]),
            name: new FormControl('', [Validators.required, Validators.maxLength(30)]),
            icon: new FormControl('', [Validators.maxLength(300)]),
            appId: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            moduleId: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            description: new FormControl('', [Validators.maxLength(500)]),
            orderNo: new FormControl(0, [Validators.maxLength(500)]),
            url: new FormControl('', [Validators.required, Validators.maxLength(500)]),
            showInMenu: true,
            roleActions: []
        }

        // Register form validate
        this.pageForm = this.formBuilder.group(this.pageData);

        // Reset check all action
        this.roleActions.forEach(action => {
            action.checked = true;
            this.pageData.roleActions.push(action);
        });
    }

    setUpdate(page) {
        this.buildForm();

        // Reset form Value
        this.pageData.showInMenu = page.showInMenu;
        this.pageData.roleActions = [];
        this.pageData.id.setValue(page.id);
        this.pageData.name.setValue(page.name);
        this.pageData.icon.setValue(page.icon);
        this.pageData.appId.setValue(page.appId);
        this.pageData.description.setValue(page.description);
        this.pageData.orderNo.setValue(page.orderNo);
        this.pageData.url.setValue(page.url);

        let actions = [];
        page.actions.forEach(a => {
            actions.push({ id: a.roleActionId, name: a.roleName, checked: a.checked });
        });

        this.pageData.roleActions = actions;

        this.changeApp(page.appId);

        this.pageData.moduleId.setValue(page.moduleId);

        $("#modal-page").modal("show");
        this.isAdd = false;
        this.hasUpdate = false;
    }

    private changeApp(appId: number) {
        // ReSharper disable once CoercedEqualsUsing
        this.modulesbyeAppId = this.modules.filter(x => x.appId == appId);
    }

    showAddForm() {
        // isAdd
        this.isAdd = true;
        this.hasUpdate = false;

        this.buildForm();
        $("#modal-page").modal("show");
    }

    hide() {
        $("#modal-page").modal("hide");
    }

    private save() {
        if (!this.pageForm.valid) {
            return;
        }

        let formData = this.pageForm.value;
        formData.roleActions = this.pageData.roleActions;
        formData.showInMenu = this.pageData.showInMenu;

        this.isSubmit = true;
        if (this.isAdd) {
            this.pageService.add(formData)
                .subscribe(data => {
                        if (data.status > 0) {
                            toastr.success(data.content);
                            this.hasUpdate = true;
                            this.buildForm();
                            // emit event
                            this.addSuccess.emit(data.status);
                            return;
                        }

                        toastr.error(data.content);
                    },
                    () => {},
                    () => { this.isSubmit = false});
        } else {
            this.pageService.update(formData)
                .subscribe(data => {
                    if (data.status > 0) {
                        toastr.success(data.content);
                        this.hasUpdate = true;
                        this.hide();
                        return;
                    }

                    toastr.error(data.content);
                },
                () => {},
                () => { this.isSubmit = false});
        }
    }

    ngAfterContentInit() {
        $('#modal-page').on("hide", () => {
            this.afterHideModal.emit(this.hasUpdate);
        });
    }
}