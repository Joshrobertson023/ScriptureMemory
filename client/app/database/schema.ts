import { sql } from "drizzle-orm";
import { integer, primaryKey, sqliteTable, text } from "drizzle-orm/sqlite-core";

export const usersTable = sqliteTable("users", {
    userId: text().primaryKey(),
    loginCount: integer().notNull().default(0),
    dateStarted: text().default(sql`(CURRENT_TIMESTAMP)`).notNull(),
    dateRegistered: text(),
    username: text(),
    firstName: text(),
    lastName: text(),
    email: text(),
    profileDescription: text(),
    profileImagePath: text(),
    versesMemorized: integer().notNull().default(0),
    points: integer().notNull().default(0)
});

export const userPreferencesTable = sqliteTable("user_preferences", {
    userId: text().primaryKey().references(() => usersTable.userId, { onDelete: "cascade" }),
    themePreference: text({ enum: ["SystemDefault", "Light", "Dark"] }).notNull().default("SystemDefault"),
    preferredBibleVersion: text().default("kjv"),
    subscribedVerseOfDay: integer({ mode: "boolean" }).notNull().default(true),
    notifyFriendsMemorizedPassage: integer({ mode: "boolean" }).notNull().default(true),
    notifyFriendsPublishedCollection: integer({ mode: "boolean" }).notNull().default(true),
    notifyCollectionSaved: integer({ mode: "boolean" }).notNull().default(true),
    notifyNoteLikedCommented: integer({ mode: "boolean" }).notNull().default(true),
    friendsActivityNotificationsEnabled: integer({ mode: "boolean" }).notNull().default(true),
    overdueRemindersEnabled: integer({ mode: "boolean" }).notNull().default(true),
    typeOutReference: integer({ mode: "boolean" }).notNull().default(false),
});

export const collectionsTable = sqliteTable("collections", {
    id: text().primaryKey(),
    userId: text().references(() => usersTable.userId),
    title: text().notNull().default(''),
    status: text({ enum: ["draft", "active", "archived"] }).notNull().default("active"),
    visibility: text({ enum: ["Private", "Friends", "Public"] }).notNull().default("Private"),
    dateCreated: text().default(sql`(CURRENT_TIMESTAMP)`).notNull(),
    orderPosition: integer().notNull().default(0),
    isFavorites: integer({ mode: "boolean" }).notNull().default(false),
    isUncategorized: integer({ mode: "boolean" }).notNull().default(false),
    description: text().notNull().default(''),
    progressPercent: integer().notNull().default(0),
})

export const passagesTable = sqliteTable("passages", {
    id: text().primaryKey(),
    reference: text().notNull(),
    verses: text().notNull().default('[]'),
    userId: text().references(() => usersTable.userId),
    collectionId: text().references(() => collectionsTable.id, { onDelete: "cascade" }),
    orderPosition: integer().notNull().default(0),
    dateAdded: text().default(sql`(CURRENT_TIMESTAMP)`).notNull(),
    progressPercentage: integer().default(0),
    timesMemorized: integer().default(0),
    dateLastPracticed: text(),
    dateDue: text(),
    notifyMemorized: integer({ mode: "boolean" }).notNull().default(true),
})

export const versesTable = sqliteTable("verses", {
    id: text().notNull(),
    passageId: text().notNull().references(() => passagesTable.id, { onDelete: "cascade" }),
    version: text().notNull(),
    book: text().notNull(),
    chapter: integer().notNull(),
    verseNum: integer().notNull(),
    plainText: text(),
    contentUsx: text()
}, (table) => [
    primaryKey({ columns: [table.passageId, table.id] })
])

export const notesTable = sqliteTable("notes", {
    id: text().primaryKey(),
    collectionId: text().notNull().references(() => collectionsTable.id, { onDelete: "cascade" }),
    userId: text().references(() => usersTable.userId),
    text: text().notNull().default(''),
    orderPosition: integer().notNull().default(0),
    dateAdded: text().default(sql`(CURRENT_TIMESTAMP)`).notNull(),
})

export const highlightsTable = sqliteTable("highlights", {
    id: text().primaryKey(),
    verseId: text().notNull(),
    userId: text().references(() => usersTable.userId),
    dateCreated: text().default(sql`(CURRENT_TIMESTAMP)`).notNull(),
})
