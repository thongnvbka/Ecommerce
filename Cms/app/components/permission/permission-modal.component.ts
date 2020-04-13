import { Component, AfterContentInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormGroup, FormControl, AbstractControl } from '@angular/forms';
import { ValidationService } from '../../services/validation.service';
import { GroupPermissionService } from '../../services/group-permission.service';

declare var _;
declare var $;
declare var toastr;

export interface IPermissionForm {
    id: FormControl,
    name: FormControl,
    description: FormControl
}

@Component({
    selector: 'permission-modal',
    templateUrl: '/app/components/permission/permission-modal.html'
})
export class PermissionModalComponent implements AfterContentInit {
    isUpdate = false;
    permissionData: IPermissionForm;
    permissionForm: FormGroup;

    @Output() afterHideModal = new EventEmitter();

    private isAdd = true;
    private hasUpdate = false;
    isSubmit = false;

    constructor(private formBuilder: FormBuilder, private permissionService: GroupPermissionService) {
        this.buildForm();
    }

    buildForm() {
        // Init Form Control
        this.permissionData = {
            id: new FormControl(0, [Validators.required]),
            name: new FormControl('', [Validators.required, Validators.maxLength(300)]),
            description: new FormControl(null, [Validators.maxLength(300)])
        };

        // Register form validate
        this.permissionForm = this.formBuilder.group(this.permissionData);
    }

    showAddForm() {
        // isAdd
        this.isAdd = true;
        this.isSubmit = false;
        this.hasUpdate = false;
        this.buildForm();
        this.show(false);
    }

    setUpdate(permission) {
        this.buildForm();
        this.isAdd = false;
        this.isSubmit = false;
        this.hasUpdate = false;

        // Set value
        this.permissionData.id.setValue(permission.id);
        this.permissionData.name.setValue(permission.name);
        this.permissionData.description.setValue(permission.description);

        this.show(true);
    }

    save() {
        if (!this.permissionForm.valid) {
            return;
        }

        let formData = this.permissionForm.value;

        this.isSubmit = true;
        if (this.isAdd) {
            this.permissionService.add(formData).subscribe(data => {
                if (data.status > 0) {
                    toastr.success(data.content);
                    this.hasUpdate = true;
                    this.buildForm();
                    // emit event
                    // this.addSuccess.emit(data.status);
                    return;
                }
                toastr.error(data.content);
            }, () => {}, () => { this.isSubmit = false; });
        } else {
            this.permissionService.update(formData)
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
                    () => { this.isSubmit = false });
        }
    }

    show(isUpdate) {
        this.isUpdate = isUpdate;

        $("#permission-modal").modal('show');
    }

    hide() {
        $("#permission-modal").modal('hide');
    }

    ngAfterContentInit() {
        $('#permission-modal').on("hide", () => {
            this.afterHideModal.emit(this.hasUpdate);
        });
    }
}
