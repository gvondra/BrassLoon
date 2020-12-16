import { Component, OnInit } from '@angular/core';
import { Account } from '../models/account';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { Client } from '../models/client';
import { HttpClientUtilService } from '../http-client-util.service';

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
  NewDomainName: string = null;
  Clients: Array<Client> = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private domainService: DomainService,
    private httpClientUtilService: HttpClientUtilService) { }

  ngOnInit(): void {            
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      this.Domains = null;
      this.NewDomainName = null;
      this.Clients = null;
      if (params["id"]) {
        this.IsNew = false;
        this.AccountId = params["id"];
        this.accountService.Get(params["id"])
        .then(account => this.Account = account)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });    
        this.accountService.GetDomains(params["id"])
        .then(domains => this.Domains = domains)
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
      }
      else {
        this.IsNew = true;
        this.Account= new Account();
      }
    });
  }

  Save() {
    if (this.IsNew) {
      this.accountService.Create(this.Account)
      .then(account => {
        // an existing access token wouldn't have this new account
        // listed, so drop the cache to force retrieval of a new token
        this.httpClientUtilService.DropCache();
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
}
