import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { UserInvitation } from '../models/user-invitation';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class UserInvitationService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetByAccountId(accountId: string) : Promise<Array<UserInvitation>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${accountId}/Invitation`)
    ); 
  }

  Get(id: string) : Promise<UserInvitation> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}UserInvitation/${id}`)
    );
  }

  Create(accountId: string, invitation: UserInvitation) :Promise<UserInvitation> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${accountId}/Invitation`),
      invitation
    );
  }

  Update(id: string, invitation: UserInvitation) : Promise<UserInvitation> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}UserInvitation/${id}`),
      invitation
    );
  }
}
