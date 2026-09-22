CREATE TABLE `collections` (
	`id` text PRIMARY KEY,
	`userId` text,
	`title` text DEFAULT '' NOT NULL,
	`status` text DEFAULT 'active' NOT NULL,
	`visibility` text DEFAULT 'Private' NOT NULL,
	`dateCreated` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	`orderPosition` integer DEFAULT 0 NOT NULL,
	`isFavorites` integer DEFAULT false NOT NULL,
	`isUncategorized` integer DEFAULT false NOT NULL,
	`description` text DEFAULT '' NOT NULL,
	`progressPercent` integer DEFAULT 0 NOT NULL,
	CONSTRAINT `fk_collections_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`)
);
--> statement-breakpoint
CREATE TABLE `highlights` (
	`id` text PRIMARY KEY,
	`verseId` text NOT NULL,
	`userId` text,
	`dateCreated` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	CONSTRAINT `fk_highlights_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`)
);
--> statement-breakpoint
CREATE TABLE `notes` (
	`id` text PRIMARY KEY,
	`collectionId` text NOT NULL,
	`userId` text,
	`text` text DEFAULT '' NOT NULL,
	`orderPosition` integer DEFAULT 0 NOT NULL,
	`dateAdded` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	CONSTRAINT `fk_notes_collectionId_collections_id_fk` FOREIGN KEY (`collectionId`) REFERENCES `collections`(`id`) ON DELETE CASCADE,
	CONSTRAINT `fk_notes_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`)
);
--> statement-breakpoint
CREATE TABLE `passages` (
	`id` text PRIMARY KEY,
	`reference` text NOT NULL,
	`verses` text DEFAULT '[]' NOT NULL,
	`userId` text,
	`collectionId` text,
	`orderPosition` integer DEFAULT 0 NOT NULL,
	`dateAdded` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	`progressPercentage` integer DEFAULT 0,
	`timesMemorized` integer DEFAULT 0,
	`dateLastPracticed` text,
	`dateDue` text,
	`notifyMemorized` integer DEFAULT true NOT NULL,
	CONSTRAINT `fk_passages_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`),
	CONSTRAINT `fk_passages_collectionId_collections_id_fk` FOREIGN KEY (`collectionId`) REFERENCES `collections`(`id`) ON DELETE CASCADE
);
--> statement-breakpoint
CREATE TABLE `user_preferences` (
	`userId` text PRIMARY KEY,
	`themePreference` text DEFAULT 'SystemDefault' NOT NULL,
	`preferredBibleVersion` text DEFAULT 'kjv',
	`subscribedVerseOfDay` integer DEFAULT true NOT NULL,
	`notifyFriendsMemorizedPassage` integer DEFAULT true NOT NULL,
	`notifyFriendsPublishedCollection` integer DEFAULT true NOT NULL,
	`notifyCollectionSaved` integer DEFAULT true NOT NULL,
	`notifyNoteLikedCommented` integer DEFAULT true NOT NULL,
	`friendsActivityNotificationsEnabled` integer DEFAULT true NOT NULL,
	`overdueRemindersEnabled` integer DEFAULT true NOT NULL,
	`typeOutReference` integer DEFAULT false NOT NULL,
	CONSTRAINT `fk_user_preferences_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`) ON DELETE CASCADE
);
--> statement-breakpoint
CREATE TABLE `users` (
	`userId` text PRIMARY KEY,
	`loginCount` integer DEFAULT 0 NOT NULL,
	`dateStarted` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	`dateRegistered` text,
	`username` text,
	`firstName` text,
	`lastName` text,
	`email` text,
	`profileDescription` text,
	`profileImagePath` text,
	`versesMemorized` integer DEFAULT 0 NOT NULL,
	`points` integer DEFAULT 0 NOT NULL
);
--> statement-breakpoint
CREATE TABLE `verses` (
	`id` text NOT NULL,
	`passageId` text NOT NULL,
	`version` text NOT NULL,
	`book` text NOT NULL,
	`chapter` integer NOT NULL,
	`verseNum` integer NOT NULL,
	`plainText` text,
	`contentUsx` text,
	CONSTRAINT `verses_pk` PRIMARY KEY(`passageId`, `id`),
	CONSTRAINT `fk_verses_passageId_passages_id_fk` FOREIGN KEY (`passageId`) REFERENCES `passages`(`id`) ON DELETE CASCADE
);
