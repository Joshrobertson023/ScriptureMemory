import { Reference } from "../verse/reference";
import { Verse } from "../verse/verse";

export interface Passage {
    reference: Reference;
    verses: Verse[];
}