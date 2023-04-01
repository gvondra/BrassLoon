import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { WorkTaskType } from '../models/work-task-type';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class WorkTaskTypeService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetAll(domainId: string): Promise<Array<WorkTaskType>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}`)
    );
  }

  GetByWorkGroupId(domainId: string, workGroupId: string): Promise<Array<WorkTaskType>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkGroup/${domainId}/${workGroupId}/WorkTaskType`)
    );
  }

  Get(domainId: string, id: string): Promise<WorkTaskType> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${id}`)
    );
  }

  Create(domainId: string, workTaskType: WorkTaskType): Promise<WorkTaskType> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}`),
      workTaskType
    );
  }

  Update(domainId: string, workTaskType: WorkTaskType): Promise<WorkTaskType> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetWorkTaskBaseAddress()
      .then(baseAddress => `${baseAddress}WorkTaskType/${domainId}/${workTaskType.WorkTaskTypeId}`),
      workTaskType
    );
  }
}
