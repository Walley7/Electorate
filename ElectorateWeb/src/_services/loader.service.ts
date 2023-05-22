import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { LoaderState } from '../loader/loader.model';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  private loaderSubject = new Subject<LoaderState>();

  private headerSubject = new Subject<string>();

  loaderState = this.loaderSubject.asObservable();

  headerName = this.headerSubject.asObservable();


  constructor() { }

  show() {
    this.loaderSubject.next(<LoaderState>{ show: true });
  }

  hide() {
    this.loaderSubject.next(<LoaderState>{ show: false });
  }

  header(value: string) {
    this.headerSubject.next(value);
  }
  
}

