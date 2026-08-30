import type { ExpoSQLiteDatabase } from 'drizzle-orm/expo-sqlite';
import * as schema from '../schema';
import {DB} from "../client";

export abstract class BaseRepository {
    constructor(protected db: DB) {}
}