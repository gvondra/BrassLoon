import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkGroup } from '../models/work-group';
import { WorkGroupService } from '../services/work-group.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-work-group',
  templateUrl: './work-group.component.html',
  styles: [
  ]
})
export class WorkGroupComponent implements OnInit {

  
  ShowBusy: boolean = false;
  Saving: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  WorkGroup: WorkGroup = null;

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
          if (params["id"]) {
            this.LoadWorkGroup(domain.DomainId, params["id"]);
          }
          else {
            this.NewWorkGroup(domain.DomainId);
          }
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
    this.Saving = false;
    this.ErrorMessage = null;
    this.Domain = null;
    this.WorkGroup = null;
  }

  private LoadWorkGroup(domainId, id) {
    this.workGroupService.Get(domainId, id).toPromise()
    .then(workGroup => this.WorkGroup = workGroup)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    .finally(() => this.ShowBusy = false)
    ;
  }

  private NewWorkGroup(domainId) {
    const workGroup: WorkGroup = new WorkGroup();
    workGroup.DomainId = domainId;
    workGroup.Title = "New Type";
    this.WorkGroup = workGroup;    
    this.ShowBusy = false
  }

  Save() {
    this.Saving = true;
    if (this.WorkGroup.WorkGroupId) {
      firstValueFrom(this.workGroupService.Update(this.Domain.DomainId, this.WorkGroup))
      .then(workGroup => this.WorkGroup = workGroup)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
    else {
      firstValueFrom(this.workGroupService.Create(this.Domain.DomainId, this.WorkGroup))
      .then(workGroup => this.router.navigate(['/d', this.Domain.DomainId, 'WGroup', workGroup.WorkGroupId]))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
  }

}
