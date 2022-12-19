import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DomainClientService } from '../services/domain-client.service';
import { DomainClient } from '../models/domain-client';
import { RoleService } from '../services/role.service';
import { Role } from '../models/role';

@Component({
  selector: 'app-domain-client',
  templateUrl: './domain-client.component.html',
  styles: [
  ]
})
export class DomainClientComponent implements OnInit {

  private _client: DomainClient = null;

  get Client(): DomainClient { return this._client; }
  @Input() set Client(value: DomainClient) { 
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
    this.Secret = null;
    this.Roles = null;
    this._client = value; 
    if (value) {
      this.LoadRoles(value.DomainId);
    }
  }

  @Output() OnSave = new EventEmitter<DomainClient>();

  LabelColumnClass: string = "col-md-2";
  InputColumnClass: string = "col-md-8";
  ErrorMessage: string = null;
  NotificationMessage: string = null;
  ShowBusy: boolean = false;
  Secret: string = null;
  Roles: Array<Role> = null;

  constructor(private clientService: DomainClientService,
    private roleService: RoleService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
    this.Secret = null;
  }

  private LoadRoles(domainId: string) {
    this.Roles = null;
    this.roleService.GetByDomainId(domainId)
    .then(roles => this.Roles = roles)
    .catch(err => this.CatchWebAPIError(err))
    ;
  }

  private FindClientRoleIndex(client: DomainClient, role: Role): number {
    let i = 0;
    let found = false;    
    if (client && client.Roles) {
      while (!found && i < client.Roles.length) {
        if (client.Roles[i].PolicyName === role.PolicyName) {
          found = true;
        }
        else {
          i += 1;
        }
      }
    }
    if (!found) {
      i = -1;
    }
    return i;
  }

  IsActiveRole(role: Role): boolean {
    const i = this.FindClientRoleIndex(this._client, role);
    return (i >= 0);
  }

  OnChangeActiveRole(role: Role, event) {    
    const client: DomainClient = this._client;
    const i = this.FindClientRoleIndex(client, role);
    if (event.target.checked && i < 0) {
      if (!client.Roles) {
        client.Roles = [];
      }
      client.Roles.push({ "PolicyName": role.PolicyName,  "Name": role.Name});
    }
    if (!event.target.checked && i >= 0) {
      client.Roles.splice(i, 1);
    }
  }

  Save() {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = true;
    const secret = this.Client.Secret;
    if (this.Client.ClientId) {
      this.clientService.Update(this.Client)
      .then(c => {
        this.OnSave.emit(c);
        this.NotificationMessage = "Save complete"; 
        this.Secret = secret;       
      })
      .catch(err => this.CatchWebAPIError(err))
      .finally(() => this.ShowBusy = false)
      ;
    }
    else {
      this.clientService.Create(this.Client)
      .then(c => {
        this.OnSave.emit(c);
        this.NotificationMessage = "Save complete";        
        this.Secret = secret;
      })
      .catch(err => this.CatchWebAPIError(err))
      .finally(() => this.ShowBusy = false)
      ;
    }
  }

  GenerateSecret() {
    this.clientService.GetClientCredentialSecret()
    .then(s => this.Client.Secret = s)
    .catch(err => this.CatchWebAPIError(err))
    ;
  }

  private CatchWebAPIError(err) {
    console.error(err);
    if (err.error && typeof err.error === "string") {
      this.ErrorMessage = err.error  
    }
    else {
      this.ErrorMessage = err.message || "Unexpected Error"
    }
    
  }
}
