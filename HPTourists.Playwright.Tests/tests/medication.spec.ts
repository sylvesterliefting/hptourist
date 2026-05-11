import { test, expect } from '@playwright/test'
import { MedicationPage} from '../Pages/medication-page';

test.beforeEach(async ({ page }) => {
  const medication = new MedicationPage(page);
  await medication.goto();
  await page.getByRole('link', { name: 'Nederlands' }).click();
  await page.waitForLoadState('networkidle');
});

test('open add new medication page', async ({ page }) => {
  const medication = new MedicationPage(page);
    await medication.addMedication.click()
    await expect(page.getByRole('heading', { name: 'Medicatie toevoegen' })).toBeVisible();
});

test('Validate mandatory fields', async ({ page }) => {
  const medication = new MedicationPage(page);
    await medication.addMedication.click()
    await expect(page.getByRole('heading', { name: 'Medicatie toevoegen' })).toBeVisible();

    await page.getByRole('button', { name: 'Opslaan' }).click();
    await expect(page.locator('div').filter({ hasText: /^Medicatienaam is verplicht\.$/ })).toBeVisible();
    await expect(page.locator('div').filter({ hasText: /^Werkzame stof is verplicht\.$/ })).toBeVisible();
    await expect(page.locator('div').filter({ hasText: /^Farmaceutische vorm is verplicht\.$/ })).toBeVisible();

});

test('Add new medication', async ({ page }) => {
  const medication = new MedicationPage(page);
    await medication.addMedication.click()
    await expect(page.getByRole('heading', { name: 'Medicatie toevoegen' })).toBeVisible();
    await page.getByRole('textbox', { name: 'Medicatienaam' }).fill('Paracetamol');
    await page.getByRole('textbox', { name: 'Werkzame stof' }).fill('Paracetamol');
    await page.getByRole('textbox', { name: 'ATC-code (optioneel)' }).fill('N02BE01');
    await page.getByLabel('Farmaceutische vorm').selectOption('Tablet');
    await page.getByRole('button', { name: 'Opslaan' }).click();

    const table = page.locator('table.table');
    const row = table.locator('tbody tr').filter({ hasText: 'Paracetamol' });
    await expect(row).toBeVisible();
    await expect(row.locator('td').filter({ has: page.locator('span.badge') })).toContainText('In behandeling');
    await row.getByRole('button', { name: 'Verwijderen' }).click();


});