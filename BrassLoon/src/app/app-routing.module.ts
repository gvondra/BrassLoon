import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AutoLoginComponent } from './auto-login/auto-login.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';


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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
