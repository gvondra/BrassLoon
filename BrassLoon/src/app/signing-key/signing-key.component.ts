import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { SigningKey } from '../models/signing-key';
import { SigningKeyService } from '../services/signing-key.service';

@Component({
  selector: 'app-signing-key',
  templateUrl: './signing-key.component.html',
  styles: [
  ]
})
export class SigningKeyComponent implements OnInit {

  private _signingKey: SigningKey = null;

  get SigningKey(): SigningKey { return this._signingKey; }
  @Input() set SigningKey(value: SigningKey) { 
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
    this._signingKey = value; 
  }

  @Output() OnSave = new EventEmitter<SigningKey>();

  LabelColumnClass: string = "col-md-2";
  InputColumnClass: string = "col-md-8";
  ErrorMessage: string = null;
  NotificationMessage: string = null;
  ShowBusy: boolean = false;

  constructor(private signingKeyService: SigningKeyService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
  }

  Save() {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = true;
    if (this.SigningKey.SigningKeyId) {
      this.signingKeyService.Update(this.SigningKey)
      .then(c => {
        this.OnSave.emit(c);
        this.NotificationMessage = "Save complete"; 
      })
      .catch(err => this.CatchWebAPIError(err))
      .finally(() => this.ShowBusy = false)
      ;
    }
    else {
      this.signingKeyService.Create(this.SigningKey)
      .then(c => {
        this.OnSave.emit(c);
        this.NotificationMessage = "Save complete"; 
      })
      .catch(err => this.CatchWebAPIError(err))
      .finally(() => this.ShowBusy = false)
      ;
    }
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
