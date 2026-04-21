import { test, expect } from '@playwright/test';

  test('find headings by role', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'Hello, world!' })).toBeVisible();
  });
