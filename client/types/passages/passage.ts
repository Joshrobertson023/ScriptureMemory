import { Reference } from "../verse/reference";
import { Verse } from "../verse/verse";

export interface Passage {
    id: string;
    reference: Reference;
    verses: Verse[];
    passageSaved?: boolean;
}