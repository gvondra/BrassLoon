import { Component, OnInit } from '@angular/core';
import { Account } from '../models/account';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ClientService } from '../services/client.service';
import { Client } from '../models/client';
import { ClientCredentialRequest } from '../models/client-credential-request';

@Component({
  selector: 'app-client',
  templateUrl: './client.component.html',
  styles: [
  ]
})
export class ClientComponent implements OnInit {

  ErrorMessage: string = null;
  Account: Account = null;
  AccountId: string = null;
  ClientRequest: ClientCredentialRequest = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private clientService: ClientService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      this.ClientRequest = null;
      if (params["accountId"]) {
        this.AccountId = params["accountId"];
        this.accountService.Get(params["accountId"])
        .then(account => this.Account = account)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });  
        if (params["clientId"]) {
          this.clientService.Get(params["clientId"])
          .then(client => this.SetRequest(client, ''))
          .catch(err => {
            console.error(err);
            this.ErrorMessage = err.message || "Unexpected Error"
          });  
        }
        else {
          this.clientService.CreateClientSecret()
          .then(secret => {
            let client: Client = new Client()
            client.AccountId = params["accountId"];
            this.SetRequest(client, secret);
          })
          .catch(err => {
            console.error(err);
            this.ErrorMessage = err.message || "Unexpected Error"
          });            
        }
      }
      else {
        this.ErrorMessage = "Account Not Found";
      }
    });
  }

  private SetRequest(client: Client, secret: string) {
    let request: ClientCredentialRequest = new ClientCredentialRequest();
    if (client.ClientId) {
      request.ClientId = client.ClientId;
    }    
    else {
      request.ClientId = null;
    }
    request.AccountId = client.AccountId;
    request.Name = client.Name;
    request.Secret = secret;
    this.ClientRequest = request;
  }

  GenerateSecret() {
    if (this.ClientRequest) {
      this.clientService.CreateClientSecret()
      .then(secret => this.ClientRequest.Secret = secret)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });   
    }    
  }

  Save() {
    if (this.ClientRequest.ClientId && this.ClientRequest.ClientId != '') {
      this.clientService.Update(this.ClientRequest.ClientId, this.ClientRequest)
      .then(client => this.SetRequest(client, ''))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });   
    }
    else {
      this.clientService.Create(this.ClientRequest)
      .then(client => {
        this.router.navigate(['/a', client.AccountId, 'Client', client.ClientId]);
      })
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });   
    }
  }
}
