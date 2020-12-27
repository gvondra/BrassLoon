import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AutoLoginComponent } from './auto-login/auto-login.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { AccountComponent } from './account/account.component';
import { ClientComponent } from './client/client.component';
import { ExceptionsComponent } from './exceptions/exceptions.component';
import { ExceptionComponent } from './exception/exception.component';
import { DomainComponent } from './domain/domain.component';
import { TracesComponent } from './traces/traces.component';
import { MetricsComponent } from './metrics/metrics.component';
import { PurgeWorkersComponent } from './purge-workers/purge-workers.component';
import { AuthGuard } from './auth.guard';
import { AccountSearchComponent } from './account-search/account-search.component';

const routes: Routes = [
  {
    path: "autologin",
    component: AutoLoginComponent
  },
  {
    path: "",
    component: HomeComponent
  },
  {
    path: "home",
    component: HomeComponent
  },
  {
    path: "unauthorized",
    component: UnauthorizedComponent
  },
  {
    path: "forbidden",
    component: ForbiddenComponent
  },
  {
    path: "a",
    component: AccountComponent
  },
  {
    path: "a/:id",
    component: AccountComponent
  },
  {
    path: "a/:accountId/Client",
    component: ClientComponent
  },
  {
    path: "a/:accountId/Client/:clientId",
    component: ClientComponent
  },
  {
    path: "d/:id",
    component: DomainComponent
  },
  {
    path: "d/:domainId/Exception",
    component: ExceptionsComponent
  },
  {
    path: "d/:domainId/Exception/:id",
    component: ExceptionComponent
  },
  {
    path: "d/:domainId/Trace",
    component: TracesComponent
  },
  {
    path: "d/:domainId/Metric",
    component: MetricsComponent
  },
  {
    path: 'sa/PurgeWorker',
    component: PurgeWorkersComponent,
    data: { "Role": "sysadmin" },
    canActivate: [ AuthGuard ]
  },
  {
    path: 'aa',
    component: AccountSearchComponent,
    data: { "Role": "actadmin" },
    canActivate: [ AuthGuard ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
