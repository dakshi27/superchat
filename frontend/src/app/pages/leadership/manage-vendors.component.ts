// src/app/pages/leadership/manage-vendors.component.ts
/*
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LeadershipService } from '../../services/leadership.service';
import { Vendor } from '../../models';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <div class="p-6 bg-white rounded-lg shadow">
        <h2 class="text-xl font-semibold">Invite New Vendor</h2>
        <form #vendorForm="ngForm" (ngSubmit)="createVendor(vendorForm.value)" class="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
          <input name="companyName" ngModel required placeholder="Company Name" class="px-3 py-2 border rounded">
          <input name="contactEmail" ngModel required type="email" placeholder="Contact Email" class="px-3 py-2 border rounded">
          <input name="country" ngModel required placeholder="Country" class="px-3 py-2 border rounded">
          <button type="submit" [disabled]="vendorForm.invalid" class="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700 disabled:bg-gray-400 col-span-1 md:col-span-3">Send Invitation</button>
        </form>
      </div>

      <div class="p-6 bg-white rounded-lg shadow">
        <h2 class="text-xl font-semibold">Manage Vendors</h2>
        <input [(ngModel)]="countryFilter" (ngModelChange)="loadVendors()" placeholder="Filter by Country..." class="w-full md:w-1/3 my-4 px-3 py-2 border rounded">
        <ul class="mt-4 space-y-2">
          <li *ngFor="let vendor of vendors" class="flex justify-between items-center p-3 bg-gray-50 rounded">
            <div>
              <p class="font-semibold">{{ vendor.companyName }} ({{vendor.country}})</p>
              <p class="text-sm text-gray-600">{{ vendor.contactEmail }} - <span class="font-medium">{{ vendor.status }}</span></p>
            </div>
            <button (click)="deleteVendor(vendor)" class="px-3 py-1 text-sm text-white bg-red-500 rounded hover:bg-red-600">Delete</button>
          </li>
        </ul>
      </div>
    </div>
  `
})
export class ManageVendorsComponent implements OnInit {
  vendors: Vendor[] = [];
  countryFilter: string = 'USA'; // Default filter

  constructor(private leadershipService: LeadershipService) {}

  ngOnInit() { this.loadVendors(); }

  loadVendors() {
    if (!this.countryFilter) return;
    this.leadershipService.getVendorsByCountry(this.countryFilter).subscribe(data => this.vendors = data);
  }

  createVendor(data: any) {
    this.leadershipService.createVendor(data).subscribe(() => this.loadVendors());
  }

  deleteVendor(vendor: Vendor) {
    if (confirm(`Are you sure you want to delete ${vendor.companyName}?`)) {
      this.leadershipService.deleteVendor(vendor.publicId).subscribe(() => this.loadVendors());
    }
  }
}
  */

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { LeadershipService } from '../../services/leadership.service';
import { Vendor } from '../../models';
import { COUNTRIES } from '../../constants/countries';
import { Observable, startWith, map } from 'rxjs';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
 
@Component({
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule
  ],
  template: `
    <div class="space-y-6">
      <div class="p-6 bg-white rounded-lg shadow">
        <h2 class="text-xl font-semibold">Invite New Vendor</h2>
        <form #vendorForm="ngForm" (ngSubmit)="createVendor(vendorForm.value)" class="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
          <input name="companyName" ngModel required placeholder="Company Name" class="px-3 py-2 border rounded">
          <input name="contactEmail" ngModel required type="email" placeholder="Contact Email" class="px-3 py-2 border rounded">
 
          <mat-form-field appearance="outline" class="w-full">
            <input
              type="text"
              matInput
              [formControl]="countryControl"
              [matAutocomplete]="auto"
              name="country"
              ngModel
              required
              placeholder="Select or type a country"
              class="px-3 py-2 border rounded"
            >
            <mat-autocomplete #auto="matAutocomplete">
              <mat-option *ngFor="let country of filteredCountries | async" [value]="country">
                {{ country }}
              </mat-option>
            </mat-autocomplete>
          </mat-form-field>
 
          <button type="submit" [disabled]="vendorForm.invalid" class="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700 disabled:bg-gray-400 col-span-1 md:col-span-3">Send Invitation</button>
        </form>
      </div>
 
      <div class="p-6 bg-white rounded-lg shadow">
        <h2 class="text-xl font-semibold">Manage Vendors</h2>
        <input [(ngModel)]="countryFilter" (ngModelChange)="loadVendors()" placeholder="Filter by Country..." class="w-full md:w-1/3 my-4 px-3 py-2 border rounded">
        <ul class="mt-4 space-y-2">
          <li *ngFor="let vendor of vendors" class="flex justify-between items-center p-3 bg-gray-50 rounded">
            <div>
              <p class="font-semibold">{{ vendor.companyName }} ({{vendor.country}})</p>
              <p class="text-sm text-gray-600">{{ vendor.contactEmail }} - <span class="font-medium">{{ vendor.status }}</span></p>
            </div>
            <button (click)="deleteVendor(vendor)" class="px-3 py-1 text-sm text-white bg-red-500 rounded hover:bg-red-600">Delete</button>
          </li>
        </ul>
      </div>
    </div>
  `
})
export class ManageVendorsComponent implements OnInit {
  vendors: Vendor[] = [];
  countryFilter: string = 'USA'; // Default filter
  countries = COUNTRIES;
  countryControl = new FormControl('');
  filteredCountries!: Observable<string[]>;
 
  constructor(private leadershipService: LeadershipService) {}
 
  ngOnInit() {
    this.loadVendors();
    this.filteredCountries = this.countryControl.valueChanges.pipe(
      startWith(''),
      map(value => this.filterCountries(value || ''))
    );
  }
 
  private filterCountries(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.countries.filter(country =>
      country.toLowerCase().includes(filterValue)
    );
  }
 
  loadVendors() {
    if (!this.countryFilter) return;
    this.leadershipService.getVendorsByCountry(this.countryFilter).subscribe(data => this.vendors = data);
  }
 
  createVendor(data: any) {
    this.leadershipService.createVendor(data).subscribe(() => this.loadVendors());
  }
 
  deleteVendor(vendor: Vendor) {
    if (confirm(`Are you sure you want to delete ${vendor.companyName}?`)) {
      this.leadershipService.deleteVendor(vendor.publicId).subscribe(() => this.loadVendors());
    }
  }
}
 
 