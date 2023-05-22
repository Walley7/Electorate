import { Component, ChangeDetectorRef, OnDestroy, Injectable, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Organisation, Ballot } from '../../models/vmModels';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-frmBallot',
  templateUrl: './frmBallot.component.html',
  styleUrls: ['./frmBallot.component.css']
})
export class frmBallotComponent implements OnInit {

  public organisations: Organisation[];
  public organisation: Organisation;
  public filteredOptions: Observable<Organisation[]>;
  public myControl = new FormControl();

  public ballot: Ballot
   
  constructor(private http: HttpClient, private router: Router, public snackBar: MatSnackBar) {
    this.ballot = new Ballot();
  }
  
  ngOnInit() {

    this.http.get(environment.base_url + 'Organisation').subscribe(result => {
      this.organisations = result as Organisation[];

      this.filteredOptions = this.myControl.valueChanges
        .pipe(
          startWith<string | Organisation>(''),
          map(value => typeof value === 'string' ? value : value.name),
          map(name => name ? this._filter(name) : this.organisations.slice())
        );
    }, error => this.openSnackBar(error.error.message, "Error"));

    
  }

  onSubmit() {
    console.info(this.ballot);
    console.info(this.organisation);
    if (this.organisation != null) {

      this.ballot.privateKey = this.organisation.privateKey;
      this.ballot.publicKey = this.organisation.publicKey;

    }

    this.http.post<any>(environment.base_url + 'Ballot', this.ballot).subscribe(result => {
      console.log("Data saved successfully");
      this.router.navigate(['ballot']);
    }, error =>      
        this.openSnackBar(error.error.message, "Error")
    );
    
  }


  displayFn(organisation?: Organisation): string | undefined {
    return organisation ? organisation.name : undefined;
  }


  private _filter(name: string): Organisation[] {

    const filterValue = name.toLowerCase();
    return this.organisations.filter(option => option.name.toLowerCase().indexOf(filterValue) === 0);
  }

  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }



}
