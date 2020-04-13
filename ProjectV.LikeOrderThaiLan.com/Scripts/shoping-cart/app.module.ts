/// <reference path="numberic.pipe.ts" />
import { NgModule }      from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule }        from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';
import { NumbericPipe } from './numberic.pipe'

@NgModule({
    imports: [BrowserModule, CommonModule, FormsModule, HttpModule],
    declarations: [AppComponent, NumbericPipe],
    bootstrap: [AppComponent]
})
export class AppModule { }
