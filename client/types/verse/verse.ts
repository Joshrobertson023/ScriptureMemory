import { Reference } from "./reference";

export interface Verse {
    id: number;
    reference: Reference;
    text: string;
    usersSavedCount: number;
    usersMemorizedCount: number;
    verseNumbers: string;
}