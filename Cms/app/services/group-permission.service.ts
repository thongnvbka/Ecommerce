import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, RequestMethod, BaseRequestOptions, Request, Response, URLSearchParams } from '@angular/http';
import 'rxjs/Rx';

//import { erpApiDomain } from '../../shared/constants/app.constant';

@Injectable()
export class GroupPermissionService {
    private apiUrl: string;
    private headers: Headers;

    constructor(private http: Http) {
        this.apiUrl = `/group-permission`;;
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
    }
    
    search(keyword: string = '', recordPerPage?: number, currentPage?: number) {
        let params = new URLSearchParams();

        params.set('keyword', keyword);

        if (recordPerPage)
            params.set('recordPerPage', recordPerPage.toString());

        if (currentPage)
            params.set('currentPage', currentPage.toString());

        let url = `${this.apiUrl}/search`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '', search: params });

        return this.http.get(url, options).map(x => x.json());
    }

    add(groupPermission) {
        let url = `${this.apiUrl}/add`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify(groupPermission), options).map(res => res.json());
    }

    addPermission(page) {
        let url = `${this.apiUrl}/add-permission`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify(page), options).map(res => res.json());
    }

    deletePermission(permissionId,  pageId) {
        let url = `${this.apiUrl}/delete-permission?permissionId=${permissionId}&pageId=${pageId}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.post(url, options).map(res => res.json());
    }


    updatePermission(permissionId, pageId, actionId, checked) {
        let url = `${this.apiUrl}/update-permission`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify({ permissionId, pageId, actionId, checked }), options)
            .map(res => res.json());
    }

    update(groupPermission) {
        let url = `${this.apiUrl}/update`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify(groupPermission), options).map(res => res.json());
    }

    delete(id: number) {
        let url = `${this.apiUrl}/delete/?id=${id}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.post(url, options).map(res => res.json());
    }
}