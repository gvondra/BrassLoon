import { TestBed } from '@angular/core/testing';

import { PurgeWorkerService } from './purge-worker.service';

describe('PurgeWorkerService', () => {
  let service: PurgeWorkerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PurgeWorkerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
