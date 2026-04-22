import { Category } from "../category";
import { Reference } from "./reference";

export interface Verse {
    id: number;
    reference: Reference;
    readableReference?: string;
    votes?: number;
    text: string;
    usersSavedCount: number;
    usersMemorizedCount: number;
    verseNumbers: string;
    categories: Category[];
}