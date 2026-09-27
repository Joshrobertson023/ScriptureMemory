import { createNavigationContainerRef, StackActions } from "@react-navigation/native";
import { UserPassage } from "../types/passages/userPassage";
import { RootStackParamList } from "../types/router";

export const navigationRef = createNavigationContainerRef<RootStackParamList>();

export function isCurrentCollectionRoute(collectionId: string): boolean {
    if (!navigationRef.isReady())
        return false;

    const route = navigationRef.getCurrentRoute();
    if (route?.name !== "collection")
        return false;

    const params = route.params as RootStackParamList["collection"] | undefined;
    return params?.id === collectionId;
}

export function pushCollectionRoute(collectionId: string) {
    if (!navigationRef.isReady())
        return;

    navigationRef.dispatch(StackActions.push("collection", { id: collectionId }));
}

export function pushEditCollectionRoute(collectionId: string) {
    if (!navigationRef.isReady())
        return;

    navigationRef.dispatch(StackActions.push("editCollection", { id: collectionId }));
}

export function pushPracticeSessionRoute(userPassage: UserPassage) {
    if (!navigationRef.isReady())
        return;

    navigationRef.dispatch(StackActions.push("practiceSession", { userPassage }));
}

export function pushReadRoute(book: string, chapter: number, highlightVerses?: number[]) {
    if (!navigationRef.isReady())
        return;

    navigationRef.dispatch(StackActions.push("read", { book, chapter, highlightVerses }));
}
