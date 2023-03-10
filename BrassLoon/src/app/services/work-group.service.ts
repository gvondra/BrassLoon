import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { WorkGroup } from '../models/work-group';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class WorkGroupService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetAll(domainId: string): Promise<Array<WorkGroup>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}`)
    );
  }

  Get(domainId: string, id: string): Promise<WorkGroup> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}/${id}`)
    );
  }

  Create(domainId: string, workTaskType: WorkGroup): Promise<WorkGroup> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}`),
      workTaskType
    );
  }

  Update(domainId: string, workTaskType: WorkGroup): Promise<WorkGroup> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}/${workTaskType.WorkGroupId}`),
      workTaskType
    );
  }

  AddWorkTaskTypeLink(domainId: string, workGroupId: string, workTaskTypeId: string): Promise<any> {  
    let params: HttpParams = new HttpParams()
    .append("workTaskTypeId", workTaskTypeId);
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}/${workGroupId}/WorkTaskType`),
      params = params
    ); 
  }

  DeleteWorkTaskTypeLink(domainId: string, workGroupId: string, workTaskTypeId: string): Promise<any> {  
    let params: HttpParams = new HttpParams()
    .append("workTaskTypeId", workTaskTypeId);
    return this.httpClientUtil.DeleteRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}/${workGroupId}/WorkTaskType`),
      params = params
    );
  }
}
