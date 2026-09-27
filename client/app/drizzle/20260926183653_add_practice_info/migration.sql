CREATE TABLE `practice_info` (
	`id` text PRIMARY KEY,
	`passageId` text NOT NULL,
	`userId` text,
	`dateOpened` text DEFAULT (CURRENT_TIMESTAMP) NOT NULL,
	`datePracticed` text,
	`stage1Percent` integer,
	`stage2Percent` integer,
	`stage3Percent` integer,
	`stage4Percent` integer,
	`nextDueDate` text,
	CONSTRAINT `fk_practice_info_passageId_passages_id_fk` FOREIGN KEY (`passageId`) REFERENCES `passages`(`id`) ON DELETE CASCADE,
	CONSTRAINT `fk_practice_info_userId_users_userId_fk` FOREIGN KEY (`userId`) REFERENCES `users`(`userId`)
);
