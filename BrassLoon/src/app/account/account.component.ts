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

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private domainService: DomainService,
    private oidcSecurityService: OidcSecurityService,
    private tokenService: TokenService,
    private httpClientUtil: HttpClientUtilService) { }

  ngOnInit(): void {   
    this.RoleCheck();         
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      this.Domains = null;
      this.DeletedDomains = null;
      this.NewDomainName = null;
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
        this.LoadDomains(params["id"]);
      }
      else {
        this.IsNew = true;
        this.Account= new Account();
      }
    });
  }

  private RoleCheck() {
    this.oidcSecurityService.isAuthenticated$.subscribe(isAuthenticated => {
      this.ShowAdmin = false;
      if (isAuthenticated) {
        this.httpClientUtil.GetRoles(this.tokenService)
        .then(role => {
          if (role && role.length > 0 && role.some(r => r === 'actadmin')) {
            this.ShowAdmin = true;
          }
          this.LoadDeletedDomains(this.AccountId);
        })
        .catch(err => {
          console.error(err);
        });   
      }
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
}
