import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { WorkTaskType } from '../models/work-task-type';
import { mergeMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkTaskTypeService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetAll(domainId: string): Observable<Array<WorkTaskType>> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<Array<WorkTaskType>>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}`, {headers: headers}))
    );
  }

  GetByWorkGroupId(domainId: string, workGroupId: string): Observable<Array<WorkTaskType>> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<Array<WorkTaskType>>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkGroup/${domainId}/${workGroupId}/WorkTaskType`, {headers: headers}))
    );
  }

  Get(domainId: string, id: string): Observable<WorkTaskType> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.get<WorkTaskType>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${id}`, {headers: headers}))
    );
  }

  Create(domainId: string, workTaskType: WorkTaskType): Observable<WorkTaskType> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.post<WorkTaskType>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}`, workTaskType, {headers: headers}))
    );
  }

  Update(domainId: string, workTaskType: WorkTaskType): Observable<WorkTaskType> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => this.httpClient.put<WorkTaskType>(`${this.httpClientUtil.GetWorkTaskBaseAddress()}WorkTaskType/${domainId}/${workTaskType.WorkTaskTypeId}`, workTaskType, {headers: headers}))
    );
  }
}
