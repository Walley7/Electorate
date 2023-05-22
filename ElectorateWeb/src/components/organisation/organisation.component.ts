import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Organisation } from '../../models/vmModels';



@Component({
  selector: 'app-organisation',
  templateUrl: './organisation.component.html',
  styleUrls: ['./organisation.component.css']
})
export class organisationComponent {

  public Organisations: Organisation[];
  displayedColumns = ['Name', 'RegistrationNo', 'Address'];


  constructor(private http: HttpClient) {
    
    http.get(environment.base_url + 'Organisation').subscribe(result => {
      this.Organisations = result as Organisation[];      
    }, error => console.error(error));
    
  }

}
