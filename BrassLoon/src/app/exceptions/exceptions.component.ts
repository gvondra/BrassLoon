import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { Exception } from '../models/exception';
import { ExceptionService } from '../services/exception.service';

@Component({
  selector: 'app-exceptions',
  templateUrl: './exceptions.component.html',
  styles: [
  ]
})
export class ExceptionsComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  MaxTimestamp: string = null;
  ShowBusy: boolean = false;
  Exceptions: Array<Exception> = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private exceptionService: ExceptionService) { }

  ngOnInit(): void {
    this.MaxTimestamp = null;
    this.activatedRoute.params.subscribe(params => {
      this.ShowBusy = false;
      this.ErrorMessage = null;
      this.Domain == null;
      if (params["domainId"]) { 
        this.Exceptions = null;  
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadExceptions();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });            
      }
    });
    this.activatedRoute.queryParams.subscribe(params => {
      this.MaxTimestamp = null;
      this.Exceptions = null;
      if (params["maxTimestamp"]) {
        let dt: Date = new Date(Number(params["maxTimestamp"]));
        this.MaxTimestamp = dt.toLocaleString();        
      }
      this.LoadExceptions();
    });
  }

  private LoadExceptions() {
    if (this.Domain) {
      if (this.MaxTimestamp == null) {
        let maxTimestamp: Date = new Date();
        maxTimestamp = new Date(maxTimestamp.getFullYear(), maxTimestamp.getMonth(), maxTimestamp.getDate() + 1);
        this.MaxTimestamp = maxTimestamp.toLocaleDateString();
        this.Load();
      }   
      else {
        this.ShowBusy = true;
        const dt: Date = new Date(this.MaxTimestamp);
        this.exceptionService.Search(this.Domain.DomainId, dt.toISOString())
        .then(exceptions => this.Exceptions = exceptions)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false)
        ;         
      }   
    }    
  }

  Load() {
    this.router.navigate(["/d", this.Domain.DomainId, "Exception"], { queryParams: { "maxTimestamp": Date.parse(this.MaxTimestamp) } })
  }

  FormatDate(dt: string) : string {
    let result: string = "";
    if (dt && dt != "") {
      let hold: Date = new Date(dt);
      result = hold.toLocaleString();
    }
    return result;
  }
}
