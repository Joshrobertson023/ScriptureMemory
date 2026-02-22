import { BibleVersion, CollectionsSort, ThemePreference } from "../enums"

export interface UserSettings {
    themePreference: ThemePreference;
    bibleVersion: BibleVersion;
    collectionsSort: CollectionsSort;
    subscribedVerseOfDay: boolean;
    pushNotificationsEnabled: boolean;
    notifyMemorizedVerse: boolean;
    notifyPublishedCollection: boolean;
    notifyCollecitonSaved: boolean;
    notifyNoteLiked: boolean;
    friendsActivityNotificationsEnabled: boolean;
    streakRemindersEnabled: boolean;
    appBadgesEnabled: boolean;
    practiceTabBadgesEnabled: boolean;
    typeOutReference: boolean;
}