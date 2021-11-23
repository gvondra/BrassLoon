import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ActAdminComponent } from './act-admin.component';

describe('ActAdminComponent', () => {
  let component: ActAdminComponent;
  let fixture: ComponentFixture<ActAdminComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ActAdminComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ActAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
