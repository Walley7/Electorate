import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Ballot } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-ballot',
  templateUrl: './ballot.component.html',
  styleUrls: ['./ballot.component.css']
})
export class ballotComponent {

  public ballots: Ballot[];
  displayedColumns = ['Address', 'Name', 'Organisation', 'symbol'];


  constructor(private http: HttpClient, private dataservice: DataService, private router: Router, public snackBar: MatSnackBar) {

    http.get(environment.base_url + 'Ballot').subscribe(result => {
      this.ballots = result as Ballot[];
    }, error => console.error(error));
  }

  onAllocateVoter(ballot: Ballot): void {
    console.info(ballot);
    this.dataservice.ballot = ballot;
    this.router.navigate(['allocateVoter']);
  }

  onRegisterVoter(ballot: Ballot): void {
    console.info(ballot);
    this.dataservice.ballot = ballot;
    this.router.navigate(['registerVoter']);
  }


  onLockBallot(ballot: Ballot): void {
    console.info(ballot);

    this.http.post<any>(environment.base_url + 'Ballot/LockBallot', ballot).subscribe(result => {    
      console.info(ballot);
      this.openSnackBar("Successfully saved data", "Info");
    }, error => this.openSnackBar(error.error.message, "Error"));
        
  }


  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }


}
