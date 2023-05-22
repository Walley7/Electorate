import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Voter } from '../../models/vmModels';


@Component({
  selector: 'app-voter',
  templateUrl: './voter.component.html',
  styleUrls: ['./voter.component.css']
})
export class voterComponent {

  public voters: Voter[];
  displayedColumns = ['Name', 'Organisation'];


  constructor(private http: HttpClient) {
    
    http.get(environment.base_url + 'Voter').subscribe(result => {
      this.voters = result as Voter[];      
    }, error => console.error(error));
    
  }

}
