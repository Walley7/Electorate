import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Organisation, Ballot, Candidate } from '../../models/vmModels';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-frmCandidate',
  templateUrl: './frmCandidate.component.html',
  styleUrls: ['./frmCandidate.component.css']
})
export class frmCandidateComponent {

  public organisation: Organisation;
  public ballot: Ballot;
  public candidate: Candidate;
  public organisations: Organisation[];
  public ballots: Ballot[];
  public organisationControl = new FormControl('', [Validators.required]);
  public ballotControl = new FormControl('', [Validators.required]);
  public nameControl = new FormControl('', [Validators.required]);

  constructor(private http: HttpClient, private router: Router, public snackBar: MatSnackBar) {
    this.candidate = new Candidate();
    this.http.get(environment.base_url + 'Organisation/OrganisationsWithBallot').subscribe(result => {
      this.organisations = result as Organisation[];
    }, error => console.error(error));

    this.organisationControl.valueChanges.subscribe(value => {
      console.log(value);
      if (value != null) {
        this.GetBallots(value)
      }
      else {
        this.ballots = null;
      }
    });
  }


  onSubmit() {

    if ((this.organisationControl.hasError('required') || this.ballotControl.hasError('required') || this.nameControl.hasError('required'))) {
      this.openSnackBar("Please resolve input error", "Invalid inputs");
    } else {
      this.candidate.privateKey = this.organisation.privateKey;
      this.candidate.publicKey = this.organisation.publicKey;
      this.candidate.ballotAddresskey = this.ballot.address;
      
      this.http.post<any>(environment.base_url + 'Candidate', this.candidate).subscribe(result => {
        console.log("Data saved successfully");
        this.openSnackBar("Data saved successfully", "Success");
        this.router.navigate(['candidate']);
      }, error =>
          this.openSnackBar(error.error.message, "Error")
      );




    }


    
  }

  GetBallots(selectedOrganisation: Organisation): void {

    this.http.post<any>(environment.base_url + 'Ballot/BallotsOfOrganisation', selectedOrganisation).subscribe(result => {
      console.log(result);
      this.ballots = result as Ballot[];
    }, error =>
        this.openSnackBar(error.error.message, "Error")
    );
  }

  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }



}
