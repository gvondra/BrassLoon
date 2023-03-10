import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkGroup } from '../models/work-group';
import { WorkGroupService } from '../services/work-group.service';

@Component({
  selector: 'app-work-groups',
  templateUrl: './work-groups.component.html',
  styles: [
  ]
})
export class WorkGroupsComponent implements OnInit {

  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  WorkGroups: Array<WorkGroup> = null;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private domainService: DomainService,
    private workGroupService: WorkGroupService) { }

    ngOnInit(): void {
      this.activatedRoute.params.subscribe(params => {
        this.InitializeComponent();
        if (params["domainId"]) { 
          this.ShowBusy = true;
          this.domainService.Get(params["domainId"])     
          .then(domain => {
            this.Domain = domain;
            this.LoadWorkGroups(domain.DomainId);
          })
          .catch(err => {
            console.error(err);
            this.ErrorMessage = err.message || "Unexpected Error"
          }); 
        }
      });
    }
  
    private InitializeComponent() {
      this.ShowBusy = false;
      this.ErrorMessage = null;
      this.Domain = null;
      this.WorkGroups = null;
    }
  
    private LoadWorkGroups(domainId) {
      this.workGroupService.GetAll(domainId)
      .then(workGroups => this.WorkGroups = workGroups)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.ShowBusy = false)
      ;
    }
  
    OnNewGroup() {
      this.router.navigate(['/d', this.Domain.DomainId, 'WGroup']);
    }

}
