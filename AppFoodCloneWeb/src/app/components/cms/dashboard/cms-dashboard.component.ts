import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoriesComponent } from '../categories/categories.component';
import { RestaurantsComponent } from '../restaurants/restaurants.component';
import { DishesComponent } from '../dishes/dishes.component';
import { UsersComponent } from '../users/users.component';
import { OrdersComponent } from '../orders/orders.component';

@Component({
  selector: 'app-cms-dashboard',
  standalone: true,
  imports: [CommonModule, CategoriesComponent, RestaurantsComponent, DishesComponent, UsersComponent, OrdersComponent],
  templateUrl: './cms-dashboard.component.html',
  styleUrls: ['./cms-dashboard.component.css']
})
export class CmsDashboardComponent {
  selectedTab: string = 'categories';

  selectTab(tab: string) {
    this.selectedTab = tab;
  }
}
