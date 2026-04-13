import { Verse } from "./verse";

export interface Vod {
    id?: number;
    reference: string;
    verses: Verse[];
    adminId?: number;
    orderPosition?: number;
    date?: Date;
    mostMemorized: number;
    mostSaved: number;
}