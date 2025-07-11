import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule, 
    RouterLink,
    MatButtonModule, 
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  @Output() closeSidebar = new EventEmitter<void>();
  onOverlayClick() {
    this.closeSidebar.emit();
  }

  @Input() isOpen = false;
  @Output() logout = new EventEmitter<void>();
}