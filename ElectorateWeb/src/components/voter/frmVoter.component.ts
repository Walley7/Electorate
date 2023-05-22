import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Organisation, Ballot, Candidate, Voter } from '../../models/vmModels';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-frmVoter',
  templateUrl: './frmVoter.component.html',
  styleUrls: ['./frmVoter.component.css']
})
export class frmVoterComponent {

  public organisation: Organisation;
  public voter: Voter;
  public organisations: Organisation[];  
  public organisationControl = new FormControl('', [Validators.required]);  
  public nameControl = new FormControl('', [Validators.required]);

  constructor(private http: HttpClient, private router: Router, public snackBar: MatSnackBar) {
    this.voter = new Voter();
    this.http.get(environment.base_url + 'Organisation').subscribe(result => {
      this.organisations = result as Organisation[];
    }, error => console.error(error));

  }


  onSubmit() {

    if ((this.organisationControl.hasError('required') ||  this.nameControl.hasError('required'))) {
      this.openSnackBar("Please resolve input error", "Invalid inputs");
    } else {
      this.voter.privateKey = this.organisation.privateKey;
      this.voter.publicKey = this.organisation.publicKey;
     
      
      this.http.post<any>(environment.base_url + 'Voter', this.voter).subscribe(result => {
        console.log("Data saved successfully");
        this.openSnackBar("Data saved successfully", "Success");
        this.router.navigate(['voter']);
      }, error =>
          this.openSnackBar(error.error.message, "Error")
      );

    }
  }

 

  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }



}
