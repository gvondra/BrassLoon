import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ExceptionDataComponent } from './exception-data.component';

describe('ExceptionDataComponent', () => {
  let component: ExceptionDataComponent;
  let fixture: ComponentFixture<ExceptionDataComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ExceptionDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExceptionDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
