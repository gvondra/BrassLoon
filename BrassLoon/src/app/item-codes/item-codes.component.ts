import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { ConfigItemService } from '../services/config-item.service';

@Component({
  selector: 'app-item-codes',
  templateUrl: './item-codes.component.html',
  styles: [
  ]
})
export class ItemCodesComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  Codes: string[] = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private configItemService: ConfigItemService) { }

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
    this.configItemService.GetCodes(domainId).subscribe(
      codes => this.Codes = codes,
      err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      }
    )
  }

}
