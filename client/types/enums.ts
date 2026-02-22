const Status = {
    Active: 0,
    Inactive: 1,
    Deleted: 2
} as const;
export type Status = typeof Status[keyof typeof Status]

const CollectionsSort = {
    Newest: 0,
    Title: 1,
    LastPracticed: 2,
    Completion: 3,
    Custom: 4
} as const;
export type CollectionsSort = typeof CollectionsSort[keyof typeof CollectionsSort]

const ThemePreference = {
    SystemDefault: 0,
    Light: 1,
    Dark: 2,
} as const;
export type ThemePreference = typeof ThemePreference[keyof typeof ThemePreference]

const BibleVersion = {
    Kjv: 0,
} as const;
export type BibleVersion = typeof BibleVersion[keyof typeof BibleVersion]

