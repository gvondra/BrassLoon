import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkGroup } from '../models/work-group';
import { WorkGroupService } from '../services/work-group.service';
import { firstValueFrom } from 'rxjs';
import { WorkTaskTypeService } from '../services/work-task-type.service';
import { WorkTaskType } from '../models/work-task-type';

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
  WorkTaskTypes: Array<WorkTaskType> = [];
  ShowWorkTaskTypes: boolean = false;
  AllWorkTaskTypes: Array<WorkTaskType> = [];
  SelectedWorkTaskTypeId: string | null = null;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private domainService: DomainService,
    private workGroupService: WorkGroupService,
    private workTaskTypeService: WorkTaskTypeService) { }

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
            this.LoadWorkTaskTypes(domain.DomainId, params["id"]);
            this.LoadAllWorkTaskTypes(domain.DomainId);
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
    this.WorkTaskTypes = [];
    this.AllWorkTaskTypes = [];
    this.ShowWorkTaskTypes = false;
    this.SelectedWorkTaskTypeId = null;
  }

  private LoadWorkGroup(domainId, id) {
    firstValueFrom(this.workGroupService.Get(domainId, id))
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

  private LoadWorkTaskTypes(domain, id): void {
    firstValueFrom(this.workTaskTypeService.GetByWorkGroupId(domain, id))
    .then(types => {
      this.WorkTaskTypes = types;
      this.ShowWorkTaskTypes = true;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    ;
  }

  private LoadAllWorkTaskTypes(domain): void {
    firstValueFrom(this.workTaskTypeService.GetAll(domain))
    .then(types => {
      this.AllWorkTaskTypes = types;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    ;
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

  LinkWorkTaskType() {
    if (this.SelectedWorkTaskTypeId != null && this.SelectedWorkTaskTypeId != "") {
      firstValueFrom(this.workGroupService.AddWorkTaskTypeLink(this.Domain.DomainId, this.WorkGroup.WorkGroupId, this.SelectedWorkTaskTypeId))
      .then(() => this.LoadWorkTaskTypes(this.Domain.DomainId, this.WorkGroup.WorkGroupId))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });
    }
  }

  UnLinkWorkTaskType(workTaskTypeId) {
    firstValueFrom(this.workGroupService.DeleteWorkTaskTypeLink(this.Domain.DomainId, this.WorkGroup.WorkGroupId, workTaskTypeId))
    .then(() => this.LoadWorkTaskTypes(this.Domain.DomainId, this.WorkGroup.WorkGroupId))
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
  }

}
