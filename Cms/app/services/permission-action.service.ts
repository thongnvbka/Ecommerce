import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, RequestMethod, BaseRequestOptions, Request, Response, URLSearchParams } from '@angular/http';
import 'rxjs/Rx';
//import { erpApiDomain } from '../../shared/constants/app.constant';

@Injectable()
export class PermissionActionService {
    private apiUrl: string;
    private headers: Headers;

    constructor(private http: Http) {
        this.apiUrl = `/permission-action`;
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
    }

    getByPageId(pageId) {
        let url = `${this.apiUrl}/by-page-${pageId}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    getByPermissionId(permissionId: number, keyword: string = '') {
        let params = new URLSearchParams();

        params.set('keyword', keyword);
        params.set('permissionId', permissionId.toString());

        let url = `${this.apiUrl}/get-by-permission`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '', search: params });

        return this.http.get(url, options).map(x => x.json());
    }
}