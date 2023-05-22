import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS  } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { CustomMaterialModule } from '../core/material.module';
import { HomeComponent } from '../home/home.component';
import { RouterModule, Routes } from '@angular/router';
import { AppRoutingModule } from '../core/app.routing.module';
import { organisationComponent } from '../components/organisation/organisation.component';
import { frmOrganisationComponent } from '../components/organisation/frmOrganisation.component';
import { frmBallotComponent } from '../components/ballot/frmBallot.component';
import { candidateComponent } from '../components/candidate/candidate.component';
import { frmCandidateComponent } from '../components/candidate/frmCandidate.component';
import { LoaderInterceptorService } from '../_services/loader-interceptor.service';
import { LoaderComponent } from '../loader/loader.component';
import { frmVoterComponent } from '../components/voter/frmVoter.component';
import { voterComponent } from '../components/voter/voter.component';
import { ballotComponent } from '../components/ballot/ballot.component';
import { allocateVoterComponent } from '../components/voter/allocateVoter.component';
import { registerVoterComponent } from '../components/voter/registerVoter.component';
import { pollingVotersComponent } from '../components/pollingVoters/pollingVoters.component';
import { pollingBallotsComponent } from '../components/ballot/pollingBallots.component';
import { pollingCandidatesComponent } from '../components/candidate/pollingCandidates.component';
import { resultComponent } from '../components/pollingResult/result.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    organisationComponent,
    frmOrganisationComponent,
    frmBallotComponent,
    candidateComponent,
    frmCandidateComponent,
    LoaderComponent,
    frmVoterComponent,
    voterComponent,
    ballotComponent,
    allocateVoterComponent,
    registerVoterComponent,
    pollingVotersComponent,
    pollingBallotsComponent,
    pollingCandidatesComponent,
    resultComponent
  ],
  imports: [
    BrowserModule,
    CustomMaterialModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoaderInterceptorService,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
