import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Ballot, Voter } from '../models/vmModels';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  
  ballot: Ballot;
  voter: Voter;

  constructor() { }  
}

