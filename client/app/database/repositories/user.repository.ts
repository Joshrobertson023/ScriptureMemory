import { eq } from 'drizzle-orm';
import { usersTable } from '../schema';
import { BaseRepository } from './base.repository';
import type { InferInsertModel } from 'drizzle-orm';

export class UserRepository extends BaseRepository {
    async findAll() {
        return this.db.select().from(usersTable);
    }
}