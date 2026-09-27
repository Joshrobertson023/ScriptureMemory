import AsyncStorage from "@react-native-async-storage/async-storage";
import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";
import { Collection } from "../../types/collection/collection";
import { CollectionItem } from "../../types/collection/collectionItem";
import { Note } from "../../types/note";
import { Passage } from "../../types/passages/passage";
import { UserPassage } from "../../types/passages/userPassage";
import { Reference } from "../../types/verse/reference";

// NOTE: This store is no longer wired into the app - collections now live in
// SQLite (see app/database/repositories/collections.repository.ts and
// app/hooks/useCollections.ts). Left in place, unused, as a reference for the
// local-id/reconcile pattern the DB-backed version replaced. Ids below were
// switched from number -> string only to keep this file compiling against
// the shared Collection/CollectionItem/UserPassage/Note types (which changed
// to string ids for UUIDv7) - no other logic here was touched.

const LOCAL_ID_PREFIX = -1;

export const initialCollection: Collection = {
    id: '',
    userId: 0,
    title: '',
    visibility: 'Private',
    dateCreated: new Date(),
    orderPosition: 0,
    isFavorites: false,
    isUncategorized: false,
    isArchived: false,
    description: '',
    progressPercent: 0,
    items: []
}

export const initialReference: Reference = {
    book: '',
    chapter: 0,
    verses: [],
    readableReference: ''
}

const initialPassage: Passage = {
    reference: initialReference,
    verses: []
}

export const initialUserPassage: UserPassage = {
    passage: initialPassage
}

interface CollectionsStore {
    userCollections: Collection[];
    archivedCollections: Collection[];
    newCollection: Collection;
    editingCollection: Collection;
    _localIdCounter: number;
    _localPassageIdCounter: number;

    setCollections: (c: Collection[]) => void;
    setCollection: (c: Collection) => void;
    deleteCollection: (id: string) => void;
    setCollectionItems: (cId: string, i: CollectionItem[]) => void;
    removeItemFromCollection: (cId: string, itemId: string) => void;

    addNoteToCollection: (cId: string, note: Note) => void;
    updateNoteInCollection: (cId: string, itemId: string, text: string) => void;
    removeNoteFromCollection: (cId: string, itemId: string) => void;

    setNewCollection: (nc: Collection) => void;
    clearNewCollection: () => void;
    setNewCollectionVisibility: (v: string) => void;
    setNewCollectionItems: (items: CollectionItem[]) => void;
    addPassageToNewCollection: (p: Passage) => void;
    addPassageToCollection: (cId: string, p: Passage) => void;
    addNoteToNewCollection: (note: Note) => void;
    updateNoteInNewCollection: (itemId: string, text: string) => void;
    removeItemFromNewCollection: (id: string) => void;
    addCollection: (c: Omit<Collection, 'id'>) => Collection;

    setEditingCollection: (c: Collection) => void;
    clearEditingCollection: () => void;

    addCollectionToArchived: (id: string) => void;
    removeCollectionFromArchived: (id: string) => void;
}

