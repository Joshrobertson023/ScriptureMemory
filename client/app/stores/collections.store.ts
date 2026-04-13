import { create } from "zustand";
import { Collection } from "../../types/collection/collection";
import { createJSONStorage, persist } from "zustand/middleware";
import AsyncStorage from "@react-native-async-storage/async-storage";

const LOCAL_ID_PREFIX = -1; // Local ids are negative, server-confirmed ids are positive

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

interface CollectionsStore {
    userCollections: Collection[];
    newCollection: Collection;
    _localIdCounter: number; // Local id counter

    setCollections: (c: Collection[]) => void;
    setCollection: (c: Collection) => void;
    setNewCollection: (nc: Collection) => void;
    addCollection: (c: Omit<Collection, 'id'>) => Collection;
    reconcileServerId: (localId: number, serverId: number) => void; // Replace local id with server id after first sync
}

export const useCollectionsStore = create<CollectionsStore>()(
    persist(
        (set, get) => ({
            userCollections: [],
            newCollection: initialCollection,
            _localIdCounter: 0,

            setCollections(c: Collection[]) {
                set({userCollections: c})
            },

            setCollection(c: Collection) {
                set((state) => {
                    return {
                        userCollections: state.userCollections.map((_c) => (_c.id === c.id ? c : _c))
                    }
                });
            },

            /**
             * 
             * Adds a new collection to collections with a unique local (negative) id
             */
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

            /**
             * Called for each collection after a successful server sync, replaces temporary local id with server-assigned id
             * @param localId 
             * @param serverId 
             */
            reconcileServerId(localId: number, serverId: number) {
                set((state) => ({
                    userCollections: state.userCollections.map((c) => 
                        c.id === localId ? {...c, id: serverId } : c)
                }));
            },

            /**
             * 
             * Sets the "New Collection", a temporary collection state for the "Create Collection" screen
             */
            setNewCollection(nc: Collection) {
                set({newCollection: nc})
            }
        }),
        {
            name: 'collection-storage',
            storage: createJSONStorage(() => AsyncStorage)
        }
    )
)