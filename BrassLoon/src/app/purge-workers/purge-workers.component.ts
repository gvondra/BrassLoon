import { Component, OnInit } from '@angular/core';
import { PurgeWorkerService } from '../services/purge-worker.service';
import { PurgeWorker } from '../models/purge-worker';

@Component({
  selector: 'app-purge-workers',
  templateUrl: './purge-workers.component.html',
  styles: [
  ]
})
export class PurgeWorkersComponent implements OnInit {

  ErrorMessage: string = null;
  PurgeWorkers: Array<PurgeWorker> = null;

  constructor(private purgeWorkerService: PurgeWorkerService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.PurgeWorkers = null;
    this.purgeWorkerService.Search()
    .then(workers => this.PurgeWorkers = workers)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });    
  }

  FormatTimestamp(value: string) : string {
    if (value && value != "") {
      let dt : Date = new Date(value);
      return dt.toLocaleString();
    }
    else {
      return value;
    }
  }

  GetStatusText(value: number) : string {
    switch(value) { 
      case 0: { 
         return "Ready";
         break; 
      } 
      case 1: { 
         return "In Progress";
         break; 
      } 
      case 255: { 
         return "Complete";
         break; 
      } 
      default: { 
         return "Error"; 
         break; 
      } 
   } 
  }

  CanReset(status: number) : boolean {
    if (status != 0) {
      return true;
    }
    else {
      return false;
    }
  }

  ResetStatus(purgeWorker: PurgeWorker) {
    this.ErrorMessage = null;
    this.purgeWorkerService.PatchStatus(purgeWorker.PurgeWorkerId, 0)
    .then(res => purgeWorker.Status = res.Status)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });    
  }

}
