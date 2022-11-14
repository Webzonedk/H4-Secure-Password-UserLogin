import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../models/user';
import { UserClaim } from '../interfaces/user-claim';
import { BehaviorSubject, catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  apiUrl = 'https://localhost:7232/Auth/';
  $responseMessage: string = '';
  $testResponse:BehaviorSubject<Boolean> = new BehaviorSubject<Boolean>(false);
  
  
  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
    withCredentials: true, 
    observe: 'response' as 'response'
  };  


  constructor(private http: HttpClient) { }



  signIn(userName: string, password: string) {
    return this.http.post<Response>(this.apiUrl + 'Login', {
      userName: userName,
      password: password
    },{withCredentials: true});
  }
  

//Not working in Dev mode due to https and missing domain (Works fine on live server) Can not delete the Authentication cookie
  signOut() {
    return this.http.get<Response>(this.apiUrl + 'Logout');
  }



  user() {
    return this.http.get<UserClaim[]>(this.apiUrl + 'User',{withCredentials: true});
  }



  isSignedIn(): Observable<boolean> {
    let result = this.user();
    return result.pipe(
      map((userClaims) => {
             // console.log("hasClainms: ",userClaims); //DEBUG
              const hasClaims = userClaims.length > 0;
              return !hasClaims ? false : true;
      }),
      catchError((error) => {
        //console.log("Error from AuthService: ",error); //DEBUG
        return of(false);
      }));
  }
}
