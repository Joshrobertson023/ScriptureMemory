import { create } from "zustand";
import { Collection } from "../../types/collection/collection";
import { createJSONStorage, persist } from "zustand/middleware";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { Passage } from "../../types/passages/passage";
import { UserPassage } from "../../types/passages/userPassage";
import { Reference } from "../../types/verse/reference";

const LOCAL_ID_PREFIX = -1;

const initialCollection: Collection = {
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
    passages: [],
    notes: []
}

const initialReference: Reference = {
    book: '',
    chapter: 0,
    verses: [],
    readableReference: ''
}

export const initialUserPassage: UserPassage = {
    id: 0,
    userId: 0,
    orderPosition: 0,
    collectionId: 0,
    dateAdded: new Date,
    progressPercent: 0,
    timesMemorized: 0,
    lastPracticed: new Date,
    dueDate: new Date,
    notifyMemorized: true,
    reference: initialReference,
    verses: []
}

interface CollectionsStore {
    userCollections: Collection[];
    newCollection: Collection;
    _localIdCounter: number;
    _localPassageIdCounter: number; // Local id counter for passages
    setCollections: (c: Collection[]) => void;
    setCollection: (c: Collection) => void;
    setNewCollection: (nc: Collection) => void;
    addPassageToNewCollection: (p: Passage) => void;
    removePassageFromNewCollection: (p: Passage) => void;
    addCollection: (c: Omit<Collection, 'id'>) => Collection;
    reconcileServerId: (localId: number, serverId: number) => void;
    reconcilePassageServerId: (localId: number, serverId: number) => void; // Replace local passage id with server id after first sync
}

export const useCollectionsStore = create<CollectionsStore>()(
    persist(
        (set, get) => ({
            userCollections: [],
            newCollection: initialCollection,
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
                        passages: c.passages.map((p) =>
                            p.id === localId ? { ...p, id: serverId } : p)
                    })),
                    newCollection: {
                        ...state.newCollection,
                        passages: state.newCollection.passages.map((p) =>
                            p.id === localId ? { ...p, id: serverId } : p)
                    }
                }));
            },
            setNewCollection(nc: Collection) {
                set({ newCollection: nc })
            },
            /**
             * Adds a passage to newCollection with a unique local (negative) id
             * Ignores the passage if it already exists
             */
            addPassageToNewCollection(p: Passage) {
                const state = get();
                if (state.newCollection.passages.some((existing) => existing.reference.readableReference === p.reference.readableReference)) 
                    return;

                const nextCounter = state._localPassageIdCounter + 1;
                const passageWithLocalId: UserPassage = {
                    ...p,
                    id: nextCounter * LOCAL_ID_PREFIX,
                    userId: 0,
                    orderPosition: 0,
                    collectionId: 0,
                    dateAdded: new Date,
                    progressPercent: 0,
                    timesMemorized: 0,
                    lastPracticed: new Date,
                    dueDate: new Date,
                    notifyMemorized: true,
                };
                set((state) => ({
                    _localPassageIdCounter: nextCounter,
                    newCollection: {
                        ...state.newCollection,
                        passages: [...state.newCollection.passages, passageWithLocalId]
                    }
                }));
            },
            /**
             * Removes a passage from newCollection by id
             */
            removePassageFromNewCollection(p: Passage) {
                set((state) => ({
                    newCollection: {
                        ...state.newCollection,
                        passages: state.newCollection.passages.filter((existing) => existing.reference.readableReference !== p.reference.readableReference)
                    }
                }));
            }
        }),
        {
            name: 'collection-storage',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)