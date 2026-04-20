import { create } from "zustand";
import { Collection } from "../../types/collection/collection";
import { CollectionItem } from "../../types/collection/collectionItem";
import { createJSONStorage, persist } from "zustand/middleware";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { Passage } from "../../types/passages/passage";
import { UserPassage } from "../../types/passages/userPassage";
import { Reference } from "../../types/verse/reference";
import { Note } from "../../types/note";

const LOCAL_ID_PREFIX = -1;

export const initialCollection: Collection = {
    id: 0,
    userId: 0,
    title: '',
    visibility: 0,
    dateCreated: new Date(),
    orderPosition: 0,
    isFavorites: false,
    isUncategorized: false,
    isArchived: false,
    description: '',
    progressPercent: 0,
    items: []
}

const initialReference: Reference = {
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
    deleteCollection: (id: number) => void;
    setCollectionItems: (cId: number, i: CollectionItem[]) => void;
    removeItemFromCollection: (cId: number, itemId: number) => void;

    addNoteToCollection: (cId: number, note: Note) => void;
    updateNoteInCollection: (cId: number, itemId: number, text: string) => void;
    removeNoteFromCollection: (cId: number, itemId: number) => void;

    setNewCollection: (nc: Collection) => void;
    clearNewCollection: () => void;
    setNewCollectionVisibility: (v: number) => void;
    setNewCollectionItems: (items: CollectionItem[]) => void;
    addPassageToNewCollection: (p: Passage) => void;
    addNoteToNewCollection: (note: Note) => void;
    updateNoteInNewCollection: (itemId: number, text: string) => void;
    removeItemFromNewCollection: (id: number) => void;
    addCollection: (c: Omit<Collection, 'id'>) => Collection;

    reconcileServerId: (localId: number, serverId: number) => void;
    reconcilePassageServerId: (localId: number, serverId: number) => void;

    setEditingCollection: (c: Collection) => void;
    clearEditingCollection: () => void;

    addCollectionToArchived: (id: number) => void;
    removeCollectionFromArchived: (id: number) => void;
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
                    id: nextCounter * LOCAL_ID_PREFIX
                };
                set((state) => ({
                    _localIdCounter: nextCounter,
                    userCollections: [...state.userCollections, newCollection]
                }));
                return newCollection;
            },
            setCollectionItems(cId: number, i: CollectionItem[]) {
                const state = get();
                const collectionExists = state.userCollections.some((col) => col.id === cId);
                if (!collectionExists)
                    return;
                set((state) => ({
                    userCollections: state.userCollections.map((col) =>
                    col.id === cId ? {...col, items: i } : col)
                }))
            },
            removeItemFromCollection(cId: number, itemId: number) {
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

            addNoteToCollection(cId: number, note: Note) {
                const nextCounter = get()._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'note',
                    id: nextCounter * LOCAL_ID_PREFIX,
                    note,
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    userCollections: state.userCollections.map((c) =>
                        c.id === cId ? { ...c, items: [...c.items, item] } : c
                    )
                }));
            },
            updateNoteInCollection(cId: number, itemId: number, text: string) {
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
            removeNoteFromCollection(cId: number, itemId: number) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) =>
                        c.id !== cId ? c : {
                            ...c,
                            items: c.items.filter((i) => i.id !== itemId)
                        }
                    )
                }));
            },

            deleteCollection(id: number) {
                set((state) => ({
                    userCollections: state.userCollections.filter((c) => c.id !== id)
                }))
            },
            reconcileServerId(localId: number, serverId: number) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) =>
                        c.id === localId ? { ...c, id: serverId } : c)
                }));
            },
            /**
             * Called after a successful server sync, replaces temporary local passage id with server-assigned id
             * Updates the passage both in userCollections and newCollection
             */
            reconcilePassageServerId(localId: number, serverId: number) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) => ({
                        ...c,
                        items: c.items.map((i) =>
                            i.id === localId ? { ...i, id: serverId } : i)
                    })),
                    newCollection: {
                        ...state.newCollection,
                        items: state.newCollection.items.map((i) =>
                            i.id === localId ? { ...i, id: serverId } : i)
                    }
                }));
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
            setNewCollectionVisibility(v: number) {
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
                    (i) => i.type === 'passage' && i.passage.reference.readableReference === p.reference.readableReference
                );
                if (alreadyExists)
                    return;

                const nextCounter = state._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'passage',
                    id: nextCounter * LOCAL_ID_PREFIX,
                    passage: p,
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    newCollection: {
                        ...state.newCollection,
                        items: [...state.newCollection.items, item],
                    }
                }));
            },
            addNoteToNewCollection(note: Note) {
                const nextCounter = get()._localPassageIdCounter + 1;
                const item: CollectionItem = {
                    type: 'note',
                    id: nextCounter * LOCAL_ID_PREFIX,
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
            updateNoteInNewCollection(itemId: number, text: string) {
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
            removeItemFromNewCollection(id: number) {
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

            addCollectionToArchived(id: number) {
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
            removeCollectionFromArchived(id: number) {
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
            name: 'collection-storage-5',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)