export const useCollectionsStore = create<CollectionsStore>()(
    persist(
        (set, get) => ({
            userCollections: [],
            archivedCollections: [],
            newCollection: initialCollection,
            editingCollection: initialCollection,
            _localIdCounter: 0,
            _localPassageIdCounter: 0,


            setCollections(c: Collection[]) {
                set({ userCollections: c })
            },
            setCollection(c: Collection) {
                set((state) => ({
                    userCollections: state.userCollections.map((_c) => (_c.id === c.id ? c : _c))
                }));
            },
            addCollection(partialCollection: Omit<Collection, 'id'>): Collection {
                const state = get();
                const nextCounter = state._localIdCounter + 1;
                const newCollection: Collection = {
                    ...partialCollection,
                    id: String(nextCounter * LOCAL_ID_PREFIX)
                };
                set((state) => ({
                    _localIdCounter: nextCounter,
                    userCollections: [...state.userCollections, newCollection]
                }));
                return newCollection;
            },
            setCollectionItems(cId: string, i: CollectionItem[]) {
                const state = get();
                const collectionExists = state.userCollections.some((col) => col.id === cId);
                if (!collectionExists)
                    return;
                set((state) => ({
                    userCollections: state.userCollections.map((col) =>
                    col.id === cId ? {...col, items: i } : col)
                }))
            },
            removeItemFromCollection(cId: string, itemId: string) {
                const state = get();
                const collection = state.userCollections.find((col) => col.id === cId);
                if (!collection)
                    return;

                set((currentState) => ({
                    userCollections: currentState.userCollections.map((col) =>
                        col.id === cId
                            ? { ...col, items: col.items.filter((item) => item.id !== itemId) }
                            : col
                    )
                }));
            },

            addNoteToCollection(cId: string, note: Note) {
                const nextCounter = get()._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'note',
                    id: String(nextCounter * LOCAL_ID_PREFIX),
                    note,
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    userCollections: state.userCollections.map((c) =>
                        c.id === cId ? { ...c, items: [...c.items, item] } : c
                    )
                }));
            },
            updateNoteInCollection(cId: string, itemId: string, text: string) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) =>
                        c.id !== cId ? c : {
                            ...c,
                            items: c.items.map((i) =>
                                i.type !== 'note' || i.id !== itemId ? i : { ...i, note: { ...i.note, text } }
                            )
                        }
                    )
                }));
            },
            removeNoteFromCollection(cId: string, itemId: string) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) =>
                        c.id !== cId ? c : {
                            ...c,
                            items: c.items.filter((i) => i.id !== itemId)
                        }
                    )
                }));
            },

            deleteCollection(id: string) {
                set((state) => ({
                    userCollections: state.userCollections.filter((c) => c.id !== id)
                }))
            },
            setNewCollection(nc: Collection) {
                set({ newCollection: nc })
            },
            clearNewCollection() {
                set({ newCollection: initialCollection })
            },
            /**
             * Adds a passage to newCollection with a unique local (negative) id
             * Ignores the passage if it already exists
             */
            setNewCollectionVisibility(v: string) {
                set((state) => ({
                    newCollection: {
                        ...state.newCollection,
                        visibility: v
                    } as Collection
                }));
            },
            setNewCollectionItems(items) {
                set((state) => ({
                    newCollection: {
                        ...state.newCollection,
                        items,
                    }
                }));
            },
            addPassageToNewCollection(p: Passage) {
                const state = get();
                const alreadyExists = state.newCollection.items.some(
                    (i) => i.type === 'passage' && i.passage.passage.reference.readableReference === p.reference.readableReference
                );
                if (alreadyExists)
                    return;

                const nextCounter = state._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'passage',
                    id: String(nextCounter * LOCAL_ID_PREFIX),
                    passage: {
                        id: String(nextCounter * LOCAL_ID_PREFIX),
                        passage: p,
                    },
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    newCollection: {
                        ...state.newCollection,
                        items: [...state.newCollection.items, item],
                    }
                }));
            },
            addPassageToCollection(cId: string, p: Passage) {
                const state = get();
                const collection = state.userCollections.find((col) => col.id === cId);
                if (!collection) {
                    return;
                }

                const alreadyExists = collection.items.some(
                    (i) => i.type === 'passage' && i.passage.passage.reference.readableReference === p.reference.readableReference
                );
                if (alreadyExists) {
                    return;
                }

                const nextCounter = state._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'passage',
                    id: String(nextCounter * LOCAL_ID_PREFIX),
                    passage: {
                        id: String(nextCounter * LOCAL_ID_PREFIX),
                        collectionId: cId,
                        passage: p,
                    },
                };

                set((currentState) => ({
                    _localPassageIdCounter: nextCounter,
                    userCollections: currentState.userCollections.map((col) =>
                        col.id === cId ? { ...col, items: [...col.items, item] } : col
                    )
                }));
            },
            addNoteToNewCollection(note: Note) {
                const nextCounter = get()._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'note',
                    id: String(nextCounter * LOCAL_ID_PREFIX),
                    note,
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    newCollection: {
                        ...state.newCollection,
                        items: [...state.newCollection.items, item],
                    }
                }));
            },
            updateNoteInNewCollection(itemId: string, text: string) {
                set((state) => ({
                    newCollection: {
                        ...state.newCollection,
                        items: state.newCollection.items.map((item) => {
                            if (item.type !== 'note' || item.id !== itemId)
                                return item;
                            return {
                                ...item,
                                note: {
                                    ...item.note,
                                    text
                                }
                            };
                        }),
                    }
                }));
            },
            removeItemFromNewCollection(id: string) {
                set((state) => ({
                    newCollection: {
                        ...state.newCollection,
                        items: state.newCollection.items.filter((i) => i.id !== id),
                    }
                }));
            },

            setEditingCollection(c: Collection) {
                set((state) => ({
                    editingCollection: c
                }))
            },
            clearEditingCollection() {
                set((state) => ({
                    editingCollection: initialCollection
                }))
            },

            addCollectionToArchived(id: string) {
                const state = get();
                const collection = state.userCollections.find((col) => col.id === id);
                if (!collection) return;
                this.deleteCollection(id);

                set((state) => ({
                    archivedCollections: [
                        ...state.archivedCollections, collection
                    ]
                }))
            },
            removeCollectionFromArchived(id: string) {
                const state = get();
                const collection = state.archivedCollections.find((col) => col.id === id);
                if (!collection) return;

                set((state) => ({
                    userCollections: [...state.userCollections, collection],
                    archivedCollections: state.archivedCollections.filter((col) => col.id !== id)
                }))
            }
        }),
        {
            name: 'collection-storage-6',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)
