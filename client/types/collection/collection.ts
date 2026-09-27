import { CollectionItem } from "./collectionItem";

export interface Collection {
    id: string;
    userId: string;
    title: string;
    visibility: string;
    dateCreated: Date;
    orderPosition: number;
    isFavorites: boolean;
    isUncategorized: boolean;
    isArchived: boolean;
    description: string;
    progressPercent: number;
    items: CollectionItem[];
    passageCount?: number;
}