import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { AuthModule, LogLevel, OidcConfigService, OidcSecurityService } from 'angular-auth-oidc-client';

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

const InitializeConfig = (oidcConfigService: OidcConfigService, appSettingsService: AppSettingsService) => {
  return () => {
      return appSettingsService.LoadSettings()
      .then(() =>       
      oidcConfigService.withConfig({
          stsServer: appSettingsService.GetSettings().GoogleStsServer,
          redirectUrl: window.location.origin,
          clientId: appSettingsService.GetSettings().GoogleClientId,
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
      })
      );
  }
}

@NgModule({
  declarations: [
    AppComponent,
    AutoLoginComponent,
    HomeComponent,
    UnauthorizedComponent,
    ForbiddenComponent,
    AccountsComponent,
    AccountComponent,
    ClientComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    FontAwesomeModule,
    AppRoutingModule,
    HttpClientModule,
    AuthModule.forRoot()
  ],
  providers: [    
    AppSettingsService,
    OidcSecurityService,
    OidcConfigService,
    {
        provide: APP_INITIALIZER,
        useFactory: InitializeConfig,
        deps: [OidcConfigService, AppSettingsService],
        multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
