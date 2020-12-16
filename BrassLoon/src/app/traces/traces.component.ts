import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { Trace } from '../models/trace';
import { DomainService } from '../services/domain.service';
import { TraceService } from '../services/trace.service';

@Component({
  selector: 'app-traces',
  templateUrl: './traces.component.html',
  styles: [
  ]
})
export class TracesComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  MaxTimestamp: string = null;
  ShowBusy: boolean = false;
  EventCodes: Array<string> = null;
  EventCode: string = null;
  Traces: Array<Trace> = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private traceService: TraceService) { }

  ngOnInit(): void {
    this.MaxTimestamp = null;
    this.EventCodes = null;
    this.activatedRoute.params.subscribe(params => {
      this.ShowBusy = false;
      this.ErrorMessage = null;
      this.Domain == null;
      this.EventCodes = null;
      this.Traces = null;
      if (params["domainId"]) { 
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadTraces();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });  
        this.traceService.GetEventCodes(params["domainId"])          
        .then(codes => {
          this.EventCodes = codes;
          this.LoadTraces();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
    this.activatedRoute.queryParams.subscribe(params => {
      this.MaxTimestamp = null;
      this.EventCode = null;
      this.Traces = null;
      if (params["maxTimestamp"] && params["eventCode"]) {
        let dt: Date = new Date(Number(params["maxTimestamp"]));
        this.MaxTimestamp = dt.toLocaleString();     
        this.EventCode = params["eventCode"];
      }
      this.LoadTraces();
    });
  }

  private LoadTraces() {
    if (this.Domain && this.EventCodes && this.EventCodes.length > 0) {
      if (this.MaxTimestamp == null || this.EventCode == null || this.EventCode == "") {
        let maxTimestamp: Date = new Date();
        maxTimestamp = new Date(maxTimestamp.getFullYear(), maxTimestamp.getMonth(), maxTimestamp.getDate() + 1);
        this.MaxTimestamp = maxTimestamp.toLocaleDateString();
        this.EventCode = this.EventCodes[0];
        this.Load();
      }
      else {
        this.ShowBusy = true;
        const dt: Date = new Date(this.MaxTimestamp);
        this.traceService.Search(this.Domain.DomainId, dt.toISOString(), this.EventCode)
        .then(traces => this.Traces = traces)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false); 
      }
    }
  }  

  Load() {
    this.router.navigate(["/d", this.Domain.DomainId, "Trace"], { queryParams: { "maxTimestamp": Date.parse(this.MaxTimestamp), "eventCode": this.EventCode } })
  }

  FormatDate(dt: string) : string {
    let result: string = "";
    if (dt && dt != "") {
      let hold: Date = new Date(dt);
      result = hold.toLocaleString();
    }
    return result;
  }

  GetEventCodeSelect(code) : string {
    let result: string = null;
    if (code && this.EventCode && code == this.EventCode) {
      result = "true";
    }
    return result;
  }

  GetKeys(object) : Array<string> {
    return Object.keys(object);
  }
}
