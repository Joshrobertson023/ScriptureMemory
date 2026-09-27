import { and, asc, eq } from "drizzle-orm";
import * as Crypto from "expo-crypto";
import { Collection } from "../../../types/collection/collection";
import { CollectionItem } from "../../../types/collection/collectionItem";
import { Note } from "../../../types/note";
import { Passage } from "../../../types/passages/passage";
import { UserPassage } from "../../../types/passages/userPassage";
import { Reference } from "../../../types/verse/reference";
import { Verse } from "../../../types/verse/verse";
import { useUserStore } from "../../stores/user.store";
import { db } from "../client";
import { collectionsTable, notesTable, passagesTable } from "../schema";

type CollectionRow = typeof collectionsTable.$inferSelect;
type PassageRow = typeof passagesTable.$inferSelect;
type NoteRow = typeof notesTable.$inferSelect;

function getUserId(): string {
    return useUserStore.getState().userId;
}

export function activeCollectionsQuery() {
    return db.select().from(collectionsTable)
        .where(and(eq(collectionsTable.userId, getUserId()), eq(collectionsTable.status, "active")))
        .orderBy(asc(collectionsTable.orderPosition));
}

export function archivedCollectionsQuery() {
    return db.select().from(collectionsTable)
        .where(and(eq(collectionsTable.userId, getUserId()), eq(collectionsTable.status, "archived")))
        .orderBy(asc(collectionsTable.orderPosition));
}

export function allPassagesQuery() {
    return db.select().from(passagesTable)
        .where(eq(passagesTable.userId, getUserId()))
        .orderBy(asc(passagesTable.orderPosition));
}

export function allNotesQuery() {
    return db.select().from(notesTable)
        .where(eq(notesTable.userId, getUserId()))
        .orderBy(asc(notesTable.orderPosition));
}

export function collectionByIdQuery(id: string) {
    return db.select().from(collectionsTable).where(eq(collectionsTable.id, id));
}

export async function getCollectionsContainingVerses(verseIds: string[]): Promise<Collection[]> {
    if (verseIds.length === 0) return [];

    const userId = getUserId();
    const verseIdSet = new Set(verseIds);
    const [collectionRows, passageRows] = await Promise.all([
        db.select().from(collectionsTable).where(and(eq(collectionsTable.userId, userId), eq(collectionsTable.status, "active"))),
        db.select().from(passagesTable).where(eq(passagesTable.userId, userId)),
    ]);

    const matchingCollectionIds = new Set(
        passageRows
            .filter((r) => r.collectionId && (JSON.parse(r.verses) as Verse[]).some((v) => verseIdSet.has(v.id)))
            .map((r) => r.collectionId as string)
    );

    return collectionRows
        .filter((r) => matchingCollectionIds.has(r.id))
        .map((r) => ({ ...mapCollectionRow(r), items: [] }));
}

export function mapCollectionRow(row: CollectionRow): Omit<Collection, "items"> {
    return {
        id: row.id,
        userId: row.userId ?? getUserId(),
        title: row.title,
        visibility: row.visibility,
        dateCreated: new Date(row.dateCreated),
        orderPosition: row.orderPosition,
        isFavorites: row.isFavorites,
        isUncategorized: row.isUncategorized,
        isArchived: row.status === "archived",
        description: row.description,
        progressPercent: row.progressPercent,
    };
}

export function mapPassageRow(row: PassageRow): CollectionItem {
    const reference = JSON.parse(row.reference) as Reference;
    const verses = JSON.parse(row.verses) as Verse[];
    const passage: Passage = { reference, verses };
    const userPassage: UserPassage = {
        id: row.id,
        userId: row.userId ?? undefined,
        collectionId: row.collectionId ?? undefined,
        orderPosition: row.orderPosition,
        dateAdded: new Date(row.dateAdded),
        progressPercent: row.progressPercentage ?? 0,
        timesMemorized: row.timesMemorized ?? 0,
        lastPracticed: row.dateLastPracticed ? new Date(row.dateLastPracticed) : undefined,
        dueDate: row.dateDue ? new Date(row.dateDue) : undefined,
        notifyMemorized: row.notifyMemorized,
        passage,
    };
    return { type: "passage", id: row.id, passage: userPassage };
}

export function mapNoteRow(row: NoteRow): CollectionItem {
    const note: Note = { id: row.id, text: row.text };
    return { type: "note", id: row.id, note };
}

export function assembleItems(passageRows: PassageRow[], noteRows: NoteRow[], collectionId: string): CollectionItem[] {
    const items = [
        ...passageRows.filter((r) => r.collectionId === collectionId).map((r) => ({ item: mapPassageRow(r), orderPosition: r.orderPosition })),
        ...noteRows.filter((r) => r.collectionId === collectionId).map((r) => ({ item: mapNoteRow(r), orderPosition: r.orderPosition })),
    ];
    items.sort((a, b) => a.orderPosition - b.orderPosition);
    return items.map((i) => i.item);
}

async function nextOrderPosition(collectionId: string): Promise<number> {
    const [passageRows, noteRows] = await Promise.all([
        db.select().from(passagesTable).where(eq(passagesTable.collectionId, collectionId)),
        db.select().from(notesTable).where(eq(notesTable.collectionId, collectionId)),
    ]);
    return passageRows.length + noteRows.length;
}

