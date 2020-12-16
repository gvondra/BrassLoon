import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DomainNavComponent } from './domain-nav.component';

describe('DomainNavComponent', () => {
  let component: DomainNavComponent;
  let fixture: ComponentFixture<DomainNavComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DomainNavComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DomainNavComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
