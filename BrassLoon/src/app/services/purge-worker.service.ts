import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { PurgeWorker } from '../models/purge-worker';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class PurgeWorkerService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }
   
  Search() : Promise<Array<PurgeWorker>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}PurgeWorker`)
    );  
  }
  
  Get(id: string) : Promise<PurgeWorker> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}PurgeWorker/${id}`)
    );  
  }
  
  PatchStatus(id: string, status: number) : Promise<PurgeWorker> {
    return this.httpClientUtil.PatchRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}PurgeWorker/${id}/Status`),
      { "Status": status }
    );  
  }
}
