import { Text, TouchableHighlight, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import { useCollectionsStore } from "../../stores/collections.store";
import PassageContent from "./passageContent";
import { Check, Users } from "lucide-react-native";
import useAppTheme from "../../theme";
import React from "react";

interface AddPassageProps {
    passage: Passage;
}

const AddPassage = React.memo(({passage}: AddPassageProps) => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const {addPassageToNewCollection, removeItemFromNewCollection, newCollection} = useCollectionsStore();

    const savedPassageItem = newCollection.items.find(
        (i) => i.type === 'passage' && i.passage.reference.readableReference === passage.reference.readableReference
    );
    const passageSaved = !!savedPassageItem;

    return (
        <View style={{display: 'flex', justifyContent: 'flex-start', alignItems: 'flex-start', padding: 10}}>
            <PassageContent passage={passage} />

            

            <View style={{flexDirection: 'row', justifyContent: 'center', alignItems: 'center', marginVertical: 10}}>
                <View style={{flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 7}}>
                </View>
                <View style={{flexDirection: 'row', justifyContent: 'center', alignItems: 'center', gap: 7}}>
                    <Check size={16} color={theme.colors.onBackground} />
                    <Text style={styles.p4}>In 1 Collection</Text>
                </View>
            </View>

            {passageSaved ? (
                <TouchableHighlight onPress={() => {
                        if (savedPassageItem) {
                            removeItemFromNewCollection(savedPassageItem.id);
                        }
                    }} style={{width: '100%', flex: 1}}>
                    <View style={styles.outlineButtonSkinny}>
                        <Text style={styles.outlineButtonSkinnyText}>Remove</Text>
                    </View>
                </TouchableHighlight>
            ) : (
                <TouchableHighlight onPress={() => {
                        addPassageToNewCollection(passage);
                    }} style={{width: '100%', flex: 1}}>
                    <View style={styles.outlineButtonSkinny}>
                        <Text style={styles.outlineButtonSkinnyText}>Add</Text>
                    </View>
                </TouchableHighlight>
            )}
        </View>
    )
})

export default AddPassage;