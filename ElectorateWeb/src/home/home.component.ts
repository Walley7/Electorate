import { Component, ChangeDetectorRef, OnDestroy, OnInit } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';
import { MediaMatcher } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { filter, map, take, mergeMap } from 'rxjs/operators';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnDestroy, OnInit {

  mobileQuery: MediaQueryList;
  pageTitle: string;

  constructor(changeDetectorRef: ChangeDetectorRef, media: MediaMatcher, private router: Router, public dialog: MatDialog, private activatedRoute: ActivatedRoute ) {

    this.mobileQuery = media.matchMedia('(max-width: 600px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  private _mobileQueryListener: () => void;

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map((route) => {
        while (route.firstChild) route = route.firstChild;
        return route;
      }),

      filter((route) => route.outlet === 'primary'),
      mergeMap((route) => route.data)
    ).subscribe((event) => {

      this.pageTitle = event['title'] ? '{{ ' + event['title']  + ' }}' : "";
      console.log( event['title']);
      });

      /*.map(() => this.activatedRoute)
      .map((route) => {
        while (route.firstChild) route = route.firstChild;
        return route;
      })
      .filter((route) => route.outlet === 'primary')
      .mergeMap((route) => route.data)
      .subscribe((event) => this.titleService.setTitle(event['title']));*/
  }


  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }

  logOff(): void {
   // this.token.signOut();
   // this.router.navigate(['login']);
  }

}
