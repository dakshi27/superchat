import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VendorService, Vendor } from '../services/vendor.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-vendor-list',
  standalone: true,
  imports: [CommonModule, FormsModule,RouterModule],
  templateUrl: './vendor-list.component.html',
  styleUrls: ['./vendor-list.component.css'],
})
export class VendorListComponent implements OnInit {
  searchTerm: string = '';
  vendors: Vendor[] = [];
  loading = false;

  constructor(private vendorService: VendorService) {}

  ngOnInit(): void {
    // 🔹 Dummy data for testing UI
    this.vendors = [
      
      { id: 3, companyName: 'Tech World', contactEmail: 'contact@techworld.com', status: 'approved' },
     
    ];
  }

  loadVendors() {
    this.loading = true;
    this.vendorService.getVendors().subscribe({
      next: (data) => {
        this.vendors = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to fetch vendors', err);
        this.loading = false;
      },
    });
  }

  statusLabel(status: Vendor['status']) {
    switch (status) {
      case 'approved': return 'Approved';
      case 'pending': return 'Pending';
      case 'requesting': return 'Requesting';
      default: return 'Inactive';
    }
  }

  approveVendor(v: Vendor) {
    this.vendorService.approveVendor(v.id).subscribe({
      next: () => {
        v.status = 'approved'; // update UI
      },
      error: (err) => {
        console.error(`Failed to approve vendor ${v.id}`, err);
      },
    });
  }

  rejectVendor(v: Vendor) {
    const confirmDelete = window.confirm(
      `Are you sure you want to reject vendor "${v.companyName}"?`
    );
    if (!confirmDelete) return;

    this.vendorService.rejectVendor(v.id).subscribe({
      next: () => {
        this.vendors = this.vendors.filter(x => x.id !== v.id);
      },
      error: (err) => {
        console.error(`Failed to reject vendor ${v.id}`, err);
      },
    });
  }

   deleteVendor(vendor: Vendor) {
    this.vendorService.deleteVendor(vendor.id).subscribe({
      next: () => {
        this.vendors = this.vendors.filter(v => v.id !== vendor.id);
      },
      error: (err) => console.error('Error deleting vendor', err)
    });
  }

  get filteredVendors(): Vendor[] {
    if (!this.searchTerm.trim()) return this.vendors;
    const term = this.searchTerm.toLowerCase();
    return this.vendors.filter(v =>
      v.companyName.toLowerCase().includes(term) ||
      v.contactEmail.toLowerCase().includes(term)
    );
  }
}




