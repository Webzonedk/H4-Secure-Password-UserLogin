import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { Response } from 'src/app/interfaces/response';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  responseMessage: string = '';
  responseData: any;
  loginForm!: FormGroup;
  authFailed: boolean = false;
  signedIn: boolean = false;

  constructor(private authService: AuthService,
    private formBuilder: FormBuilder,
    private router: Router) {
    this.authService.isSignedIn().subscribe(
      isSignedIn => {
        this.signedIn = isSignedIn;
      });
  }


  ngOnInit(): void {
    this.authFailed = false;
    this.loginForm = this.formBuilder.group(
      {
        userName: ['', [Validators.required]],
        password: ['', [Validators.required]]
      });
  }


  //Function to signin and navigate to guarded area
  public signIn(event: any) {
    if (!this.loginForm.valid) {
      return;
    }
    const userName = this.loginForm.get('userName')?.value;
    const password = this.loginForm.get('password')?.value;

    this.authService.signIn(userName, password).subscribe({
      next: (response:any) => {
        //console.log("response: ", response["isSuccess"]) //DEBUG
        //let success =response["isSuccess"]; //DEBUG
        //console.log(response["isSuccess"]); //DEBUG
        if (response["isSuccess"]) {
          //console.log("Vi kommer her ind"); //DEBUG
          // this.authService.sendToken(); //Sending the token from the cookie to API via authservice
          this.router.navigate(['secret-content']);
        }
      },
      error: (error) => {
        //console.log("Error: ", error); //DEBUG
        if (!error?.error?.isSuccess) {
          this.authFailed = true;
        }
      }
    });
  }
//Added to push to git
//Added to push to git

}
