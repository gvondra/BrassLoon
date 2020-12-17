import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { Metric } from '../models/metric';
import { DomainService } from '../services/domain.service';
import { MetricService } from '../services/metric.service';

@Component({
  selector: 'app-metrics',
  templateUrl: './metrics.component.html',
  styles: [
  ]
})
export class MetricsComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  MaxTimestamp: string = null;
  MinLoadedTimestamp: string = null;
  ShowLoadNext: boolean = false;
  ShowBusy: boolean = false;
  EventCodes: Array<string> = null;
  EventCode: string = null;
  Metrics: Array<Metric> = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private metricService: MetricService) { }

  ngOnInit(): void {
    this.MaxTimestamp = null;
    this.MinLoadedTimestamp = null;
    this.ShowLoadNext = false;
    this.EventCodes = null;
    this.activatedRoute.params.subscribe(params => {
      this.ShowBusy = false;
      this.ErrorMessage = null;
      this.Domain == null;
      this.EventCodes = null;
      this.Metrics = null;
      this.MinLoadedTimestamp = null;
      this.ShowLoadNext = false;
      if (params["domainId"]) { 
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadMetrics();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });  
        this.metricService.GetEventCodes(params["domainId"])          
        .then(codes => {
          this.EventCodes = codes;
          this.LoadMetrics();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
    this.activatedRoute.queryParams.subscribe(params => {
      this.MaxTimestamp = null;
      this.MinLoadedTimestamp = null;
      this.ShowLoadNext = false;
      this.EventCode = null;
      this.Metrics = null;
      if (params["maxTimestamp"] && params["eventCode"]) {
        let dt: Date = new Date(Number(params["maxTimestamp"]));
        this.MaxTimestamp = dt.toLocaleString();     
        this.EventCode = params["eventCode"];
      }
      this.LoadMetrics();
    });
  }

  private LoadMetrics() {
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
        let dt: Date = new Date(this.MaxTimestamp);
        dt = new Date(dt.toUTCString());
        this.metricService.Search(this.Domain.DomainId, dt.toISOString(), this.EventCode)
        .then(metrics => this.AppendMetricsResult(metrics))
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false); 
      }
    }
  }  

  private AppendMetricsResult(metrics: Array<Metric>) : void {
    if (!this.Metrics) {
      this.Metrics = metrics;
    }    
    else {
      this.Metrics = this.Metrics.concat(metrics);
    }
    if (metrics && metrics.length > 0) {
      this.ShowLoadNext = true;
    }
    else {
      this.ShowLoadNext = false;
    }
    this.MinLoadedTimestamp = this.GetMinLoadedTimestamp(metrics);
  }

  private GetMinLoadedTimestamp(metrics: Array<Metric>) : string {
    // it's assumed that the records are in descending order
    if (metrics && metrics.length > 0) {
      return metrics[metrics.length - 1].CreateTimestamp;
    }
    else {
      return null;
    }
  }

  LoadNext() {
    this.ShowBusy = false;
    this.ShowBusy = true;
    let dt: Date = new Date(this.MinLoadedTimestamp);
    dt = new Date(dt.toUTCString());
    this.metricService.Search(this.Domain.DomainId, dt.toISOString(), this.EventCode)
    .then(metrics => this.AppendMetricsResult(metrics))
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    .finally(() => this.ShowBusy = false); 
  }

  Load() {
    this.router.navigate(["/d", this.Domain.DomainId, "Metric"], { queryParams: { "maxTimestamp": Date.parse(this.MaxTimestamp), "eventCode": this.EventCode } })
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
