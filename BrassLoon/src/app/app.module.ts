import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { AuthModule, LogLevel, OidcSecurityService, StsConfigHttpLoader, StsConfigLoader } from 'angular-auth-oidc-client';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AutoLoginComponent } from './auto-login/auto-login.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { AppSettingsService } from './app-settings.service';
import { AccountsComponent } from './accounts/accounts.component';
import { AccountComponent } from './account/account.component';
import { ClientComponent } from './client/client.component';
import { ExceptionsComponent } from './exceptions/exceptions.component';
import { ExceptionComponent } from './exception/exception.component';
import { ExceptionDataComponent } from './exception-data/exception-data.component';
import { DomainComponent } from './domain/domain.component';
import { DomainNavComponent } from './domain-nav/domain-nav.component';
import { TracesComponent } from './traces/traces.component';
import { MetricsComponent } from './metrics/metrics.component';
import { SysAdminComponent } from './sys-admin/sys-admin.component';
import { PurgeWorkersComponent } from './purge-workers/purge-workers.component';
import { ActAdminComponent } from './act-admin/act-admin.component';
import { AccountSearchComponent } from './account-search/account-search.component';
import { CreateInvitationComponent } from './create-invitation/create-invitation.component';
import { UserInvitationComponent } from './user-invitation/user-invitation.component';
import { AcceptInvitationComponent } from './accept-invitation/accept-invitation.component';
import { LookupCodesComponent } from './lookup-codes/lookup-codes.component';
import { ItemCodesComponent } from './item-codes/item-codes.component';
import { UserSearchComponent } from './user-search/user-search.component';
import { UserComponent } from './user/user.component';

export const httpLoaderFactory = (appSettingsService: AppSettingsService) => {  
  const settings$: any = appSettingsService.LoadSettings()
  .then((settings) => 
  {    
    return {
      authority: settings.GoogleStsServer,     
      authWellknownEndpointUrl: settings.AuthWellknownEndpointUrl,
      redirectUrl: window.location.origin + settings.BaseHref + "/",
      clientId: settings.GoogleClientId,
      responseType: 'id_token token',
      scope: 'openid email profile',
      triggerAuthorizationResultEvent: true,
      postLogoutRedirectUri: window.location.origin + '/unauthorized',
      startCheckSession: false,
      silentRenew: false,
      silentRenewUrl: window.location.origin + '/silent-renew.html',
      postLoginRoute: '/home',
      forbiddenRoute: '/forbidden',
      unauthorizedRoute: '/unauthorized',
      logLevel: LogLevel.Debug,
      historyCleanupOff: true,
      autoUserinfo: false
      // iss_validation_off: false
      // disable_iat_offset_validation: true
    }
  });

  return new StsConfigHttpLoader(settings$);
};

@NgModule({
  declarations: [
    AppComponent,
    AutoLoginComponent,
    HomeComponent,
    UnauthorizedComponent,
    ForbiddenComponent,
    AccountsComponent,
    AccountComponent,
    ClientComponent,
    ExceptionsComponent,
    ExceptionComponent,
    ExceptionDataComponent,
    DomainComponent,
    DomainNavComponent,
    TracesComponent,
    MetricsComponent,
    SysAdminComponent,
    PurgeWorkersComponent,
    ActAdminComponent,
    AccountSearchComponent,
    CreateInvitationComponent,
    UserInvitationComponent,
    AcceptInvitationComponent,
    LookupCodesComponent,
    ItemCodesComponent,
    UserSearchComponent,
    UserComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    FontAwesomeModule,
    AppRoutingModule,
    HttpClientModule,
    AuthModule.forRoot({
      loader: {
        provide: StsConfigLoader,
        useFactory: httpLoaderFactory,
        deps: [AppSettingsService],
      }
    })
  ],
  providers: [    
    AppSettingsService,
    OidcSecurityService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
