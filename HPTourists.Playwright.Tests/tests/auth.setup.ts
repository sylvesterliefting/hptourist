import {test as setup, expect} from '@playwright/test'

const authFile = '.auth/user.json'

setup('authentication', async({page}) =>{
    await page.goto('/');
 await page.getByRole('link', { name: 'Login' }).click();
 await page.getByRole('textbox', { name: 'Email' }).fill(process.env.USEREMAIL ?? '');
 await page.getByRole('textbox', { name: 'Password' }).fill(process.env.PASSWORD ?? '');
 await page.getByRole('button', { name: 'Log in' }).click();
 await expect(
    page.getByText('You\'ve successfully logged in.')
  ).toBeVisible();

await page.context().storageState({path: authFile})
});
