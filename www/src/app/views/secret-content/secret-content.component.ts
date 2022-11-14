import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-secret-content',
  templateUrl: './secret-content.component.html',
  styleUrls: ['./secret-content.component.scss']
})
export class SecretContentComponent implements OnInit {

  constructor(private router: Router, private authService: AuthService) { }

  ngOnInit(): void {
  }

  //Not working atm
  public signOut() {
    this.authService.signOut().subscribe();
    this.router.navigate(['login']);
  }

}
