import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { WorkTaskStatus } from '../models/work-task-status';
import { mergeMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkTaskStatusService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetAll(domainId: string, workTaskTypeId: string): Observable<Array<WorkTaskStatus>> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<Array<WorkTaskStatus>>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${workTaskTypeId}/Status`, {headers: headers}))
    );
  }

  Get(domainId: string, workTaskTypeId: string, id: string): Observable<WorkTaskStatus> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<WorkTaskStatus>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${workTaskTypeId}/Status/${id}`, {headers: headers}))
    );
  }

  Create(domainId: string, workTaskTypeId: string, workTaskStatus: WorkTaskStatus): Observable<WorkTaskStatus> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.post<WorkTaskStatus>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${workTaskTypeId}/Status`, workTaskStatus, {headers: headers}))
    );
  }

  Update(domainId: string, workTaskTypeId: string, workTaskStatus: WorkTaskStatus): Observable<WorkTaskStatus> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.put<WorkTaskStatus>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${workTaskTypeId}/Status/${workTaskStatus.WorkTaskStatusId}`, workTaskStatus, {headers: headers}))
    );
  }
}
