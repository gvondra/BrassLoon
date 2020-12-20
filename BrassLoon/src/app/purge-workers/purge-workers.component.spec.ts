import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PurgeWorkersComponent } from './purge-workers.component';

describe('PurgeWorkersComponent', () => {
  let component: PurgeWorkersComponent;
  let fixture: ComponentFixture<PurgeWorkersComponent>;

  beforeEach(async(() => {
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
