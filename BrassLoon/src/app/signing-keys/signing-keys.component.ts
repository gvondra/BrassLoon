import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { SigningKey } from '../models/signing-key';
import { SigningKeyService } from '../services/signing-key.service';
import { AppSettingsService } from '../app-settings.service';

@Component({
  selector: 'app-signing-keys',
  templateUrl: './signing-keys.component.html',
  styles: [
  ]
})
export class SigningKeysComponent implements OnInit {

  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  SigningKeys: Array<SigningKey> = null;
  SelectedSigningKey: SigningKey = null; 
  JwksLink: string | null = null;
  
  constructor(private activatedRoute: ActivatedRoute,
    private appSettings: AppSettingsService,
    private domainService: DomainService,
    private signingKeyService: SigningKeyService) { }

  ngOnInit(): void {
    this.JwksLink = null;
    this.activatedRoute.params.subscribe(params => {
      this.InitializeMemberVariables();
      if (params["domainId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadSigningKeys(domain.DomainId);
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
        this.appSettings.GetSettings()
        .then(appSettings => this.JwksLink = `${appSettings.AuthoriaztionBaseAddress}jwks/${this.Domain.DomainId}`)
      }
    });
  }

  private LoadSigningKeys(domainId: string) {
    this.signingKeyService.GetByDomainId(domainId)
    .then(signingKeys => {
      this.SigningKeys = signingKeys;
      if (signingKeys && signingKeys.length > 0) {
        this.SelectedSigningKey = signingKeys[0];
      }
      if (this.SelectedSigningKey == null) {
        this.SelectedSigningKey = this.NewSigningKey();
      }
      this.ShowBusy = false;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
  }

  private NewSigningKey() : SigningKey {
    const signingKey: SigningKey = new SigningKey();
    signingKey.IsActive = true;   
    signingKey.DomainId = this.Domain.DomainId; 
    return signingKey;
  }

  OnNewSigningKey() {
    this.SelectedSigningKey = this.NewSigningKey();
  }

  private InitializeMemberVariables() {
    this.ErrorMessage = null;
    this.Domain = null;
    this.SigningKeys = null;
    this.SelectedSigningKey = null;
    this.ShowBusy = false;
  }

  private FindSigningKeyIndex(signingKeys: Array<SigningKey>, signingKey: SigningKey) : number {
    let i : number = 0;
    let found: boolean = false;
    while (!found && i < signingKeys.length) {
      if (signingKeys[i].SigningKeyId === signingKey.SigningKeyId) {
        found = true;
      }
      else {
        i += 1;
      }
    }
    if (found) { return i; }
    else return -1;
  }

  OnSaveSigningKey(signingKey: SigningKey) { 
    const i: number = this.FindSigningKeyIndex(this.SigningKeys, signingKey);
    if (i >= 0) { this.SigningKeys[i] = signingKey; }
    else { this.SigningKeys.push(signingKey); }
  }
}
