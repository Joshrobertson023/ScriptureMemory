import { StyleSheet, Text, TouchableHighlight, View } from "react-native";
import { useCollectionsStore } from "../../stores/collections.store"
import { Collection } from "../../../types/collection/collection";
import { useMemo } from "react";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";

interface CollecitonCardProps {
    collection: Collection;
}

export const CollectionCard = ({collection}: CollecitonCardProps) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        highlight: {
            marginTop: 10,
            borderRadius: 10
        }
    }), [theme])
    const styles = useLocalStyles();

    const totalPassages = (collection.items.map((item) => item.type === 'passage')).reduce((prev, next) => prev + 1, 0);
    console.log(totalPassages);

    return (
        <TouchableHighlight style={styles.highlight} onPress={() => {

        }}>
            <View style={globalStyles.collectionCard}>
                <Text style={globalStyles.collectionCardTitle}>{collection.title}</Text>
            </View>
        </TouchableHighlight>
    )
}