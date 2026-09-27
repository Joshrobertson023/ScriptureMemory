import React from "react";
import { Text, View } from "react-native";
import { Collection } from "../../../types/collection/collection";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { CollectionCard } from "../collection/collectionCard";

interface CollectionsProps {
    collections: Collection[];
    onCollectionPress?: (collection: Collection) => void;
}

const Collections = React.memo(({collections, onCollectionPress}: CollectionsProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();

    return (
        <View style={{marginTop: 20}}>
            <Text style={globalStyles.p2}>
                In {collections.length} Collections
            </Text>

            {collections.map((item) => (
                <CollectionCard key={item.id} collection={item} onPress={onCollectionPress} />
            ))}
        </View>
    )
})

export default Collections;