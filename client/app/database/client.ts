import { drizzle } from 'drizzle-orm/expo-sqlite';
import { openDatabaseSync } from 'expo-sqlite';
import * as schema from './schema';

export const sqlite = openDatabaseSync('verseapp.db', { enableChangeListener: true });
sqlite.execSync('PRAGMA foreign_keys = ON;');
export const db = drizzle(sqlite);

export type DB = typeof db;