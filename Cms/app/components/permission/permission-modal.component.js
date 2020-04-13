System.register(['@angular/core', '@angular/forms', '../../services/group-permission.service'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, forms_1, group_permission_service_1;
    var PermissionModalComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (forms_1_1) {
                forms_1 = forms_1_1;
            },
            function (group_permission_service_1_1) {
                group_permission_service_1 = group_permission_service_1_1;
            }],
        execute: function() {
            PermissionModalComponent = (function () {
                function PermissionModalComponent(formBuilder, permissionService) {
                    this.formBuilder = formBuilder;
                    this.permissionService = permissionService;
                    this.isUpdate = false;
                    this.afterHideModal = new core_1.EventEmitter();
                    this.isAdd = true;
                    this.hasUpdate = false;
                    this.isSubmit = false;
                    this.buildForm();
                }
                PermissionModalComponent.prototype.buildForm = function () {
                    // Init Form Control
                    this.permissionData = {
                        id: new forms_1.FormControl(0, [forms_1.Validators.required]),
                        name: new forms_1.FormControl('', [forms_1.Validators.required, forms_1.Validators.maxLength(300)]),
                        description: new forms_1.FormControl(null, [forms_1.Validators.maxLength(300)])
                    };
                    // Register form validate
                    this.permissionForm = this.formBuilder.group(this.permissionData);
                };
                PermissionModalComponent.prototype.showAddForm = function () {
                    // isAdd
                    this.isAdd = true;
                    this.isSubmit = false;
                    this.hasUpdate = false;
                    this.buildForm();
                    this.show(false);
                };
                PermissionModalComponent.prototype.setUpdate = function (permission) {
                    this.buildForm();
                    this.isAdd = false;
                    this.isSubmit = false;
                    this.hasUpdate = false;
                    // Set value
                    this.permissionData.id.setValue(permission.id);
                    this.permissionData.name.setValue(permission.name);
                    this.permissionData.description.setValue(permission.description);
                    this.show(true);
                };
                PermissionModalComponent.prototype.save = function () {
                    var _this = this;
                    if (!this.permissionForm.valid) {
                        return;
                    }
                    var formData = this.permissionForm.value;
                    this.isSubmit = true;
                    if (this.isAdd) {
                        this.permissionService.add(formData).subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _this.hasUpdate = true;
                                _this.buildForm();
                                // emit event
                                // this.addSuccess.emit(data.status);
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { }, function () { _this.isSubmit = false; });
                    }
                    else {
                        this.permissionService.update(formData)
                            .subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _this.hasUpdate = true;
                                _this.hide();
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { }, function () { _this.isSubmit = false; });
                    }
                };
                PermissionModalComponent.prototype.show = function (isUpdate) {
                    this.isUpdate = isUpdate;
                    $("#permission-modal").modal('show');
                };
                PermissionModalComponent.prototype.hide = function () {
                    $("#permission-modal").modal('hide');
                };
                PermissionModalComponent.prototype.ngAfterContentInit = function () {
                    var _this = this;
                    $('#permission-modal').on("hide", function () {
                        _this.afterHideModal.emit(_this.hasUpdate);
                    });
                };
                __decorate([
                    core_1.Output(), 
                    __metadata('design:type', Object)
                ], PermissionModalComponent.prototype, "afterHideModal", void 0);
                PermissionModalComponent = __decorate([
                    core_1.Component({
                        selector: 'permission-modal',
                        templateUrl: '/app/components/permission/permission-modal.html'
                    }), 
                    __metadata('design:paramtypes', [forms_1.FormBuilder, group_permission_service_1.GroupPermissionService])
                ], PermissionModalComponent);
                return PermissionModalComponent;
            }());
            exports_1("PermissionModalComponent", PermissionModalComponent);
        }
    }
});
//# sourceMappingURL=permission-modal.component.js.map