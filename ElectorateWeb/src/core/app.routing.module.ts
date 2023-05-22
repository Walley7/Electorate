import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { organisationComponent } from '../components/organisation/organisation.component';
import { frmOrganisationComponent } from '../components/organisation/frmOrganisation.component';
import { frmBallotComponent } from '../components/ballot/frmBallot.component';
import { ballotComponent } from '../components/ballot/ballot.component';
import { candidateComponent } from '../components/candidate/candidate.component';
import { frmCandidateComponent } from '../components/candidate/frmCandidate.component';
import { frmVoterComponent } from '../components/voter/frmVoter.component';
import { voterComponent } from '../components/voter/voter.component';
import { allocateVoterComponent } from '../components/voter/allocateVoter.component';
import { registerVoterComponent } from '../components/voter/registerVoter.component';
import { pollingVotersComponent } from '../components/pollingVoters/pollingVoters.component';
import { pollingBallotsComponent } from '../components/ballot/pollingBallots.component';
import { pollingCandidatesComponent } from '../components/candidate/pollingCandidates.component';
import { resultComponent } from '../components/pollingResult/result.component';

const routes: Routes = [
  { path: 'organisastion', component: organisationComponent, data: { title: 'Organisations' } },
  { path: 'frmOrganisation', component: frmOrganisationComponent, data: { title: 'Add Organisation' }  },
  { path: 'frmBallot', component: frmBallotComponent, data: { title: 'Add Ballot' } },
  { path: 'ballot', component: ballotComponent, data: { title: 'Ballots' }},
  { path: 'frmCandidate', component: frmCandidateComponent, data: { title: 'Add Candidate' }},
  { path: 'candidate', component: candidateComponent, data: { title: 'Candidates' } },
  { path: 'frmVoter', component: frmVoterComponent, data: { title: 'Add Voter' } },
  { path: 'voter', component: voterComponent, data: { title: 'Voters' } },
  { path: 'allocateVoter', component: allocateVoterComponent, data: { title: 'Allocate Voter' } },
  { path: 'registerVoter', component: registerVoterComponent, data: { title: 'Register Voter' }},
  { path: 'pollingVoters', component: pollingVotersComponent, data: { title: 'Polling Voters' }},
  { path: 'pollingBallots', component: pollingBallotsComponent, data: { title: 'Polling Ballots' } },
  { path: 'pollingCandidates', component: pollingCandidatesComponent, data: { title: 'Cast a vote' } },
  { path: 'result', component: resultComponent, data: { title: 'Polling result' } }

];


@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class AppRoutingModule { }

