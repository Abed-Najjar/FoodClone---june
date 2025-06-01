import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { RestaurantListComponent } from './components/restaurant-list/restaurant-list.component';
import { RestaurantDetailComponent } from './components/restaurant-detail/restaurant-detail.component';
import { CartComponent } from './components/cart/cart.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { OtpVerificationComponent } from './components/otp-verification/otp-verification.component';
import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { CmsDashboardComponent } from './components/cms/dashboard/cms-dashboard.component';
import { CategoriesComponent } from './components/cms/categories/categories.component';
import { RestaurantsComponent } from './components/cms/restaurants/restaurants.component';
import { DishesComponent } from './components/cms/dishes/dishes.component';
import { UsersComponent } from './components/cms/users/users.component';
import { OrdersComponent } from './components/cms/orders/orders.component';
import { AddressListComponent } from './components/address-management/address-list.component';
import { AddressFormComponent } from './components/address-management/address-form.component';
import { OrderTrackingComponent } from './components/order-tracking/order-tracking.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';
import { authGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'verify-otp', component: OtpVerificationComponent },
  { path: '', component: HomeComponent, canActivate: [authGuard] },
  { path: 'restaurants', component: RestaurantListComponent, canActivate: [authGuard] },
  { path: 'restaurants/:id', component: RestaurantDetailComponent, canActivate: [authGuard] },
  { path: 'cart', component: CartComponent, canActivate: [authGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [authGuard] },
  { path: 'track-orders', component: OrderTrackingComponent, canActivate: [authGuard] },
  { path: 'addresses', component: AddressListComponent, canActivate: [authGuard] },
  { path: 'addresses/new', component: AddressFormComponent, canActivate: [authGuard] },
  { path: 'addresses/edit/:id', component: AddressFormComponent, canActivate: [authGuard] },

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
