/// <reference path="pipes/permission-detail.pipe.ts" />
import { NgModule }      from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule }        from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';

import { PagingComponent }   from './components/paging-component';

import { PermissionComponent } from './components/permission/permission.component';
import { PermissionUsersModalComponent } from './components/permission/permission-users-modal.component';
import { PermissionPageModalComponent } from './components/permission/permission-page-modal.component';
import { PermissionModalComponent } from './components/permission/permission-modal.component';
import { PermissionDetailModalComponent } from './components/permission/permission-detail-modal.component';
import { ControlMessages } from './components/control-messages.component';
import { PermissionDetailPipe } from './pipes/permission-detail.pipe';

@NgModule({
    imports: [BrowserModule, CommonModule, FormsModule, ReactiveFormsModule, HttpModule],
    //declarations: [PermissionComponent, PagingComponent, PermissionUsersModalComponent, PermissionPageModalComponent,
    //    PermissionModalComponent, PermissionDetailModalComponent, , ControlMessages],
    declarations: [PermissionComponent, PagingComponent, PermissionModalComponent,
        PermissionUsersModalComponent, PermissionDetailModalComponent, ControlMessages,
        PermissionDetailPipe, PermissionPageModalComponent],
    bootstrap: [PermissionComponent]
})
export class PermissionModule { }
