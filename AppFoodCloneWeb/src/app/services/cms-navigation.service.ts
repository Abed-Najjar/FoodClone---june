import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CmsNavigationService {
  private tabSource = new BehaviorSubject<string>('categories');
  currentTab = this.tabSource.asObservable();

  constructor() {}

  changeTab(tabName: string) {
    this.tabSource.next(tabName);
  }
}
