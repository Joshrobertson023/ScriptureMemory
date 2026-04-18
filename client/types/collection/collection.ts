import { CollectionVisibility } from "../enums";
import { CollectionItem } from "./collectionItem";

export interface Collection {
    id: number;
    userId: number;
    title: string;
    visibility: CollectionVisibility;
    dateCreated: Date;
    orderPosition: number;
    isFavorites: boolean;
    isUncategorized: boolean;
    isArchived: boolean;
    description: string;
    progressPercent: number;
    items: CollectionItem[];
}