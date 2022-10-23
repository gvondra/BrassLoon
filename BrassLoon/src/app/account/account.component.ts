import { Component, OnInit } from '@angular/core';
import { Account } from '../models/account';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { Client } from '../models/client';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from '../services/token.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { UserInvitation } from '../models/user-invitation';
import { UserInvitationService } from '../services/user-invitation.service';
import { User } from '../models/user';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styles: [
  ]
})
export class AccountComponent implements OnInit {

  ErrorMessage: string = null;
  Account: Account = null;
  AccountId: string = null;
  IsNew: boolean = false;
  Domains: Array<Domain> = null;
  DeletedDomains: Array<Domain> = null;
  NewDomainName: string = null;
  Clients: Array<Client> = null;
  ShowAdmin: boolean = false;
  Invitations: UserInvitation[] = null;
  Users: Array<User> = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private domainService: DomainService,
    private invitationService: UserInvitationService,
    private oidcSecurityService: OidcSecurityService,
    private tokenService: TokenService,
    private httpClientUtil: HttpClientUtilService) { }

  ngOnInit(): void {       
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      this.Domains = null;
      this.DeletedDomains = null;
      this.NewDomainName = null;
      this.Users = null;
      this.Invitations = null;
      this.Clients = null;
      this.ShowAdmin = false;
      if (params["id"]) {
        this.IsNew = false;
        this.AccountId = params["id"];
        this.accountService.Get(params["id"])
        .then(account => this.Account = account)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });            
        this.accountService.GetClients(params["id"])
        .then(clients => this.Clients = clients)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });
        
        this.LoadInvitations(params["id"]);
        this.LoadDomains(params["id"]);
      }
      else {
        this.IsNew = true;
        this.Account= new Account();
      }
    });   
    this.RoleCheck();  
  }

  private RoleCheck() {
    this.oidcSecurityService.isAuthenticated$.subscribe(isAuthenticated => {
      this.ShowAdmin = false;
      if (isAuthenticated.isAuthenticated) {
        this.httpClientUtil.GetRoles(this.tokenService)
        .then(role => {
          if (role && role.length > 0 && role.some(r => r === 'actadmin')) {
            this.ShowAdmin = true;
          }
          if (this.AccountId) {
            this.LoadDeletedDomains(this.AccountId);
            this.LoadUsers(this.AccountId);
          }
        })
        .catch(err => {
          console.error(err);
        });   
      }
    });
  }

  private LoadInvitations(accountId: string) {    
    this.invitationService.GetByAccountId(accountId)
    .then(invitations => {
      this.Invitations = [];
      if (invitations) {
        for (let invitation of invitations) {
          const currentDate = new Date();
          if (invitation.Status === 0 && currentDate < new Date(Date.parse(invitation.ExpirationTimestamp))) {
            this.Invitations.push(invitation);
          }
        }
      }
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }

  private LoadUsers(accountId: string) {
    this.accountService.GetUsers(accountId)
        .then(users => this.Users = users)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });
  }

  private LoadDomains(accountId: string) {
    this.accountService.GetDomains(accountId)
    .then(domains => {
      this.Domains = domains;
      this.LoadDeletedDomains(accountId);
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }

  private LoadDeletedDomains(accountId: string) {
    if (this.AccountId && this.ShowAdmin && this.Domains) {
      this.accountService.GetDomainsByDeleted(accountId, true)
      .then(domains => {
        if (domains && domains.length > 0) {
          this.DeletedDomains = domains;
        }        
        else {
          this.DeletedDomains = null;
        }
      })
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      }); 
      ;
    }
  }

  Save() {
    if (this.IsNew) {
      this.accountService.Create(this.Account)
      .then(account => {
        // an existing access token wouldn't have this new account
        // listed, so drop the cache to force retrieval of a new token
        this.httpClientUtil.DropCache();
        this.router.navigate(["/a", account.AccountId])
      })
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });    
    }
    else {
      this.accountService.Update(this.AccountId, this.Account)
      .then(account => this.Account = account)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });    
    }
  }

  AddDomain() : void {
    if (this.NewDomainName && this.NewDomainName != "") {
      let domain: Domain = new Domain();
      domain.Name = this.NewDomainName;
      domain.AccountId = this.Account.AccountId;
      this.domainService.Create(domain)    
      .then(domain => {
        this.NewDomainName = null;
        this.Domains.push(domain);
      })
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });  
    }
  }

  UnDelete(id: string) {
    this.domainService.UnDelete(id)
    .then(domain => {
      this.Domains = null;
      this.DeletedDomains = null;
      this.LoadDomains(domain.AccountId);
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });  
  }

  RemoveUser(userId: string) {
    this.accountService.RemoveUser(this.AccountId, userId)
    .then(() => {
      this.Users = null;
      this.LoadUsers(this.AccountId);
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });  
  }

  GetLockButtonText() : string {
    if (this.Account && this.Account.Locked) {
      return 'Unlock Account';
    }
    else {
      return 'Lock Account';
    }
  }

  GetLockText() : string {
    if (this.Account && this.Account.Locked) {
      return 'unlock';
    }
    else {
      return 'lock';
    }
  }

  ToggleLock() {
    this.accountService.UpdateLock(this.Account.AccountId, !this.Account.Locked)
    .then(() => this.Account.Locked = !this.Account.Locked)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });  
  }
}
