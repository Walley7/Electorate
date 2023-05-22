import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Ballot, Voter } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-pollingBallots',
  templateUrl: './pollingBallots.component.html',
  styleUrls: ['./pollingBallots.component.css']
})
export class pollingBallotsComponent {

  public ballots: Ballot[];
  private voter: Voter;
  displayedColumns = ['Address', 'Name', 'Organisation', 'symbol'];


  constructor(private http: HttpClient, private dataservice: DataService, private router: Router, public snackBar: MatSnackBar) {

    this.voter = this.dataservice.voter;
    console.info(this.voter);
    this.http.post<any>(environment.base_url + 'Ballot/BallotsOfVoter', this.voter).subscribe(result => {
      this.ballots = result as Ballot[];
      console.info(this.ballots);
    }, error => console.error(error));

  }
  

  onCastVote(ballot: Ballot): void {
    console.info(ballot);    
    this.voter.ballotAddresskey = ballot.address;
    this.dataservice.voter = this.voter;
    this.router.navigate(['pollingCandidates']);
  }

  


  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }


}
