import { Component, ChangeDetectorRef, OnDestroy, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Organisation } from '../../models/vmModels';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-frmOrganisation',
  templateUrl: './frmOrganisation.component.html',
  styleUrls: ['./frmOrganisation.component.css']
})
export class frmOrganisationComponent {

  public organisation: Organisation;

  constructor(private http: HttpClient, private router: Router) {
    this.organisation = new Organisation();
  }


  onSubmit() {

    console.info(this.organisation);
    this.http.post<any>(environment.base_url + 'Organisation', this.organisation).subscribe(result => {
      this.router.navigate(['organisastion']);
    }, error => console.error(error));



  }


}
