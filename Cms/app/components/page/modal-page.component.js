System.register(['@angular/core', '@angular/forms', '../../services/validation.service', '../../services/permission-action.service', '../../services/page.service'], function(exports_1, context_1) {
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
    var core_1, forms_1, validation_service_1, permission_action_service_1, page_service_1;
    var ModalPageComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (forms_1_1) {
                forms_1 = forms_1_1;
            },
            function (validation_service_1_1) {
                validation_service_1 = validation_service_1_1;
            },
            function (permission_action_service_1_1) {
                permission_action_service_1 = permission_action_service_1_1;
            },
            function (page_service_1_1) {
                page_service_1 = page_service_1_1;
            }],
        execute: function() {
            ModalPageComponent = (function () {
                function ModalPageComponent(formBuilder, permissionActionService, pageService) {
                    this.formBuilder = formBuilder;
                    this.permissionActionService = permissionActionService;
                    this.pageService = pageService;
                    this.apps = [];
                    this.modules = [];
                    this.roleActions = [];
                    this.addSuccess = new core_1.EventEmitter();
                    this.afterHideModal = new core_1.EventEmitter();
                    this.modulesbyeAppId = [];
                    this.isAdd = true;
                    this.hasUpdate = false;
                    this.isSubmit = false;
                    this.buildForm();
                }
                ModalPageComponent.prototype.buildForm = function () {
                    var _this = this;
                    // Init Form Control
                    this.pageData = {
                        id: new forms_1.FormControl(0, [forms_1.Validators.required, forms_1.Validators.maxLength(300), validation_service_1.ValidationService.minValue(1)]),
                        name: new forms_1.FormControl('', [forms_1.Validators.required, forms_1.Validators.maxLength(30)]),
                        icon: new forms_1.FormControl('', [forms_1.Validators.maxLength(300)]),
                        appId: new forms_1.FormControl(null, [forms_1.Validators.required, forms_1.Validators.maxLength(50)]),
                        moduleId: new forms_1.FormControl(null, [forms_1.Validators.required, forms_1.Validators.maxLength(500)]),
                        description: new forms_1.FormControl('', [forms_1.Validators.maxLength(500)]),
                        orderNo: new forms_1.FormControl(0, [forms_1.Validators.maxLength(500)]),
                        url: new forms_1.FormControl('', [forms_1.Validators.required, forms_1.Validators.maxLength(500)]),
                        showInMenu: true,
                        roleActions: []
                    };
                    // Register form validate
                    this.pageForm = this.formBuilder.group(this.pageData);
                    // Reset check all action
                    this.roleActions.forEach(function (action) {
                        action.checked = true;
                        _this.pageData.roleActions.push(action);
                    });
                };
                ModalPageComponent.prototype.setUpdate = function (page) {
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
                    var actions = [];
                    page.actions.forEach(function (a) {
                        actions.push({ id: a.roleActionId, name: a.roleName, checked: a.checked });
                    });
                    this.pageData.roleActions = actions;
                    this.changeApp(page.appId);
                    this.pageData.moduleId.setValue(page.moduleId);
                    $("#modal-page").modal("show");
                    this.isAdd = false;
                    this.hasUpdate = false;
                };
                ModalPageComponent.prototype.changeApp = function (appId) {
                    // ReSharper disable once CoercedEqualsUsing
                    this.modulesbyeAppId = this.modules.filter(function (x) { return x.appId == appId; });
                };
                ModalPageComponent.prototype.showAddForm = function () {
                    // isAdd
                    this.isAdd = true;
                    this.hasUpdate = false;
                    this.buildForm();
                    $("#modal-page").modal("show");
                };
                ModalPageComponent.prototype.hide = function () {
                    $("#modal-page").modal("hide");
                };
                ModalPageComponent.prototype.save = function () {
                    var _this = this;
                    if (!this.pageForm.valid) {
                        return;
                    }
                    var formData = this.pageForm.value;
                    formData.roleActions = this.pageData.roleActions;
                    formData.showInMenu = this.pageData.showInMenu;
                    this.isSubmit = true;
                    if (this.isAdd) {
                        this.pageService.add(formData)
                            .subscribe(function (data) {
                            if (data.status > 0) {
                                toastr.success(data.content);
                                _this.hasUpdate = true;
                                _this.buildForm();
                                // emit event
                                _this.addSuccess.emit(data.status);
                                return;
                            }
                            toastr.error(data.content);
                        }, function () { }, function () { _this.isSubmit = false; });
                    }
                    else {
                        this.pageService.update(formData)
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
                ModalPageComponent.prototype.ngAfterContentInit = function () {
                    var _this = this;
                    $('#modal-page').on("hide", function () {
                        _this.afterHideModal.emit(_this.hasUpdate);
                    });
                };
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Object)
                ], ModalPageComponent.prototype, "apps", void 0);
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Object)
                ], ModalPageComponent.prototype, "modules", void 0);
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Object)
                ], ModalPageComponent.prototype, "roleActions", void 0);
                __decorate([
                    core_1.Output(), 
                    __metadata('design:type', Object)
                ], ModalPageComponent.prototype, "addSuccess", void 0);
                __decorate([
                    core_1.Output(), 
                    __metadata('design:type', Object)
                ], ModalPageComponent.prototype, "afterHideModal", void 0);
                ModalPageComponent = __decorate([
                    core_1.Component({
                        selector: 'modal-page',
                        templateUrl: '/app/components/page/modal-page.html'
                    }), 
                    __metadata('design:paramtypes', [forms_1.FormBuilder, permission_action_service_1.PermissionActionService, page_service_1.PageService])
                ], ModalPageComponent);
                return ModalPageComponent;
            }());
            exports_1("ModalPageComponent", ModalPageComponent);
        }
    }
});
//# sourceMappingURL=modal-page.component.js.map