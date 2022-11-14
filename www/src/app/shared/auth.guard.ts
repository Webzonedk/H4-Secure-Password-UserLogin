import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {

  }
  
  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.isSignedIn();
}
  
  


  isSignedIn(): Observable<boolean> {
    return this.authService.isSignedIn().pipe(
      map((isSignedIn) => {
        console.log("AuthGuard says isSignedIn: ", isSignedIn); //DEBUG
        if (!isSignedIn) {
          this.router.navigate(['login']);
          return false;
        }
        return true;
      })
    );
  }
}
