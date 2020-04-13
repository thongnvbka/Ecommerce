import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, RequestMethod, BaseRequestOptions, Request, Response  } from '@angular/http';
import 'rxjs/Rx';
//import { erpApiDomain } from '../../shared/constants/app.constant';

@Injectable()
export class ModuleService {
    private apiDomain: string;
    private headers: Headers;

    constructor(private http: Http) {
        //this.apiDomain = erpApiDomain;
    }

    private setHeaders() {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'application/json');
        this.headers.append('Authorization', 'Bearer ' + localStorage.getItem('access_token'));
    }

    getAll() {
        let url = `/module/all`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers, body: '' });

        return this.http.get(url, options).map(x => x.json());
    }

    getByAppId(appId: number) {
        let url = `/module/app-id${appId}`;

        this.setHeaders();

        let options = new RequestOptions({ headers: this.headers });

        return this.http.get(url, options).map(x => x.json());
    }
}