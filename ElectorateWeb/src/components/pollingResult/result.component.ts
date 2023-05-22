import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Ballot, Candidate } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-result',
  templateUrl: './result.component.html',
  styleUrls: ['./result.component.css']
})
export class resultComponent {

  public ballots: Ballot[];
  panelOpenState = false;

  displayedColumns = ['Name', 'Result'];
  
  constructor(private http: HttpClient, private dataservice: DataService, private router: Router, public snackBar: MatSnackBar) {

    http.get(environment.base_url + 'Ballot/Result').subscribe(result => {
      this.ballots = result as Ballot[];
      console.info(this.ballots);
    }, error => this.openSnackBar(error.error.message, "Error"));
  }
  
  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }


}
