import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExceptionDataComponent } from './exception-data.component';

describe('ExceptionDataComponent', () => {
  let component: ExceptionDataComponent;
  let fixture: ComponentFixture<ExceptionDataComponent>;

  beforeEach(async(() => {
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
