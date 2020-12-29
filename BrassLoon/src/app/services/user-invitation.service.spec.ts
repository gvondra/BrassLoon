import { TestBed } from '@angular/core/testing';

import { UserInvitationService } from './user-invitation.service';

describe('UserInvitationService', () => {
  let service: UserInvitationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserInvitationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
