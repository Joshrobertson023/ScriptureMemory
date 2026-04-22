import { Passage } from "./passage";
import { Note } from "../note";

export interface UserPassage {
    passage: Passage;
    notes?: Note[];
    id?: number;
    userId?: number;
    collectionId?: number;
    orderPosition?: number;
    dateAdded?: Date;
    progressPercent?: number;
    timesMemorized?: number;
    lastPracticed?: Date;
    dueDate?: Date;
    notifyMemorized?: boolean;
}