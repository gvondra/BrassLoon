import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { WorkGroup } from '../models/work-group';
import { mergeMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkGroupService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetAll(domainId: string): Observable<Array<WorkGroup>> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<Array<WorkGroup>>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkGroup/${domainId}`, {headers: headers}))
    );
  }

  Get(domainId: string, id: string): Observable<WorkGroup> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<WorkGroup>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkGroup/${domainId}/${id}`, {headers: headers}))
    );
  }

  Create(domainId: string, workTaskType: WorkGroup): Observable<WorkGroup> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.post<WorkGroup>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkGroup/${domainId}`, workTaskType, {headers: headers}))
    );
  }

  Update(domainId: string, workTaskType: WorkGroup): Observable<WorkGroup> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.put<WorkGroup>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkGroup/${domainId}/${workTaskType.WorkGroupId}`, workTaskType, {headers: headers}))
    );
  }
}
