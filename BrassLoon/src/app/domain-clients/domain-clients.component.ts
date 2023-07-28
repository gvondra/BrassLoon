import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { DomainClientService } from '../services/domain-client.service';
import { DomainClient } from '../models/domain-client';

@Component({
  selector: 'app-domain-clients',
  templateUrl: './domain-clients.component.html',
  styles: [
  ]
})
export class DomainClientsComponent implements OnInit {

  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  Clients: Array<DomainClient> = null;
  SelectedClient: DomainClient = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private clientService: DomainClientService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.InitializeMemberVariables();
      if (params["domainId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadClients(domain.DomainId);
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
  }

  private LoadClients(domainId: string) {
    this.clientService.GetByDomainId(domainId)
    .then(clients => {
      this.Clients = clients;
      if (clients && clients.length > 0) {
        this.SelectedClient = clients[0];
      }
      if (this.SelectedClient == null) {
        this.SelectedClient = this.NewClient();
      }
      this.ShowBusy = false;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
  }

  private NewClient() : DomainClient {
    const client: DomainClient = new DomainClient();
    client.Name = "New Client";
    client.IsActive = true;   
    client.DomainId = this.Domain.DomainId; 
    this.clientService.GetClientCredentialSecret()
    .then(s => client.Secret = s)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
    return client;
  }

  OnNewClient() {
    this.SelectedClient = this.NewClient();
  }

  private InitializeMemberVariables() {
    this.ErrorMessage = null;
    this.Domain = null;
    this.Clients = null;
    this.SelectedClient = null;
    this.ShowBusy = false;
  }

  private FindClientIndex(clients: Array<DomainClient>, client: DomainClient) : number {
    let i : number = 0;
    let found: boolean = false;
    while (!found && i < clients.length) {
      if (clients[i].ClientId === client.ClientId) {
        found = true;
      }
      else {
        i += 1;
      }
    }
    if (found) { return i; }
    else return -1;
  }

  OnSaveClient(client: DomainClient) { 
    const i: number = this.FindClientIndex(this.Clients, client);
    if (i < 0) { 
      this.Clients.push(client);
      this.SelectedClient = client; 
    }
  }

}
