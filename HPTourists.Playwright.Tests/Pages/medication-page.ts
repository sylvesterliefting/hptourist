import { type Page, type Locator } from '@playwright/test';

export class MedicationPage {
  readonly page: Page;
  readonly addMedication: Locator;


  constructor(page: Page) {
    this.page = page;
    this.addMedication = this.page.getByRole('link', { name: 'Medicatie toevoegen' });


  }

  async goto() {
    await this.page.goto('/patient/medication');
  }

}