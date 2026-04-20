import { StyleSheet, Text, TouchableHighlight, View } from "react-native";
import { Passage } from "../../../types/passages/passage";
import useGlobalStyles from "../../styles/gobalStyles";
import AddPassageContent from "./addPassageContent";
import React from "react";

interface AddPassageProps {
    passage: Passage;
    savedItemId: number | null;
    savePassage: (passage: Passage) => void;
    removePassage: (itemId: number) => void;
}

const useLocalStyles = () => StyleSheet.create({
    container: {
        display: 'flex', justifyContent: 'flex-start', alignItems: 'flex-start', padding: 10
    },
    button: {
        width: '100%', flex: 1
    }
})

const AddPassage = React.memo(({passage, savedItemId, savePassage, removePassage}: AddPassageProps) => {
    const globalStyles = useGlobalStyles();
    const styles = useLocalStyles();
    const passageSaved = savedItemId !== null;

    return (
        <View style={styles.container}>
            <AddPassageContent passage={passage} />

            {passageSaved ? (
                <TouchableHighlight onPress={() => {
                        if (savedItemId !== null) {
                            removePassage(savedItemId);
                        }
                    }} style={styles.button}>
                    <View style={globalStyles.outlineButtonSkinny}>
                        <Text style={globalStyles.outlineButtonSkinnyText}>Remove</Text>
                    </View>
                </TouchableHighlight>
            ) : (
                <TouchableHighlight onPress={() => {
                        savePassage(passage);
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