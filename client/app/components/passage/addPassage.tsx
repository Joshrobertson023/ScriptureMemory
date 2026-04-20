import { StyleSheet, Text, TouchableHighlight, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import { useCollectionsStore } from "../../stores/collections.store";
import AddPassageContent from "./addPassageContent";
import { Check, Users } from "lucide-react-native";
import useAppTheme from "../../theme";
import React from "react";

interface AddPassageProps {
    passage: Passage;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        display: 'flex', justifyContent: 'flex-start', alignItems: 'flex-start', padding: 10
    },
    button: {
        width: '100%', flex: 1
    }
})

const AddPassage = React.memo(({passage}: AddPassageProps) => {
    const globalStyles = useGlobalStyles();
    const styles = useLocalStyles();
    const theme = useAppTheme();
    const {addPassageToNewCollection, removeItemFromNewCollection, newCollection} = useCollectionsStore();

    const savedPassageItem = newCollection.items.find(
        (i) => i.type === 'passage' && i.passage.reference.readableReference === passage.reference.readableReference
    );
    const passageSaved = !!savedPassageItem;

    return (
        <View style={styles.container}>
            <AddPassageContent passage={passage} />

            {passageSaved ? (
                <TouchableHighlight onPress={() => {
                        if (savedPassageItem) {
                            removeItemFromNewCollection(savedPassageItem.id);
                        }
                    }} style={styles.button}>
                    <View style={globalStyles.outlineButtonSkinny}>
                        <Text style={globalStyles.outlineButtonSkinnyText}>Remove</Text>
                    </View>
                </TouchableHighlight>
            ) : (
                <TouchableHighlight onPress={() => {
                        addPassageToNewCollection(passage);
                    }} style={styles.button}>
                    <View style={globalStyles.outlineButtonSkinny}>
                        <Text style={globalStyles.outlineButtonSkinnyText}>Add</Text>
                    </View>
                </TouchableHighlight>
            )}
        </View>
    )
})

export default AddPassage;