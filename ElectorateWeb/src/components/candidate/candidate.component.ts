import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Candidate } from '../../models/vmModels';

@Component({
  selector: 'app-candidate',
  templateUrl: './candidate.component.html',
  styleUrls: ['./candidate.component.css']
})
export class candidateComponent {

  public candidates: Candidate[];
  displayedColumns = ['Name', 'Ballot', 'Organisation'];


  constructor(private http: HttpClient) {
    
    http.get(environment.base_url + 'Candidate').subscribe(result => {
      this.candidates = result as Candidate[];      
    }, error => console.error(error));
    
  }

}
