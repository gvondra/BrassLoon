import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { DomainClientService } from '../services/domain-client.service';
import { DomainClient } from '../models/domain-client';

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
    this._client = value; 
  }

  @Output() OnSave = new EventEmitter<DomainClient>();

  LabelColumnClass: string = "col-md-2";
  InputColumnClass: string = "col-md-8";
  ErrorMessage: string = null;
  NotificationMessage: string = null;
  ShowBusy: boolean = false;
  Secret: string = null;

  constructor(private clientService: DomainClientService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
    this.Secret = null;
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
        this._client = c;
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
        this._client = c;
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
    this.ErrorMessage = err.error || err.message || "Unexpected Error"
  }
}
