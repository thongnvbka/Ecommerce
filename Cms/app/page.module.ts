/// <reference path="components/control-messages.component.ts" />
import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { FormsModule }        from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { CommonModule } from '@angular/common';

import { PagingComponent }   from './components/paging-component';
import { PageComponent }   from './components/page/page.component';
import { ModalPageComponent } from './components/page/modal-page.component';
import { ControlMessages } from './components/control-messages.component';

@NgModule({
    imports: [BrowserModule, CommonModule, FormsModule, ReactiveFormsModule, HttpModule],
    declarations: [PageComponent, PagingComponent, ModalPageComponent, ControlMessages],
    bootstrap: [PageComponent]
})
export class PageModule { }