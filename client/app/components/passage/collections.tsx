import { View, Text, FlatList } from "react-native";
import { Collection } from "../../../types/collection/collection";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import { CollectionCard } from "../collection/collectionCard";

interface CollectionsProps {
    collections: Collection[];
    onCollectionPress?: (collection: Collection) => void;
}

const Collections = ({collections, onCollectionPress}: CollectionsProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();

    return (
        <View style={{marginTop: 20}}>
            <Text style={globalStyles.p2}>
                In {collections.length} Collections
            </Text>

            <FlatList
                data={collections}
                renderItem={({item}) => <CollectionCard collection={item} onPress={onCollectionPress} />}
            />
        </View>
    )
}

export default Collections;