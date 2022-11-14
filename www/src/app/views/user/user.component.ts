import { Component, OnInit } from '@angular/core';
import { UserClaim } from 'src/app/interfaces/user-claim';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})

export class UserComponent {
  userClaims: UserClaim[] = []


  constructor(private authService: AuthService) {
    this.getUser();
  }

// Component to get userclaims
  getUser() {
    this.authService.user().subscribe(result => {
      this.userClaims = result;
      //console.log(this.userClaims);//DEBUG
    });
  }

}
