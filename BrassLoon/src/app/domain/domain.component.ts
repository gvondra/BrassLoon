import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';

@Component({
  selector: 'app-domain',
  templateUrl: './domain.component.html',
  styles: [
  ]
})
export class DomainComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private domainService: DomainService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Domain == null;
      if (params["id"]) {   
        this.domainService.Get(params["id"])     
        .then(domain => this.Domain = domain)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });    
      }
    });
  }

  UpdateDomain(domain: Domain) : void {
    if (domain.Name && domain.Name != "") {
      this.domainService.Update(domain.DomainId, domain)    
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });  
    }
  }

}
