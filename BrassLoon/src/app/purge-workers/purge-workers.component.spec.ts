import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PurgeWorkersComponent } from './purge-workers.component';

describe('PurgeWorkersComponent', () => {
  let component: PurgeWorkersComponent;
  let fixture: ComponentFixture<PurgeWorkersComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PurgeWorkersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PurgeWorkersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
