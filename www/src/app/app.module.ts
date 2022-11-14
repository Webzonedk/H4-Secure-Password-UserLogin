import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthInterceptorService } from './services/auth-interceptor.service';
import { AuthGuard } from './shared/auth.guard';
import { LoginComponent } from './views/login/login.component';
import { SecretContentComponent } from './views/secret-content/secret-content.component';
import { UserComponent } from './views/user/user.component';
import { SignoutComponent } from './views/signout/signout.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    SecretContentComponent,
    UserComponent,
    SignoutComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
  ],
  providers: [AuthGuard,{ provide: HTTP_INTERCEPTORS, useClass: AuthInterceptorService, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule { }
