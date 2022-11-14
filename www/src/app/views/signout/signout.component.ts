import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signout',
  templateUrl: './signout.component.html',
  styleUrls: ['./signout.component.scss']
})
export class SignoutComponent {

  constructor(private authService: AuthService) {
    this.signOut();
  }

//Not working in Dev mode due to https and missing domain (Works fine live)
  public signOut() {
    this.authService.signOut().subscribe();
  }



}
