import { createNavigationContainerRef, StackActions } from "@react-navigation/native";
import { RootStackParamList } from "../types/router";

export const navigationRef = createNavigationContainerRef<RootStackParamList>();

export function isCurrentCollectionRoute(collectionId: number): boolean {
    if (!navigationRef.isReady())
        return false;

    const route = navigationRef.getCurrentRoute();
    if (route?.name !== "collection")
        return false;

    const params = route.params as RootStackParamList["collection"] | undefined;
    return params?.id === collectionId;
}

export function pushCollectionRoute(collectionId: number) {
    if (!navigationRef.isReady())
        return;

    navigationRef.dispatch(StackActions.push("collection", { id: collectionId }));
}
