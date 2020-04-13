import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, RequestMethod, BaseRequestOptions, Request, Response, URLSearchParams } from '@angular/http';
import 'rxjs/Rx';

@Injectable()
export class PageService {
    private apiUrl: string;
    private headers: Headers;

    constructor(private http: Http) {
        this.apiUrl = `/page`;;
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
    }

    getPageAllTree() {
        let url = `${this.apiUrl}/page-all-tree`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    getAll() {
        let url = `${this.apiUrl}/all`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    getByAppId(appId: number) {
        let url = `${this.apiUrl}/app-id${appId}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    getByModuleId(appId: number) {
        let url = `${this.apiUrl}/module-id${appId}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    search(keyword: string = '', appId?: number, moduleId?: number, recordPerPage?: number, currentPage?: number) {
        let params = new URLSearchParams();
        params.set('keyword', keyword);

        if (appId)
            params.set('appId', appId.toString());

        if (moduleId)
            params.set('moduleId', moduleId.toString());

        if (recordPerPage)
            params.set('recordPerPage', recordPerPage.toString());

        if (currentPage)
            params.set('currentPage', currentPage.toString());

        let url = `${this.apiUrl}/search`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '', search: params });

        return this.http.get(url, options).map(x => x.json());
    }

    add(page) {
        let url = `${this.apiUrl}/add`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify(page), options).map(res => res.json());
    }

    update(page) {
        let url = `${this.apiUrl}/update`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.post(url, JSON.stringify(page), options).map(res => res.json());
    }

    delete(id: number) {
        let url = `${this.apiUrl}/delete/?id=${id}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.post(url, options).map(res => res.json());
    }
}