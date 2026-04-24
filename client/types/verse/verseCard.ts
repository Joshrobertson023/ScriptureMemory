import { initialReference, initialUserPassage } from "../../app/stores/collections.store";
import { Collection } from "../collection/collection";
import { Note } from "../note";
import { UserPassage } from "../passages/userPassage";
import { Verse } from "./verse"
import { Passage } from "../passages/passage";

export interface CrossReferenceGroup {
    fromVerse: Verse;
    crossReferences: Passage[];
}

export type VerseCardResponse = {
    totalSaved: number;
    totalMemorized: number;
    numPracticed: number;
    nextDue: Date;
    crossReferences: CrossReferenceGroup[];
    similar: Verse[];
}

export const initialVerseCardResponse: VerseCardResponse = {
    totalSaved: 0,
    totalMemorized: 0,
    numPracticed: 0,
    nextDue: new Date(),
    crossReferences: [],
    similar: []
}