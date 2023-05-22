import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Candidate, Voter } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-pollingCandidates',
  templateUrl: './pollingCandidates.component.html',
  styleUrls: ['./pollingCandidates.component.css']
})
export class pollingCandidatesComponent {

  public candidates: Candidate[];
  public selectedCandidate: Candidate;
  private voter: Voter;
  displayedColumns = ['Name', 'Ballot', 'Organisation', 'select'];


  constructor(private http: HttpClient, private dataservice: DataService, private router: Router, public snackBar: MatSnackBar) {
    this.voter = this.dataservice.voter;
    this.http.post<any>(environment.base_url + 'Candidate/CandidatesOfBallots', this.voter).subscribe(result => {
      this.candidates = result as Candidate[];
      console.info(this.candidates);
    }, error => this.openSnackBar(error.error.message, "Error"));

  }

  onBack(): void {
    this.dataservice.voter = this.voter;
    this.router.navigate(['pollingBallots']);
  }

  onVote(): void {

    if (this.selectedCandidate == null) {
      this.openSnackBar("Please select candidate", "Info");
      return;
    }

    console.info(this.selectedCandidate);
    this.http.post<any>(environment.base_url + 'Candidate/CastVote', this.selectedCandidate).subscribe(result => {     
      console.info(result);
      this.openSnackBar(result.message, "Info")
      this.onBack();
    }, error => this.openSnackBar(error.error.message, "Error"));
    

  }


  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }

}
