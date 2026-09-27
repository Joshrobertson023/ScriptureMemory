import { Passage } from "./passage";

export interface UserPassage {
    passage: Passage;
    id?: string;
    userId?: string;
    collectionId?: string;
    orderPosition?: number;
    dateAdded?: Date;
    progressPercent?: number;
    timesMemorized?: number;
    lastPracticed?: Date;
    dueDate?: Date;
    notifyMemorized?: boolean;
}