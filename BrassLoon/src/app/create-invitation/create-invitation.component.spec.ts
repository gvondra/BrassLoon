import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreateInvitationComponent } from './create-invitation.component';

describe('CreateInvitationComponent', () => {
  let component: CreateInvitationComponent;
  let fixture: ComponentFixture<CreateInvitationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateInvitationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateInvitationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
