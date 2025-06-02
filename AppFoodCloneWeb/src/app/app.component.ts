import { Component, OnInit, HostListener } from '@angular/core';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Title, Meta } from '@angular/platform-browser';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, CommonModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Careem Food - Order Food Online';
  showBackToTop = false;
  hideHeaderFooter = false;

  constructor(
    private titleService: Title,
    private metaService: Meta,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Set page title and meta tags
    this.titleService.setTitle(this.title);
    this.metaService.addTag({ name: 'description', content: 'Order food online from your favorite restaurants with fast delivery' });
    this.metaService.addTag({ name: 'keywords', content: 'food delivery, restaurants, online ordering, meals' });

    // Check current route immediately
    this.checkCurrentRoute();

    // Listen to route changes to hide header/footer on auth pages
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.checkRouteForHeaderVisibility(event.url);
      });
  }

  private checkCurrentRoute(): void {
    this.checkRouteForHeaderVisibility(this.router.url);
  }

  private checkRouteForHeaderVisibility(url: string): void {
    const authRoutes = ['/login', '/register', '/forgot-password', '/admin-login'];
    this.hideHeaderFooter = authRoutes.some(route => url.startsWith(route));
    console.log('Current URL:', url, 'Hide header:', this.hideHeaderFooter); // Debug log
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.showBackToTop = window.scrollY > 300;
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
