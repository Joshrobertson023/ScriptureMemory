import { db } from '../client';
import { UserRepository } from './user.repository';

export const userRepository = new UserRepository(db);

export * as collectionsRepository from './collections.repository';
export * as userPreferencesRepository from './userPreferences.repository';
export * as highlightsRepository from './highlights.repository';
