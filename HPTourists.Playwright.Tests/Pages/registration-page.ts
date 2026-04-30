import { type Page, type Locator } from '@playwright/test';
 
export class RegisterPage {
  readonly page: Page;
 
  readonly firstNameInput: Locator;
  readonly lastNameInput: Locator;
  readonly dobInput: Locator;
  readonly genderSelect: Locator;
  readonly ehicInput: Locator;
  readonly ehicExpiryInput: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly passwordConfirmInput: Locator;
  readonly submitButton: Locator;
 
 
  constructor(page: Page) {
    this.page = page;
 
    this.firstNameInput     = page.locator('#first-name');
    this.lastNameInput      = page.locator('#last-name');
    this.dobInput           = page.locator('#dob');
    this.genderSelect       = page.locator('#gender');
    this.ehicInput          = page.locator('#ehic');
    this.ehicExpiryInput    = page.locator('#ehic-expiry');
    this.emailInput         = page.locator('#email');
    this.passwordInput      = page.locator('#password');
    this.passwordConfirmInput = page.locator('#password-confirm');
    this.submitButton       = page.getByRole('button', { name: /register/i });
 
  }
 
  async goto() {
    await this.page.goto('/register');
  }
 
}