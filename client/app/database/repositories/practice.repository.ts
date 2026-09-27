import { and, desc, eq, isNull, sql } from "drizzle-orm";
import * as Crypto from "expo-crypto";
import { UserPassage } from "../../../types/passages/userPassage";
import { useUserStore } from "../../stores/user.store";
import { db } from "../client";
import { passagesTable, practiceInfoTable } from "../schema";

function getUserId(): string {
    return useUserStore.getState().userId;
}

export function practiceListQuery() {
    return db.select({ practice: practiceInfoTable, passage: passagesTable })
        .from(practiceInfoTable)
        .innerJoin(passagesTable, eq(practiceInfoTable.passageId, passagesTable.id))
        .where(eq(practiceInfoTable.userId, getUserId()))
        .orderBy(desc(practiceInfoTable.datePracticed));
}

async function getOrCreatePassageId(userPassage: UserPassage): Promise<string> {
    if (userPassage.id) {
        const [existing] = await db.select({ id: passagesTable.id }).from(passagesTable).where(eq(passagesTable.id, userPassage.id));
        if (existing) return existing.id;
    }

    const { reference, verses } = userPassage.passage;
    const [uncollected] = await db.select({ id: passagesTable.id }).from(passagesTable)
        .where(and(
            eq(passagesTable.userId, getUserId()),
            isNull(passagesTable.collectionId),
            sql`json_extract(${passagesTable.reference}, '$.readableReference') = ${reference.readableReference}`
        ));
    if (uncollected) return uncollected.id;

    const id = Crypto.randomUUID();
    await db.insert(passagesTable).values({
        id,
        userId: getUserId(),
        reference: JSON.stringify(reference),
        verses: JSON.stringify(verses),
    });
    return id;
}

export async function completePractice(userPassage: UserPassage, dateOpened: string, stagePercents: number[]): Promise<void> {
    const passageId = await getOrCreatePassageId(userPassage);
    await db.insert(practiceInfoTable).values({
        id: Crypto.randomUUID(),
        passageId,
        userId: getUserId(),
        dateOpened,
        datePracticed: new Date().toISOString(),
        stage1Percent: stagePercents[0] ?? null,
        stage2Percent: stagePercents[1] ?? null,
        stage3Percent: stagePercents[2] ?? null,
        stage4Percent: stagePercents[3] ?? null,
    });
}
