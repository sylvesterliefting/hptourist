import { test, expect } from '@playwright/test'
import { MedicationPage} from '../Pages/medication-page';

test.beforeEach(async ({ page }) => {
  const medication = new MedicationPage(page);
  await medication.goto();
  await page.getByRole('link', { name: 'English' }).click();
});

test('add new medication', async ({ page }) => {
  const medication = new MedicationPage(page);
    await medication.addMedication.click()

});
