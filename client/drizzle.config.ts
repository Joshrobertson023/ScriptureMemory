import { defineConfig } from 'drizzle-kit';

export default defineConfig({
    out: './app/drizzle',
    schema: './app/database/schema.ts',
    dialect: 'sqlite',
    driver: 'expo'
});
