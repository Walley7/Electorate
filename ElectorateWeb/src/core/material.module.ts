import {NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  MatButtonModule, MatCardModule, MatDialogModule, MatInputModule, MatTableModule, MatRadioModule,
  MatToolbarModule, MatCheckboxModule, MatIconModule, MatSidenavModule, MatListModule, MatChipsModule,
  MatDividerModule, MatSelectModule, MatFormFieldModule, MatAutocompleteModule, MatSnackBarModule, MatMenuModule, MatExpansionModule
} from '@angular/material';

@NgModule({
  imports: [CommonModule, MatToolbarModule, MatButtonModule, MatCardModule, MatInputModule, MatExpansionModule,
    MatDialogModule, MatTableModule, MatCheckboxModule, MatIconModule, MatFormFieldModule, MatAutocompleteModule,
    MatSnackBarModule, MatMenuModule, MatChipsModule, MatRadioModule,
    MatSidenavModule, MatListModule, MatDividerModule, MatSelectModule],
  exports: [CommonModule, MatToolbarModule, MatButtonModule, MatCardModule, MatExpansionModule,
    MatInputModule, MatDialogModule, MatTableModule, MatFormFieldModule, MatAutocompleteModule,
    MatSnackBarModule, MatMenuModule, MatChipsModule, MatRadioModule,
    MatCheckboxModule, MatIconModule, MatSidenavModule, MatListModule, MatDividerModule, MatSelectModule],
})
export class CustomMaterialModule { }
