import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from '../categories/categories.component';
import { RestaurantsComponent } from '../restaurants/restaurants.component';
import { DishesComponent } from '../dishes/dishes.component';
import { UsersComponent } from '../users/users.component';
import { OrdersComponent } from '../orders/orders.component';
import { CmsNavigationService } from '../../../services/cms-navigation.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cms-dashboard',
  standalone: true,
  imports: [CommonModule, CategoriesComponent, RestaurantsComponent, DishesComponent, UsersComponent, OrdersComponent],
  templateUrl: './cms-dashboard.component.html',
  styleUrls: ['./cms-dashboard.component.css']
})
export class CmsDashboardComponent implements OnInit, OnDestroy {
  selectedTab: string = 'categories';
  private tabSubscription!: Subscription;

  constructor(private navigationService: CmsNavigationService) {}

  ngOnInit() {
    // Subscribe to tab changes from the service
    this.tabSubscription = this.navigationService.currentTab.subscribe(tab => {
      this.selectedTab = tab;
    });
  }

  ngOnDestroy() {
    // Clean up subscription when component is destroyed
    if (this.tabSubscription) {
      this.tabSubscription.unsubscribe();
    }
  }

  selectTab(tab: string) {
    this.navigationService.changeTab(tab);
  }
}
