import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Voter } from '../../models/vmModels';
import { DataService } from '../../_services/data.service';

@Component({
  selector: 'app-pollingVoters',
  templateUrl: './pollingVoters.component.html',
  styleUrls: ['./pollingVoters.component.css']
})
export class pollingVotersComponent {

  public voters: Voter[];
  displayedColumns = ['Name', 'Organisation','link'];


  constructor(private http: HttpClient, private dataservice: DataService, private router: Router) {
    
    http.get(environment.base_url + 'Voter').subscribe(result => {
      this.voters = result as Voter[];      
    }, error => console.error(error));
    
  }


  onSelectBallot(voter: Voter): void {
    console.info(voter);
    this.dataservice.voter = voter;
    this.router.navigate(['pollingBallots']);
  }


}
