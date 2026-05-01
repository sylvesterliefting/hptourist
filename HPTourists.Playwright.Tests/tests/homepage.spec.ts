import { test, expect } from '@playwright/test';

  test('change language homepage', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('link', { name: 'English' }).click();
    await expect(page.getByRole('heading', { name: 'Welcome to Huisartsenpraktijk Tourist Doctor Amsterdam!' })).toBeVisible();
    await page.getByRole('link', { name: 'Nederlands' }).click();
    await expect(page.getByRole('heading', { name: 'Welkom bij Huisartsenpraktijk Tourist Doctor Amsterdam!' })).toBeVisible();
    await page.getByRole('link', { name: 'polski' }).click();
    await expect(page.getByRole('heading', { name: 'Witamy w Huisartsenpraktijk Tourist Doctor Amsterdam!' })).toBeVisible();
  });
