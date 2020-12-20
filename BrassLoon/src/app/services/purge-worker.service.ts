import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { PurgeWorker } from '../models/purge-worker';

@Injectable({
  providedIn: 'root'
})
export class PurgeWorkerService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }
   
  Search() : Promise<Array<PurgeWorker>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}PurgeWorker`, {headers: headers}).toPromise()
        .then(res => res as PurgeWorker[]);
    });      
  }
  
  Get(id: string) : Promise<PurgeWorker> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}PurgeWorker/${id}`, {headers: headers}).toPromise()
        .then(res => res as PurgeWorker);
    });      
  }
  
  PatchStatus(id: string, status: number) : Promise<PurgeWorker> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.patch(`${this.httpClientUtil.GetLogBaseAddress()}PurgeWorker/${id}/Status`, { "Status": status }, {headers: headers}).toPromise()
        .then(res => res as PurgeWorker);
    });      
  }
}
