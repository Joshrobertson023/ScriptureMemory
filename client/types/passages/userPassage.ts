import { Passage } from "./passage";

export interface UserPassage extends Passage {
    id: number;
    userId: number;
    collectionId: number;
    orderPosition: number;
    dateAdded: Date;
    progressPercent: number;
    timesMemorized: number;
    lastPracticed: Date;
    dueDate: Date;
    notifyMemorized: boolean;
}