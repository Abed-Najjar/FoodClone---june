import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RestaurantListComponent } from './components/restaurant-list/restaurant-list.component';
import { RestaurantDetailComponent } from './components/restaurant-detail/restaurant-detail.component';
import { CartComponent } from './components/cart/cart.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { CmsDashboardComponent } from './components/cms/dashboard/cms-dashboard.component';
import { CategoriesComponent } from './components/cms/categories/categories.component';
import { RestaurantsComponent } from './components/cms/restaurants/restaurants.component';
import { DishesComponent } from './components/cms/dishes/dishes.component';
import { UsersComponent } from './components/cms/users/users.component';
import { OrdersComponent } from './components/cms/orders/orders.component';
import { authGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', component: HomeComponent, canActivate: [authGuard] },
  { path: 'restaurants', component: RestaurantListComponent, canActivate: [authGuard] },
  { path: 'restaurants/:id', component: RestaurantDetailComponent, canActivate: [authGuard] },
  { path: 'cart', component: CartComponent, canActivate: [authGuard] },

  // Admin routes
  { path: 'admin/login', component: AdminLoginComponent },
  {
    path: 'admin',
    component: CmsDashboardComponent,
    canActivate: [AdminGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: CmsDashboardComponent },
      { path: 'categories', component: CategoriesComponent },
      { path: 'restaurants', component: RestaurantsComponent },
      { path: 'dishes', component: DishesComponent },
      { path: 'users', component: UsersComponent },
      { path: 'orders', component: OrdersComponent }
    ]
  },

  { path: '**', redirectTo: 'login' } // Redirect to login for any unknown routes
];
