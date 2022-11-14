import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/auth.guard';
import { LoginComponent } from './views/login/login.component';
import { SecretContentComponent } from './views/secret-content/secret-content.component';
import { SignoutComponent } from './views/signout/signout.component';
import { UserComponent } from './views/user/user.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'user',
    component: UserComponent
  },
  {
    path: 'logout',
    component: SignoutComponent
  },
  {
    path: 'secret-content',
    component: SecretContentComponent,
    canActivate: [AuthGuard]
  },
  {
    path: '**', redirectTo: ''
  }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