export async function createDraftCollection(): Promise<string> {
    const id = Crypto.randomUUID();
    await db.insert(collectionsTable).values({
        id,
        userId: getUserId(),
        title: "",
        status: "draft",
        visibility: "Private",
    });
    return id;
}

export async function commitDraftCollection(id: string, title: string): Promise<void> {
    const finalTitle = title.trim() === "" ? "New Collection" : title.trim();
    await db.update(collectionsTable)
        .set({ status: "active", title: finalTitle })
        .where(eq(collectionsTable.id, id));
}

export async function updateCollectionMeta(id: string, meta: Partial<Pick<Collection, "title" | "visibility" | "description">>): Promise<void> {
    await db.update(collectionsTable).set(meta as Partial<typeof collectionsTable.$inferInsert>).where(eq(collectionsTable.id, id));
}

export async function deleteCollection(id: string): Promise<void> {
    await db.delete(passagesTable).where(eq(passagesTable.collectionId, id));
    await db.delete(notesTable).where(eq(notesTable.collectionId, id));
    await db.delete(collectionsTable).where(eq(collectionsTable.id, id));
}

export async function archiveCollection(id: string): Promise<void> {
    await db.update(collectionsTable).set({ status: "archived" }).where(eq(collectionsTable.id, id));
}

export async function unarchiveCollection(id: string): Promise<void> {
    await db.update(collectionsTable).set({ status: "active" }).where(eq(collectionsTable.id, id));
}

export async function reorderCollections(orderedIds: string[]): Promise<void> {
    await Promise.all(orderedIds.map((id, index) =>
        db.update(collectionsTable).set({ orderPosition: index }).where(eq(collectionsTable.id, id))
    ));
}

export async function addPassageToCollection(collectionId: string, passage: Passage): Promise<string | null> {
    console.log('saving passage', passage.reference.readableReference, 'to collection', collectionId);

    const [passageRows, noteRows] = await Promise.all([
        db.select().from(passagesTable).where(eq(passagesTable.collectionId, collectionId)),
        db.select().from(notesTable).where(eq(notesTable.collectionId, collectionId)),
    ]);
    const alreadyExists = passageRows.some((r) => {
        const ref = JSON.parse(r.reference) as Reference;
        console.log('checking existing passage', ref.readableReference, 'against new passage', passage.reference.readableReference);
        return ref.readableReference === passage.reference.readableReference;
    });
    console.log('already exists?', alreadyExists);
    if (alreadyExists) return null;

    const id = Crypto.randomUUID();
    await db.insert(passagesTable).values({
        id,
        collectionId,
        userId: getUserId(),
        reference: JSON.stringify(passage.reference),
        verses: JSON.stringify(passage.verses),
    });
    console.log('id of new passage row', id);
    return id;
}

export async function removePassageItem(collectionId: string, itemId: string): Promise<void> {
    await db.delete(passagesTable).where(and(eq(passagesTable.id, itemId), eq(passagesTable.collectionId, collectionId)));
}

export async function addNoteToCollection(collectionId: string, note: Omit<Note, "id">): Promise<string> {
    const id = Crypto.randomUUID();
    const orderPosition = await nextOrderPosition(collectionId);
    await db.insert(notesTable).values({
        id,
        collectionId,
        userId: getUserId(),
        text: note.text,
        orderPosition,
    });
    return id;
}

export async function updateNoteInCollection(collectionId: string, itemId: string, text: string): Promise<void> {
    await db.update(notesTable).set({ text }).where(and(eq(notesTable.id, itemId), eq(notesTable.collectionId, collectionId)));
}

export async function removeNoteFromCollection(collectionId: string, itemId: string): Promise<void> {
    await db.delete(notesTable).where(and(eq(notesTable.id, itemId), eq(notesTable.collectionId, collectionId)));
}

export async function removeItemFromCollection(collectionId: string, itemId: string): Promise<void> {
    await Promise.all([
        removePassageItem(collectionId, itemId),
        removeNoteFromCollection(collectionId, itemId),
    ]);
}

export async function reorderItems(collectionId: string, orderedItems: CollectionItem[]): Promise<void> {
    await Promise.all(orderedItems.map((item, index) =>
        item.type === "passage"
            ? db.update(passagesTable).set({ orderPosition: index }).where(eq(passagesTable.id, item.id))
            : db.update(notesTable).set({ orderPosition: index }).where(eq(notesTable.id, item.id))
    ));
}

export async function applyCollectionEdit(edited: Collection): Promise<void> {
    await updateCollectionMeta(edited.id, {
        title: edited.title,
        visibility: edited.visibility,
        description: edited.description,
    });
    await db.delete(passagesTable).where(eq(passagesTable.collectionId, edited.id));
    await db.delete(notesTable).where(eq(notesTable.collectionId, edited.id));

    for (let index = 0; index < edited.items.length; index++) {
        const item = edited.items[index];
        if (item.type === "passage") {
            await db.insert(passagesTable).values({
                id: Crypto.randomUUID(),
                collectionId: edited.id,
                userId: getUserId(),
                reference: JSON.stringify(item.passage.passage.reference),
                verses: JSON.stringify(item.passage.passage.verses),
                orderPosition: index,
            });
        } else {
            await db.insert(notesTable).values({
                id: Crypto.randomUUID(),
                collectionId: edited.id,
                userId: getUserId(),
                text: item.note.text,
                orderPosition: index,
            });
        }
    }
}
