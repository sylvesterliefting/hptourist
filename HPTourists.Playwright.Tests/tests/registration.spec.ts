import { test, expect } from '@playwright/test'
import { RegisterPage } from '../Pages/registration-page';

test.beforeEach(async ({ page }) => {
  const register = new RegisterPage(page);
  await register.goto();
  await page.getByRole('link', { name: 'English' }).click();
});

test('fill form', async ({ page }) => {
  const register = new RegisterPage(page);
  await register.firstNameInput.fill('Jane');
  await register.lastNameInput.fill('Doe');
  await register.dobInput.fill('1990-06-15');
  await register.genderSelect.selectOption('Female');
  await register.ehicInput.fill('abcdefghij1234567890');
  await register.ehicExpiryInput.fill('2027-12-31');
  await register.emailInput.fill(process.env.USEREMAIL ?? '');
  await register.passwordInput.fill(process.env.PASSWORD ?? '');
  await register.passwordConfirmInput.fill(process.env.PASSWORD ?? '');

  await register.submitButton.click();
  await expect(page.getByRole('alert')).toContainText("You've successfully registered — welcome!");


});

test('show required field validation message', async ({ page }) => {
  const register = new RegisterPage(page);
  await register.submitButton.click();

  await expect(register.validationSummary).toBeVisible();

  await expect(register.page.locator('div').filter({ hasText: /^The First name field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The Last name field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The Date of birth field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^Please select a gender\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The EHIC number field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The EHIC expiry date field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The Email field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The Password field is required\.$/ })).toBeVisible();
  await expect(register.page.locator('div').filter({ hasText: /^The Confirm password field is required\.$/ })).toBeVisible();


});

test('show error when passwords do not match', async ({ page }) => {
  const register = new RegisterPage(page);
  await register.passwordInput.fill('password123');
  await register.passwordConfirmInput.fill('password456');
  await register.submitButton.click();

  await expect(page).toHaveURL(/\/register/);
  await expect(
    page.locator('.text-danger').filter({ hasText: /password/i }).first()
  ).toBeVisible();
});

test('ehic field cant be more than 20 characters', async ({ page }) => {
  const register = new RegisterPage(page);
  await register.ehicInput.fill('A'.repeat(30));

  const actual = await register.ehicInput.inputValue();
  expect(actual.length).toBeLessThanOrEqual(20);
});

test('ehic date must be in the future', async ({ page }) => {
  const register = new RegisterPage(page);
  await register.ehicExpiryInput.fill('2020-01-01');

  await register.submitButton.click();

  await expect(
    page.locator('div').filter({ hasText: /^Date must be in the future\.$/ })).toBeVisible();
});