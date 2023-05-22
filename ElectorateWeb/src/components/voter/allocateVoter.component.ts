import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Voter } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-allocateVoter',
  templateUrl: './allocateVoter.component.html',
  styleUrls: ['./allocateVoter.component.css']
})
export class allocateVoterComponent {

  public voters: Voter[];
  displayedColumns = ['Name', 'Organisation','select'];

  constructor(private http: HttpClient, private dataservice: DataService, public snackBar: MatSnackBar) {
    console.info('allocateVoterComponent.constructor');
    let value = this.dataservice.ballot;
    console.info(value);
    this.http.post<any>(environment.base_url + 'Voter/AllocatedVoters', value).subscribe(result => {
      this.voters = result as Voter[];
      console.info(this.voters);
    }, error => console.error(error));
  }

  onSave(): void {
    console.info(this.voters);
    this.http.post<any>(environment.base_url + 'Voter/AllocatVoters', this.voters).subscribe(result => {
      this.voters = result as Voter[];
      console.info(this.voters);
      this.openSnackBar("Successfully saved data", "Info");
    }, error => this.openSnackBar(error.error.message, "Error"));
  }

  openSnackBar(message: string, action: string): void {
    this.snackBar.open(message, action, {
      duration: 5000,
    });
  }


}
