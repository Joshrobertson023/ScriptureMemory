CREATE TABLE `passage_notes` (
	`id` text PRIMARY KEY,
	`passageKey` text NOT NULL,
	`userId` text,
	`text` text DEFAULT '' NOT NULL,
	`dateAdded` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	CONSTRAINT `fk_passage_notes_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`)
);
