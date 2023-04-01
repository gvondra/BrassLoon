import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { WorkTaskStatus } from '../models/work-task-status';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class WorkTaskStatusService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetAll(domainId: string, workTaskTypeId: string): Promise<Array<WorkTaskStatus>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${workTaskTypeId}/Status`)
    );
  }

  Get(domainId: string, workTaskTypeId: string, id: string): Promise<WorkTaskStatus> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${workTaskTypeId}/Status/${id}`)
    );
  }

  Create(domainId: string, workTaskTypeId: string, workTaskStatus: WorkTaskStatus): Promise<WorkTaskStatus> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${workTaskTypeId}/Status`),
      workTaskStatus
    );
  }

  Update(domainId: string, workTaskTypeId: string, workTaskStatus: WorkTaskStatus): Promise<WorkTaskStatus> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${workTaskTypeId}/Status/${workTaskStatus.WorkTaskStatusId}`),
      workTaskStatus
    );
  }
}
