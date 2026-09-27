export interface Bible {
    id: string;
    abbreviation: string;
    name: string;
    nameLocal: string | null;
    copyright: string | null;
    info: string;
    active: boolean;
}
