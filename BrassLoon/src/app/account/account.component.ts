import { Component, OnInit } from '@angular/core';
import { Account } from '../models/account';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';

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

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private domainService: DomainService) { }

  ngOnInit(): void {            
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      this.Domains = null;
      this.NewDomainName = null;
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
      .then(account => this.router.navigate(["/a", account.AccountId]))
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

  UpdateDomain(domain: Domain) : void {
    if (domain.Name && domain.Name != "") {
      this.domainService.Update(domain.DomainId, domain)    
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });  
    }
  }
}
