﻿import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, RequestMethod, BaseRequestOptions, Request, Response  } from '@angular/http';
import 'rxjs/Rx';

@Injectable()
export class RoleActionService {
    private headers: Headers;

    constructor(private http: Http) {
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
    }

    getAll() {
        let url = `/role-action/all`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }
}