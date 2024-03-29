import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { ConfigLookupService } from '../services/config-lookup.service';
import { DomainService } from '../services/domain.service';

@Component({
  selector: 'app-lookup-codes',
  templateUrl: './lookup-codes.component.html',
  styles: [
  ]
})
export class LookupCodesComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  Codes: string[] = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private configLookupService: ConfigLookupService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Domain == null;      
      if (params["domainId"]) { 
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });  
        this.LoadCodes(params["domainId"]);
      }
    });
  }

  private LoadCodes(domainId: string) {
    this.Codes = null;
    this.configLookupService.GetCodes(domainId)
    .then(codes => this.Codes = codes)
    .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
    })
  }

}
