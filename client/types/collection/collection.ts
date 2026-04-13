import { CollectionVisibility } from "../enums";
import { CollectionNote } from "../notes/collectionNote";
import { UserPassage } from "../passages/userPassage";

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
    passages: UserPassage[];
    notes: CollectionNote[];
}