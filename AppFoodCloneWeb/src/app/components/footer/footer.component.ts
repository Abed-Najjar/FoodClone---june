import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {
  currentYear = new Date().getFullYear();

  // Social media links
  socialLinks = [
    { icon: 'bi-facebook', url: 'https://facebook.com', name: 'Facebook' },
    { icon: 'bi-twitter-x', url: 'https://twitter.com', name: 'Twitter' },
    { icon: 'bi-instagram', url: 'https://instagram.com', name: 'Instagram' },
    { icon: 'bi-youtube', url: 'https://youtube.com', name: 'YouTube' }
  ];

  // Footer menu links
  aboutLinks = [
    { text: 'About Us', url: '/about' },
    { text: 'Careers', url: '/careers' },
    { text: 'Blog', url: '/blog' },
    { text: 'Press', url: '/press' }
  ];

  supportLinks = [
    { text: 'Contact Us', url: '/contact' },
    { text: 'Help Center', url: '/help' },
    { text: 'Safety', url: '/safety' },
    { text: 'FAQs', url: '/faqs' }
  ];

  legalLinks = [
    { text: 'Terms & Conditions', url: '/terms' },
    { text: 'Privacy Policy', url: '/privacy' },
    { text: 'Cookies Policy', url: '/cookies' },
    { text: 'Accessibility', url: '/accessibility' }
  ];
}
